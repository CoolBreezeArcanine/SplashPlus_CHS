using System;

namespace Manager.UserDatas
{
	[Serializable]
	public class UserCourse
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

		public UserCourse(int _id)
		{
			courseId = _id;
			isLastClear = false;
			totalRestlife = 0u;
			totalAchievement = 0u;
			totalDeluxscore = 0u;
			playCount = 0u;
			clearDate = "";
			lastPlayDate = "";
			bestAchievement = 0u;
			bestAchievementDate = "";
			bestDeluxscore = 0u;
			bestDeluxscoreDate = "";
		}

		public UserCourse(int id, bool lastClear, uint life, uint achieve, uint score, uint count, string clearDat, string lastDat, uint bestAchieve, string bestAchieveDate, uint bestScore, string bestScoreDate)
		{
			courseId = id;
			isLastClear = lastClear;
			totalRestlife = life;
			totalAchievement = achieve;
			totalDeluxscore = score;
			playCount = count;
			clearDate = clearDat;
			lastPlayDate = lastDat;
			bestAchievement = bestAchieve;
			bestAchievementDate = bestAchieveDate;
			bestDeluxscore = bestScore;
			bestDeluxscoreDate = bestScoreDate;
		}
	}
}
