
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using UserSystem.API.Data;
using UserSystem.API.Models.Helpers;
using UserSystem.API.Infrastructure.Helpers;
using UserSystem.API.Infrastructure.Middlewares;
using UserSystem.API.Interfaces.Services.Users;
using UserSystem.API.Interfaces.Repositories.Users;
using UserSystem.API.Services.Users;
using UserSystem.API.Repositories.Users;
using UserSystem.API.Interfaces.Services.Security;

namespace UserSystem.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder);

            var app = builder.Build();
            Configure(app);
        }


        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            var services = builder.Services;
            var appSettingConfig = builder.Configuration.GetSection("ApplicationSettings");
            var appSettings = appSettingConfig.Get<AppSettings>();

            services.AddDbContext<UserSystemDBContext>();
            services.AddControllers();

            // configure automapper with all automapper profiles from this assembly
            services.AddAutoMapper(typeof(Program));

            // configure strongly typed settings object
            services.Configure<AppSettings>(appSettingConfig);

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.JwtToken.Key)),
                    ValidateIssuer = true,
                    ValidIssuer = appSettings.JwtToken.Issuer,
                    ValidateAudience = true,
                    ValidAudience = appSettings.JwtToken.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "User System API",
                    Version = "v1"
                });
                // Include 'SecurityScheme' to use JWT Authentication
                OpenApiSecurityScheme jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer jhfdkj.jkdsakjdsa.jkdsajk\"",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });
            });

            // configure DI for application services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddSingleton<IJwtHelper, JwtHelper>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ITokenReaderService, TokenReaderService>();
            services.AddTransient<IUserRepository, UserRepository>();
        }
        private static void Configure(WebApplication app)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapControllers();
            app.Run();
        }
    }
}
