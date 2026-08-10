using System.Diagnostics;
using System.Globalization;
using System.Timers;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events.Autotracking;
using TrackOMatic.Logic.Models;
using TrackOMatic.Logic.Models.Autotracking;
using TrackOMatic.Services.TrackerState;

using ITimer = TrackOMatic.Logic.Models.ITimer;

namespace TrackOMatic.Services;

/// <summary>
/// Event-driven autotracking service that monitors emulator memory in real-time.
/// Restores all original autotracker logic while using dependency injection and events.
/// </summary>
public class AutotrackerService : IAutotrackerService
{
    private readonly IItemTrackingService _itemTrackingService;
    private readonly IEmulatorAttacher _emulatorAttacher;
    private readonly IProcessMemoryReader _memoryReader;
    private readonly ITimerFactory _timerFactory;
    private readonly IApplicationStateService _appState;
    private readonly IUserSettingsService _settings;
    private readonly ICollectiblesService _collectiblesService;
    private readonly IRegionPlacementOrchestrator _regionPlacementOrchestrator;
    private bool _disposed;
    private ITimer? _pollingTimer;
    private ItemType _progHintItem;
    private IntPtr _processHandle;
    private int _processId;
    private const double POLLING_INTERVAL_MS = 1000;

    #region Event Implementations
    public event EventHandler<AutotrackerRegionEventArgs>? RegionLightingChanged;
    public event EventHandler<AutotrackerSongEventArgs>? SongChanged;
    public event EventHandler<AutotrackerHintEventArgs>? HintProgressUpdated;
    public event EventHandler<AutotrackerProgHintItemEventArgs>? ProgHintItemUpdated;
    #endregion

    #region State Properties

    public IReadOnlyList<AutotrackedCheck> Checks
        => _checks.AsReadOnly();

    public IReadOnlyDictionary<ItemName, bool> TrackedAlready
        => new Dictionary<ItemName, bool>(_trackedAlready);

    public IReadOnlyDictionary<ItemName, RegionName> StartingItems
        => new Dictionary<ItemName, RegionName>(_startingItems);

    public GameVerificationInfo? GameVerificationInfo { get; private set; }

    public RegionName CurrentRegion { get; private set; }

    public string CurrentSongGame { get; private set; }

    public string CurrentSongName { get; private set; }

    public int RandomizerVersion { get; private set; }

    public int RandomizerSubVersion { get; private set; }

    #endregion

    #region Internal State

    private List<AutotrackedCheck> _checks { get; set; }
    /// <summary>
    /// A property that tracks which items have already been processed to avoid duplicate events.
    /// </summary>
    private Dictionary<ItemName, bool> _trackedAlready { get; set; }
    private Dictionary<ItemName, RegionName> _startingItems { get; set; }
    private bool _attached { get; set; }
    private ulong _startAddress { get; set; }
    private bool _is64Bit { get; set; }
    private bool _spoilerLoaded { get; set; }
    private int _previousMap { get; set; }
    private uint _addressBase { get; set; }
    private RegionName _previousRegion { get; set; }
    private int Timeout { get; set; }
    private bool _autosave { get; set; }

    private Dictionary<ItemType, int> _collectibleItemAmounts { get; set; } = new()
    {
        { ItemType.GOLDEN_BANANA, 0 },
        { ItemType.DONKEY_BLUEPRINT, 0 },
        { ItemType.DIDDY_BLUEPRINT, 0 },
        { ItemType.LANKY_BLUEPRINT, 0 },
        { ItemType.TINY_BLUEPRINT, 0 },
        { ItemType.CHUNKY_BLUEPRINT, 0 },
        { ItemType.PEARL, 0 },
        { ItemType.BANANA_MEDAL, 0 },
        { ItemType.FAIRY, 0 },
        { ItemType.RAINBOW_COIN, 0 },
        { ItemType.BATTLE_CROWN, 0 },
        { ItemType.COMPANY_COIN, 0 },
        { ItemType.TOTAL_BLUEPRINTS, 0 }
    };

    private Dictionary<ItemType, ItemType> _turnedBlueprintToCollectible { get; init; } = new()
    {
        { ItemType.DONKEY_BLUEPRINT_TURNED, ItemType.DONKEY_BLUEPRINT },
        { ItemType.DIDDY_BLUEPRINT_TURNED, ItemType.DIDDY_BLUEPRINT },
        { ItemType.LANKY_BLUEPRINT_TURNED, ItemType.LANKY_BLUEPRINT },
        { ItemType.TINY_BLUEPRINT_TURNED, ItemType.TINY_BLUEPRINT },
        { ItemType.CHUNKY_BLUEPRINT_TURNED, ItemType.CHUNKY_BLUEPRINT }
    };
    #endregion

    /// <summary>
    /// Creates a new AutotrackerService with injected platform dependencies.
    /// </summary>
    public AutotrackerService(
        IItemTrackingService itemTrackingService,
        IEmulatorAttacher emulatorAttacher,
        IProcessMemoryReader memoryReader,
        ITimerFactory timerFactory,
        IApplicationStateService appState,
        IUserSettingsService settings,
        ICollectiblesService collectiblesService,
        IRegionPlacementOrchestrator regionPlacementOrchestrator
    )
    {
        _itemTrackingService = itemTrackingService ?? throw new ArgumentNullException(nameof(itemTrackingService));
        _emulatorAttacher = emulatorAttacher ?? throw new ArgumentNullException(nameof(emulatorAttacher));
        _memoryReader = memoryReader ?? throw new ArgumentNullException(nameof(memoryReader));
        _timerFactory = timerFactory ?? throw new ArgumentNullException(nameof(timerFactory));
        _appState = appState ?? throw new ArgumentNullException(nameof(appState));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _collectiblesService = collectiblesService ?? throw new ArgumentNullException(nameof(collectiblesService));
        _regionPlacementOrchestrator = regionPlacementOrchestrator ?? throw new ArgumentNullException(nameof(regionPlacementOrchestrator));

        // Initialize state
        CurrentRegion = RegionName.UNKNOWN;
        _previousRegion = RegionName.UNKNOWN;
        _checks = [];
        _startingItems = [];
        _trackedAlready = [];
        InitializeChecks();

        CurrentSongName = "";
        CurrentSongGame = "";
        _previousMap = -1;
        _addressBase = 0x00000000;
        _attached = false;
        _spoilerLoaded = false;
        _is64Bit = false;
        Timeout = 0;
        _autosave = false;
        _progHintItem = ItemType.GOLDEN_BANANA;
    }

    /// <summary>
    /// Starts the autotracking service.
    /// </summary>
    public void Start()
    {
        if (_pollingTimer != null)
        {
            return; // Already running
        }

        _pollingTimer = _timerFactory.CreateTimer(POLLING_INTERVAL_MS);
        _pollingTimer.Elapsed += OnPollingTick;
        _pollingTimer.Start();
    }

    /// <summary>
    /// Stops the autotracking service.
    /// </summary>
    public void Stop()
    {
        if (_pollingTimer != null)
        {
            _pollingTimer.Stop();
            _pollingTimer.Elapsed -= OnPollingTick;
            _pollingTimer.Dispose();
            _pollingTimer = null;
        }
    }

    /// <summary>
    /// Resets the service to initial state.
    /// </summary>
    public void Reset()
    {
        _spoilerLoaded = false;
        _attached = false;
        _trackedAlready.Clear();
        InitializeChecks();
        RegionLightingChanged?.Invoke(this, new AutotrackerRegionEventArgs { Region = CurrentRegion, LightUp = false });
        CurrentRegion = RegionName.UNKNOWN;
        CurrentSongName = "";
        RandomizerVersion = 0;
        RandomizerSubVersion = 0;
        CurrentSongGame = "";
        _autosave = false;
        _previousMap = -1;
    }

    /// <summary>
    /// Resets collection state but preserves emulator attachment.
    /// </summary>
    public void ResetChecks()
    {
        InitializeChecks();
        ExcludeStartingItems();
        CurrentRegion = RegionName.UNKNOWN;
    }

    /// <summary>
    /// Sets the starting items for the current seed.
    /// </summary>
    public void SetStartingItems(Dictionary<ItemName, RegionName> newItems)
    {
        _startingItems = new Dictionary<ItemName, RegionName>(newItems);
        ExcludeStartingItems();
    }

    /// <summary>
    /// Marks that a spoiler log has been loaded.
    /// </summary>
    public void SetSpoilerLoaded(string fileName)
    {
        _spoilerLoaded = true;
    }

    public void UpdateProgHintItem()
    {
        if (!_attached)
        {
            return;
        }
        var hintItemValue = ReadMemory(0x7FF8C3, 8);
        ItemType itemType = hintItemValue switch
        {
            4 => ItemType.TOTAL_BLUEPRINTS,
            5 => ItemType.FAIRY,
            6 => ItemType.KEY,
            7 => ItemType.BATTLE_CROWN,
            9 => ItemType.BANANA_MEDAL,
            11 => ItemType.PEARL,
            12 => ItemType.RAINBOW_COIN,
            15 => ItemType.COLORED_BANANA,
            _ => ItemType.GOLDEN_BANANA
        };
        if (_progHintItem == itemType)
        {
            return;
        }
        _progHintItem = itemType;
        ProgHintItemUpdated?.Invoke(this, new AutotrackerProgHintItemEventArgs { ProgHintItem = itemType });
    }

    /// <summary>
    /// Updates hint pack progress tracking.
    /// Reads thresholds from game memory if autotracking is active,
    /// otherwise calculates them using HintHelper.
    /// </summary>
    public void UpdateAmountToNextHint()
    {
        int totalItems = 0;

        var hintItem = _progHintItem;

        if (hintItem == ItemType.COLORED_BANANA)
        {
            totalItems = GetTotalCBs();
        }
        else if (_collectibleItemAmounts.TryGetValue(hintItem, out int itemAmount))
        {
            totalItems = itemAmount;
        }

        // If we have access to the game's memory (autotracking active), read thresholds from there
        // Otherwise, fall back to calculating thresholds using HintHelper
        int amount;
        if (_attached)
        {
            amount = GetAmountToNextHintPack(totalItems);
        }
        else
        {
            // Fallback: calculate thresholds from settings
            int progressiveHintCap = _appState.ProgressiveHintCap;
            var thresholds = HintHelper.GenerateThresholds(progressiveHintCap);
            amount = HintHelper.GetAmountToNextHint(totalItems, thresholds);
        }

        HintProgressUpdated?.Invoke(this, new AutotrackerHintEventArgs { AmountToNextHint = amount });
    }

    public void Shutdown()
    {
        Stop();
        Detach();
    }

    private int GetAmountToNextHintPack(int totalItems)
    {
        if (!_attached)
        {
            return 0;
        }
        uint baseAddress = 0x7FF898;
        for (uint i = 0; i < 10; ++i)
        {
            var threshold = ReadMemory(baseAddress + (i * 2), 16);
            if (totalItems < threshold)
            {
                return threshold - totalItems;
            }
        }
        return 0;
    }

    /// <summary>
    /// Manually process a saved item.
    /// </summary>
    public void ProcessSavedItem(ItemName item)
    {
        _trackedAlready[item] = true;
    }

    /// <summary>
    /// Checks if an item has been tracked.
    /// </summary>
    public bool ItemWasTracked(ItemName item)
    {
        return _trackedAlready.TryGetValue(item, out var tracked) && tracked;
    }

    /// <summary>
    /// Releases resources used by the service.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Shutdown();
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Raises the RegionLightingChanged event.
    /// </summary>
    public void OnRegionLightingChanged(AutotrackerRegionEventArgs e)
    {
        RegionLightingChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the SongChanged event.
    /// </summary>
    public void OnSongChanged(AutotrackerSongEventArgs e)
    {
        SongChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the HintProgressUpdated event.
    /// </summary>
    public void OnHintProgressUpdated(AutotrackerHintEventArgs e)
    {
        HintProgressUpdated?.Invoke(this, e);
    }

    #region Private Implementation Methods

    /// <summary>
    /// Called on each polling tick to read emulator memory and update state.
    /// </summary>
    private void OnPollingTick(object? sender, ElapsedEventArgs e)
    {
        try
        {
            Autotrack();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Autotrack exception: {ex.Message}");
            _attached = false;
        }
    }

    private void Detach()
    {
        if (_attached)
        {
            _memoryReader.CloseHandleSafe(_processHandle);
            _processHandle = IntPtr.Zero;
        }
        _attached = false;
    }

    /// <summary>
    /// Main autotracking loop - orchestrates all memory reads and state updates.
    /// </summary>
    private void Autotrack()
    {
        if (!_settings.Autotracking)
        {
            return;
        }

        AttachIfNecessary();
        if (!_attached)
        {
            return;
        }

        if (!ProcessConnected())
        {
            return;
        }

        UpdateVersion();
        CheckVersion();
        UpdateAddressBase();
        UpdateCurrentRegion();
        UpdateCurrentSong();

        if (CurrentRegion == RegionName.UNKNOWN)
        {
            return;
        }

        ResetCollectibleAmounts();
        ReadBlueprintsObtained();
        ReadMemoryForChecks();
        UpdateCollectibles();
        UpdateProgHintItem();
        UpdateAmountToNextHint();
    }

    /// <summary>
    /// Attempts to attach to an emulator process.
    /// </summary>
    private void AttachIfNecessary()
    {
        if (_attached)
        {
            return;
        }

        var verificationInfo = new GameVerificationInfo(
            0x759290, // DK64_ROM_SIGNATURE_OFFSET,
            32, // DK64_ROM_SIGNATURE_BITS,
            0x52414D42 // DK64_ROM_SIGNATURE_VALUE
        );

        AttachedEmulatorInfo? attachedProcessInfo = _emulatorAttacher.Attach(verificationInfo);
        
        if (attachedProcessInfo == null)
        {
            return;
        }

        _attached = true;
        _startAddress = attachedProcessInfo.BaseAddress;
        _processHandle = attachedProcessInfo.ProcessHandle;
        GameVerificationInfo = verificationInfo;
        _processId = attachedProcessInfo.ProcessId;
    }

    /// <summary>
    /// Checks if the process is still running and responsive.
    /// </summary>
    private bool ProcessConnected()
    {
        if (GameVerificationInfo is null)
        {
            Detach();
            return false;
        }
        if (ReadMemory(GameVerificationInfo.TargetAddress, GameVerificationInfo.TotalBits) == GameVerificationInfo.TargetValue)
        {
            Timeout = 0;
            return true;
        }

        Timeout++;
        if (Timeout > 10)
        {
            _attached = false;
        }

        return false;
    }

    /// <summary>
    /// Updates the randomizer version from memory.
    /// </summary>
    private void UpdateVersion()
    {
        RandomizerVersion = ReadMemory(0x7FFFF4, 8);
        RandomizerSubVersion = ReadMemory(0x7FFFF5, 8);
    }

    /// <summary>
    /// Checks if the randomizer version has changed and updates offsets if needed.
    /// </summary>
    private void CheckVersion()
    {
        var useNewOffsets = (RandomizerVersion >= 5);
        if (useNewOffsets != OffsetInfo.UseNewOffsets)
        {
            OffsetInfo.UseNewOffsets = useNewOffsets;
            InitializeChecks(false);
        }
    }

    /// <summary>
    /// Updates the address base for version 5+ randomizer.
    /// </summary>
    private void UpdateAddressBase()
    {
        if (RandomizerVersion < 5.0)
        {
            return;
        }

        uint countStructAddress = 0x7FFFB8;
        _addressBase = ReadPointer(countStructAddress);
    }

    /// <summary>
    /// Updates the current region based on map ID.
    /// </summary>
    private void UpdateCurrentRegion()
    {
        uint offset = 0x76A0A8;
        int area = ReadMemory(offset, 32);

        if (MapToRegion.MAP.TryGetValue(area, out RegionName newRegion))
        {
            if (newRegion != CurrentRegion)
            {
                RegionLightingChanged?.Invoke(this, new AutotrackerRegionEventArgs { Region = newRegion, LightUp = true });
                RegionLightingChanged?.Invoke(this, new AutotrackerRegionEventArgs { Region = CurrentRegion, LightUp = false });
            }

            if (_previousMap != -1 && area != _previousMap)
            {
                _autosave = true;
            }

            _previousRegion = CurrentRegion;
            CurrentRegion = newRegion;
            _previousMap = area;
        }
    }

    /// <summary>
    /// Reads and updates the current song from memory.
    /// </summary>
    private void UpdateCurrentSong()
    {
        var songGame = "";
        var songName = "";

        if (RandomizerVersion >= 4)
        {
            uint songPointer = 0x7FFFF0;
            uint songAddr = ReadPointer(songPointer);
            if (songAddr == 0x00000000)
            {
                return;
            }

            songGame = ReadAscii(ref songAddr);
            songAddr++;
            songName = ReadAscii(ref songAddr);
        }

        if (songName == "")
        {
            songName = songGame;
            songGame = "Donkey Kong 64";
        }

        if (songGame != CurrentSongGame || songName != CurrentSongName)
        {
            CurrentSongGame = songGame;
            CurrentSongName = songName;
            SongChanged?.Invoke(this, new AutotrackerSongEventArgs { SongGame = songGame, SongName = songName });
            WriteToSongFiles(songGame, songName);
        }
    }

    /// <summary>
    /// Reads a null-terminated ASCII string from memory.
    /// </summary>
    private string ReadAscii(ref uint startAddress)
    {
        List<byte> ascii = [];
        while (ascii.Count < 50)
        {
            byte next = (byte)ReadMemory(startAddress, 8);
            if (next > 127)
            {
                return "";
            }

            if (next == 0x00)
            {
                break;
            }

            ascii.Add(next);
            ++startAddress;
        }

        _ = new CultureInfo("en-US", false).TextInfo;
        string name = System.Text.Encoding.ASCII.GetString([.. ascii]);
        return SongFormatter.FormatSongString(name);
    }

    /// <summary>
    /// Writes song information to files for OBS display.
    /// </summary>
    private static void WriteToSongFiles(string songGame, string songName)
    {
        var songDisplayFolder = "TrackOMatic_SongDisplayOutput";
        Dictionary<string, string> fileWrites = new()
        {
            { "song_game_and_name.txt", songGame + "\n" + songName },
            { "song_game.txt", songGame },
            { "song_name.txt", songName }
        };

        try
        {
            Directory.CreateDirectory(songDisplayFolder);
            foreach (var entry in fileWrites)
            {
                try
                {
                    File.WriteAllText(Path.Combine(songDisplayFolder, entry.Key), entry.Value);
                }
                catch (IOException) { }
                catch (Exception) { }
            }
        }
        catch (Exception)
        {
            // Silently fail if song output folder cannot be created
        }
    }

    /// <summary>
    /// Reads a pointer value from memory, handling N64 address formatting.
    /// </summary>
    private uint ReadPointer(uint pointerAddr)
    {
        uint addr = (uint)ReadMemory(pointerAddr, 32);
        var wrongFirstByte = (addr >> 24) != 0x80;
        var blankAddr = addr == 0;
        return (blankAddr || wrongFirstByte) ? 0 : (addr & 0x00FFFFFF);
    }

    /// <summary>
    /// Resets collectible amounts for re-counting.
    /// </summary>
    private void ResetCollectibleAmounts()
    {
        foreach (var key in _collectibleItemAmounts.Keys.ToList())
        {
            _collectibleItemAmounts[key] = 0;
        }
    }

    /// <summary>
    /// Gets the total number of colored bananas collected.
    /// </summary>
    private int GetTotalCBs()
    {
        uint world_cb_offset_donkey = 0x7FC95A;
        int diff_between_kongs = 0x5E;
        int total = 0;

        for (int kong = 0; kong < 5; ++kong)
        {
            uint world_start = (uint)(world_cb_offset_donkey + (diff_between_kongs * kong));
            uint tns_start = world_start + 0x1C;

            for (int world = 0; world < 16; world += 2)
            {
                total += ReadMemory((uint)(world_start + world), 16);
            }

            for (int tns_count = 0; tns_count < 16; tns_count += 2)
            {
                total += ReadMemory((uint)(tns_start + tns_count), 16);
            }
        }

        return total;
    }

    /// <summary>
    /// Reads blueprint count data from memory.
    /// </summary>
    private void ReadBlueprintsObtained()
    {
        _collectibleItemAmounts[ItemType.TOTAL_BLUEPRINTS] = 0;
        if (RandomizerVersion < 5.0)
        {
            return;
        }

        var blueprintKeys = new List<ItemType>()
        {
            ItemType.DONKEY_BLUEPRINT, ItemType.DIDDY_BLUEPRINT, ItemType.LANKY_BLUEPRINT,
            ItemType.TINY_BLUEPRINT, ItemType.CHUNKY_BLUEPRINT
        };

        for (int i = 0; i < blueprintKeys.Count; i++)
        {
            var itemType = blueprintKeys[i];
            var kongBlueprints = ReadMemory((uint)(_addressBase + i), 8);
            kongBlueprints = IsOldBlueprintSystem() ? CountBits(kongBlueprints) : kongBlueprints;
            _collectibleItemAmounts[itemType] = kongBlueprints;
            _collectibleItemAmounts[ItemType.TOTAL_BLUEPRINTS] += kongBlueprints;

            if (!IsOldBlueprintSystem())
            {
                var turnedInBPs = ReadMemory((uint)(_addressBase + 0x19 + i), 8);
                _collectibleItemAmounts[itemType] -= turnedInBPs;
            }
        }
    }

    /// <summary>
    /// Determines if the blueprint system is the old version (pre-5.0).
    /// </summary>
    private bool IsOldBlueprintSystem()
    {
        return ((RandomizerVersion < 5.0) || (RandomizerVersion == 5.0 && RandomizerSubVersion == 0));
    }

    /// <summary>
    /// Counts the number of set bits in a value.
    /// </summary>
    private static int CountBits(int value)
    {
        int count = 0;
        while (value != 0)
        {
            value &= (value - 1);
            count++;
        }
        return count;
    }

    /// <summary>
    /// Reads memory for all tracked checks and processes new items.
    /// </summary>
    private void ReadMemoryForChecks()
    {
        foreach (var check in _checks)
        {
            var checkInfo = ImportantCheckList.ITEMS[check.ItemName];
            uint offset = 0x0000000;
            if (check.UsesCountStruct)
            {
                offset = _addressBase;
            }

            var bitMask = check.Bitmask;
            var isFlag = (bitMask != 0);
            var isSlam = check.ItemName.ToString().Contains("PROGRESSIVE_SLAM");

            if (isSlam)
            {
                bitMask = 0xF;
            }

            var output = ReadMemory(offset + check.Offset, check.TotalBits, bitMask);
            var valid = (output == check.Bitmask) || (bitMask == 0);
            if (isSlam)
            {
                valid = (output >= check.Bitmask);
            }

            if (!valid)
            {
                continue;
            }

            var collectible = _collectibleItemAmounts.ContainsKey(checkInfo.ItemType) ||
                            _turnedBlueprintToCollectible.ContainsKey(checkInfo.ItemType);

            if (collectible)
            {
                ProcessCollectible(output, checkInfo, isFlag);
            }
            else
            {
                ProcessRegularItem(check);
            }
        }
    }

    /// <summary>
    /// Processes a collectible item (blueprint, golden banana, etc).
    /// </summary>
    private void ProcessCollectible(int output, ImportantCheck checkInfo, bool isFlag)
    {
        var toAdd = output;
        var itemTypeToUse = checkInfo.ItemType;

        if (checkInfo.ItemType.ToString().EndsWith("BLUEPRINT"))
        {
            _collectibleItemAmounts[ItemType.TOTAL_BLUEPRINTS] += 1;
        }

        if (isFlag)
        {
            toAdd = 1;
            if (_turnedBlueprintToCollectible.ContainsKey(checkInfo.ItemType) && IsOldBlueprintSystem())
            {
                itemTypeToUse = _turnedBlueprintToCollectible[itemTypeToUse];
                toAdd = -1;
            }
        }

        _collectibleItemAmounts[itemTypeToUse] += toAdd;
    }

    /// <summary>
    /// Processes a regular (non-collectible) item.
    /// </summary>
    private void ProcessRegularItem(AutotrackedCheck check)
    {
        if (CurrentRegion == RegionName.START)
        {
            return;
        }

        var regionToUse = CurrentRegion;
        if (CurrentRegion == RegionName.DK_ISLES && _previousRegion == RegionName.START && !_spoilerLoaded)
        {
            regionToUse = RegionName.START;
        }

        var checkInfo = ImportantCheckList.ITEMS[check.ItemName];
        if (checkInfo.ItemType == ItemType.SHOPKEEPER && RandomizerVersion < 4)
        {
            _trackedAlready[check.ItemName] = true;
            return;
        }

        if (_trackedAlready[check.ItemName])
        {
            return;
        }

        // Write directly to the item tracking service now.
        var oldItem = _itemTrackingService.GetItemState(check.ItemName) ?? SavedItem.CreateEmpty(check.ItemName);
        var newItem = oldItem with
        {
            Autotracked = true,
            Region = regionToUse,
            Opacity = 1,
        };

        // Try to place in a spoiler vial slot first (if spoiler mode is active)
        if (!_regionPlacementOrchestrator.TryPlaceAutoTrackedItem(check.ItemName, regionToUse, newItem))
        {
            // Fall back to direct item tracking for non-spoiler items or if vial placement failed
            _itemTrackingService.SetItemState(check.ItemName, newItem, ChangeReason.AutoTracked);
        }

        _trackedAlready[check.ItemName] = true;
    }

    /// <summary>
    /// Updates collectible counts and fires events.
    /// </summary>
    private void UpdateCollectibles()
    {
        foreach (var entry in _collectibleItemAmounts.ToList())
        {
            _collectiblesService.SetCollectibleCount(entry.Key, entry.Value);
        }
    }

    /// <summary>
    /// Initializes or resets the list of tracked checks.
    /// </summary>
    private void InitializeChecks(bool resetTrackedItems = true)
    {
        _checks = [];
        foreach (var offsetInfo in OffsetInfo.OFFSETS)
        {
            _checks.Add(new AutotrackedCheck(offsetInfo.ItemName, offsetInfo.Offset, offsetInfo.TotalBits, offsetInfo.Bitmask, offsetInfo.UsesCountStruct));
            if (resetTrackedItems && !_trackedAlready.ContainsKey(offsetInfo.ItemName))
            {
                _trackedAlready[offsetInfo.ItemName] = false;
            }
        }
    }

    /// <summary>
    /// Marks starting items as already tracked.
    /// </summary>
    private void ExcludeStartingItems()
    {
        foreach (var entry in _startingItems)
        {
            _trackedAlready[entry.Key] = true;
        }
    }

    /// <summary>
    /// Core memory read operation (address fixing, type conversion, exception handling).
    /// Extracted for testability - this contains the pure read logic without state gates.
    /// </summary>
    private int PerformMemoryRead(uint addr, int numOfBits)
    {
        int toReturn;
        try
        {
            switch (numOfBits)
            {
                case 8:
                    toReturn = _memoryReader.ReadInt8(_processId, (uint)(_startAddress + _memoryReader.Int8AddrFix(addr)));
                    break;
                case 16:
                    toReturn = _memoryReader.ReadInt16(_processId, (uint)(_startAddress + _memoryReader.Int16AddrFix(addr)));
                    break;
                case 32:
                    toReturn = _memoryReader.ReadInt32(_processId, (uint)(_startAddress + addr));
                    break;
                default:
                    return 0;
            }
        }
        catch
        {
            return 0;
        }

        return toReturn;
    }

    /// <summary>
    /// Protected wrapper for PerformMemoryRead to enable clean testing without reflection.
    /// </summary>
    protected int TestPerformMemoryRead(uint addr, int numOfBits)
    {
        return PerformMemoryRead(addr, numOfBits);
    }

    /// <summary>
    /// Reads memory with state guards and bit masking.
    /// Calls PerformMemoryRead for the actual read operation.
    /// </summary>
    private int ReadMemory(uint addr, int numOfBits, int bitmask = 0)
    {
        if (!_attached)
        {
            return 0;
        }

        int toReturn = PerformMemoryRead(addr, numOfBits);

        if (bitmask != 0)
        {
            toReturn &= bitmask;
        }

        return toReturn;
    }

    #endregion
}
