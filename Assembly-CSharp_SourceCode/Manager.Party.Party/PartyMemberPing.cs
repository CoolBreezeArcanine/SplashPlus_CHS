using System;
using PartyLink;

namespace Manager.Party.Party
{
	[Serializable]
	public class PartyMemberPing
	{
		public long[] Ping = new long[PartyLink.Party.c_maxMecha];

		public PartyMemberPing()
		{
			Clear();
		}

		public void Clear()
		{
			for (int i = 0; i < Ping.Length; i++)
			{
				Ping[i] = 0L;
			}
		}
	}
}
