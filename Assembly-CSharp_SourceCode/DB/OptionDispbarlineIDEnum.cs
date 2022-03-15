namespace DB
{
	public static class OptionDispbarlineIDEnum
	{
		private static readonly OptionDispbarlineTableRecord[] records = new OptionDispbarlineTableRecord[2]
		{
			new OptionDispbarlineTableRecord(0, "Off", "OFF", "", "", "", "UI_OPT_B_07_01", 1),
			new OptionDispbarlineTableRecord(1, "On", "ON", "", "", "", "UI_OPT_B_07_02", 0)
		};

		public static bool IsActive(this OptionDispbarlineID self)
		{
			if (self >= OptionDispbarlineID.Off && self < OptionDispbarlineID.End)
			{
				return self != OptionDispbarlineID.Off;
			}
			return false;
		}

		public static bool IsValid(this OptionDispbarlineID self)
		{
			if (self >= OptionDispbarlineID.Off)
			{
				return self < OptionDispbarlineID.End;
			}
			return false;
		}

		public static void Clamp(this OptionDispbarlineID self)
		{
			if (self < OptionDispbarlineID.Off)
			{
				self = OptionDispbarlineID.Off;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionDispbarlineID)GetEnd();
			}
		}

		public static int GetEnd(this OptionDispbarlineID self)
		{
			return GetEnd();
		}

		public static OptionDispbarlineID FindID(string enumName)
		{
			for (OptionDispbarlineID optionDispbarlineID = OptionDispbarlineID.Off; optionDispbarlineID < OptionDispbarlineID.End; optionDispbarlineID++)
			{
				if (optionDispbarlineID.GetEnumName() == enumName)
				{
					return optionDispbarlineID;
				}
			}
			return OptionDispbarlineID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionDispbarlineID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionDispbarlineID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionDispbarlineID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionDispbarlineID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionDispbarlineID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionDispbarlineID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this OptionDispbarlineID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		public static bool IsDefault(this OptionDispbarlineID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}
	}
}
