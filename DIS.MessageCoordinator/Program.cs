using System;
using System.Linq;
using DIS.MessageCoordinator.Services.EventConsumers;
using DIS.MessageCoordinator.Services.EventProcessors;
using DIS.MessageEvent.Extensions;
using DIS.MessageEvent.Interfaces;
using DIS.MessageEvent.Models.Events;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DIS.MessageCoordinator
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProviderFactory = new DefaultServiceProviderFactory();

            var services = new ServiceCollection();
            var availablePastEvents =
                typeof(EbcDeclarationCreatedEvent)
                    .Assembly
                    .GetTypes()
                    .Where(x => typeof(IPastEvent).IsAssignableFrom(x) && !x.IsInterface)
                    .ToArray();

            var availableConsumerTypes = typeof(EbcDeclarationCreatedEventConsumer).Assembly
                .GetTypes()
                .Where(x => typeof(IConsumer).IsAssignableFrom(x) && !x.IsInterface)
                .ToArray();

            services.AddEventProcessors<EbcDeclarationCreatedEvent>(
                typeof(EbcDeclarationCreatedEventProcessor1).Assembly);

            foreach (var availablePastEvent in availablePastEvents)
            {
                var genericConsumerType = typeof(IConsumer<>).MakeGenericType(availablePastEvent);

                var availableConsumerType = availableConsumerTypes
                    .FirstOrDefault(x => genericConsumerType.IsAssignableFrom(x));

                if (availableConsumerType != null)
                    services.AddSingleton(genericConsumerType, availableConsumerType);
            }

            // Build the configuration
            var config = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();

            var secretProvider = config.Providers.First();
            services.AddSingleton(provider =>
            {
                return Bus.Factory.CreateUsingRabbitMq(cfg =>
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

                        foreach (var availablePastEvent in availablePastEvents)
                        {
                            var genericConsumerType = typeof(IConsumer<>).MakeGenericType(availablePastEvent);
                            e.Consumer(genericConsumerType, type =>
                            {
                                var consumerServices = provider.GetServices(type);
                                var consumerService = consumerServices.FirstOrDefault();
                                return consumerService;
                            });
                        }
                    });
                });
            });

            services.AddSingleton<IBus>(provider => provider.GetService<IBusControl>());

            var serviceProvider = serviceProviderFactory.CreateServiceProvider(services);
            Console.WriteLine("[DIS.MessageCoordinator] starting");

            var busControl = serviceProvider.GetService<IBusControl>();
            busControl.StartAsync().Wait();
            Console.WriteLine("[DIS.MessageCoordinator] started");

            Console.ReadLine();
        }
    }
}
