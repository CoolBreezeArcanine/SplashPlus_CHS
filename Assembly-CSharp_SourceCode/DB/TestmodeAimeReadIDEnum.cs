namespace DB
{
	public static class TestmodeAimeReadIDEnum
	{
		private static readonly TestmodeAimeReadTableRecord[] records = new TestmodeAimeReadTableRecord[21]
		{
			new TestmodeAimeReadTableRecord(0, "Title0", "Aimeカードリーダーテスト", ""),
			new TestmodeAimeReadTableRecord(1, "Label00", "ハードウェアバージョン", ""),
			new TestmodeAimeReadTableRecord(2, "Label01", "ファームウェアバージョン", ""),
			new TestmodeAimeReadTableRecord(3, "Label02", "カードリーダー状態", ""),
			new TestmodeAimeReadTableRecord(4, "Label03", "読み取り結果", ""),
			new TestmodeAimeReadTableRecord(5, "Label04", "所要時間", ""),
			new TestmodeAimeReadTableRecord(6, "Label05", "LED状態", ""),
			new TestmodeAimeReadTableRecord(7, "Label06", "読み取り開始", ""),
			new TestmodeAimeReadTableRecord(8, "Label07", "LEDチェック", ""),
			new TestmodeAimeReadTableRecord(9, "Label08", "終了", ""),
			new TestmodeAimeReadTableRecord(10, "Label02_00", "待機中", ""),
			new TestmodeAimeReadTableRecord(11, "Label02_01", "読み取り中…", ""),
			new TestmodeAimeReadTableRecord(12, "Label02_02", "読み取り終了", ""),
			new TestmodeAimeReadTableRecord(13, "Label05_00", "OFF", ""),
			new TestmodeAimeReadTableRecord(14, "Label05_01", "ON(RED)", ""),
			new TestmodeAimeReadTableRecord(15, "Label05_02", "ON(GREEN)", ""),
			new TestmodeAimeReadTableRecord(16, "Label05_03", "ON(BLUE)", ""),
			new TestmodeAimeReadTableRecord(17, "Label05_04", "ON(WHITE)", ""),
			new TestmodeAimeReadTableRecord(18, "Label05_05", "----", ""),
			new TestmodeAimeReadTableRecord(19, "Label06_00", "読み取り開始", ""),
			new TestmodeAimeReadTableRecord(20, "Label06_01", "読み取り停止", "")
		};

		public static bool IsActive(this TestmodeAimeReadID self)
		{
			if (self >= TestmodeAimeReadID.Title0 && self < TestmodeAimeReadID.End)
			{
				return self != TestmodeAimeReadID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeAimeReadID self)
		{
			if (self >= TestmodeAimeReadID.Title0)
			{
				return self < TestmodeAimeReadID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeAimeReadID self)
		{
			if (self < TestmodeAimeReadID.Title0)
			{
				self = TestmodeAimeReadID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeAimeReadID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeAimeReadID self)
		{
			return GetEnd();
		}

		public static TestmodeAimeReadID FindID(string enumName)
		{
			for (TestmodeAimeReadID testmodeAimeReadID = TestmodeAimeReadID.Title0; testmodeAimeReadID < TestmodeAimeReadID.End; testmodeAimeReadID++)
			{
				if (testmodeAimeReadID.GetEnumName() == enumName)
				{
					return testmodeAimeReadID;
				}
			}
			return TestmodeAimeReadID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeAimeReadID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeAimeReadID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeAimeReadID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeAimeReadID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}
	}
}
