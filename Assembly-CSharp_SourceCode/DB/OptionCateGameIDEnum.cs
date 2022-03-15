namespace DB
{
	public static class OptionCateGameIDEnum
	{
		private static readonly OptionCateGameTableRecord[] records;

		public static bool IsActive(this OptionCateGameID self)
		{
			if (self >= OptionCateGameID.TrackSkip && self < OptionCateGameID.End)
			{
				return self != OptionCateGameID.TrackSkip;
			}
			return false;
		}

		public static bool IsValid(this OptionCateGameID self)
		{
			if (self >= OptionCateGameID.TrackSkip)
			{
				return self < OptionCateGameID.End;
			}
			return false;
		}

		public static void Clamp(this OptionCateGameID self)
		{
			if (self < OptionCateGameID.TrackSkip)
			{
				self = OptionCateGameID.TrackSkip;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionCateGameID)GetEnd();
			}
		}

		public static int GetEnd(this OptionCateGameID self)
		{
			return GetEnd();
		}

		public static OptionCateGameID FindID(string enumName)
		{
			for (OptionCateGameID optionCateGameID = OptionCateGameID.TrackSkip; optionCateGameID < OptionCateGameID.End; optionCateGameID++)
			{
				if (optionCateGameID.GetEnumName() == enumName)
				{
					return optionCateGameID;
				}
			}
			return OptionCateGameID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionCateGameID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionCateGameID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionCateGameID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionCateGameID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionCateGameID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionCateGameID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		static OptionCateGameIDEnum()
		{
			records = new OptionCateGameTableRecord[7]
			{
				new OptionCateGameTableRecord(0, "TrackSkip", "跳过乐曲", string.Empty, "中止游戏进入结算画面", string.Empty),
				new OptionCateGameTableRecord(1, "Mirror", "镜像模式", string.Empty, "替换音符的上下和左右的配置", string.Empty),
				new OptionCateGameTableRecord(2, "StarRotate", "旋转滑动", string.Empty, "使☆随着滑动操作的速度进行旋转", string.Empty),
				new OptionCateGameTableRecord(3, "AdjustTiming", "判定调整A", string.Empty, "适合靠听节奏来游玩的玩家\r\n调整音符到达判定线的时间", string.Empty),
				new OptionCateGameTableRecord(4, "JudgeTiming", "判定调整B", string.Empty, "适合靠目押来游玩的玩家\r\n直接调整判定的时机", string.Empty),
				new OptionCateGameTableRecord(5, "Brightness", "影像的亮度", string.Empty, "调整游戏过程中背景影像的亮度", string.Empty),
				new OptionCateGameTableRecord(6, "TouchEffect", "反馈特效", string.Empty, "改变触碰到屏幕时播放的特效", string.Empty)
			};
		}
	}
}
