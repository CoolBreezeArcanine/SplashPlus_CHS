namespace DB
{
	public static class TestmodeCloseConfirmIDEnum
	{
		private static readonly TestmodeCloseConfirmTableRecord[] records;

		public static bool IsActive(this TestmodeCloseConfirmID self)
		{
			if (self >= TestmodeCloseConfirmID.Title0 && self < TestmodeCloseConfirmID.End)
			{
				return self != TestmodeCloseConfirmID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeCloseConfirmID self)
		{
			if (self >= TestmodeCloseConfirmID.Title0)
			{
				return self < TestmodeCloseConfirmID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeCloseConfirmID self)
		{
			if (self < TestmodeCloseConfirmID.Title0)
			{
				self = TestmodeCloseConfirmID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeCloseConfirmID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeCloseConfirmID self)
		{
			return GetEnd();
		}

		public static TestmodeCloseConfirmID FindID(string enumName)
		{
			for (TestmodeCloseConfirmID testmodeCloseConfirmID = TestmodeCloseConfirmID.Title0; testmodeCloseConfirmID < TestmodeCloseConfirmID.End; testmodeCloseConfirmID++)
			{
				if (testmodeCloseConfirmID.GetEnumName() == enumName)
				{
					return testmodeCloseConfirmID;
				}
			}
			return TestmodeCloseConfirmID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeCloseConfirmID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeCloseConfirmID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeCloseConfirmID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeCloseConfirmID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static TestmodeCloseConfirmIDEnum()
		{
			records = new TestmodeCloseConfirmTableRecord[3]
			{
				new TestmodeCloseConfirmTableRecord(0, "Title0", "恢复到出厂设置，可以吗？", ""),
				new TestmodeCloseConfirmTableRecord(1, "Label00", "是", ""),
				new TestmodeCloseConfirmTableRecord(2, "Label01", "否", "")
			};
		}
	}
}
