namespace DB
{
	public static class NgwordJpIDEnum
	{
		private static readonly NgwordJpTableRecord[] records = new NgwordJpTableRecord[5]
		{
			new NgwordJpTableRecord(0, "NgWord_001", "ｄｏｑ"),
			new NgwordJpTableRecord(1, "NgWord_002", "ｄｑｎ"),
			new NgwordJpTableRecord(2, "NgWord_003", "ｆｕｃｋ"),
			new NgwordJpTableRecord(3, "NgWord_004", "ｓｅｘ"),
			new NgwordJpTableRecord(4, "NgWord_005", "ｘｘｘ")
		};

		public static bool IsActive(this NgwordJpID self)
		{
			if (self >= NgwordJpID.NgWord_001 && self < NgwordJpID.End)
			{
				return self != NgwordJpID.NgWord_001;
			}
			return false;
		}

		public static bool IsValid(this NgwordJpID self)
		{
			if (self >= NgwordJpID.NgWord_001)
			{
				return self < NgwordJpID.End;
			}
			return false;
		}

		public static void Clamp(this NgwordJpID self)
		{
			if (self < NgwordJpID.NgWord_001)
			{
				self = NgwordJpID.NgWord_001;
			}
			else if ((int)self >= GetEnd())
			{
				self = (NgwordJpID)GetEnd();
			}
		}

		public static int GetEnd(this NgwordJpID self)
		{
			return GetEnd();
		}

		public static NgwordJpID FindID(string enumName)
		{
			for (NgwordJpID ngwordJpID = NgwordJpID.NgWord_001; ngwordJpID < NgwordJpID.End; ngwordJpID++)
			{
				if (ngwordJpID.GetEnumName() == enumName)
				{
					return ngwordJpID;
				}
			}
			return NgwordJpID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this NgwordJpID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this NgwordJpID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this NgwordJpID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
