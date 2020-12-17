using System;
using System.Threading.Tasks;
using MassTransit;
using MTransit.Messages.Models;
using Newtonsoft.Json;

namespace MassTransitDemo.Services
{
	public class CommonMessageConsumer : IConsumer<CommonMessage>
	{
		#region Constructor

		public CommonMessageConsumer()
		{
		}

		#endregion

		#region Methods

		public Task Consume(ConsumeContext<CommonMessage> context)
		{
			var message = JsonConvert.SerializeObject(context.Message);
			Console.WriteLine(message);
			return Task.CompletedTask;
		}

		#endregion
	}
}