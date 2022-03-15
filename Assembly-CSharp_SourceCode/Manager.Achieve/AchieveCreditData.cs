using System.Collections.Generic;
using System.Linq;
using DB;
using MAI2.Util;
using Manager.MaiStudio;
using Manager.MaiStudio.Serialize;
using Manager.UserDatas;

namespace Manager.Achieve
{
	public class AchieveCreditData
	{
		public Dictionary<int, GameScoreList[]> MusicIdScores;

		public Dictionary<int, GameScoreList[]> ScoreIdScores;

		public Dictionary<int, UserScore> AnyScores;

		public PlayComboflagID ComboType;

		public int PlayerNum;

		public MusicClearrankID Rank;

		public uint ComboNum;

		public uint ChainNum;

		public PlaySyncflagID SyncType;

		public MusicDifficultyID Difficulty;

		public int EquipTitle;

		public int EquipPlate;

		public int EquipIcon;

		public int EquipFrame;

		public int[] CommonCharaIds;

		public int Satellite;

		public OptionNotespeedID NoteSpeed;

		public OptionTouchspeedID TouchSpeed;

		public OptionSlidespeedID SlideSpeed;

		public int TrackNum;

		public Dictionary<int, int[]> MusicGroups;

		public Manager.MaiStudio.Serialize.ReleaseItemConditions SConditions;

		public Manager.MaiStudio.ReleaseItemConditions Conditions;

		public static AchieveCreditData Create(int index)
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(index);
			GameScoreList[] gameScores = Singleton<GamePlayManager>.Instance.GetGameScores(index);
			int scoresLen = gameScores.Length;
			AchieveCreditData achieveCreditData = new AchieveCreditData
			{
				ComboType = gameScores.Min((GameScoreList i) => i.ComboType),
				PlayerNum = gameScores.Min((GameScoreList i) => Singleton<GamePlayManager>.Instance.GetPlayerIgnoreNpcNum(i.TrackNo - 1)),
				Rank = gameScores.Min((GameScoreList i) => GameManager.GetClearRank(i.Achivement)),
				ComboNum = gameScores.Min((GameScoreList i) => i.Combo),
				ChainNum = gameScores.Min((GameScoreList i) => i.Chain),
				SyncType = gameScores.Min((GameScoreList i) => i.SyncType),
				Difficulty = MusicDifficultyID.Invalid,
				EquipTitle = userData.Detail.EquipTitleID,
				EquipPlate = userData.Detail.EquipPlateID,
				EquipIcon = userData.Detail.EquipIconID,
				EquipFrame = userData.Detail.EquipFrameID,
				Satellite = index,
				NoteSpeed = OptionNotespeedID.Invalid,
				TouchSpeed = OptionTouchspeedID.Invalid,
				SlideSpeed = OptionSlidespeedID.Invalid,
				TrackNum = scoresLen,
				SConditions = new Manager.MaiStudio.Serialize.ReleaseItemConditions(),
				Conditions = new Manager.MaiStudio.ReleaseItemConditions()
			};
			achieveCreditData.MusicIdScores = (from i in gameScores
				group i by i.SessionInfo.musicId).ToDictionary((IGrouping<int, GameScoreList> i) => i.Key, (IGrouping<int, GameScoreList> i) => i.ToArray());
			achieveCreditData.ScoreIdScores = (from i in gameScores
				group i by Util.ToScoreId(i.SessionInfo.musicId, i.SessionInfo.difficulty)).ToDictionary((IGrouping<int, GameScoreList> i) => i.Key, (IGrouping<int, GameScoreList> i) => i.ToArray());
			achieveCreditData.AnyScores = achieveCreditData.MusicIdScores.ToDictionary((KeyValuePair<int, GameScoreList[]> i) => i.Key, delegate(KeyValuePair<int, GameScoreList[]> i)
			{
				GameScoreList[] array = i.Value.ToArray();
				return new UserScore(i.Key)
				{
					playcount = (uint)array.Length,
					combo = array.Max((GameScoreList j) => j.ComboType),
					sync = array.Max((GameScoreList j) => j.SyncType)
				};
			});
			achieveCreditData.MusicGroups = Singleton<DataManager>.Instance.GetMusicGroups().ToDictionary((KeyValuePair<int, Manager.MaiStudio.MusicGroupData> i) => i.Key, (KeyValuePair<int, Manager.MaiStudio.MusicGroupData> i) => i.Value.MusicIds.list.Select((Manager.MaiStudio.StringID j) => j.id).ToArray());
			if (gameScores.Select((GameScoreList i) => i.SessionInfo.difficulty).Distinct().Count() == 1)
			{
				achieveCreditData.Difficulty = (MusicDifficultyID)gameScores.First().SessionInfo.difficulty;
			}
			achieveCreditData.CommonCharaIds = (from id in gameScores.SelectMany((GameScoreList i) => i.CharaSlot)
				where id != 0
				group id by id into g
				where g.Count() == scoresLen
				select g.Key).ToArray();
			if (gameScores.All((GameScoreList i) => i.UserOption != null))
			{
				if (gameScores.Select((GameScoreList i) => i.UserOption.GetNoteSpeed).Distinct().Count() == 1)
				{
					achieveCreditData.NoteSpeed = gameScores.First().UserOption.GetNoteSpeed;
				}
				if (gameScores.Select((GameScoreList i) => i.UserOption.GetTouchSpeed).Distinct().Count() == 1)
				{
					achieveCreditData.TouchSpeed = gameScores.First().UserOption.GetTouchSpeed;
				}
				if (gameScores.Select((GameScoreList i) => i.UserOption.SlideSpeed).Distinct().Count() == 1)
				{
					achieveCreditData.SlideSpeed = gameScores.First().UserOption.SlideSpeed;
				}
			}
			return achieveCreditData;
		}
	}
}
