namespace CineMeoTic.UserService.API.Services.Intefaces;

public interface IBackgroundJobQueue
{
    void Enqueue(Func<CancellationToken, Task> job);
    Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
}