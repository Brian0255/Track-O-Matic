using System.Windows;

using Microsoft.Extensions.DependencyInjection;

using TrackOMatic.Properties;
using TrackOMatic.Services;

namespace TrackOMatic;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    App()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IVersionService, VersionService>();

        _serviceProvider = services.BuildServiceProvider();
        ServiceLocator.Initialize(_serviceProvider);
        Dispatcher.UnhandledException += OnDispatcherUnhandledException;
    }

    private void App_Exit(object sender, ExitEventArgs e)
    {
    }

    void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var versionService = _serviceProvider.GetRequiredService<IVersionService>();
        MainWindow mainWindow = new(versionService);
        mainWindow.Show();

        UpdatePadBarrelImages();
    }

    public void UpdatePadBarrelImages()
    {
        var dicts = Resources.MergedDictionaries;
        dicts.Clear();
        dicts.Add(new ResourceDictionary
        {
            Source = new Uri("Dictionary1.xaml", UriKind.Relative)
        });
        var path = Settings.Default.ColoredBarrelPadMoves ? "ColoredBarrelPadImages.xaml" : "BaseBarrelPadImages.xaml";
        dicts.Add(new ResourceDictionary
        {
            Source = new Uri(path, UriKind.Relative)
        });
    }
}
