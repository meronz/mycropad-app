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

    protected Task<IJSObjectReference> JsModule => _moduleTask.Value;

    public virtual async ValueTask DisposeAsync()
    {
        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}