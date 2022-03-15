namespace DB
{
	public static class TestmodeEmoneyRemoveIDEnum
	{
		private static readonly TestmodeEmoneyRemoveTableRecord[] records = new TestmodeEmoneyRemoveTableRecord[5]
		{
			new TestmodeEmoneyRemoveTableRecord(0, "Title0", "電子決済端末撤去"),
			new TestmodeEmoneyRemoveTableRecord(1, "Title1", ""),
			new TestmodeEmoneyRemoveTableRecord(2, "Title2", "「撤去する」を選ぶと、端末撤去を開始します"),
			new TestmodeEmoneyRemoveTableRecord(3, "Label00", "撤去する"),
			new TestmodeEmoneyRemoveTableRecord(4, "Label01", "撤去しない")
		};

		public static bool IsActive(this TestmodeEmoneyRemoveID self)
		{
			if (self >= TestmodeEmoneyRemoveID.Title0 && self < TestmodeEmoneyRemoveID.End)
			{
				return self != TestmodeEmoneyRemoveID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeEmoneyRemoveID self)
		{
			if (self >= TestmodeEmoneyRemoveID.Title0)
			{
				return self < TestmodeEmoneyRemoveID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeEmoneyRemoveID self)
		{
			if (self < TestmodeEmoneyRemoveID.Title0)
			{
				self = TestmodeEmoneyRemoveID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeEmoneyRemoveID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeEmoneyRemoveID self)
		{
			return GetEnd();
		}

		public static TestmodeEmoneyRemoveID FindID(string enumName)
		{
			for (TestmodeEmoneyRemoveID testmodeEmoneyRemoveID = TestmodeEmoneyRemoveID.Title0; testmodeEmoneyRemoveID < TestmodeEmoneyRemoveID.End; testmodeEmoneyRemoveID++)
			{
				if (testmodeEmoneyRemoveID.GetEnumName() == enumName)
				{
					return testmodeEmoneyRemoveID;
				}
			}
			return TestmodeEmoneyRemoveID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeEmoneyRemoveID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeEmoneyRemoveID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeEmoneyRemoveID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
