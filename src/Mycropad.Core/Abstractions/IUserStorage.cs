namespace Mycropad.Core.Abstractions;

public interface IUserStorage
{
    public string Read(string filename);
    public void Write(string filename, string content);
}