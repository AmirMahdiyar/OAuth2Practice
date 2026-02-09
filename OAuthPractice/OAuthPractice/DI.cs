using Microsoft.EntityFrameworkCore;
using OAuthPractice.Common.Utils;
using OAuthPractice.Contracts;
using OAuthPractice.Database;
using OAuthPractice.Services;

namespace OAuthPractice
{
    public static class DI
    {
        public static IServiceCollection ApplyDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<OAuthDbContext>(cfg =>
            {
                cfg.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });


            services.Configure<JwsInformationOptions>(configuration.GetSection("Jws"));
            services.Configure<GoogleInformation>(configuration.GetSection("GoogleInformation"));
            services.Configure<EmailInformation>(configuration.GetSection("EmailInformation"));



            services.AddScoped<IExternalOAuthService, GoogleService>();
            services.AddScoped<IJwsService, JwsService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IOAuthStrategyRepository, OAuthStrategyRepository>();

            return services;

        }
    }
}
