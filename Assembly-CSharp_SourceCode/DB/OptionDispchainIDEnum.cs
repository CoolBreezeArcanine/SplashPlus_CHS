namespace DB
{
	public static class OptionDispchainIDEnum
	{
		private static readonly OptionDispchainTableRecord[] records = new OptionDispchainTableRecord[3]
		{
			new OptionDispchainTableRecord(0, "Off", "OFF", "", "", "", "UI_OPT_B_12_02", 0),
			new OptionDispchainTableRecord(1, "Achievement", "達成率差分", "", "", "", "UI_OPT_B_12_03", 0),
			new OptionDispchainTableRecord(2, "Sync", "SYNCカウント", "", "", "", "UI_OPT_B_12_01", 1)
		};

		public static bool IsActive(this OptionDispchainID self)
		{
			if (self >= OptionDispchainID.Off && self < OptionDispchainID.End)
			{
				return self != OptionDispchainID.Off;
			}
			return false;
		}

		public static bool IsValid(this OptionDispchainID self)
		{
			if (self >= OptionDispchainID.Off)
			{
				return self < OptionDispchainID.End;
			}
			return false;
		}

		public static void Clamp(this OptionDispchainID self)
		{
			if (self < OptionDispchainID.Off)
			{
				self = OptionDispchainID.Off;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionDispchainID)GetEnd();
			}
		}

		public static int GetEnd(this OptionDispchainID self)
		{
			return GetEnd();
		}

		public static OptionDispchainID FindID(string enumName)
		{
			for (OptionDispchainID optionDispchainID = OptionDispchainID.Off; optionDispchainID < OptionDispchainID.End; optionDispchainID++)
			{
				if (optionDispchainID.GetEnumName() == enumName)
				{
					return optionDispchainID;
				}
			}
			return OptionDispchainID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionDispchainID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionDispchainID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionDispchainID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionDispchainID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionDispchainID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionDispchainID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this OptionDispchainID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		public static bool IsDefault(this OptionDispchainID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}
	}
}
