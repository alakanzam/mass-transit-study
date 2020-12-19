using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DIS.MessageEvent.Interfaces;
using DIS.MessageEvent.Models.Actions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace DIS.MessageRecipient.Consumers
{
    public class ValidateTariffRecordActionConsumer : IConsumer<ValidateTariffRecordAction>
    {
        #region Properties

        private readonly IEnumerable<IActionProcessor<ValidateTariffRecordAction>> _actionProcessors;


        #endregion

        #region Constructor

        public ValidateTariffRecordActionConsumer(IServiceProvider serviceProvider)
        {
            _actionProcessors = serviceProvider.GetServices<IActionProcessor<ValidateTariffRecordAction>>();
        }


        #endregion

        #region Methods

        public async Task Consume(ConsumeContext<ValidateTariffRecordAction> context)
        {
            if (_actionProcessors == null || !_actionProcessors.Any())
                return;

            foreach (var actionProcessor in _actionProcessors)
                await actionProcessor.ProcessAsync(context.Message);
        }

        #endregion
    }
}