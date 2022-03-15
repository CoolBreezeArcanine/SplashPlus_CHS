namespace Manager
{
	public struct ChallengeDetail
	{
		public int challengeId;

		public bool isEnable;

		public int startLife;

		public MusicDifficultyID unlockDifficulty;

		public int nextRelaxDay;

		public bool infoEnable;

		public ChallengeDetail(int id = -1, bool enable = false, int life = 0, MusicDifficultyID diff = MusicDifficultyID.Invalid, int day = -1, bool info = true)
		{
			challengeId = id;
			isEnable = enable;
			startLife = life;
			unlockDifficulty = diff;
			nextRelaxDay = day;
			infoEnable = info;
		}
	}
}
