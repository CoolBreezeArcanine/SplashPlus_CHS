namespace DB
{
	public static class OptionSubmonitorIDEnum
	{
		private static readonly OptionSubmonitorTableRecord[] records = new OptionSubmonitorTableRecord[3]
		{
			new OptionSubmonitorTableRecord(0, "animation_type01", "スクロール", "", "", "", 0, "UI_OPT_B_10_01"),
			new OptionSubmonitorTableRecord(1, "charactor_only", "キャラクター", "", "", "", 0, "UI_OPT_B_10_03"),
			new OptionSubmonitorTableRecord(2, "achievement_only", "楽曲情報・プレイ結果", "", "", "", 1, "UI_OPT_B_10_02")
		};

		public static bool IsActive(this OptionSubmonitorID self)
		{
			if (self >= OptionSubmonitorID.animation_type01 && self < OptionSubmonitorID.End)
			{
				return self != OptionSubmonitorID.animation_type01;
			}
			return false;
		}

		public static bool IsValid(this OptionSubmonitorID self)
		{
			if (self >= OptionSubmonitorID.animation_type01)
			{
				return self < OptionSubmonitorID.End;
			}
			return false;
		}

		public static void Clamp(this OptionSubmonitorID self)
		{
			if (self < OptionSubmonitorID.animation_type01)
			{
				self = OptionSubmonitorID.animation_type01;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionSubmonitorID)GetEnd();
			}
		}

		public static int GetEnd(this OptionSubmonitorID self)
		{
			return GetEnd();
		}

		public static OptionSubmonitorID FindID(string enumName)
		{
			for (OptionSubmonitorID optionSubmonitorID = OptionSubmonitorID.animation_type01; optionSubmonitorID < OptionSubmonitorID.End; optionSubmonitorID++)
			{
				if (optionSubmonitorID.GetEnumName() == enumName)
				{
					return optionSubmonitorID;
				}
			}
			return OptionSubmonitorID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionSubmonitorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionSubmonitorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionSubmonitorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionSubmonitorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionSubmonitorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionSubmonitorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static bool IsDefault(this OptionSubmonitorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		public static string GetFilePath(this OptionSubmonitorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}
	}
}
