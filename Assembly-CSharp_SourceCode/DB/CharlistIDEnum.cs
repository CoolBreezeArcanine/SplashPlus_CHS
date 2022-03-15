namespace DB
{
	public static class CharlistIDEnum
	{
		private static readonly CharlistTableRecord[] records = new CharlistTableRecord[4]
		{
			new CharlistTableRecord(0, "chara_large", "Ａ～Ｚ"),
			new CharlistTableRecord(1, "chara_small", "ａ～ｚ"),
			new CharlistTableRecord(2, "chara_num", "０～９"),
			new CharlistTableRecord(3, "chara_symbole", "記号")
		};

		public static bool IsActive(this CharlistID self)
		{
			if (self >= CharlistID.chara_large && self < CharlistID.End)
			{
				return self != CharlistID.chara_large;
			}
			return false;
		}

		public static bool IsValid(this CharlistID self)
		{
			if (self >= CharlistID.chara_large)
			{
				return self < CharlistID.End;
			}
			return false;
		}

		public static void Clamp(this CharlistID self)
		{
			if (self < CharlistID.chara_large)
			{
				self = CharlistID.chara_large;
			}
			else if ((int)self >= GetEnd())
			{
				self = (CharlistID)GetEnd();
			}
		}

		public static int GetEnd(this CharlistID self)
		{
			return GetEnd();
		}

		public static CharlistID FindID(string enumName)
		{
			for (CharlistID charlistID = CharlistID.chara_large; charlistID < CharlistID.End; charlistID++)
			{
				if (charlistID.GetEnumName() == enumName)
				{
					return charlistID;
				}
			}
			return CharlistID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this CharlistID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this CharlistID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this CharlistID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
