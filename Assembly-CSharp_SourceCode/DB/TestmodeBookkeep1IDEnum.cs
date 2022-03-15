namespace DB
{
	public static class TestmodeBookkeep1IDEnum
	{
		private static readonly TestmodeBookkeep1TableRecord[] records;

		public static bool IsActive(this TestmodeBookkeep1ID self)
		{
			if (self >= TestmodeBookkeep1ID.Title0 && self < TestmodeBookkeep1ID.End)
			{
				return self != TestmodeBookkeep1ID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeBookkeep1ID self)
		{
			if (self >= TestmodeBookkeep1ID.Title0)
			{
				return self < TestmodeBookkeep1ID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeBookkeep1ID self)
		{
			if (self < TestmodeBookkeep1ID.Title0)
			{
				self = TestmodeBookkeep1ID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeBookkeep1ID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeBookkeep1ID self)
		{
			return GetEnd();
		}

		public static TestmodeBookkeep1ID FindID(string enumName)
		{
			for (TestmodeBookkeep1ID testmodeBookkeep1ID = TestmodeBookkeep1ID.Title0; testmodeBookkeep1ID < TestmodeBookkeep1ID.End; testmodeBookkeep1ID++)
			{
				if (testmodeBookkeep1ID.GetEnumName() == enumName)
				{
					return testmodeBookkeep1ID;
				}
			}
			return TestmodeBookkeep1ID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeBookkeep1ID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeBookkeep1ID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeBookkeep1ID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeBookkeep1ID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static TestmodeBookkeep1IDEnum()
		{
			records = new TestmodeBookkeep1TableRecord[9]
			{
				new TestmodeBookkeep1TableRecord(0, "Title0", "簿记(1/3)", ""),
				new TestmodeBookkeep1TableRecord(1, "Label00", "投币1", ""),
				new TestmodeBookkeep1TableRecord(2, "Label01", "投币2", ""),
				new TestmodeBookkeep1TableRecord(3, "Label02", "总投币数", ""),
				new TestmodeBookkeep1TableRecord(4, "Label03", "投币可用点数", ""),
				new TestmodeBookkeep1TableRecord(5, "Label04", "服务可用点数", ""),
				new TestmodeBookkeep1TableRecord(6, "Label05", "总可用点数", ""),
				new TestmodeBookkeep1TableRecord(7, "Label06", "サービスクレジット", ""),
				new TestmodeBookkeep1TableRecord(8, "Label07", "トータルクレジット", "")
			};
		}
	}
}
