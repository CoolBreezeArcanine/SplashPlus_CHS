namespace DB
{
	public static class ButtonKindIDEnum
	{
		private static readonly ButtonKindTableRecord[] records = new ButtonKindTableRecord[4]
		{
			new ButtonKindTableRecord(0, "Circle", "丸ボタン"),
			new ButtonKindTableRecord(1, "Flat", "フラットボタン"),
			new ButtonKindTableRecord(2, "Square", "四角ボタン"),
			new ButtonKindTableRecord(3, "Horizon", "垂直ボタン")
		};

		public static bool IsActive(this ButtonKindID self)
		{
			if (self >= ButtonKindID.Circle && self < ButtonKindID.End)
			{
				return self != ButtonKindID.Circle;
			}
			return false;
		}

		public static bool IsValid(this ButtonKindID self)
		{
			if (self >= ButtonKindID.Circle)
			{
				return self < ButtonKindID.End;
			}
			return false;
		}

		public static void Clamp(this ButtonKindID self)
		{
			if (self < ButtonKindID.Circle)
			{
				self = ButtonKindID.Circle;
			}
			else if ((int)self >= GetEnd())
			{
				self = (ButtonKindID)GetEnd();
			}
		}

		public static int GetEnd(this ButtonKindID self)
		{
			return GetEnd();
		}

		public static ButtonKindID FindID(string enumName)
		{
			for (ButtonKindID buttonKindID = ButtonKindID.Circle; buttonKindID < ButtonKindID.End; buttonKindID++)
			{
				if (buttonKindID.GetEnumName() == enumName)
				{
					return buttonKindID;
				}
			}
			return ButtonKindID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this ButtonKindID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this ButtonKindID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this ButtonKindID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
