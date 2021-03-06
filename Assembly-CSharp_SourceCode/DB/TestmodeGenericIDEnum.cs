namespace DB
{
	public static class TestmodeGenericIDEnum
	{
		private static readonly TestmodeGenericTableRecord[] records;

		public static bool IsActive(this TestmodeGenericID self)
		{
			if (self >= TestmodeGenericID.OpTypeSelect && self < TestmodeGenericID.End)
			{
				return self != TestmodeGenericID.OpTypeSelect;
			}
			return false;
		}

		public static bool IsValid(this TestmodeGenericID self)
		{
			if (self >= TestmodeGenericID.OpTypeSelect)
			{
				return self < TestmodeGenericID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeGenericID self)
		{
			if (self < TestmodeGenericID.OpTypeSelect)
			{
				self = TestmodeGenericID.OpTypeSelect;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeGenericID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeGenericID self)
		{
			return GetEnd();
		}

		public static TestmodeGenericID FindID(string enumName)
		{
			for (TestmodeGenericID testmodeGenericID = TestmodeGenericID.OpTypeSelect; testmodeGenericID < TestmodeGenericID.End; testmodeGenericID++)
			{
				if (testmodeGenericID.GetEnumName() == enumName)
				{
					return testmodeGenericID;
				}
			}
			return TestmodeGenericID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeGenericID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeGenericID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeGenericID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeGenericID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static TestmodeGenericIDEnum()
		{
			records = new TestmodeGenericTableRecord[5]
			{
				new TestmodeGenericTableRecord(0, "OpTypeSelect", "????????????????????? ??????????????????", "SELECT WITH SERVICE BUTTON, \nAND PRESS TEST BUTTON"),
				new TestmodeGenericTableRecord(1, "OpTypeTextExit", "??????????????????", "PRESS TEST BUTTON TO EXIT"),
				new TestmodeGenericTableRecord(2, "OpTypeTestServiceExit", "????????????????????????????????????", "PRESS TEST AND SERVICE BUTTON TO EXIT"),
				new TestmodeGenericTableRecord(3, "OpTypeTextContinue", "?????????????????????", "PRESS TEST BUTTON TO CONTINUE"),
				new TestmodeGenericTableRecord(4, "OpTypeServiceAbort", "????????????????????????????????????", "PRESS TEST AND SERVICE BUTTON TO ABORT")
			};
		}
	}
}
