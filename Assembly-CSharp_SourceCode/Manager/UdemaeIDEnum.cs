using MAI2.Util;
using Manager.MaiStudio;

namespace Manager
{
	public static class UdemaeIDEnum
	{
		public static int GetEnumValue(this UdemaeID self)
		{
			if (self.IsValid())
			{
				return (int)self;
			}
			return 0;
		}

		public static bool IsActive(this UdemaeID self)
		{
			if (self >= UdemaeID.Class_B5 && self < UdemaeID.End)
			{
				return self != UdemaeID.Class_B5;
			}
			return false;
		}

		public static bool IsValid(this UdemaeID self)
		{
			if (self >= UdemaeID.Class_B5)
			{
				return self < UdemaeID.End;
			}
			return false;
		}

		public static string GetName(this UdemaeID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetClass((int)self).name.str;
			}
			return "";
		}

		public static int GetStartPoint(this UdemaeID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetClass((int)self).startPoint;
			}
			return 0;
		}

		public static int GetBorder(this UdemaeID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetClass((int)self).classupPoint;
			}
			return 0;
		}

		public static int GetWinSame(this UdemaeID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetClass((int)self).winSameClass;
			}
			return 0;
		}

		public static int GetLoseSame(this UdemaeID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetClass((int)self).loseSameClass;
			}
			return 0;
		}

		public static int GetWinUpper(this UdemaeID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetClass((int)self).winUpperClass;
			}
			return 0;
		}

		public static int GetLoseUpper(this UdemaeID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetClass((int)self).loseUpperClass;
			}
			return 0;
		}

		public static int GetWinLower(this UdemaeID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetClass((int)self).winLowerClass;
			}
			return 0;
		}

		public static int GetLoseLower(this UdemaeID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetClass((int)self).loseLowerClass;
			}
			return 0;
		}

		public static int GetClassBoss(this UdemaeID self)
		{
			if (self.IsValid())
			{
				StringID classBoss = Singleton<DataManager>.Instance.GetClass((int)self).classBoss;
				if (classBoss != null)
				{
					return classBoss.id;
				}
			}
			return -1;
		}

		public static int GetNpcParamBoss(this UdemaeID self)
		{
			if (self.IsValid())
			{
				StringID npcBaseMusicByBoss = Singleton<DataManager>.Instance.GetClass((int)self).npcBaseMusicByBoss;
				if (npcBaseMusicByBoss != null)
				{
					return npcBaseMusicByBoss.id;
				}
			}
			return -1;
		}

		public static int GetNpcFluctuateAchieveLower(this UdemaeID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetClass((int)self).npcFluctuationAchieveLower;
			}
			return 0;
		}

		public static int GetNpcFluctuateAchieveUpper(this UdemaeID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetClass((int)self).npcFluctuationAchieveUpper;
			}
			return 0;
		}

		public static int GetNpcFluctuateLevelLower(this UdemaeID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetClass((int)self).npcFluctuationLevelLower;
			}
			return 0;
		}

		public static int GetNpcFluctuateLevelUpper(this UdemaeID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetClass((int)self).npcFluctuationLevelUpper;
			}
			return 0;
		}
	}
}
