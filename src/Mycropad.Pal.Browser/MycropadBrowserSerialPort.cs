using Microsoft.JSInterop;
using Mycropad.Lib.Serial;

namespace Mycropad.Pal.Browser;

public class MycropadBrowserSerialPort(IJSRuntime jsRuntime) : JsModuleBase(jsRuntime, "serialPort"), ISerialPort
{
    public async ValueTask<string> Prompt(string message)
    {
        var module = await JsModule();
        return await module.InvokeAsync<string>("showPrompt", message);
    }

    public bool IsOpen { get; }
    public string PortName { get; set; }
    public int WriteTimeout { get; set; }
    public int ReadTimeout { get; set; }
    public void Open(uint usbVid, uint usbPid)
    {

    }

    public void DiscardInBuffer()
    {
        throw new NotImplementedException();
    }

    public void DiscardOutBuffer()
    {
        throw new NotImplementedException();
    }

    public int Read(byte[] responseData, int offset, int remainingLength)
    {
        throw new NotImplementedException();
    }

    public void Close()
    {
        throw new NotImplementedException();
    }

    public void Write(byte[] data, int i, int dataLength)
    {
        throw new NotImplementedException();
    }
}