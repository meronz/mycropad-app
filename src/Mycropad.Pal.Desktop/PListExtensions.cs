using Claunia.PropertyList;

namespace Mycropad.Pal.Desktop;

public static class PListExtensions
{
    public static bool Compare(this NSDictionary dict, string key, string value)
    {
        if (!dict.TryGetValue(key, out var nsObj)) return false;
        return ((NSString)nsObj).Content == value;
    }

    public static bool Compare(this NSDictionary dict, string key, uint value)
    {
        if (!dict.TryGetValue(key, out var nsObj)) return false;
        return ((NSNumber)nsObj).ToInt() == value;
    }

    public static IEnumerable<NSObject> TryEnumerate(this NSDictionary dict, string key)
    {
        if(dict.TryGetValue(key, out var array) && array is NSArray nsArray)
        {
            return nsArray;
        }

        return [];
    }
}