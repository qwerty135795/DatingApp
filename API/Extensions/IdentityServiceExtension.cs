using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtension
    {
        public static IServiceCollection AddIdentityService(this IServiceCollection services
        , IConfiguration config)
        {
            services.AddIdentityCore<AppUser>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<AppRole>().AddRoleManager<RoleManager<AppRole>>()
            .AddEntityFrameworkStores<DataContext>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWTKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                options.Events = new JwtBearerEvents {
                    OnMessageReceived = context => {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.Request.Path;

                        if(!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs")) {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            }
            );
            services.AddAuthorization(builder =>
            {
                builder.AddPolicy("RequiredAdminRole", polBuild =>
                {
                    polBuild.RequireRole("Admin");
                });
                builder.AddPolicy("ModeratePhotoRole", opt =>
                {
                    opt.RequireRole("Admin", "Moderator");
                });
            });
            return services;
        }
    }
}