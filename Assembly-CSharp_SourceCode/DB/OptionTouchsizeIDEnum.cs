namespace DB
{
	public static class OptionTouchsizeIDEnum
	{
		private static readonly OptionTouchsizeTableRecord[] records = new OptionTouchsizeTableRecord[2]
		{
			new OptionTouchsizeTableRecord(0, "Small", "小さい", "", "", "", "", 0, 0.7f),
			new OptionTouchsizeTableRecord(1, "Middle", "ふつう", "", "", "", "", 1, 1f)
		};

		public static bool IsActive(this OptionTouchsizeID self)
		{
			if (self >= OptionTouchsizeID.Small && self < OptionTouchsizeID.End)
			{
				return self != OptionTouchsizeID.Small;
			}
			return false;
		}

		public static bool IsValid(this OptionTouchsizeID self)
		{
			if (self >= OptionTouchsizeID.Small)
			{
				return self < OptionTouchsizeID.End;
			}
			return false;
		}

		public static void Clamp(this OptionTouchsizeID self)
		{
			if (self < OptionTouchsizeID.Small)
			{
				self = OptionTouchsizeID.Small;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionTouchsizeID)GetEnd();
			}
		}

		public static int GetEnd(this OptionTouchsizeID self)
		{
			return GetEnd();
		}

		public static OptionTouchsizeID FindID(string enumName)
		{
			for (OptionTouchsizeID optionTouchsizeID = OptionTouchsizeID.Small; optionTouchsizeID < OptionTouchsizeID.End; optionTouchsizeID++)
			{
				if (optionTouchsizeID.GetEnumName() == enumName)
				{
					return optionTouchsizeID;
				}
			}
			return OptionTouchsizeID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionTouchsizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionTouchsizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionTouchsizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionTouchsizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionTouchsizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionTouchsizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this OptionTouchsizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		public static bool IsDefault(this OptionTouchsizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		public static float GetValue(this OptionTouchsizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Value;
			}
			return 0f;
		}
	}
}
