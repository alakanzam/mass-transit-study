using System;
using System.Linq;
using System.Reflection;
using System.Web;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransitDemo.Services;
using Microsoft.Extensions.DependencyInjection;
using MTransit.Messages.Models;

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

            //foreach (var context in contexts)
            //	services.AddSingleton(typeof(IConsumer<>), context);
            services.AddSingleton<IConsumer<CommonMessage>, CommonMessageConsumer>();

            var serviceProvider = serviceProviderFactory.CreateServiceProvider(services);

            var commonMessageQueueName = "common-message";

            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                //var connectionString = $"amqp://admin:admin123@10.16.114.42:15672/";

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

                    // a type-based factory that returns an object (specialized uses)
                    var consumerType = typeof(IConsumer<CommonMessage>);
                    e.Consumer(() => (IConsumer<CommonMessage>)serviceProvider.GetService(consumerType));

                });
            });

            busControl.StartAsync().Wait();
            Console.WriteLine("MassTransit has been started");

            var message = new CommonMessage();
            message.Name = "Name 001";
            message.Description = "Description 001";
            busControl.Publish(message).Wait();
            Console.WriteLine("MassTransit has been sent");

            Console.ReadLine();
        }
    }
}
