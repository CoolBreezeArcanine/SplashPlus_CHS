using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public struct UserCourse
	{
		public int courseId;

		public bool isLastClear;

		public uint totalRestlife;

		public uint totalAchievement;

		public uint totalDeluxscore;

		public uint playCount;

		public string clearDate;

		public string lastPlayDate;

		public uint bestAchievement;

		public string bestAchievementDate;

		public uint bestDeluxscore;

		public string bestDeluxscoreDate;
	}
}
