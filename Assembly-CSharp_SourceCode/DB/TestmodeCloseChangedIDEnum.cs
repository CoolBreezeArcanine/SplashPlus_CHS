namespace DB
{
	public static class TestmodeCloseChangedIDEnum
	{
		private static readonly TestmodeCloseChangedTableRecord[] records;

		public static bool IsActive(this TestmodeCloseChangedID self)
		{
			if (self >= TestmodeCloseChangedID.Title0 && self < TestmodeCloseChangedID.End)
			{
				return self != TestmodeCloseChangedID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeCloseChangedID self)
		{
			if (self >= TestmodeCloseChangedID.Title0)
			{
				return self < TestmodeCloseChangedID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeCloseChangedID self)
		{
			if (self < TestmodeCloseChangedID.Title0)
			{
				self = TestmodeCloseChangedID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeCloseChangedID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeCloseChangedID self)
		{
			return GetEnd();
		}

		public static TestmodeCloseChangedID FindID(string enumName)
		{
			for (TestmodeCloseChangedID testmodeCloseChangedID = TestmodeCloseChangedID.Title0; testmodeCloseChangedID < TestmodeCloseChangedID.End; testmodeCloseChangedID++)
			{
				if (testmodeCloseChangedID.GetEnumName() == enumName)
				{
					return testmodeCloseChangedID;
				}
			}
			return TestmodeCloseChangedID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeCloseChangedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeCloseChangedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeCloseChangedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeCloseChangedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static TestmodeCloseChangedIDEnum()
		{
			records = new TestmodeCloseChangedTableRecord[1]
			{
				new TestmodeCloseChangedTableRecord(0, "Title0", "已恢复到出厂设置", "")
			};
		}
	}
}
