namespace DB
{
	public static class TestmodeAccountingIDEnum
	{
		private static readonly TestmodeAccountingTableRecord[] records;

		public static bool IsActive(this TestmodeAccountingID self)
		{
			if (self >= TestmodeAccountingID.Title0 && self < TestmodeAccountingID.End)
			{
				return self != TestmodeAccountingID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeAccountingID self)
		{
			if (self >= TestmodeAccountingID.Title0)
			{
				return self < TestmodeAccountingID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeAccountingID self)
		{
			if (self < TestmodeAccountingID.Title0)
			{
				self = TestmodeAccountingID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeAccountingID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeAccountingID self)
		{
			return GetEnd();
		}

		public static TestmodeAccountingID FindID(string enumName)
		{
			for (TestmodeAccountingID testmodeAccountingID = TestmodeAccountingID.Title0; testmodeAccountingID < TestmodeAccountingID.End; testmodeAccountingID++)
			{
				if (testmodeAccountingID.GetEnumName() == enumName)
				{
					return testmodeAccountingID;
				}
			}
			return TestmodeAccountingID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeAccountingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeAccountingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeAccountingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeAccountingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static TestmodeAccountingIDEnum()
		{
			records = new TestmodeAccountingTableRecord[13]
			{
				new TestmodeAccountingTableRecord(0, "Title0", "Net????????????", string.Empty),
				new TestmodeAccountingTableRecord(1, "Label00", "????????????", string.Empty),
				new TestmodeAccountingTableRecord(2, "Label01", "????????????", string.Empty),
				new TestmodeAccountingTableRecord(3, "Label02", "??????????????????", string.Empty),
				new TestmodeAccountingTableRecord(4, "Label03", "----/--", string.Empty),
				new TestmodeAccountingTableRecord(5, "Label04", "----/--", string.Empty),
				new TestmodeAccountingTableRecord(6, "Label05", "----/--", string.Empty),
				new TestmodeAccountingTableRecord(7, "Label06", "????????????", string.Empty),
				new TestmodeAccountingTableRecord(8, "Label07", "???????????????", string.Empty),
				new TestmodeAccountingTableRecord(9, "Label08", "???????????????", string.Empty),
				new TestmodeAccountingTableRecord(10, "Label09", string.Empty, string.Empty),
				new TestmodeAccountingTableRecord(11, "Label10", "Net??????????????????", string.Empty),
				new TestmodeAccountingTableRecord(12, "Label11", "??????", string.Empty)
			};
		}
	}
}
