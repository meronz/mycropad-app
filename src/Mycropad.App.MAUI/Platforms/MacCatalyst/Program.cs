using ObjCRuntime;
using UIKit;

namespace Mycropad.App.MAUI;

public class Program
{
    // This is the main entry point of the application.
    static void Main(string[] args)
    {
#if DEBUG
        // Debugger can't attach otherwise...
        Thread.Sleep(5000);
#endif
        // if you want to use a different Application Delegate class from "AppDelegate"
        // you can specify it here.
        UIApplication.Main(args, null, typeof(AppDelegate));
    }
}