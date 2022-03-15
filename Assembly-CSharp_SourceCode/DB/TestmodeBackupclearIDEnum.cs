namespace DB
{
	public static class TestmodeBackupclearIDEnum
	{
		private static readonly TestmodeBackupclearTableRecord[] records;

		public static bool IsActive(this TestmodeBackupclearID self)
		{
			if (self >= TestmodeBackupclearID.Title0 && self < TestmodeBackupclearID.End)
			{
				return self != TestmodeBackupclearID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeBackupclearID self)
		{
			if (self >= TestmodeBackupclearID.Title0)
			{
				return self < TestmodeBackupclearID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeBackupclearID self)
		{
			if (self < TestmodeBackupclearID.Title0)
			{
				self = TestmodeBackupclearID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeBackupclearID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeBackupclearID self)
		{
			return GetEnd();
		}

		public static TestmodeBackupclearID FindID(string enumName)
		{
			for (TestmodeBackupclearID testmodeBackupclearID = TestmodeBackupclearID.Title0; testmodeBackupclearID < TestmodeBackupclearID.End; testmodeBackupclearID++)
			{
				if (testmodeBackupclearID.GetEnumName() == enumName)
				{
					return testmodeBackupclearID;
				}
			}
			return TestmodeBackupclearID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeBackupclearID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeBackupclearID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeBackupclearID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeBackupclearID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static TestmodeBackupclearIDEnum()
		{
			records = new TestmodeBackupclearTableRecord[6]
			{
				new TestmodeBackupclearTableRecord(0, "Title0", "要清空备份数据吗？", ""),
				new TestmodeBackupclearTableRecord(1, "Title1", "", ""),
				new TestmodeBackupclearTableRecord(2, "Title2", "选择“清空”的话", ""),
				new TestmodeBackupclearTableRecord(3, "Title3", "会删除簿记和可用点数数据", ""),
				new TestmodeBackupclearTableRecord(4, "Label00", "是（清除）", ""),
				new TestmodeBackupclearTableRecord(5, "Label01", "否（取消）", "")
			};
		}
	}
}
