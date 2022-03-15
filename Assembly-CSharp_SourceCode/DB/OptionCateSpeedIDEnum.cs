namespace DB
{
	public static class OptionCateSpeedIDEnum
	{
		private static readonly OptionCateSpeedTableRecord[] records;

		public static bool IsActive(this OptionCateSpeedID self)
		{
			if (self >= OptionCateSpeedID.NoteSpeed && self < OptionCateSpeedID.End)
			{
				return self != OptionCateSpeedID.NoteSpeed;
			}
			return false;
		}

		public static bool IsValid(this OptionCateSpeedID self)
		{
			if (self >= OptionCateSpeedID.NoteSpeed)
			{
				return self < OptionCateSpeedID.End;
			}
			return false;
		}

		public static void Clamp(this OptionCateSpeedID self)
		{
			if (self < OptionCateSpeedID.NoteSpeed)
			{
				self = OptionCateSpeedID.NoteSpeed;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionCateSpeedID)GetEnd();
			}
		}

		public static int GetEnd(this OptionCateSpeedID self)
		{
			return GetEnd();
		}

		public static OptionCateSpeedID FindID(string enumName)
		{
			for (OptionCateSpeedID optionCateSpeedID = OptionCateSpeedID.NoteSpeed; optionCateSpeedID < OptionCateSpeedID.End; optionCateSpeedID++)
			{
				if (optionCateSpeedID.GetEnumName() == enumName)
				{
					return optionCateSpeedID;
				}
			}
			return OptionCateSpeedID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionCateSpeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionCateSpeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionCateSpeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionCateSpeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionCateSpeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionCateSpeedID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		static OptionCateSpeedIDEnum()
		{
			records = new OptionCateSpeedTableRecord[3]
			{
				new OptionCateSpeedTableRecord(0, "NoteSpeed", "TAP速度", string.Empty, "调整音符圈的移动速度", string.Empty),
				new OptionCateSpeedTableRecord(1, "TouchSpeed", "TOUCH速度", string.Empty, "调整在触摸之前的显示速度", string.Empty),
				new OptionCateSpeedTableRecord(2, "SlideSpeed", "SLIDE显示时机", string.Empty, "调整提前显示的出现时间", string.Empty)
			};
		}
	}
}
