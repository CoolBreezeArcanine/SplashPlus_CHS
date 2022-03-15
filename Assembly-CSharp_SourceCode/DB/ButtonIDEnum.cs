namespace DB
{
	public static class ButtonIDEnum
	{
		private static readonly ButtonTableRecord[] records = new ButtonTableRecord[1]
		{
			new ButtonTableRecord(0, "NPlus", 0, 0, 0)
		};

		public static bool IsActive(this ButtonID self)
		{
			if (self >= ButtonID.NPlus && self < ButtonID.End)
			{
				return self != ButtonID.NPlus;
			}
			return false;
		}

		public static bool IsValid(this ButtonID self)
		{
			if (self >= ButtonID.NPlus)
			{
				return self < ButtonID.End;
			}
			return false;
		}

		public static void Clamp(this ButtonID self)
		{
			if (self < ButtonID.NPlus)
			{
				self = ButtonID.NPlus;
			}
			else if ((int)self >= GetEnd())
			{
				self = (ButtonID)GetEnd();
			}
		}

		public static int GetEnd(this ButtonID self)
		{
			return GetEnd();
		}

		public static ButtonID FindID(string enumName)
		{
			for (ButtonID buttonID = ButtonID.NPlus; buttonID < ButtonID.End; buttonID++)
			{
				if (buttonID.GetEnumName() == enumName)
				{
					return buttonID;
				}
			}
			return ButtonID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this ButtonID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this ButtonID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static ButtonKindID GetKind(this ButtonID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Kind;
			}
			return ButtonKindID.Invalid;
		}

		public static ButtonTypeID GetType(this ButtonID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Type;
			}
			return ButtonTypeID.Invalid;
		}

		public static int GetName(this ButtonID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return 0;
		}
	}
}
