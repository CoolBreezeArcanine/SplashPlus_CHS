namespace DB
{
	public static class TestmodeVfdIDEnum
	{
		private static readonly TestmodeVfdTableRecord[] records = new TestmodeVfdTableRecord[2]
		{
			new TestmodeVfdTableRecord(0, "Title0", "VFD表示テスト", ""),
			new TestmodeVfdTableRecord(1, "Label00", "VFDをご覧ください", "")
		};

		public static bool IsActive(this TestmodeVfdID self)
		{
			if (self >= TestmodeVfdID.Title0 && self < TestmodeVfdID.End)
			{
				return self != TestmodeVfdID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeVfdID self)
		{
			if (self >= TestmodeVfdID.Title0)
			{
				return self < TestmodeVfdID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeVfdID self)
		{
			if (self < TestmodeVfdID.Title0)
			{
				self = TestmodeVfdID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeVfdID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeVfdID self)
		{
			return GetEnd();
		}

		public static TestmodeVfdID FindID(string enumName)
		{
			for (TestmodeVfdID testmodeVfdID = TestmodeVfdID.Title0; testmodeVfdID < TestmodeVfdID.End; testmodeVfdID++)
			{
				if (testmodeVfdID.GetEnumName() == enumName)
				{
					return testmodeVfdID;
				}
			}
			return TestmodeVfdID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeVfdID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeVfdID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeVfdID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeVfdID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}
	}
}
