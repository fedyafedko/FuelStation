using FuelStation.ActionFilters;
using FuelStation.BLL.Profiles;
using FuelStation.BLL.Services.Auth;
using FuelStation.BLL.Services.Interfaces.Auth;
using FuelStation.Common.Models.Configs;
using FuelStation.DAL.Contexts;
using FuelStation.DAL.Entities;
using FuelStation.DAL.Repositories;
using FuelStation.DAL.Repositories.Interfaces;
using FuelStation.Extensions;
using FuelStation.Seeding.Extentions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configs
var jwtConfig = new JwtConfig();

builder.Services.AddConfigs(builder.Configuration, opt =>
    opt.AddConfig<JwtConfig>(out jwtConfig));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors());

// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();

// Auth
builder.Services.AddIdentity<User, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider);

var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret)),
    ValidIssuer = jwtConfig.Issuer,
    ValidAudience = jwtConfig.Audience,
    ClockSkew = jwtConfig.ClockSkew
};
builder.Services.AddSingleton(tokenValidationParameters);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = tokenValidationParameters;
    });

// Utility
builder.Services.AddCors();
builder.Services.AddSeeding();

// Mapper
builder.Services.AddAutoMapper(typeof(UserProfile));

builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers(opt => opt.Filters.Add<CustomExceptionFilterAttribute>());

var app = builder.Build();

app.MigrateDatabase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FuelStation API V1");
    });
}

app.UseHttpsRedirection();

app.UseCors(x => x.AllowAnyHeader()
    .AllowAnyOrigin()
    .AllowAnyMethod());

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
