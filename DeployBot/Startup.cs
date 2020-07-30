using DeployBot.Features.Authentication;
using DeployBot.Features.Deployments.Services;
using DeployBot.Features.Releases.Services;
using DeployBot.Features.Shared.Exceptions;
using DeployBot.Features.Shared.Services;
using DeployBot.Infrastructure.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DeployBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var config = new ServiceConfiguration(Configuration);

            services.AddControllers();
            services.AddDbContext<DeployBotDbContext>(opts =>
            {
                opts.UseSqlite(config.ConnectionString);
            });
            services.AddAuthentication(ApiKeyAuthenticationOptions.DefaultScheme).AddApiKeyAuthentication(opt => {});

            services.AddSingleton<ServiceConfiguration>();
            services.AddScoped<ReleaseService>();
            services.AddScoped<DeploymentService>();
            services.AddTransient<ReleaseDeploymentProcessor>();

            services.AddTransient<HttpExceptionMiddleware>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs\\{Date}.txt");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<HttpExceptionMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            UpdateDatabase(app);
        }

        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();

            using var context = serviceScope.ServiceProvider.GetService<DeployBotDbContext>();
            context.Database.Migrate();
        }
    }
}
