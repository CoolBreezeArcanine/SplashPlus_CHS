namespace DB
{
	public static class OptionSlideseIDEnum
	{
		private static readonly OptionSlideseTableRecord[] records = new OptionSlideseTableRecord[31]
		{
			new OptionSlideseTableRecord(0, "Se1", "デフォルト", "DEFAULT", "", "", "SE_GAME_SLIDE_000001", 1, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(1, "Se2", "イヌ", "DOG", "", "", "SE_GAME_SLIDE_000002", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(2, "Se3", "ネコ", "CAT", "", "", "SE_GAME_SLIDE_000003", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(3, "Se4", "刀", "KATANA", "", "", "SE_GAME_SLIDE_000004", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(4, "Se5", "ソニック（ダッシュ）", "SONIC(DASH)", "", "", "SE_GAME_SLIDE_000005", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(5, "Se6", "ソニック（ゲート）", "SONIC(GATE)", "", "", "SE_GAME_SLIDE_000006", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(6, "Se7", "PSO SOUND", "PSO SOUND", "", "", "SE_GAME_SLIDE_000007", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(7, "Se8", "MI・TSU・YO・SHI", "MI･TSU･YO･SHI", "", "", "SE_GAME_SLIDE_000008", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(8, "Se9", "はっぴー", "Happy", "", "", "SE_GAME_SLIDE_000009", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(9, "Se10", "タンバリン", "Tambourine", "", "", "SE_GAME_SLIDE_000010", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(10, "Se11", "DARTSLIVE", "DARTSLIVE", "", "", "SE_GAME_SLIDE_000011", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(11, "Se12", "カブキ", "KABUKI", "", "", "SE_GAME_SLIDE_000012", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(12, "Se13", "SEGA", "SEGA", "", "", "SE_GAME_SLIDE_000013", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(13, "Se14", "ぷよぷよ", "PUYOPUYO", "", "", "SE_GAME_SLIDE_000014", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(14, "Se15", "DIVA SOUND A", "DIVA SOUND A", "", "", "SE_GAME_SLIDE_000015", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(15, "Se16", "maimai SLIDE", "maimai SLIDE", "", "", "SE_GAME_SLIDE_000016", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(16, "Se17", "CHUNITHM", "CHUNITHM", "", "", "SE_GAME_SLIDE_000017", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(17, "Se18", "オンゲキ", "ONGEKI", "", "", "SE_GAME_SLIDE_000018", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(18, "Se19", "ラズ1", "Ras1", "", "", "SE_GAME_SLIDE_000019", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(19, "Se20", "シフォン1", "Chiffon1", "", "", "SE_GAME_SLIDE_000020", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(20, "Se21", "ソルト1", "Salt1", "", "", "SE_GAME_SLIDE_000021", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(21, "Se22", "しゃま1", "Shama1", "", "", "SE_GAME_SLIDE_000027", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(22, "Se23", "みるく1", "Milk1", "", "", "SE_GAME_SLIDE_000029", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(23, "Se24", "OTOHIME", "OTOHIME", "", "", "SE_GAME_SLIDE_000022", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(24, "Se25", "ラズ2", "Ras2", "", "", "SE_GAME_SLIDE_000023", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(25, "Se26", "シフォン2", "Chiffon2", "", "", "SE_GAME_SLIDE_000024", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(26, "Se27", "ソルト2", "Salt2", "", "", "SE_GAME_SLIDE_000025", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(27, "Se28", "しゃま2", "Shama2", "", "", "SE_GAME_SLIDE_000028", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(28, "Se29", "みるく2", "Milk2", "", "", "SE_GAME_SLIDE_000030", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(29, "Se30", "でらっくま", "Delukkuma", "", "", "SE_GAME_SLIDE_000026", 0, "UI_OPT_E_24_01"),
			new OptionSlideseTableRecord(30, "Se31", "らいむっくま＆れもんっくま", "", "", "", "SE_GAME_SLIDE_000031", 0, "UI_OPT_E_24_01")
		};

		public static bool IsActive(this OptionSlideseID self)
		{
			if (self >= OptionSlideseID.Se1 && self < OptionSlideseID.End)
			{
				return self != OptionSlideseID.Se1;
			}
			return false;
		}

		public static bool IsValid(this OptionSlideseID self)
		{
			if (self >= OptionSlideseID.Se1)
			{
				return self < OptionSlideseID.End;
			}
			return false;
		}

		public static void Clamp(this OptionSlideseID self)
		{
			if (self < OptionSlideseID.Se1)
			{
				self = OptionSlideseID.Se1;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionSlideseID)GetEnd();
			}
		}

		public static int GetEnd(this OptionSlideseID self)
		{
			return GetEnd();
		}

		public static OptionSlideseID FindID(string enumName)
		{
			for (OptionSlideseID optionSlideseID = OptionSlideseID.Se1; optionSlideseID < OptionSlideseID.End; optionSlideseID++)
			{
				if (optionSlideseID.GetEnumName() == enumName)
				{
					return optionSlideseID;
				}
			}
			return OptionSlideseID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionSlideseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionSlideseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionSlideseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionSlideseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionSlideseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionSlideseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetSeEnum(this OptionSlideseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].SeEnum;
			}
			return "";
		}

		public static bool IsDefault(this OptionSlideseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		public static string GetFilePath(this OptionSlideseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}
	}
}
