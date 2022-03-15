namespace DB
{
	public static class OptionCategoryIDEnum
	{
		private static readonly OptionCategoryTableRecord[] records;

		public static bool IsActive(this OptionCategoryID self)
		{
			if (self >= OptionCategoryID.SpeedSetting && self < OptionCategoryID.End)
			{
				return self != OptionCategoryID.SpeedSetting;
			}
			return false;
		}

		public static bool IsValid(this OptionCategoryID self)
		{
			if (self >= OptionCategoryID.SpeedSetting)
			{
				return self < OptionCategoryID.End;
			}
			return false;
		}

		public static void Clamp(this OptionCategoryID self)
		{
			if (self < OptionCategoryID.SpeedSetting)
			{
				self = OptionCategoryID.SpeedSetting;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionCategoryID)GetEnd();
			}
		}

		public static int GetEnd(this OptionCategoryID self)
		{
			return GetEnd();
		}

		public static OptionCategoryID FindID(string enumName)
		{
			for (OptionCategoryID optionCategoryID = OptionCategoryID.SpeedSetting; optionCategoryID < OptionCategoryID.End; optionCategoryID++)
			{
				if (optionCategoryID.GetEnumName() == enumName)
				{
					return optionCategoryID;
				}
			}
			return OptionCategoryID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionCategoryID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionCategoryID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionCategoryID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionCategoryID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static uint GetMainColor(this OptionCategoryID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].MainColor;
			}
			return 0u;
		}

		public static string GetFilename(this OptionCategoryID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Filename;
			}
			return "";
		}

		static OptionCategoryIDEnum()
		{
			records = new OptionCategoryTableRecord[5]
			{
				new OptionCategoryTableRecord(0, "SpeedSetting", "速度设定", "Speed", 4290405878u, "UI_CMN_TabTitle_Option_01"),
				new OptionCategoryTableRecord(1, "GameSetting", "游戏设定", "Game", 4290405878u, "UI_CMN_TabTitle_Option_02"),
				new OptionCategoryTableRecord(2, "JudgeSetting", "判定表示设定", "Judge", 4290405878u, "UI_CMN_TabTitle_Option_03"),
				new OptionCategoryTableRecord(3, "DesignSetting", "美术设定", "Design", 4290405878u, "UI_CMN_TabTitle_Option_04"),
				new OptionCategoryTableRecord(4, "SoundSetting", "声音设定", "Sound", 4290405878u, "UI_CMN_TabTitle_Option_05")
			};
		}
	}
}
