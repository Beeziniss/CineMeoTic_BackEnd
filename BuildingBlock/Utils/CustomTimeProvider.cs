namespace BuildingBlocks.Utils;

public static class CustomTimeProvider
{
    public static Func<DateTimeOffset> UtcNowProvider { get; set; } = () => DateTimeOffset.UtcNow;
    public static DateTimeOffset UtcNow => UtcNowProvider();

    public static DateTime GetUtcPlus7Time()
    {
        return DateTime.UtcNow.AddHours(7);
    }

    public static DateTimeOffset GetUtcPlus7TimeOffset()
    {
        return UtcNow.AddHours(7);

        // Neon (Database Service) doesn't support others timezones than UTC Now (+0)
        //return DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7));
    }

    public static DateOnly GetUtcPlus7DateOnly()
    {
        DateTime utcPlus7Now = DateTime.UtcNow.AddHours(7);
        return DateOnly.FromDateTime(utcPlus7Now);
    }
}
