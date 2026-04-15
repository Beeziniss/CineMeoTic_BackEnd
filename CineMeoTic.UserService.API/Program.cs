using BuildingBlocks.Exceptions.Handler;
using Carter;
using CineMeoTic.UserService.API;
using CineMeoTic.UserService.API.Middlewares;
using CineMeoTic.UserService.API.Services;
using dotenv.net;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
{
    DotEnv.Load(new DotEnvOptions(envFilePaths: [".env.dev"]));
}
else if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
{
    DotEnv.Load(new DotEnvOptions(envFilePaths: [".env.prod"]));
}
else
{
    return;
}

builder.AddServiceDefaults();

builder.Host.UseSerilog((hostingContext, LoggerConfiguration) =>
{
    LoggerConfiguration
        //.Enrich.With(new CustomDateFormatter())
        .ReadFrom.Configuration(hostingContext.Configuration);
        //.WriteTo.Seq(Environment.GetEnvironmentVariable("SEQ_URL")!);
});

builder.Services.AddCarter();

builder.Services.AddScoped<IJsonWebTokenService, JsonWebTokenService>();
// Add services to the container.
builder.Services.AddDependencyInjections();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapCarter();
    app.MapOpenApi();
    app.UseExceptionHandler("/Error");
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<ResponseWrapperMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
