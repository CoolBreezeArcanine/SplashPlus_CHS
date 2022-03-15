namespace DB
{
	public static class OptionHeadphonevolumeIDEnum
	{
		private static readonly OptionHeadphonevolumeTableRecord[] records = new OptionHeadphonevolumeTableRecord[20]
		{
			new OptionHeadphonevolumeTableRecord(0, "Vol1", 0.05f, "1", "", "", "", 1),
			new OptionHeadphonevolumeTableRecord(1, "Vol2", 0.1f, "2", "", "", "", 0),
			new OptionHeadphonevolumeTableRecord(2, "Vol3", 0.15f, "3", "", "", "", 0),
			new OptionHeadphonevolumeTableRecord(3, "Vol4", 0.2f, "4", "", "", "", 0),
			new OptionHeadphonevolumeTableRecord(4, "Vol5", 0.25f, "5", "", "", "", 0),
			new OptionHeadphonevolumeTableRecord(5, "Vol6", 0.3f, "6", "", "", "", 0),
			new OptionHeadphonevolumeTableRecord(6, "Vol7", 0.35f, "7", "", "", "", 0),
			new OptionHeadphonevolumeTableRecord(7, "Vol8", 0.4f, "8", "", "", "", 0),
			new OptionHeadphonevolumeTableRecord(8, "Vol9", 0.45f, "9", "", "", "", 0),
			new OptionHeadphonevolumeTableRecord(9, "Vol10", 0.5f, "10", "", "", "", 0),
			new OptionHeadphonevolumeTableRecord(10, "Vol11", 0.55f, "11", "", "", "", 0),
			new OptionHeadphonevolumeTableRecord(11, "Vol12", 0.6f, "12", "", "", "", 0),
			new OptionHeadphonevolumeTableRecord(12, "Vol13", 0.65f, "13", "", "", "", 0),
			new OptionHeadphonevolumeTableRecord(13, "Vol14", 0.7f, "14", "", "", "", 0),
			new OptionHeadphonevolumeTableRecord(14, "Vol15", 0.75f, "15", "", "", "", 0),
			new OptionHeadphonevolumeTableRecord(15, "Vol16", 0.8f, "16", "", "", "", 0),
			new OptionHeadphonevolumeTableRecord(16, "Vol17", 0.85f, "17", "", "", "", 0),
			new OptionHeadphonevolumeTableRecord(17, "Vol18", 0.9f, "18", "", "", "", 0),
			new OptionHeadphonevolumeTableRecord(18, "Vol19", 0.95f, "19", "", "", "", 0),
			new OptionHeadphonevolumeTableRecord(19, "Vol20", 1f, "20", "", "", "", 0)
		};

		public static bool IsActive(this OptionHeadphonevolumeID self)
		{
			if (self >= OptionHeadphonevolumeID.Vol1 && self < OptionHeadphonevolumeID.End)
			{
				return self != OptionHeadphonevolumeID.Vol1;
			}
			return false;
		}

		public static bool IsValid(this OptionHeadphonevolumeID self)
		{
			if (self >= OptionHeadphonevolumeID.Vol1)
			{
				return self < OptionHeadphonevolumeID.End;
			}
			return false;
		}

		public static void Clamp(this OptionHeadphonevolumeID self)
		{
			if (self < OptionHeadphonevolumeID.Vol1)
			{
				self = OptionHeadphonevolumeID.Vol1;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionHeadphonevolumeID)GetEnd();
			}
		}

		public static int GetEnd(this OptionHeadphonevolumeID self)
		{
			return GetEnd();
		}

		public static OptionHeadphonevolumeID FindID(string enumName)
		{
			for (OptionHeadphonevolumeID optionHeadphonevolumeID = OptionHeadphonevolumeID.Vol1; optionHeadphonevolumeID < OptionHeadphonevolumeID.End; optionHeadphonevolumeID++)
			{
				if (optionHeadphonevolumeID.GetEnumName() == enumName)
				{
					return optionHeadphonevolumeID;
				}
			}
			return OptionHeadphonevolumeID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionHeadphonevolumeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionHeadphonevolumeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static float GetValue(this OptionHeadphonevolumeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Value;
			}
			return 0f;
		}

		public static string GetName(this OptionHeadphonevolumeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionHeadphonevolumeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionHeadphonevolumeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionHeadphonevolumeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static bool IsDefault(this OptionHeadphonevolumeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}
	}
}
