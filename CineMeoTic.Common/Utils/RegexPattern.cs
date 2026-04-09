namespace CineMeoTic.Common.Utils;

public static class RegexPattern
{
    /// <summary>
    /// Gets the regular expression pattern for validating passwords.
    /// Message: The pattern requires at least one lowercase letter, one uppercase letter, one digit, and a minimum length of 6 characters.
    /// </summary>
    public static string PasswordRegexPattern
    {
        get
        {
            return @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$";
        }
    }

    /// <summary>
    /// Gets the regular expression pattern for validating Vietnamese phone numbers.
    /// Message: The pattern requires the phone number to start with a valid Vietnamese carrier code.
    /// 32|33|34|35|36|37|38|39|86|96|97|98  # Viettel
    /// 81|82|83|84|85|88|91|94              # Vinaphone
    /// 70|76|77|78|79|89|90|93              # Mobifone
    /// 52|56|58|92                          # Vietnamobile
    /// 059|099                              # Beeline/Gmobile
    /// 095                                  # S‑Fone (WTF)
    /// </summary>
    public static string PhoneNumberRegexPattern
    {
        get
        {
            return @"^(0|\+84)(32|33|34|35|36|37|38|39|86|96|97|98|81|82|83|84|85|88|91|94|70|76|77|78|79|89|90|93|52|56|58|92|059|099|095)[0-9]{7}$";
        }
    }
}
