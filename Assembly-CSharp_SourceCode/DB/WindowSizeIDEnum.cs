namespace DB
{
	public static class WindowSizeIDEnum
	{
		private static readonly WindowSizeTableRecord[] records = new WindowSizeTableRecord[5]
		{
			new WindowSizeTableRecord(0, "Large", "大"),
			new WindowSizeTableRecord(1, "LargeVertical", "大・縦分割"),
			new WindowSizeTableRecord(2, "LargeHorizontal", "大・横分割"),
			new WindowSizeTableRecord(3, "Middle", "中"),
			new WindowSizeTableRecord(4, "Small", "小")
		};

		public static bool IsActive(this WindowSizeID self)
		{
			if (self >= WindowSizeID.Large && self < WindowSizeID.End)
			{
				return self != WindowSizeID.Large;
			}
			return false;
		}

		public static bool IsValid(this WindowSizeID self)
		{
			if (self >= WindowSizeID.Large)
			{
				return self < WindowSizeID.End;
			}
			return false;
		}

		public static void Clamp(this WindowSizeID self)
		{
			if (self < WindowSizeID.Large)
			{
				self = WindowSizeID.Large;
			}
			else if ((int)self >= GetEnd())
			{
				self = (WindowSizeID)GetEnd();
			}
		}

		public static int GetEnd(this WindowSizeID self)
		{
			return GetEnd();
		}

		public static WindowSizeID FindID(string enumName)
		{
			for (WindowSizeID windowSizeID = WindowSizeID.Large; windowSizeID < WindowSizeID.End; windowSizeID++)
			{
				if (windowSizeID.GetEnumName() == enumName)
				{
					return windowSizeID;
				}
			}
			return WindowSizeID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this WindowSizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this WindowSizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this WindowSizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
