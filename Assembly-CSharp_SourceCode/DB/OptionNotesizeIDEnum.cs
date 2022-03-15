namespace DB
{
	public static class OptionNotesizeIDEnum
	{
		private static readonly OptionNotesizeTableRecord[] records = new OptionNotesizeTableRecord[3]
		{
			new OptionNotesizeTableRecord(0, "Small", "小さい", "", "", "", "", 0, 0.7f),
			new OptionNotesizeTableRecord(1, "Middle", "ふつう", "", "", "", "", 1, 1f),
			new OptionNotesizeTableRecord(2, "Big", "大きい", "", "", "", "", 0, 1.3f)
		};

		public static bool IsActive(this OptionNotesizeID self)
		{
			if (self >= OptionNotesizeID.Small && self < OptionNotesizeID.End)
			{
				return self != OptionNotesizeID.Small;
			}
			return false;
		}

		public static bool IsValid(this OptionNotesizeID self)
		{
			if (self >= OptionNotesizeID.Small)
			{
				return self < OptionNotesizeID.End;
			}
			return false;
		}

		public static void Clamp(this OptionNotesizeID self)
		{
			if (self < OptionNotesizeID.Small)
			{
				self = OptionNotesizeID.Small;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionNotesizeID)GetEnd();
			}
		}

		public static int GetEnd(this OptionNotesizeID self)
		{
			return GetEnd();
		}

		public static OptionNotesizeID FindID(string enumName)
		{
			for (OptionNotesizeID optionNotesizeID = OptionNotesizeID.Small; optionNotesizeID < OptionNotesizeID.End; optionNotesizeID++)
			{
				if (optionNotesizeID.GetEnumName() == enumName)
				{
					return optionNotesizeID;
				}
			}
			return OptionNotesizeID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionNotesizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionNotesizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionNotesizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionNotesizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionNotesizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionNotesizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this OptionNotesizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		public static bool IsDefault(this OptionNotesizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		public static float GetValue(this OptionNotesizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Value;
			}
			return 0f;
		}
	}
}
