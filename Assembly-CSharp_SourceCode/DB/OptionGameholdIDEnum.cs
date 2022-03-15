namespace DB
{
	public static class OptionGameholdIDEnum
	{
		private static readonly OptionGameholdTableRecord[] records;

		public static bool IsActive(this OptionGameholdID self)
		{
			if (self >= OptionGameholdID.Cute && self < OptionGameholdID.End)
			{
				return self != OptionGameholdID.Cute;
			}
			return false;
		}

		public static bool IsValid(this OptionGameholdID self)
		{
			if (self >= OptionGameholdID.Cute)
			{
				return self < OptionGameholdID.End;
			}
			return false;
		}

		public static void Clamp(this OptionGameholdID self)
		{
			if (self < OptionGameholdID.Cute)
			{
				self = OptionGameholdID.Cute;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionGameholdID)GetEnd();
			}
		}

		public static int GetEnd(this OptionGameholdID self)
		{
			return GetEnd();
		}

		public static OptionGameholdID FindID(string enumName)
		{
			for (OptionGameholdID optionGameholdID = OptionGameholdID.Cute; optionGameholdID < OptionGameholdID.End; optionGameholdID++)
			{
				if (optionGameholdID.GetEnumName() == enumName)
				{
					return optionGameholdID;
				}
			}
			return OptionGameholdID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionGameholdID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionGameholdID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionGameholdID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionGameholdID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionGameholdID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionGameholdID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static bool IsDefault(this OptionGameholdID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		public static string GetFilePath(this OptionGameholdID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		static OptionGameholdIDEnum()
		{
			records = new OptionGameholdTableRecord[2]
			{
				new OptionGameholdTableRecord(0, "Cute", "简洁", string.Empty, string.Empty, string.Empty, 1, "UI_OPT_D_19_01"),
				new OptionGameholdTableRecord(1, "Legacy", "经典", string.Empty, string.Empty, string.Empty, 0, "UI_OPT_D_19_02")
			};
		}
	}
}
