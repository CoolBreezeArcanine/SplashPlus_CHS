namespace DB
{
	public static class TestmodeCloseIDEnum
	{
		private static readonly TestmodeCloseTableRecord[] records;

		public static bool IsActive(this TestmodeCloseID self)
		{
			if (self >= TestmodeCloseID.Title0 && self < TestmodeCloseID.End)
			{
				return self != TestmodeCloseID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeCloseID self)
		{
			if (self >= TestmodeCloseID.Title0)
			{
				return self < TestmodeCloseID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeCloseID self)
		{
			if (self < TestmodeCloseID.Title0)
			{
				self = TestmodeCloseID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeCloseID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeCloseID self)
		{
			return GetEnd();
		}

		public static TestmodeCloseID FindID(string enumName)
		{
			for (TestmodeCloseID testmodeCloseID = TestmodeCloseID.Title0; testmodeCloseID < TestmodeCloseID.End; testmodeCloseID++)
			{
				if (testmodeCloseID.GetEnumName() == enumName)
				{
					return testmodeCloseID;
				}
			}
			return TestmodeCloseID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeCloseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeCloseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeCloseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeCloseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static TestmodeCloseIDEnum()
		{
			records = new TestmodeCloseTableRecord[23]
			{
				new TestmodeCloseTableRecord(0, "Title0", "关店设定", string.Empty),
				new TestmodeCloseTableRecord(1, "Label00", "日程类型", string.Empty),
				new TestmodeCloseTableRecord(2, "Label01", "点", string.Empty),
				new TestmodeCloseTableRecord(3, "Label02", "分", string.Empty),
				new TestmodeCloseTableRecord(4, "Label03", "（星期天）点", string.Empty),
				new TestmodeCloseTableRecord(5, "Label04", "\u3000\u3000\u3000\u3000\u3000分", string.Empty),
				new TestmodeCloseTableRecord(6, "Label05", "（星期一）点", string.Empty),
				new TestmodeCloseTableRecord(7, "Label06", "\u3000\u3000\u3000\u3000\u3000分", string.Empty),
				new TestmodeCloseTableRecord(8, "Label07", "（星期二）点", string.Empty),
				new TestmodeCloseTableRecord(9, "Label08", "\u3000\u3000\u3000\u3000\u3000分", string.Empty),
				new TestmodeCloseTableRecord(10, "Label09", "（星期三）点", string.Empty),
				new TestmodeCloseTableRecord(11, "Label10", "\u3000\u3000\u3000\u3000\u3000分", string.Empty),
				new TestmodeCloseTableRecord(12, "Label11", "（星期四）点", string.Empty),
				new TestmodeCloseTableRecord(13, "Label12", "\u3000\u3000\u3000\u3000\u3000分", string.Empty),
				new TestmodeCloseTableRecord(14, "Label13", "（星期五）点", string.Empty),
				new TestmodeCloseTableRecord(15, "Label14", "\u3000\u3000\u3000\u3000\u3000分", string.Empty),
				new TestmodeCloseTableRecord(16, "Label15", "（星期六）点", string.Empty),
				new TestmodeCloseTableRecord(17, "Label16", "\u3000\u3000\u3000\u3000\u3000分", string.Empty),
				new TestmodeCloseTableRecord(18, "Label17", "恢复出厂设置", string.Empty),
				new TestmodeCloseTableRecord(19, "Label18", "离开", string.Empty),
				new TestmodeCloseTableRecord(20, "Label00_00", "天", string.Empty),
				new TestmodeCloseTableRecord(21, "Label00_01", "周", string.Empty),
				new TestmodeCloseTableRecord(22, "AllTime", "总时长", string.Empty)
			};
		}
	}
}
