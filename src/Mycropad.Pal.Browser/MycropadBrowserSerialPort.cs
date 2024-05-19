using Microsoft.JSInterop;
using Mycropad.Core.Abstractions;

namespace Mycropad.Pal.Browser;

public class MycropadBrowserSerialPort(IJSRuntime jsRuntime) : JsModuleBase(jsRuntime, "serialPort"), ISerialPort
{
    private DotNetObjectReference<MycropadBrowserSerialPort>? _thisReference;

    public bool IsOpen { get; private set; }
    public int WriteTimeout { get; set; }
    public int ReadTimeout { get; set; }

    public async Task Open(uint usbVid, uint usbPid)
    {
        if (IsOpen) return;
        var module = await JsModule;
        _thisReference ??= DotNetObjectReference.Create(this);
        await module.InvokeVoidAsync("open", usbVid, usbPid, _thisReference);
        IsOpen = true;
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
        var readBuffer = await module.InvokeAsync<byte[]>("read");
        readBuffer.CopyTo(buffer, offset);
        return readBuffer.Length;
    }

    public async Task Close()
    {
        if (!IsOpen) return;
        var module = await JsModule;
        await module.InvokeVoidAsync("close");
        IsOpen = false;
    }

    public async Task Write(byte[] buffer, int offset, int count)
    {
        var module = await JsModule;
        await module.InvokeVoidAsync("write", buffer);
    }

    [JSInvokable]
    public ValueTask OnDisconnect()
    {
        IsOpen = false;
        return ValueTask.CompletedTask;
    }

    public override async ValueTask DisposeAsync()
    {
        if (IsOpen) await Close();
        _thisReference?.Dispose();
        await base.DisposeAsync();
    }
}