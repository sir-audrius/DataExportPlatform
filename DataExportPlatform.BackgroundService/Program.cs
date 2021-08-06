using DataExportPlatform.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System;
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
                    var factory = new ConnectionFactory() { HostName = "localhost", DispatchConsumersAsync = true };
                    var connection = factory.CreateConnection();
                    var channel = connection.CreateModel();

                    services.AddSingleton(channel);
                    services.AddHostedService<BackgoundProcess>();
                    services.AddScoped<IDataExportRegisteredHandler, DataExportRegisteredHandler>();
                    services.AddSingleton<IMessageBus, MessageBus>();

                    services.AddDbContext<DataExportContext>(
                        options => options.UseSqlServer(@"Server=(local);Database=DataExport;User ID=sa;Password=ABcd1234"));
                });
    }
}
