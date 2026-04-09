using System.Globalization;
using System.Text;

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

    /// <summary>
    /// Converts the specified string to a lowercase, unsigned form by removing diacritical marks and replacing special
    /// characters.
    /// </summary>
    /// <remarks>This method is useful for generating normalized, ASCII-compatible representations of strings
    /// for search, comparison, or URL generation. The method specifically replaces the character 'đ' with 'd' after
    /// normalization.</remarks>
    /// <param name="term">The input string to convert. Can be null or empty.</param>
    /// <returns>A lowercase string with diacritical marks removed and special characters replaced. Returns an empty string if
    /// the input is null or empty.</returns>
    public static string ToUnsigned(this string term)
    {
        if (string.IsNullOrEmpty(term))
        {
            return string.Empty;
        }

        string normalizedString = term.Normalize(NormalizationForm.FormD);

        StringBuilder stringBuilder = new();
        foreach (char character in normalizedString)
        {
            UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(character);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant().Replace('đ', 'd');
    }

    /// <summary>
    /// Converts the specified string to a URL-friendly slug by lowercasing and replacing spaces with hyphens.
    /// </summary>
    /// <param name="term">The string to convert to a slug. If null or empty, an empty string is returned.</param>
    /// <returns>A slug representation of the input string, with all characters in lowercase and spaces replaced by hyphens.
    /// Returns an empty string if the input is null or empty.</returns>
    public static string ToSlug(this string term)
    {
        if (string.IsNullOrEmpty(term))
        {
            return string.Empty;
        }

        return term.ToLowerInvariant().Replace(" ", "-").Trim('-');
    }
}
