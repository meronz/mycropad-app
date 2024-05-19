using Blazored.Modal;
using Microsoft.Extensions.DependencyInjection;
using Mycropad.Core.Abstractions;
using Mycropad.Lib;
using Mycropad.Lib.Device;
using Mycropad.Lib.Profiles;
using Mycropad.Lib.Serial;

namespace Mycropad.App.Shared;

#pragma warning disable CA1416
public static class ConfigurationExtensions
{
    public static IServiceCollection ConfigureMycropadApp(this IServiceCollection services)
        => services.AddBlazoredModal()
            .AddSingleton<IMycropadDevice, MycropadDeviceSerial>()
            .AddSingleton<DeviceManager>()
            .AddSingleton<ProfileManager>();
}
#pragma warning restore CA1416
