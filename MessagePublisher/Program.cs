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

                    // // delegate consumer factory
                    // e.Consumer(() => new SubmitOrderConsumer());
                    //
                    // // another delegate consumer factory, with dependency
                    // e.Consumer(() => new LogOrderSubmittedConsumer(Console.Out));
                });
			});

            busControl.StartAsync().Wait();

			var message = new CommonMessage();
			message.Name = "Name 001";
			message.Description = "Description 001";
            busControl.Publish(message).Wait();

			Console.WriteLine("MassTransit publisher has been started");
			Console.ReadLine();
		}
	}
}
