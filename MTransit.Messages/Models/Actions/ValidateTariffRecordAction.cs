using MTransit.Messages.Interfaces;

namespace MTransit.Messages.Models.Actions
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

		public ValidateTariffRecordAction(string sourceEvent, string name)
		{
			SourceEvent = sourceEvent;
			Name = name;
		}

		#endregion
	}
}