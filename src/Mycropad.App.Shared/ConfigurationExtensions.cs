using Blazored.Modal;
using Microsoft.Extensions.DependencyInjection;
using Mycropad.App.Shared.Services;
using Mycropad.Lib.Device;

namespace Mycropad.App.Shared;

#pragma warning disable CA1416
public static class ConfigurationExtensions
{
    public static IServiceCollection ConfigureMycropadApp(this IServiceCollection services) =>
        services.AddBlazoredModal()
            .AddSingleton<IMycropadDevice>(MycropadDeviceSerial.Instance)
            .AddSingleton<DeviceManager>()
            .AddSingleton<ProfileManager>();
}
#pragma warning restore CA1416
