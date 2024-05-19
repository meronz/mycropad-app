namespace Mycropad.Core;

public class Guard : IDisposable
{
    private readonly SemaphoreSlim _semaphore;

    public Guard(SemaphoreSlim semaphore)
    {
        _semaphore = semaphore;
        _semaphore.Wait();
    }

    public void Dispose()
    {
        _semaphore.Release();
    }
}