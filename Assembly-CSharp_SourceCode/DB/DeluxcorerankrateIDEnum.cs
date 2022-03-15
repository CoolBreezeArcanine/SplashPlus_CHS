namespace DB
{
	public static class DeluxcorerankrateIDEnum
	{
		private static readonly DeluxcorerankrateTableRecord[] records = new DeluxcorerankrateTableRecord[6]
		{
			new DeluxcorerankrateTableRecord(0, "Rank_00", "Rank_00", 0),
			new DeluxcorerankrateTableRecord(1, "Rank_01", "Rank_01", 85),
			new DeluxcorerankrateTableRecord(2, "Rank_02", "Rank_02", 90),
			new DeluxcorerankrateTableRecord(3, "Rank_03", "Rank_03", 93),
			new DeluxcorerankrateTableRecord(4, "Rank_04", "Rank_04", 95),
			new DeluxcorerankrateTableRecord(5, "Rank_05", "Rank_05", 97)
		};

		public static bool IsActive(this DeluxcorerankrateID self)
		{
			if (self >= DeluxcorerankrateID.Rank_00 && self < DeluxcorerankrateID.End)
			{
				return self != DeluxcorerankrateID.Rank_00;
			}
			return false;
		}

		public static bool IsValid(this DeluxcorerankrateID self)
		{
			if (self >= DeluxcorerankrateID.Rank_00)
			{
				return self < DeluxcorerankrateID.End;
			}
			return false;
		}

		public static void Clamp(this DeluxcorerankrateID self)
		{
			if (self < DeluxcorerankrateID.Rank_00)
			{
				self = DeluxcorerankrateID.Rank_00;
			}
			else if ((int)self >= GetEnd())
			{
				self = (DeluxcorerankrateID)GetEnd();
			}
		}

		public static int GetEnd(this DeluxcorerankrateID self)
		{
			return GetEnd();
		}

		public static DeluxcorerankrateID FindID(string enumName)
		{
			for (DeluxcorerankrateID deluxcorerankrateID = DeluxcorerankrateID.Rank_00; deluxcorerankrateID < DeluxcorerankrateID.End; deluxcorerankrateID++)
			{
				if (deluxcorerankrateID.GetEnumName() == enumName)
				{
					return deluxcorerankrateID;
				}
			}
			return DeluxcorerankrateID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this DeluxcorerankrateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this DeluxcorerankrateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this DeluxcorerankrateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static int GetAchieve(this DeluxcorerankrateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Achieve;
			}
			return 0;
		}
	}
}
