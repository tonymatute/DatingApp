using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extentions {
    public static class ApplicationServiceExtensions {
        public static IServiceCollection AddApplicationServices (this IServiceCollection services, IConfiguration config) {
            services.Configure<CloudinarySettings> (config.GetSection ("CloudinarySettings"));
            services.AddScoped<ITokenService, TokenService> ();
            services.AddScoped<IPhotoService, PhotoService> ();
            services.AddScoped<ILikeRepository, LikesRepository>();
            services.AddScoped<IUserRepository, UserRepository> ();
            services.AddScoped<LogUserActivity>();
            services.AddAutoMapper (typeof (AutomapperProfiles).Assembly);
            services.AddDbContext<DataContext> (options => {
                options.UseSqlServer (config.GetConnectionString ("DefaultConnection"));
            });
            return services;
        }
    }
}