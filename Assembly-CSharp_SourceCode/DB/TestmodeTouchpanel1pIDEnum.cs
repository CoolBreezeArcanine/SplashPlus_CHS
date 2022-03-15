namespace DB
{
	public static class TestmodeTouchpanel1pIDEnum
	{
		private static readonly TestmodeTouchpanel1pTableRecord[] records;

		public static bool IsActive(this TestmodeTouchpanel1pID self)
		{
			if (self >= TestmodeTouchpanel1pID.Title0 && self < TestmodeTouchpanel1pID.End)
			{
				return self != TestmodeTouchpanel1pID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeTouchpanel1pID self)
		{
			if (self >= TestmodeTouchpanel1pID.Title0)
			{
				return self < TestmodeTouchpanel1pID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeTouchpanel1pID self)
		{
			if (self < TestmodeTouchpanel1pID.Title0)
			{
				self = TestmodeTouchpanel1pID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeTouchpanel1pID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeTouchpanel1pID self)
		{
			return GetEnd();
		}

		public static TestmodeTouchpanel1pID FindID(string enumName)
		{
			for (TestmodeTouchpanel1pID testmodeTouchpanel1pID = TestmodeTouchpanel1pID.Title0; testmodeTouchpanel1pID < TestmodeTouchpanel1pID.End; testmodeTouchpanel1pID++)
			{
				if (testmodeTouchpanel1pID.GetEnumName() == enumName)
				{
					return testmodeTouchpanel1pID;
				}
			}
			return TestmodeTouchpanel1pID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeTouchpanel1pID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeTouchpanel1pID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeTouchpanel1pID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeTouchpanel1pID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static TestmodeTouchpanel1pIDEnum()
		{
			records = new TestmodeTouchpanel1pTableRecord[12]
			{
				new TestmodeTouchpanel1pTableRecord(0, "Title0", "1P\u3000触摸感应器", ""),
				new TestmodeTouchpanel1pTableRecord(1, "Label00", "当前灵敏度", ""),
				new TestmodeTouchpanel1pTableRecord(2, "Label01", "状态", ""),
				new TestmodeTouchpanel1pTableRecord(3, "Label02", "灵敏度＋", ""),
				new TestmodeTouchpanel1pTableRecord(4, "Label03", "灵敏度－", ""),
				new TestmodeTouchpanel1pTableRecord(5, "Label04", "设定变更", ""),
				new TestmodeTouchpanel1pTableRecord(6, "Label05", "离开", ""),
				new TestmodeTouchpanel1pTableRecord(7, "Status0", "正在初始化", ""),
				new TestmodeTouchpanel1pTableRecord(8, "Status1", "运行中", ""),
				new TestmodeTouchpanel1pTableRecord(9, "Status2", "设定中", ""),
				new TestmodeTouchpanel1pTableRecord(10, "Error0", "无法连接（超时）", ""),
				new TestmodeTouchpanel1pTableRecord(11, "Error1", "初始化失败", "")
			};
		}
	}
}
