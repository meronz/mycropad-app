namespace Mycropad.Core.Abstractions;

public interface ISerialPort
{
    public bool IsOpen { get; }
    public int WriteTimeout { get; set; }
    public int ReadTimeout { get; set; }
    Task Open(uint usbVid, uint usbPid);
    public Task DiscardInBuffer();
    public Task DiscardOutBuffer();
    public Task<int> Read(byte[] buffer, int offset, int count);
    public Task Close();
    public Task Write(byte[] buffer, int offset, int count);
}