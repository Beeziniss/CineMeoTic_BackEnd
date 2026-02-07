using BuildingBlocks.Exceptions.Handler;
using Carter;
using CineMeoTic.UserService.API.Middlewares;
using CineMeoTic.UserService.API.Services;
using dotenv.net;
using Marten;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();

builder.AddServiceDefaults();

builder.Services.AddMarten(options =>
{
    options.Connection(Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING") ?? throw new Exception("No connection string connect to user database service"));
    options.DatabaseSchemaName = Environment.GetEnvironmentVariable("POSTGRES_DB_SCHEMA") ?? throw new Exception("The database user service schema name does not exist");
    options.AutoRegister();
})
    .UseLightweightSessions();
    //.UseNpgsqlDataSource();

builder.Services.AddCarter();

builder.Services.AddScoped<IJsonWebTokenService, JsonWebTokenService>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
