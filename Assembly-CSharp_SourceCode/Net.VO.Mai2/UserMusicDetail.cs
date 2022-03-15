using System;
using DB;
using Manager;

namespace Net.VO.Mai2
{
	[Serializable]
	public struct UserMusicDetail
	{
		public int musicId;

		public MusicDifficultyID level;

		public uint playCount;

		public uint achievement;

		public PlayComboflagID comboStatus;

		public PlaySyncflagID syncStatus;

		public uint deluxscoreMax;

		public MusicClearrankID scoreRank;
	}
}
