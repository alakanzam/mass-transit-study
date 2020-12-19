using System;
using System.Threading.Tasks;
using DIS.MessageEvent.Interfaces;
using DIS.MessageEvent.Models.Actions;

namespace DIS.MessageRecipient.ActionProcessors
{
	public class ValidateTariffRecordActionProcessor : IActionProcessor<ValidateTariffRecordAction>
	{
		#region Properties

		public Guid Id { get; }

		public string Description { get; }

		#endregion

		#region Constructor

        public ValidateTariffRecordActionProcessor()
        {
			Id = Guid.NewGuid();
            Description = string.Empty;
        }

		#endregion

		#region Methods

		public virtual async Task<IEventProcessResult> ProcessAsync(ValidateTariffRecordAction actionEvent)
		{
			Console.WriteLine($"{nameof(ValidateTariffRecordActionProcessor)} is calling");
            await Task.Delay(TimeSpan.FromSeconds(2));
			Console.WriteLine($"{nameof(ValidateTariffRecordActionProcessor)} has called");
			return null;
		}

		#endregion
	}
}