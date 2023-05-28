using Microsoft.Extensions.Logging;
using Mycropad.App.Shared;
using Mycropad.App.Shared.Interfaces;
using Mycropad.App.Shared.Services;

namespace Mycropad.App.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        builder.Services.ConfigureMycropadApp();
        builder.Services.AddSingleton<IWindowProvider, MAUIWindowProvider>();

        var mauiApp = builder.Build();
        var deviceManager = mauiApp.Services.GetRequiredService<DeviceManager>();
        deviceManager.Start();
        return mauiApp;
    }
}

public class MAUIWindowProvider: IWindowProvider
{
    public void Hide()
    {
    }

    public void Close()
    {
    }
}