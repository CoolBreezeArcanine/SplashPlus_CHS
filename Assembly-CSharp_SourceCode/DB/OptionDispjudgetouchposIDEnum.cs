namespace DB
{
	public static class OptionDispjudgetouchposIDEnum
	{
		private static readonly OptionDispjudgetouchposTableRecord[] records;

		public static bool IsActive(this OptionDispjudgetouchposID self)
		{
			if (self >= OptionDispjudgetouchposID.Off && self < OptionDispjudgetouchposID.End)
			{
				return self != OptionDispjudgetouchposID.Off;
			}
			return false;
		}

		public static bool IsValid(this OptionDispjudgetouchposID self)
		{
			if (self >= OptionDispjudgetouchposID.Off)
			{
				return self < OptionDispjudgetouchposID.End;
			}
			return false;
		}

		public static void Clamp(this OptionDispjudgetouchposID self)
		{
			if (self < OptionDispjudgetouchposID.Off)
			{
				self = OptionDispjudgetouchposID.Off;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionDispjudgetouchposID)GetEnd();
			}
		}

		public static int GetEnd(this OptionDispjudgetouchposID self)
		{
			return GetEnd();
		}

		public static OptionDispjudgetouchposID FindID(string enumName)
		{
			for (OptionDispjudgetouchposID optionDispjudgetouchposID = OptionDispjudgetouchposID.Off; optionDispjudgetouchposID < OptionDispjudgetouchposID.End; optionDispjudgetouchposID++)
			{
				if (optionDispjudgetouchposID.GetEnumName() == enumName)
				{
					return optionDispjudgetouchposID;
				}
			}
			return OptionDispjudgetouchposID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionDispjudgetouchposID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionDispjudgetouchposID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionDispjudgetouchposID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionDispjudgetouchposID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionDispjudgetouchposID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionDispjudgetouchposID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this OptionDispjudgetouchposID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		public static bool IsDefault(this OptionDispjudgetouchposID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		static OptionDispjudgetouchposIDEnum()
		{
			records = new OptionDispjudgetouchposTableRecord[3]
			{
				new OptionDispjudgetouchposTableRecord(0, "Off", "OFF", string.Empty, string.Empty, string.Empty, "UI_OPT_C_17_01", 0),
				new OptionDispjudgetouchposTableRecord(1, "In", "内侧", string.Empty, string.Empty, string.Empty, "UI_OPT_C_17_02", 1),
				new OptionDispjudgetouchposTableRecord(2, "Out", "外侧", string.Empty, string.Empty, string.Empty, "UI_OPT_C_17_03", 0)
			};
		}
	}
}
