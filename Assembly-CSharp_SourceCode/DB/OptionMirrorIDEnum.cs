namespace DB
{
	public static class OptionMirrorIDEnum
	{
		private static readonly OptionMirrorTableRecord[] records;

		public static bool IsActive(this OptionMirrorID self)
		{
			if (self >= OptionMirrorID.Normal && self < OptionMirrorID.End)
			{
				return self != OptionMirrorID.Normal;
			}
			return false;
		}

		public static bool IsValid(this OptionMirrorID self)
		{
			if (self >= OptionMirrorID.Normal)
			{
				return self < OptionMirrorID.End;
			}
			return false;
		}

		public static void Clamp(this OptionMirrorID self)
		{
			if (self < OptionMirrorID.Normal)
			{
				self = OptionMirrorID.Normal;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionMirrorID)GetEnd();
			}
		}

		public static int GetEnd(this OptionMirrorID self)
		{
			return GetEnd();
		}

		public static OptionMirrorID FindID(string enumName)
		{
			for (OptionMirrorID optionMirrorID = OptionMirrorID.Normal; optionMirrorID < OptionMirrorID.End; optionMirrorID++)
			{
				if (optionMirrorID.GetEnumName() == enumName)
				{
					return optionMirrorID;
				}
			}
			return OptionMirrorID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionMirrorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionMirrorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionMirrorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionMirrorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionMirrorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionMirrorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this OptionMirrorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		public static bool IsDefault(this OptionMirrorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		static OptionMirrorIDEnum()
		{
			records = new OptionMirrorTableRecord[4]
			{
				new OptionMirrorTableRecord(0, "Normal", "OFF", string.Empty, "默认配置", string.Empty, "UI_OPT_B_06_01", 1),
				new OptionMirrorTableRecord(1, "LR", "⇄", string.Empty, "左右颠倒", string.Empty, "UI_OPT_B_06_02", 0),
				new OptionMirrorTableRecord(2, "UD", "⇅", string.Empty, "上下颠倒", string.Empty, "UI_OPT_B_06_04", 0),
				new OptionMirrorTableRecord(3, "UDLR", "↻", string.Empty, "旋转180°", string.Empty, "UI_OPT_B_06_03", 0)
			};
		}
	}
}
