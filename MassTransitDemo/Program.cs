using System;
using System.Linq;
using DIS.MessageCoordinator.Services;
using DIS.MessageCoordinator.Services.EventConsumers;
using DIS.MessageCoordinator.Services.EventProcessors;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using MTransit.Messages.Extensions;
using MTransit.Messages.Interfaces;
using MTransit.Messages.Models;
using MTransit.Messages.Models.Actions;
using MTransit.Messages.Models.Events;

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

			services.AddActionProcessors<ValidateTariffRecordAction>(
				typeof(ValidateTariffRecordAction).Assembly);

			foreach (var availablePastEvent in availablePastEvents)
			{
				var genericConsumerType = typeof(IConsumer<>).MakeGenericType(availablePastEvent);
				//var genericProcessorType = typeof(IEventProcessor<>).MakeGenericType(availablePastEvent);

				var availableConsumerType = availableConsumerTypes
					.FirstOrDefault(x => genericConsumerType.IsAssignableFrom(x));

				if (availableConsumerType != null)
					services.AddSingleton(genericConsumerType, availableConsumerType);

				//var processorTypes = availableProcessorTypes
				//	.Where(x => genericProcessorType.IsAssignableFrom(x) && !x.IsInterface)
				//	.ToArray();

				if (availableConsumerType != null)
					services.AddSingleton(genericConsumerType, availableConsumerType);

				//if (processorTypes.Any())
				//{
				//	foreach (var processorType in processorTypes)
				//		services.AddSingleton(genericProcessorType, processorType);
				//}
			}

			//foreach (var context in contexts)
			//	services.AddSingleton(typeof(IConsumer<>), context);
			//services.AddSingleton<IConsumer<CommonMessage>, EbcCreatedEventConsumer>();
			var commonMessageQueueName = "event-message";
			services.AddSingleton<IBusControl>(provider =>
			{
				return Bus.Factory.CreateUsingRabbitMq(cfg =>
				{
					//var connectionString = $"amqp://admin:admin123@10.16.114.42:15672/";

					//cfg.Host(new Uri("amqps://mustang.rmq.cloudamqp.com/vjqpxaxw"), configurator =>
					//{
					// configurator.Username("vjqpxaxw");
					// configurator.Password("n8WSzZNJEl3Oi2L8RvHUFK_n0nwTl2BN");
					//});
					cfg.Host(new Uri("amqp://10.16.114.42:5672/"), configurator =>
					{
						configurator.Username("admin");
						configurator.Password("admin@123");
					});

					cfg.ReceiveEndpoint(commonMessageQueueName, e =>
					{
						e.Durable = true;
						e.Exclusive = false;
						e.AutoDelete = true;

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

			var serviceProvider = serviceProviderFactory.CreateServiceProvider(services);
			Console.WriteLine("MassTransit is starting");

			var busControl = serviceProvider.GetService<IBusControl>();
			busControl.StartAsync().Wait();
			Console.WriteLine("MassTransit has been started");

			var declarationIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
			var ebcCreatedEvent = new EbcDeclarationCreatedEvent(Guid.NewGuid(), "Name-001", declarationIds);
			busControl.Publish(ebcCreatedEvent).Wait();
			Console.WriteLine("MassTransit has been sent");

			Console.ReadLine();
		}
	}
}
