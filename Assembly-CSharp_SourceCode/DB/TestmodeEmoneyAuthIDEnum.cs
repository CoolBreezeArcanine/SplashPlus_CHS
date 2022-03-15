namespace DB
{
	public static class TestmodeEmoneyAuthIDEnum
	{
		private static readonly TestmodeEmoneyAuthTableRecord[] records = new TestmodeEmoneyAuthTableRecord[5]
		{
			new TestmodeEmoneyAuthTableRecord(0, "Title0", "電子決済端末認証"),
			new TestmodeEmoneyAuthTableRecord(1, "Title1", ""),
			new TestmodeEmoneyAuthTableRecord(2, "Title2", "「認証する」を選ぶと、認証を開始します"),
			new TestmodeEmoneyAuthTableRecord(3, "Label00", "認証する"),
			new TestmodeEmoneyAuthTableRecord(4, "Label01", "認証しない")
		};

		public static bool IsActive(this TestmodeEmoneyAuthID self)
		{
			if (self >= TestmodeEmoneyAuthID.Title0 && self < TestmodeEmoneyAuthID.End)
			{
				return self != TestmodeEmoneyAuthID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeEmoneyAuthID self)
		{
			if (self >= TestmodeEmoneyAuthID.Title0)
			{
				return self < TestmodeEmoneyAuthID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeEmoneyAuthID self)
		{
			if (self < TestmodeEmoneyAuthID.Title0)
			{
				self = TestmodeEmoneyAuthID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeEmoneyAuthID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeEmoneyAuthID self)
		{
			return GetEnd();
		}

		public static TestmodeEmoneyAuthID FindID(string enumName)
		{
			for (TestmodeEmoneyAuthID testmodeEmoneyAuthID = TestmodeEmoneyAuthID.Title0; testmodeEmoneyAuthID < TestmodeEmoneyAuthID.End; testmodeEmoneyAuthID++)
			{
				if (testmodeEmoneyAuthID.GetEnumName() == enumName)
				{
					return testmodeEmoneyAuthID;
				}
			}
			return TestmodeEmoneyAuthID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeEmoneyAuthID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeEmoneyAuthID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeEmoneyAuthID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
