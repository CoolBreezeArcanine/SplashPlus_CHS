namespace DB
{
	public static class OptionCateJudgeIDEnum
	{
		private static readonly OptionCateJudgeTableRecord[] records;

		public static bool IsActive(this OptionCateJudgeID self)
		{
			if (self >= OptionCateJudgeID.DispCenter && self < OptionCateJudgeID.End)
			{
				return self > OptionCateJudgeID.DispCenter;
			}
			return false;
		}

		public static bool IsValid(this OptionCateJudgeID self)
		{
			if (self >= OptionCateJudgeID.DispCenter)
			{
				return self < OptionCateJudgeID.End;
			}
			return false;
		}

		public static void Clamp(this OptionCateJudgeID self)
		{
			if (self < OptionCateJudgeID.DispCenter)
			{
				self = OptionCateJudgeID.DispCenter;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionCateJudgeID)GetEnd();
			}
		}

		public static int GetEnd(this OptionCateJudgeID self)
		{
			return GetEnd();
		}

		public static OptionCateJudgeID FindID(string enumName)
		{
			for (OptionCateJudgeID optionCateJudgeID = OptionCateJudgeID.DispCenter; optionCateJudgeID < OptionCateJudgeID.End; optionCateJudgeID++)
			{
				if (optionCateJudgeID.GetEnumName() == enumName)
				{
					return optionCateJudgeID;
				}
			}
			return OptionCateJudgeID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionCateJudgeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionCateJudgeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionCateJudgeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionCateJudgeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionCateJudgeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionCateJudgeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		static OptionCateJudgeIDEnum()
		{
			records = new OptionCateJudgeTableRecord[8]
			{
				new OptionCateJudgeTableRecord(0, "DispCenter", "画面中央显示", string.Empty, "变更画面中央显示的信息", string.Empty),
				new OptionCateJudgeTableRecord(1, "DispJudge", "判定显示", string.Empty, "改变碰到音符时出现判定的显示设置", string.Empty),
				new OptionCateJudgeTableRecord(2, "DispJudgeTapPos", "TAP判定的显示位置", string.Empty, "调整TOUCH音符以外的评价判定所显示的位置", string.Empty),
				new OptionCateJudgeTableRecord(3, "DispJudgeTouchPos", "TOUCH判定的显示位置", string.Empty, "调整TOUCH音符的判定显示位置", string.Empty),
				new OptionCateJudgeTableRecord(4, "ChainDisp", "显示同步率计算", string.Empty, "改变多人游戏时同步率计算的显示设置", string.Empty),
				new OptionCateJudgeTableRecord(5, "SubMonitor_Achive", "在上方画面显示达成率", string.Empty, "设置游戏时上方画面中达成率的显示方法", string.Empty),
				new OptionCateJudgeTableRecord(6, "RatingDisp", "评级･段位显示", string.Empty, "可以改变评级和段位的显示", string.Empty),
				new OptionCateJudgeTableRecord(7, "SubMonitor_Appeal", "招募邀请", string.Empty, "在上方画面中显示说明", string.Empty)
			};
		}
	}
}
