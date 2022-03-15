using System;
using DB;
using PartyLink;

namespace Manager.Party.Party
{
	[Serializable]
	public class PartyMemberState : ICommandParam, ICloneable
	{
		public int[] StateList = new int[PartyLink.Party.c_maxMecha];

		public Command getCommand()
		{
			return Command.PartyMemberState;
		}

		public PartyMemberState()
		{
			Clear();
		}

		public void Clear()
		{
			for (int i = 0; i < StateList.Length; i++)
			{
				StateList[i] = 0;
			}
		}

		public object Clone()
		{
			return new PartyMemberState
			{
				StateList = (int[])StateList.Clone()
			};
		}

		public void SetState(int n, PartyPartyClientStateID id)
		{
			if (0 <= n && n < StateList.Length)
			{
				StateList[n] = (int)id;
			}
		}

		public PartyPartyClientStateID GetState(int n)
		{
			return (PartyPartyClientStateID)StateList[n];
		}

		public void Info(ref string os)
		{
			for (int i = 0; i < PartyLink.Party.c_maxMecha; i++)
			{
				PartyPartyClientStateID state = GetState(i);
				os = os + i + " " + (state.IsValid() ? state.GetName() : "") + "\n";
			}
		}

		public override string ToString()
		{
			string os = "";
			Info(ref os);
			return os;
		}
	}
}
