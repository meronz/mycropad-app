using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mycropad.App.Electron.Services;
using Mycropad.App.Shared;
using Mycropad.App.Shared.Interfaces;
using Mycropad.Core.Abstractions;
using Mycropad.Lib.Device;
using Mycropad.Lib.Profiles;
using Mycropad.Pal.Desktop;

#pragma warning disable CA1416

namespace Mycropad.App.Electron;

public class Startup
{
    private readonly IConfiguration _configuration;
    private bool _trayShown;
    private ProfileManager _profileManager = null!;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddServerSideBlazor();

        services.AddSingleton<IUserStorage>(new MycropadDesktopUserStorage())
            .AddSingleton<ISerialPort>(new MycropadDesktopSerialPort())
            .ConfigureMycropadApp();
        services.AddSingleton<IWindowProvider, ElectronWindowProvider>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseStaticFiles();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
        });

        if (HybridSupport.IsElectronActive) ElectronBootstrap();


        // Start DeviceManager after profiles have been loaded.
        _profileManager = app.ApplicationServices.GetRequiredService<ProfileManager>();
        app.ApplicationServices.GetRequiredService<DeviceManager>().Start();
    }

    private async void ElectronBootstrap()
    {
        var startMinimized = _configuration.GetValue<int>("START_MINIMIZED") != 0;
        Console.WriteLine($"Minimized: {startMinimized}");

        var browserWindow = await ElectronNET.API.Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
        {
            Width = 800,
            Height = 600,
            Show = false,
            AutoHideMenuBar = true,
            Resizable = false,
            Maximizable = false,
            Fullscreenable = false,
            Frame = false,
        });

        await browserWindow.WebContents.Session.ClearCacheAsync();

        browserWindow.SetTitle("Mycropad");
        browserWindow.OnReadyToShow += () =>
        {
            if (!startMinimized) browserWindow.Show();
            TrayIconSetup(browserWindow);
        };
    }

    public void TrayIconSetup(BrowserWindow window)
    {
        var menu = new List<MenuItem>();
        menu.AddRange(_profileManager.Select(profile => new MenuItem
        {
            Label = profile.Name, 
            Click = () => _profileManager.SwitchProfile(profile.Id),
        }));
            
        menu.Add(new() {Type = MenuType.separator});
        menu.Add(new() {Label = "Show", Click = window.Show});
        menu.Add(new() {Label = "Exit", Click = window.Close});

        var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "trayicon.png");

        // Electron.Tray.Destroy is bugged upstream:
        // https://github.com/electron/electron/issues/17622
        if (_trayShown) ElectronNET.API.Electron.Tray.Destroy();

        ElectronNET.API.Electron.Tray.Show(iconPath, menu.ToArray());
        ElectronNET.API.Electron.Tray.SetToolTip("Mycropad");
        _trayShown = true;
    }
}

#pragma warning restore CA1416