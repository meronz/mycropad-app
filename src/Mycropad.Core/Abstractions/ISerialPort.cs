namespace Mycropad.Lib.Serial;

public interface ISerialPort
{
    public bool IsOpen { get; }
    public string PortName { get; set; }
    public int WriteTimeout { get; set; }
    public int ReadTimeout { get; set; }
    void Open(uint usbVid, uint usbPid);
    public void DiscardInBuffer();
    public void DiscardOutBuffer();
    public int Read(byte[] responseData, int offset, int remainingLength);
    public void Close();
    public void Write(byte[] data, int i, int dataLength);
}