namespace DB
{
	public static class TestmodeTouchpanel2pIDEnum
	{
		private static readonly TestmodeTouchpanel2pTableRecord[] records;

		public static bool IsActive(this TestmodeTouchpanel2pID self)
		{
			if (self >= TestmodeTouchpanel2pID.Title0 && self < TestmodeTouchpanel2pID.End)
			{
				return self != TestmodeTouchpanel2pID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeTouchpanel2pID self)
		{
			if (self >= TestmodeTouchpanel2pID.Title0)
			{
				return self < TestmodeTouchpanel2pID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeTouchpanel2pID self)
		{
			if (self < TestmodeTouchpanel2pID.Title0)
			{
				self = TestmodeTouchpanel2pID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeTouchpanel2pID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeTouchpanel2pID self)
		{
			return GetEnd();
		}

		public static TestmodeTouchpanel2pID FindID(string enumName)
		{
			for (TestmodeTouchpanel2pID testmodeTouchpanel2pID = TestmodeTouchpanel2pID.Title0; testmodeTouchpanel2pID < TestmodeTouchpanel2pID.End; testmodeTouchpanel2pID++)
			{
				if (testmodeTouchpanel2pID.GetEnumName() == enumName)
				{
					return testmodeTouchpanel2pID;
				}
			}
			return TestmodeTouchpanel2pID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeTouchpanel2pID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeTouchpanel2pID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeTouchpanel2pID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeTouchpanel2pID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static TestmodeTouchpanel2pIDEnum()
		{
			records = new TestmodeTouchpanel2pTableRecord[12]
			{
				new TestmodeTouchpanel2pTableRecord(0, "Title0", "2P\u3000触摸感应器", ""),
				new TestmodeTouchpanel2pTableRecord(1, "Label00", "当前灵敏度", ""),
				new TestmodeTouchpanel2pTableRecord(2, "Label01", "状态", ""),
				new TestmodeTouchpanel2pTableRecord(3, "Label02", "灵敏度＋", ""),
				new TestmodeTouchpanel2pTableRecord(4, "Label03", "灵敏度－", ""),
				new TestmodeTouchpanel2pTableRecord(5, "Label04", "设定变更", ""),
				new TestmodeTouchpanel2pTableRecord(6, "Label05", "离开", ""),
				new TestmodeTouchpanel2pTableRecord(7, "Status0", "正在初始化", ""),
				new TestmodeTouchpanel2pTableRecord(8, "Status1", "运行中", ""),
				new TestmodeTouchpanel2pTableRecord(9, "Status2", "设定中", ""),
				new TestmodeTouchpanel2pTableRecord(10, "Error0", "无法连接（超时）", ""),
				new TestmodeTouchpanel2pTableRecord(11, "Error1", "初始化失败", "")
			};
		}
	}
}
