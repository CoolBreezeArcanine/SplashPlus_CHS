namespace DB
{
	public static class OptionSlidespeedIDEnum
	{
		private static readonly OptionSlidespeedTableRecord[] records = new OptionSlidespeedTableRecord[21]
		{
			new OptionSlidespeedTableRecord(0, "Fast1_0", "-1.0", "-1.0", "", "", "UI_OPT_A_03_01", 0),
			new OptionSlidespeedTableRecord(1, "Fast0_9", "-0.9", "-0.9", "", "", "UI_OPT_A_03_01", 0),
			new OptionSlidespeedTableRecord(2, "Fast0_8", "-0.8", "-0.8", "", "", "UI_OPT_A_03_01", 0),
			new OptionSlidespeedTableRecord(3, "Fast0_7", "-0.7", "-0.7", "", "", "UI_OPT_A_03_01", 0),
			new OptionSlidespeedTableRecord(4, "Fast0_6", "-0.6", "-0.6", "", "", "UI_OPT_A_03_01", 0),
			new OptionSlidespeedTableRecord(5, "Fast0_5", "-0.5", "-0.5", "", "", "UI_OPT_A_03_01", 0),
			new OptionSlidespeedTableRecord(6, "Fast0_4", "-0.4", "-0.4", "", "", "UI_OPT_A_03_01", 0),
			new OptionSlidespeedTableRecord(7, "Fast0_3", "-0.3", "-0.3", "", "", "UI_OPT_A_03_01", 0),
			new OptionSlidespeedTableRecord(8, "Fast0_2", "-0.2", "-0.2", "", "", "UI_OPT_A_03_01", 0),
			new OptionSlidespeedTableRecord(9, "Fast0_1", "-0.1", "-0.1", "", "", "UI_OPT_A_03_01", 0),
			new OptionSlidespeedTableRecord(10, "Normal", "0.0", "0.0", "", "", "UI_OPT_A_03_01", 1),
			new OptionSlidespeedTableRecord(11, "Late0_1", "0.1", "0.1", "", "", "UI_OPT_A_03_01", 0),
			new OptionSlidespeedTableRecord(12, "Late0_2", "0.2", "0.2", "", "", "UI_OPT_A_03_01", 0),
			new OptionSlidespeedTableRecord(13, "Late0_3", "0.3", "0.3", "", "", "UI_OPT_A_03_01", 0),
			new OptionSlidespeedTableRecord(14, "Late0_4", "0.4", "0.4", "", "", "UI_OPT_A_03_01", 0),
			new OptionSlidespeedTableRecord(15, "Late0_5", "0.5", "0.5", "", "", "UI_OPT_A_03_01", 0),
			new OptionSlidespeedTableRecord(16, "Late0_6", "0.6", "0.6", "", "", "UI_OPT_A_03_01", 0),
			new OptionSlidespeedTableRecord(17, "Late0_7", "0.7", "0.7", "", "", "UI_OPT_A_03_01", 0),
			new OptionSlidespeedTableRecord(18, "Late0_8", "0.8", "0.8", "", "", "UI_OPT_A_03_01", 0),
			new OptionSlidespeedTableRecord(19, "Late0_9", "0.9", "0.9", "", "", "UI_OPT_A_03_01", 0),
			new OptionSlidespeedTableRecord(20, "Late1_0", "1.0", "1.0", "", "", "UI_OPT_A_03_01", 0)
		};

		public static bool IsActive(this OptionSlidespeedID self)
		{
			if (self >= OptionSlidespeedID.Fast1_0 && self < OptionSlidespeedID.End)
			{
				return self != OptionSlidespeedID.Fast1_0;
			}
			return false;
		}

		public static bool IsValid(this OptionSlidespeedID self)
		{
			if (self >= OptionSlidespeedID.Fast1_0)
			{
				return self < OptionSlidespeedID.End;
			}
			return false;
		}

		public static void Clamp(this OptionSlidespeedID self)
		{
			if (self < OptionSlidespeedID.Fast1_0)
			{
				self = OptionSlidespeedID.Fast1_0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionSlidespeedID)GetEnd();
			}
		}

		public static int GetEnd(this OptionSlidespeedID self)
		{
			return GetEnd();
		}

		public static OptionSlidespeedID FindID(string enumName)
		{
			for (OptionSlidespeedID optionSlidespeedID = OptionSlidespeedID.Fast1_0; optionSlidespeedID < OptionSlidespeedID.End; optionSlidespeedID++)
			{
				if (optionSlidespeedID.GetEnumName() == enumName)
				{
					return optionSlidespeedID;
				}
			}
			return OptionSlidespeedID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionSlidespeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionSlidespeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionSlidespeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionSlidespeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionSlidespeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionSlidespeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this OptionSlidespeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		public static bool IsDefault(this OptionSlidespeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}
	}
}
