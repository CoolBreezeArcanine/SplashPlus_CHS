using System;
using System.Diagnostics;
using System.Linq;
using DB;
using Manager.MaiStudio;

namespace Manager.Achieve
{
	public class AchieveTrack
	{
		private static readonly Func<AchieveTrackData, ReleaseItemConditions, bool>[] Commands = new Func<AchieveTrackData, ReleaseItemConditions, bool>[24]
		{
			FuncNone, FuncMusic, FuncFullCombo, FuncPeople, FuncRank, FuncCombo, FuncChain, FuncSync, FuncDifficulty, FuncTitle,
			FuncPlate, FuncIcon, FuncCharaSet, FuncSatellite, FuncMiss, FuncTapSpeed1Sonic, FuncTouchSpeed1Sonic, FuncSlideSpeedm1Speedp1, FuncRating, FuncRankWin,
			FuncRankLose, FuncOtomodachiWin, FuncOtomodachiLose, FuncDanni
		};

		public static bool Checks(int id, object arg, ReleaseConditions conds)
		{
			AchieveTrackData data = (AchieveTrackData)arg;
			return conds.GetItemConditions().All((ReleaseItemConditions cond) => Commands[(int)cond.kindTrack](data, cond));
		}

		[Conditional("_DEBUG_CHECK")]
		private static void DebugCheck(int id, AchieveTrackData data, ReleaseConditions conds)
		{
			conds.GetItemConditions().Select((ReleaseItemConditions i, int idx) => new
			{
				idx = idx,
				cmd = i.kindTrack,
				res = Commands[(int)i.kindTrack](data, i)
			}).ToArray();
		}

		private static bool FuncNone(AchieveTrackData data, ReleaseItemConditions cond)
		{
			return true;
		}

		private static bool FuncMusic(AchieveTrackData data, ReleaseItemConditions cond)
		{
			if (data.MusicId == cond.musicId.id)
			{
				return FuncDifficulty(data, cond);
			}
			return false;
		}

		private static bool FuncFullCombo(AchieveTrackData data, ReleaseItemConditions cond)
		{
			return cond.fcId switch
			{
				FcID.None => true, 
				FcID.FC => data.ComboType >= PlayComboflagID.Silver, 
				FcID.FCp => data.ComboType >= PlayComboflagID.Gold, 
				FcID.AP => data.ComboType >= PlayComboflagID.AllPerfect, 
				FcID.APp => data.ComboType >= PlayComboflagID.AllPerfectPlus, 
				_ => false, 
			};
		}

		private static bool FuncPeople(AchieveTrackData data, ReleaseItemConditions cond)
		{
			return data.PlayerNum >= cond.param;
		}

		private static bool FuncRank(AchieveTrackData data, ReleaseItemConditions cond)
		{
			return cond.rankId switch
			{
				RankID.D => data.Rank >= MusicClearrankID.Rank_D, 
				RankID.C => data.Rank >= MusicClearrankID.Rank_C, 
				RankID.B => data.Rank >= MusicClearrankID.Rank_B, 
				RankID.BB => data.Rank >= MusicClearrankID.Rank_BB, 
				RankID.BBB => data.Rank >= MusicClearrankID.Rank_BBB, 
				RankID.A => data.Rank >= MusicClearrankID.Rank_A, 
				RankID.AA => data.Rank >= MusicClearrankID.Rank_AA, 
				RankID.AAA => data.Rank >= MusicClearrankID.Rank_AAA, 
				RankID.S => data.Rank >= MusicClearrankID.Rank_S, 
				RankID.Sp => data.Rank >= MusicClearrankID.Rank_SP, 
				RankID.SS => data.Rank >= MusicClearrankID.Rank_SS, 
				RankID.SSp => data.Rank >= MusicClearrankID.Rank_SSP, 
				RankID.SSS => data.Rank >= MusicClearrankID.Rank_SSS, 
				RankID.SSSp => data.Rank >= MusicClearrankID.Rank_SSSP, 
				_ => false, 
			};
		}

		private static bool FuncCombo(AchieveTrackData data, ReleaseItemConditions cond)
		{
			return data.ComboNum >= cond.param;
		}

		private static bool FuncChain(AchieveTrackData data, ReleaseItemConditions cond)
		{
			return data.ChainNum >= cond.param;
		}

		private static bool FuncSync(AchieveTrackData data, ReleaseItemConditions cond)
		{
			return cond.syncId switch
			{
				SyncID.None => true, 
				SyncID.FS => data.SyncType >= PlaySyncflagID.ChainLow, 
				SyncID.FSp => data.SyncType >= PlaySyncflagID.ChainHi, 
				SyncID.FSD => data.SyncType >= PlaySyncflagID.SyncLow, 
				SyncID.FSDp => data.SyncType >= PlaySyncflagID.SyncHi, 
				_ => false, 
			};
		}

		private static bool FuncDifficulty(AchieveTrackData data, ReleaseItemConditions cond)
		{
			return cond.difficultyId switch
			{
				DifficultyID.BAS => data.Difficulty == MusicDifficultyID.Basic, 
				DifficultyID.ADV => data.Difficulty == MusicDifficultyID.Advanced, 
				DifficultyID.EXP => data.Difficulty == MusicDifficultyID.Expert, 
				DifficultyID.MAS => data.Difficulty == MusicDifficultyID.Master, 
				DifficultyID.REM => data.Difficulty == MusicDifficultyID.ReMaster, 
				DifficultyID.ANY => true, 
				DifficultyID.ALL => false, 
				_ => false, 
			};
		}

		private static bool FuncTitle(AchieveTrackData data, ReleaseItemConditions cond)
		{
			return data.EquipTitle == cond.titleId.id;
		}

		private static bool FuncPlate(AchieveTrackData data, ReleaseItemConditions cond)
		{
			return data.EquipPlate == cond.plateId.id;
		}

		private static bool FuncIcon(AchieveTrackData data, ReleaseItemConditions cond)
		{
			return data.EquipIcon == cond.iconId.id;
		}

		private static bool FuncCharaSet(AchieveTrackData data, ReleaseItemConditions cond)
		{
			return data.CharaSlot.FirstOrDefault((int i) => i == cond.charaId.id) != 0;
		}

		private static bool FuncSatellite(AchieveTrackData data, ReleaseItemConditions cond)
		{
			return data.Satellite == (int)cond.satelliteId;
		}

		private static bool FuncMiss(AchieveTrackData data, ReleaseItemConditions cond)
		{
			return data.MissNum == cond.param;
		}

		private static bool FuncTapSpeed1Sonic(AchieveTrackData data, ReleaseItemConditions cond)
		{
			return cond.hiSpeedTapTouchId switch
			{
				HiSpeedTapTouchID.SPEED_10 => data.NoteSpeed == OptionNotespeedID.Speed1_0, 
				HiSpeedTapTouchID.SPEED_15 => data.NoteSpeed == OptionNotespeedID.Speed1_5, 
				HiSpeedTapTouchID.SPEED_20 => data.NoteSpeed == OptionNotespeedID.Speed2_0, 
				HiSpeedTapTouchID.SPEED_25 => data.NoteSpeed == OptionNotespeedID.Speed2_5, 
				HiSpeedTapTouchID.SPEED_30 => data.NoteSpeed == OptionNotespeedID.Speed3_0, 
				HiSpeedTapTouchID.SPEED_35 => data.NoteSpeed == OptionNotespeedID.Speed3_5, 
				HiSpeedTapTouchID.SPEED_40 => data.NoteSpeed == OptionNotespeedID.Speed4_0, 
				HiSpeedTapTouchID.SPEED_45 => data.NoteSpeed == OptionNotespeedID.Speed4_5, 
				HiSpeedTapTouchID.SPEED_50 => data.NoteSpeed == OptionNotespeedID.Speed5_0, 
				HiSpeedTapTouchID.SPEED_55 => data.NoteSpeed == OptionNotespeedID.Speed5_5, 
				HiSpeedTapTouchID.SPEED_60 => data.NoteSpeed == OptionNotespeedID.Speed6_0, 
				HiSpeedTapTouchID.SPEED_65 => data.NoteSpeed == OptionNotespeedID.Speed6_5, 
				HiSpeedTapTouchID.SPEED_70 => data.NoteSpeed == OptionNotespeedID.Speed7_0, 
				HiSpeedTapTouchID.SPEED_75 => data.NoteSpeed == OptionNotespeedID.Speed7_5, 
				HiSpeedTapTouchID.SPEED_80 => data.NoteSpeed == OptionNotespeedID.Speed8_0, 
				HiSpeedTapTouchID.SPEED_85 => data.NoteSpeed == OptionNotespeedID.Speed8_5, 
				HiSpeedTapTouchID.SPEED_90 => data.NoteSpeed == OptionNotespeedID.Speed9_0, 
				HiSpeedTapTouchID.SPEED_95 => data.NoteSpeed == OptionNotespeedID.Speed9_5, 
				HiSpeedTapTouchID.SPEED_100 => data.NoteSpeed == OptionNotespeedID.Speed10_0, 
				HiSpeedTapTouchID.SPEED_SONIC => data.NoteSpeed == OptionNotespeedID.Speed_Sonic, 
				_ => false, 
			};
		}

		private static bool FuncTouchSpeed1Sonic(AchieveTrackData data, ReleaseItemConditions cond)
		{
			return cond.hiSpeedTapTouchId switch
			{
				HiSpeedTapTouchID.SPEED_10 => data.TouchSpeed == OptionTouchspeedID.Speed1_0, 
				HiSpeedTapTouchID.SPEED_15 => data.TouchSpeed == OptionTouchspeedID.Speed1_5, 
				HiSpeedTapTouchID.SPEED_20 => data.TouchSpeed == OptionTouchspeedID.Speed2_0, 
				HiSpeedTapTouchID.SPEED_25 => data.TouchSpeed == OptionTouchspeedID.Speed2_5, 
				HiSpeedTapTouchID.SPEED_30 => data.TouchSpeed == OptionTouchspeedID.Speed3_0, 
				HiSpeedTapTouchID.SPEED_35 => data.TouchSpeed == OptionTouchspeedID.Speed3_5, 
				HiSpeedTapTouchID.SPEED_40 => data.TouchSpeed == OptionTouchspeedID.Speed4_0, 
				HiSpeedTapTouchID.SPEED_45 => data.TouchSpeed == OptionTouchspeedID.Speed4_5, 
				HiSpeedTapTouchID.SPEED_50 => data.TouchSpeed == OptionTouchspeedID.Speed5_0, 
				HiSpeedTapTouchID.SPEED_55 => data.TouchSpeed == OptionTouchspeedID.Speed5_5, 
				HiSpeedTapTouchID.SPEED_60 => data.TouchSpeed == OptionTouchspeedID.Speed6_0, 
				HiSpeedTapTouchID.SPEED_65 => data.TouchSpeed == OptionTouchspeedID.Speed6_5, 
				HiSpeedTapTouchID.SPEED_70 => data.TouchSpeed == OptionTouchspeedID.Speed7_0, 
				HiSpeedTapTouchID.SPEED_75 => data.TouchSpeed == OptionTouchspeedID.Speed7_5, 
				HiSpeedTapTouchID.SPEED_80 => data.TouchSpeed == OptionTouchspeedID.Speed8_0, 
				HiSpeedTapTouchID.SPEED_85 => data.TouchSpeed == OptionTouchspeedID.Speed8_5, 
				HiSpeedTapTouchID.SPEED_90 => data.TouchSpeed == OptionTouchspeedID.Speed9_0, 
				HiSpeedTapTouchID.SPEED_95 => data.TouchSpeed == OptionTouchspeedID.Speed9_5, 
				HiSpeedTapTouchID.SPEED_100 => data.TouchSpeed == OptionTouchspeedID.Speed10_0, 
				HiSpeedTapTouchID.SPEED_SONIC => data.TouchSpeed == OptionTouchspeedID.Speed_Sonic, 
				_ => false, 
			};
		}

		private static bool FuncSlideSpeedm1Speedp1(AchieveTrackData data, ReleaseItemConditions cond)
		{
			return cond.hiSpeedSlideId switch
			{
				HiSpeedSlideID.SPEED_m10 => data.SlideSpeed == OptionSlidespeedID.Fast1_0, 
				HiSpeedSlideID.SPEED_m09 => data.SlideSpeed == OptionSlidespeedID.Fast0_9, 
				HiSpeedSlideID.SPEED_m08 => data.SlideSpeed == OptionSlidespeedID.Fast0_8, 
				HiSpeedSlideID.SPEED_m07 => data.SlideSpeed == OptionSlidespeedID.Fast0_7, 
				HiSpeedSlideID.SPEED_m06 => data.SlideSpeed == OptionSlidespeedID.Fast0_6, 
				HiSpeedSlideID.SPEED_m05 => data.SlideSpeed == OptionSlidespeedID.Fast0_5, 
				HiSpeedSlideID.SPEED_m04 => data.SlideSpeed == OptionSlidespeedID.Fast0_4, 
				HiSpeedSlideID.SPEED_m03 => data.SlideSpeed == OptionSlidespeedID.Fast0_3, 
				HiSpeedSlideID.SPEED_m02 => data.SlideSpeed == OptionSlidespeedID.Fast0_2, 
				HiSpeedSlideID.SPEED_m01 => data.SlideSpeed == OptionSlidespeedID.Fast0_1, 
				HiSpeedSlideID.SPEED_00 => data.SlideSpeed == OptionSlidespeedID.Normal, 
				HiSpeedSlideID.SPEED_p01 => data.SlideSpeed == OptionSlidespeedID.Late0_1, 
				HiSpeedSlideID.SPEED_p02 => data.SlideSpeed == OptionSlidespeedID.Late0_2, 
				HiSpeedSlideID.SPEED_p03 => data.SlideSpeed == OptionSlidespeedID.Late0_3, 
				HiSpeedSlideID.SPEED_p04 => data.SlideSpeed == OptionSlidespeedID.Late0_4, 
				HiSpeedSlideID.SPEED_p05 => data.SlideSpeed == OptionSlidespeedID.Late0_5, 
				HiSpeedSlideID.SPEED_p06 => data.SlideSpeed == OptionSlidespeedID.Late0_6, 
				HiSpeedSlideID.SPEED_p07 => data.SlideSpeed == OptionSlidespeedID.Late0_7, 
				HiSpeedSlideID.SPEED_p08 => data.SlideSpeed == OptionSlidespeedID.Late0_8, 
				HiSpeedSlideID.SPEED_p09 => data.SlideSpeed == OptionSlidespeedID.Late0_9, 
				HiSpeedSlideID.SPEED_p10 => data.SlideSpeed == OptionSlidespeedID.Late1_0, 
				_ => false, 
			};
		}

		private static bool FuncRating(AchieveTrackData data, ReleaseItemConditions cond)
		{
			return data.Rating >= cond.param;
		}

		private static bool FuncRankWin(AchieveTrackData data, ReleaseItemConditions cond)
		{
			if (data.PlayerNum >= 2 && data.VsRank <= data.PlayerNum - 1)
			{
				return FuncRank(data, cond);
			}
			return false;
		}

		private static bool FuncRankLose(AchieveTrackData data, ReleaseItemConditions cond)
		{
			if (data.PlayerNum >= 2 && data.VsRank >= 2)
			{
				return FuncRank(data, cond);
			}
			return false;
		}

		private static bool FuncOtomodachiWin(AchieveTrackData data, ReleaseItemConditions cond)
		{
			if (data.OtomodatiID != 0L && data.OtomodatiID == (ulong)cond.otomodachiId.id)
			{
				return data.OtomodatiVsWin;
			}
			return false;
		}

		private static bool FuncOtomodachiLose(AchieveTrackData data, ReleaseItemConditions cond)
		{
			if (data.OtomodatiID != 0L && data.OtomodatiID == (ulong)cond.otomodachiId.id)
			{
				return !data.OtomodatiVsWin;
			}
			return false;
		}

		private static bool FuncDanni(AchieveTrackData data, ReleaseItemConditions cond)
		{
			return false;
		}
	}
}
