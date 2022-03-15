namespace DB
{
	public static class OptionBreakseIDEnum
	{
		private static readonly OptionBreakseTableRecord[] records = new OptionBreakseTableRecord[31]
		{
			new OptionBreakseTableRecord(0, "Se1", "デフォルト", "DEFAULT", "", "", "SE_GAME_BREAK_01_000001", "SE_GAME_BREAK_02_000001", 1, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(1, "Se2", "イヌ", "DOG", "", "", "SE_GAME_BREAK_01_000002", "SE_GAME_BREAK_02_000002", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(2, "Se3", "ネコ", "CAT", "", "", "SE_GAME_BREAK_01_000003", "SE_GAME_BREAK_02_000003", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(3, "Se4", "刀", "KATANA", "", "", "SE_GAME_BREAK_01_000004", "SE_GAME_BREAK_02_000004", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(4, "Se5", "ソニック（リング）", "SONIC(RING)", "", "", "SE_GAME_BREAK_01_000005", "SE_GAME_BREAK_02_000005", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(5, "Se6", "ソニック（エメラルド）", "SONIC（EMERALD）", "", "", "SE_GAME_BREAK_01_000006", "SE_GAME_BREAK_02_000006", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(6, "Se7", "PSO SOUND", "PSO SOUND", "", "", "SE_GAME_BREAK_01_000007", "SE_GAME_BREAK_02_000007", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(7, "Se8", "MI・TSU・YO・SHI", "MI･TSU･YO･SHI", "", "", "SE_GAME_BREAK_01_000008", "SE_GAME_BREAK_02_000008", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(8, "Se9", "はっぴー", "Happy", "", "", "SE_GAME_BREAK_01_000009", "SE_GAME_BREAK_02_000009", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(9, "Se10", "タンバリン", "Tambourine", "", "", "SE_GAME_BREAK_01_000010", "SE_GAME_BREAK_02_000010", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(10, "Se11", "DARTSLIVE", "DARTSLIVE", "", "", "SE_GAME_BREAK_01_000011", "SE_GAME_BREAK_02_000011", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(11, "Se12", "カブキ", "KABUKI", "", "", "SE_GAME_BREAK_01_000012", "SE_GAME_BREAK_02_000012", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(12, "Se13", "SEGA", "SEGA", "", "", "SE_GAME_BREAK_01_000013", "SE_GAME_BREAK_02_000013", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(13, "Se14", "ぷよぷよ", "PUYOPUYO", "", "", "SE_GAME_BREAK_01_000014", "SE_GAME_BREAK_02_000014", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(14, "Se15", "DIVA SOUND A", "DIVA SOUND A", "", "", "SE_GAME_BREAK_01_000015", "SE_GAME_BREAK_02_000015", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(15, "Se16", "maimai BREAK", "maimai BREAK", "", "", "SE_GAME_BREAK_01_000016", "SE_GAME_BREAK_02_000016", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(16, "Se17", "CHUNITHM", "CHUNITHM", "", "", "SE_GAME_BREAK_01_000017", "SE_GAME_BREAK_02_000017", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(17, "Se18", "オンゲキ", "ONGEKI", "", "", "SE_GAME_BREAK_01_000018", "SE_GAME_BREAK_02_000018", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(18, "Se19", "ラズ1", "Ras1", "", "", "SE_GAME_BREAK_01_000019", "SE_GAME_BREAK_02_000019", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(19, "Se20", "シフォン1", "Chiffon1", "", "", "SE_GAME_BREAK_01_000020", "SE_GAME_BREAK_02_000020", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(20, "Se21", "ソルト1", "Salt1", "", "", "SE_GAME_BREAK_01_000021", "SE_GAME_BREAK_02_000021", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(21, "Se22", "しゃま1", "Shama1", "", "", "SE_GAME_BREAK_01_000027", "SE_GAME_BREAK_02_000027", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(22, "Se23", "みるく1", "Milk1", "", "", "SE_GAME_BREAK_01_000029", "SE_GAME_BREAK_02_000029", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(23, "Se24", "OTOHIME", "OTOHIME", "", "", "SE_GAME_BREAK_01_000022", "SE_GAME_BREAK_02_000022", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(24, "Se25", "ラズ2", "Ras2", "", "", "SE_GAME_BREAK_01_000023", "SE_GAME_BREAK_02_000023", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(25, "Se26", "シフォン2", "Chiffon2", "", "", "SE_GAME_BREAK_01_000024", "SE_GAME_BREAK_02_000024", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(26, "Se27", "ソルト2", "Salt2", "", "", "SE_GAME_BREAK_01_000025", "SE_GAME_BREAK_02_000025", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(27, "Se28", "しゃま2", "Shama2", "", "", "SE_GAME_BREAK_01_000028", "SE_GAME_BREAK_02_000028", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(28, "Se29", "みるく2", "Milk2", "", "", "SE_GAME_BREAK_01_000030", "SE_GAME_BREAK_02_000030", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(29, "Se30", "でらっくま", "Delukkuma", "", "", "SE_GAME_BREAK_01_000026", "SE_GAME_BREAK_02_000026", 0, "UI_OPT_E_24_01"),
			new OptionBreakseTableRecord(30, "Se31", "らいむっくま＆れもんっくま", "", "", "", "SE_GAME_BREAK_01_000031", "SE_GAME_BREAK_02_000031", 0, "UI_OPT_E_24_01")
		};

		public static bool IsActive(this OptionBreakseID self)
		{
			if (self >= OptionBreakseID.Se1 && self < OptionBreakseID.End)
			{
				return self != OptionBreakseID.Se1;
			}
			return false;
		}

		public static bool IsValid(this OptionBreakseID self)
		{
			if (self >= OptionBreakseID.Se1)
			{
				return self < OptionBreakseID.End;
			}
			return false;
		}

		public static void Clamp(this OptionBreakseID self)
		{
			if (self < OptionBreakseID.Se1)
			{
				self = OptionBreakseID.Se1;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionBreakseID)GetEnd();
			}
		}

		public static int GetEnd(this OptionBreakseID self)
		{
			return GetEnd();
		}

		public static OptionBreakseID FindID(string enumName)
		{
			for (OptionBreakseID optionBreakseID = OptionBreakseID.Se1; optionBreakseID < OptionBreakseID.End; optionBreakseID++)
			{
				if (optionBreakseID.GetEnumName() == enumName)
				{
					return optionBreakseID;
				}
			}
			return OptionBreakseID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionBreakseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionBreakseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionBreakseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionBreakseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionBreakseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionBreakseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		public static string GetSeGoodEnum(this OptionBreakseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].SeGoodEnum;
			}
			return "";
		}

		public static string GetSeBadEnum(this OptionBreakseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].SeBadEnum;
			}
			return "";
		}

		public static bool IsDefault(this OptionBreakseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isDefault;
			}
			return false;
		}

		public static string GetFilePath(this OptionBreakseID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FilePath;
			}
			return "";
		}
	}
}
