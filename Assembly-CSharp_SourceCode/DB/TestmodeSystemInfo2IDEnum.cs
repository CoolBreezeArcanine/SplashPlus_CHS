namespace DB
{
	public static class TestmodeSystemInfo2IDEnum
	{
		private static readonly TestmodeSystemInfo2TableRecord[] records;

		public static bool IsActive(this TestmodeSystemInfo2ID self)
		{
			if (self >= TestmodeSystemInfo2ID.Title0 && self < TestmodeSystemInfo2ID.End)
			{
				return self != TestmodeSystemInfo2ID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeSystemInfo2ID self)
		{
			if (self >= TestmodeSystemInfo2ID.Title0)
			{
				return self < TestmodeSystemInfo2ID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeSystemInfo2ID self)
		{
			if (self < TestmodeSystemInfo2ID.Title0)
			{
				self = TestmodeSystemInfo2ID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeSystemInfo2ID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeSystemInfo2ID self)
		{
			return GetEnd();
		}

		public static TestmodeSystemInfo2ID FindID(string enumName)
		{
			for (TestmodeSystemInfo2ID testmodeSystemInfo2ID = TestmodeSystemInfo2ID.Title0; testmodeSystemInfo2ID < TestmodeSystemInfo2ID.End; testmodeSystemInfo2ID++)
			{
				if (testmodeSystemInfo2ID.GetEnumName() == enumName)
				{
					return testmodeSystemInfo2ID;
				}
			}
			return TestmodeSystemInfo2ID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeSystemInfo2ID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeSystemInfo2ID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeSystemInfo2ID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeSystemInfo2ID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static TestmodeSystemInfo2IDEnum()
		{
			records = new TestmodeSystemInfo2TableRecord[19]
			{
				new TestmodeSystemInfo2TableRecord(0, "Title0", "游戏系统信息", string.Empty),
				new TestmodeSystemInfo2TableRecord(1, "Title1", "追加数据一览", string.Empty),
				new TestmodeSystemInfo2TableRecord(2, "Label00", "数据名", string.Empty),
				new TestmodeSystemInfo2TableRecord(3, "Label01", "", ""),
				new TestmodeSystemInfo2TableRecord(4, "Label02", "", ""),
				new TestmodeSystemInfo2TableRecord(5, "Label03", "", ""),
				new TestmodeSystemInfo2TableRecord(6, "Label04", "", ""),
				new TestmodeSystemInfo2TableRecord(7, "Label05", "", ""),
				new TestmodeSystemInfo2TableRecord(8, "Label06", "", ""),
				new TestmodeSystemInfo2TableRecord(9, "Label07", "", ""),
				new TestmodeSystemInfo2TableRecord(10, "Label08", "", ""),
				new TestmodeSystemInfo2TableRecord(11, "Label09", "", ""),
				new TestmodeSystemInfo2TableRecord(12, "Label10", "", ""),
				new TestmodeSystemInfo2TableRecord(13, "Label11", "", ""),
				new TestmodeSystemInfo2TableRecord(14, "Label12", "", ""),
				new TestmodeSystemInfo2TableRecord(15, "Label13", "", ""),
				new TestmodeSystemInfo2TableRecord(16, "Label14", "", ""),
				new TestmodeSystemInfo2TableRecord(17, "Label15", "", ""),
				new TestmodeSystemInfo2TableRecord(18, "Label00_1", "日期", "")
			};
		}
	}
}
