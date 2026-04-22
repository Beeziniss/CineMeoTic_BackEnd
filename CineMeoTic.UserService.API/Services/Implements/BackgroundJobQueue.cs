namespace CineMeoTic.UserService.API.Services.Implements;

using CineMeoTic.UserService.API.Services.Intefaces;
using System.Threading.Channels;

public class BackgroundJobQueue : IBackgroundJobQueue
{
    private readonly Channel<Func<CancellationToken, Task>> _queue;

    public BackgroundJobQueue()
    {
        _queue = Channel.CreateUnbounded<Func<CancellationToken, Task>>();
    }

    public void Enqueue(Func<CancellationToken, Task> job)
    {
        if (!_queue.Writer.TryWrite(job))
        {
            throw new Exception("Queue is full");
        }
    }

    public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}
