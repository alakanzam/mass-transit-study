namespace MTransit.Messages.Interfaces
{
	public interface IActionEvent
	{
		#region Properties

		/// <summary>
		/// Event which is the source of action.
		/// </summary>
		string SourceEvent { get; }

		/// <summary>
		/// Name of action
		/// </summary>
		string Name { get; }

		#endregion
	}
}