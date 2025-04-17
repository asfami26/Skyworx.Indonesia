

using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Skyworx.Common.Command;
using Skyworx.Common.Config;
using Skyworx.Common.Exception;
using Skyworx.Common.Validation;
using Skyworx.Repository.DataContext;
using Skyworx.Service.Impl;
using Skyworx.Service.Interface;

namespace Skyworx.PengajuanKredit;

public class Startup
{
    public IConfiguration Configuration { get; }
    public ConfigExtension AppConfig { get; }

    public Startup(IConfiguration configuration)
    {
        var builder = new ConfigurationBuilder()
            .AddYamlFile("generalConfig.yml", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        Configuration = builder.Build();
        
        AppConfig = new ConfigExtension();
        Configuration.Bind("generalConfig", AppConfig);
        
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
    }

    [Obsolete("Obsolete")]
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(AppConfig);
        services.AddDbContext<DataContext>(options =>
            options.UseNpgsql(AppConfig.ConnectionStrings.Postgres));
        
        services.AddAutoMapper(typeof(KreditService).Assembly);
        services.AddScoped<IKreditService, KreditService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddTransient<IValidator<CreatePengajuanKreditCommand>, KreditRequestValidator<CreatePengajuanKreditCommand>>();
        services.AddTransient<IValidator<CalculateAngsuranCommand>, KreditRequestValidator<CalculateAngsuranCommand>>();
        
        var key = Encoding.ASCII.GetBytes(AppConfig.Jwt.SecretKey);
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = AppConfig.Jwt.Issuer,
                ValidAudience = AppConfig.Jwt.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Skyworx API", Version = "v1" });

            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });

            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    new string[] {}
                }
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        Log.Information("Skyworx API Starting...");
        
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Skyworx API v1");
            c.RoutePrefix = string.Empty;
        });
        
        app.UseRouting();
        app.UseMiddleware<GlobalExceptionMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        
        AppDomain.CurrentDomain.ProcessExit += (_, _) => Log.CloseAndFlush();
    }
}