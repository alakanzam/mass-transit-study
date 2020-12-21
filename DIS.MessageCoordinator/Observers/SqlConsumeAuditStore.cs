using System;
using System.Threading.Tasks;
using MassTransit.Audit;
using Newtonsoft.Json;

namespace DIS.MessageCoordinator.Observers
{
	public class SqlConsumeAuditStore : IMessageAuditStore
	{
		#region Constructor

		#endregion

		#region Methods

		public virtual Task StoreMessage<T>(T message, MessageAuditMetadata metadata) where T : class
		{
			Console.WriteLine($"{nameof(StoreMessage)} is called");

			var instance = JsonConvert.SerializeObject(message);
			Console.WriteLine(instance);

			return Task.CompletedTask;
		}

		#endregion
	}
}