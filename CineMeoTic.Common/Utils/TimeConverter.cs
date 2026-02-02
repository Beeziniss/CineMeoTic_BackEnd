namespace CineMeoTic.Common.Utils;

public static class TimeConverter
{
    public static string FormatOffsetToString(this DateTimeOffset dateTimeOffset, string pattern = "dd-MM-yyyy HH:mm:ss")
    {
        return dateTimeOffset.ToString(pattern);
    }

    public static string FormatDateTimeToString(this DateTime dateTime, string pattern = "dd-MM-yyyy HH:mm:ss")
    {
        return dateTime.ToString(pattern);
    }


}
