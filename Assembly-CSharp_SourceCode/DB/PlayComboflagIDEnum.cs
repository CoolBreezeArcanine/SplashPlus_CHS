namespace DB
{
	public static class PlayComboflagIDEnum
	{
		private static readonly PlayComboflagTableRecord[] records = new PlayComboflagTableRecord[5]
		{
			new PlayComboflagTableRecord(0, "None", "なし"),
			new PlayComboflagTableRecord(1, "Silver", "銀フルコンボ"),
			new PlayComboflagTableRecord(2, "Gold", "金フルコンボ"),
			new PlayComboflagTableRecord(3, "AllPerfect", "AllPerfect"),
			new PlayComboflagTableRecord(4, "AllPerfectPlus", "AllPerfect+")
		};

		public static bool IsActive(this PlayComboflagID self)
		{
			if (self >= PlayComboflagID.None && self < PlayComboflagID.End)
			{
				return self != PlayComboflagID.None;
			}
			return false;
		}

		public static bool IsValid(this PlayComboflagID self)
		{
			if (self >= PlayComboflagID.None)
			{
				return self < PlayComboflagID.End;
			}
			return false;
		}

		public static void Clamp(this PlayComboflagID self)
		{
			if (self < PlayComboflagID.None)
			{
				self = PlayComboflagID.None;
			}
			else if ((int)self >= GetEnd())
			{
				self = (PlayComboflagID)GetEnd();
			}
		}

		public static int GetEnd(this PlayComboflagID self)
		{
			return GetEnd();
		}

		public static PlayComboflagID FindID(string enumName)
		{
			for (PlayComboflagID playComboflagID = PlayComboflagID.None; playComboflagID < PlayComboflagID.End; playComboflagID++)
			{
				if (playComboflagID.GetEnumName() == enumName)
				{
					return playComboflagID;
				}
			}
			return PlayComboflagID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this PlayComboflagID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this PlayComboflagID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this PlayComboflagID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
