namespace DB
{
	public static class OptionSlidesizeIDEnum
	{
		private static readonly OptionSlidesizeTableRecord[] records = new OptionSlidesizeTableRecord[3]
		{
			new OptionSlidesizeTableRecord(0, "Small", "細い", "", "", "", "", 0, 0.7f),
			new OptionSlidesizeTableRecord(1, "Middle", "ふつう", "", "", "", "", 1, 1f),
			new OptionSlidesizeTableRecord(2, "Big", "太い", "", "", "", "", 0, 1.3f)
		};

		public static bool IsActive(this OptionSlidesizeID self)
		{
			if (self >= OptionSlidesizeID.Small && self < OptionSlidesizeID.End)
			{
				return self != OptionSlidesizeID.Small;
			}
			return false;
		}

		public static bool IsValid(this OptionSlidesizeID self)
		{
			if (self >= OptionSlidesizeID.Small)
			{
				return self < OptionSlidesizeID.End;
			}
			return false;
		}

		public static void Clamp(this OptionSlidesizeID self)
		{
			if (self < OptionSlidesizeID.Small)
			{
				self = OptionSlidesizeID.Small;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionSlidesizeID)GetEnd();
			}
		}

		public static int GetEnd(this OptionSlidesizeID self)
		{
			return GetEnd();
		}

		public static OptionSlidesizeID FindID(string enumName)
		{
			for (OptionSlidesizeID optionSlidesizeID = OptionSlidesizeID.Small; optionSlidesizeID < OptionSlidesizeID.End; optionSlidesizeID++)
			{
				if (optionSlidesizeID.GetEnumName() == enumName)
				{
					return optionSlidesizeID;
				}
			}
			return OptionSlidesizeID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionSlidesizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionSlidesizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionSlidesizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionSlidesizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionSlidesizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionSlidesizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this OptionSlidesizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		public static bool IsDefault(this OptionSlidesizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		public static float GetValue(this OptionSlidesizeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Value;
			}
			return 0f;
		}
	}
}
