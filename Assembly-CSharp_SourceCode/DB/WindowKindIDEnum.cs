namespace DB
{
	public static class WindowKindIDEnum
	{
		private static readonly WindowKindTableRecord[] records = new WindowKindTableRecord[2]
		{
			new WindowKindTableRecord(0, "Common", "ノーマル"),
			new WindowKindTableRecord(1, "Attention", "アテンション")
		};

		public static bool IsActive(this WindowKindID self)
		{
			if (self >= WindowKindID.Common && self < WindowKindID.End)
			{
				return self != WindowKindID.Common;
			}
			return false;
		}

		public static bool IsValid(this WindowKindID self)
		{
			if (self >= WindowKindID.Common)
			{
				return self < WindowKindID.End;
			}
			return false;
		}

		public static void Clamp(this WindowKindID self)
		{
			if (self < WindowKindID.Common)
			{
				self = WindowKindID.Common;
			}
			else if ((int)self >= GetEnd())
			{
				self = (WindowKindID)GetEnd();
			}
		}

		public static int GetEnd(this WindowKindID self)
		{
			return GetEnd();
		}

		public static WindowKindID FindID(string enumName)
		{
			for (WindowKindID windowKindID = WindowKindID.Common; windowKindID < WindowKindID.End; windowKindID++)
			{
				if (windowKindID.GetEnumName() == enumName)
				{
					return windowKindID;
				}
			}
			return WindowKindID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this WindowKindID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this WindowKindID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this WindowKindID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
