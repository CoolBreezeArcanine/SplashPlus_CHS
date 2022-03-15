namespace DB
{
	public static class TestmodeOutputIDEnum
	{
		private static readonly TestmodeOutputTableRecord[] records;

		public static bool IsActive(this TestmodeOutputID self)
		{
			if (self >= TestmodeOutputID.Title0 && self < TestmodeOutputID.End)
			{
				return self != TestmodeOutputID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeOutputID self)
		{
			if (self >= TestmodeOutputID.Title0)
			{
				return self < TestmodeOutputID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeOutputID self)
		{
			if (self < TestmodeOutputID.Title0)
			{
				self = TestmodeOutputID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeOutputID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeOutputID self)
		{
			return GetEnd();
		}

		public static TestmodeOutputID FindID(string enumName)
		{
			for (TestmodeOutputID testmodeOutputID = TestmodeOutputID.Title0; testmodeOutputID < TestmodeOutputID.End; testmodeOutputID++)
			{
				if (testmodeOutputID.GetEnumName() == enumName)
				{
					return testmodeOutputID;
				}
			}
			return TestmodeOutputID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeOutputID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeOutputID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeOutputID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeOutputID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static TestmodeOutputIDEnum()
		{
			records = new TestmodeOutputTableRecord[19]
			{
				new TestmodeOutputTableRecord(0, "Title0", "输出测试", ""),
				new TestmodeOutputTableRecord(1, "Label00", "按键灯光(1P)", ""),
				new TestmodeOutputTableRecord(2, "Label01", "框体机身灯光(1P)", ""),
				new TestmodeOutputTableRecord(3, "Label02", "画面外周LED(1P)", ""),
				new TestmodeOutputTableRecord(4, "Label03", "侧盖灯光(1P)", ""),
				new TestmodeOutputTableRecord(5, "Label04", "广告牌灯光(1P)", ""),
				new TestmodeOutputTableRecord(6, "Label05", "读卡器LED(1P)", ""),
				new TestmodeOutputTableRecord(7, "Label06", "按键灯光(2P)", ""),
				new TestmodeOutputTableRecord(8, "Label07", "框体机身灯光(2P)", ""),
				new TestmodeOutputTableRecord(9, "Label08", "画面外周LED(2P)", ""),
				new TestmodeOutputTableRecord(10, "Label09", "侧盖灯光(2P)", ""),
				new TestmodeOutputTableRecord(11, "Label10", "广告牌灯光(2P)", ""),
				new TestmodeOutputTableRecord(12, "Label11", "读卡器LED(2P)", ""),
				new TestmodeOutputTableRecord(13, "Label12", "摄像机RING LED", ""),
				new TestmodeOutputTableRecord(14, "Label13", "摄像机REC LED", ""),
				new TestmodeOutputTableRecord(15, "Label14", "禁止投币", ""),
				new TestmodeOutputTableRecord(16, "Label15", "离开", ""),
				new TestmodeOutputTableRecord(17, "BlockerOn", "ON（不可投币）", ""),
				new TestmodeOutputTableRecord(18, "BlockerOff", "OFF（可投币）", "")
			};
		}
	}
}
