namespace DB
{
	public static class OptionMoviebrightnessIDEnum
	{
		private static readonly OptionMoviebrightnessTableRecord[] records;

		public static bool IsActive(this OptionMoviebrightnessID self)
		{
			if (self >= OptionMoviebrightnessID.Bright_0 && self < OptionMoviebrightnessID.End)
			{
				return self != OptionMoviebrightnessID.Bright_0;
			}
			return false;
		}

		public static bool IsValid(this OptionMoviebrightnessID self)
		{
			if (self >= OptionMoviebrightnessID.Bright_0)
			{
				return self < OptionMoviebrightnessID.End;
			}
			return false;
		}

		public static void Clamp(this OptionMoviebrightnessID self)
		{
			if (self < OptionMoviebrightnessID.Bright_0)
			{
				self = OptionMoviebrightnessID.Bright_0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionMoviebrightnessID)GetEnd();
			}
		}

		public static int GetEnd(this OptionMoviebrightnessID self)
		{
			return GetEnd();
		}

		public static OptionMoviebrightnessID FindID(string enumName)
		{
			for (OptionMoviebrightnessID optionMoviebrightnessID = OptionMoviebrightnessID.Bright_0; optionMoviebrightnessID < OptionMoviebrightnessID.End; optionMoviebrightnessID++)
			{
				if (optionMoviebrightnessID.GetEnumName() == enumName)
				{
					return optionMoviebrightnessID;
				}
			}
			return OptionMoviebrightnessID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionMoviebrightnessID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionMoviebrightnessID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static float GetValue(this OptionMoviebrightnessID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Value;
			}
			return 0f;
		}

		public static string GetName(this OptionMoviebrightnessID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionMoviebrightnessID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionMoviebrightnessID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionMoviebrightnessID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this OptionMoviebrightnessID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		public static bool IsDefault(this OptionMoviebrightnessID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		static OptionMoviebrightnessIDEnum()
		{
			records = new OptionMoviebrightnessTableRecord[4]
			{
				new OptionMoviebrightnessTableRecord(0, "Bright_0", 0.75f, "暗", "25%", "调到很暗", string.Empty, "UI_OPT_B_09_01", 0),
				new OptionMoviebrightnessTableRecord(1, "Bright_1", 0.5f, "较暗", "50%", "调暗", string.Empty, "UI_OPT_B_09_02", 0),
				new OptionMoviebrightnessTableRecord(2, "Bright_2", 0.25f, "普通", "75%", "稍微调暗", string.Empty, "UI_OPT_B_09_03", 1),
				new OptionMoviebrightnessTableRecord(3, "Bright_3", 0f, "明亮", "100%", "通常的亮度", string.Empty, "UI_OPT_B_09_04", 0)
			};
		}
	}
}
