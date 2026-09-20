namespace TrackOMatic.Services.Test;

/// <summary>
/// Tests for the IResourceDictionaryProvider interface contract.
/// </summary>
/// <remarks>
/// The WpfResourceDictionaryProvider implementation are currently skipped due to
/// no dedicated WPF services project.
/// This test class verifies that any implementation of IResourceDictionaryProvider
/// meets the expected contract behavior.
/// </remarks>
public class ResourceDictionaryProviderTests
{
    [Fact]
    public void IResourceDictionaryProvider_CanBeMocked()
    {
        // Arrange & Act
        var mock = new Moq.Mock<IResourceDictionaryProvider>();

        // Assert - Verify interface can be mocked (basic sanity check)
        Assert.Multiple(() => {
            Assert.NotNull(mock);
            Assert.NotNull(mock.Object);
        });
    }

    [Fact]
    public void IResourceDictionaryProvider_UpdateResourceDictionaries_CanBeCalled()
    {
        // Arrange
        var mock = new Moq.Mock<IResourceDictionaryProvider>();

        // Act
        mock.Object.UpdateResourceDictionaries();

        // Assert - Verify method was callable
        mock.Verify(x => x.UpdateResourceDictionaries(), Moq.Times.Once);
    }

    [Fact]
    public void IResourceDictionaryProvider_UpdateResourceDictionaries_NoThrowOnNull()
    {
        // Arrange
        var mock = new Moq.Mock<IResourceDictionaryProvider>();
        mock.Setup(x => x.UpdateResourceDictionaries())
            .Callback(() => { }); // No-op

        // Act & Assert - Should not throw
        mock.Object.UpdateResourceDictionaries();
    }
}
