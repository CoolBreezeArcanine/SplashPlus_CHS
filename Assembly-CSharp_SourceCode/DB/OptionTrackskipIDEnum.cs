namespace DB
{
	public static class OptionTrackskipIDEnum
	{
		private static readonly OptionTrackskipTableRecord[] records;

		public static bool IsActive(this OptionTrackskipID self)
		{
			if (self >= OptionTrackskipID.Off && self < OptionTrackskipID.End)
			{
				return self != OptionTrackskipID.Off;
			}
			return false;
		}

		public static bool IsValid(this OptionTrackskipID self)
		{
			if (self >= OptionTrackskipID.Off)
			{
				return self < OptionTrackskipID.End;
			}
			return false;
		}

		public static void Clamp(this OptionTrackskipID self)
		{
			if (self < OptionTrackskipID.Off)
			{
				self = OptionTrackskipID.Off;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionTrackskipID)GetEnd();
			}
		}

		public static int GetEnd(this OptionTrackskipID self)
		{
			return GetEnd();
		}

		public static OptionTrackskipID FindID(string enumName)
		{
			for (OptionTrackskipID optionTrackskipID = OptionTrackskipID.Off; optionTrackskipID < OptionTrackskipID.End; optionTrackskipID++)
			{
				if (optionTrackskipID.GetEnumName() == enumName)
				{
					return optionTrackskipID;
				}
			}
			return OptionTrackskipID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionTrackskipID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionTrackskipID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionTrackskipID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionTrackskipID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionTrackskipID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionTrackskipID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this OptionTrackskipID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		public static bool IsDefault(this OptionTrackskipID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		static OptionTrackskipIDEnum()
		{
			records = new OptionTrackskipTableRecord[6]
			{
				new OptionTrackskipTableRecord(0, "Off", "OFF", string.Empty, "不使用跳过乐曲", string.Empty, "UI_OPT_B_05_01", 1),
				new OptionTrackskipTableRecord(1, "Push", "长按按键", string.Empty, "通过长按左右的4个按键", string.Empty, "UI_OPT_B_05_02", 0),
				new OptionTrackskipTableRecord(2, "AutoS", "自动/S级", string.Empty, "当在某一时刻失分导致无法\r\n达成S级评价时跳过该乐曲\r\n", string.Empty, "UI_OPT_B_05_03", 0),
				new OptionTrackskipTableRecord(3, "AutoSS", "自动/SS级", string.Empty, "当在某一时刻失分导致无法\r\n达成SS级评价时跳过该乐曲", string.Empty, "UI_OPT_B_05_04", 0),
				new OptionTrackskipTableRecord(4, "AutoSSS", "自动/SSS级", string.Empty, "当在某一时刻失分导致无法\r\n达成SSS级评价时跳过该乐曲", string.Empty, "UI_OPT_B_05_05", 0),
				new OptionTrackskipTableRecord(5, "AutoBest", "自动/最好成绩", string.Empty, "当在某一时刻失分导致无法\r\n达成自己最好成绩时跳过该乐曲", string.Empty, "UI_OPT_B_05_06", 0)
			};
		}
	}
}
