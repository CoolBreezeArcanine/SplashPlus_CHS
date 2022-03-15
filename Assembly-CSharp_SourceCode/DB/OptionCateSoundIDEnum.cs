namespace DB
{
	public static class OptionCateSoundIDEnum
	{
		private static readonly OptionCateSoundTableRecord[] records;

		public static bool IsActive(this OptionCateSoundID self)
		{
			if (self >= OptionCateSoundID.Ans_Vol && self < OptionCateSoundID.End)
			{
				return self != OptionCateSoundID.Ans_Vol;
			}
			return false;
		}

		public static bool IsValid(this OptionCateSoundID self)
		{
			if (self >= OptionCateSoundID.Ans_Vol)
			{
				return self < OptionCateSoundID.End;
			}
			return false;
		}

		public static void Clamp(this OptionCateSoundID self)
		{
			if (self < OptionCateSoundID.Ans_Vol)
			{
				self = OptionCateSoundID.Ans_Vol;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionCateSoundID)GetEnd();
			}
		}

		public static int GetEnd(this OptionCateSoundID self)
		{
			return GetEnd();
		}

		public static OptionCateSoundID FindID(string enumName)
		{
			for (OptionCateSoundID optionCateSoundID = OptionCateSoundID.Ans_Vol; optionCateSoundID < OptionCateSoundID.End; optionCateSoundID++)
			{
				if (optionCateSoundID.GetEnumName() == enumName)
				{
					return optionCateSoundID;
				}
			}
			return OptionCateSoundID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionCateSoundID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionCateSoundID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionCateSoundID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionCateSoundID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionCateSoundID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionCateSoundID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		static OptionCateSoundIDEnum()
		{
			records = new OptionCateSoundTableRecord[11]
			{
				new OptionCateSoundTableRecord(0, "Ans_Vol", "正解音音量", string.Empty, "设定以正确时机拍打到\n音符时的音效音量\r\n", string.Empty),
				new OptionCateSoundTableRecord(1, "Tap_Se", "音符判定音", string.Empty, "设定TAP･HOLD･TOUCH等\n音符在哪个判定进行音效播放", string.Empty),
				new OptionCateSoundTableRecord(2, "Tap_Vol", "音符判定音量", string.Empty, "设定TAP･HOLD･TOUCH等\r\n音符的判定音量", string.Empty),
				new OptionCateSoundTableRecord(3, "Break_Se", "BREAK判定音", string.Empty, "切换BREAK判定的判定音效", string.Empty),
				new OptionCateSoundTableRecord(4, "Break_Vol", "BREAK音符音量", string.Empty, "设定BREAK音符的音量", string.Empty),
				new OptionCateSoundTableRecord(5, "Ex_Se", "EX判定音", string.Empty, "切换EX判定音的音效", string.Empty),
				new OptionCateSoundTableRecord(6, "Ex_Vol", "EX判定音量", string.Empty, "设定EX音符的判定音量", string.Empty),
				new OptionCateSoundTableRecord(7, "Slide_Se", "SLIDE音", string.Empty, "切换SLIDE时的音效", string.Empty),
				new OptionCateSoundTableRecord(8, "Slide_Vol", "SLIDE时的音量", string.Empty, "设定SLIDE时的音量", string.Empty),
				new OptionCateSoundTableRecord(9, "TouchHold_Vol", "TOUCH特效音量", string.Empty, "设定TOUCH特效和\r\nTOUCH持特效的音量", string.Empty),
				new OptionCateSoundTableRecord(10, "DamageSe_Vol", "受到伤害时的音量", "", "设置完美挑战中受到伤害时的音效音量", "")
			};
		}
	}
}
