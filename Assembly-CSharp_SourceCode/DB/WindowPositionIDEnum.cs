namespace DB
{
	public static class WindowPositionIDEnum
	{
		private static readonly WindowPositionTableRecord[] records = new WindowPositionTableRecord[3]
		{
			new WindowPositionTableRecord(0, "Upper", "上"),
			new WindowPositionTableRecord(1, "Middle", "中央"),
			new WindowPositionTableRecord(2, "Lower", "下")
		};

		public static bool IsActive(this WindowPositionID self)
		{
			if (self >= WindowPositionID.Upper && self < WindowPositionID.End)
			{
				return self != WindowPositionID.Upper;
			}
			return false;
		}

		public static bool IsValid(this WindowPositionID self)
		{
			if (self >= WindowPositionID.Upper)
			{
				return self < WindowPositionID.End;
			}
			return false;
		}

		public static void Clamp(this WindowPositionID self)
		{
			if (self < WindowPositionID.Upper)
			{
				self = WindowPositionID.Upper;
			}
			else if ((int)self >= GetEnd())
			{
				self = (WindowPositionID)GetEnd();
			}
		}

		public static int GetEnd(this WindowPositionID self)
		{
			return GetEnd();
		}

		public static WindowPositionID FindID(string enumName)
		{
			for (WindowPositionID windowPositionID = WindowPositionID.Upper; windowPositionID < WindowPositionID.End; windowPositionID++)
			{
				if (windowPositionID.GetEnumName() == enumName)
				{
					return windowPositionID;
				}
			}
			return WindowPositionID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this WindowPositionID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this WindowPositionID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this WindowPositionID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
