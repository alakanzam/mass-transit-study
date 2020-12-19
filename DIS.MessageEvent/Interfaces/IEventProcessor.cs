using System;
using System.Threading.Tasks;

namespace DIS.MessageEvent.Interfaces
{
	public interface IEventProcessor
	{

	}

	public interface IEventProcessor<in T> : IEventProcessor where T : IPastEvent
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

		Task<IEventProcessResult> ProcessAsync(T pastEvent);

		#endregion
	}
}