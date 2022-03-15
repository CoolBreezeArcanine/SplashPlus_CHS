using MAI2.Util;
using Manager.MaiStudio;

namespace Manager.UserDatas
{
	public class UserUdemae
	{
		public enum ChangeGrade
		{
			Even,
			WinUp,
			LoseDown,
			BossWinUp,
			BossStay,
			End
		}

		public int UdemaeValue;

		public int MaxUdemaeValue;

		public int ClassValue;

		public int MaxClassValue;

		public uint TotalWinNum;

		public uint TotalLoseNum;

		public uint MaxWinNum;

		public uint MaxLoseNum;

		public uint WinNum;

		public uint LoseNum;

		public uint NpcTotalWinNum;

		public uint NpcTotalLoseNum;

		public uint NpcMaxWinNum;

		public uint NpcMaxLoseNum;

		public uint NpcWinNum;

		public uint NpcLoseNum;

		public UdemaeID ClassID;

		public int ClassPoint;

		public UdemaeID MaxClassID;

		public const int CLASS_MAX_POINT = 10000;

		public UserUdemae()
		{
			Initialize();
		}

		public void Initialize()
		{
			UdemaeValue = 0;
			MaxUdemaeValue = 0;
			ClassValue = 0;
			MaxClassValue = 0;
			TotalWinNum = 0u;
			TotalLoseNum = 0u;
			MaxWinNum = 0u;
			MaxLoseNum = 0u;
			WinNum = 0u;
			LoseNum = 0u;
			NpcTotalWinNum = 0u;
			NpcTotalLoseNum = 0u;
			NpcMaxWinNum = 0u;
			NpcMaxLoseNum = 0u;
			NpcWinNum = 0u;
			NpcLoseNum = 0u;
			SetClassData();
			SetMaxClassData();
		}

		public void SetClassValue(int classValue)
		{
			ClassValue = classValue;
			SetClassData();
			SetMaxClassData();
		}

		public void SetMaxClassValue(int maxCassValue)
		{
			MaxClassValue = maxCassValue;
			SetMaxClassData();
		}

		private void SetClassData()
		{
			UdemaeID udemaeID = (UdemaeID)(ClassValue / 10000);
			int num = ClassValue % 10000;
			ClassID = ((udemaeID < UdemaeID.End) ? udemaeID : UdemaeID.Class_LEGEND);
			ClassData @class = Singleton<DataManager>.Instance.GetClass((int)ClassID);
			if (@class == null)
			{
				return;
			}
			int classupPoint = @class.classupPoint;
			if (num < classupPoint)
			{
				ClassPoint = num;
				return;
			}
			if (num > classupPoint)
			{
				ClassPoint = 0;
				return;
			}
			ClassPoint = classupPoint;
			if (!IsBossBattleEnable())
			{
				ClassPoint = classupPoint - 1;
			}
		}

		private void SetMaxClassData()
		{
			UdemaeID udemaeID = (UdemaeID)(MaxClassValue / 10000);
			MaxClassID = ((udemaeID < UdemaeID.End) ? udemaeID : UdemaeID.Class_LEGEND);
		}

		public static UdemaeID GetRateToUdemaeID(int classValue)
		{
			UdemaeID udemaeID = (UdemaeID)(classValue / 10000);
			if (udemaeID >= UdemaeID.End)
			{
				return UdemaeID.Class_LEGEND;
			}
			return udemaeID;
		}

		public static int GetClassPointToClassValue(int classValue)
		{
			UdemaeID udemaeID = (UdemaeID)(classValue / 10000);
			int num = classValue % 10000;
			int result = 0;
			udemaeID = ((udemaeID < UdemaeID.End) ? udemaeID : UdemaeID.Class_LEGEND);
			ClassData @class = Singleton<DataManager>.Instance.GetClass((int)udemaeID);
			if (@class != null)
			{
				int classupPoint = @class.classupPoint;
				result = ((num >= classupPoint) ? classupPoint : num);
			}
			return result;
		}

		public static bool IsBossSpecial(int classValue)
		{
			UdemaeID rateToUdemaeID = GetRateToUdemaeID(classValue);
			if (rateToUdemaeID != UdemaeID.Invalid)
			{
				return IsBossSpecial(rateToUdemaeID);
			}
			return false;
		}

		public static bool IsBossSpecial(UdemaeID classID)
		{
			return GetBossData(classID.GetClassBoss())?.isTransmission ?? false;
		}

		public bool UpdateResult(bool isWin, int rivalClassValue, bool isNpc, bool isBoss)
		{
			UdemaeID classID = ClassID;
			int num = GetRateToUdemaeID(rivalClassValue) - classID;
			int num2 = 0;
			if (isWin)
			{
				if (isNpc)
				{
					NpcTotalWinNum++;
					NpcWinNum++;
					NpcLoseNum = 0u;
					if (NpcMaxWinNum < NpcWinNum)
					{
						NpcMaxWinNum = NpcWinNum;
					}
				}
				else
				{
					TotalWinNum++;
					WinNum++;
					LoseNum = 0u;
					if (MaxWinNum < WinNum)
					{
						MaxWinNum = WinNum;
					}
				}
				if (num > -3)
				{
					num2 = ((num < 0) ? classID.GetWinLower() : ((num != 0) ? classID.GetWinUpper() : classID.GetWinSame()));
				}
			}
			else
			{
				if (isNpc)
				{
					NpcTotalLoseNum++;
					NpcLoseNum++;
					NpcWinNum = 0u;
					if (NpcMaxLoseNum < NpcLoseNum)
					{
						NpcMaxLoseNum = NpcLoseNum;
					}
				}
				else
				{
					TotalLoseNum++;
					LoseNum++;
					WinNum = 0u;
					if (MaxLoseNum < LoseNum)
					{
						MaxLoseNum = LoseNum;
					}
				}
				num2 = ((num < 0) ? classID.GetLoseLower() : ((num != 0) ? classID.GetLoseUpper() : classID.GetLoseSame()));
			}
			bool flag = false;
			bool flag2 = false;
			if (isBoss)
			{
				if (isWin)
				{
					flag = true;
				}
				else
				{
					num2 = 0;
				}
			}
			else
			{
				int border = classID.GetBorder();
				ClassPoint += num2;
				if (IsBossBattleEnable())
				{
					ClassPoint = border;
				}
				else if (ClassPoint >= border)
				{
					flag = true;
				}
				else if (ClassPoint < 0)
				{
					flag2 = true;
				}
			}
			if (classID == UdemaeID.Class_LEGEND)
			{
				if (ClassPoint < 0)
				{
					ClassPoint = 0;
				}
				if (ClassPoint > UdemaeID.Class_LEGEND.GetBorder())
				{
					ClassPoint = UdemaeID.Class_LEGEND.GetBorder();
				}
			}
			else if (flag)
			{
				if (classID == UdemaeID.Class_LEGEND)
				{
					ClassPoint = classID.GetBorder();
				}
				else
				{
					ClassID++;
					ClassPoint = ClassID.GetStartPoint();
				}
			}
			else if (flag2)
			{
				if (classID == UdemaeID.Class_B5)
				{
					ClassPoint = 0;
				}
				else
				{
					ClassID--;
					ClassPoint = ClassID.GetStartPoint();
				}
			}
			if (ClassPoint >= 10000)
			{
				ClassPoint = 9999;
			}
			ClassValue = (int)ClassID * 10000 + ClassPoint;
			if (MaxClassValue < ClassValue)
			{
				MaxClassValue = ClassValue;
			}
			MaxClassID = (UdemaeID)(MaxClassValue / 10000);
			return num2 != 0;
		}

		public uint GetClassRate()
		{
			return 0u;
		}

		public bool IsBossBattleEnable(UdemaeID classID, int classPoint, UdemaeID maxClassID = UdemaeID.Invalid)
		{
			if (maxClassID == UdemaeID.Invalid)
			{
				maxClassID = classID;
			}
			UdemaeID udemaeID = classID + 1;
			if (classID.GetClassBoss() != 0 && maxClassID < udemaeID && classID.GetBorder() <= classPoint)
			{
				return true;
			}
			return false;
		}

		public bool IsBossBattleEnable()
		{
			return IsBossBattleEnable(ClassID, ClassPoint, MaxClassID);
		}

		public bool IsBossExist(UdemaeID classID, UdemaeID maxClassID)
		{
			if (classID < maxClassID)
			{
				return false;
			}
			if (classID.GetClassBoss() != 0)
			{
				return true;
			}
			return false;
		}

		public bool IsBossExist()
		{
			return IsBossExist(ClassID, MaxClassID);
		}

		public ChangeGrade CalcChangeGrade(int rivalRate, bool isBoss)
		{
			UdemaeID classID = ClassID;
			int num = GetRateToUdemaeID(rivalRate) - classID;
			int classPoint = ClassPoint;
			int classPoint2 = ClassPoint;
			if (classID == UdemaeID.Class_LEGEND)
			{
				return ChangeGrade.Even;
			}
			if (num <= -3)
			{
				classPoint = classPoint;
				classPoint2 += classID.GetLoseLower();
			}
			else if (num < 0)
			{
				classPoint += classID.GetWinLower();
				classPoint2 += classID.GetLoseLower();
			}
			else if (num == 0)
			{
				classPoint += classID.GetWinSame();
				classPoint2 += classID.GetLoseSame();
			}
			else
			{
				classPoint += classID.GetWinUpper();
				classPoint2 += classID.GetLoseUpper();
			}
			if (isBoss)
			{
				return ChangeGrade.BossWinUp;
			}
			if (classID.GetBorder() <= classPoint)
			{
				if (IsBossBattleEnable(classID, classPoint, MaxClassID))
				{
					return ChangeGrade.BossStay;
				}
				return ChangeGrade.WinUp;
			}
			if (classPoint2 < 0)
			{
				return ChangeGrade.LoseDown;
			}
			return ChangeGrade.Even;
		}

		public UdemaeBossData GetBossData()
		{
			return GetBossData(ClassID.GetClassBoss());
		}

		private static UdemaeBossData GetBossData(int bossId)
		{
			if (bossId == 0)
			{
				return null;
			}
			return Singleton<DataManager>.Instance.GetUdemaeBoss(bossId);
		}
	}
}
