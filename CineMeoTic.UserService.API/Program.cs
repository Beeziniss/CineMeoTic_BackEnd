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
    app.MapScalarApiReference(options =>
    {
        options.AddPreferredSecuritySchemes("Bearer");

        options.AddHeadContent(
            """
            <script>
            (() => {
                const TOKEN_KEY = 'scalar_access_token';

                const readToken = () => localStorage.getItem(TOKEN_KEY);
                const saveToken = (token) => {
                    if (typeof token === 'string' && token.trim().length > 0) {
                        localStorage.setItem(TOKEN_KEY, token.trim());
                    }
                };

                const resolveAccessToken = (payload) => {
                    if (!payload || typeof payload !== 'object') return null;
                    return payload.accessToken
                        ?? payload.access_token
                        ?? payload?.data?.accessToken
                        ?? payload?.data?.access_token
                        ?? null;
                };

                const tryPopulateAuthInput = () => {
                    const token = readToken();
                    if (!token) return;

                    const input = document.querySelector('input[placeholder*="token" i]');
                    if (!input || input.value === token) return;

                    input.value = token;
                    input.dispatchEvent(new Event('input', { bubbles: true }));
                    input.dispatchEvent(new Event('change', { bubbles: true }));
                };

                const originalFetch = window.fetch.bind(window);
                window.fetch = async (...args) => {
                    const response = await originalFetch(...args);

                    try {
                        const requestUrl = typeof args[0] === 'string' ? args[0] : args[0]?.url;
                        if (requestUrl && /\/api\/login(\?.*)?$/i.test(requestUrl) && response.ok) {
                            const contentType = response.headers.get('content-type') ?? '';
                            if (contentType.includes('application/json')) {
                                const payload = await response.clone().json();
                                const token = resolveAccessToken(payload);
                                if (token) {
                                    saveToken(token);
                                    setTimeout(tryPopulateAuthInput, 0);
                                }
                            }
                        }
                    } catch {
                        // Ignore UI script errors
                    }

                    return response;
                };

                const observer = new MutationObserver(() => tryPopulateAuthInput());
                observer.observe(document.documentElement, { childList: true, subtree: true });

                document.addEventListener('DOMContentLoaded', () => {
                    tryPopulateAuthInput();
                    setTimeout(tryPopulateAuthInput, 400);
                });
            })();
            </script>
            """);
    });
    app.MapGet("/", context =>
    {
        context.Response.Redirect("/scalar");
        return Task.CompletedTask;
    });
    app.UseExceptionHandler("/Error");
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseMiddleware<ResponseWrapperMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
