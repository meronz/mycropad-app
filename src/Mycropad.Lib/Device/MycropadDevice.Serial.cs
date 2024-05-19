using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mycropad.Core;
using Mycropad.Core.Abstractions;
using Mycropad.Lib.Device.Messages;
using Mycropad.Lib.Enums;
using Mycropad.Lib.Types;

namespace Mycropad.Lib.Device;

public sealed class MycropadDeviceSerial(ISerialPort device) : MycropadDeviceBase, IMycropadDevice, IAsyncDisposable
{
    private const uint USB_VID = 0xCAFE;
    private const uint USB_PID = 0x4005;

    private readonly SemaphoreSlim _deviceSemaphore = new(1, 1);

    public bool Connected => device.IsOpen;
    public Action? OnDeviceConnected { get; set; }
    public Action? OnDeviceDisconnected { get; set; }


    public async Task Start()
    {
        await OpenDevice();
        OnDeviceConnected?.Invoke();
    }


    public async Task Heartbeat()
    {
        using var guard = new Guard(_deviceSemaphore);
        var data = Command(CommandTypes.Heartbeat);
        await Write(data);
        var (readData, readLength) = await Read();

        var (cmd, ok, _) = Response(readData, readLength);
        if (cmd != CommandTypes.Heartbeat) throw new($"Bad CommandType {cmd}");
        if (!ok) throw new($"{cmd} failed!");
    }

    public async Task SetKeymap(DeviceKeymap deviceKeymap)
    {
        using var guard = new Guard(_deviceSemaphore);
        var data = Command(CommandTypes.SetKeymap, deviceKeymap.ToBytes());
        await Write(data);

        var (readData, readLength) = await Read();
        var (cmd, ok, _) = Response(readData, readLength);
        if (cmd != CommandTypes.SetKeymap) throw new($"Bad CommandType {cmd}");
        if (!ok) throw new($"{cmd} failed!");
    }

    public async Task<DeviceKeymap> ReadKeymap()
    {
        using var guard = new Guard(_deviceSemaphore);
        var data = Command(CommandTypes.ReadKeymap);
        await Write(data);

        var (readData, readLength) = await Read();
        var (cmd, ok, keymapBytes) = Response(readData, readLength);

        if (cmd != CommandTypes.ReadKeymap) throw new($"Bad CommandType {cmd}");
        if (!ok) throw new($"{cmd} failed!");

        return DeviceKeymap.FromBytes(keymapBytes);
    }

    public async Task DefaultKeymap()
    {
        using var guard = new Guard(_deviceSemaphore);
        var data = Command(CommandTypes.DefaultKeymap);
        await Write(data);
        var (readData, readLength) = await Read();

        var (cmd, ok, _) = Response(readData, readLength);
        if (cmd != CommandTypes.DefaultKeymap) throw new($"Bad CommandType {cmd}");
        if (!ok) throw new($"{cmd} failed!");
    }

    public async Task SwitchKeymap(DeviceKeymap deviceKeymap)
    {
        using var guard = new Guard(_deviceSemaphore);
        var data = Command(CommandTypes.SwitchKeymap, deviceKeymap.ToBytes());
        await Write(data);

        var (readData, readLength) = await Read();
        var (cmd, ok, _) = Response(readData, readLength);
        if (cmd != CommandTypes.SwitchKeymap) throw new($"Bad CommandType {cmd}");
        if (!ok) throw new($"{cmd} failed!");
    }

    public async Task LedsSwitchPattern(LedsPattern pattern)
    {
        using var guard = new Guard(_deviceSemaphore);
        var data = Command(CommandTypes.LedsSwitchPattern, new[] {(byte) pattern});
        await Write(data);

        var (readData, readLength) = await Read();
        var (cmd, ok, _) = Response(readData, readLength);
        if (cmd != CommandTypes.LedsSwitchPattern) throw new($"Bad CommandType {cmd}");
        if (!ok) throw new($"{cmd} failed!");
    }

    public async Task LedsSetFixedMap(IEnumerable<LedColor> map)
    {
        using var guard = new Guard(_deviceSemaphore);
        var mapBytes = map.Take(8).SelectMany(x => BitConverter.GetBytes(x.ToUInt32())).ToArray();
        var data = Command(CommandTypes.LedsSetFixedMap, mapBytes);
        await Write(data);

        var (readData, readLength) = await Read();
        var (cmd, ok, _) = Response(readData, readLength);
        if (cmd != CommandTypes.LedsSetFixedMap) throw new($"Bad CommandType {cmd}");
        if (!ok) throw new($"{cmd} failed!");
    }

    private async Task OpenDevice()
    {
        device.WriteTimeout = 500;
        device.ReadTimeout = 500;

        await device.Open(USB_VID, USB_PID);
        await device.DiscardInBuffer();
        await device.DiscardOutBuffer();
    }

    private async Task<(byte[] data, int length)> Read()
    {
        try
        {
            var offset = 0;
            var stxReceived = false;
            var responseData = new byte[1024];
            var toReceive = 0;

            do
            {
                var remainingLength = responseData.Length - offset;
                var bytesRead = await device.Read(responseData, offset, remainingLength);
                offset += bytesRead;

                if (remainingLength == 0)
                {
                    var newBuf = new byte[responseData.Length + 1024];
                    Buffer.BlockCopy(responseData, 0, newBuf, 0, responseData.Length);
                    responseData = newBuf;
                }

                if (!stxReceived)
                {
                    if (responseData[0] == 0x02)
                    {
                        stxReceived = true;
                        toReceive = (int) BitConverter.ToUInt32(responseData, 3);
                        toReceive += 8;
                    }
                    else
                    {
                        offset = 0;
                        continue;
                    }
                }

                // here stx has been received.
                // etx received
                if (responseData[offset - 1] == 0x03 && toReceive == offset) return (responseData, offset);
            } while (true);
        }
        catch (Exception)
        {
            await device.Close();
            OnDeviceDisconnected?.Invoke();
        }

        return ([], -1);
    }

    private async Task Write(byte[] data)
    {
        try
        {
            await device.DiscardInBuffer();
            await device.Write(data, 0, data.Length);
        }
        catch (Exception)
        {
            await device.Close();
            OnDeviceDisconnected?.Invoke();
        }
    }

    public async ValueTask DisposeAsync()
    {
        _deviceSemaphore.Dispose();
        await device.Close();
    }
}