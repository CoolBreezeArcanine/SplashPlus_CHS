namespace DB
{
	public static class ButtonTypeIDEnum
	{
		private static readonly ButtonTypeTableRecord[] records = new ButtonTypeTableRecord[1]
		{
			new ButtonTypeTableRecord(0, "N_Plus", "通常+")
		};

		public static bool IsActive(this ButtonTypeID self)
		{
			if (self >= ButtonTypeID.N_Plus && self < ButtonTypeID.End)
			{
				return self != ButtonTypeID.N_Plus;
			}
			return false;
		}

		public static bool IsValid(this ButtonTypeID self)
		{
			if (self >= ButtonTypeID.N_Plus)
			{
				return self < ButtonTypeID.End;
			}
			return false;
		}

		public static void Clamp(this ButtonTypeID self)
		{
			if (self < ButtonTypeID.N_Plus)
			{
				self = ButtonTypeID.N_Plus;
			}
			else if ((int)self >= GetEnd())
			{
				self = (ButtonTypeID)GetEnd();
			}
		}

		public static int GetEnd(this ButtonTypeID self)
		{
			return GetEnd();
		}

		public static ButtonTypeID FindID(string enumName)
		{
			for (ButtonTypeID buttonTypeID = ButtonTypeID.N_Plus; buttonTypeID < ButtonTypeID.End; buttonTypeID++)
			{
				if (buttonTypeID.GetEnumName() == enumName)
				{
					return buttonTypeID;
				}
			}
			return ButtonTypeID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this ButtonTypeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this ButtonTypeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this ButtonTypeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
