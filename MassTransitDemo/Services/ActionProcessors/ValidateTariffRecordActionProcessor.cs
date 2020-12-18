using System;
using System.Threading.Tasks;
using MTransit.Messages.Interfaces;
using MTransit.Messages.Models.Actions;

namespace DIS.MessageCoordinator.Services.ActionProcessors
{
	public class ValidateTariffRecordActionProcessor : IActionProcessor<ValidateTariffRecordAction>
	{
		#region Properties

		public Guid Id { get; }

		public string Description { get; }

		#endregion

		#region Methods

		public virtual async Task<IEventProcessResult> ProcessAsync(ValidateTariffRecordAction actionEvent)
		{
			return null;
		}

		#endregion
	}
}