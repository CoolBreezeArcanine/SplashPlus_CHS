namespace DB
{
	public static class OptionSubmonAchiveIDEnum
	{
		private static readonly OptionSubmonAchiveTableRecord[] records;

		public static bool IsActive(this OptionSubmonAchiveID self)
		{
			if (self >= OptionSubmonAchiveID.AchivePlus && self < OptionSubmonAchiveID.End)
			{
				return self != OptionSubmonAchiveID.AchivePlus;
			}
			return false;
		}

		public static bool IsValid(this OptionSubmonAchiveID self)
		{
			if (self >= OptionSubmonAchiveID.AchivePlus)
			{
				return self < OptionSubmonAchiveID.End;
			}
			return false;
		}

		public static void Clamp(this OptionSubmonAchiveID self)
		{
			if (self < OptionSubmonAchiveID.AchivePlus)
			{
				self = OptionSubmonAchiveID.AchivePlus;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionSubmonAchiveID)GetEnd();
			}
		}

		public static int GetEnd(this OptionSubmonAchiveID self)
		{
			return GetEnd();
		}

		public static OptionSubmonAchiveID FindID(string enumName)
		{
			for (OptionSubmonAchiveID optionSubmonAchiveID = OptionSubmonAchiveID.AchivePlus; optionSubmonAchiveID < OptionSubmonAchiveID.End; optionSubmonAchiveID++)
			{
				if (optionSubmonAchiveID.GetEnumName() == enumName)
				{
					return optionSubmonAchiveID;
				}
			}
			return OptionSubmonAchiveID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionSubmonAchiveID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionSubmonAchiveID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionSubmonAchiveID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionSubmonAchiveID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionSubmonAchiveID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionSubmonAchiveID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this OptionSubmonAchiveID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		public static bool IsDefault(this OptionSubmonAchiveID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		static OptionSubmonAchiveIDEnum()
		{
			records = new OptionSubmonAchiveTableRecord[2]
			{
				new OptionSubmonAchiveTableRecord(0, "AchivePlus", "类型＋", string.Empty, string.Empty, string.Empty, "UI_OPT_B_13_01", 1),
				new OptionSubmonAchiveTableRecord(1, "AchiveMinus", "类型－", string.Empty, string.Empty, string.Empty, "UI_OPT_B_13_02", 0)
			};
		}
	}
}
