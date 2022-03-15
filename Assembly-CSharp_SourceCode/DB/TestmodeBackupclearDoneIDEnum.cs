namespace DB
{
	public static class TestmodeBackupclearDoneIDEnum
	{
		private static readonly TestmodeBackupclearDoneTableRecord[] records;

		public static bool IsActive(this TestmodeBackupclearDoneID self)
		{
			if (self >= TestmodeBackupclearDoneID.Title0 && self < TestmodeBackupclearDoneID.End)
			{
				return self != TestmodeBackupclearDoneID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeBackupclearDoneID self)
		{
			if (self >= TestmodeBackupclearDoneID.Title0)
			{
				return self < TestmodeBackupclearDoneID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeBackupclearDoneID self)
		{
			if (self < TestmodeBackupclearDoneID.Title0)
			{
				self = TestmodeBackupclearDoneID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeBackupclearDoneID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeBackupclearDoneID self)
		{
			return GetEnd();
		}

		public static TestmodeBackupclearDoneID FindID(string enumName)
		{
			for (TestmodeBackupclearDoneID testmodeBackupclearDoneID = TestmodeBackupclearDoneID.Title0; testmodeBackupclearDoneID < TestmodeBackupclearDoneID.End; testmodeBackupclearDoneID++)
			{
				if (testmodeBackupclearDoneID.GetEnumName() == enumName)
				{
					return testmodeBackupclearDoneID;
				}
			}
			return TestmodeBackupclearDoneID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeBackupclearDoneID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeBackupclearDoneID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeBackupclearDoneID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeBackupclearDoneID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static TestmodeBackupclearDoneIDEnum()
		{
			records = new TestmodeBackupclearDoneTableRecord[1]
			{
				new TestmodeBackupclearDoneTableRecord(0, "Title0", "备份数据已清空", "")
			};
		}
	}
}
