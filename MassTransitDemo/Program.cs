using System;
using System.Linq;
using System.Reflection;
using System.Web;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransitDemo.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MassTransitDemo
{
	class Program
	{
		static void Main(string[] args)
		{
			var serviceProviderFactory = new DefaultServiceProviderFactory();

			var services = new ServiceCollection();
			var contexts =
				typeof(CommonMessageConsumer)
					.Assembly
					.GetTypes()
					.Where(x => typeof(IConsumer).IsAssignableFrom(x) && !x.IsInterface)
					.ToArray();

			foreach (var context in contexts)
				services.AddSingleton(typeof(IConsumer), context);

			var serviceProvider = serviceProviderFactory.CreateServiceProvider(services);

			var commonMessageQueueName = "common-message";
			
			var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
			{
				//var connectionString = $"amqp://admin:admin123@10.16.114.42:15672/";

				cfg.Host(new Uri("amqp://10.16.114.42:5672/"), configurator =>
				{
					configurator.Username("admin");
					configurator.Password("admin123");
				});

				cfg.ReceiveEndpoint(commonMessageQueueName, e =>
				{
					e.Durable = true;
					e.Exclusive = false;
					e.AutoDelete = true;

					// // delegate consumer factory
					// e.Consumer(() => new SubmitOrderConsumer());
					//
					// // another delegate consumer factory, with dependency
					// e.Consumer(() => new LogOrderSubmittedConsumer(Console.Out));

					// a type-based factory that returns an object (specialized uses)
					var consumerType = typeof(CommonMessageConsumer);
					e.Consumer(consumerType, type => serviceProvider.GetService(type));
				});
			});

			busControl.StartAsync().Wait();
			Console.WriteLine("MassTransit has been started");
			Console.ReadLine();
		}
	}
}
