using System;
using System.Threading.Tasks;

namespace MTransit.Messages.Interfaces
{
	public interface IActionProcessor
	{
		
	}

	public interface IActionProcessor<in T> : IActionProcessor where T : IActionEvent
	{
		#region Properties

		/// <summary>
		/// Id of event processor.
		/// </summary>
		Guid Id { get; }

		/// <summary>
		/// Description of event processor.
		/// </summary>
		string Description { get; }

		#endregion

		#region Methods

		Task<IEventProcessResult> ProcessAsync(T actionEvent);

		#endregion
	}
}