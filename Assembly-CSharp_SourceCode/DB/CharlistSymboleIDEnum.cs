namespace DB
{
	public static class CharlistSymboleIDEnum
	{
		private static readonly CharlistSymboleTableRecord[] records = new CharlistSymboleTableRecord[46]
		{
			new CharlistSymboleTableRecord(0, "SymboleChar_TYUUTEN", "・"),
			new CharlistSymboleTableRecord(1, "SymboleChar_COLON", "："),
			new CharlistSymboleTableRecord(2, "SymboleChar_SEMICOLON", "；"),
			new CharlistSymboleTableRecord(3, "SymboleChar_QUESTION", "？"),
			new CharlistSymboleTableRecord(4, "SymboleChar_EXCLAMATION", "！"),
			new CharlistSymboleTableRecord(5, "SymboleChar_TILDE", "～"),
			new CharlistSymboleTableRecord(6, "SymboleChar_SLASH", "／"),
			new CharlistSymboleTableRecord(7, "SymboleChar_PLUS", "＋"),
			new CharlistSymboleTableRecord(8, "SymboleChar_MINUS", "－"),
			new CharlistSymboleTableRecord(9, "SymboleChar_KAKERU", "×"),
			new CharlistSymboleTableRecord(10, "SymboleChar_WARU", "÷"),
			new CharlistSymboleTableRecord(11, "SymboleChar_EQUAL", "＝"),
			new CharlistSymboleTableRecord(12, "SymboleChar_OSU", "♂"),
			new CharlistSymboleTableRecord(13, "SymboleChar_MESU", "♀"),
			new CharlistSymboleTableRecord(14, "SymboleChar_SUBETE", "∀"),
			new CharlistSymboleTableRecord(15, "SymboleChar_SHARP", "＃"),
			new CharlistSymboleTableRecord(16, "SymboleChar_AMPERSAND", "＆"),
			new CharlistSymboleTableRecord(17, "SymboleChar_ASTERISK", "＊"),
			new CharlistSymboleTableRecord(18, "SymboleChar_AT", "＠"),
			new CharlistSymboleTableRecord(19, "SymboleChar_STAR", "☆"),
			new CharlistSymboleTableRecord(20, "SymboleChar_MARU", "○"),
			new CharlistSymboleTableRecord(21, "SymboleChar_2MARU", "◎"),
			new CharlistSymboleTableRecord(22, "SymboleChar_KUKEI", "◇"),
			new CharlistSymboleTableRecord(23, "SymboleChar_SQUARE", "□"),
			new CharlistSymboleTableRecord(24, "SymboleChar_TRIANGLE", "△"),
			new CharlistSymboleTableRecord(25, "SymboleChar_TRIANGLE2", "▽"),
			new CharlistSymboleTableRecord(26, "SymboleChar_ONNPU", "♪"),
			new CharlistSymboleTableRecord(27, "SymboleChar_DAGGER", "†"),
			new CharlistSymboleTableRecord(28, "SymboleChar_D_DAGGER", "‡"),
			new CharlistSymboleTableRecord(29, "SymboleChar_SIGMA", "Σ"),
			new CharlistSymboleTableRecord(30, "SymboleChar_ALPHA", "α"),
			new CharlistSymboleTableRecord(31, "SymboleChar_BETA", "β"),
			new CharlistSymboleTableRecord(32, "SymboleChar_GAMMA", "γ"),
			new CharlistSymboleTableRecord(33, "SymboleChar_THETA", "θ"),
			new CharlistSymboleTableRecord(34, "SymboleChar_PHI", "φ"),
			new CharlistSymboleTableRecord(35, "SymboleChar_PSI", "ψ"),
			new CharlistSymboleTableRecord(36, "SymboleChar_OMEGA", "ω"),
			new CharlistSymboleTableRecord(37, "SymboleChar_DE", "Д"),
			new CharlistSymboleTableRecord(38, "SymboleChar_YO", "ё"),
			new CharlistSymboleTableRecord(39, "SymboleChar_DOLLAR", "＄"),
			new CharlistSymboleTableRecord(40, "SymboleChar_LEFT_PARENTHESIS", "（"),
			new CharlistSymboleTableRecord(41, "SymboleChar_RIGHT_PARENTHESIS", "）"),
			new CharlistSymboleTableRecord(42, "SymboleChar_PERIOD", "．"),
			new CharlistSymboleTableRecord(43, "SymboleChar_LOW_LINE", "\uff3f"),
			new CharlistSymboleTableRecord(44, "SymboleChar_Space", "␣"),
			new CharlistSymboleTableRecord(45, "SumboleChar_End", "终")
		};

		public static bool IsActive(this CharlistSymboleID self)
		{
			if (self >= CharlistSymboleID.SymboleChar_TYUUTEN && self < CharlistSymboleID.End)
			{
				return self != CharlistSymboleID.SymboleChar_TYUUTEN;
			}
			return false;
		}

		public static bool IsValid(this CharlistSymboleID self)
		{
			if (self >= CharlistSymboleID.SymboleChar_TYUUTEN)
			{
				return self < CharlistSymboleID.End;
			}
			return false;
		}

		public static void Clamp(this CharlistSymboleID self)
		{
			if (self < CharlistSymboleID.SymboleChar_TYUUTEN)
			{
				self = CharlistSymboleID.SymboleChar_TYUUTEN;
			}
			else if ((int)self >= GetEnd())
			{
				self = (CharlistSymboleID)GetEnd();
			}
		}

		public static int GetEnd(this CharlistSymboleID self)
		{
			return GetEnd();
		}

		public static CharlistSymboleID FindID(string enumName)
		{
			for (CharlistSymboleID charlistSymboleID = CharlistSymboleID.SymboleChar_TYUUTEN; charlistSymboleID < CharlistSymboleID.End; charlistSymboleID++)
			{
				if (charlistSymboleID.GetEnumName() == enumName)
				{
					return charlistSymboleID;
				}
			}
			return CharlistSymboleID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this CharlistSymboleID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this CharlistSymboleID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this CharlistSymboleID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
