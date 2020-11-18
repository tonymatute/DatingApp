using API.Extentions;
using API.MiddleWare;
using API.SignalR;
using EmailService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API {
    public class Startup {
        private readonly IConfiguration _config;

        public Startup (IConfiguration config) {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            services.AddApplicationServices (_config);
            services.AddControllers ();
            services.AddCors ();
            services.AddIdentityServices (_config);
            services.AddSignalR ();
            var emailConfig = _config
                .GetSection ("EmailConfiguration")
                .Get<EmailConfiguration> ();
            services.AddSingleton (emailConfig);
            services.AddScoped<IEmailSender, EmailSender> ();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env) {
            app.UseMiddleware<ExceptionMiddleware> ();

            app.UseHttpsRedirection ();

            app.UseRouting ();

            app.UseCors (policy => policy.AllowAnyHeader ()
                .AllowAnyMethod ()
                .AllowCredentials ()
                .WithOrigins ("https://localhost:4200"));

            app.UseAuthentication ();
            app.UseAuthorization ();

            app.UseDefaultFiles ();
            app.UseStaticFiles ();

            app.UseEndpoints (endpoints => {
                endpoints.MapControllers ();
                endpoints.MapHub<PresenceHub> ("hubs/presence");
                endpoints.MapHub<MessageHub> ("hubs/message");
                endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}