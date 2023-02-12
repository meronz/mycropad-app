using Mycropad.Core.Abstractions;

namespace Mycropad.Pal.Desktop;

public class MycropadDesktopUserStorage : IUserStorage
{
    public string Read(string filename)
    {
        var dirPath = Path.Combine(DesktopPlatformUtils.GetHomeDirectory(), "mycropad");
        if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

        var path = Path.Combine(dirPath, filename);
        return File.ReadAllText(path);
    }

    public void Write(string filename, string content)
    {
        var dirPath = Path.Combine(DesktopPlatformUtils.GetHomeDirectory(), "mycropad");
        if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

        var path = Path.Combine(dirPath, filename);
        File.WriteAllText(path, content);
    }
}