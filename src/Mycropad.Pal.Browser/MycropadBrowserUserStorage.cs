using Microsoft.JSInterop;
using Mycropad.Core.Abstractions;

namespace Mycropad.Pal.Browser;

public class MycropadBrowserUserStorage(IJSRuntime jsRuntime) : IUserStorage
{
    public string Read(string filename)
    {
        return ((IJSInProcessRuntime)jsRuntime).Invoke<string>("localStorage.getItem", filename);
    }

    public void Write(string filename, string content)
    {
        ((IJSInProcessRuntime)jsRuntime).InvokeVoid("localStorage.setItem", filename, content);
    }
}