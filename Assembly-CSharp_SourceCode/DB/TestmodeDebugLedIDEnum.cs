namespace DB
{
	public static class TestmodeDebugLedIDEnum
	{
		private static readonly TestmodeDebugLedTableRecord[] records = new TestmodeDebugLedTableRecord[13]
		{
			new TestmodeDebugLedTableRecord(0, "Title0", "デバッグ：LED調光"),
			new TestmodeDebugLedTableRecord(1, "Label00", "帽子のLED色"),
			new TestmodeDebugLedTableRecord(2, "Label01", "\u3000R"),
			new TestmodeDebugLedTableRecord(3, "Label02", "\u3000G"),
			new TestmodeDebugLedTableRecord(4, "Label03", "\u3000B"),
			new TestmodeDebugLedTableRecord(5, "Label04", "ボタンのLED色"),
			new TestmodeDebugLedTableRecord(6, "Label05", "\u3000R"),
			new TestmodeDebugLedTableRecord(7, "Label06", "\u3000G"),
			new TestmodeDebugLedTableRecord(8, "Label07", "\u3000B"),
			new TestmodeDebugLedTableRecord(9, "Label08", "ボディの明るさ"),
			new TestmodeDebugLedTableRecord(10, "Label09", "\u3000明るさ"),
			new TestmodeDebugLedTableRecord(11, "Label10", "加算される値"),
			new TestmodeDebugLedTableRecord(12, "Label11", "\u3000終了")
		};

		public static bool IsActive(this TestmodeDebugLedID self)
		{
			if (self >= TestmodeDebugLedID.Title0 && self < TestmodeDebugLedID.End)
			{
				return self != TestmodeDebugLedID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeDebugLedID self)
		{
			if (self >= TestmodeDebugLedID.Title0)
			{
				return self < TestmodeDebugLedID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeDebugLedID self)
		{
			if (self < TestmodeDebugLedID.Title0)
			{
				self = TestmodeDebugLedID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeDebugLedID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeDebugLedID self)
		{
			return GetEnd();
		}

		public static TestmodeDebugLedID FindID(string enumName)
		{
			for (TestmodeDebugLedID testmodeDebugLedID = TestmodeDebugLedID.Title0; testmodeDebugLedID < TestmodeDebugLedID.End; testmodeDebugLedID++)
			{
				if (testmodeDebugLedID.GetEnumName() == enumName)
				{
					return testmodeDebugLedID;
				}
			}
			return TestmodeDebugLedID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeDebugLedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeDebugLedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeDebugLedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
