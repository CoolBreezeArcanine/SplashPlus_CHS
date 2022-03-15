using System;
using System.Collections.Generic;
using MAI2.Memory;
using PartyLink;
using UnityEngine;

namespace Manager.Party.Party
{
	[Serializable]
	public class MechaInfo : ICloneable
	{
		public bool IsJoin;

		public uint IpAddress;

		public int MusicID;

		public bool[] Entrys = new bool[2];

		public long[] UserIDs = new long[2];

		public string[] UserNames = new string[2];

		public int[] IconIDs = new int[2];

		public int[] FumenDifs = new int[2];

		public int[] Rateing = new int[2];

		public int[] ClassValue = new int[2];

		public int[] MaxClassValue = new int[2];

		public MechaInfo()
		{
			Clear();
		}

		public MechaInfo(IReadOnlyList<UserData> userDatas, IReadOnlyList<MusicDifficultyID> fumenDifs, int musicID)
			: this(userDatas, fumenDifs, musicID, 0)
		{
		}

		public MechaInfo(IReadOnlyList<UserData> userDatas, IReadOnlyList<MusicDifficultyID> fumenDifs, int musicID, int mockID)
			: this()
		{
			if (userDatas != null && fumenDifs != null && userDatas.Count == fumenDifs.Count)
			{
				IsJoin = true;
				IpAddress = PartyLink.Util.MyIpAddress(mockID).ToNetworkByteOrderU32();
				MusicID = musicID;
				for (int i = 0; i < 2; i++)
				{
					Entrys[i] = userDatas[i].IsEntry;
					UserIDs[i] = (long)userDatas[i].Detail.UserID;
					UserNames[i] = userDatas[i].Detail.UserName;
					IconIDs[i] = userDatas[i].Detail.EquipIconID;
					FumenDifs[i] = (int)fumenDifs[i];
					Rateing[i] = (int)userDatas[i].Detail.Rating;
					ClassValue[i] = userDatas[i].RatingList.Udemae.ClassValue;
					MaxClassValue[i] = userDatas[i].RatingList.Udemae.MaxClassValue;
				}
			}
		}

		public MechaInfo(MechaInfo arg)
		{
			SetAll(arg);
		}

		public object Clone()
		{
			return new MechaInfo(this);
		}

		public void SetAll(MechaInfo src)
		{
			if (src == null)
			{
				Clear();
			}
			else
			{
				CopyFrom(src);
			}
		}

		public void Clear()
		{
			IsJoin = false;
			IpAddress = 0u;
			MusicID = 0;
			for (int i = 0; i < 2; i++)
			{
				Entrys[i] = false;
				UserIDs[i] = 0L;
				UserNames[i] = "";
				FumenDifs[i] = 0;
				IconIDs[i] = 1;
				Rateing[i] = 0;
				ClassValue[i] = 0;
				MaxClassValue[i] = 0;
			}
		}

		public void CopyFrom(MechaInfo src)
		{
			IsJoin = src.IsJoin;
			IpAddress = src.IpAddress;
			MusicID = src.MusicID;
			for (int i = 0; i < 2; i++)
			{
				Entrys[i] = src.Entrys[i];
				UserIDs[i] = src.UserIDs[i];
				UserNames[i] = src.UserNames[i];
				FumenDifs[i] = src.FumenDifs[i];
				IconIDs[i] = src.IconIDs[i];
				Rateing[i] = src.Rateing[i];
				ClassValue[i] = src.ClassValue[i];
				MaxClassValue[i] = src.MaxClassValue[i];
			}
		}

		public int Serialize(int pos, Chunk chunk)
		{
			pos = chunk.writeBool(pos, IsJoin);
			pos = chunk.writeU32(pos, IpAddress);
			pos = chunk.writeS32(pos, MusicID);
			for (int i = 0; i < 2; i++)
			{
				pos = chunk.writeBool(pos, Entrys[i]);
				pos = chunk.writeS64(pos, UserIDs[i]);
				pos = chunk.writeString(pos, UserNames[i]);
				pos = chunk.writeS32(pos, FumenDifs[i]);
				pos = chunk.writeS32(pos, IconIDs[i]);
				pos = chunk.writeS32(pos, Rateing[i]);
				pos = chunk.writeS32(pos, ClassValue[i]);
				pos = chunk.writeS32(pos, MaxClassValue[i]);
			}
			return pos;
		}

		public MechaInfo Deserialize(ref int pos, Chunk chunk)
		{
			IsJoin = chunk.readBool(ref pos);
			IpAddress = chunk.readU32(ref pos);
			MusicID = chunk.readS32(ref pos);
			for (int i = 0; i < 2; i++)
			{
				Entrys[i] = chunk.readBool(ref pos);
				UserIDs[i] = chunk.readS64(ref pos);
				UserNames[i] = chunk.readString(ref pos);
				FumenDifs[i] = chunk.readS32(ref pos);
				IconIDs[i] = chunk.readS32(ref pos);
				Rateing[i] = chunk.readS32(ref pos);
				ClassValue[i] = chunk.readS32(ref pos);
				MaxClassValue[i] = chunk.readS32(ref pos);
			}
			return this;
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

		public MusicDifficultyID GetMusicDifID(int index)
		{
			return (MusicDifficultyID)FumenDifs[index];
		}

		public override string ToString()
		{
			return JsonUtility.ToJson(this);
		}
	}
}
