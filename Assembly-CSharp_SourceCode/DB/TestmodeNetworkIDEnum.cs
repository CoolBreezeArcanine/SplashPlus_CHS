namespace DB
{
	public static class TestmodeNetworkIDEnum
	{
		private static readonly TestmodeNetworkTableRecord[] records;

		public static bool IsActive(this TestmodeNetworkID self)
		{
			if (self >= TestmodeNetworkID.Title0 && self < TestmodeNetworkID.End)
			{
				return self != TestmodeNetworkID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeNetworkID self)
		{
			if (self >= TestmodeNetworkID.Title0)
			{
				return self < TestmodeNetworkID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeNetworkID self)
		{
			if (self < TestmodeNetworkID.Title0)
			{
				self = TestmodeNetworkID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeNetworkID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeNetworkID self)
		{
			return GetEnd();
		}

		public static TestmodeNetworkID FindID(string enumName)
		{
			for (TestmodeNetworkID testmodeNetworkID = TestmodeNetworkID.Title0; testmodeNetworkID < TestmodeNetworkID.End; testmodeNetworkID++)
			{
				if (testmodeNetworkID.GetEnumName() == enumName)
				{
					return testmodeNetworkID;
				}
			}
			return TestmodeNetworkID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeNetworkID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeNetworkID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeNetworkID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeNetworkID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static TestmodeNetworkIDEnum()
		{
			records = new TestmodeNetworkTableRecord[10]
			{
				new TestmodeNetworkTableRecord(0, "Title0", "网络测试", ""),
				new TestmodeNetworkTableRecord(1, "Label00", "IP ADDRESS", ""),
				new TestmodeNetworkTableRecord(2, "Label01", "GATEWAY", ""),
				new TestmodeNetworkTableRecord(3, "Label02", "DNS(LAN)", ""),
				new TestmodeNetworkTableRecord(4, "Label03", "HOPS", ""),
				new TestmodeNetworkTableRecord(5, "Label04", "LINE TYPE", ""),
				new TestmodeNetworkTableRecord(6, "Label05", "Net认证", ""),
				new TestmodeNetworkTableRecord(7, "Label06", "Aime", ""),
				new TestmodeNetworkTableRecord(8, "Label07", "E-MONEY", ""),
				new TestmodeNetworkTableRecord(9, "Label08", "标题服务器", "")
			};
		}
	}
}
