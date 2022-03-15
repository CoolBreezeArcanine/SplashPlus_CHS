namespace DB
{
	public static class OptionCriticalIDEnum
	{
		private static readonly OptionCriticalTableRecord[] records;

		public static bool IsActive(this OptionCriticalID self)
		{
			if (self >= OptionCriticalID.Default && self < OptionCriticalID.End)
			{
				return self != OptionCriticalID.Default;
			}
			return false;
		}

		public static bool IsValid(this OptionCriticalID self)
		{
			if (self >= OptionCriticalID.Default)
			{
				return self < OptionCriticalID.End;
			}
			return false;
		}

		public static void Clamp(this OptionCriticalID self)
		{
			if (self < OptionCriticalID.Default)
			{
				self = OptionCriticalID.Default;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionCriticalID)GetEnd();
			}
		}

		public static int GetEnd(this OptionCriticalID self)
		{
			return GetEnd();
		}

		public static OptionCriticalID FindID(string enumName)
		{
			for (OptionCriticalID optionCriticalID = OptionCriticalID.Default; optionCriticalID < OptionCriticalID.End; optionCriticalID++)
			{
				if (optionCriticalID.GetEnumName() == enumName)
				{
					return optionCriticalID;
				}
			}
			return OptionCriticalID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionCriticalID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionCriticalID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionCriticalID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionCriticalID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionCriticalID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionCriticalID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static bool IsDefault(this OptionCriticalID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		public static string GetFilePath(this OptionCriticalID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		static OptionCriticalIDEnum()
		{
			records = new OptionCriticalTableRecord[4]
			{
				new OptionCriticalTableRecord(0, "Default", "GOOD～PERFECT", string.Empty, string.Empty, string.Empty, 1, "UI_OPT_E_24_01"),
				new OptionCriticalTableRecord(1, "CriticalOn", "GOOD～CRITICAL PERFECT", string.Empty, string.Empty, string.Empty, 0, "UI_OPT_E_24_01"),
				new OptionCriticalTableRecord(2, "CriticalOnly", "只有CRITICAL PERFECT", string.Empty, string.Empty, string.Empty, 0, "UI_OPT_E_24_01"),
				new OptionCriticalTableRecord(3, "NotPerfect", "PERFECT或以下时", string.Empty, string.Empty, string.Empty, 0, "UI_OPT_E_24_01")
			};
		}
	}
}
