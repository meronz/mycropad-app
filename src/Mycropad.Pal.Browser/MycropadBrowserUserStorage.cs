using Microsoft.JSInterop;
using Mycropad.Core.Abstractions;

namespace Mycropad.Pal.Browser;

public class MycropadBrowserUserStorage(IJSRuntime jsRuntime) : JsModuleBase(jsRuntime, "userStorage"), IUserStorage
{
    public string Read(string filename)
    {
        var module = JsModule().Result;
        return ((IJSInProcessObjectReference)module).Invoke<string>("read", filename);
    }

    public void Write(string filename, string content)
    {
        var module = JsModule().Result;
        ((IJSInProcessObjectReference)module).InvokeVoid("write", filename, content);
    }
}