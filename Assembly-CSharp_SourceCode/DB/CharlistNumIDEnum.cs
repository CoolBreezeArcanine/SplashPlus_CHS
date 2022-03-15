namespace DB
{
	public static class CharlistNumIDEnum
	{
		private static readonly CharlistNumTableRecord[] records = new CharlistNumTableRecord[12]
		{
			new CharlistNumTableRecord(0, "NumChar_0", "０"),
			new CharlistNumTableRecord(1, "NumChar_1", "１"),
			new CharlistNumTableRecord(2, "NumChar_2", "２"),
			new CharlistNumTableRecord(3, "NumChar_3", "３"),
			new CharlistNumTableRecord(4, "NumChar_4", "４"),
			new CharlistNumTableRecord(5, "NumChar_5", "５"),
			new CharlistNumTableRecord(6, "NumChar_6", "６"),
			new CharlistNumTableRecord(7, "NumChar_7", "７"),
			new CharlistNumTableRecord(8, "NumChar_8", "８"),
			new CharlistNumTableRecord(9, "NumChar_9", "９"),
			new CharlistNumTableRecord(10, "NumChar_Space", "␣"),
			new CharlistNumTableRecord(11, "NumChar_End", "终")
		};

		public static bool IsActive(this CharlistNumID self)
		{
			if (self >= CharlistNumID.NumChar_0 && self < CharlistNumID.End)
			{
				return self != CharlistNumID.NumChar_0;
			}
			return false;
		}

		public static bool IsValid(this CharlistNumID self)
		{
			if (self >= CharlistNumID.NumChar_0)
			{
				return self < CharlistNumID.End;
			}
			return false;
		}

		public static void Clamp(this CharlistNumID self)
		{
			if (self < CharlistNumID.NumChar_0)
			{
				self = CharlistNumID.NumChar_0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (CharlistNumID)GetEnd();
			}
		}

		public static int GetEnd(this CharlistNumID self)
		{
			return GetEnd();
		}

		public static CharlistNumID FindID(string enumName)
		{
			for (CharlistNumID charlistNumID = CharlistNumID.NumChar_0; charlistNumID < CharlistNumID.End; charlistNumID++)
			{
				if (charlistNumID.GetEnumName() == enumName)
				{
					return charlistNumID;
				}
			}
			return CharlistNumID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this CharlistNumID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this CharlistNumID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this CharlistNumID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
