using System;
using System.Threading.Tasks;
using MTransit.Messages.Interfaces;
using MTransit.Messages.Models;
using MTransit.Messages.Models.Events;

namespace DIS.MessageCoordinator.Services.EventProcessors
{
	public class EbcDeclarationCreatedEventProcessor1 : IEventProcessor<EbcDeclarationCreatedEvent>
	{
		#region Properties

		public Guid Id { get; }

		public string Description { get; }

		#endregion

		#region Constructor

		public EbcDeclarationCreatedEventProcessor1()
		{
			Id = Guid.NewGuid();
			Description = "Processor for EBC Declaration created event";
		}

		#endregion

		#region Methods

		public async Task<IEventProcessResult> ProcessAsync(EbcDeclarationCreatedEvent pastEvent)
		{
			Console.WriteLine($"[EXECUTING] {nameof(EbcDeclarationCreatedEventProcessor1)}");
			await Task.Delay(5000);
			Console.WriteLine($"[EXECUTED] {nameof(EbcDeclarationCreatedEventProcessor1)}");
			return null;
		}

		#endregion
	}
}