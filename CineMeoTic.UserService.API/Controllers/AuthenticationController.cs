using CineMeoTic.UserService.API.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineMeoTic.UserService.API.Controllers;

[Route("api/authentication")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class AuthenticationController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [AllowAnonymous, HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        string result = await sender.Send(request);

        return Ok(result);
    }
}
