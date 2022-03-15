namespace DB
{
	public static class OptionStarrotateIDEnum
	{
		private static readonly OptionStarrotateTableRecord[] records;

		public static bool IsActive(this OptionStarrotateID self)
		{
			if (self >= OptionStarrotateID.Off && self < OptionStarrotateID.End)
			{
				return self != OptionStarrotateID.Off;
			}
			return false;
		}

		public static bool IsValid(this OptionStarrotateID self)
		{
			if (self >= OptionStarrotateID.Off)
			{
				return self < OptionStarrotateID.End;
			}
			return false;
		}

		public static void Clamp(this OptionStarrotateID self)
		{
			if (self < OptionStarrotateID.Off)
			{
				self = OptionStarrotateID.Off;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionStarrotateID)GetEnd();
			}
		}

		public static int GetEnd(this OptionStarrotateID self)
		{
			return GetEnd();
		}

		public static OptionStarrotateID FindID(string enumName)
		{
			for (OptionStarrotateID optionStarrotateID = OptionStarrotateID.Off; optionStarrotateID < OptionStarrotateID.End; optionStarrotateID++)
			{
				if (optionStarrotateID.GetEnumName() == enumName)
				{
					return optionStarrotateID;
				}
			}
			return OptionStarrotateID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionStarrotateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionStarrotateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionStarrotateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionStarrotateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionStarrotateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionStarrotateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this OptionStarrotateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		public static bool IsDefault(this OptionStarrotateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		static OptionStarrotateIDEnum()
		{
			records = new OptionStarrotateTableRecord[2]
			{
				new OptionStarrotateTableRecord(0, "Off", "OFF", string.Empty, "☆不旋转", string.Empty, "UI_OPT_B_08_01", 0),
				new OptionStarrotateTableRecord(1, "On", "ON", string.Empty, "☆会旋转", string.Empty, "UI_OPT_B_08_02", 1)
			};
		}
	}
}
