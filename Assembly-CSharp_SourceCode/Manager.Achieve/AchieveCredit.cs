using System;
using System.Diagnostics;
using System.Linq;
using DB;
using Manager.MaiStudio;

namespace Manager.Achieve
{
	public class AchieveCredit
	{
		private static readonly Func<AchieveCreditData, ReleaseItemConditions, bool>[] Commands = new Func<AchieveCreditData, ReleaseItemConditions, bool>[19]
		{
			FuncNone, FuncMusic, FuncFullCombo, FuncPeople, FuncRank, FuncCombo, FuncChain, FuncSync, FuncDifficulty, FuncTitle,
			FuncPlate, FuncIcon, FuncCharaSet, FuncSatellite, FuncTapSpeed1Sonic, FuncTouchSpeed1Sonic, FuncSlideSpeedm1Speedp1, FuncMusicGroup, FuncNPlay
		};

		public static bool Checks(int id, object arg, ReleaseConditions conds)
		{
			AchieveCreditData data = (AchieveCreditData)arg;
			return conds.GetItemConditions().All((ReleaseItemConditions cond) => Commands[(int)cond.kindCredit](data, cond));
		}

		[Conditional("_DEBUG_CHECK")]
		private static void DebugCheck(int id, AchieveCreditData data, ReleaseConditions conds)
		{
			conds.GetItemConditions().Select((ReleaseItemConditions i, int idx) => new
			{
				idx = idx,
				cmd = i.kindCredit,
				res = Commands[(int)i.kindCredit](data, i)
			}).ToArray();
		}

		private static bool FuncNone(AchieveCreditData data, ReleaseItemConditions cond)
		{
			return true;
		}

		private static bool FuncMusic(AchieveCreditData data, ReleaseItemConditions cond)
		{
			switch (cond.difficultyId)
			{
			case DifficultyID.BAS:
			case DifficultyID.ADV:
			case DifficultyID.EXP:
			case DifficultyID.MAS:
			case DifficultyID.REM:
			{
				if (!data.ScoreIdScores.TryGetValue(Util.ToScoreId(cond.musicId.id, (int)Util.ToMusicDifficultyID(cond.difficultyId)), out var value2))
				{
					return false;
				}
				return value2.Count((GameScoreList i) => Util.CheckCombo(i.ComboType, cond) && Util.CheckSync(i.SyncType, cond) && Util.CheckRank(GameManager.GetClearRank(i.Achivement), cond)) >= cond.param;
			}
			case DifficultyID.ANY:
			{
				if (!data.AnyScores.TryGetValue(cond.musicId.id, out var value))
				{
					return false;
				}
				if (value.playcount >= cond.param && Util.CheckCombo(value.combo, cond) && Util.CheckSync(value.sync, cond))
				{
					return Util.CheckRank(value.scoreRank, cond);
				}
				return false;
			}
			case DifficultyID.ALL:
				return false;
			default:
				return false;
			}
		}

		private static bool FuncFullCombo(AchieveCreditData data, ReleaseItemConditions cond)
		{
			return Util.CheckCombo(data.ComboType, cond);
		}

		private static bool FuncPeople(AchieveCreditData data, ReleaseItemConditions cond)
		{
			return data.PlayerNum >= cond.param;
		}

		private static bool FuncRank(AchieveCreditData data, ReleaseItemConditions cond)
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

		private static bool FuncCombo(AchieveCreditData data, ReleaseItemConditions cond)
		{
			return data.ComboNum >= cond.param;
		}

		private static bool FuncChain(AchieveCreditData data, ReleaseItemConditions cond)
		{
			return data.ChainNum >= cond.param;
		}

		private static bool FuncSync(AchieveCreditData data, ReleaseItemConditions cond)
		{
			return Util.CheckSync(data.SyncType, cond);
		}

		private static bool FuncDifficulty(AchieveCreditData data, ReleaseItemConditions cond)
		{
			switch (cond.difficultyId)
			{
			case DifficultyID.BAS:
				return data.Difficulty == MusicDifficultyID.Basic;
			case DifficultyID.ADV:
				return data.Difficulty == MusicDifficultyID.Advanced;
			case DifficultyID.EXP:
				return data.Difficulty == MusicDifficultyID.Expert;
			case DifficultyID.MAS:
				return data.Difficulty == MusicDifficultyID.Master;
			case DifficultyID.REM:
				return data.Difficulty == MusicDifficultyID.ReMaster;
			case DifficultyID.ANY:
			case DifficultyID.ALL:
				return true;
			default:
				return false;
			}
		}

		private static bool FuncTitle(AchieveCreditData data, ReleaseItemConditions cond)
		{
			return data.EquipTitle == cond.titleId.id;
		}

		private static bool FuncPlate(AchieveCreditData data, ReleaseItemConditions cond)
		{
			return data.EquipPlate == cond.plateId.id;
		}

		private static bool FuncIcon(AchieveCreditData data, ReleaseItemConditions cond)
		{
			return data.EquipIcon == cond.iconId.id;
		}

		private static bool FuncCharaSet(AchieveCreditData data, ReleaseItemConditions cond)
		{
			return data.CommonCharaIds.FirstOrDefault((int i) => i == cond.charaId.id) != 0;
		}

		private static bool FuncSatellite(AchieveCreditData data, ReleaseItemConditions cond)
		{
			return data.Satellite == (int)cond.satelliteId;
		}

		private static bool FuncTapSpeed1Sonic(AchieveCreditData data, ReleaseItemConditions cond)
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

		private static bool FuncTouchSpeed1Sonic(AchieveCreditData data, ReleaseItemConditions cond)
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

		private static bool FuncSlideSpeedm1Speedp1(AchieveCreditData data, ReleaseItemConditions cond)
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

		private static bool FuncMusicGroup(AchieveCreditData data, ReleaseItemConditions cond)
		{
			if (!data.MusicGroups.TryGetValue(cond.musicGroupId.id, out var value))
			{
				return false;
			}
			data.SConditions.difficultyId = cond.difficultyId;
			data.SConditions.param = 1;
			data.SConditions.fcId = cond.fcId;
			data.SConditions.syncId = cond.syncId;
			data.SConditions.rankId = cond.rankId;
			return value.All(delegate(int id)
			{
				data.SConditions.musicId.id = id;
				data.Conditions.Init(data.SConditions);
				return FuncMusic(data, data.Conditions);
			});
		}

		private static bool FuncNPlay(AchieveCreditData data, ReleaseItemConditions cond)
		{
			return data.TrackNum >= cond.param;
		}
	}
}
