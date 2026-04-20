using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Exceptions.Handler;
using Carter;
using Cinemeotic.MovieService.API.Data;
using Cinemeotic.MovieService.API.Data.Enums;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Cinemeotic.MovieService.API;

public static class DependencyInjections
{
    public static void AddDependencyInjections(this IServiceCollection services)
    {
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

    public static void AddCustomExceptionHandlerExtension(this IServiceCollection services)
    {
        services.AddExceptionHandler<CustomExceptionHandler>();
    }

    public static void AddOpenApiExtension(this IServiceCollection services)
    {
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();
    }

    public static void AddCarterExtension(this IServiceCollection services)
    {
        //services.AddSingleton<ICarterModule, >();
        //services.AddSingleton<ICarterModule, >();
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
        //services.AddScoped<IMovieService, MovieService>();
        //services.AddScoped<IGenreService, GenreService>();
        //services.AddScoped<ICommentService, CommentService>();
        //services.AddScoped<IMovieRatingService, MovieRatingService>();
    }

    public static void MapsterExtension(this IServiceCollection services)
    {
        services.AddSingleton(provider =>
        {
            TypeAdapterConfig config = TypeAdapterConfig.GlobalSettings;
            config.Scan(typeof(DependencyInjections).Assembly);
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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? throw new Exception("JWT's Secret CancelMode property is not set in environment or not found"))),

                ClockSkew = TimeSpan.Zero,

                // Đặt RoleClaimType
                RoleClaimType = ClaimTypes.Role
            };

            // Cấu hình SignalR để đọc token
            opt.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    // Lấy origin từ request
                    string? origin = context.Request.Headers.Origin;

                    // Các origin được phép truy cập
                    IEnumerable<string> securedOrigins = new[]
                    {
                        Environment.GetEnvironmentVariable("FRONTEND_LOCAL_URL") ?? throw new UnconfiguredEnvironmentCustomException("FRONTEND_LOCAL_URL is not set in the environment"),
                        Environment.GetEnvironmentVariable("FRONTEND_URL") ?? throw new UnconfiguredEnvironmentCustomException("FRONTEND_URL is not set in the environment"),
                        Environment.GetEnvironmentVariable("BACKEND_URL") ?? throw new UnconfiguredEnvironmentCustomException("BACKEND_URL is not set in the environment")
                    }.Where(origin => !string.IsNullOrWhiteSpace(origin));

                    // Kiểm tra xem origin có trong danh sách được phép không
                    if (string.IsNullOrWhiteSpace(origin) || !securedOrigins.Any(securedOrigin => securedOrigin is not null && securedOrigin.Equals(origin, StringComparison.Ordinal)))
                    {
                        return Task.CompletedTask;
                    }

                    // Query chứa token, sử dụng nó
                    string? accessToken = context.Request.Query["access_token"];
                    PathString path = context.HttpContext.Request.Path;

                    // Các segment được bảo mật
                    //IEnumerable<string> securedSegments = new[]
                    //{
                    //    Environment.GetEnvironmentVariable("EKOFY_SIGNALR_CHAT_URL") ?? throw new UnconfiguredEnvironmentCustomException("Not set EKOFY_SIGNALR_CHAT_URL in the environment"),
                    //    Environment.GetEnvironmentVariable("EKOFY_SIGNALR_NOTIFICATION_URL")?? throw new UnconfiguredEnvironmentCustomException("Not set EKOFY_SIGNALR_NOTIFICATION_URL in the environment"),
                    //}
                    //.Where(url => !string.IsNullOrWhiteSpace(url))
                    //.Select(url => new Uri(url!).AbsolutePath); // <-- Chỉ lấy phần path, ví dụ "/hub/chat"

                    //if (!string.IsNullOrWhiteSpace(accessToken) &&
                    //    securedSegments.Any(segment => path.StartsWithSegments(segment, StringComparison.Ordinal)))
                    //{
                    //}
                    if (!string.IsNullOrWhiteSpace(accessToken))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };

            // Remove "Bearer " prefix
            // Chỉ remove Bearer prefix khi đang trong môi trường phát triển hoặc debug
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
        services.AddAuthorizationBuilder().AddPolicy("GoogleOrJwt", policy =>
        {
            policy.AddAuthenticationSchemes(GoogleDefaults.AuthenticationScheme, JwtBearerDefaults.AuthenticationScheme);
            policy.RequireAuthenticatedUser();
        });
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
        //services.AddMarten(options =>
        //{
        //    options.Connection(Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING") ?? throw new UnconfiguredEnvironmentCustomException("POSTGRES_CONNECTION_STRING is not set in the environment"));
        //    options.Schema.For<Movie>().Identity(m => m.Id);
        //    options.Schema.For<Genre>().Identity(g => g.Id);
        //    options.Schema.For<MovieCredit>()
        //        .Identity(mc => mc.Id)
        //        .Duplicate(mc => mc.Role);
        //    options.Schema.For<Comment>()
        //        .Identity(c => c.Id)
        //        .Duplicate(c => c.MovieId)
        //        .Duplicate(c => c.UserId)
        //        .Duplicate(c => c.ParentCommentId)
        //        .Duplicate(c => c.RootCommentId);
        //    options.Schema.For<MovieRating>()
        //        .Identity(r => r.Id)
        //        .Duplicate(r => r.MovieId)
        //        .Duplicate(r => r.UserId);
        //    options.DatabaseSchemaName = Environment.GetEnvironmentVariable("POSTGRES_DB_SCHEMA") ?? throw new UnconfiguredEnvironmentCustomException("POSTGRES_DB_SCHEMA is not set in the environment");
        //    options.UseSystemTextJsonForSerialization(enumStorage: EnumStorage.AsString, configure: serializerOptions =>
        //    {
        //        serializerOptions.Converters.Add(new EnumMemberJsonConverter<Role>());
        //    });
        //    options.AutoRegister();
        //}).UseLightweightSessions();
    }
}
