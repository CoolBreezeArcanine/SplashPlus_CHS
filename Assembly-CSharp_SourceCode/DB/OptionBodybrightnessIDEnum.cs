namespace DB
{
	public static class OptionBodybrightnessIDEnum
	{
		private static readonly OptionBodybrightnessTableRecord[] records;

		public static bool IsActive(this OptionBodybrightnessID self)
		{
			if (self >= OptionBodybrightnessID.Bright_0 && self < OptionBodybrightnessID.End)
			{
				return self != OptionBodybrightnessID.Bright_0;
			}
			return false;
		}

		public static bool IsValid(this OptionBodybrightnessID self)
		{
			if (self >= OptionBodybrightnessID.Bright_0)
			{
				return self < OptionBodybrightnessID.End;
			}
			return false;
		}

		public static void Clamp(this OptionBodybrightnessID self)
		{
			if (self < OptionBodybrightnessID.Bright_0)
			{
				self = OptionBodybrightnessID.Bright_0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionBodybrightnessID)GetEnd();
			}
		}

		public static int GetEnd(this OptionBodybrightnessID self)
		{
			return GetEnd();
		}

		public static OptionBodybrightnessID FindID(string enumName)
		{
			for (OptionBodybrightnessID optionBodybrightnessID = OptionBodybrightnessID.Bright_0; optionBodybrightnessID < OptionBodybrightnessID.End; optionBodybrightnessID++)
			{
				if (optionBodybrightnessID.GetEnumName() == enumName)
				{
					return optionBodybrightnessID;
				}
			}
			return OptionBodybrightnessID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionBodybrightnessID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionBodybrightnessID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionBodybrightnessID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetDetail(this OptionBodybrightnessID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetFilePath(this OptionBodybrightnessID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		public static bool IsDefault(this OptionBodybrightnessID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		public static float GetValue(this OptionBodybrightnessID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Value;
			}
			return 0f;
		}

		static OptionBodybrightnessIDEnum()
		{
			records = new OptionBodybrightnessTableRecord[4]
			{
				new OptionBodybrightnessTableRecord(0, "Bright_0", "暗", "调到最暗", "UI_OPT_B_09_01", 0, 0.25f),
				new OptionBodybrightnessTableRecord(1, "Bright_1", "较暗", "调暗", "UI_OPT_B_09_02", 0, 0.5f),
				new OptionBodybrightnessTableRecord(2, "Bright_2", "普通", "稍微调暗", "UI_OPT_B_09_03", 0, 0.75f),
				new OptionBodybrightnessTableRecord(3, "Bright_3", "亮", "通常的亮度", "UI_OPT_B_09_04", 1, 1f)
			};
		}
	}
}
