namespace DB
{
	public static class TestmodeTouchpanelIDEnum
	{
		private static readonly TestmodeTouchpanelTableRecord[] records;

		public static bool IsActive(this TestmodeTouchpanelID self)
		{
			if (self >= TestmodeTouchpanelID.Title0 && self < TestmodeTouchpanelID.End)
			{
				return self != TestmodeTouchpanelID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeTouchpanelID self)
		{
			if (self >= TestmodeTouchpanelID.Title0)
			{
				return self < TestmodeTouchpanelID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeTouchpanelID self)
		{
			if (self < TestmodeTouchpanelID.Title0)
			{
				self = TestmodeTouchpanelID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeTouchpanelID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeTouchpanelID self)
		{
			return GetEnd();
		}

		public static TestmodeTouchpanelID FindID(string enumName)
		{
			for (TestmodeTouchpanelID testmodeTouchpanelID = TestmodeTouchpanelID.Title0; testmodeTouchpanelID < TestmodeTouchpanelID.End; testmodeTouchpanelID++)
			{
				if (testmodeTouchpanelID.GetEnumName() == enumName)
				{
					return testmodeTouchpanelID;
				}
			}
			return TestmodeTouchpanelID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeTouchpanelID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeTouchpanelID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeTouchpanelID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeTouchpanelID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static TestmodeTouchpanelIDEnum()
		{
			records = new TestmodeTouchpanelTableRecord[4]
			{
				new TestmodeTouchpanelTableRecord(0, "Title0", "触摸感应器", ""),
				new TestmodeTouchpanelTableRecord(1, "Label00", "触摸感应器（玩家1）", ""),
				new TestmodeTouchpanelTableRecord(2, "Label01", "触摸感应器（玩家2）", ""),
				new TestmodeTouchpanelTableRecord(3, "Label02", "离开", "")
			};
		}
	}
}
