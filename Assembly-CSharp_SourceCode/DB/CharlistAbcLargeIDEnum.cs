namespace DB
{
	public static class CharlistAbcLargeIDEnum
	{
		private static readonly CharlistAbcLargeTableRecord[] records = new CharlistAbcLargeTableRecord[28]
		{
			new CharlistAbcLargeTableRecord(0, "LargeChar_A", "Ａ"),
			new CharlistAbcLargeTableRecord(1, "LargeChar_B", "Ｂ"),
			new CharlistAbcLargeTableRecord(2, "LargeChar_C", "Ｃ"),
			new CharlistAbcLargeTableRecord(3, "LargeChar_D", "Ｄ"),
			new CharlistAbcLargeTableRecord(4, "LargeChar_E", "Ｅ"),
			new CharlistAbcLargeTableRecord(5, "LargeChar_F", "Ｆ"),
			new CharlistAbcLargeTableRecord(6, "LargeChar_G", "Ｇ"),
			new CharlistAbcLargeTableRecord(7, "LargeChar_H", "Ｈ"),
			new CharlistAbcLargeTableRecord(8, "LargeChar_I", "Ｉ"),
			new CharlistAbcLargeTableRecord(9, "LargeChar_J", "Ｊ"),
			new CharlistAbcLargeTableRecord(10, "LargeChar_K", "Ｋ"),
			new CharlistAbcLargeTableRecord(11, "LargeChar_L", "Ｌ"),
			new CharlistAbcLargeTableRecord(12, "LargeChar_M", "Ｍ"),
			new CharlistAbcLargeTableRecord(13, "LargeChar_N", "Ｎ"),
			new CharlistAbcLargeTableRecord(14, "LargeChar_O", "Ｏ"),
			new CharlistAbcLargeTableRecord(15, "LargeChar_P", "Ｐ"),
			new CharlistAbcLargeTableRecord(16, "LargeChar_Q", "Ｑ"),
			new CharlistAbcLargeTableRecord(17, "LargeChar_R", "Ｒ"),
			new CharlistAbcLargeTableRecord(18, "LargeChar_S", "Ｓ"),
			new CharlistAbcLargeTableRecord(19, "LargeChar_T", "Ｔ"),
			new CharlistAbcLargeTableRecord(20, "LargeChar_U", "Ｕ"),
			new CharlistAbcLargeTableRecord(21, "LargeChar_V", "Ｖ"),
			new CharlistAbcLargeTableRecord(22, "LargeChar_W", "Ｗ"),
			new CharlistAbcLargeTableRecord(23, "LargeChar_X", "Ｘ"),
			new CharlistAbcLargeTableRecord(24, "LargeChar_Y", "Ｙ"),
			new CharlistAbcLargeTableRecord(25, "LargeChar_Z", "Ｚ"),
			new CharlistAbcLargeTableRecord(26, "LargeChar_Space", "␣"),
			new CharlistAbcLargeTableRecord(27, "LargeChar_End", "终")
		};

		public static bool IsActive(this CharlistAbcLargeID self)
		{
			if (self >= CharlistAbcLargeID.LargeChar_A && self < CharlistAbcLargeID.End)
			{
				return self != CharlistAbcLargeID.LargeChar_A;
			}
			return false;
		}

		public static bool IsValid(this CharlistAbcLargeID self)
		{
			if (self >= CharlistAbcLargeID.LargeChar_A)
			{
				return self < CharlistAbcLargeID.End;
			}
			return false;
		}

		public static void Clamp(this CharlistAbcLargeID self)
		{
			if (self < CharlistAbcLargeID.LargeChar_A)
			{
				self = CharlistAbcLargeID.LargeChar_A;
			}
			else if ((int)self >= GetEnd())
			{
				self = (CharlistAbcLargeID)GetEnd();
			}
		}

		public static int GetEnd(this CharlistAbcLargeID self)
		{
			return GetEnd();
		}

		public static CharlistAbcLargeID FindID(string enumName)
		{
			for (CharlistAbcLargeID charlistAbcLargeID = CharlistAbcLargeID.LargeChar_A; charlistAbcLargeID < CharlistAbcLargeID.End; charlistAbcLargeID++)
			{
				if (charlistAbcLargeID.GetEnumName() == enumName)
				{
					return charlistAbcLargeID;
				}
			}
			return CharlistAbcLargeID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this CharlistAbcLargeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this CharlistAbcLargeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this CharlistAbcLargeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
