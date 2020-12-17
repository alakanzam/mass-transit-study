using System;
using System.Linq;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using MTransit.Messages.Models;

namespace MessagePublisher
{
	class Program
	{
		static void Main(string[] args)
		{
			var commonMessageQueueName = "common-message";

			var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
			{
				cfg.Host("rabbit@CPP00089015D", "/", configurator =>
				{
					configurator.Username("admin");
					configurator.Password("admin@123");
				});

				cfg.ReceiveEndpoint(commonMessageQueueName, e =>
				{
					e.Durable = true;
					e.Exclusive = false;
					e.AutoDelete = true;
				});
			});

			var message = new CommonMessage();
			message.Name = "Name 001";
			message.Description = "Description 001";
			busControl.Publish(busControl);

			Console.WriteLine("MassTransit publisher has been started");
			Console.ReadLine();
		}
	}
}
