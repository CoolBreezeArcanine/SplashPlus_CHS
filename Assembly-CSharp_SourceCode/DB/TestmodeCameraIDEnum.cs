namespace DB
{
	public static class TestmodeCameraIDEnum
	{
		private static readonly TestmodeCameraTableRecord[] records;

		public static bool IsActive(this TestmodeCameraID self)
		{
			if (self >= TestmodeCameraID.Title0 && self < TestmodeCameraID.End)
			{
				return self != TestmodeCameraID.Title0;
			}
			return false;
		}

		public static bool IsValid(this TestmodeCameraID self)
		{
			if (self >= TestmodeCameraID.Title0)
			{
				return self < TestmodeCameraID.End;
			}
			return false;
		}

		public static void Clamp(this TestmodeCameraID self)
		{
			if (self < TestmodeCameraID.Title0)
			{
				self = TestmodeCameraID.Title0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (TestmodeCameraID)GetEnd();
			}
		}

		public static int GetEnd(this TestmodeCameraID self)
		{
			return GetEnd();
		}

		public static TestmodeCameraID FindID(string enumName)
		{
			for (TestmodeCameraID testmodeCameraID = TestmodeCameraID.Title0; testmodeCameraID < TestmodeCameraID.End; testmodeCameraID++)
			{
				if (testmodeCameraID.GetEnumName() == enumName)
				{
					return testmodeCameraID;
				}
			}
			return TestmodeCameraID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this TestmodeCameraID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this TestmodeCameraID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this TestmodeCameraID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this TestmodeCameraID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static TestmodeCameraIDEnum()
		{
			records = new TestmodeCameraTableRecord[10]
			{
				new TestmodeCameraTableRecord(0, "Title0", "摄像机测试", ""),
				new TestmodeCameraTableRecord(1, "SubTitle", "摄像机视图", ""),
				new TestmodeCameraTableRecord(2, "ListLabel00", "摄像机状态", ""),
				new TestmodeCameraTableRecord(3, "ListLabel01", "类型", ""),
				new TestmodeCameraTableRecord(4, "ListLabel02", "认证卡", ""),
				new TestmodeCameraTableRecord(5, "ListLabel03", "检测", ""),
				new TestmodeCameraTableRecord(6, "CameraInit", "摄像机正在初始化", ""),
				new TestmodeCameraTableRecord(7, "LeftCamera", "二维码扫描(1P)", ""),
				new TestmodeCameraTableRecord(8, "RightCamera", "二维码扫描(2P)", ""),
				new TestmodeCameraTableRecord(9, "PhotoCamera", "玩家摄像机", "")
			};
		}
	}
}
