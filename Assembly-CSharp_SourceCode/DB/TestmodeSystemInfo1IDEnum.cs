namespace DB
{
	public static class TestmodeSystemInfo1IDEnum
	{
		private static readonly TestmodeSystemInfo1TableRecord[] records;

		public static bool IsActive(this TestmodeSystemInfo1ID self)
		{
			if (self >= TestmodeSystemInfo1ID.Title0 && self < TestmodeSystemInfo1ID.End)
			{
				return self != TestmodeSystemInfo1ID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeSystemInfo1ID self)
		{
			if (self >= TestmodeSystemInfo1ID.Title0)
			{
				return self < TestmodeSystemInfo1ID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeSystemInfo1ID self)
		{
			if (self < TestmodeSystemInfo1ID.Title0)
			{
				self = TestmodeSystemInfo1ID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeSystemInfo1ID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeSystemInfo1ID self)
		{
			return GetEnd();
		}

		public static TestmodeSystemInfo1ID FindID(string enumName)
		{
			for (TestmodeSystemInfo1ID testmodeSystemInfo1ID = TestmodeSystemInfo1ID.Title0; testmodeSystemInfo1ID < TestmodeSystemInfo1ID.End; testmodeSystemInfo1ID++)
			{
				if (testmodeSystemInfo1ID.GetEnumName() == enumName)
				{
					return testmodeSystemInfo1ID;
				}
			}
			return TestmodeSystemInfo1ID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeSystemInfo1ID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeSystemInfo1ID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeSystemInfo1ID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeSystemInfo1ID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static TestmodeSystemInfo1IDEnum()
		{
			records = new TestmodeSystemInfo1TableRecord[10]
			{
				new TestmodeSystemInfo1TableRecord(0, "Title0", "游戏系统信息", ""),
				new TestmodeSystemInfo1TableRecord(1, "Label00", "软件版本", ""),
				new TestmodeSystemInfo1TableRecord(2, "Label01", "关键芯片", ""),
				new TestmodeSystemInfo1TableRecord(3, "Label02", "主账号", ""),
				new TestmodeSystemInfo1TableRecord(4, "Label03", "LED控制板(1P)", ""),
				new TestmodeSystemInfo1TableRecord(5, "Label04", "基板号", ""),
				new TestmodeSystemInfo1TableRecord(6, "Label05", "固件", ""),
				new TestmodeSystemInfo1TableRecord(7, "Label06", "LED控制板(2P)", ""),
				new TestmodeSystemInfo1TableRecord(8, "Label07", "基板号", ""),
				new TestmodeSystemInfo1TableRecord(9, "Label08", "固件", "")
			};
		}
	}
}
