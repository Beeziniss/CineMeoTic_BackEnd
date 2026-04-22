using CineMeoTic.UserService.API.Services.Intefaces;
using Serilog;

namespace CineMeoTic.UserService.API.Services;

public class BackgroundWorker(IBackgroundJobQueue queue) : BackgroundService
{
    private readonly IBackgroundJobQueue _queue = queue;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var job = await _queue.DequeueAsync(stoppingToken);

            try
            {
                await job(stoppingToken);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Background job failed");
            }
        }
    }
}
