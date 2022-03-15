namespace DB
{
	public static class TestmodeRootIDEnum
	{
		private static readonly TestmodeRootTableRecord[] records;

		public static bool IsActive(this TestmodeRootID self)
		{
			if (self >= TestmodeRootID.Title0 && self < TestmodeRootID.End)
			{
				return self != TestmodeRootID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeRootID self)
		{
			if (self >= TestmodeRootID.Title0)
			{
				return self < TestmodeRootID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeRootID self)
		{
			if (self < TestmodeRootID.Title0)
			{
				self = TestmodeRootID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeRootID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeRootID self)
		{
			return GetEnd();
		}

		public static TestmodeRootID FindID(string enumName)
		{
			for (TestmodeRootID testmodeRootID = TestmodeRootID.Title0; testmodeRootID < TestmodeRootID.End; testmodeRootID++)
			{
				if (testmodeRootID.GetEnumName() == enumName)
				{
					return testmodeRootID;
				}
			}
			return TestmodeRootID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeRootID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeRootID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeRootID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeRootID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static TestmodeRootIDEnum()
		{
			records = new TestmodeRootTableRecord[26]
			{
				new TestmodeRootTableRecord(0, "Title0", "游戏测试模式", ""),
				new TestmodeRootTableRecord(1, "Label00", "簿记", ""),
				new TestmodeRootTableRecord(2, "Label01", "输入测试", ""),
				new TestmodeRootTableRecord(3, "Label02", "输出测试", ""),
				new TestmodeRootTableRecord(4, "Label03", "显示器测试", ""),
				new TestmodeRootTableRecord(5, "Label04", "触摸感应器测试", ""),
				new TestmodeRootTableRecord(6, "Label05", "摄像机测试", ""),
				new TestmodeRootTableRecord(7, "Label06", "游戏设定", ""),
				new TestmodeRootTableRecord(8, "Label07", "游戏系统消息", ""),
				new TestmodeRootTableRecord(9, "Label08", "Aime读卡器测试", ""),
				new TestmodeRootTableRecord(10, "Label09", "关店设定", ""),
				new TestmodeRootTableRecord(11, "Label10", "网络测试", ""),
				new TestmodeRootTableRecord(12, "Label11", "电子支付信息", ""),
				new TestmodeRootTableRecord(13, "Label12", "VFD显示测试", ""),
				new TestmodeRootTableRecord(14, "Label13", "ALL.Net充值记录", ""),
				new TestmodeRootTableRecord(15, "Label14", "ALL.Net下载情况", ""),
				new TestmodeRootTableRecord(16, "Label15", "备份数据清空", ""),
				new TestmodeRootTableRecord(17, "Label16", "系统测试", ""),
				new TestmodeRootTableRecord(18, "Label17", "\u3000服务器设定", ""),
				new TestmodeRootTableRecord(19, "Label18", "Debug: Sound Test", ""),
				new TestmodeRootTableRecord(20, "Label19", "Debug: Ini Edit", ""),
				new TestmodeRootTableRecord(21, "Label20", "Debug: Event Settings", ""),
				new TestmodeRootTableRecord(22, "Label21", "Debug: LED Dimming", ""),
				new TestmodeRootTableRecord(23, "Label22", "离开", ""),
				new TestmodeRootTableRecord(24, "Deliver00", "服务器", ""),
				new TestmodeRootTableRecord(25, "Deliver01", "客户端", "")
			};
		}
	}
}
