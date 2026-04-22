namespace CineMeoTic.UserService.API.Services.Intefaces;

public interface IRedisCacheService
{
    Task<string?> GetStringAsync(string key);
    Task SetStringAsync(string key, string value, TimeSpan? expiry = null);
}
