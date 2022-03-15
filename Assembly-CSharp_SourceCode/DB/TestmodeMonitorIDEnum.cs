namespace DB
{
	public static class TestmodeMonitorIDEnum
	{
		private static readonly TestmodeMonitorTableRecord[] records;

		public static bool IsActive(this TestmodeMonitorID self)
		{
			if (self >= TestmodeMonitorID.Title0 && self < TestmodeMonitorID.End)
			{
				return self != TestmodeMonitorID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeMonitorID self)
		{
			if (self >= TestmodeMonitorID.Title0)
			{
				return self < TestmodeMonitorID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeMonitorID self)
		{
			if (self < TestmodeMonitorID.Title0)
			{
				self = TestmodeMonitorID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeMonitorID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeMonitorID self)
		{
			return GetEnd();
		}

		public static TestmodeMonitorID FindID(string enumName)
		{
			for (TestmodeMonitorID testmodeMonitorID = TestmodeMonitorID.Title0; testmodeMonitorID < TestmodeMonitorID.End; testmodeMonitorID++)
			{
				if (testmodeMonitorID.GetEnumName() == enumName)
				{
					return testmodeMonitorID;
				}
			}
			return TestmodeMonitorID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeMonitorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeMonitorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeMonitorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeMonitorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static TestmodeMonitorIDEnum()
		{
			records = new TestmodeMonitorTableRecord[2]
			{
				new TestmodeMonitorTableRecord(0, "Title0", "显示器测试", ""),
				new TestmodeMonitorTableRecord(1, "Label00", "", "")
			};
		}
	}
}
