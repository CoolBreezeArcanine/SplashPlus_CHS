namespace DB
{
	public static class TestmodeDebugEventsetIDEnum
	{
		private static readonly TestmodeDebugEventsetTableRecord[] records = new TestmodeDebugEventsetTableRecord[23]
		{
			new TestmodeDebugEventsetTableRecord(0, "Title0", "デバッグ：イベント設定"),
			new TestmodeDebugEventsetTableRecord(1, "Title1", "イベント一覧"),
			new TestmodeDebugEventsetTableRecord(2, "Label00", ""),
			new TestmodeDebugEventsetTableRecord(3, "Label01", ""),
			new TestmodeDebugEventsetTableRecord(4, "Label02", ""),
			new TestmodeDebugEventsetTableRecord(5, "Label03", ""),
			new TestmodeDebugEventsetTableRecord(6, "Label04", ""),
			new TestmodeDebugEventsetTableRecord(7, "Label05", ""),
			new TestmodeDebugEventsetTableRecord(8, "Label06", ""),
			new TestmodeDebugEventsetTableRecord(9, "Label07", ""),
			new TestmodeDebugEventsetTableRecord(10, "Label08", ""),
			new TestmodeDebugEventsetTableRecord(11, "Label09", ""),
			new TestmodeDebugEventsetTableRecord(12, "Label10", ""),
			new TestmodeDebugEventsetTableRecord(13, "Label11", ""),
			new TestmodeDebugEventsetTableRecord(14, "Label12", ""),
			new TestmodeDebugEventsetTableRecord(15, "Label13", ""),
			new TestmodeDebugEventsetTableRecord(16, "Label14", ""),
			new TestmodeDebugEventsetTableRecord(17, "Label15", ""),
			new TestmodeDebugEventsetTableRecord(18, "Label16", ""),
			new TestmodeDebugEventsetTableRecord(19, "Label17", ""),
			new TestmodeDebugEventsetTableRecord(20, "Label18", ""),
			new TestmodeDebugEventsetTableRecord(21, "Label19", ""),
			new TestmodeDebugEventsetTableRecord(22, "Label20", "終了")
		};

		public static bool IsActive(this TestmodeDebugEventsetID self)
		{
			if (self >= TestmodeDebugEventsetID.Title0 && self < TestmodeDebugEventsetID.End)
			{
				return self != TestmodeDebugEventsetID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeDebugEventsetID self)
		{
			if (self >= TestmodeDebugEventsetID.Title0)
			{
				return self < TestmodeDebugEventsetID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeDebugEventsetID self)
		{
			if (self < TestmodeDebugEventsetID.Title0)
			{
				self = TestmodeDebugEventsetID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeDebugEventsetID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeDebugEventsetID self)
		{
			return GetEnd();
		}

		public static TestmodeDebugEventsetID FindID(string enumName)
		{
			for (TestmodeDebugEventsetID testmodeDebugEventsetID = TestmodeDebugEventsetID.Title0; testmodeDebugEventsetID < TestmodeDebugEventsetID.End; testmodeDebugEventsetID++)
			{
				if (testmodeDebugEventsetID.GetEnumName() == enumName)
				{
					return testmodeDebugEventsetID;
				}
			}
			return TestmodeDebugEventsetID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeDebugEventsetID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeDebugEventsetID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeDebugEventsetID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
