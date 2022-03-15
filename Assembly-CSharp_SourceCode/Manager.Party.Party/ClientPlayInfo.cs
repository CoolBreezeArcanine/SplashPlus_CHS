using System;
using System.Linq;
using MAI2.Memory;
using PartyLink;
using UnityEngine;

namespace Manager.Party.Party
{
	[Serializable]
	public class ClientPlayInfo : ICommandParam, ICloneable
	{
		public uint IpAddress;

		public int Count;

		public bool[] IsValids = new bool[2];

		public int[] Achieves = new int[2];

		public int[] Combos = new int[2];

		public bool[] Miss = new bool[2];

		public int[] MissNums = new int[2];

		public bool[] GameOvers = new bool[2];

		public int[] FullCombos = new int[2];

		public Command getCommand()
		{
			return Command.ClientPlayInfo;
		}

		public ClientPlayInfo()
		{
			Clear();
		}

		public ClientPlayInfo(ClientPlayInfo src)
		{
			CopyFrom(src);
		}

		public object Clone()
		{
			return new ClientPlayInfo(this);
		}

		public void Clear()
		{
			IpAddress = 0u;
			Count = 0;
			for (int i = 0; i < 2; i++)
			{
				IsValids[i] = false;
				Achieves[i] = 0;
				Combos[i] = 0;
				MissNums[i] = 0;
				Miss[i] = false;
				GameOvers[i] = false;
				FullCombos[i] = 0;
			}
		}

		public void CopyFrom(ClientPlayInfo src)
		{
			IpAddress = src.IpAddress;
			Count = src.Count;
			for (int i = 0; i < 2; i++)
			{
				IsValids[i] = src.IsValids[i];
				Achieves[i] = src.Achieves[i];
				Combos[i] = src.Combos[i];
				MissNums[i] = src.MissNums[i];
				Miss[i] = src.Miss[i];
				GameOvers[i] = src.GameOvers[i];
				FullCombos[i] = src.FullCombos[i];
			}
		}

		public int Serialize(int pos, Chunk chunk)
		{
			pos = chunk.writeU32(pos, IpAddress);
			pos = chunk.writeS32(pos, Count);
			for (int i = 0; i < 2; i++)
			{
				pos = chunk.writeBool(pos, IsValids[i]);
				pos = chunk.writeS32(pos, Achieves[i]);
				pos = chunk.writeS32(pos, Combos[i]);
				pos = chunk.writeS32(pos, MissNums[i]);
				pos = chunk.writeBool(pos, Miss[i]);
				pos = chunk.writeBool(pos, GameOvers[i]);
				pos = chunk.writeS32(pos, FullCombos[i]);
			}
			return pos;
		}

		public ClientPlayInfo Deserialize(ref int pos, Chunk chunk)
		{
			IpAddress = chunk.readU32(ref pos);
			Count = chunk.readS32(ref pos);
			for (int i = 0; i < 2; i++)
			{
				IsValids[i] = chunk.readBool(ref pos);
				Achieves[i] = chunk.readS32(ref pos);
				Combos[i] = chunk.readS32(ref pos);
				MissNums[i] = chunk.readS32(ref pos);
				Miss[i] = chunk.readBool(ref pos);
				GameOvers[i] = chunk.readBool(ref pos);
				FullCombos[i] = chunk.readS32(ref pos);
			}
			return this;
		}

		public bool IsAlive()
		{
			return GameOvers.All((bool i) => !i);
		}

		public int GetAliveNum()
		{
			int num = 0;
			for (int i = 0; i < 2; i++)
			{
				if (IsValids[i] && !GameOvers[i])
				{
					num++;
				}
			}
			return num;
		}

		public string GetIpAddress()
		{
			return IpAddress.convIpString();
		}

		public void Info(ref string os)
		{
			os += JsonUtility.ToJson(this);
		}

		public MemberPlayInfo GetMemberPlayInfo()
		{
			return new MemberPlayInfo(IpAddress, Achieves[0], Combos[0], MissNums[0], Miss[0], GameOvers[0], FullCombos[0], Achieves[1], Combos[1], MissNums[1], Miss[1], GameOvers[1], FullCombos[1]);
		}

		public override string ToString()
		{
			string os = "";
			Info(ref os);
			return os;
		}
	}
}
