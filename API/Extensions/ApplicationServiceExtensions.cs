using API.Data;
using API.interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services,
        IConfiguration config)
        {
            services.AddCors();
            services.AddDbContext<DataContext>(opt => opt.UseSqlite(config.GetConnectionString("Default")));
            services.AddScoped<ITokenService, TokenService>();
            return services;
        }
    }
}