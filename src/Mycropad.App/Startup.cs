using System;
using Blazored.Modal;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mycropad.Lib.Device;

namespace Mycropad.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddBlazoredModal();

            services.AddSingleton<IMycropadDevice>(MycropadDevice_Serial.Instance);
            services.AddSingleton<DeviceManager>();
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

            if (HybridSupport.IsElectronActive)
            {
                ElectronBootstrap();
            }


            // Start DeviceManager
            app.ApplicationServices.GetRequiredService<DeviceManager>();
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
                Frame = false
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
            var menu = new MenuItem[] {
                new MenuItem
                {
                    Label = "Show",
                    Click = () => window.Maximize()
                },
                new MenuItem
                {
                    Label = "Exit",
                    Click = () => window.Close()
                }
            };

            var iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "trayicon.png");
            Electron.Tray.Show(iconPath, menu);
            Electron.Tray.SetToolTip("Mycropad");
        }
    }
}