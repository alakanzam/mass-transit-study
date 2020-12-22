using System;
using System.Text;
using System.Threading.Tasks;
using DIS.MessageEvent.Interfaces;
using EventStore.ClientAPI;
using MassTransit.Audit;
using Newtonsoft.Json;

namespace DIS.MessageCoordinator.Observers
{
	public class EventStoreAuditStore : IMessageAuditStore
	{
		#region Constructor

		private readonly IEventStoreConnection _eventStoreConnection;

		#endregion

		#region Constructor

		public EventStoreAuditStore(IEventStoreConnection eventStoreConnection)
		{
			_eventStoreConnection = eventStoreConnection;
		}

		#endregion

		#region Methods

		public virtual Task StoreMessage<T>(T message, MessageAuditMetadata metadata) where T : class
		{
			if (message is IPastEvent pastEvent)
			{
				var streamId = $"{pastEvent.Name}-{metadata.CorrelationId}";
				var jObject = JsonConvert.SerializeObject(message);
				var jMetadataObject = JsonConvert.SerializeObject(metadata);
				var eventData = new EventData(Guid.NewGuid(), pastEvent.Name, true, Encoding.UTF8.GetBytes(jObject), 
					Encoding.UTF8.GetBytes(jMetadataObject));

				return _eventStoreConnection.AppendToStreamAsync(streamId, 1, eventData);
			}

			return Task.CompletedTask;
		}

		#endregion
	}
}