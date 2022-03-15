namespace DB
{
	public static class CharlistAbcSmallIDEnum
	{
		private static readonly CharlistAbcSmallTableRecord[] records = new CharlistAbcSmallTableRecord[28]
		{
			new CharlistAbcSmallTableRecord(0, "SmallChar_a", "ａ"),
			new CharlistAbcSmallTableRecord(1, "SmallChar_b", "ｂ"),
			new CharlistAbcSmallTableRecord(2, "SmallChar_c", "ｃ"),
			new CharlistAbcSmallTableRecord(3, "SmallChar_d", "ｄ"),
			new CharlistAbcSmallTableRecord(4, "SmallChar_e", "ｅ"),
			new CharlistAbcSmallTableRecord(5, "SmallChar_f", "ｆ"),
			new CharlistAbcSmallTableRecord(6, "SmallChar_g", "ｇ"),
			new CharlistAbcSmallTableRecord(7, "SmallChar_h", "ｈ"),
			new CharlistAbcSmallTableRecord(8, "SmallChar_i", "ｉ"),
			new CharlistAbcSmallTableRecord(9, "SmallChar_j", "ｊ"),
			new CharlistAbcSmallTableRecord(10, "SmallChar_k", "ｋ"),
			new CharlistAbcSmallTableRecord(11, "SmallChar_l", "ｌ"),
			new CharlistAbcSmallTableRecord(12, "SmallChar_n", "ｍ"),
			new CharlistAbcSmallTableRecord(13, "SmallChar_m", "ｎ"),
			new CharlistAbcSmallTableRecord(14, "SmallChar_o", "ｏ"),
			new CharlistAbcSmallTableRecord(15, "SmallChar_p", "ｐ"),
			new CharlistAbcSmallTableRecord(16, "SmallChar_q", "ｑ"),
			new CharlistAbcSmallTableRecord(17, "SmallChar_r", "ｒ"),
			new CharlistAbcSmallTableRecord(18, "SmallChar_s", "ｓ"),
			new CharlistAbcSmallTableRecord(19, "SmallChar_t", "ｔ"),
			new CharlistAbcSmallTableRecord(20, "SmallChar_u", "ｕ"),
			new CharlistAbcSmallTableRecord(21, "SmallChar_v", "ｖ"),
			new CharlistAbcSmallTableRecord(22, "SmallChar_w", "ｗ"),
			new CharlistAbcSmallTableRecord(23, "SmallChar_x", "ｘ"),
			new CharlistAbcSmallTableRecord(24, "SmallChar_y", "ｙ"),
			new CharlistAbcSmallTableRecord(25, "SmallChar_z", "ｚ"),
			new CharlistAbcSmallTableRecord(26, "SmallChar_Space", "␣"),
			new CharlistAbcSmallTableRecord(27, "SmallChar_End", "终")
		};

		public static bool IsActive(this CharlistAbcSmallID self)
		{
			if (self >= CharlistAbcSmallID.SmallChar_a && self < CharlistAbcSmallID.End)
			{
				return self != CharlistAbcSmallID.SmallChar_a;
			}
			return false;
		}

		public static bool IsValid(this CharlistAbcSmallID self)
		{
			if (self >= CharlistAbcSmallID.SmallChar_a)
			{
				return self < CharlistAbcSmallID.End;
			}
			return false;
		}

		public static void Clamp(this CharlistAbcSmallID self)
		{
			if (self < CharlistAbcSmallID.SmallChar_a)
			{
				self = CharlistAbcSmallID.SmallChar_a;
			}
			else if ((int)self >= GetEnd())
			{
				self = (CharlistAbcSmallID)GetEnd();
			}
		}

		public static int GetEnd(this CharlistAbcSmallID self)
		{
			return GetEnd();
		}

		public static CharlistAbcSmallID FindID(string enumName)
		{
			for (CharlistAbcSmallID charlistAbcSmallID = CharlistAbcSmallID.SmallChar_a; charlistAbcSmallID < CharlistAbcSmallID.End; charlistAbcSmallID++)
			{
				if (charlistAbcSmallID.GetEnumName() == enumName)
				{
					return charlistAbcSmallID;
				}
			}
			return CharlistAbcSmallID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this CharlistAbcSmallID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this CharlistAbcSmallID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this CharlistAbcSmallID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
