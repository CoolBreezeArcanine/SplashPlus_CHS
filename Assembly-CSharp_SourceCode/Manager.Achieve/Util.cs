using DB;
using Manager.MaiStudio;

namespace Manager.Achieve
{
	public class Util
	{
		public static int ToScoreId(int musicId, int difficulty)
		{
			return musicId * 100 + difficulty;
		}

		public static MusicDifficultyID ToMusicDifficultyID(DifficultyID src)
		{
			return src switch
			{
				DifficultyID.BAS => MusicDifficultyID.Basic, 
				DifficultyID.ADV => MusicDifficultyID.Advanced, 
				DifficultyID.EXP => MusicDifficultyID.Expert, 
				DifficultyID.MAS => MusicDifficultyID.Master, 
				DifficultyID.REM => MusicDifficultyID.ReMaster, 
				DifficultyID.ANY => MusicDifficultyID.Invalid, 
				DifficultyID.ALL => MusicDifficultyID.Invalid, 
				_ => MusicDifficultyID.Invalid, 
			};
		}

		public static bool CheckCombo(PlayComboflagID id, ReleaseItemConditions cond)
		{
			return cond.fcId switch
			{
				FcID.None => true, 
				FcID.FC => id >= PlayComboflagID.Silver, 
				FcID.FCp => id >= PlayComboflagID.Gold, 
				FcID.AP => id >= PlayComboflagID.AllPerfect, 
				FcID.APp => id >= PlayComboflagID.AllPerfectPlus, 
				_ => false, 
			};
		}

		public static bool CheckSync(PlaySyncflagID id, ReleaseItemConditions cond)
		{
			return cond.syncId switch
			{
				SyncID.None => true, 
				SyncID.FS => id >= PlaySyncflagID.ChainLow, 
				SyncID.FSp => id >= PlaySyncflagID.ChainHi, 
				SyncID.FSD => id >= PlaySyncflagID.SyncLow, 
				SyncID.FSDp => id >= PlaySyncflagID.SyncHi, 
				_ => false, 
			};
		}

		public static bool CheckRank(MusicClearrankID id, ReleaseItemConditions cond)
		{
			return cond.rankId switch
			{
				RankID.D => id >= MusicClearrankID.Rank_D, 
				RankID.C => id >= MusicClearrankID.Rank_C, 
				RankID.B => id >= MusicClearrankID.Rank_B, 
				RankID.BB => id >= MusicClearrankID.Rank_BB, 
				RankID.BBB => id >= MusicClearrankID.Rank_BBB, 
				RankID.A => id >= MusicClearrankID.Rank_A, 
				RankID.AA => id >= MusicClearrankID.Rank_AA, 
				RankID.AAA => id >= MusicClearrankID.Rank_AAA, 
				RankID.S => id >= MusicClearrankID.Rank_S, 
				RankID.Sp => id >= MusicClearrankID.Rank_SP, 
				RankID.SS => id >= MusicClearrankID.Rank_SS, 
				RankID.SSp => id >= MusicClearrankID.Rank_SSP, 
				RankID.SSS => id >= MusicClearrankID.Rank_SSS, 
				RankID.SSSp => id >= MusicClearrankID.Rank_SSSP, 
				_ => false, 
			};
		}
	}
}
