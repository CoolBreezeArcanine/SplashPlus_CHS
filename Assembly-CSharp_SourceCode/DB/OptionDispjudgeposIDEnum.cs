namespace DB
{
	public static class OptionDispjudgeposIDEnum
	{
		private static readonly OptionDispjudgeposTableRecord[] records;

		public static bool IsActive(this OptionDispjudgeposID self)
		{
			if (self >= OptionDispjudgeposID.Off && self < OptionDispjudgeposID.End)
			{
				return self != OptionDispjudgeposID.Off;
			}
			return false;
		}

		public static bool IsValid(this OptionDispjudgeposID self)
		{
			if (self >= OptionDispjudgeposID.Off)
			{
				return self < OptionDispjudgeposID.End;
			}
			return false;
		}

		public static void Clamp(this OptionDispjudgeposID self)
		{
			if (self < OptionDispjudgeposID.Off)
			{
				self = OptionDispjudgeposID.Off;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionDispjudgeposID)GetEnd();
			}
		}

		public static int GetEnd(this OptionDispjudgeposID self)
		{
			return GetEnd();
		}

		public static OptionDispjudgeposID FindID(string enumName)
		{
			for (OptionDispjudgeposID optionDispjudgeposID = OptionDispjudgeposID.Off; optionDispjudgeposID < OptionDispjudgeposID.End; optionDispjudgeposID++)
			{
				if (optionDispjudgeposID.GetEnumName() == enumName)
				{
					return optionDispjudgeposID;
				}
			}
			return OptionDispjudgeposID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionDispjudgeposID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionDispjudgeposID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionDispjudgeposID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionDispjudgeposID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionDispjudgeposID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionDispjudgeposID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this OptionDispjudgeposID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		public static bool IsDefault(this OptionDispjudgeposID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		static OptionDispjudgeposIDEnum()
		{
			records = new OptionDispjudgeposTableRecord[6]
			{
				new OptionDispjudgeposTableRecord(0, "Off", "OFF", string.Empty, string.Empty, string.Empty, "UI_OPT_C_16_01", 0),
				new OptionDispjudgeposTableRecord(1, "In", "内侧○・・・・外侧", string.Empty, string.Empty, string.Empty, "UI_OPT_C_16_06", 0),
				new OptionDispjudgeposTableRecord(2, "In2Mid", "内侧・○・・・外侧", string.Empty, string.Empty, string.Empty, "UI_OPT_C_16_05", 0),
				new OptionDispjudgeposTableRecord(3, "Mid", "内侧・・○・・外侧", string.Empty, string.Empty, string.Empty, "UI_OPT_C_16_04", 0),
				new OptionDispjudgeposTableRecord(4, "Mid2Out", "内侧・・・○・外侧", string.Empty, string.Empty, string.Empty, "UI_OPT_C_16_03", 0),
				new OptionDispjudgeposTableRecord(5, "Out", "内侧・・・・○外侧", string.Empty, string.Empty, string.Empty, "UI_OPT_C_16_02", 1)
			};
		}
	}
}
