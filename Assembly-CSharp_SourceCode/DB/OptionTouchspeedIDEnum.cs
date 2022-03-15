namespace DB
{
	public static class OptionTouchspeedIDEnum
	{
		private static readonly OptionTouchspeedTableRecord[] records;

		public static bool IsActive(this OptionTouchspeedID self)
		{
			if (self >= OptionTouchspeedID.Speed1_0 && self < OptionTouchspeedID.End)
			{
				return self != OptionTouchspeedID.Speed1_0;
			}
			return false;
		}

		public static bool IsValid(this OptionTouchspeedID self)
		{
			if (self >= OptionTouchspeedID.Speed1_0)
			{
				return self < OptionTouchspeedID.End;
			}
			return false;
		}

		public static void Clamp(this OptionTouchspeedID self)
		{
			if (self < OptionTouchspeedID.Speed1_0)
			{
				self = OptionTouchspeedID.Speed1_0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionTouchspeedID)GetEnd();
			}
		}

		public static int GetEnd(this OptionTouchspeedID self)
		{
			return GetEnd();
		}

		public static OptionTouchspeedID FindID(string enumName)
		{
			for (OptionTouchspeedID optionTouchspeedID = OptionTouchspeedID.Speed1_0; optionTouchspeedID < OptionTouchspeedID.End; optionTouchspeedID++)
			{
				if (optionTouchspeedID.GetEnumName() == enumName)
				{
					return optionTouchspeedID;
				}
			}
			return OptionTouchspeedID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionTouchspeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionTouchspeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static float GetValue(this OptionTouchspeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Value;
			}
			return 0f;
		}

		public static string GetName(this OptionTouchspeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionTouchspeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionTouchspeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionTouchspeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this OptionTouchspeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		public static bool IsDefault(this OptionTouchspeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		static OptionTouchspeedIDEnum()
		{
			records = new OptionTouchspeedTableRecord[20]
			{
				new OptionTouchspeedTableRecord(0, "Speed1_0", 175f, "1.0", "1.0", "比标准速度慢", string.Empty, "UI_OPT_A_02_01", 0),
				new OptionTouchspeedTableRecord(1, "Speed1_5", 200f, "1.5", "1.5", "比标准速度慢", string.Empty, "UI_OPT_A_02_01", 0),
				new OptionTouchspeedTableRecord(2, "Speed2_0", 225f, "2.0", "2.0", "推荐给选择BASIC的玩家", string.Empty, "UI_OPT_A_02_01", 0),
				new OptionTouchspeedTableRecord(3, "Speed2_5", 250f, "2.5", "2.5", "推荐给选择BASIC的玩家", string.Empty, "UI_OPT_A_02_01", 0),
				new OptionTouchspeedTableRecord(4, "Speed3_0", 275f, "3.0", "3.0", "推荐给选择ADVANCED的玩家", string.Empty, "UI_OPT_A_02_01", 0),
				new OptionTouchspeedTableRecord(5, "Speed3_5", 300f, "3.5", "3.5", "推荐给选择ADVANCED的玩家", string.Empty, "UI_OPT_A_02_01", 0),
				new OptionTouchspeedTableRecord(6, "Speed4_0", 325f, "4.0", "4.0", "推荐给选择ADVANCED的玩家", string.Empty, "UI_OPT_A_02_01", 0),
				new OptionTouchspeedTableRecord(7, "Speed4_5", 350f, "4.5", "4.5", "推荐给选择EXPERT的玩家", string.Empty, "UI_OPT_A_02_01", 1),
				new OptionTouchspeedTableRecord(8, "Speed5_0", 400f, "5.0", "5.0", "推荐给选择EXPERT的玩家", string.Empty, "UI_OPT_A_02_01", 0),
				new OptionTouchspeedTableRecord(9, "Speed5_5", 450f, "5.5", "5.5", "推荐给选择EXPERT的玩家", string.Empty, "UI_OPT_A_02_01", 0),
				new OptionTouchspeedTableRecord(10, "Speed6_0", 500f, "6.0", "6.0", "请按照喜好设定速度", string.Empty, "UI_OPT_A_02_01", 0),
				new OptionTouchspeedTableRecord(11, "Speed6_5", 550f, "6.5", "6.5", "请按照喜好设定速度", string.Empty, "UI_OPT_A_02_01", 0),
				new OptionTouchspeedTableRecord(12, "Speed7_0", 600f, "7.0", "7.0", "请按照喜好设定速度", string.Empty, "UI_OPT_A_02_01", 0),
				new OptionTouchspeedTableRecord(13, "Speed7_5", 650f, "7.5", "7.5", "请按照喜好设定速度", string.Empty, "UI_OPT_A_02_01", 0),
				new OptionTouchspeedTableRecord(14, "Speed8_0", 700f, "8.0", "8.0", "请按照喜好设定速度", string.Empty, "UI_OPT_A_02_01", 0),
				new OptionTouchspeedTableRecord(15, "Speed8_5", 750f, "8.5", "8.5", "请按照喜好设定速度", string.Empty, "UI_OPT_A_02_01", 0),
				new OptionTouchspeedTableRecord(16, "Speed9_0", 800f, "9.0", "9.0", "请按照喜好设定速度", string.Empty, "UI_OPT_A_02_01", 0),
				new OptionTouchspeedTableRecord(17, "Speed9_5", 850f, "9.5", "9.5", "请按照喜好设定速度", string.Empty, "UI_OPT_A_02_01", 0),
				new OptionTouchspeedTableRecord(18, "Speed10_0", 900f, "10.0", "10.0", "请按照喜好设定速度", string.Empty, "UI_OPT_A_02_01", 0),
				new OptionTouchspeedTableRecord(19, "Speed_Sonic", 5000f, "SONIC", "SONIC", "注意！ 光环只会显示一瞬间", string.Empty, "UI_OPT_A_02_01", 0)
			};
		}
	}
}
