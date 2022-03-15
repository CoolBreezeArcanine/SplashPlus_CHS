namespace DB
{
	public static class TestmodeEmoneyIDEnum
	{
		private static readonly TestmodeEmoneyTableRecord[] records = new TestmodeEmoneyTableRecord[13]
		{
			new TestmodeEmoneyTableRecord(0, "Title0", "電子決済情報", ""),
			new TestmodeEmoneyTableRecord(1, "Label00", "端末認証", ""),
			new TestmodeEmoneyTableRecord(2, "Label01", "端末撤去", ""),
			new TestmodeEmoneyTableRecord(3, "Label02", "終了", ""),
			new TestmodeEmoneyTableRecord(4, "Label03", "端末ＩＤ", ""),
			new TestmodeEmoneyTableRecord(5, "Label04", "ブランド", ""),
			new TestmodeEmoneyTableRecord(6, "Label05", "", ""),
			new TestmodeEmoneyTableRecord(7, "Label06", "", ""),
			new TestmodeEmoneyTableRecord(8, "Label07", "", ""),
			new TestmodeEmoneyTableRecord(9, "Label08", "", ""),
			new TestmodeEmoneyTableRecord(10, "Label09", "", ""),
			new TestmodeEmoneyTableRecord(11, "Label10", "", ""),
			new TestmodeEmoneyTableRecord(12, "Label11", "", "")
		};

		public static bool IsActive(this TestmodeEmoneyID self)
		{
			if (self >= TestmodeEmoneyID.Title0 && self < TestmodeEmoneyID.End)
			{
				return self != TestmodeEmoneyID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeEmoneyID self)
		{
			if (self >= TestmodeEmoneyID.Title0)
			{
				return self < TestmodeEmoneyID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeEmoneyID self)
		{
			if (self < TestmodeEmoneyID.Title0)
			{
				self = TestmodeEmoneyID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeEmoneyID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeEmoneyID self)
		{
			return GetEnd();
		}

		public static TestmodeEmoneyID FindID(string enumName)
		{
			for (TestmodeEmoneyID testmodeEmoneyID = TestmodeEmoneyID.Title0; testmodeEmoneyID < TestmodeEmoneyID.End; testmodeEmoneyID++)
			{
				if (testmodeEmoneyID.GetEnumName() == enumName)
				{
					return testmodeEmoneyID;
				}
			}
			return TestmodeEmoneyID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeEmoneyID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeEmoneyID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeEmoneyID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeEmoneyID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}
	}
}
