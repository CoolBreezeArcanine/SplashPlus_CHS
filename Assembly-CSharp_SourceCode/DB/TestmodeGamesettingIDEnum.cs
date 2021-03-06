namespace DB
{
	public static class TestmodeGamesettingIDEnum
	{
		private static readonly TestmodeGamesettingTableRecord[] records;

		public static bool IsActive(this TestmodeGamesettingID self)
		{
			if (self >= TestmodeGamesettingID.Title0 && self < TestmodeGamesettingID.End)
			{
				return self != TestmodeGamesettingID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeGamesettingID self)
		{
			if (self >= TestmodeGamesettingID.Title0)
			{
				return self < TestmodeGamesettingID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeGamesettingID self)
		{
			if (self < TestmodeGamesettingID.Title0)
			{
				self = TestmodeGamesettingID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeGamesettingID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeGamesettingID self)
		{
			return GetEnd();
		}

		public static TestmodeGamesettingID FindID(string enumName)
		{
			for (TestmodeGamesettingID testmodeGamesettingID = TestmodeGamesettingID.Title0; testmodeGamesettingID < TestmodeGamesettingID.End; testmodeGamesettingID++)
			{
				if (testmodeGamesettingID.GetEnumName() == enumName)
				{
					return testmodeGamesettingID;
				}
			}
			return TestmodeGamesettingID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeGamesettingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeGamesettingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeGamesettingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeGamesettingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static TestmodeGamesettingIDEnum()
		{
			records = new TestmodeGamesettingTableRecord[10]
			{
				new TestmodeGamesettingTableRecord(0, "Title0", "????????????", ""),
				new TestmodeGamesettingTableRecord(1, "Label00", "??????????????????", ""),
				new TestmodeGamesettingTableRecord(2, "Label01", "???????????????????????????", ""),
				new TestmodeGamesettingTableRecord(3, "Label02", "????????????", ""),
				new TestmodeGamesettingTableRecord(4, "Label03", "?????????????????????", ""),
				new TestmodeGamesettingTableRecord(5, "Label04", "????????????", ""),
				new TestmodeGamesettingTableRecord(6, "Label05", "??????????????????", ""),
				new TestmodeGamesettingTableRecord(7, "Label06", "??????", ""),
				new TestmodeGamesettingTableRecord(8, "Label00_00", "???????????????", ""),
				new TestmodeGamesettingTableRecord(9, "Label00_01", "?????????????????????", "")
			};
		}
	}
}
