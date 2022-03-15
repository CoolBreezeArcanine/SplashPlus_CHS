using System;
using DB;

namespace Manager.UserDatas
{
	[Serializable]
	public class UserScore
	{
		public int id;

		public uint playcount;

		public uint achivement;

		public PlayComboflagID combo;

		public PlaySyncflagID sync;

		public uint deluxscore;

		public MusicClearrankID scoreRank;

		public UserScore(int _id)
		{
			id = _id;
			playcount = 0u;
			achivement = 0u;
			combo = PlayComboflagID.None;
			sync = PlaySyncflagID.None;
			deluxscore = 0u;
			scoreRank = MusicClearrankID.Rank_D;
		}

		public bool IsFullCombo()
		{
			if (combo != PlayComboflagID.Gold)
			{
				return combo == PlayComboflagID.Silver;
			}
			return true;
		}

		public bool IsAllPerfect()
		{
			if (combo != PlayComboflagID.AllPerfect)
			{
				return combo == PlayComboflagID.AllPerfectPlus;
			}
			return true;
		}

		public bool IsChain()
		{
			if (sync != PlaySyncflagID.ChainLow)
			{
				return sync == PlaySyncflagID.ChainHi;
			}
			return true;
		}

		public bool IsSync()
		{
			if (sync != PlaySyncflagID.SyncLow)
			{
				return sync == PlaySyncflagID.SyncHi;
			}
			return true;
		}
	}
}
