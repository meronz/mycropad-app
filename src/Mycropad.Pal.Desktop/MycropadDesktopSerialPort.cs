using System.IO.Ports;
using Mycropad.Lib.Serial;

namespace Mycropad.Pal.Desktop;

public class MycropadDesktopSerialPort : ISerialPort
{
    private readonly SerialPort _port = new();

    public bool IsOpen => _port.IsOpen;

    public string PortName
    {
        get => _port.PortName;
        set => _port.PortName = value;
    }

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

    public void Open() => _port.Open();

    public void Open(uint usbVid, uint usbPid)
    {
        _port.PortName = DesktopPlatformUtils.FindSerialPort(usbVid, usbPid);
        _port.Open();
    }

    public void DiscardInBuffer() => _port.DiscardInBuffer();

    public void DiscardOutBuffer() => _port.DiscardOutBuffer();

    public int Read(byte[] responseData, int offset, int remainingLength) => _port.Read(responseData, offset, remainingLength);

    public void Close() => _port.Close();

    public void Write(byte[] data, int i, int dataLength) => _port.Write(data, i, dataLength);
}