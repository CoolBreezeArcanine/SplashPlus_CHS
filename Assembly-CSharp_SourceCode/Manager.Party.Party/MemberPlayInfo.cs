using System;
using MAI2.Memory;
using PartyLink;
using UnityEngine;

namespace Manager.Party.Party
{
	[Serializable]
	public class MemberPlayInfo : ICloneable
	{
		public uint IpAddress;

		public bool IsActive;

		public byte[] Rankings = new byte[2];

		public int[] Achieves = new int[2];

		public int[] Combos = new int[2];

		public bool[] Miss = new bool[2];

		public int[] MissNums = new int[2];

		public bool[] GameOvers = new bool[2];

		public int[] FullCombos = new int[2];

		public MemberPlayInfo()
		{
			Clear();
		}

		public MemberPlayInfo(MemberPlayInfo src)
		{
			CopyFrom(src);
		}

		public MemberPlayInfo(uint ipAddress, int achieve0, int combo0, int missNum0, bool miss0, bool gameOver0, int fullCombo0, int achieve1, int combo1, int missNum1, bool miss1, bool gameOver1, int fullCombo1)
			: this()
		{
			IpAddress = ipAddress;
			Achieves[0] = achieve0;
			Achieves[1] = achieve1;
			Combos[0] = combo0;
			Combos[1] = combo1;
			MissNums[0] = missNum0;
			MissNums[1] = missNum1;
			Miss[0] = miss0;
			Miss[1] = miss1;
			GameOvers[0] = gameOver0;
			GameOvers[1] = gameOver1;
			FullCombos[0] = fullCombo0;
			FullCombos[1] = fullCombo1;
		}

		public object Clone()
		{
			return new MemberPlayInfo(this);
		}

		public void Clear()
		{
			IpAddress = 0u;
			IsActive = false;
			for (int i = 0; i < 2; i++)
			{
				Rankings[i] = 0;
				Achieves[i] = 0;
				Combos[i] = 0;
				MissNums[i] = 0;
				Miss[i] = false;
				GameOvers[i] = false;
				FullCombos[i] = 0;
			}
		}

		public void CopyFrom(MemberPlayInfo src)
		{
			IpAddress = src.IpAddress;
			IsActive = src.IsActive;
			for (int i = 0; i < 2; i++)
			{
				Rankings[i] = src.Rankings[i];
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
			pos = chunk.writeBool(pos, IsActive);
			for (int i = 0; i < 2; i++)
			{
				pos = chunk.writeU8(pos, Rankings[i]);
				pos = chunk.writeS32(pos, Achieves[i]);
				pos = chunk.writeS32(pos, Combos[i]);
				pos = chunk.writeS32(pos, MissNums[i]);
				pos = chunk.writeBool(pos, Miss[i]);
				pos = chunk.writeBool(pos, GameOvers[i]);
				pos = chunk.writeS32(pos, FullCombos[i]);
			}
			return pos;
		}

		public MemberPlayInfo Deserialize(ref int pos, Chunk chunk)
		{
			IpAddress = chunk.readU32(ref pos);
			IsActive = chunk.readBool(ref pos);
			for (int i = 0; i < 2; i++)
			{
				Rankings[i] = chunk.readU8(ref pos);
				Achieves[i] = chunk.readS32(ref pos);
				Combos[i] = chunk.readS32(ref pos);
				MissNums[i] = chunk.readS32(ref pos);
				Miss[i] = chunk.readBool(ref pos);
				GameOvers[i] = chunk.readBool(ref pos);
				FullCombos[i] = chunk.readS32(ref pos);
			}
			return this;
		}

		public bool IsError()
		{
			return !IsActive;
		}

		public bool IsExist()
		{
			return IpAddress != 0;
		}

		public string GetIpAddress()
		{
			return IpAddress.convIpString();
		}

		public bool IsMe()
		{
			return IsMe(0);
		}

		public bool IsMe(int mockID)
		{
			return PartyLink.Util.isMyIP(IpAddress, mockID);
		}

		public void Info(ref string os)
		{
			os += JsonUtility.ToJson(this);
		}
	}
}
