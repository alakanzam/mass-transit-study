using DIS.MessageEvent.Interfaces;

namespace DIS.MessageEvent.Models.Actions
{
	public class ValidateTariffRecordAction : IActionEvent
	{
		#region Properties

		// ReSharper disable once UnusedAutoPropertyAccessor.Local
		public string SourceEvent { get; private set; }

		// ReSharper disable once UnassignedGetOnlyAutoProperty
		public string Name { get; private set; }

		#endregion

		#region Constructor

		public ValidateTariffRecordAction(string sourceEvent)
		{
			SourceEvent = sourceEvent;
			Name = nameof(ValidateTariffRecordAction);
		}

		#endregion
	}
}