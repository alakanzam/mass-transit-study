using System;
using System.Threading.Tasks;
using DIS.MessageEvent.Interfaces;
using DIS.MessageEvent.Models.Events;

namespace DIS.MessageCoordinator.Services.EventProcessors
{
	public class EbcDeclarationCreatedEventProcessor2 : IEventProcessor<EbcDeclarationCreatedEvent>
	{
		#region Properties

		public Guid Id { get; }

		public string Description { get; }

		#endregion

		#region Constructor

		public EbcDeclarationCreatedEventProcessor2()
		{
			Id = Guid.NewGuid();
			Description = "Processor for EBC Declaration created event";
		}

		#endregion

		#region Methods

		public async Task<IEventProcessResult> ProcessAsync(EbcDeclarationCreatedEvent pastEvent)
		{
			Console.WriteLine($"[EXECUTING] {nameof(EbcDeclarationCreatedEventProcessor2)}");
			await Task.Delay(1000);
			Console.WriteLine($"[EXECUTED] {nameof(EbcDeclarationCreatedEventProcessor2)}");
			return null;
		}

		#endregion
	}
}