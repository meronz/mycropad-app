using ElectronNET.API;
using Mycropad.App.Shared.Interfaces;

namespace Mycropad.App.Services;

public class ElectronWindowProvider: IWindowProvider
{
    public void Hide()
    {
        foreach (var w in Electron.WindowManager.BrowserWindows)
        {
            w.Hide();
        }
    }

    public void Close()
    {
        foreach (var w in Electron.WindowManager.BrowserWindows)
        {
            w.Close();
        }
    }
}