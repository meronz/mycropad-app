using Microsoft.JSInterop;
using Mycropad.Core.Abstractions;

namespace Mycropad.Pal.Browser;

public class MycropadBrowserSerialPort : JsModuleBase, ISerialPort
{
    public MycropadBrowserSerialPort(IJSRuntime jsRuntime) : base(jsRuntime, "serialPort") { }

    public bool IsOpen { get; }
    public int WriteTimeout { get; set; }
    public int ReadTimeout { get; set; }

    public async Task Open(uint usbVid, uint usbPid)
    {
        var module = await JsModule;
        await module.InvokeVoidAsync("open", usbVid, usbPid);
    }

    public async Task DiscardInBuffer()
    {
        var module = await JsModule;
        await module.InvokeVoidAsync("discardInBuffer");
    }

    public async Task DiscardOutBuffer()
    {
        var module = await JsModule;
        await module.InvokeVoidAsync("discardOutBuffer");
    }

    public async Task<int> Read(byte[] buffer, int offset, int count)
    {
        var module = await JsModule;
        return await module.InvokeAsync<int>("read", buffer, offset, count);
    }

    public async Task Close()
    {
        var module = await JsModule;
        await module.InvokeVoidAsync("close");
    }

    public async Task Write(byte[] buffer, int offset, int count)
    {
        var module = await JsModule;
        await module.InvokeVoidAsync("write", buffer, offset, count);
    }
}