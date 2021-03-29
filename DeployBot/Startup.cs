using DeployBot.Features.Authentication;
using DeployBot.Features.Deployments.Services;
using DeployBot.Features.Shared.Exceptions;
using DeployBot.Features.Shared.Services;
using DeployBot.Infrastructure.Database;
using LiteDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;

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
            services.AddAuthentication(BasicAuthenticationOptions.DefaultScheme)
                .AddBasicAuthentication(opt => { });

            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder(BasicAuthenticationOptions.DefaultScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            services.AddSingleton<ServiceConfiguration>();

            services.AddScoped<LiteDatabase>(ctx => new LiteDatabase(ctx.GetRequiredService<ServiceConfiguration>().ConnectionString));
            services.AddScoped<LiteDbRepository<Applications>>();
            services.AddScoped<LiteDbRepository<Deployment>>();
            services.AddScoped<ProductService>();
            services.AddScoped<DeploymentService>();

            services.AddHostedService<ReleaseDeploymentProcessor>();

            services.AddTransient<HttpExceptionMiddleware>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile(Path.Combine("Logs", "{Date}.txt"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<HttpExceptionMiddleware>();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            if (env.IsDevelopment())
            {
                app.UseSpa(spa =>
                {
                    spa.Options.SourcePath = Path.Combine("ClientApp");
                    spa.Options.DevServerPort = 3000;

                    spa.UseReactDevelopmentServer("dev");
                });
            }
        }
    }
}
