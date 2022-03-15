using System;
using DB;
using PartyLink;

namespace Manager.Party.Party
{
	[Serializable]
	public class JoinResult : ICommandParam
	{
		public int ResultID;

		public Command getCommand()
		{
			return Command.JoinResult;
		}

		public JoinResult()
		{
			ResultID = 0;
		}

		public JoinResult(PartyPartyJoinResultID id)
		{
			ResultID = (int)id;
		}

		public PartyPartyJoinResultID GetResult()
		{
			return (PartyPartyJoinResultID)ResultID;
		}

		public bool IsSuccess()
		{
			return 1 == ResultID;
		}

		public override string ToString()
		{
			return GetResult().GetName();
		}
	}
}
