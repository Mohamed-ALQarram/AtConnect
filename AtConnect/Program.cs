
using AtConnect.BLL.Interfaces;
using AtConnect.BLL.Options;
using AtConnect.BLL.Services;
using AtConnect.Core.Interfaces;
using AtConnect.DAL.Data;
using AtConnect.DAL.Repositories;
using AtConnect.Middlewares;
using AtConnect.SignalR_Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AtConnect
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add environment variables to configuration
            builder.Configuration.AddEnvironmentVariables();
            
            // Configure options
            builder.Services.Configure<JwtOptions>(
                builder.Configuration.GetSection("AtConnect:Jwt"));
            builder.Services.Configure<DatabaseOptions>(
                builder.Configuration.GetSection("AtConnect:Database"));
            builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("AtConnect:Smtp"));

            // Add services to the container.

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                });

            //Adding SignalR
            builder.Services.AddSignalR();
            builder.Services.AddSingleton<IUserConnectionManager, InMemoryUserConnectionManager>();
            

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            {
                var dbOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
                options.UseSqlServer(dbOptions.AtConnectSqlServerConnection);
            });
            // Adding CORS
            // WARNING: AllowAnyOrigin is insecure for production. Specify allowed origins instead.
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
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<IRequestService,RequestService >();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<INotificationService, NotificationService>();


            var JwtOptions = builder.Configuration.GetSection("AtConnect:Jwt").Get<JwtOptions>();
            
            // Validate JWT configuration is loaded properly
            if (JwtOptions == null || string.IsNullOrEmpty(JwtOptions.SigningKey))
            {
                throw new InvalidOperationException(
                    "JWT configuration is missing or invalid. Ensure 'AtConnect:Jwt:SigningKey' is configured in appsettings.json or environment variables.");
            }
            
            var TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidIssuer = JwtOptions.Issuer,

                ValidateAudience = false,
                ValidAudience = JwtOptions.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtOptions.SigningKey)),
                RequireExpirationTime = false,
                ValidateLifetime = true

            };
            //Avoid duplicates creation when injecting in DI container
            builder.Services.AddSingleton(TokenValidationParameters);
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            ///
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AtConnect API V1");
            });
            app.UseCors("AllowAll");
            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseHttpsRedirection();
            app.UseRouting(); 

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<AtConnectHub>("/Hubs/AtConnect"); 


            app.Run();
        }
    }
}
