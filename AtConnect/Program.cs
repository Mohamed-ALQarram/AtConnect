
using AtConnect.BLL.Interfaces;
using AtConnect.BLL.Services;
using AtConnect.Core.Interfaces;
using AtConnect.DAL.Data;
using AtConnect.DAL.Repositories;
using AtConnect.Middleware;
using AtConnect.BLL.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;

//using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace AtConnect
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.Configure<JwtOptions>(
                builder.Configuration.GetSection("AtConnect:Jwt"));
            builder.Services.Configure<DatabaseOptions>(
                builder.Configuration.GetSection("ConnectionStrings"));
            builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("AtConnect:Smtp"));
            var configuration = builder.Configuration
                .AddEnvironmentVariables()
                .Build();

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            {
                var dbOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
                options.UseNpgsql(dbOptions.AtConnectPostgresConnection);
            });
            //Adding CROS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });


            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            var JwtOptions = builder.Configuration.GetSection("AtConnect:Jwt").Get<JwtOptions>();
            var TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidIssuer = JwtOptions?.Issuer,

                ValidateAudience = false,
                ValidAudience = JwtOptions?.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtOptions?.SigningKey ?? "")),
                RequireExpirationTime = false,
                ValidateLifetime = true

            };
            //Avoid duplicates creation when injecting in DI container
            builder.Services.AddSingleton(TokenValidationParameters);
            builder.Services.AddAuthentication()
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = TokenValidationParameters;      
                }); 
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            ////if (app.Environment.IsDevelopment())
            ////{
            ////    app.UseSwagger();
            ////    app.UseSwaggerUI();
            ////}
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AtConnect API V1");
            });
            app.UseCors("AllowAll");
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseHttpsRedirection();

            app.UseRouting(); 

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
