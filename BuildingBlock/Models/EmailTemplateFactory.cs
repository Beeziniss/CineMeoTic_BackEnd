namespace BuildingBlocks.Models;

public static class EmailTemplateFactory
{
    public static Func<string[], string> GetTemplate(EmailTemplateType type)
    {
        return type switch
        {
            EmailTemplateType.ForgotPassword => EmailTemplate.ForgotPassword,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public static string GetSubject(EmailTemplateType type)
    {
        return type switch
        {
            EmailTemplateType.ForgotPassword => EmailTemplate.SubjectForgotPassword,
            _ => "CineMeoTic Notification"
        };
    }
}

public enum EmailTemplateType
{
    ForgotPassword
}

public static class EmailTemplate
{
    public const string SubjectForgotPassword = "CineMeoTic - Forgot Password";

    /// <summary>
    /// Email template for forgot password OTP.
    /// Expected parameters:
    /// 0 - OTP Code
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static string ForgotPassword(string[] parameters)
    {
        string otp = parameters[0];

        return @$"<!doctype html>
			<html lang=""en"">
				<head>
					<meta charset=""UTF-8"" />
					<title>Forgot Password - CineMeoTic</title>
				</head>

				<body
					style="";
						margin: 0;
						padding: 0;
						font-family: &quot;Helvetica Neue&quot;, Helvetica, Arial, sans-serif;
						color: #333;
						background-color: #fff;
					""
				>
					<div
						class=""background-cinemetic""
						style=""background: linear-gradient(45deg, #3b54ea 0%, #ab4ee5 100%); padding: 40px 0""
					>
						<div
							class=""container""
							style=""
								margin: 0 auto;
								padding: 64px 56px;
								width: 100%;
								max-width: 600px;
								background-color: #ffffff;
								border-radius: 32px;
								line-height: 1.8;
							""
						>
							<div class=""header"" style=""text-align: center"">
								<img src=""https://res.cloudinary.com/dofnn7sbx/image/upload/v1759760383/logo_yqjeui.png"" alt=""CineMeoTic Logo"" />
							</div>

							<p
								class=""separator""
								style=""height: 1px; width: 100%; background-color: #d9d9d9; margin: 32px 0""
							></p>

							<strong>Dear Viewer,</strong>
							<p>
								We have received a password reset request for your CineMeoTic account. For security purposes,
								please use the following One-Time Password (OTP) to reset your password.
								<br />
								<b>Your Password Reset OTP code is:</b>
							</p>
							<h2
								class=""otp""
								style=""
									background: linear-gradient(to right, #3b54ea 0%, #ab4ee5 100%);
									margin: 0 auto;
									width: max-content;
									padding: 0 10px;
									color: #fff;
									border-radius: 4px;
								""
							>
								{otp}
							</h2>
							<p style=""font-size: 0.9em"">
								<strong>This OTP is valid for 10 minutes.</strong>
								<br />
								<br />
								If you did not request a password reset, please disregard this message and ensure your account security. 
								Please ensure the confidentiality of your OTP and do not share it with anyone.<br />
								<strong>Do not forward or give this code to anyone.</strong>
								<br />
								<br />
								<strong>Thank you for using Ekofy.</strong>
								<br />
								<br />
								Best regards,
								<br />
								<strong>The CineMeoTic Team</strong>
							</p>
						</div>
					</div>
				</body>
			</html>";
    }
}
