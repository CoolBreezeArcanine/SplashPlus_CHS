namespace DB
{
	public static class OptionDispjudgeIDEnum
	{
		private static readonly OptionDispjudgeTableRecord[] records = new OptionDispjudgeTableRecord[13]
		{
			new OptionDispjudgeTableRecord(0, "Type1A", "タイプ1-A", "", "通常判定のみ。CRITICAL PERFECT無し。", "", "UI_OPT_C_15_01", 1, 0, 0),
			new OptionDispjudgeTableRecord(1, "Type1B", "タイプ1-B", "", "GREAT以下2行表示。CRITICAL PERFECT無し。", "", "UI_OPT_C_15_02", 0, 0, 1),
			new OptionDispjudgeTableRecord(2, "Type1C", "タイプ1-C", "", "GREAT以下判定の色をFAST/LATEに変更。CRITICAL PERFECT無し。", "", "UI_OPT_C_15_04", 0, 0, 0),
			new OptionDispjudgeTableRecord(3, "Type1D", "タイプ1-D", "", "GREAT以下FAST/LATEだけで表示。PERFECT以上の判定表示は無し。", "", "UI_OPT_C_15_05", 0, 0, 1),
			new OptionDispjudgeTableRecord(4, "Type1E", "タイプ1-E", "", "GREAT以下2行表示。CRITICAL PERFECT無し。BREAKはCRITICAL以外FAST/LATE表示。", "", "UI_OPT_C_15_03", 0, 0, 1),
			new OptionDispjudgeTableRecord(5, "Type2A", "タイプ2-A", "", "通常判定のみ。CRITICAL PERFECT有り。", "", "UI_OPT_C_15_06", 0, 1, 0),
			new OptionDispjudgeTableRecord(6, "Type2B", "タイプ2-B", "", "GREAT以下2行表示。CRITICAL PERFECT有り。", "", "UI_OPT_C_15_08", 0, 1, 1),
			new OptionDispjudgeTableRecord(7, "Type2C", "タイプ2-C", "", "GREAT以下判定の色をFAST/LATEに変更。CRITICAL PERFECT有り。", "", "UI_OPT_C_15_09", 0, 1, 0),
			new OptionDispjudgeTableRecord(8, "Type2D", "タイプ2-D", "", "GREAT以下FAST/LATEだけで表示。PERFECT以上の判定表示は無し。", "", "UI_OPT_C_15_10", 0, 1, 1),
			new OptionDispjudgeTableRecord(9, "Type2E", "タイプ2-E", "", "GREAT以下2行表示。CRITICAL PERFECT有り。BREAKはCRITICAL以外FAST/LATE表示。", "", "UI_OPT_C_15_07", 0, 1, 1),
			new OptionDispjudgeTableRecord(10, "Type3B", "タイプ3-B", "", "PERFECT以下2行表示。CRITICAL PERFECT有り。", "", "UI_OPT_C_15_11", 0, 1, 1),
			new OptionDispjudgeTableRecord(11, "Type3C", "タイプ3-C", "", "PERFECT以下判定の色をFAST/LATEに変更。CRITICAL PERFECT有り。", "", "UI_OPT_C_15_12", 0, 1, 0),
			new OptionDispjudgeTableRecord(12, "Type3D", "タイプ3-D", "", "PERFECT以下FAST/LATEだけで表示。CRITICAL PERFECTの判定表示は無し。", "", "UI_OPT_C_15_13", 0, 1, 1)
		};

		public static bool IsActive(this OptionDispjudgeID self)
		{
			if (self >= OptionDispjudgeID.Type1A && self < OptionDispjudgeID.End)
			{
				return self != OptionDispjudgeID.Type1A;
			}
			return false;
		}

		public static bool IsValid(this OptionDispjudgeID self)
		{
			if (self >= OptionDispjudgeID.Type1A)
			{
				return self < OptionDispjudgeID.End;
			}
			return false;
		}

		public static void Clamp(this OptionDispjudgeID self)
		{
			if (self < OptionDispjudgeID.Type1A)
			{
				self = OptionDispjudgeID.Type1A;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionDispjudgeID)GetEnd();
			}
		}

		public static int GetEnd(this OptionDispjudgeID self)
		{
			return GetEnd();
		}

		public static OptionDispjudgeID FindID(string enumName)
		{
			for (OptionDispjudgeID optionDispjudgeID = OptionDispjudgeID.Type1A; optionDispjudgeID < OptionDispjudgeID.End; optionDispjudgeID++)
			{
				if (optionDispjudgeID.GetEnumName() == enumName)
				{
					return optionDispjudgeID;
				}
			}
			return OptionDispjudgeID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionDispjudgeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionDispjudgeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionDispjudgeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionDispjudgeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionDispjudgeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionDispjudgeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this OptionDispjudgeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		public static bool IsDefault(this OptionDispjudgeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		public static bool IsCritical(this OptionDispjudgeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isCritical;
			}
			return false;
		}

		public static bool IsFastlate(this OptionDispjudgeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isFastlate;
			}
			return false;
		}
	}
}
