using System;
using DB;
using PartyLink;

namespace Manager.Party.Party
{
	[Serializable]
	public class StartClientState : ICommandParam
	{
		public int ClientStateID;

		public Command getCommand()
		{
			return Command.StartClientState;
		}

		public StartClientState()
		{
			ClientStateID = 0;
		}

		public StartClientState(PartyPartyClientStateID id)
		{
			ClientStateID = (int)id;
		}

		public PartyPartyClientStateID GetState()
		{
			return (PartyPartyClientStateID)ClientStateID;
		}

		public override string ToString()
		{
			string text = "";
			PartyPartyClientStateID state = GetState();
			return string.Concat(text, getCommand(), " ", state.IsValid() ? state.GetName() : "INVALID", "\n");
		}
	}
}
