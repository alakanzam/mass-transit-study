using System;
using System.Linq;
using DIS.MessageEvent.Models.Events;
using MassTransit;
using Microsoft.Extensions.Configuration;

namespace DIS.App
{
    class Program
    {
        static void Main(string[] args)
        {
            // Build the configuration
            var config = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();

            var secretProvider = config.Providers.First();
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                secretProvider.TryGet("RabbitMq:Uri", out var uri);
                secretProvider.TryGet("RabbitMq:Username", out var username);
                secretProvider.TryGet("RabbitMq:Password", out var password);

                secretProvider.TryGet("RabbitMq:Queue:Name", out var szQueueName);
                secretProvider.TryGet("RabbitMq:Queue:Durable", out var szDurable);
                secretProvider.TryGet("RabbitMq:Queue:Exclusive", out var szExclusive);
                secretProvider.TryGet("RabbitMq:Queue:AutoDelete", out var szAutoDelete);

                bool.TryParse(szDurable, out var durable);
                bool.TryParse(szExclusive, out var exclusive);
                bool.TryParse(szAutoDelete, out var autoDelete);

                cfg.Host(new Uri(uri), configurator =>
                {
                    configurator.Username(username);
                    configurator.Password(password);
                });

                cfg.ReceiveEndpoint(szQueueName, e =>
                {
                    e.Durable = durable;
                    e.Exclusive = exclusive;
                    e.AutoDelete = autoDelete;
                });
            });

            Console.WriteLine("[DIS.App] starting");

            // Start the bus.
            busControl.StartAsync().Wait();

            //var message = new CommonMessage();
            var declarationIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
            var ebcDeclarationCreatedEvent = new EbcDeclarationCreatedEvent(Guid.NewGuid(), "EBC-1000", declarationIds);
            busControl.Publish(ebcDeclarationCreatedEvent).Wait();


            Console.WriteLine("[DIS.App] started");
            Console.ReadLine();
        }
    }
}
