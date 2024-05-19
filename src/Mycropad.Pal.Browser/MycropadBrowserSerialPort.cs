using Microsoft.JSInterop;
using Mycropad.Lib.Serial;

namespace Mycropad.Pal.Browser;

public class MycropadBrowserSerialPort : JsModuleBase, ISerialPort
{
    public MycropadBrowserSerialPort(IJSRuntime jsRuntime) : base(jsRuntime, "serialPort") { }

    public bool IsOpen { get; }
    public string PortName { get; set; }
    public int WriteTimeout { get; set; }
    public int ReadTimeout { get; set; }

    public void Open(uint usbVid, uint usbPid)
    {
        SyncJsModule.InvokeVoid("open", usbVid, usbPid);
    }

    public void DiscardInBuffer()
    {
        SyncJsModule.InvokeVoid("discardInBuffer");
    }

    public void DiscardOutBuffer()
    {
        SyncJsModule.InvokeVoid("discardOutBuffer");
    }

    public int Read(byte[] buffer, int offset, int count)
    {
        return SyncJsModule.Invoke<int>("read", buffer, offset, count);
    }

    public void Close()
    {
        SyncJsModule.InvokeVoid("close");
    }

    public void Write(byte[] buffer, int offset, int count)
    {
        SyncJsModule.InvokeVoid("write", buffer, offset, count);
    }
}