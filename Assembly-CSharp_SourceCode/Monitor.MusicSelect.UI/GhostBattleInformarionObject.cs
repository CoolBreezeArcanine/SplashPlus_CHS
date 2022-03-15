using Timeline;
using UnityEngine;

namespace Monitor.MusicSelect.UI
{
	public class GhostBattleInformarionObject : UserInformationController
	{
		[SerializeField]
		[Header("達成率")]
		private AchievementCounterObject _achievementObject;

		public void SetAchievementRate(uint achieve)
		{
		}
	}
}
