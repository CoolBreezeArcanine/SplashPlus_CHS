using System;
using PartyLink;

namespace Manager.Party.Party
{
	[Serializable]
	public class CancelJoin : ICommandParam
	{
		public Command getCommand()
		{
			return Command.CancelJoin;
		}
	}
}
