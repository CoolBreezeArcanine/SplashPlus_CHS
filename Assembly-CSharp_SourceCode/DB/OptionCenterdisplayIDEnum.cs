namespace DB
{
	public static class OptionCenterdisplayIDEnum
	{
		private static readonly OptionCenterdisplayTableRecord[] records;

		public static bool IsActive(this OptionCenterdisplayID self)
		{
			if (self >= OptionCenterdisplayID.Off && self < OptionCenterdisplayID.End)
			{
				return self != OptionCenterdisplayID.Off;
			}
			return false;
		}

		public static bool IsValid(this OptionCenterdisplayID self)
		{
			if (self >= OptionCenterdisplayID.Off)
			{
				return self < OptionCenterdisplayID.End;
			}
			return false;
		}

		public static void Clamp(this OptionCenterdisplayID self)
		{
			if (self < OptionCenterdisplayID.Off)
			{
				self = OptionCenterdisplayID.Off;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionCenterdisplayID)GetEnd();
			}
		}

		public static int GetEnd(this OptionCenterdisplayID self)
		{
			return GetEnd();
		}

		public static OptionCenterdisplayID FindID(string enumName)
		{
			for (OptionCenterdisplayID optionCenterdisplayID = OptionCenterdisplayID.Off; optionCenterdisplayID < OptionCenterdisplayID.End; optionCenterdisplayID++)
			{
				if (optionCenterdisplayID.GetEnumName() == enumName)
				{
					return optionCenterdisplayID;
				}
			}
			return OptionCenterdisplayID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionCenterdisplayID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionCenterdisplayID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionCenterdisplayID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionCenterdisplayID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionCenterdisplayID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionCenterdisplayID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this OptionCenterdisplayID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		public static bool IsDefault(this OptionCenterdisplayID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		static OptionCenterdisplayIDEnum()
		{
			records = new OptionCenterdisplayTableRecord[11]
			{
				new OptionCenterdisplayTableRecord(0, "Off", "OFF", "", "画面中央不显示信息", "", "UI_OPT_B_04_01", 0),
				new OptionCenterdisplayTableRecord(1, "Combo", "COMBO", "", "显示没有MISS连续完成音符的总数", "", "UI_OPT_B_04_02", 1),
				new OptionCenterdisplayTableRecord(2, "AchivePlus", "达成率（+型）", "", "从0%开始\r\n百分比数字会随判定增加", "", "UI_OPT_B_04_03", 0),
				new OptionCenterdisplayTableRecord(3, "AchiveMinus1", "达成率（-型）", "", "从100%开始\r\n百分比数字会随判定减少", "", "UI_OPT_B_04_04", 0),
				new OptionCenterdisplayTableRecord(4, "AchiveMinus2", "達成率(-型2)", "", "从101%开始\r\n百分比数字会随判定减少", "", "UI_OPT_B_04_10", 0),
				new OptionCenterdisplayTableRecord(5, "DeluxScore", "DX分数(+型)", "", "显示在游戏中取得的DX分数", "", "UI_OPT_B_04_05", 0),
				new OptionCenterdisplayTableRecord(6, "DeluxScoreMinus", "DX分数(-型)", "", "显示在游戏中取得的DX分数\r\n数字会随判定减少", "", "UI_OPT_B_04_11", 0),
				new OptionCenterdisplayTableRecord(7, "BoarderS", "距S级", "", "显示距离达成S级所需的达成率，\r\n随判定减少", "", "UI_OPT_B_04_06", 0),
				new OptionCenterdisplayTableRecord(8, "BoarderSS", "距SS级", "", "显示距离达成SS级所需的达成率，\r\n随判定减少", "", "UI_OPT_B_04_07", 0),
				new OptionCenterdisplayTableRecord(9, "BoarderSSS", "距SSS级", "", "显示距离达成SSS级所需的达成率，\r\n随判定减少", "", "UI_OPT_B_04_08", 0),
				new OptionCenterdisplayTableRecord(10, "BoarderBest", "距最好成绩", "", "显示距离自己最好成绩所需的达成率，\r\n随判定减少", "", "UI_OPT_B_04_09", 0)
			};
		}
	}
}
