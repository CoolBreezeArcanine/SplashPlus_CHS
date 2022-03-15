namespace DB
{
	public static class OptionMatchingIDEnum
	{
		private static readonly OptionMatchingTableRecord[] records = new OptionMatchingTableRecord[2]
		{
			new OptionMatchingTableRecord(0, "Off", "不参加", "", "店内マッチングに参加しません\n募集のインフォメーションも表示しません", "", ""),
			new OptionMatchingTableRecord(1, "On", "参加", "", "店内マッチングに参加します", "", "")
		};

		public static bool IsActive(this OptionMatchingID self)
		{
			if (self >= OptionMatchingID.Off && self < OptionMatchingID.End)
			{
				return self != OptionMatchingID.Off;
			}
			return false;
		}

		public static bool IsValid(this OptionMatchingID self)
		{
			if (self >= OptionMatchingID.Off)
			{
				return self < OptionMatchingID.End;
			}
			return false;
		}

		public static void Clamp(this OptionMatchingID self)
		{
			if (self < OptionMatchingID.Off)
			{
				self = OptionMatchingID.Off;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionMatchingID)GetEnd();
			}
		}

		public static int GetEnd(this OptionMatchingID self)
		{
			return GetEnd();
		}

		public static OptionMatchingID FindID(string enumName)
		{
			for (OptionMatchingID optionMatchingID = OptionMatchingID.Off; optionMatchingID < OptionMatchingID.End; optionMatchingID++)
			{
				if (optionMatchingID.GetEnumName() == enumName)
				{
					return optionMatchingID;
				}
			}
			return OptionMatchingID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionMatchingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionMatchingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionMatchingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionMatchingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionMatchingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionMatchingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this OptionMatchingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}
	}
}
