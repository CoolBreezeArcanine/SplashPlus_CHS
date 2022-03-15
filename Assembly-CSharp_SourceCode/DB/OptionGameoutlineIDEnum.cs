namespace DB
{
	public static class OptionGameoutlineIDEnum
	{
		private static readonly OptionGameoutlineTableRecord[] records;

		public static bool IsActive(this OptionGameoutlineID self)
		{
			if (self >= OptionGameoutlineID.Hide && self < OptionGameoutlineID.End)
			{
				return self != OptionGameoutlineID.Hide;
			}
			return false;
		}

		public static bool IsValid(this OptionGameoutlineID self)
		{
			if (self >= OptionGameoutlineID.Hide)
			{
				return self < OptionGameoutlineID.End;
			}
			return false;
		}

		public static void Clamp(this OptionGameoutlineID self)
		{
			if (self < OptionGameoutlineID.Hide)
			{
				self = OptionGameoutlineID.Hide;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionGameoutlineID)GetEnd();
			}
		}

		public static int GetEnd(this OptionGameoutlineID self)
		{
			return GetEnd();
		}

		public static OptionGameoutlineID FindID(string enumName)
		{
			for (OptionGameoutlineID optionGameoutlineID = OptionGameoutlineID.Hide; optionGameoutlineID < OptionGameoutlineID.End; optionGameoutlineID++)
			{
				if (optionGameoutlineID.GetEnumName() == enumName)
				{
					return optionGameoutlineID;
				}
			}
			return OptionGameoutlineID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionGameoutlineID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionGameoutlineID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionGameoutlineID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionGameoutlineID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionGameoutlineID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionGameoutlineID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this OptionGameoutlineID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		public static bool IsDefault(this OptionGameoutlineID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		static OptionGameoutlineIDEnum()
		{
			records = new OptionGameoutlineTableRecord[13]
			{
				new OptionGameoutlineTableRecord(0, "Hide", "隐藏", "", "", "", "UI_OPT_D_22_01", 0),
				new OptionGameoutlineTableRecord(1, "Dot", "点", "", "", "", "UI_OPT_D_22_02", 0),
				new OptionGameoutlineTableRecord(2, "Simple", "简洁", "", "", "", "UI_OPT_D_22_03", 0),
				new OptionGameoutlineTableRecord(3, "Sensor", "感应器", "", "", "", "UI_OPT_D_22_04", 0),
				new OptionGameoutlineTableRecord(4, "maimai", "maimai", "", "", "", "UI_OPT_D_22_05", 0),
				new OptionGameoutlineTableRecord(5, "GreeN", "GreeN", "", "", "", "UI_OPT_D_22_06", 0),
				new OptionGameoutlineTableRecord(6, "ORANGE", "ORANGE", "", "", "", "UI_OPT_D_22_07", 0),
				new OptionGameoutlineTableRecord(7, "PiNK", "PiNK", "", "", "", "UI_OPT_D_22_08", 0),
				new OptionGameoutlineTableRecord(8, "MURASAKi", "MURASAKi", "", "", "", "UI_OPT_D_22_09", 0),
				new OptionGameoutlineTableRecord(9, "MiLK", "MiLK", "", "", "", "UI_OPT_D_22_10", 0),
				new OptionGameoutlineTableRecord(10, "FiNALE", "FiNALE", "", "", "", "UI_OPT_D_22_11", 0),
				new OptionGameoutlineTableRecord(11, "DX", "DX", "", "", "", "UI_OPT_D_22_12", 0),
				new OptionGameoutlineTableRecord(12, "Splash", "Splash", "", "", "", "UI_OPT_D_22_13", 1)
			};
		}
	}
}
