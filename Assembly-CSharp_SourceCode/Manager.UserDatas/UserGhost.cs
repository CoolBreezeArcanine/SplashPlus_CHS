using System.Collections.Generic;
using MAI2.Util;

namespace Manager.UserDatas
{
	public class UserGhost
	{
		public enum GhostType
		{
			MapNpc,
			Player,
			Boss,
			End
		}

		public ulong Id;

		public GhostType Type;

		public string Name;

		public int IconId;

		public int PlateId;

		public int TitleId;

		public int Rate;

		public uint ClassRank;

		public int ClassValue;

		public uint CourseRank;

		public long UnixTime;

		public uint ShopId;

		public int RegionCode;

		public MusicDifficultyID TypeId;

		public int MusicId;

		public int Difficulty;

		public int Version;

		public List<byte> ResultBitList = new List<byte>();

		public int ResultNum;

		public int Achievement;

		public MusicClearrankID Rank;

		private const byte ShiftBit = 4;

		public bool IsUsed;

		public bool NeedSave;

		public UserGhost()
		{
			Clear();
		}

		public void Clear()
		{
			Id = 0uL;
			Type = GhostType.Player;
			Name = "";
			IconId = 1;
			PlateId = 1;
			TitleId = 1;
			Rate = 0;
			ClassValue = 0;
			ClassRank = 0u;
			CourseRank = 0u;
			UnixTime = 0L;
			ShopId = 0u;
			RegionCode = 0;
			ResultBitList.Clear();
			ResultNum = 0;
			Achievement = 0;
			Rank = MusicClearrankID.Rank_A;
			TypeId = MusicDifficultyID.End;
			MusicId = 0;
			Difficulty = 0;
			Version = 0;
			IsUsed = false;
			NeedSave = false;
		}

		public void CreateGhost(ulong userId, GhostType type, int index, string name, int icon, int plate, int title, int rate, int classValue, uint daniId)
		{
			Id = userId;
			Type = type;
			UnixTime = TimeManager.GetNowUnixTime();
			Name = name;
			IconId = icon;
			PlateId = plate;
			TitleId = title;
			Rate = rate;
			ClassValue = classValue;
			ClassRank = (uint)UserUdemae.GetRateToUdemaeID(classValue);
			CourseRank = daniId;
			ShopId = Singleton<OperationManager>.Instance.ShopData.LocationId;
			RegionCode = Singleton<OperationManager>.Instance.ShopData.RegionCode;
			if (Type != GhostType.Boss)
			{
				CreateBossGhostBitData(index);
			}
			else
			{
				CreateGhostBitData(index);
			}
			IsUsed = false;
			NeedSave = true;
		}

		public void CreateGhostBitData(int index)
		{
			GameScoreList gameScore = Singleton<GamePlayManager>.Instance.GetGameScore(index);
			MusicId = gameScore.SessionInfo.musicId;
			Difficulty = gameScore.SessionInfo.difficulty;
			TypeId = (MusicDifficultyID)gameScore.SessionInfo.difficulty;
			Achievement = GameManager.ConvAchiveDecimalToInt(gameScore.GetAchivement());
			Rank = GhostManager.GetRank(Achievement);
			ResultBitList.Clear();
			ResultNum = (int)gameScore.TheoryCombo;
			IsUsed = false;
			NeedSave = false;
			byte b = 0;
			byte b2 = 0;
			for (int i = 0; i < gameScore.GetScoreLength(); i++)
			{
				b = (byte)(b | (byte)((byte)gameScore.GetScoreAt(i).Timing << (int)b2));
				b2 = (byte)(b2 + 4);
				if (b2 > 7)
				{
					b2 = 0;
					ResultBitList.Add(b);
					b = 0;
				}
			}
			if (ResultNum % 8 != 0)
			{
				ResultBitList.Add(b);
			}
		}

		private void CreateBossGhostBitData(int index)
		{
			GameScoreList gameScore = Singleton<GamePlayManager>.Instance.GetGameScore(index);
			MusicId = gameScore.SessionInfo.musicId;
			Difficulty = gameScore.SessionInfo.difficulty;
			TypeId = (MusicDifficultyID)gameScore.SessionInfo.difficulty;
			Achievement = GameManager.ConvAchiveDecimalToInt(gameScore.GetAchivement());
			Rank = GhostManager.GetRank(Achievement);
			ResultBitList.Clear();
			ResultNum = (int)gameScore.TheoryCombo;
			IsUsed = false;
			NeedSave = false;
			byte b = 0;
			byte b2 = 0;
			for (int i = 0; i < gameScore.GetScoreLength(); i++)
			{
				b = (byte)(b | (byte)((byte)gameScore.GetScoreAt(i).Timing << (int)b2));
				b2 = (byte)(b2 + 4);
				if (b2 > 7)
				{
					b2 = 0;
					ResultBitList.Add(b);
					b = 0;
				}
			}
			if (ResultNum % 8 != 0)
			{
				ResultBitList.Add(b);
			}
		}

		public byte GetResultIndexTo(int index)
		{
			int num = index / 2;
			if (ResultBitList.Count <= num)
			{
				return 0;
			}
			if (index % 2 == 0)
			{
				return (byte)(ResultBitList[num] & 0xFu);
			}
			return (byte)((uint)(ResultBitList[num] >> 4) & 0xFu);
		}
	}
}
