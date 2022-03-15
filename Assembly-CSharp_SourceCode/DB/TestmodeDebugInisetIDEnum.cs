namespace DB
{
	public static class TestmodeDebugInisetIDEnum
	{
		private static readonly TestmodeDebugInisetTableRecord[] records = new TestmodeDebugInisetTableRecord[20]
		{
			new TestmodeDebugInisetTableRecord(0, "Title0", "デバッグ：Ini編集"),
			new TestmodeDebugInisetTableRecord(1, "Label00", "デバッグ表示"),
			new TestmodeDebugInisetTableRecord(2, "Label01", "全曲全譜面解放"),
			new TestmodeDebugInisetTableRecord(3, "Label02", "全コレクション獲得"),
			new TestmodeDebugInisetTableRecord(4, "Label03", "全キャラ獲得"),
			new TestmodeDebugInisetTableRecord(5, "Label04", "最大トラック数(0～4=>0なら無効)"),
			new TestmodeDebugInisetTableRecord(6, "Label05", "譜面チェックモード"),
			new TestmodeDebugInisetTableRecord(7, "Label06", "イベント開催上書き"),
			new TestmodeDebugInisetTableRecord(8, "Label07", "マップ進行距離(0なら無効)"),
			new TestmodeDebugInisetTableRecord(9, "Label08", "ゴーストの達成率(0なら無効)"),
			new TestmodeDebugInisetTableRecord(10, "Label09", "ゴーストの腕前(0なら無効)"),
			new TestmodeDebugInisetTableRecord(11, "Label10", "強制Dxスコア（T）"),
			new TestmodeDebugInisetTableRecord(12, "Label11", "強制Dxスコア（Y）"),
			new TestmodeDebugInisetTableRecord(13, "Label12", "強制Dxスコア（U）"),
			new TestmodeDebugInisetTableRecord(14, "Label13", "強制Dxスコア（I）"),
			new TestmodeDebugInisetTableRecord(15, "Label14", "強制達成率（T）"),
			new TestmodeDebugInisetTableRecord(16, "Label15", "強制達成率（Y）"),
			new TestmodeDebugInisetTableRecord(17, "Label16", "強制達成率（U）"),
			new TestmodeDebugInisetTableRecord(18, "Label17", "強制達成率（I）"),
			new TestmodeDebugInisetTableRecord(19, "Label18", "終了")
		};

		public static bool IsActive(this TestmodeDebugInisetID self)
		{
			if (self >= TestmodeDebugInisetID.Title0 && self < TestmodeDebugInisetID.End)
			{
				return self != TestmodeDebugInisetID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeDebugInisetID self)
		{
			if (self >= TestmodeDebugInisetID.Title0)
			{
				return self < TestmodeDebugInisetID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeDebugInisetID self)
		{
			if (self < TestmodeDebugInisetID.Title0)
			{
				self = TestmodeDebugInisetID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeDebugInisetID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeDebugInisetID self)
		{
			return GetEnd();
		}

		public static TestmodeDebugInisetID FindID(string enumName)
		{
			for (TestmodeDebugInisetID testmodeDebugInisetID = TestmodeDebugInisetID.Title0; testmodeDebugInisetID < TestmodeDebugInisetID.End; testmodeDebugInisetID++)
			{
				if (testmodeDebugInisetID.GetEnumName() == enumName)
				{
					return testmodeDebugInisetID;
				}
			}
			return TestmodeDebugInisetID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeDebugInisetID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeDebugInisetID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeDebugInisetID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
