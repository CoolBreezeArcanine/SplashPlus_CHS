namespace DB
{
	public static class TestmodeBookkeep3IDEnum
	{
		private static readonly TestmodeBookkeep3TableRecord[] records;

		public static bool IsActive(this TestmodeBookkeep3ID self)
		{
			if (self >= TestmodeBookkeep3ID.Title0 && self < TestmodeBookkeep3ID.End)
			{
				return self != TestmodeBookkeep3ID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeBookkeep3ID self)
		{
			if (self >= TestmodeBookkeep3ID.Title0)
			{
				return self < TestmodeBookkeep3ID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeBookkeep3ID self)
		{
			if (self < TestmodeBookkeep3ID.Title0)
			{
				self = TestmodeBookkeep3ID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeBookkeep3ID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeBookkeep3ID self)
		{
			return GetEnd();
		}

		public static TestmodeBookkeep3ID FindID(string enumName)
		{
			for (TestmodeBookkeep3ID testmodeBookkeep3ID = TestmodeBookkeep3ID.Title0; testmodeBookkeep3ID < TestmodeBookkeep3ID.End; testmodeBookkeep3ID++)
			{
				if (testmodeBookkeep3ID.GetEnumName() == enumName)
				{
					return testmodeBookkeep3ID;
				}
			}
			return TestmodeBookkeep3ID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeBookkeep3ID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeBookkeep3ID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeBookkeep3ID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeBookkeep3ID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static TestmodeBookkeep3IDEnum()
		{
			records = new TestmodeBookkeep3TableRecord[6]
			{
				new TestmodeBookkeep3TableRecord(0, "Title0", "簿记(3/3)", ""),
				new TestmodeBookkeep3TableRecord(1, "Label00", "总时间", ""),
				new TestmodeBookkeep3TableRecord(2, "Label01", "总游戏时间", ""),
				new TestmodeBookkeep3TableRecord(3, "Label02", "平均游戏时间", ""),
				new TestmodeBookkeep3TableRecord(4, "TimeStr", "{0,2}时间{1,2}分{2,2}秒", ""),
				new TestmodeBookkeep3TableRecord(5, "DateTimeStr", "{0}日{1,2}時間{2,2}分{3,2}秒", "")
			};
		}
	}
}
