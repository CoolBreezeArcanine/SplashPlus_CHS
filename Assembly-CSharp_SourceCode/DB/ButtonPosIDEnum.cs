namespace DB
{
	public static class ButtonPosIDEnum
	{
		private static readonly ButtonPosTableRecord[] records = new ButtonPosTableRecord[8]
		{
			new ButtonPosTableRecord(0, "Button1", "1番ボタン"),
			new ButtonPosTableRecord(1, "Button2", "2番ボタン"),
			new ButtonPosTableRecord(2, "Button3", "3番ボタン"),
			new ButtonPosTableRecord(3, "Button4", "4番ボタン"),
			new ButtonPosTableRecord(4, "Button5", "5番ボタン"),
			new ButtonPosTableRecord(5, "Button6", "6番ボタン"),
			new ButtonPosTableRecord(6, "Button7", "7番ボタン"),
			new ButtonPosTableRecord(7, "Button8", "8番ボタン")
		};

		public static bool IsActive(this ButtonPosID self)
		{
			if (self >= ButtonPosID.Button1 && self < ButtonPosID.End)
			{
				return self != ButtonPosID.Button1;
			}
			return false;
		}

		public static bool IsValid(this ButtonPosID self)
		{
			if (self >= ButtonPosID.Button1)
			{
				return self < ButtonPosID.End;
			}
			return false;
		}

		public static void Clamp(this ButtonPosID self)
		{
			if (self < ButtonPosID.Button1)
			{
				self = ButtonPosID.Button1;
			}
			else if ((int)self >= GetEnd())
			{
				self = (ButtonPosID)GetEnd();
			}
		}

		public static int GetEnd(this ButtonPosID self)
		{
			return GetEnd();
		}

		public static ButtonPosID FindID(string enumName)
		{
			for (ButtonPosID buttonPosID = ButtonPosID.Button1; buttonPosID < ButtonPosID.End; buttonPosID++)
			{
				if (buttonPosID.GetEnumName() == enumName)
				{
					return buttonPosID;
				}
			}
			return ButtonPosID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this ButtonPosID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this ButtonPosID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this ButtonPosID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
