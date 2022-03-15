using System;
using System.Collections.Generic;
using System.Linq;
using MAI2.Memory;
using PartyLink;

namespace Manager.Party.Party
{
	[Serializable]
	public class PartyPlayInfo : ICommandParam
	{
		public MemberPlayInfo[] Member = new MemberPlayInfo[2];

		public ChainHistory[] ChainHistory = new ChainHistory[PartyLink.Party.c_chainHistorySize];

		public int Chain;

		public int ChainMiss;

		public int MaxChain;

		public bool IsFullChain;

		public int CalcStatus;

		public Command getCommand()
		{
			return Command.PartyPlayInfo;
		}

		public PartyPlayInfo()
		{
			for (int i = 0; i < Member.Length; i++)
			{
				Member[i] = new MemberPlayInfo();
			}
			for (int j = 0; j < ChainHistory.Length; j++)
			{
				ChainHistory[j] = new ChainHistory();
			}
			Clear();
		}

		public PartyPlayInfo(PartyPlayInfo src)
		{
			for (int i = 0; i < Member.Length; i++)
			{
				Member[i] = new MemberPlayInfo(src.Member[i]);
			}
			for (int j = 0; j < ChainHistory.Length; j++)
			{
				ChainHistory[j] = new ChainHistory(src.ChainHistory[j]);
			}
			Chain = src.Chain;
			ChainMiss = src.ChainMiss;
			MaxChain = src.MaxChain;
			CalcStatus = src.CalcStatus;
			IsFullChain = src.IsFullChain;
		}

		public void CopyFrom(PartyPlayInfo src)
		{
			for (int i = 0; i < Member.Length; i++)
			{
				Member[i].CopyFrom(src.Member[i]);
			}
			for (int j = 0; j < ChainHistory.Length; j++)
			{
				ChainHistory[j].CopyFrom(src.ChainHistory[j]);
			}
			Chain = src.Chain;
			ChainMiss = src.ChainMiss;
			MaxChain = src.MaxChain;
			CalcStatus = src.CalcStatus;
			IsFullChain = src.IsFullChain;
		}

		public int Serialize(int pos, Chunk chunk)
		{
			MemberPlayInfo[] member = Member;
			for (int i = 0; i < member.Length; i++)
			{
				pos = member[i].Serialize(pos, chunk);
			}
			ChainHistory[] chainHistory = ChainHistory;
			for (int i = 0; i < chainHistory.Length; i++)
			{
				pos = chainHistory[i].Serialize(pos, chunk);
			}
			pos = chunk.writeS32(pos, Chain);
			pos = chunk.writeS32(pos, ChainMiss);
			pos = chunk.writeS32(pos, MaxChain);
			pos = chunk.writeS32(pos, CalcStatus);
			pos = chunk.writeBool(pos, IsFullChain);
			return pos;
		}

		public PartyPlayInfo Deserialize(ref int pos, Chunk chunk)
		{
			MemberPlayInfo[] member = Member;
			for (int i = 0; i < member.Length; i++)
			{
				member[i].Deserialize(ref pos, chunk);
			}
			ChainHistory[] chainHistory = ChainHistory;
			for (int i = 0; i < chainHistory.Length; i++)
			{
				chainHistory[i].Deserialize(ref pos, chunk);
			}
			Chain = chunk.readS32(ref pos);
			ChainMiss = chunk.readS32(ref pos);
			MaxChain = chunk.readS32(ref pos);
			CalcStatus = chunk.readS32(ref pos);
			IsFullChain = chunk.readBool(ref pos);
			return this;
		}

		public void Clear()
		{
			MemberPlayInfo[] member = Member;
			for (int i = 0; i < member.Length; i++)
			{
				member[i].Clear();
			}
			ChainHistory[] chainHistory = ChainHistory;
			for (int i = 0; i < chainHistory.Length; i++)
			{
				chainHistory[i].Clear();
			}
			Chain = 0;
			ChainMiss = 0;
			MaxChain = 0;
			CalcStatus = 0;
			IsFullChain = false;
		}

		public uint GetActiveNumber()
		{
			return (uint)Member.Count((MemberPlayInfo i) => i.IsActive);
		}

		public int CountExistMembers()
		{
			return Member.Count((MemberPlayInfo i) => i.IsExist());
		}

		public IEnumerable<MemberPlayInfo> GetJoinMembers()
		{
			return Member.Where((MemberPlayInfo i) => i.IsExist());
		}

		public IEnumerable<MemberPlayInfo> GetJoinMembersWithoutMe(int mockID = 0)
		{
			uint myIp = PartyLink.Util.MyIpAddress(mockID).ToNetworkByteOrderU32();
			return Member.Where((MemberPlayInfo i) => i.IsExist() && i.IpAddress != myIp);
		}

		public MemberPlayInfo GetJoinMemberMe(int mockID = 0)
		{
			uint myIp = PartyLink.Util.MyIpAddress(mockID).ToNetworkByteOrderU32();
			return Member.FirstOrDefault((MemberPlayInfo i) => i.IsExist() && i.IpAddress == myIp);
		}

		private bool IsFullChainCheck()
		{
			bool result = false;
			if (CalcStatus != 0 && Chain > 0 && IsFullChain)
			{
				result = true;
			}
			return result;
		}

		public void Info(ref string os)
		{
			os += "PartyPlayInfo{\n";
			int num = 0;
			MemberPlayInfo[] member = Member;
			foreach (MemberPlayInfo memberPlayInfo in member)
			{
				os = os + "[" + num + "]";
				memberPlayInfo.Info(ref os);
				num++;
			}
			os += "}\n";
		}

		public override string ToString()
		{
			string os = "";
			Info(ref os);
			return os;
		}
	}
}
