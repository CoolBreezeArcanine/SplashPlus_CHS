namespace DB
{
	public static class TestmodeBookkeep2IDEnum
	{
		private static readonly TestmodeBookkeep2TableRecord[] records;

		public static bool IsActive(this TestmodeBookkeep2ID self)
		{
			if (self >= TestmodeBookkeep2ID.Title0 && self < TestmodeBookkeep2ID.End)
			{
				return self != TestmodeBookkeep2ID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeBookkeep2ID self)
		{
			if (self >= TestmodeBookkeep2ID.Title0)
			{
				return self < TestmodeBookkeep2ID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeBookkeep2ID self)
		{
			if (self < TestmodeBookkeep2ID.Title0)
			{
				self = TestmodeBookkeep2ID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeBookkeep2ID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeBookkeep2ID self)
		{
			return GetEnd();
		}

		public static TestmodeBookkeep2ID FindID(string enumName)
		{
			for (TestmodeBookkeep2ID testmodeBookkeep2ID = TestmodeBookkeep2ID.Title0; testmodeBookkeep2ID < TestmodeBookkeep2ID.End; testmodeBookkeep2ID++)
			{
				if (testmodeBookkeep2ID.GetEnumName() == enumName)
				{
					return testmodeBookkeep2ID;
				}
			}
			return TestmodeBookkeep2ID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeBookkeep2ID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeBookkeep2ID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeBookkeep2ID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeBookkeep2ID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static TestmodeBookkeep2IDEnum()
		{
			records = new TestmodeBookkeep2TableRecord[12]
			{
				new TestmodeBookkeep2TableRecord(0, "Title0", "簿记(2/3)", ""),
				new TestmodeBookkeep2TableRecord(1, "Label00", "开始游戏需要点数（单人游戏）", ""),
				new TestmodeBookkeep2TableRecord(2, "Label01", "开始游戏需要点数（双人游戏）", ""),
				new TestmodeBookkeep2TableRecord(3, "Label02", "开始游戏需要点数（自由模式）", ""),
				new TestmodeBookkeep2TableRecord(4, "Label03", "开始游戏需要点数（Ticket）", ""),
				new TestmodeBookkeep2TableRecord(5, "Label04", "总游戏次数", ""),
				new TestmodeBookkeep2TableRecord(6, "Label05", "\u3000会员游玩次数", ""),
				new TestmodeBookkeep2TableRecord(7, "Label06", "\u3000游客游玩次数", ""),
				new TestmodeBookkeep2TableRecord(8, "Label07", "单人游戏次数", ""),
				new TestmodeBookkeep2TableRecord(9, "Label08", "双人游戏次数", ""),
				new TestmodeBookkeep2TableRecord(10, "Label09", "自由模式选择次数", ""),
				new TestmodeBookkeep2TableRecord(11, "Label10", "初次免费游戏次数", "")
			};
		}
	}
}
