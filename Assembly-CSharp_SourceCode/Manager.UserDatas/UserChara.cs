using System.Collections.Generic;
using MAI2.Util;
using Manager.MaiStudio;

namespace Manager.UserDatas
{
	public class UserChara
	{
		public struct CharaLevelUpParam
		{
			public uint LevelUpCount;

			public uint AwakeUpCount;

			public List<float> NextAwakeGage;

			public uint preLevel;

			public uint preAwake;
		}

		public const uint AwakeningBonus = 100u;

		public const uint NotAwakeningBonus = 10u;

		public const uint LevelMax = 9999u;

		public const uint LevelMin = 1u;

		public int ID { get; set; }

		public uint Count { get; set; }

		public uint Level { get; set; }

		public int NextAwake { get; set; }

		public float NextAwakePercent { get; set; }

		public uint Awakening { get; set; }

		public UserChara()
		{
			Clear();
		}

		public UserChara(int id)
		{
			ID = id;
			Clear();
		}

		public UserChara(int id, uint count, uint level)
		{
			ID = id;
			Clear();
			Count = count;
			Level = level;
			CalcLevelToAwake();
		}

		public void Clear()
		{
			Count = 0u;
			Level = 1u;
			Awakening = 0u;
			NextAwake = -1;
			NextAwakePercent = 0f;
		}

		public void AddLevel(uint addLevel)
		{
			Count++;
			Level += addLevel;
			CalcLevelToAwake();
		}

		public void CalcLevelToAwake()
		{
			Awakening = 0u;
			NextAwake = -1;
			NextAwakePercent = 0f;
			uint num = 0u;
			foreach (KeyValuePair<int, CharaAwakeData> charaAwake in Singleton<DataManager>.Instance.GetCharaAwakes())
			{
				uint awakeLevel = (uint)charaAwake.Value.awakeLevel;
				if (awakeLevel <= Level)
				{
					Awakening++;
					num = awakeLevel;
					continue;
				}
				NextAwake = (int)(awakeLevel - Level);
				NextAwakePercent = 1f - (float)NextAwake / (float)(awakeLevel - num);
				break;
			}
			if (Level > 9999)
			{
				Level = 9999u;
			}
		}

		public uint GetMovementParam(bool matchColor, bool leader = false)
		{
			uint num = 1000u;
			if (leader)
			{
				num = 4000u;
			}
			else if (matchColor)
			{
				num = 2000u;
			}
			return num / 1000u;
		}
	}
}
