
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;

using Microsoft.AspNetCore.Identity;

using Microsoft.OpenApi.Models;
using TaskManagerAPI.Repositories;
using TaskManagerAPI.Services;
using TaskManagerAPI.Services.impl;
using TaskManagerAPI.Repositories.impl;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký các Service vào DI Container
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<ITaskService, TaskService>(); // Register TaskService
// Cấu hình PostgreSQL
builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình Identity với PostgreSQL
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

// Cấu hình JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Thêm các dịch vụ khác
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", builder =>
        builder.WithOrigins("http://localhost:5173")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        );
});

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization Example : 'Bearer eyeleieieekeieieie",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement(){
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "outh2",
                Name="Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });

});

var app = builder.Build();


// Tạo vai trò mặc định nếu chưa tồn tại
// Apply EF Core Migrations and Seed Roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Apply Migrations
        var context = services.GetRequiredService<DataContext>();
        context.Database.Migrate(); // Applies pending migrations

        // Seed Roles
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        string[] roleNames = ["Admin", "User"];

        foreach (var role in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(role);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
    catch (Exception ex)
    {
        // Log error - consider a proper logging framework
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database migration or seeding.");
        // Optionally, rethrow or handle appropriately depending on desired startup behavior on error
    }
}


// Tạo vai trò mặc định nếu chưa tồn tại
// var scope = app.Services.CreateScope(); // Commented out or remove original seeding block start
// var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(); // Commented out
// string[] roleNames = ["Admin", "User"]; // Commented out

// foreach (var role in roleNames) // Commented out
// { // Commented out
//     var roleExist = await roleManager.RoleExistsAsync(role); // Commented out
//     if (!roleExist) // Commented out
//     { // Commented out
//         await roleManager.CreateAsync(new IdentityRole(role)); // Commented out
//     } // Commented out
// } // Commented out or remove original seeding block end

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

// app.UseCors(options =>
// {

//     options.AllowAnyHeader();
//     options.AllowAnyMethod();
//     options.AllowAnyOrigin();
// });
// Middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
