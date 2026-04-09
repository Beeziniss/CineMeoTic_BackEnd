namespace CineMeoTic.Common.Utils;

public static class CustomString
{
    /// <summary>
    /// Converts the specified string to lowercase using the invariant culture after removing leading and trailing
    /// whitespace.
    /// </summary>
    /// <param name="content">The string to normalize and convert to lowercase. If null or empty, the result will be an empty string.</param>
    /// <returns>A new string that is the lowercase, trimmed version of the input. Returns an empty string if the input is null
    /// or empty.</returns>
    public static string NormalizeLower(this string content)
    {
        return content.Trim().ToLowerInvariant();
    }
}
