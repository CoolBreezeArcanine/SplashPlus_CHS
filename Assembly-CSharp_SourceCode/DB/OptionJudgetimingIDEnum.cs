namespace DB
{
	public static class OptionJudgetimingIDEnum
	{
		private static readonly OptionJudgetimingTableRecord[] records;

		public static bool IsActive(this OptionJudgetimingID self)
		{
			if (self >= OptionJudgetimingID.Fast2_0 && self < OptionJudgetimingID.End)
			{
				return self != OptionJudgetimingID.Fast2_0;
			}
			return false;
		}

		public static bool IsValid(this OptionJudgetimingID self)
		{
			if (self >= OptionJudgetimingID.Fast2_0)
			{
				return self < OptionJudgetimingID.End;
			}
			return false;
		}

		public static void Clamp(this OptionJudgetimingID self)
		{
			if (self < OptionJudgetimingID.Fast2_0)
			{
				self = OptionJudgetimingID.Fast2_0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionJudgetimingID)GetEnd();
			}
		}

		public static int GetEnd(this OptionJudgetimingID self)
		{
			return GetEnd();
		}

		public static OptionJudgetimingID FindID(string enumName)
		{
			for (OptionJudgetimingID optionJudgetimingID = OptionJudgetimingID.Fast2_0; optionJudgetimingID < OptionJudgetimingID.End; optionJudgetimingID++)
			{
				if (optionJudgetimingID.GetEnumName() == enumName)
				{
					return optionJudgetimingID;
				}
			}
			return OptionJudgetimingID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionJudgetimingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionJudgetimingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionJudgetimingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionJudgetimingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionJudgetimingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionJudgetimingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this OptionJudgetimingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		public static bool IsDefault(this OptionJudgetimingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		static OptionJudgetimingIDEnum()
		{
			records = new OptionJudgetimingTableRecord[41]
			{
				new OptionJudgetimingTableRecord(0, "Fast2_0", "-2.0", "-2.0", "“FAST”较多时请选择这项", string.Empty, "UI_OPT_B_07_01", 0),
				new OptionJudgetimingTableRecord(1, "Fast1_9", "-1.9", "-1.9", "“FAST”较多时请选择这项", string.Empty, "UI_OPT_B_07_01", 0),
				new OptionJudgetimingTableRecord(2, "Fast1_8", "-1.8", "-1.8", "“FAST”较多时请选择这项", string.Empty, "UI_OPT_B_07_01", 0),
				new OptionJudgetimingTableRecord(3, "Fast1_7", "-1.7", "-1.7", "“FAST”较多时请选择这项", string.Empty, "UI_OPT_B_07_01", 0),
				new OptionJudgetimingTableRecord(4, "Fast1_6", "-1.6", "-1.6", "“FAST”较多时请选择这项", string.Empty, "UI_OPT_B_07_01", 0),
				new OptionJudgetimingTableRecord(5, "Fast1_5", "-1.5", "-1.5", "“FAST”较多时请选择这项", string.Empty, "UI_OPT_B_07_01", 0),
				new OptionJudgetimingTableRecord(6, "Fast1_4", "-1.4", "-1.4", "“FAST”较多时请选择这项", string.Empty, "UI_OPT_B_07_01", 0),
				new OptionJudgetimingTableRecord(7, "Fast1_3", "-1.3", "-1.3", "“FAST”较多时请选择这项", string.Empty, "UI_OPT_B_07_01", 0),
				new OptionJudgetimingTableRecord(8, "Fast1_2", "-1.2", "-1.2", "“FAST”较多时请选择这项", string.Empty, "UI_OPT_B_07_01", 0),
				new OptionJudgetimingTableRecord(9, "Fast1_1", "-1.1", "-1.1", "“FAST”较多时请选择这项", string.Empty, "UI_OPT_B_07_01", 0),
				new OptionJudgetimingTableRecord(10, "Fast1_0", "-1.0", "-1.0", "“FAST”较多时请选择这项", string.Empty, "UI_OPT_B_07_01", 0),
				new OptionJudgetimingTableRecord(11, "Fast0_9", "-0.9", "-0.9", "“FAST”较多时请选择这项", string.Empty, "UI_OPT_B_07_01", 0),
				new OptionJudgetimingTableRecord(12, "Fast0_8", "-0.8", "-0.8", "“FAST”较多时请选择这项", string.Empty, "UI_OPT_B_07_01", 0),
				new OptionJudgetimingTableRecord(13, "Fast0_7", "-0.7", "-0.7", "“FAST”较多时请选择这项", string.Empty, "UI_OPT_B_07_01", 0),
				new OptionJudgetimingTableRecord(14, "Fast0_6", "-0.6", "-0.6", "“FAST”较多时请选择这项", string.Empty, "UI_OPT_B_07_01", 0),
				new OptionJudgetimingTableRecord(15, "Fast0_5", "-0.5", "-0.5", "“FAST”较多时请选择这项", string.Empty, "UI_OPT_B_07_01", 0),
				new OptionJudgetimingTableRecord(16, "Fast0_4", "-0.4", "-0.4", "“FAST”较多时请选择这项", string.Empty, "UI_OPT_B_07_01", 0),
				new OptionJudgetimingTableRecord(17, "Fast0_3", "-0.3", "-0.3", "“FAST”较多时请选择这项", string.Empty, "UI_OPT_B_07_01", 0),
				new OptionJudgetimingTableRecord(18, "Fast0_2", "-0.2", "-0.2", "“FAST”较多时请选择这项", string.Empty, "UI_OPT_B_07_01", 0),
				new OptionJudgetimingTableRecord(19, "Fast0_1", "-0.1", "-0.1", "“FAST”较多时请选择这项", string.Empty, "UI_OPT_B_07_01", 0),
				new OptionJudgetimingTableRecord(20, "Normal", "0.0", "0.0", "通常的时机", string.Empty, "UI_OPT_B_07_02", 1),
				new OptionJudgetimingTableRecord(21, "Late0_1", "0.1", "0.1", "“太晚”较多时请选择这项", string.Empty, "UI_OPT_B_07_03", 0),
				new OptionJudgetimingTableRecord(22, "Late0_2", "0.2", "0.2", "“LATE”较多时请选择这项", string.Empty, "UI_OPT_B_07_03", 0),
				new OptionJudgetimingTableRecord(23, "Late0_3", "0.3", "0.3", "“LATE”较多时请选择这项", string.Empty, "UI_OPT_B_07_03", 0),
				new OptionJudgetimingTableRecord(24, "Late0_4", "0.4", "0.4", "“LATE”较多时请选择这项", string.Empty, "UI_OPT_B_07_03", 0),
				new OptionJudgetimingTableRecord(25, "Late0_5", "0.5", "0.5", "“LATE”较多时请选择这项", string.Empty, "UI_OPT_B_07_03", 0),
				new OptionJudgetimingTableRecord(26, "Late0_6", "0.6", "0.6", "“LATE”较多时请选择这项", string.Empty, "UI_OPT_B_07_03", 0),
				new OptionJudgetimingTableRecord(27, "Late0_7", "0.7", "0.7", "“LATE”较多时请选择这项", string.Empty, "UI_OPT_B_07_03", 0),
				new OptionJudgetimingTableRecord(28, "Late0_8", "0.8", "0.8", "“LATE”较多时请选择这项", string.Empty, "UI_OPT_B_07_03", 0),
				new OptionJudgetimingTableRecord(29, "Late0_9", "0.9", "0.9", "“LATE”较多时请选择这项", string.Empty, "UI_OPT_B_07_03", 0),
				new OptionJudgetimingTableRecord(30, "Late1_0", "1.0", "1.0", "“LATE”较多时请选择这项", string.Empty, "UI_OPT_B_07_03", 0),
				new OptionJudgetimingTableRecord(31, "Late1_1", "1.1", "1.1", "“LATE”较多时请选择这项", string.Empty, "UI_OPT_B_07_03", 0),
				new OptionJudgetimingTableRecord(32, "Late1_2", "1.2", "1.2", "“LATE”较多时请选择这项", string.Empty, "UI_OPT_B_07_03", 0),
				new OptionJudgetimingTableRecord(33, "Late1_3", "1.3", "1.3", "“LATE”较多时请选择这项", string.Empty, "UI_OPT_B_07_03", 0),
				new OptionJudgetimingTableRecord(34, "Late1_4", "1.4", "1.4", "“LATE”较多时请选择这项", string.Empty, "UI_OPT_B_07_03", 0),
				new OptionJudgetimingTableRecord(35, "Late1_5", "1.5", "1.5", "“LATE”较多时请选择这项", string.Empty, "UI_OPT_B_07_03", 0),
				new OptionJudgetimingTableRecord(36, "Late1_6", "1.6", "1.6", "“LATE”较多时请选择这项", string.Empty, "UI_OPT_B_07_03", 0),
				new OptionJudgetimingTableRecord(37, "Late1_7", "1.7", "1.7", "“LATE”较多时请选择这项", string.Empty, "UI_OPT_B_07_03", 0),
				new OptionJudgetimingTableRecord(38, "Late1_8", "1.8", "1.8", "“LATE”较多时请选择这项", string.Empty, "UI_OPT_B_07_03", 0),
				new OptionJudgetimingTableRecord(39, "Late1_9", "1.9", "1.9", "“LATE”较多时请选择这项", string.Empty, "UI_OPT_B_07_03", 0),
				new OptionJudgetimingTableRecord(40, "Late2_0", "2.0", "2.0", "“LATE”较多时请选择这项", string.Empty, "UI_OPT_B_07_03", 0)
			};
		}
	}
}
