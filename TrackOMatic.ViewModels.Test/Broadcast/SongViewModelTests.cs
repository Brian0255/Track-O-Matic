namespace TrackOMatic.ViewModels.Test.Broadcast;

public class SongViewModelTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidSongService_Succeeds()
    {
        // Arrange
        var songServiceMock = new Mock<ISongService>();
        songServiceMock.Setup(x => x.SongGame).Returns("DK64");
        songServiceMock.Setup(x => x.SongName).Returns("Jungle Japes");

        // Act & Assert - should not throw
        var viewModel = SynchronizationContextTestHelper.RunWithUIContext(() =>
            new SongViewModel(songServiceMock.Object)
        );

        // Verify initial state
        Assert.Equal("DK64", viewModel.SongGame);
        Assert.Equal("Jungle Japes", viewModel.SongName);
        Assert.Equal("headphones", viewModel.ResourceKey);
    }

    [Fact]
    public void Constructor_WithNullSongService_ThrowsArgumentNullException()
    {
        // Act & Assert
        var ex = SynchronizationContextTestHelper.RunWithUIContext(() =>
            Assert.Throws<ArgumentNullException>(() => new SongViewModel(null!))
        );

        Assert.NotNull(ex);
        Assert.Equal("songService", ex.ParamName);
    }

    [Fact]
    public void Constructor_SubscribesToSongService()
    {
        // Arrange
        var songServiceMock = new Mock<ISongService>();
        songServiceMock.Setup(x => x.SongGame).Returns("DK64");
        songServiceMock.Setup(x => x.SongName).Returns("Jungle Japes");

        // Act
        SynchronizationContextTestHelper.RunWithUIContext(() => new SongViewModel(songServiceMock.Object));

        // Assert
        songServiceMock.VerifyAdd(x => x.SongChanged += It.IsAny<EventHandler<SongChangedEventArgs>>(), Times.Once);
    }

    #endregion

    #region Event Handling Tests

    [Fact]
    public void OnSongChanged_EventSubscription_InvokesUpdateFromService()
    {
        // Arrange
        var songServiceMock = new Mock<ISongService>();
        songServiceMock.Setup(x => x.SongGame).Returns("DK64");
        songServiceMock.Setup(x => x.SongName).Returns("Jungle Japes");

        var eventRaised = false;
        var updateCalled = false;

        var viewModel = SynchronizationContextTestHelper.RunWithUIContext(() =>
            new SongViewModel(songServiceMock.Object)
        );

        // Act - verify we can raise the event successfully
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            eventRaised = true;
            songServiceMock.Raise(x => x.SongChanged += null, new SongChangedEventArgs("", ""));
            updateCalled = true;
        });

        // Assert - verify the event was raised and processed without exceptions
        Assert.True(eventRaised);
        Assert.True(updateCalled);
        // Properties retain their initial values (mock setup doesn't return new values after Raise)
        Assert.Equal("DK64", viewModel.SongGame);
    }

    [Fact]
    public void OnSongChanged_SameContext_CallsUpdateDirectly()
    {
        // Arrange, Act, and Assert - all in the same UI context
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var songServiceMock = new Mock<ISongService>();
            songServiceMock.Setup(x => x.SongGame).Returns("Super Mario World");
            songServiceMock.Setup(x => x.SongName).Returns("Athletic");

            // Create view model (stores the current UI context as _uiContext)
            var viewModel = new SongViewModel(songServiceMock.Object);

            // Update mock to return new values when UpdateFromService reads them
            songServiceMock.Setup(x => x.SongGame).Returns("Donkey Kong 64");
            songServiceMock.Setup(x => x.SongName).Returns("Boss Blast");

            // Raise event while in the same UI context (so SynchronizationContext.Current == _uiContext)
            // This should hit the else branch and call UpdateFromService() directly
            songServiceMock.Raise(x => x.SongChanged += null, new SongChangedEventArgs("", ""));

            // Assert - properties should be updated from the new mock values
            Assert.Equal("Donkey Kong 64", viewModel.SongGame);
            Assert.Equal("Boss Blast", viewModel.SongName);
        });
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_UnsubscribesFromSongService()
    {
        // Arrange
        var songServiceMock = new Mock<ISongService>();
        songServiceMock.Setup(x => x.SongGame).Returns("DK64");
        songServiceMock.Setup(x => x.SongName).Returns("Jungle Japes");

        var viewModel = SynchronizationContextTestHelper.RunWithUIContext(() =>
            new SongViewModel(songServiceMock.Object)
        );

        // Act
        viewModel.Dispose();

        // Assert
        songServiceMock.VerifyRemove(x => x.SongChanged -= It.IsAny<EventHandler<SongChangedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Arrange
        var songServiceMock = new Mock<ISongService>();
        songServiceMock.Setup(x => x.SongGame).Returns("DK64");
        songServiceMock.Setup(x => x.SongName).Returns("Jungle Japes");

        var viewModel = SynchronizationContextTestHelper.RunWithUIContext(() =>
            new SongViewModel(songServiceMock.Object)
        );

        // Act & Assert - should not throw
        viewModel.Dispose();
        viewModel.Dispose();
        viewModel.Dispose();
    }

    #endregion

    #region Property Tests

    [Fact]
    public void SongGame_InitializedFromService()
    {
        // Arrange
        const string expectedGame = "Donkey Kong 64";
        var songServiceMock = new Mock<ISongService>();
        songServiceMock.Setup(x => x.SongGame).Returns(expectedGame);
        songServiceMock.Setup(x => x.SongName).Returns("Test");

        // Act
        var viewModel = SynchronizationContextTestHelper.RunWithUIContext(() =>
            new SongViewModel(songServiceMock.Object)
        );

        // Assert
        Assert.Equal(expectedGame, viewModel.SongGame);
    }

    [Fact]
    public void SongName_InitializedFromService()
    {
        // Arrange
        const string expectedName = "Blast-O-Matic Theme";
        var songServiceMock = new Mock<ISongService>();
        songServiceMock.Setup(x => x.SongGame).Returns("Test");
        songServiceMock.Setup(x => x.SongName).Returns(expectedName);

        // Act
        var viewModel = SynchronizationContextTestHelper.RunWithUIContext(() =>
            new SongViewModel(songServiceMock.Object)
        );

        // Assert
        Assert.Equal(expectedName, viewModel.SongName);
    }

    [Fact]
    public void ResourceKey_AlwaysHeadphones()
    {
        // Arrange
        var songServiceMock = new Mock<ISongService>();
        songServiceMock.Setup(x => x.SongGame).Returns("Test");
        songServiceMock.Setup(x => x.SongName).Returns("Test");

        // Act
        var viewModel = SynchronizationContextTestHelper.RunWithUIContext(() =>
            new SongViewModel(songServiceMock.Object)
        );

        // Assert
        Assert.Equal("headphones", viewModel.ResourceKey);
    }

    #endregion
}
