using Microsoft.JSInterop;

namespace Mycropad.Pal.Browser;

public class JsModuleBase : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

    protected JsModuleBase(IJSRuntime jsRuntime, string moduleName)
    {
        _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", $"./_content/Mycropad.Pal.Browser/{moduleName}.js").AsTask());
        _ = _moduleTask.Value;
    }

    private IJSObjectReference? _reference;

    protected IJSInProcessObjectReference SyncJsModule
    {
        get
        {
            _reference ??= _moduleTask.Value.Result;
            return (IJSInProcessObjectReference)_reference;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}