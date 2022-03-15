namespace DB
{
	public static class OptionCateDesignIDEnum
	{
		private static readonly OptionCateDesignTableRecord[] records;

		public static bool IsActive(this OptionCateDesignID self)
		{
			if (self >= OptionCateDesignID.TapDesign && self < OptionCateDesignID.End)
			{
				return self != OptionCateDesignID.TapDesign;
			}
			return false;
		}

		public static bool IsValid(this OptionCateDesignID self)
		{
			if (self >= OptionCateDesignID.TapDesign)
			{
				return self < OptionCateDesignID.End;
			}
			return false;
		}

		public static void Clamp(this OptionCateDesignID self)
		{
			if (self < OptionCateDesignID.TapDesign)
			{
				self = OptionCateDesignID.TapDesign;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionCateDesignID)GetEnd();
			}
		}

		public static int GetEnd(this OptionCateDesignID self)
		{
			return GetEnd();
		}

		public static OptionCateDesignID FindID(string enumName)
		{
			for (OptionCateDesignID optionCateDesignID = OptionCateDesignID.TapDesign; optionCateDesignID < OptionCateDesignID.End; optionCateDesignID++)
			{
				if (optionCateDesignID.GetEnumName() == enumName)
				{
					return optionCateDesignID;
				}
			}
			return OptionCateDesignID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionCateDesignID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionCateDesignID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionCateDesignID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionCateDesignID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionCateDesignID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionCateDesignID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		static OptionCateDesignIDEnum()
		{
			records = new OptionCateDesignTableRecord[5]
			{
				new OptionCateDesignTableRecord(0, "TapDesign", "TAP音符款式", string.Empty, "改变TAP音符的款式", string.Empty),
				new OptionCateDesignTableRecord(1, "HoldDesign", "HOLD音符款式", string.Empty, "改变HOLD音符款式", string.Empty),
				new OptionCateDesignTableRecord(2, "SlideDesign", "SLIDE音符款式", string.Empty, "改变SLIDE音符款式", string.Empty),
				new OptionCateDesignTableRecord(3, "StarDesign", "SLIDE音符的颜色", string.Empty, "☆的颜色变更（拖尾部分不会变）", string.Empty),
				new OptionCateDesignTableRecord(4, "OutlineDesign", "判定线款式", string.Empty, "改变判定线的款式", string.Empty)
			};
		}
	}
}
