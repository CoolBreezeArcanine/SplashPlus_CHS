namespace DB
{
	public static class OptionToucheffectIDEnum
	{
		private static readonly OptionToucheffectTableRecord[] records;

		public static bool IsActive(this OptionToucheffectID self)
		{
			if (self >= OptionToucheffectID.Off && self < OptionToucheffectID.End)
			{
				return self != OptionToucheffectID.Off;
			}
			return false;
		}

		public static bool IsValid(this OptionToucheffectID self)
		{
			if (self >= OptionToucheffectID.Off)
			{
				return self < OptionToucheffectID.End;
			}
			return false;
		}

		public static void Clamp(this OptionToucheffectID self)
		{
			if (self < OptionToucheffectID.Off)
			{
				self = OptionToucheffectID.Off;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionToucheffectID)GetEnd();
			}
		}

		public static int GetEnd(this OptionToucheffectID self)
		{
			return GetEnd();
		}

		public static OptionToucheffectID FindID(string enumName)
		{
			for (OptionToucheffectID optionToucheffectID = OptionToucheffectID.Off; optionToucheffectID < OptionToucheffectID.End; optionToucheffectID++)
			{
				if (optionToucheffectID.GetEnumName() == enumName)
				{
					return optionToucheffectID;
				}
			}
			return OptionToucheffectID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionToucheffectID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionToucheffectID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionToucheffectID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionToucheffectID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionToucheffectID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionToucheffectID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this OptionToucheffectID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		public static bool IsDefault(this OptionToucheffectID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		static OptionToucheffectIDEnum()
		{
			records = new OptionToucheffectTableRecord[3]
			{
				new OptionToucheffectTableRecord(0, "Off", "OFF", string.Empty, string.Empty, string.Empty, "UI_OPT_B_10_01", 0),
				new OptionToucheffectTableRecord(1, "Outline", "只在外圈显示", string.Empty, string.Empty, string.Empty, "UI_OPT_B_10_02", 0),
				new OptionToucheffectTableRecord(2, "On", "ON", string.Empty, string.Empty, string.Empty, "UI_OPT_B_10_03", 1)
			};
		}
	}
}
