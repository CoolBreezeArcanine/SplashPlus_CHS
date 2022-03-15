namespace DB
{
	public static class RatingTableIDEnum
	{
		private static readonly RatingTableTableRecord[] records = new RatingTableTableRecord[23]
		{
			new RatingTableTableRecord(0, "Rate_00", "D", 0, 0),
			new RatingTableTableRecord(1, "Rate_01", "D", 100000, 16),
			new RatingTableTableRecord(2, "Rate_02", "D", 200000, 32),
			new RatingTableTableRecord(3, "Rate_03", "D", 300000, 48),
			new RatingTableTableRecord(4, "Rate_04", "D", 400000, 64),
			new RatingTableTableRecord(5, "Rate_05", "C", 500000, 80),
			new RatingTableTableRecord(6, "Rate_06", "B", 600000, 96),
			new RatingTableTableRecord(7, "Rate_07", "BB", 700000, 112),
			new RatingTableTableRecord(8, "Rate_08", "BBB", 750000, 120),
			new RatingTableTableRecord(9, "Rate_09", "BBB", 799999, 128),
			new RatingTableTableRecord(10, "Rate_10", "A", 800000, 136),
			new RatingTableTableRecord(11, "Rate_11", "AA", 900000, 152),
			new RatingTableTableRecord(12, "Rate_12", "AAA", 940000, 168),
			new RatingTableTableRecord(13, "Rate_13", "AAA", 969999, 176),
			new RatingTableTableRecord(14, "Rate_14", "S", 970000, 200),
			new RatingTableTableRecord(15, "Rate_15", "S+", 980000, 203),
			new RatingTableTableRecord(16, "Rate_16", "S+", 989999, 206),
			new RatingTableTableRecord(17, "Rate_17", "SS", 990000, 208),
			new RatingTableTableRecord(18, "Rate_18", "SS+", 995000, 211),
			new RatingTableTableRecord(19, "Rate_19", "SS+", 999999, 214),
			new RatingTableTableRecord(20, "Rate_20", "SSS", 1000000, 216),
			new RatingTableTableRecord(21, "Rate_21", "SSS", 1004999, 222),
			new RatingTableTableRecord(22, "Rate_22", "SSS+", 1005000, 224)
		};

		public static bool IsActive(this RatingTableID self)
		{
			if (self >= RatingTableID.Rate_00 && self < RatingTableID.End)
			{
				return self != RatingTableID.Rate_00;
			}
			return false;
		}

		public static bool IsValid(this RatingTableID self)
		{
			if (self >= RatingTableID.Rate_00)
			{
				return self < RatingTableID.End;
			}
			return false;
		}

		public static void Clamp(this RatingTableID self)
		{
			if (self < RatingTableID.Rate_00)
			{
				self = RatingTableID.Rate_00;
			}
			else if ((int)self >= GetEnd())
			{
				self = (RatingTableID)GetEnd();
			}
		}

		public static int GetEnd(this RatingTableID self)
		{
			return GetEnd();
		}

		public static RatingTableID FindID(string enumName)
		{
			for (RatingTableID ratingTableID = RatingTableID.Rate_00; ratingTableID < RatingTableID.End; ratingTableID++)
			{
				if (ratingTableID.GetEnumName() == enumName)
				{
					return ratingTableID;
				}
			}
			return RatingTableID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this RatingTableID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this RatingTableID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this RatingTableID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static int GetAchive(this RatingTableID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Achive;
			}
			return 0;
		}

		public static int GetOffset(this RatingTableID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Offset;
			}
			return 0;
		}
	}
}
