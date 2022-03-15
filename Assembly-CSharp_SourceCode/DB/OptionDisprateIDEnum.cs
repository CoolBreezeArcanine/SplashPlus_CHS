namespace DB
{
	public static class OptionDisprateIDEnum
	{
		private static readonly OptionDisprateTableRecord[] records = new OptionDisprateTableRecord[8]
		{
			new OptionDisprateTableRecord(0, "AllDisp", "全て表示", "", "", "", "UI_OPT_B_11_01", 1),
			new OptionDisprateTableRecord(1, "DispRateDan", "レーティングと段位表示", "", "", "", "UI_OPT_B_11_02", 0),
			new OptionDisprateTableRecord(2, "DispRateClass", "レーティングとクラス表示", "", "", "", "UI_OPT_B_11_03", 0),
			new OptionDisprateTableRecord(3, "DispDanClass", "段位とクラス表示", "", "", "", "UI_OPT_B_11_04", 0),
			new OptionDisprateTableRecord(4, "DispRate", "レーティングのみ表示", "", "", "", "UI_OPT_B_11_05", 0),
			new OptionDisprateTableRecord(5, "DispDan", "段位のみ表示", "", "", "", "UI_OPT_B_11_06", 0),
			new OptionDisprateTableRecord(6, "DispClass", "クラスのみ表示", "", "", "", "UI_OPT_B_11_07", 0),
			new OptionDisprateTableRecord(7, "Hide", "全て非表示", "", "", "", "UI_OPT_B_11_08", 0)
		};

		public static bool IsActive(this OptionDisprateID self)
		{
			if (self >= OptionDisprateID.AllDisp && self < OptionDisprateID.End)
			{
				return self != OptionDisprateID.AllDisp;
			}
			return false;
		}

		public static bool IsValid(this OptionDisprateID self)
		{
			if (self >= OptionDisprateID.AllDisp)
			{
				return self < OptionDisprateID.End;
			}
			return false;
		}

		public static void Clamp(this OptionDisprateID self)
		{
			if (self < OptionDisprateID.AllDisp)
			{
				self = OptionDisprateID.AllDisp;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionDisprateID)GetEnd();
			}
		}

		public static int GetEnd(this OptionDisprateID self)
		{
			return GetEnd();
		}

		public static OptionDisprateID FindID(string enumName)
		{
			for (OptionDisprateID optionDisprateID = OptionDisprateID.AllDisp; optionDisprateID < OptionDisprateID.End; optionDisprateID++)
			{
				if (optionDisprateID.GetEnumName() == enumName)
				{
					return optionDisprateID;
				}
			}
			return OptionDisprateID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionDisprateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionDisprateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionDisprateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionDisprateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionDisprateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionDisprateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this OptionDisprateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		public static bool IsDefault(this OptionDisprateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}
	}
}
