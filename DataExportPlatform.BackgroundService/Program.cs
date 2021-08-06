using DataExportPlatform.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataExportPlatform.BackgroundService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            await CreateHostBuilder(args)
                .Build()
                .RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) => {
                    for (int i = 0; i < 10; i++)
                    {
                        try
                        {
                            var factory = new ConnectionFactory() { HostName = "rabbit", DispatchConsumersAsync = true };
                            var connection = factory.CreateConnection();
                            var channel = connection.CreateModel();

                            services.AddSingleton(channel);
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Thread.Sleep(TimeSpan.FromSeconds(10));
                        }
                    }
                    
                    services.AddHostedService<BackgoundProcess>();
                    services.AddScoped<IDataExportRegisteredHandler, DataExportRegisteredHandler>();
                    services.AddSingleton<IMessageBus, MessageBus>();

                    services.AddDbContext<DataExportContext>(
                        options => options.UseSqlServer(@"Server=sql;Database=DataExport;User ID=sa;Password=ABcd1234"));
                });
    }
}
