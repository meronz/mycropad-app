using System;
using System.IO;
using Blazored.Modal;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mycropad.App.Services;
using Mycropad.Lib.Device;

#pragma warning disable CA1416

namespace Mycropad.App
{
    public class Startup
    {
        private bool _trayShown;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddBlazoredModal();

            services.AddSingleton<IMycropadDevice>(MycropadDeviceSerial.Instance);
            services.AddSingleton<DeviceManager>();
            services.AddSingleton<ProfileManager>();
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


            // Start DeviceManager
            app.ApplicationServices.GetRequiredService<ProfileManager>();
            app.ApplicationServices.GetRequiredService<DeviceManager>().Start();
        }

        public async void ElectronBootstrap()
        {
            var browserWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
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
                browserWindow.Show();
                TrayIconSetup(browserWindow);
            };
        }

        public void TrayIconSetup(BrowserWindow window)
        {
            var menu = new MenuItem[]
            {
                new()
                {
                    Label = "Show",
                    Click = window.Show,
                },
                new()
                {
                    Label = "Exit",
                    Click = window.Close,
                },
            };

            var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "trayicon.png");

            if (_trayShown) Electron.Tray.Destroy();

            Electron.Tray.Show(iconPath, menu);
            Electron.Tray.SetToolTip("Mycropad");
            _trayShown = true;
        }
    }
}

#pragma warning restore CA1416