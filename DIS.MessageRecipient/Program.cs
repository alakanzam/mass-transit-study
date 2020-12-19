using System;
using System.Linq;
using DIS.MessageEvent.Interfaces;
using DIS.MessageEvent.Models.Actions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace DIS.MessageRecipient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var serviceProviderFactory = new DefaultServiceProviderFactory();

            var services = new ServiceCollection();

            var availableActions =
                typeof(ValidateTariffRecordAction)
                    .Assembly
                    .GetTypes()
                    .Where(x => typeof(IActionEvent).IsAssignableFrom(x) && !x.IsInterface)
                    .ToArray();

            var commonMessageQueueName = "event-message";
            services.AddSingleton(provider =>
            {
                return Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host(new Uri("amqps://mustang.rmq.cloudamqp.com/vjqpxaxw"), configurator =>
                    {
                        configurator.Username("vjqpxaxw");
                        configurator.Password("n8WSzZNJEl3Oi2L8RvHUFK_n0nwTl2BN");
                    });

                    cfg.ReceiveEndpoint(commonMessageQueueName, e =>
                    {
                        e.Durable = true;
                        e.Exclusive = false;
                        e.AutoDelete = true;

                        foreach (var availableAction in availableActions)
                        {
                            var genericConsumerType = typeof(IConsumer<>).MakeGenericType(availableAction);
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

            // Build service provider.
            var serviceProvider = serviceProviderFactory.CreateServiceProvider(services);
            Console.WriteLine("Message recipient is starting");

            var busControl = serviceProvider.GetService<IBusControl>();
            busControl.StartAsync().Wait();
            Console.WriteLine("Message recipient has been started");

            Console.ReadLine();
        }
    }
}