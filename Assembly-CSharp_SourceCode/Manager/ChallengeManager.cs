using MAI2.Util;
using Manager.MaiStudio;

namespace Manager
{
	public class ChallengeManager
	{
		public static ChallengeDetail GetChallengeDetail(int challengeID)
		{
			EventManager instance = Singleton<EventManager>.Instance;
			ChallengeData challengeData = Singleton<DataManager>.Instance.GetChallengeData(challengeID);
			ChallengeDetail result = default(ChallengeDetail);
			if (challengeData != null)
			{
				result.challengeId = challengeID;
				if (instance.IsOpenEvent(challengeData.EventName.id))
				{
					long eventStartUnixTime = Singleton<EventManager>.Instance.GetEventStartUnixTime(challengeData.EventName.id);
					int num = (int)(TimeManager.PlayBaseTime - eventStartUnixTime) / 86400;
					int num2 = 9999;
					int num3 = -1;
					if (challengeData.Relax != null)
					{
						foreach (ChallengeRelax item in challengeData.Relax)
						{
							if (num < item.Day && item.Day < num2)
							{
								num2 = item.Day;
							}
							if (num3 < item.Day)
							{
								num3 = item.Day;
							}
						}
					}
					if (num2 == 9999)
					{
						num2 = num3;
					}
					bool flag = num2 == num3;
					bool flag2 = false;
					if (challengeData.Relax != null)
					{
						foreach (ChallengeRelax item2 in challengeData.Relax)
						{
							if (num2 == item2.Day)
							{
								flag2 = true;
								result.isEnable = true;
								result.startLife = item2.Life;
								result.nextRelaxDay = (flag ? (-1) : (num2 - num));
								result.unlockDifficulty = (MusicDifficultyID)item2.ReleaseDiff.id;
								result.infoEnable = true;
								break;
							}
						}
					}
					if (num2 >= 999)
					{
						result.infoEnable = false;
					}
					if (result.nextRelaxDay <= 0)
					{
						result.infoEnable = false;
					}
					if (challengeData.Relax == null || num3 == -1)
					{
						result.isEnable = false;
						result.infoEnable = false;
					}
					if (flag2)
					{
					}
				}
			}
			else
			{
				result.isEnable = false;
				result.infoEnable = false;
			}
			return result;
		}
	}
}
