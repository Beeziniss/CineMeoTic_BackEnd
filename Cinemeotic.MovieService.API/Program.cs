using Cinemeotic.MovieService.API.Middlewares;
using dotenv.net;
using Serilog;
using Carter;
using Cinemeotic.MovieService.API;

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

// Add services to the container.
builder.Host.UseSerilog((hostingContext, LoggerConfiguration) =>
{
    LoggerConfiguration
        //.Enrich.With(new CustomDateFormatter())
        .ReadFrom.Configuration(hostingContext.Configuration);
    //.WriteTo.Seq(Environment.GetEnvironmentVariable("SEQ_URL")!);
});

builder.Services.AddCarter();

builder.Services.AddDependencyInjections();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
