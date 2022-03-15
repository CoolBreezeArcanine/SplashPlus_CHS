namespace DB
{
	public static class ContentBitIDEnum
	{
		private static readonly ContentBitTableRecord[] records = new ContentBitTableRecord[24]
		{
			new ContentBitTableRecord(0, "FirstPlay", "初回プレイフラグ", 0),
			new ContentBitTableRecord(1, "FirstPassRead", "初回パス読み込み", 1),
			new ContentBitTableRecord(2, "FirstOutOfServicePass", "初回期限切れパス使用", 1),
			new ContentBitTableRecord(3, "FirstAnotherPass", "初回他人パス使用", 1),
			new ContentBitTableRecord(4, "FirstSimpleSetting", "初回アイコン撮影シーケンス", 0),
			new ContentBitTableRecord(5, "FirstMapSelect", "初回ちほー選択", 1),
			new ContentBitTableRecord(6, "FirstCharaSelect", "初回キャラセレクト", 1),
			new ContentBitTableRecord(7, "FirstCharaGoodBad", "初回キャラ不得意", 1),
			new ContentBitTableRecord(8, "FirstRateCategory", "初回レートカテゴリ表示", 1),
			new ContentBitTableRecord(9, "FirstCharaAwake", "初回キャラ覚醒", 1),
			new ContentBitTableRecord(10, "FirstTotalResult", "初回総合リザルト", 0),
			new ContentBitTableRecord(11, "FirstCollecitonCustom", "初回コレクションカスタム", 1),
			new ContentBitTableRecord(12, "FirstCollecitonEnd", "初回コレクションカスタム終了時", 1),
			new ContentBitTableRecord(13, "FirstCollecitonPartner", "初回コレクションでのパートナーインフォ", 1),
			new ContentBitTableRecord(14, "FirstBossAppear", "初回ボス登場", 1),
			new ContentBitTableRecord(15, "FirstCharaSelectDxPlus", "初回キャラセレクトver1.10(キャラロックインフォ)", 1),
			new ContentBitTableRecord(16, "FirstLoginBonus", "初回ログインぼボーナスインフォ", 1),
			new ContentBitTableRecord(17, "FirstPassportDxPlus", "初回パスポートver1.10(パスポートインフォ)", 1),
			new ContentBitTableRecord(18, "FirstMapSelectDxPlus", "初回ちほーセレクトver1.10(キャラセレスキップインフォ)", 1),
			new ContentBitTableRecord(19, "FirstFriendBattleResult", "初回オトモダチ対戦リザルト", 1),
			new ContentBitTableRecord(20, "FirstA4Class", "初回A4クラス到達", 1),
			new ContentBitTableRecord(21, "FirstModeSelect", "初回モードセレクト", 1),
			new ContentBitTableRecord(22, "FirstTicketSelect", "初回チケットセレクト", 0),
			new ContentBitTableRecord(23, "FirstSinCourseAdd", "初回真段位追加", 0)
		};

		public static bool IsActive(this ContentBitID self)
		{
			if (self >= ContentBitID.FirstPlay && self < ContentBitID.End)
			{
				return self != ContentBitID.FirstPlay;
			}
			return false;
		}

		public static bool IsValid(this ContentBitID self)
		{
			if (self >= ContentBitID.FirstPlay)
			{
				return self < ContentBitID.End;
			}
			return false;
		}

		public static void Clamp(this ContentBitID self)
		{
			if (self < ContentBitID.FirstPlay)
			{
				self = ContentBitID.FirstPlay;
			}
			else if ((int)self >= GetEnd())
			{
				self = (ContentBitID)GetEnd();
			}
		}

		public static int GetEnd(this ContentBitID self)
		{
			return GetEnd();
		}

		public static ContentBitID FindID(string enumName)
		{
			for (ContentBitID contentBitID = ContentBitID.FirstPlay; contentBitID < ContentBitID.End; contentBitID++)
			{
				if (contentBitID.GetEnumName() == enumName)
				{
					return contentBitID;
				}
			}
			return ContentBitID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this ContentBitID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this ContentBitID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this ContentBitID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static bool IsGuestIgnore(this ContentBitID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isGuestIgnore;
			}
			return false;
		}
	}
}
