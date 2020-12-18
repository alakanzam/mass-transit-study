using System;
using MTransit.Messages.Interfaces;

namespace MTransit.Messages.Models.Events
{
	public class EbcDeclarationCreatedEvent : IPastEvent
	{
		#region Properties

		// ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
		public Guid Id { get; private set; }

		// ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
		public string Name { get; private set; }

		public Guid[] DeclarationIds { get; set; }

		#endregion

		#region Constructor

		public EbcDeclarationCreatedEvent(Guid id , string name, Guid[] declarationIds)
		{
			Id = id;
			Name = name;
			DeclarationIds = declarationIds;
		}

		#endregion
	}
}