using System;

namespace Manager.UserDatas
{
	[Serializable]
	public class UserAct
	{
		public enum ActivityCode
		{
			PlayDX = 10,
			RankS = 20,
			RankSP = 21,
			RankSS = 22,
			RankSSP = 23,
			RankSSS = 24,
			RankSSSP = 25,
			FullCombo = 30,
			FullComboP = 31,
			AllPerfect = 32,
			AllPerfectP = 33,
			FullSync = 40,
			FullSyncP = 41,
			FullSyncDx = 42,
			FullSyncDxP = 43,
			ClassUp_old = 50,
			DxRate = 60,
			Awake = 70,
			MapComplete = 80,
			MapFound = 90,
			TransmissionMusic = 100,
			TaskMusicClear = 110,
			ChallengeMusicClear = 120,
			RankUp = 130,
			ClassUp = 140
		}

		public enum MusicResult
		{
			None,
			ScoreRank,
			FullCombo,
			FullSync
		}

		public int kind;

		public int id;

		public long sortNumber;

		public int param1;

		public int param2;

		public int param3;

		public int param4;

		public UserAct(int kind, int id, long sortNumber, int param1, int param2, int param3, int param4)
		{
			this.kind = kind;
			this.id = id;
			this.sortNumber = sortNumber;
			this.param1 = param1;
			this.param2 = param2;
			this.param3 = param3;
			this.param4 = param4;
		}
	}
}
