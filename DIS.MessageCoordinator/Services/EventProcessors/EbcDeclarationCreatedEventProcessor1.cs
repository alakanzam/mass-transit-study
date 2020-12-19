using System;
using System.Threading.Tasks;
using DIS.MessageEvent.Interfaces;
using DIS.MessageEvent.Models.Actions;
using DIS.MessageEvent.Models.Events;
using MassTransit;

namespace DIS.MessageCoordinator.Services.EventProcessors
{
    public class EbcDeclarationCreatedEventProcessor1 : IEventProcessor<EbcDeclarationCreatedEvent>
    {
        #region Properties

        private readonly IBus _bus;

        public Guid Id { get; }

        public string Description { get; }

        #endregion

        #region Constructor

        public EbcDeclarationCreatedEventProcessor1(IBus bus)
        {
            _bus = bus;

            Id = Guid.NewGuid();
            Description = "Processor for EBC Declaration created event";
        }

        #endregion

        #region Methods

        public async Task<IEventProcessResult> ProcessAsync(EbcDeclarationCreatedEvent pastEvent)
        {
            Console.WriteLine($"[EXECUTING] {nameof(EbcDeclarationCreatedEventProcessor1)}");

            var action = new ValidateTariffRecordAction(pastEvent.Name);
            await _bus.Publish(action);

            await Task.Delay(5000);
            Console.WriteLine($"[EXECUTED] {nameof(EbcDeclarationCreatedEventProcessor1)}");
            return null;
        }

        #endregion
    }
}