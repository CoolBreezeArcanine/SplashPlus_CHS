namespace DB
{
	public static class TestmodeDebugSoundtestIDEnum
	{
		private static readonly TestmodeDebugSoundtestTableRecord[] records = new TestmodeDebugSoundtestTableRecord[19]
		{
			new TestmodeDebugSoundtestTableRecord(0, "Title0", "デバッグ：サウンドテスト", ""),
			new TestmodeDebugSoundtestTableRecord(1, "Label00", "SE", ""),
			new TestmodeDebugSoundtestTableRecord(2, "Label01", "戻る", ""),
			new TestmodeDebugSoundtestTableRecord(3, "Label02", "再生", ""),
			new TestmodeDebugSoundtestTableRecord(4, "Label03", "進む", ""),
			new TestmodeDebugSoundtestTableRecord(5, "Label04", "VOICE", ""),
			new TestmodeDebugSoundtestTableRecord(6, "Label05", "戻る", ""),
			new TestmodeDebugSoundtestTableRecord(7, "Label06", "再生", ""),
			new TestmodeDebugSoundtestTableRecord(8, "Label07", "進む", ""),
			new TestmodeDebugSoundtestTableRecord(9, "Label08", "P_VOICE", ""),
			new TestmodeDebugSoundtestTableRecord(10, "Label09", "戻る", ""),
			new TestmodeDebugSoundtestTableRecord(11, "Label10", "再生", ""),
			new TestmodeDebugSoundtestTableRecord(12, "Label11", "進む", ""),
			new TestmodeDebugSoundtestTableRecord(13, "Label12", "パートナー", ""),
			new TestmodeDebugSoundtestTableRecord(14, "Label13", "楽曲", ""),
			new TestmodeDebugSoundtestTableRecord(15, "Label14", "戻る", ""),
			new TestmodeDebugSoundtestTableRecord(16, "Label15", "再生", ""),
			new TestmodeDebugSoundtestTableRecord(17, "Label16", "進む", ""),
			new TestmodeDebugSoundtestTableRecord(18, "Label17", "終了", "")
		};

		public static bool IsActive(this TestmodeDebugSoundtestID self)
		{
			if (self >= TestmodeDebugSoundtestID.Title0 && self < TestmodeDebugSoundtestID.End)
			{
				return self != TestmodeDebugSoundtestID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeDebugSoundtestID self)
		{
			if (self >= TestmodeDebugSoundtestID.Title0)
			{
				return self < TestmodeDebugSoundtestID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeDebugSoundtestID self)
		{
			if (self < TestmodeDebugSoundtestID.Title0)
			{
				self = TestmodeDebugSoundtestID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeDebugSoundtestID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeDebugSoundtestID self)
		{
			return GetEnd();
		}

		public static TestmodeDebugSoundtestID FindID(string enumName)
		{
			for (TestmodeDebugSoundtestID testmodeDebugSoundtestID = TestmodeDebugSoundtestID.Title0; testmodeDebugSoundtestID < TestmodeDebugSoundtestID.End; testmodeDebugSoundtestID++)
			{
				if (testmodeDebugSoundtestID.GetEnumName() == enumName)
				{
					return testmodeDebugSoundtestID;
				}
			}
			return TestmodeDebugSoundtestID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeDebugSoundtestID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeDebugSoundtestID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeDebugSoundtestID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeDebugSoundtestID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}
	}
}
