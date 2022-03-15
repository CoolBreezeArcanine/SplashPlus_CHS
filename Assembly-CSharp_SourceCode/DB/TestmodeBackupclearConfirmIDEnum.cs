namespace DB
{
	public static class TestmodeBackupclearConfirmIDEnum
	{
		private static readonly TestmodeBackupclearConfirmTableRecord[] records;

		public static bool IsActive(this TestmodeBackupclearConfirmID self)
		{
			if (self >= TestmodeBackupclearConfirmID.Title0 && self < TestmodeBackupclearConfirmID.End)
			{
				return self != TestmodeBackupclearConfirmID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeBackupclearConfirmID self)
		{
			if (self >= TestmodeBackupclearConfirmID.Title0)
			{
				return self < TestmodeBackupclearConfirmID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeBackupclearConfirmID self)
		{
			if (self < TestmodeBackupclearConfirmID.Title0)
			{
				self = TestmodeBackupclearConfirmID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeBackupclearConfirmID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeBackupclearConfirmID self)
		{
			return GetEnd();
		}

		public static TestmodeBackupclearConfirmID FindID(string enumName)
		{
			for (TestmodeBackupclearConfirmID testmodeBackupclearConfirmID = TestmodeBackupclearConfirmID.Title0; testmodeBackupclearConfirmID < TestmodeBackupclearConfirmID.End; testmodeBackupclearConfirmID++)
			{
				if (testmodeBackupclearConfirmID.GetEnumName() == enumName)
				{
					return testmodeBackupclearConfirmID;
				}
			}
			return TestmodeBackupclearConfirmID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeBackupclearConfirmID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeBackupclearConfirmID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeBackupclearConfirmID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeBackupclearConfirmID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static TestmodeBackupclearConfirmIDEnum()
		{
			records = new TestmodeBackupclearConfirmTableRecord[5]
			{
				new TestmodeBackupclearConfirmTableRecord(0, "Title0", "确定要清空备份数据吗？", ""),
				new TestmodeBackupclearConfirmTableRecord(1, "Title1", "", ""),
				new TestmodeBackupclearConfirmTableRecord(2, "Title2", "【最终确认】", ""),
				new TestmodeBackupclearConfirmTableRecord(3, "Label00", "是（清除）", ""),
				new TestmodeBackupclearConfirmTableRecord(4, "Label01", "否（取消）", "")
			};
		}
	}
}
