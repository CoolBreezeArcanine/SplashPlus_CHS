namespace DB
{
	public static class SortTabIDEnum
	{
		private static readonly SortTabTableRecord[] records;

		public static bool IsActive(this SortTabID self)
		{
			if (self >= SortTabID.Genre && self < SortTabID.End)
			{
				return self != SortTabID.Genre;
			}
			return false;
		}

		public static bool IsValid(this SortTabID self)
		{
			if (self >= SortTabID.Genre)
			{
				return self < SortTabID.End;
			}
			return false;
		}

		public static void Clamp(this SortTabID self)
		{
			if (self < SortTabID.Genre)
			{
				self = SortTabID.Genre;
			}
			else if ((int)self >= GetEnd())
			{
				self = (SortTabID)GetEnd();
			}
		}

		public static int GetEnd(this SortTabID self)
		{
			return GetEnd();
		}

		public static SortTabID FindID(string enumName)
		{
			for (SortTabID sortTabID = SortTabID.Genre; sortTabID < SortTabID.End; sortTabID++)
			{
				if (sortTabID.GetEnumName() == enumName)
				{
					return sortTabID;
				}
			}
			return SortTabID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this SortTabID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this SortTabID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this SortTabID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this SortTabID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this SortTabID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this SortTabID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this SortTabID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		static SortTabIDEnum()
		{
			records = new SortTabTableRecord[5]
			{
				new SortTabTableRecord(0, "Genre", "??????", string.Empty, "????????????????????????????????????", string.Empty, "UI_MSS_Tabimage_01_01"),
				new SortTabTableRecord(1, "All", "????????????", string.Empty, "???????????????????????????????????????", string.Empty, "UI_MSS_Tabimage_01_02"),
				new SortTabTableRecord(2, "Version", "??????", string.Empty, "???????????????????????????", string.Empty, "UI_MSS_Tabimage_01_05"),
				new SortTabTableRecord(3, "Level", "??????", string.Empty, "???????????????????????????", string.Empty, "UI_MSS_Tabimage_01_03"),
				new SortTabTableRecord(4, "Name", "?????????", string.Empty, "??????????????????????????????????????????????????????", string.Empty, "UI_MSS_Tabimage_01_04")
			};
		}
	}
}
