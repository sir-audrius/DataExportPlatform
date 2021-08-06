using DataExportPlatform.PushNotifications;
using DataExportPlatform.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System.Text.Json.Serialization;

namespace DataExportPlatform
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
            services
                .AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    var enumConverter = new JsonStringEnumConverter();
                    options.JsonSerializerOptions.Converters.Add(enumConverter);
                });
            services
                .AddSignalR()
                .AddJsonProtocol(options =>
                {
                    var enumConverter = new JsonStringEnumConverter();
                    options.PayloadSerializerOptions.Converters.Add(enumConverter);
                });

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddDbContext<DataExportContext>(
                options => options.UseSqlServer(@"Server=(local);Database=DataExport;User ID=sa;Password=ABcd1234"));

            services.AddScoped<IDataExportRegistrationHandler, DataExportRegistrationHandler>();
            services.AddScoped<IDataExportListReader, DataExportListReader>();
            services.AddSingleton<IMessageBus, MessageBus>();
            services.AddScoped<IPushNotificationService, PushNotificationService>();
            services.AddScoped<IDataExportUpdatedHandler, DataExportUpdatedHandler>();

            var factory = new ConnectionFactory() { HostName = "localhost", DispatchConsumersAsync = true };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "DataExportRegistered",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

            channel.QueueDeclare(queue: "DataExportUpdated",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

            services.AddSingleton(channel);
            services.AddHostedService<BackgoundMessageListener>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(" / Error");
            }

            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<DataExportHub>("/exportHub");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<DataExportContext>();
                context.Database.EnsureCreated();
            }
        }
    }
}
