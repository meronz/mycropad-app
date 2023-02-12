using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Mycropad.App.Shared;
using Mycropad.App.Wasm;
using Mycropad.Core.Abstractions;
using Mycropad.Lib.Profiles;
using Mycropad.Pal.Browser;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services
    .AddSingleton<IUserStorage, MycropadBrowserUserStorage>()
    .AddSingleton<ISerialPort, MycropadBrowserSerialPort>()
    .ConfigureMycropadApp();

var app = builder.Build();
app.Services.GetRequiredService<ProfileManager>();
await app.RunAsync();
