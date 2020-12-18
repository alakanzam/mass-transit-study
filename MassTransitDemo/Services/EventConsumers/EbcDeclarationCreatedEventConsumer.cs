using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using MTransit.Messages.Interfaces;
using MTransit.Messages.Models;
using MTransit.Messages.Models.Events;

namespace DIS.MessageCoordinator.Services.EventConsumers
{
	public class EbcDeclarationCreatedEventConsumer : IConsumer<EbcDeclarationCreatedEvent>
	{
		#region Properties

		private readonly IEnumerable<IEventProcessor<EbcDeclarationCreatedEvent>> _eventProcessors;

		#endregion

		#region Constructor

		public EbcDeclarationCreatedEventConsumer(IServiceProvider serviceProvider)
		{
			_eventProcessors = serviceProvider.GetServices<IEventProcessor<EbcDeclarationCreatedEvent>>();
		}

		#endregion

		#region Methods

		public async Task Consume(ConsumeContext<EbcDeclarationCreatedEvent> context)
		{
			if (_eventProcessors == null || !_eventProcessors.Any())
				return;

			foreach (var eventProcessor in _eventProcessors)
				await eventProcessor.ProcessAsync(context.Message);
		}

		#endregion
	}
}