namespace DB
{
	public static class OptionNotespeedIDEnum
	{
		private static readonly OptionNotespeedTableRecord[] records;

		public static bool IsActive(this OptionNotespeedID self)
		{
			if (self >= OptionNotespeedID.Speed1_0 && self < OptionNotespeedID.End)
			{
				return self != OptionNotespeedID.Speed1_0;
			}
			return false;
		}

		public static bool IsValid(this OptionNotespeedID self)
		{
			if (self >= OptionNotespeedID.Speed1_0)
			{
				return self < OptionNotespeedID.End;
			}
			return false;
		}

		public static void Clamp(this OptionNotespeedID self)
		{
			if (self < OptionNotespeedID.Speed1_0)
			{
				self = OptionNotespeedID.Speed1_0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionNotespeedID)GetEnd();
			}
		}

		public static int GetEnd(this OptionNotespeedID self)
		{
			return GetEnd();
		}

		public static OptionNotespeedID FindID(string enumName)
		{
			for (OptionNotespeedID optionNotespeedID = OptionNotespeedID.Speed1_0; optionNotespeedID < OptionNotespeedID.End; optionNotespeedID++)
			{
				if (optionNotespeedID.GetEnumName() == enumName)
				{
					return optionNotespeedID;
				}
			}
			return OptionNotespeedID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionNotespeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionNotespeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static float GetValue(this OptionNotespeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Value;
			}
			return 0f;
		}

		public static string GetName(this OptionNotespeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionNotespeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionNotespeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionNotespeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this OptionNotespeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		public static bool IsDefault(this OptionNotespeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		static OptionNotespeedIDEnum()
		{
			records = new OptionNotespeedTableRecord[20]
			{
				new OptionNotespeedTableRecord(0, "Speed1_0", 200f, "1.0", "1.0", "比标准速度慢", string.Empty, "UI_OPT_A_01_01", 0),
				new OptionNotespeedTableRecord(1, "Speed1_5", 250f, "1.5", "1.5", "比标准速度慢", string.Empty, "UI_OPT_A_01_01", 0),
				new OptionNotespeedTableRecord(2, "Speed2_0", 300f, "2.0", "2.0", "推荐给选择BASIC的玩家", string.Empty, "UI_OPT_A_01_01", 0),
				new OptionNotespeedTableRecord(3, "Speed2_5", 350f, "2.5", "2.5", "推荐给选择BASIC的玩家", string.Empty, "UI_OPT_A_01_01", 0),
				new OptionNotespeedTableRecord(4, "Speed3_0", 400f, "3.0", "3.0", "推荐给选择ADVANCED的玩家", string.Empty, "UI_OPT_A_01_01", 0),
				new OptionNotespeedTableRecord(5, "Speed3_5", 450f, "3.5", "3.5", "推荐给选择ADVANCED的玩家", string.Empty, "UI_OPT_A_01_01", 0),
				new OptionNotespeedTableRecord(6, "Speed4_0", 500f, "4.0", "4.0", "推荐给选择ADVANCED的玩家", string.Empty, "UI_OPT_A_01_01", 0),
				new OptionNotespeedTableRecord(7, "Speed4_5", 550f, "4.5", "4.5", "推荐给选择EXPERT的玩家", string.Empty, "UI_OPT_A_01_01", 1),
				new OptionNotespeedTableRecord(8, "Speed5_0", 600f, "5.0", "5.0", "推荐给选择EXPERT的玩家", string.Empty, "UI_OPT_A_01_01", 0),
				new OptionNotespeedTableRecord(9, "Speed5_5", 650f, "5.5", "5.5", "推荐给选择EXPERT的玩家", string.Empty, "UI_OPT_A_01_01", 0),
				new OptionNotespeedTableRecord(10, "Speed6_0", 700f, "6.0", "6.0", "请按照喜好设定速度", string.Empty, "UI_OPT_A_01_01", 0),
				new OptionNotespeedTableRecord(11, "Speed6_5", 750f, "6.5", "6.5", "请按照喜好设定速度", string.Empty, "UI_OPT_A_01_01", 0),
				new OptionNotespeedTableRecord(12, "Speed7_0", 800f, "7.0", "7.0", "请按照喜好设定速度", string.Empty, "UI_OPT_A_01_01", 0),
				new OptionNotespeedTableRecord(13, "Speed7_5", 850f, "7.5", "7.5", "请按照喜好设定速度", string.Empty, "UI_OPT_A_01_01", 0),
				new OptionNotespeedTableRecord(14, "Speed8_0", 900f, "8.0", "8.0", "请按照喜好设定速度", string.Empty, "UI_OPT_A_01_01", 0),
				new OptionNotespeedTableRecord(15, "Speed8_5", 950f, "8.5", "8.5", "请按照喜好设定速度", string.Empty, "UI_OPT_A_01_01", 0),
				new OptionNotespeedTableRecord(16, "Speed9_0", 1000f, "9.0", "9.0", "请按照喜好设定速度", string.Empty, "UI_OPT_A_01_01", 0),
				new OptionNotespeedTableRecord(17, "Speed9_5", 1050f, "9.5", "9.5", "请按照喜好设定速度", string.Empty, "UI_OPT_A_01_01", 0),
				new OptionNotespeedTableRecord(18, "Speed10_0", 1100f, "10.0", "10.0", "请按照喜好设定速度", string.Empty, "UI_OPT_A_01_01", 0),
				new OptionNotespeedTableRecord(19, "Speed_Sonic", 5000f, "SONIC", "SONIC", "注意！ 光环只会显示一瞬间", string.Empty, "UI_OPT_A_01_01", 0)
			};
		}
	}
}
