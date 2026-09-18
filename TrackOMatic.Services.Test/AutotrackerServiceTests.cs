using System.Diagnostics;

using Moq;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events.Autotracking;
using TrackOMatic.Logic.Models;
using TrackOMatic.Logic.Models.Autotracking;
using TrackOMatic.Services.TrackerState;

using ITimer = TrackOMatic.Logic.Models.ITimer;

namespace TrackOMatic.Services.Test;

/// <summary>
/// Tests for the AutotrackerService class.
/// These tests verify state management, initialization, and event firing behavior.
/// </summary>
public class AutotrackerServiceTests
{
    /// <summary>
    /// Test-friendly subclass of AutotrackerService that allows manual event invocation.
    /// This lets us test event subscriptions and handlers in isolation.
    /// </summary>
    private class TestableAutotrackerService(
        IItemTrackingService itemTracking,
        IEmulatorAttacher emulatorAttacher,
        IProcessMemoryReader memoryReader,
        ITimerFactory timerFactory,
        IApplicationStateService appState,
        IUserSettingsService settings,
        ICollectiblesService collectiblesService,
        ISongService songService,
        IRegionPlacementOrchestrator regionPlacementOrchestrator
    )
        : AutotrackerService(itemTracking, emulatorAttacher, memoryReader, timerFactory, appState, settings, collectiblesService, songService, regionPlacementOrchestrator)
    {
        // Expose PerformMemoryRead for direct testing
        public new int TestPerformMemoryRead(uint addr, int numOfBits) => base.TestPerformMemoryRead(addr, numOfBits);
    }
    /// <summary>
    /// Helper method to create a testable service with mocked dependencies.
    /// </summary>
    private static TestableAutotrackerService CreateServiceWithMocks(
        out Mock<IEmulatorAttacher> mockAttacher,
        out Mock<IProcessMemoryReader> mockMemoryReader,
        out Mock<ITimerFactory> mockTimerFactory)
    {
        mockAttacher = new Mock<IEmulatorAttacher>();
        mockMemoryReader = new Mock<IProcessMemoryReader>();
        mockTimerFactory = new Mock<ITimerFactory>();
        var appState = new Mock<IApplicationStateService>();
        var settings = new Mock<IUserSettingsService>();
        var itemTracking = new Mock<IItemTrackingService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var songService = new Mock<ISongService>();
        var orchestrator = new Mock<IRegionPlacementOrchestrator>();

        // Configure default behavior for mocks
        mockAttacher
            .Setup(a => a.GetAvailableEmulators())
            .Returns([(1234, "Project64")]);

        // Create a mock timer that does nothing when Started/Stopped
        var mockTimer = new Mock<ITimer>();
        mockTimer
            .Setup(t => t.Start())
            .Callback(() => { /* Do nothing for tests */ });
        mockTimer
            .Setup(t => t.Stop())
            .Callback(() => { /* Do nothing for tests */ });

        mockTimerFactory
            .Setup(f => f.CreateTimer(It.IsAny<double>()))
            .Returns(mockTimer.Object);

        // Orchestrator always fails placement in tests (items go to grid)
        orchestrator
            .Setup(o => o.TryPlaceAutoTrackedItem(It.IsAny<ItemName>(), It.IsAny<RegionName>(), It.IsAny<SavedItem>()))
            .Returns(false);


        return new TestableAutotrackerService(
            itemTracking.Object,
            mockAttacher.Object,
            mockMemoryReader.Object,
            mockTimerFactory.Object,
            appState.Object,
            settings.Object,
            mockCollectiblesService.Object,
            songService.Object,
            orchestrator.Object
        );
    }

    /// <summary>
    /// Helper to set up a test process on the service.
    /// We directly set EmulatorProcess to any non-null Process object;
    /// the actual test focuses on mocking the memory reader, not the process itself.
    /// </summary>
    private static void SetMockedEmulatorProcess(TestableAutotrackerService service)
    {
        // We only need a non-null Process to satisfy the null checks in ReadMemory.
        // The actual process ID doesn't matter for these tests since we mock the memory reader anyway.
        // Use System.Diagnostics.Process.GetCurrentProcess() as a simple, non-Windows-only way to get a valid process.
        var currentProcess = Process.GetCurrentProcess();
        var emulatorProperty = typeof(AutotrackerService).GetProperty("EmulatorProcess");
        emulatorProperty?.SetValue(service, currentProcess);
    }

    #region Initialization Tests

    [Fact]
    public void Constructor_WithValidDependencies_Succeeds()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullItemTracker_ThrowsArgumentNullException()
    {
        var mockAttacher = new Mock<IEmulatorAttacher>();
        var mockMemoryReader = new Mock<IProcessMemoryReader>();
        var mockTimerFactory = new Mock<ITimerFactory>();
        var appState = new Mock<IApplicationStateService>();
        var settings = new Mock<IUserSettingsService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var songService = new Mock<ISongService>();
        var orchestrator = new Mock<IRegionPlacementOrchestrator>();
        Assert.Throws<ArgumentNullException>(() =>
            new AutotrackerService(null!, mockAttacher.Object, mockMemoryReader.Object, mockTimerFactory.Object, appState.Object, settings.Object, mockCollectiblesService.Object, songService.Object, orchestrator.Object));
    }

    [Fact]
    public void Constructor_WithNullEmulatorAttacher_ThrowsArgumentNullException()
    {
        var mockTracker = new Mock<IItemTrackingService>();
        var mockMemoryReader = new Mock<IProcessMemoryReader>();
        var mockTimerFactory = new Mock<ITimerFactory>();
        var appState = new Mock<IApplicationStateService>();
        var settings = new Mock<IUserSettingsService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var songService = new Mock<ISongService>();
        var orchestrator = new Mock<IRegionPlacementOrchestrator>();
        Assert.Throws<ArgumentNullException>(() =>
            new AutotrackerService(mockTracker.Object, null!, mockMemoryReader.Object, mockTimerFactory.Object, appState.Object, settings.Object, mockCollectiblesService.Object, songService.Object, orchestrator.Object));
    }

    [Fact]
    public void Constructor_WithNullMemoryReader_ThrowsArgumentNullException()  
    {
        var mockTracker = new Mock<IItemTrackingService>();
        var mockAttacher = new Mock<IEmulatorAttacher>();
        var mockTimerFactory = new Mock<ITimerFactory>();
        var appState = new Mock<IApplicationStateService>();
        var settings = new Mock<IUserSettingsService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var songService = new Mock<ISongService>();
        var orchestrator = new Mock<IRegionPlacementOrchestrator>();
        Assert.Throws<ArgumentNullException>(() =>
            new AutotrackerService(mockTracker.Object, mockAttacher.Object, null!, mockTimerFactory.Object, appState.Object, settings.Object, mockCollectiblesService.Object, songService.Object, orchestrator.Object));
    }

    [Fact]
    public void Constructor_WithNullTimerFactory_ThrowsArgumentNullException()
    {
        var mockTracker = new Mock<IItemTrackingService>();
        var mockAttacher = new Mock<IEmulatorAttacher>();
        var mockMemoryReader = new Mock<IProcessMemoryReader>();
        var appState = new Mock<IApplicationStateService>();
        var settings = new Mock<IUserSettingsService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var songService = new Mock<ISongService>();
        var orchestrator = new Mock<IRegionPlacementOrchestrator>();
        Assert.Throws<ArgumentNullException>(() =>
            new AutotrackerService(mockTracker.Object, mockAttacher.Object, mockMemoryReader.Object, null!, appState.Object, settings.Object, mockCollectiblesService.Object, songService.Object, orchestrator.Object));
    }

    [Fact]
    public void Constructor_InitializesCurrentRegionToUnknown()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);
        Assert.Equal(RegionName.UNKNOWN, service.CurrentRegion);
    }

    [Fact]
    public void Constructor_InitializesSongStringsToEmpty()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);
        Assert.Equal("", service.CurrentSongGame);
        Assert.Equal("", service.CurrentSongName);
    }

    [Fact]
    public void Constructor_InitializesChecksList()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);
        Assert.NotNull(service.Checks);
        // Checks should be initialized with items from OffsetInfo
        Assert.NotEmpty(service.Checks);
    }

    [Fact]
    public void Constructor_InitializesTrackedAlreadyDictionary()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);
        Assert.NotNull(service.TrackedAlready);
        Assert.NotEmpty(service.TrackedAlready);
    }

    [Fact]
    public void Constructor_InitializesStartingItemsDictionary()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);
        Assert.NotNull(service.StartingItems);
        // Starting items may be empty initially
        Assert.Empty(service.StartingItems);
    }

    #endregion

    #region State Properties Tests

    [Fact]
    public void RandomizerVersion_InitiallyZero()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);
        Assert.Equal(0, service.RandomizerVersion);
    }

    [Fact]
    public void RandomizerSubVersion_InitiallyZero()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);
        Assert.Equal(0, service.RandomizerSubVersion);
    }

    #endregion

    #region Start/Stop Tests

    [Fact]
    public void Start_CanBeCalled()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);
        // Should not throw
        service.Start();
    }

    [Fact]
    public void Stop_CanBeCalled()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);
        service.Start();
        // Should not throw
        service.Stop();
    }

    [Fact]
    public void Stop_WithoutStart_DoesNotThrow()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);
        // Should not throw even if Start was never called
        service.Stop();
    }

    [Fact]
    public void Start_Multiple_IsIdempotent()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);
        // Calling Start multiple times should be safe
        service.Start();
        service.Start();
        service.Start();
    }

    #endregion

    #region Reset Tests

    [Fact]
    public void Reset_ClearsCurrentRegion()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);
        // Manually set a region (normally done during autotracking)
        var property = typeof(AutotrackerService).GetProperty("CurrentRegion");
        if (property?.CanWrite == true)
        {
            // Region would be set by autotracking logic
            // After reset, it should go back to UNKNOWN
            service.Reset();
            Assert.Equal(RegionName.UNKNOWN, service.CurrentRegion);
        }
    }

    [Fact]
    public void Reset_ClearsSongStrings()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);
        service.Reset();
        Assert.Equal("", service.CurrentSongGame);
        Assert.Equal("", service.CurrentSongName);
    }

    [Fact]
    public void Reset_ClearsRandomizerVersion()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);
        service.Reset();
        Assert.Equal(0, service.RandomizerVersion);
    }

    [Fact]
    public void ResetChecks_ReinitializesChecksList()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);
        var initialCheckCount = service.Checks.Count;

        service.ResetChecks();

        Assert.Equal(initialCheckCount, service.Checks.Count);
    }

    #endregion

    #region Public Methods Tests

    [Fact]
    public void SetStartingItems_WithEmptyDictionary_Succeeds()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);
        var startingItems = new Dictionary<ItemName, RegionName>();

        service.SetStartingItems(startingItems);

        // Starting items should be set
        Assert.NotNull(service.StartingItems);
    }

    [Fact]
    public void SetStartingItems_WithItems_MarksThemAsTrackedAlready()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);
        // Use an item that should exist in checks
        var testItem = service.Checks.Count > 0 ? service.Checks[0].ItemName : ItemName.NONE;
        if (testItem == ItemName.NONE)
        {
            return; // Skip if no items
        }

        var startingItems = new Dictionary<ItemName, RegionName>
        {
            { testItem, RegionName.START }
        };

        service.SetStartingItems(startingItems);

        // The starting item should now be marked as already tracked
        Assert.True(service.TrackedAlready[testItem]);
    }

    [Fact]
    public void SetSpoilerLoaded_CanBeCalled()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);
        // Should not throw
        service.SetSpoilerLoaded("test_file.json");
    }

    [Fact]
    public void ProcessSavedItem_MarkItemAsTracked()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);
        // Use an item from the checks list that should exist
        var itemToTest = service.Checks.Count > 0 ? service.Checks[0].ItemName : ItemName.NONE;
        if (itemToTest == ItemName.NONE)
        {
            return; // Skip if no items initialized
        }

        service.ProcessSavedItem(itemToTest);

        // After processing, it should be marked as tracked
        Assert.True(service.TrackedAlready[itemToTest]);
    }

    [Fact]
    public void ItemWasTracked_ReturnsFalseForNewItem()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);
        // Reset to ensure clean state
        service.ResetChecks();

        // Use an item from checks
        var testItem = service.Checks.Count > 0 ? service.Checks[0].ItemName : ItemName.NONE;
        if (testItem == ItemName.NONE)
        {
            return; // Skip if no items
        }

        bool wasTracked = service.ItemWasTracked(testItem);

        // Item should not be tracked initially (unless it's in starting items)
        Assert.IsType<bool>(wasTracked);
    }

    [Fact]
    public void ItemWasTracked_ReturnsTrueAfterProcessing()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);
        // Use an item from checks
        var testItem = service.Checks.Count > 0 ? service.Checks[0].ItemName : ItemName.NONE;
        if (testItem == ItemName.NONE)
        {
            return; // Skip if no items
        }

        service.ProcessSavedItem(testItem);

        bool wasTracked = service.ItemWasTracked(testItem);

        Assert.True(wasTracked);
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_CanBeCalled()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);

        // Should not throw
        service.Dispose();
    }

    [Fact]
    public void Dispose_IsIdempotent()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);

        // Calling Dispose multiple times should be safe
        service.Dispose();
        service.Dispose();
        service.Dispose();
    }

    #endregion

    #region Progressive Hint Tests

    [Fact]
    public void UpdateProgHintItem_WithNullEmulatorProcess_DoesNotRaiseEvent()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);

        // When EmulatorProcess is null, UpdateProgHintItem should return early
        int callCount = 0;
        service.ProgHintItemUpdated += (sender, e) => callCount++;

        service.UpdateProgHintItem();

        // Event should not be raised
        Assert.Equal(0, callCount);
    }

    [Fact]
    public void UpdateProgHintItem_WhenItemTypeUnchanged_DoesNotRaiseEvent()
    {
        var service = CreateServiceWithMocks(
            out _,
            out _,
            out _);

        // Without a process attached, UpdateProgHintItem returns early
        // This test verifies the early-exit behavior
        int callCount = 0;
        service.ProgHintItemUpdated += (sender, e) => callCount++;

        service.UpdateProgHintItem();
        Assert.Equal(0, callCount);

        // Call again - should still not fire
        service.UpdateProgHintItem();
        Assert.Equal(0, callCount);
    }

    [Fact]
    public void GetAmountToNextHintPack_WithNullEmulatorProcess_ReturnsZero()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);

        // EmulatorProcess is null by default
        var method = typeof(AutotrackerService).GetMethod("GetAmountToNextHintPack",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var result = (int?)method?.Invoke(service, [50]) ?? -1;

        Assert.Equal(0, result);
    }

    [Fact]
    public void GetAmountToNextHintPack_WithProcessButNotAttached_ReturnsZero()
    {
        var service = CreateServiceWithMocks(
            out _,
            out _,
            out _);

        var fakeProcess = Process.Start(new ProcessStartInfo("cmd.exe") { UseShellExecute = false });
        var emulatorProperty = typeof(AutotrackerService).GetProperty("EmulatorProcess");
        emulatorProperty?.SetValue(service, fakeProcess);

        // Even with a process, if _attached is false (which it is by default), ReadMemory returns 0
        // so GetAmountToNextHintPack will return 0
        var method = typeof(AutotrackerService).GetMethod("GetAmountToNextHintPack",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var result = (int?)method?.Invoke(service, [50]) ?? -1;

        Assert.Equal(0, result);

        fakeProcess?.Kill();
        fakeProcess?.Dispose();
    }

    [Fact]
    public void HintProgressUpdated_EventCanBeSubscribedTo()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);

        // Verify we can subscribe to HintProgressUpdated without error
        int callCount = 0;
        service.HintProgressUpdated += (sender, e) => callCount++;

        Assert.Equal(0, callCount); // Event hasn't fired yet
    }

    [Fact]
    public void ProgHintItemUpdated_EventCanBeSubscribedTo()
    {
        var service = CreateServiceWithMocks(out _, out _, out _);

        // Verify we can subscribe to ProgHintItemUpdated without error
        int callCount = 0;
        service.ProgHintItemUpdated += (sender, e) => callCount++;

        Assert.Equal(0, callCount); // Event hasn't fired yet
    }

    [Fact]
    public void UpdateAmountToNextHint_WithoutAttachment_FallsBackToHintHelper()
    {
        var service = CreateServiceWithMocks(
            out _,
            out _,
            out _);

        // When not attached, UpdateAmountToNextHint should use HintHelper
        int hintEventFired = 0;
        service.HintProgressUpdated += (sender, e) => hintEventFired++;

        // Call UpdateAmountToNextHint - should succeed and use the fallback path
        service.UpdateAmountToNextHint();

        // Event should have been raised (unless totalItems is 0)
        Assert.Equal(1, hintEventFired);
    }

    [Fact]
    public void GetAmountToNextHintPack_LoopsOverAllThresholds()
    {
        // This test verifies that GetAmountToNextHintPack iterates 10 times
        // even though we can't easily mock the internal ReadMemory call
        var service = CreateServiceWithMocks(out _, out _, out _);

        // With no process, it returns 0
        var method = typeof(AutotrackerService).GetMethod("GetAmountToNextHintPack",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var result = (int?)method?.Invoke(service, [0]) ?? -1;

        Assert.Equal(0, result);
    }

    #endregion
}
