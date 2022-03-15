namespace DB
{
	public static class OptionAppealIDEnum
	{
		private static readonly OptionAppealTableRecord[] records;

		public static bool IsActive(this OptionAppealID self)
		{
			if (self >= OptionAppealID.OFF && self < OptionAppealID.End)
			{
				return self != OptionAppealID.OFF;
			}
			return false;
		}

		public static bool IsValid(this OptionAppealID self)
		{
			if (self >= OptionAppealID.OFF)
			{
				return self < OptionAppealID.End;
			}
			return false;
		}

		public static void Clamp(this OptionAppealID self)
		{
			if (self < OptionAppealID.OFF)
			{
				self = OptionAppealID.OFF;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionAppealID)GetEnd();
			}
		}

		public static int GetEnd(this OptionAppealID self)
		{
			return GetEnd();
		}

		public static OptionAppealID FindID(string enumName)
		{
			for (OptionAppealID optionAppealID = OptionAppealID.OFF; optionAppealID < OptionAppealID.End; optionAppealID++)
			{
				if (optionAppealID.GetEnumName() == enumName)
				{
					return optionAppealID;
				}
			}
			return OptionAppealID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionAppealID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionAppealID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionAppealID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionAppealID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionAppealID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionAppealID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this OptionAppealID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		public static bool IsDefault(this OptionAppealID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		static OptionAppealIDEnum()
		{
			records = new OptionAppealTableRecord[6]
			{
				new OptionAppealTableRecord(0, "OFF", "OFF", string.Empty, string.Empty, string.Empty, "UI_OPT_B_14_01", 1),
				new OptionAppealTableRecord(1, "Together", "不一起来玩吗！", string.Empty, string.Empty, string.Empty, "UI_OPT_B_14_02", 0),
				new OptionAppealTableRecord(2, "Tiho", "一起前进吗？", string.Empty, string.Empty, string.Empty, "UI_OPT_B_14_03", 0),
				new OptionAppealTableRecord(3, "GoldPass", "持有金卡通行证！", "", "", "", "UI_OPT_B_14_04", 0),
				new OptionAppealTableRecord(4, "FullSync", "以完全同步为目标吧！", string.Empty, string.Empty, string.Empty, "UI_OPT_B_14_05", 0),
				new OptionAppealTableRecord(5, "AllPlay", "全制霸者募集！", string.Empty, string.Empty, string.Empty, "UI_OPT_B_14_06", 0)
			};
		}
	}
}
