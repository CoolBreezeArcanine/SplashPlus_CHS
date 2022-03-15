namespace DB
{
	public static class OptionVolumeIDEnum
	{
		private static readonly OptionVolumeTableRecord[] records = new OptionVolumeTableRecord[6]
		{
			new OptionVolumeTableRecord(0, "Mute", 0f, "なし", "Mute", "", "", "UI_OPT_E_23_01", 0),
			new OptionVolumeTableRecord(1, "Vol1", 0.2f, "1", "1", "", "", "UI_OPT_E_23_02", 0),
			new OptionVolumeTableRecord(2, "Vol2", 0.4f, "2", "2", "", "", "UI_OPT_E_23_03", 0),
			new OptionVolumeTableRecord(3, "Vol3", 0.6f, "3", "3", "", "", "UI_OPT_E_23_04", 0),
			new OptionVolumeTableRecord(4, "Vol4", 0.8f, "4", "4", "", "", "UI_OPT_E_23_05", 0),
			new OptionVolumeTableRecord(5, "Vol5", 1f, "5", "5", "", "", "UI_OPT_E_23_06", 1)
		};

		public static bool IsActive(this OptionVolumeID self)
		{
			if (self >= OptionVolumeID.Mute && self < OptionVolumeID.End)
			{
				return self != OptionVolumeID.Mute;
			}
			return false;
		}

		public static bool IsValid(this OptionVolumeID self)
		{
			if (self >= OptionVolumeID.Mute)
			{
				return self < OptionVolumeID.End;
			}
			return false;
		}

		public static void Clamp(this OptionVolumeID self)
		{
			if (self < OptionVolumeID.Mute)
			{
				self = OptionVolumeID.Mute;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionVolumeID)GetEnd();
			}
		}

		public static int GetEnd(this OptionVolumeID self)
		{
			return GetEnd();
		}

		public static OptionVolumeID FindID(string enumName)
		{
			for (OptionVolumeID optionVolumeID = OptionVolumeID.Mute; optionVolumeID < OptionVolumeID.End; optionVolumeID++)
			{
				if (optionVolumeID.GetEnumName() == enumName)
				{
					return optionVolumeID;
				}
			}
			return OptionVolumeID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionVolumeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionVolumeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static float GetValue(this OptionVolumeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Value;
			}
			return 0f;
		}

		public static string GetName(this OptionVolumeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionVolumeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionVolumeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionVolumeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this OptionVolumeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		public static bool IsDefault(this OptionVolumeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}
	}
}
