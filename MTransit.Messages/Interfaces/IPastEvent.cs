using System;

namespace MTransit.Messages.Interfaces
{
	public interface IPastEvent
	{
		#region Properties

		/// <summary>
		/// Name of event which already happened.
		/// This name must be unique to define the action to be taken.
		/// </summary>
		string Name { get; }

		#endregion
	}
}