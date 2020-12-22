using System;
using System.Linq;
using DIS.MessageEvent.Interfaces;
using DIS.MessageEvent.Models.Actions;
using MassTransit;
using Microsoft.Extensions.Configuration;
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