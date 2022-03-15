namespace DB
{
	public static class TestmodeSystemtestIDEnum
	{
		private static readonly TestmodeSystemtestTableRecord[] records;

		public static bool IsActive(this TestmodeSystemtestID self)
		{
			if (self >= TestmodeSystemtestID.Title0 && self < TestmodeSystemtestID.End)
			{
				return self != TestmodeSystemtestID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeSystemtestID self)
		{
			if (self >= TestmodeSystemtestID.Title0)
			{
				return self < TestmodeSystemtestID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeSystemtestID self)
		{
			if (self < TestmodeSystemtestID.Title0)
			{
				self = TestmodeSystemtestID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeSystemtestID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeSystemtestID self)
		{
			return GetEnd();
		}

		public static TestmodeSystemtestID FindID(string enumName)
		{
			for (TestmodeSystemtestID testmodeSystemtestID = TestmodeSystemtestID.Title0; testmodeSystemtestID < TestmodeSystemtestID.End; testmodeSystemtestID++)
			{
				if (testmodeSystemtestID.GetEnumName() == enumName)
				{
					return testmodeSystemtestID;
				}
			}
			return TestmodeSystemtestID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeSystemtestID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeSystemtestID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeSystemtestID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeSystemtestID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static TestmodeSystemtestIDEnum()
		{
			records = new TestmodeSystemtestTableRecord[3]
			{
				new TestmodeSystemtestTableRecord(0, "Title0", "要切换到系统测试模式吗？", string.Empty),
				new TestmodeSystemtestTableRecord(1, "Label00", "切换", string.Empty),
				new TestmodeSystemtestTableRecord(2, "Label01", "不切换", string.Empty)
			};
		}
	}
}
