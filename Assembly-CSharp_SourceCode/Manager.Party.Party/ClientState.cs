using System;
using DB;
using PartyLink;

namespace Manager.Party.Party
{
	[Serializable]
	public class ClientState : ICommandParam
	{
		public int State;

		public Command getCommand()
		{
			return Command.ClientState;
		}

		public ClientState(PartyPartyClientStateID state)
		{
			State = (int)state;
		}

		public ClientState()
		{
			State = 0;
		}

		public PartyPartyClientStateID GetState()
		{
			return (PartyPartyClientStateID)State;
		}
	}
}
