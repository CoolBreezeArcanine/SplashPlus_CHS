using System;
using System.Diagnostics;
using System.Linq;
using Manager.MaiStudio;
using Manager.UserDatas;

namespace Manager.Achieve
{
	public class AchieveTotal
	{
		private static readonly Func<AchieveTotalData, ReleaseItemConditions, bool>[] Commands = new Func<AchieveTotalData, ReleaseItemConditions, bool>[23]
		{
			FuncNone, FuncMai2DxNPlay, FuncMusic, FuncDifficulty, FuncTodohuken, FuncTenpo, FuncMusicGroup, FuncNPlay, FuncCharaKanyu, FuncCharaKaisu,
			FuncCharaKakusei, FuncMapCharaSiyou, FuncMapCharaKakusei, FuncMapKaikin, FuncMapComplete, FuncNpcOtomodachiNWin, FuncNpcOtomodachiNRenWin, FuncZenkokuOtomodachiNWin, FuncZenkokuOtomodachiNRenWin, FuncMapKyori,
			FuncLoginRenzoku, FuncLoginRuikei, FuncTodohukenSeiha
		};

		public static bool Checks(int id, object arg, ReleaseConditions conds)
		{
			AchieveTotalData data = (AchieveTotalData)arg;
			return conds.GetItemConditions().All((ReleaseItemConditions cond) => Commands[(int)cond.kindTotal](data, cond));
		}

		[Conditional("_DEBUG_CHECK")]
		private static void DebugCheck(int id, AchieveTotalData data, ReleaseConditions conds)
		{
			conds.GetItemConditions().Select((ReleaseItemConditions i, int idx) => new
			{
				idx = idx,
				cmd = i.kindTotal,
				res = Commands[(int)i.kindTotal](data, i)
			}).ToArray();
		}

		private static bool FuncNone(AchieveTotalData data, ReleaseItemConditions cond)
		{
			return true;
		}

		private static bool FuncMai2DxNPlay(AchieveTotalData data, ReleaseItemConditions cond)
		{
			return data.PlayCount >= cond.param;
		}

		private static bool FuncMusic(AchieveTotalData data, ReleaseItemConditions cond)
		{
			if (!data.Scores.TryGetValue(cond.musicId.id, out var value))
			{
				return false;
			}
			switch (cond.difficultyId)
			{
			case DifficultyID.BAS:
			case DifficultyID.ADV:
			case DifficultyID.EXP:
			case DifficultyID.MAS:
			case DifficultyID.REM:
			{
				UserScore userScore3 = value[(int)Util.ToMusicDifficultyID(cond.difficultyId)];
				if (userScore3 == null)
				{
					return false;
				}
				if (userScore3.playcount >= cond.param && Util.CheckCombo(userScore3.combo, cond) && Util.CheckSync(userScore3.sync, cond))
				{
					return Util.CheckRank(userScore3.scoreRank, cond);
				}
				return false;
			}
			case DifficultyID.ANY:
			{
				UserScore userScore2 = data.AnyScores[cond.musicId.id];
				if (userScore2.playcount >= cond.param && Util.CheckCombo(userScore2.combo, cond) && Util.CheckSync(userScore2.sync, cond))
				{
					return Util.CheckRank(userScore2.scoreRank, cond);
				}
				return false;
			}
			case DifficultyID.ALL:
			{
				for (int i = 0; i <= 3; i++)
				{
					UserScore userScore = value[i];
					if (userScore == null)
					{
						return false;
					}
					if (userScore.playcount < cond.param || !Util.CheckCombo(userScore.combo, cond) || !Util.CheckSync(userScore.sync, cond) || !Util.CheckRank(userScore.scoreRank, cond))
					{
						return false;
					}
				}
				return true;
			}
			default:
				return false;
			}
		}

		private static bool FuncDifficulty(AchieveTotalData data, ReleaseItemConditions cond)
		{
			return data.UserData.ScoreList[(int)Util.ToMusicDifficultyID(cond.difficultyId)].Any();
		}

		private static bool FuncTodohuken(AchieveTotalData data, ReleaseItemConditions cond)
		{
			return data.PlayRegions.ContainsKey((int)(cond.todohukenId + 1));
		}

		private static bool FuncTenpo(AchieveTotalData data, ReleaseItemConditions cond)
		{
			return data.Tenpo == cond.param;
		}

		private static bool FuncMusicGroup(AchieveTotalData data, ReleaseItemConditions cond)
		{
			if (!data.MusicGroups.TryGetValue(cond.musicGroupId.id, out var value))
			{
				return false;
			}
			data.SConditions.difficultyId = cond.difficultyId;
			data.SConditions.param = 0;
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

		private static bool FuncNPlay(AchieveTotalData data, ReleaseItemConditions cond)
		{
			return data.PlayTrackCount >= cond.param;
		}

		private static bool FuncCharaKanyu(AchieveTotalData data, ReleaseItemConditions cond)
		{
			return data.CharaList.ContainsKey(cond.charaId.id);
		}

		private static bool FuncCharaKaisu(AchieveTotalData data, ReleaseItemConditions cond)
		{
			if (data.CharaList.TryGetValue(cond.charaId.id, out var value))
			{
				return value.Count >= cond.param;
			}
			return false;
		}

		private static bool FuncCharaKakusei(AchieveTotalData data, ReleaseItemConditions cond)
		{
			if (data.CharaList.TryGetValue(cond.charaId.id, out var value))
			{
				return value.Awakening >= cond.param;
			}
			return false;
		}

		private static bool FuncMapCharaSiyou(AchieveTotalData data, ReleaseItemConditions cond)
		{
			if (data.MapCharaCounts.TryGetValue(cond.mapId.id, out var value))
			{
				return value >= cond.param;
			}
			return false;
		}

		private static bool FuncMapCharaKakusei(AchieveTotalData data, ReleaseItemConditions cond)
		{
			if (data.MapCharaAwakenings.TryGetValue(cond.mapId.id, out var value))
			{
				return value >= cond.param;
			}
			return false;
		}

		private static bool FuncMapKaikin(AchieveTotalData data, ReleaseItemConditions cond)
		{
			if (data.MapList.TryGetValue(cond.mapId.id, out var value))
			{
				return !value.IsLock;
			}
			return false;
		}

		private static bool FuncMapComplete(AchieveTotalData data, ReleaseItemConditions cond)
		{
			if (data.MapList.TryGetValue(cond.mapId.id, out var value))
			{
				return value.IsComplete;
			}
			return false;
		}

		private static bool FuncNpcOtomodachiNWin(AchieveTotalData data, ReleaseItemConditions cond)
		{
			return data.UserData.RatingList.Udemae.NpcTotalWinNum >= cond.param;
		}

		private static bool FuncNpcOtomodachiNRenWin(AchieveTotalData data, ReleaseItemConditions cond)
		{
			return data.UserData.RatingList.Udemae.NpcWinNum >= cond.param;
		}

		private static bool FuncZenkokuOtomodachiNWin(AchieveTotalData data, ReleaseItemConditions cond)
		{
			return data.UserData.RatingList.Udemae.TotalWinNum >= cond.param;
		}

		private static bool FuncZenkokuOtomodachiNRenWin(AchieveTotalData data, ReleaseItemConditions cond)
		{
			return data.UserData.RatingList.Udemae.WinNum >= cond.param;
		}

		private static bool FuncMapKyori(AchieveTotalData data, ReleaseItemConditions cond)
		{
			return data.MapDistance >= cond.param;
		}

		private static bool FuncLoginRenzoku(AchieveTotalData data, ReleaseItemConditions cond)
		{
			return data.ContinueLoginCount >= cond.param;
		}

		private static bool FuncLoginRuikei(AchieveTotalData data, ReleaseItemConditions cond)
		{
			return data.TotalLoginCount >= cond.param;
		}

		private static bool FuncTodohukenSeiha(AchieveTotalData data, ReleaseItemConditions cond)
		{
			return data.PlayRegionCount >= cond.param;
		}
	}
}
