namespace DB
{
	public static class TestmodeInputIDEnum
	{
		private static readonly TestmodeInputTableRecord[] records;

		public static bool IsActive(this TestmodeInputID self)
		{
			if (self >= TestmodeInputID.Title0 && self < TestmodeInputID.End)
			{
				return self != TestmodeInputID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeInputID self)
		{
			if (self >= TestmodeInputID.Title0)
			{
				return self < TestmodeInputID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeInputID self)
		{
			if (self < TestmodeInputID.Title0)
			{
				self = TestmodeInputID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeInputID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeInputID self)
		{
			return GetEnd();
		}

		public static TestmodeInputID FindID(string enumName)
		{
			for (TestmodeInputID testmodeInputID = TestmodeInputID.Title0; testmodeInputID < TestmodeInputID.End; testmodeInputID++)
			{
				if (testmodeInputID.GetEnumName() == enumName)
				{
					return testmodeInputID;
				}
			}
			return TestmodeInputID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeInputID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeInputID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeInputID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeInputID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static TestmodeInputIDEnum()
		{
			records = new TestmodeInputTableRecord[21]
			{
				new TestmodeInputTableRecord(0, "Title0", "输入测试", ""),
				new TestmodeInputTableRecord(1, "Label00", "键1(1P)", ""),
				new TestmodeInputTableRecord(2, "Label01", "键2(1P)", ""),
				new TestmodeInputTableRecord(3, "Label02", "键3(1P)", ""),
				new TestmodeInputTableRecord(4, "Label03", "键4(1P)", ""),
				new TestmodeInputTableRecord(5, "Label04", "键5(1P)", ""),
				new TestmodeInputTableRecord(6, "Label05", "键6(1P)", ""),
				new TestmodeInputTableRecord(7, "Label06", "键7(1P)", ""),
				new TestmodeInputTableRecord(8, "Label07", "键8(1P)", ""),
				new TestmodeInputTableRecord(9, "Label08", "选择键(1P)", ""),
				new TestmodeInputTableRecord(10, "Label09", "键1(2P)", ""),
				new TestmodeInputTableRecord(11, "Label10", "键2(2P)", ""),
				new TestmodeInputTableRecord(12, "Label11", "键3(2P)", ""),
				new TestmodeInputTableRecord(13, "Label12", "键4(2P)", ""),
				new TestmodeInputTableRecord(14, "Label13", "键5(2P)", ""),
				new TestmodeInputTableRecord(15, "Label14", "键6(2P)", ""),
				new TestmodeInputTableRecord(16, "Label15", "键7(2P)", ""),
				new TestmodeInputTableRecord(17, "Label16", "键8(2P)", ""),
				new TestmodeInputTableRecord(18, "Label17", "选择键(2P)", ""),
				new TestmodeInputTableRecord(19, "Label18", "服务键", ""),
				new TestmodeInputTableRecord(20, "Label19", "测试键", "")
			};
		}
	}
}
