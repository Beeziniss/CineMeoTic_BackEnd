using Carter;
using CineMeoTic.UserService.API;
using CineMeoTic.UserService.API.Middlewares;
using dotenv.net;
using Scalar.AspNetCore;
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

// Add services to the container.
builder.Services.AddDependencyInjections();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapCarter();
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.MapGet("/", context =>
    {
        context.Response.Redirect("/scalar");
        return Task.CompletedTask;
    });
    app.UseExceptionHandler("/Error");
}

app.UseHttpsRedirection();

app.UseMiddleware<ResponseWrapperMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
