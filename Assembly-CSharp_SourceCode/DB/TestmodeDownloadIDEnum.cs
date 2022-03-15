namespace DB
{
	public static class TestmodeDownloadIDEnum
	{
		private static readonly TestmodeDownloadTableRecord[] records;

		public static bool IsActive(this TestmodeDownloadID self)
		{
			if (self >= TestmodeDownloadID.Title0 && self < TestmodeDownloadID.End)
			{
				return self != TestmodeDownloadID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeDownloadID self)
		{
			if (self >= TestmodeDownloadID.Title0)
			{
				return self < TestmodeDownloadID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeDownloadID self)
		{
			if (self < TestmodeDownloadID.Title0)
			{
				self = TestmodeDownloadID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeDownloadID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeDownloadID self)
		{
			return GetEnd();
		}

		public static TestmodeDownloadID FindID(string enumName)
		{
			for (TestmodeDownloadID testmodeDownloadID = TestmodeDownloadID.Title0; testmodeDownloadID < TestmodeDownloadID.End; testmodeDownloadID++)
			{
				if (testmodeDownloadID.GetEnumName() == enumName)
				{
					return testmodeDownloadID;
				}
			}
			return TestmodeDownloadID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeDownloadID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeDownloadID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeDownloadID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeDownloadID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static TestmodeDownloadIDEnum()
		{
			records = new TestmodeDownloadTableRecord[13]
			{
				new TestmodeDownloadTableRecord(0, "Title0", "Net下载情况", ""),
				new TestmodeDownloadTableRecord(1, "Label00", "Net认证状态", ""),
				new TestmodeDownloadTableRecord(2, "Label01", "当前的软件版本", ""),
				new TestmodeDownloadTableRecord(3, "Label02", "软件程序下载情况", ""),
				new TestmodeDownloadTableRecord(4, "Label03", "下载中的软件版本", ""),
				new TestmodeDownloadTableRecord(5, "Label04", "下载进度", ""),
				new TestmodeDownloadTableRecord(6, "Label05", "下载开始日期", ""),
				new TestmodeDownloadTableRecord(7, "Label06", "发布日期", ""),
				new TestmodeDownloadTableRecord(8, "Label07", "数据下载情况", ""),
				new TestmodeDownloadTableRecord(9, "Label08", "下载进度", ""),
				new TestmodeDownloadTableRecord(10, "Label09", "下载开始日期", ""),
				new TestmodeDownloadTableRecord(11, "Label10", "发布日期", ""),
				new TestmodeDownloadTableRecord(12, "Label11", "服务器时间", "")
			};
		}
	}
}
