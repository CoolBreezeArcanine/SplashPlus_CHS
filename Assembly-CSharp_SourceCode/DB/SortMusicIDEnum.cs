namespace DB
{
	public static class SortMusicIDEnum
	{
		private static readonly SortMusicTableRecord[] records;

		public static bool IsActive(this SortMusicID self)
		{
			if (self >= SortMusicID.ID && self < SortMusicID.End)
			{
				return self != SortMusicID.ID;
			}
			return false;
		}

		public static bool IsValid(this SortMusicID self)
		{
			if (self >= SortMusicID.ID)
			{
				return self < SortMusicID.End;
			}
			return false;
		}

		public static void Clamp(this SortMusicID self)
		{
			if (self < SortMusicID.ID)
			{
				self = SortMusicID.ID;
			}
			else if ((int)self >= GetEnd())
			{
				self = (SortMusicID)GetEnd();
			}
		}

		public static int GetEnd(this SortMusicID self)
		{
			return GetEnd();
		}

		public static SortMusicID FindID(string enumName)
		{
			for (SortMusicID sortMusicID = SortMusicID.ID; sortMusicID < SortMusicID.End; sortMusicID++)
			{
				if (sortMusicID.GetEnumName() == enumName)
				{
					return sortMusicID;
				}
			}
			return SortMusicID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this SortMusicID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this SortMusicID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this SortMusicID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this SortMusicID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this SortMusicID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this SortMusicID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetFilePath(this SortMusicID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}

		static SortMusicIDEnum()
		{
			records = new SortMusicTableRecord[6]
			{
				new SortMusicTableRecord(0, "ID", "??????????????????", string.Empty, "?????????????????????????????????", string.Empty, "UI_MSS_Sortimage_01_01"),
				new SortMusicTableRecord(1, "Level", "???????????????", string.Empty, "???????????????????????????", string.Empty, "UI_MSS_Sortimage_01_04"),
				new SortMusicTableRecord(2, "Rank", "???????????????", string.Empty, "???????????????????????????", string.Empty, "UI_MSS_Sortimage_01_02"),
				new SortMusicTableRecord(3, "ApFc", "???AP/FC????????????", string.Empty, "?????????FC???GFC???AP???AP+???????????????", string.Empty, "UI_MSS_Sortimage_01_05"),
				new SortMusicTableRecord(4, "Sync", "??????????????????", string.Empty, "??????????????????????????????99%???100%???????????????", string.Empty, "UI_MSS_Sortimage_01_03"),
				new SortMusicTableRecord(5, "Name", "??????????????????", string.Empty, "???????????????????????????????????????????????????", string.Empty, "UI_MSS_Sortimage_01_06")
			};
		}
	}
}
