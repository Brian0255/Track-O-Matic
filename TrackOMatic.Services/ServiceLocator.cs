using Microsoft.Extensions.DependencyInjection;

namespace TrackOMatic.Services;

public static class ServiceLocator
{
    private static IServiceProvider? _provider;

    public static void Initialize(IServiceProvider provider) => _provider = provider;

    public static T GetService<T>() where T : notnull
    {
        if (_provider == null)
        {
            throw new InvalidOperationException("ServiceLocator not initialized");
        }

        return _provider.GetRequiredService<T>();
    }
}
