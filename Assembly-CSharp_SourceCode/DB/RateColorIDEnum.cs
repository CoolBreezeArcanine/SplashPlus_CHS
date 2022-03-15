namespace DB
{
	public static class RateColorIDEnum
	{
		private static readonly RateColorTableRecord[] records = new RateColorTableRecord[11]
		{
			new RateColorTableRecord(0, "White1", "白", 0),
			new RateColorTableRecord(1, "Blue1", "青", 1000),
			new RateColorTableRecord(2, "Green1", "緑", 2000),
			new RateColorTableRecord(3, "Yellow1", "黄色", 4000),
			new RateColorTableRecord(4, "Red1", "赤", 7000),
			new RateColorTableRecord(5, "Purple1", "紫", 10000),
			new RateColorTableRecord(6, "Copper1", "銅", 12000),
			new RateColorTableRecord(7, "Silver1", "銀", 13000),
			new RateColorTableRecord(8, "Gold1", "金", 14000),
			new RateColorTableRecord(9, "Platinum1", "白金", 14500),
			new RateColorTableRecord(10, "Rainbow1", "虹", 15000)
		};

		public static bool IsActive(this RateColorID self)
		{
			if (self >= RateColorID.White1 && self < RateColorID.End)
			{
				return self != RateColorID.White1;
			}
			return false;
		}

		public static bool IsValid(this RateColorID self)
		{
			if (self >= RateColorID.White1)
			{
				return self < RateColorID.End;
			}
			return false;
		}

		public static void Clamp(this RateColorID self)
		{
			if (self < RateColorID.White1)
			{
				self = RateColorID.White1;
			}
			else if ((int)self >= GetEnd())
			{
				self = (RateColorID)GetEnd();
			}
		}

		public static int GetEnd(this RateColorID self)
		{
			return GetEnd();
		}

		public static RateColorID FindID(string enumName)
		{
			for (RateColorID rateColorID = RateColorID.White1; rateColorID < RateColorID.End; rateColorID++)
			{
				if (rateColorID.GetEnumName() == enumName)
				{
					return rateColorID;
				}
			}
			return RateColorID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this RateColorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this RateColorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this RateColorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static int GetRate(this RateColorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Rate;
			}
			return 0;
		}
	}
}
