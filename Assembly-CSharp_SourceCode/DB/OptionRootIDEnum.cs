namespace DB
{
	public static class OptionRootIDEnum
	{
		private static readonly OptionRootTableRecord[] records = new OptionRootTableRecord[2]
		{
			new OptionRootTableRecord(0, "DefaultColorTag", "<#ffffff>"),
			new OptionRootTableRecord(1, "NormalColorTag", "<#02f6ff>")
		};

		public static bool IsActive(this OptionRootID self)
		{
			if (self >= OptionRootID.DefaultColorTag && self < OptionRootID.End)
			{
				return self != OptionRootID.DefaultColorTag;
			}
			return false;
		}

		public static bool IsValid(this OptionRootID self)
		{
			if (self >= OptionRootID.DefaultColorTag)
			{
				return self < OptionRootID.End;
			}
			return false;
		}

		public static void Clamp(this OptionRootID self)
		{
			if (self < OptionRootID.DefaultColorTag)
			{
				self = OptionRootID.DefaultColorTag;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionRootID)GetEnd();
			}
		}

		public static int GetEnd(this OptionRootID self)
		{
			return GetEnd();
		}

		public static OptionRootID FindID(string enumName)
		{
			for (OptionRootID optionRootID = OptionRootID.DefaultColorTag; optionRootID < OptionRootID.End; optionRootID++)
			{
				if (optionRootID.GetEnumName() == enumName)
				{
					return optionRootID;
				}
			}
			return OptionRootID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionRootID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionRootID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionRootID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
