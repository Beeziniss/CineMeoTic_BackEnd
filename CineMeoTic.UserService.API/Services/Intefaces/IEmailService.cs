using BuildingBlocks.Models;

namespace CineMeoTic.UserService.API.Services.Intefaces;

public interface IEmailService
{
    void Send(EmailTemplateType templateType, string toEmail, params string[] parameters);
}
