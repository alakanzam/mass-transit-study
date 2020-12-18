using System;
using System.Threading.Tasks;
using MTransit.Messages.Interfaces;
using MTransit.Messages.Models;
using MTransit.Messages.Models.Events;

namespace DIS.MessageCoordinator.Services.EventProcessors
{
	public class EbcDeclarationCreatedEventProcessor3 : IEventProcessor<EbcDeclarationCreatedEvent>
	{
		#region Properties

		public Guid Id { get; }

		public string Description { get; }

		#endregion

		#region Constructor

		public EbcDeclarationCreatedEventProcessor3()
		{
			Id = Guid.NewGuid();
			Description = "Processor for EBC Declaration created event";
		}

		#endregion

		#region Methods

		public async Task<IEventProcessResult> ProcessAsync(EbcDeclarationCreatedEvent pastEvent)
		{
			Console.WriteLine($"[EXECUTING] {nameof(EbcDeclarationCreatedEventProcessor3)}");
			Console.WriteLine($"[EXECUTED] {nameof(EbcDeclarationCreatedEventProcessor3)}");
			return null;
		}

		#endregion
	}
}