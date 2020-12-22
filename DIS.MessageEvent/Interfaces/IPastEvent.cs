using System;

namespace DIS.MessageEvent.Interfaces
{
	public interface IPastEvent
	{
		#region Properties

		Guid Id { get; }

		/// <summary>
		/// Name of event which already happened.
		/// This name must be unique to define the action to be taken.
		/// </summary>
		string Name { get; }

		#endregion
	}
}