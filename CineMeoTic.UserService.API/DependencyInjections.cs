using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Exceptions.Handler;
using Carter;
using CineMeoTic.UserService.API.Data;
using CineMeoTic.UserService.API.Endpoints;
using CineMeoTic.UserService.API.Services;
using CineMeoTic.UserService.API.Services.Implements;
using CineMeoTic.UserService.API.Services.Intefaces;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using StackExchange.Redis;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace CineMeoTic.UserService.API;

public static class DependencyInjections
{
    public static void AddDependencyInjections(this IServiceCollection services)
    {
        services.AddRedisCacheExtension();
        services.AddCustomExceptionHandlerExtension();
        services.AddOpenApiExtension();
        services.AddCarterExtension();
        services.MapsterExtension();
        services.AddCqrs();
        services.AddServices();
        services.AddHttpContextAccessor();
        services.AddAuthenticationExtension();
        services.AddAuthorizationExtension();
        services.AddCorsExtension();
        services.AddDatabase();
    }

    public static void AddRedisCacheExtension(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            string endpoint = Environment.GetEnvironmentVariable("REDIS_END_POINTS") ?? throw new UnconfiguredEnvironmentCustomException("REDIS_END_POINTS is not set in the environment");
            int port = int.Parse(Environment.GetEnvironmentVariable("REDIS_PORT") ?? throw new UnconfiguredEnvironmentCustomException("REDIS_PORT is not set in the environment"));
            string user = Environment.GetEnvironmentVariable("REDIS_USER") ?? throw new UnconfiguredEnvironmentCustomException("REDIS_USER is not set in the environment");
            string password = Environment.GetEnvironmentVariable("REDIS_PASSWORD") ?? throw new UnconfiguredEnvironmentCustomException("REDIS_PASSWORD is not set in the environment");

            ConfigurationOptions options = new()
            {
                EndPoints = { { endpoint, port } },
                User = user,
                Password = password,
                Ssl = false,
                AbortOnConnectFail = true,
                ConnectRetry = 3,
            };

            return ConnectionMultiplexer.Connect(options);
        });

        services.AddScoped<IRedisCacheService, RedisCacheService>();
    }

    public static void AddCustomExceptionHandlerExtension(this IServiceCollection services)
    {
        services.AddExceptionHandler<CustomExceptionHandler>();
    }

    public static void AddOpenApiExtension(this IServiceCollection services)
    {
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Components ??= new();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

                document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "Enter JWT token"
                };

                return Task.CompletedTask;
            });
        });
    }

    public static void AddCarterExtension(this IServiceCollection services)
    {
        services.AddSingleton<ICarterModule, AuthenticationEndpoint>();
        services.AddSingleton<ICarterModule, PermissionEndpoint>();
        services.AddSingleton<ICarterModule, RoleEndpoint>();
        services.AddSingleton<ICarterModule, ProfileEndpoint>();
        services.AddCarter();
    }

    public static void AddCqrs(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(typeof(DependencyInjections).Assembly);
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjections).Assembly);

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticatationService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IJsonWebTokenService, JsonWebTokenService>();
        services.AddScoped<IProfileService, ProfileService>();
    }

    public static void MapsterExtension(this IServiceCollection services)
    {
        services.AddSingleton(provider =>
        {
            TypeAdapterConfig config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Assembly.GetExecutingAssembly());
            return config;
        });
        services.AddScoped<IMapper, ServiceMapper>();
    }

    public static void AddAuthenticationExtension(this IServiceCollection services)
    {
        // Config the Google Identity
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
        })
        //    .AddGoogle(googleOptions =>
        //{
        //    googleOptions.ClientId = Environment.GetEnvironmentVariable("Authentication_Google_ClientId") ?? throw new UnconfiguredEnvironmentCustomException("Google's ClientId property is not set in environment or not found");
        //    googleOptions.ClientSecret = Environment.GetEnvironmentVariable("Authentication_Google_ClientSecret") ?? throw new UnconfiguredEnvironmentCustomException("Google's Client Secret property is not set in environment or not found");

        //})
            .AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                //tự cấp token
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,

                // Các issuer và audience hợp lệ
                //ValidIssuers = [Environment.GetEnvironmentVariable("JWT_ISSUER_PRODUCTION"), "https://localhost:7018"],
                //ValidAudiences = [Environment.GetEnvironmentVariable("JWT_AUDIENCE_PRODUCTION"), Environment.GetEnvironmentVariable("JWT_AUDIENCE_PRODUCTION_BE"), "https://localhost:7018"],

                //ký vào token
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? throw new UnconfiguredEnvironmentCustomException("JWT's Secret Key property is not set in environment or not found"))),

                ClockSkew = TimeSpan.Zero,

                // Set RoleClaimType
                RoleClaimType = ClaimTypes.Role
            };

            // Event for extracting token from query string for WebSocket connections (if needed)
            //opt.Events = new JwtBearerEvents
            //{
            //    OnMessageReceived = context =>
            //    {

            //        // Lấy origin từ request
            //        string? origin = context.Request.Headers.Origin;

            //        // Các origin được phép truy cập
            //        IEnumerable<string> securedOrigins = new[]
            //        {
            //            Environment.GetEnvironmentVariable("FRONTEND_LOCAL_URL") ?? throw new UnconfiguredEnvironmentCustomException("FRONTEND_LOCAL_URL is not set in the environment"),
            //            Environment.GetEnvironmentVariable("FRONTEND_URL") ?? throw new UnconfiguredEnvironmentCustomException("FRONTEND_URL is not set in the environment"),
            //            Environment.GetEnvironmentVariable("BACKEND_URL") ?? throw new UnconfiguredEnvironmentCustomException("BACKEND_URL is not set in the environment")
            //        }.Where(origin => !string.IsNullOrWhiteSpace(origin));

            //        // Kiểm tra xem origin có trong danh sách được phép không
            //        if (string.IsNullOrWhiteSpace(origin) || !securedOrigins.Any(securedOrigin => securedOrigin is not null && securedOrigin.Equals(origin, StringComparison.Ordinal)))
            //        {
            //            return Task.CompletedTask;
            //        }

            //        // Query/Cookie chứa token, sử dụng nó
            //        string? accessToken = context.Request.Query["access_token"];
            //        PathString path = context.HttpContext.Request.Path;

            //        if (!string.IsNullOrWhiteSpace(accessToken))
            //        {
            //            context.Token = accessToken;
            //        }

            //        return Task.CompletedTask;
            //    }
            //};

            // Remove "Bearer " prefix
            // Remove only if the token is present without "Bearer" prefix in development environment or debug mode
            //opt.Events = new JwtBearerEvents
            //{
            //    OnMessageReceived = context =>
            //    {
            //        // Check if the token is present without "Bearer" prefix
            //        if (context.Requests.Headers.ContainsKey("Authorization"))
            //        {
            //            var token = context.Requests.Headers.Authorization.ToString();
            //            if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            //            {
            //                context.Token = token; // Set token without "Bearer" prefix
            //            }
            //        }
            //        return Task.CompletedTask;
            //    }
            //};
        });
    }

    public static void AddAuthorizationExtension(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder();
    }

    public static void AddCorsExtension(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.WithOrigins(Environment.GetEnvironmentVariable("FRONTEND_LOCAL_URL") ?? throw new UnconfiguredEnvironmentCustomException("FRONTEND_URL is not set in the environment"))
                    .WithOrigins(Environment.GetEnvironmentVariable("FRONTEND_URL") ?? throw new UnconfiguredEnvironmentCustomException("FRONTEND_URL is not set in the environment"))
                      .WithOrigins(Environment.GetEnvironmentVariable("BACKEND_URL") ?? throw new UnconfiguredEnvironmentCustomException("BACKEND_URL is not set in the environment"))
                      .AllowCredentials()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });
    }

    public static void AddDatabase(this IServiceCollection services)
    {
        services.AddDbContextPool<UserDbContext>(options =>
        {
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            options.UseNpgsql(Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING") ?? throw new UnconfiguredEnvironmentCustomException("POSTGRES_CONNECTION_STRING is not set in the environment"));
        });

        services.AddScoped<IUserDbContext>(provider => provider.GetRequiredService<UserDbContext>());
    }
}
