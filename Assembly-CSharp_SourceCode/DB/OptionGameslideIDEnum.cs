namespace DB
{
	public static class OptionGameslideIDEnum
	{
		private static readonly OptionGameslideTableRecord[] records;

		public static bool IsActive(this OptionGameslideID self)
		{
			if (self >= OptionGameslideID.Cute && self < OptionGameslideID.End)
			{
				return self != OptionGameslideID.Cute;
			}
			return false;
		}

		public static bool IsValid(this OptionGameslideID self)
		{
			if (self >= OptionGameslideID.Cute)
			{
				return self < OptionGameslideID.End;
			}
			return false;
		}

		public static void Clamp(this OptionGameslideID self)
		{
			if (self < OptionGameslideID.Cute)
			{
				self = OptionGameslideID.Cute;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionGameslideID)GetEnd();
			}
		}

		public static int GetEnd(this OptionGameslideID self)
		{
			return GetEnd();
		}

		public static OptionGameslideID FindID(string enumName)
		{
			for (OptionGameslideID optionGameslideID = OptionGameslideID.Cute; optionGameslideID < OptionGameslideID.End; optionGameslideID++)
			{
				if (optionGameslideID.GetEnumName() == enumName)
				{
					return optionGameslideID;
				}
			}
			return OptionGameslideID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionGameslideID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionGameslideID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionGameslideID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionGameslideID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionGameslideID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionGameslideID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static bool IsDefault(this OptionGameslideID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		public static string GetFilePath(this OptionGameslideID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		static OptionGameslideIDEnum()
		{
			records = new OptionGameslideTableRecord[2]
			{
				new OptionGameslideTableRecord(0, "Cute", "简洁", string.Empty, string.Empty, string.Empty, 1, "UI_OPT_D_20_01"),
				new OptionGameslideTableRecord(1, "Legacy", "经典", string.Empty, string.Empty, string.Empty, 0, "UI_OPT_D_20_02")
			};
		}
	}
}
