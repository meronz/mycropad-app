using Mycropad.App.Shared.Interfaces;

namespace Mycropad.App.Electron.Services;

public class ElectronWindowProvider: IWindowProvider
{
    public void Hide()
    {
        foreach (var w in ElectronNET.API.Electron.WindowManager.BrowserWindows)
        {
            w.Hide();
        }
    }

    public void Close()
    {
        foreach (var w in ElectronNET.API.Electron.WindowManager.BrowserWindows)
        {
            w.Close();
        }
    }
}