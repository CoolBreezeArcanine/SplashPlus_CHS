namespace DB
{
	public static class OptionGametapIDEnum
	{
		private static readonly OptionGametapTableRecord[] records;

		public static bool IsActive(this OptionGametapID self)
		{
			if (self >= OptionGametapID.Default && self < OptionGametapID.End)
			{
				return self != OptionGametapID.Default;
			}
			return false;
		}

		public static bool IsValid(this OptionGametapID self)
		{
			if (self >= OptionGametapID.Default)
			{
				return self < OptionGametapID.End;
			}
			return false;
		}

		public static void Clamp(this OptionGametapID self)
		{
			if (self < OptionGametapID.Default)
			{
				self = OptionGametapID.Default;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionGametapID)GetEnd();
			}
		}

		public static int GetEnd(this OptionGametapID self)
		{
			return GetEnd();
		}

		public static OptionGametapID FindID(string enumName)
		{
			for (OptionGametapID optionGametapID = OptionGametapID.Default; optionGametapID < OptionGametapID.End; optionGametapID++)
			{
				if (optionGametapID.GetEnumName() == enumName)
				{
					return optionGametapID;
				}
			}
			return OptionGametapID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionGametapID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionGametapID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionGametapID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionGametapID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionGametapID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionGametapID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static bool IsDefault(this OptionGametapID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		public static string GetFilePath(this OptionGametapID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		static OptionGametapIDEnum()
		{
			records = new OptionGametapTableRecord[5]
			{
				new OptionGametapTableRecord(0, "Default", "简洁", string.Empty, "標準的なノーツ", string.Empty, 1, "UI_OPT_D_18_01"),
				new OptionGametapTableRecord(1, "Legacy", "经典", string.Empty, string.Empty, string.Empty, 0, "UI_OPT_D_18_02"),
				new OptionGametapTableRecord(2, "Bear", "迪拉熊", string.Empty, "くまノーツ", string.Empty, 0, "UI_OPT_D_18_03"),
				new OptionGametapTableRecord(3, "Bar", "条状", string.Empty, string.Empty, string.Empty, 0, "UI_OPT_D_18_04"),
				new OptionGametapTableRecord(4, "Any", "TAP君", string.Empty, string.Empty, string.Empty, 0, "UI_OPT_D_18_05")
			};
		}
	}
}
