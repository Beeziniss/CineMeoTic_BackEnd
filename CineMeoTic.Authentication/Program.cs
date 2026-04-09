using CineMeoTic.AuthenticationService;
using CineMeoTic.AuthenticationService.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
//builder.Services.AddIdentity<IdentityUser, IdentityRole>()
//    .AddEntityFrameworkStores<AuthenticationDbContext>()
//    .AddRoles<IdentityRole>();
builder.Services.AddJWTService(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(options =>
//{
//    options.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Title = "My API",
//        Version = "v1",
//        Description = "A comprehensive API for my application",
//        Contact = new OpenApiContact
//        {
//            Name = "Your Name",
//            Email = "your.email@example.com",
//            Url = new Uri("https://yourwebsite.com")
//        }
//    });

//    // Enable XML documentation (optional but recommended)
//    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
//    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
//    options.IncludeXmlComments(xmlPath);
//});
builder.Services.AddAuthorization();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/docs");
}

app.UseHttpsRedirection();

app.UseAuthentication().UseAuthorization();

app.MapControllers();

app.Run();
