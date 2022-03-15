namespace DB
{
	public static class OptionExseIDEnum
	{
		private static readonly OptionExseTableRecord[] records;

		public static bool IsActive(this OptionExseID self)
		{
			if (self >= OptionExseID.Se1 && self < OptionExseID.End)
			{
				return self != OptionExseID.Se1;
			}
			return false;
		}

		public static bool IsValid(this OptionExseID self)
		{
			if (self >= OptionExseID.Se1)
			{
				return self < OptionExseID.End;
			}
			return false;
		}

		public static void Clamp(this OptionExseID self)
		{
			if (self < OptionExseID.Se1)
			{
				self = OptionExseID.Se1;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionExseID)GetEnd();
			}
		}

		public static int GetEnd(this OptionExseID self)
		{
			return GetEnd();
		}

		public static OptionExseID FindID(string enumName)
		{
			for (OptionExseID optionExseID = OptionExseID.Se1; optionExseID < OptionExseID.End; optionExseID++)
			{
				if (optionExseID.GetEnumName() == enumName)
				{
					return optionExseID;
				}
			}
			return OptionExseID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionExseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionExseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionExseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionExseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionExseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionExseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetSeEnum(this OptionExseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].SeEnum;
			}
			return "";
		}

		public static bool IsDefault(this OptionExseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		public static string GetFilePath(this OptionExseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		static OptionExseIDEnum()
		{
			records = new OptionExseTableRecord[7]
			{
				new OptionExseTableRecord(0, "Se1", "默认", string.Empty, string.Empty, string.Empty, "SE_GAME_EXTAP_000001", 1, "UI_OPT_E_24_01"),
				new OptionExseTableRecord(1, "Se2", "电子音1", string.Empty, string.Empty, string.Empty, "SE_GAME_EXTAP_000002", 0, "UI_OPT_E_24_01"),
				new OptionExseTableRecord(2, "Se3", "电子音2", string.Empty, string.Empty, string.Empty, "SE_GAME_EXTAP_000003", 0, "UI_OPT_E_24_01"),
				new OptionExseTableRecord(3, "Se4", "歌舞伎", string.Empty, string.Empty, string.Empty, "SE_GAME_EXTAP_000004", 0, "UI_OPT_E_24_01"),
				new OptionExseTableRecord(4, "Se5", "猫", string.Empty, string.Empty, string.Empty, "SE_GAME_EXTAP_000005", 0, "UI_OPT_E_24_01"),
				new OptionExseTableRecord(5, "SE6", "CHUNITHM", "", "", "", "SE_GAME_EXTAP_000006", 0, "UI_OPT_E_24_01"),
				new OptionExseTableRecord(6, "SE7", "音击", "", "", "", "SE_GAME_EXTAP_000007", 0, "UI_OPT_E_24_01")
			};
		}
	}
}
