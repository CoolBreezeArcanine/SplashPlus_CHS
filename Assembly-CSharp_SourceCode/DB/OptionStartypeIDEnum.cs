namespace DB
{
	public static class OptionStartypeIDEnum
	{
		private static readonly OptionStartypeTableRecord[] records;

		public static bool IsActive(this OptionStartypeID self)
		{
			if (self >= OptionStartypeID.Blue && self < OptionStartypeID.End)
			{
				return self != OptionStartypeID.Blue;
			}
			return false;
		}

		public static bool IsValid(this OptionStartypeID self)
		{
			if (self >= OptionStartypeID.Blue)
			{
				return self < OptionStartypeID.End;
			}
			return false;
		}

		public static void Clamp(this OptionStartypeID self)
		{
			if (self < OptionStartypeID.Blue)
			{
				self = OptionStartypeID.Blue;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionStartypeID)GetEnd();
			}
		}

		public static int GetEnd(this OptionStartypeID self)
		{
			return GetEnd();
		}

		public static OptionStartypeID FindID(string enumName)
		{
			for (OptionStartypeID optionStartypeID = OptionStartypeID.Blue; optionStartypeID < OptionStartypeID.End; optionStartypeID++)
			{
				if (optionStartypeID.GetEnumName() == enumName)
				{
					return optionStartypeID;
				}
			}
			return OptionStartypeID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionStartypeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionStartypeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionStartypeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionStartypeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionStartypeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionStartypeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this OptionStartypeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		public static bool IsDefault(this OptionStartypeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		static OptionStartypeIDEnum()
		{
			records = new OptionStartypeTableRecord[2]
			{
				new OptionStartypeTableRecord(0, "Blue", "蓝色", string.Empty, string.Empty, string.Empty, "UI_OPT_D_21_01", 1),
				new OptionStartypeTableRecord(1, "Pink", "粉红色", string.Empty, string.Empty, string.Empty, "UI_OPT_D_21_02", 0)
			};
		}
	}
}
