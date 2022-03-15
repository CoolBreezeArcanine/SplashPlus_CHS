using System;
using System.Collections.Generic;
using System.Linq;
using MAI2.Memory;
using PartyLink;

namespace Manager.Party.Party
{
	[Serializable]
	public class PartyMemberInfo : ICommandParam, ICloneable
	{
		public MechaInfo[] MechaInfo = new MechaInfo[PartyLink.Party.c_maxMecha];

		public Command getCommand()
		{
			return Command.PartyMemberInfo;
		}

		public PartyMemberInfo()
		{
			for (int i = 0; i < MechaInfo.Length; i++)
			{
				MechaInfo[i] = new MechaInfo();
			}
		}

		public object Clone()
		{
			PartyMemberInfo partyMemberInfo = new PartyMemberInfo();
			for (int i = 0; i < MechaInfo.Length; i++)
			{
				partyMemberInfo.MechaInfo[i].SetAll(MechaInfo[i]);
			}
			return partyMemberInfo;
		}

		public void CopyFrom(PartyMemberInfo src)
		{
			for (int i = 0; i < MechaInfo.Length; i++)
			{
				if (i < src.MechaInfo.Length)
				{
					MechaInfo[i].CopyFrom(src.MechaInfo[i]);
				}
			}
		}

		public int Serialize(int pos, Chunk chunk)
		{
			MechaInfo[] mechaInfo = MechaInfo;
			for (int i = 0; i < mechaInfo.Length; i++)
			{
				pos = mechaInfo[i].Serialize(pos, chunk);
			}
			return pos;
		}

		public PartyMemberInfo Deserialize(ref int pos, Chunk chunk)
		{
			MechaInfo[] mechaInfo = MechaInfo;
			for (int i = 0; i < mechaInfo.Length; i++)
			{
				mechaInfo[i].Deserialize(ref pos, chunk);
			}
			return this;
		}

		public int GetEntryNumber()
		{
			return MechaInfo.Count((MechaInfo i) => i.IsJoin);
		}

		public IEnumerable<MechaInfo> GetJoinMembers()
		{
			return MechaInfo.Where((MechaInfo member) => member.IsJoin);
		}

		public IEnumerable<MechaInfo> GetJoinMembersWithoutMe(int mockID = 0)
		{
			uint myIp = PartyLink.Util.MyIpAddress(mockID).ToNetworkByteOrderU32();
			return MechaInfo.Where((MechaInfo member) => member.IsJoin && member.IpAddress != myIp);
		}

		public void Info(ref string os)
		{
			int num = 0;
			MechaInfo[] mechaInfo = MechaInfo;
			foreach (MechaInfo mechaInfo2 in mechaInfo)
			{
				os = string.Concat(os, num, " ", mechaInfo2, "\n");
				num++;
			}
		}

		public void Clear()
		{
			MechaInfo[] mechaInfo = MechaInfo;
			for (int i = 0; i < mechaInfo.Length; i++)
			{
				mechaInfo[i].Clear();
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
