using System.IO.Ports;
using Mycropad.Core.Abstractions;

namespace Mycropad.Pal.Desktop;

public class MycropadDesktopSerialPort : ISerialPort
{
    private readonly SerialPort _port = new();

    public bool IsOpen => _port.IsOpen;

    public int WriteTimeout
    {
        get => _port.WriteTimeout;
        set => _port.WriteTimeout = value;
    }

    public int ReadTimeout
    {
        get => _port.ReadTimeout;
        set => _port.ReadTimeout = value;
    }

    public Task Open(uint usbVid, uint usbPid)
    {
        _port.PortName = DesktopPlatformUtils.FindSerialPort(usbVid, usbPid);
        _port.Open();
        return Task.CompletedTask;
    }

    public Task DiscardInBuffer()
    {
        _port.DiscardInBuffer();
        return Task.CompletedTask;
    }

    public Task DiscardOutBuffer()
    {
        _port.DiscardOutBuffer();
        return Task.CompletedTask;
    }

    public Task<int> Read(byte[] buffer, int offset, int count)
    {
        var read = _port.Read(buffer, offset, count);
        return Task.FromResult(read);
    }

    public Task Close()
    {
        _port.Close();
        return Task.CompletedTask;
    }

    public Task Write(byte[] buffer, int offset, int count)
    {
        _port.Write(buffer, offset, count);
        return Task.CompletedTask;
    }
}