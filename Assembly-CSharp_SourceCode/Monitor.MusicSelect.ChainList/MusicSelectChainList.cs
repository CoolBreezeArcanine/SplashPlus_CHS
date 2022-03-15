using System.Collections;
using System.Collections.Generic;
using DB;
using MAI2.Util;
using Manager;
using Manager.MaiStudio;
using Manager.UserDatas;
using Process;
using UI.DaisyChainList;
using UnityEngine;
using Util;

namespace Monitor.MusicSelect.ChainList
{
	public class MusicSelectChainList : MusicSelectDaisyChainList
	{
		public enum LockInfoType
		{
			LockMaster,
			LockReMaster,
			DisableScore,
			MatchingDisable,
			WaitMatching,
			LockChallenge
		}

		private bool _isDifficultySelect;

		private Coroutine _corutine;

		private Sprite[] _difficultyBackSpries;

		private Sprite[] _difficultyMiniBackSprites;

		private Sprite[] _difficultyTagDeluxeSprite;

		private Sprite[] _difficultyTagStandardSprite;

		private Sprite[] _rankTagAddDeluxeSprite;

		private Sprite[] _rankTagAddStandardSprite;

		private Dictionary<int, Sprite> _rankingGenre;

		private string GetDifficultySortName(MusicDifficultyID difficulty)
		{
			string result = "";
			switch (difficulty)
			{
			case MusicDifficultyID.Basic:
				result = "BSC";
				break;
			case MusicDifficultyID.Advanced:
				result = "ADV";
				break;
			case MusicDifficultyID.Expert:
				result = "EXP";
				break;
			case MusicDifficultyID.Master:
				result = "MST";
				break;
			case MusicDifficultyID.ReMaster:
				result = "MST_Re";
				break;
			case MusicDifficultyID.Strong:
				result = "TYI";
				break;
			}
			return result;
		}

		private string GetRankTagName(MusicClearrankID type)
		{
			string result = "";
			switch (type)
			{
			case MusicClearrankID.Rank_S:
			case MusicClearrankID.Rank_SP:
				result = "S";
				break;
			case MusicClearrankID.Rank_SS:
			case MusicClearrankID.Rank_SSP:
				result = "SS";
				break;
			case MusicClearrankID.Rank_SSS:
			case MusicClearrankID.Rank_SSSP:
				result = "SSS";
				break;
			}
			return result;
		}

		public override void AdvancedInitialize(IMusicSelectProcess selectProcess, AssetManager manager, int index)
		{
			_difficultyBackSpries = new Sprite[6];
			_difficultyMiniBackSprites = new Sprite[6];
			_difficultyTagDeluxeSprite = new Sprite[6];
			_difficultyTagStandardSprite = new Sprite[6];
			_rankTagAddDeluxeSprite = new Sprite[14];
			_rankTagAddStandardSprite = new Sprite[14];
			_rankingGenre = new Dictionary<int, Sprite>();
			_rankingGenre.Clear();
			int num = 0;
			for (MusicDifficultyID musicDifficultyID = MusicDifficultyID.Basic; musicDifficultyID < MusicDifficultyID.End; musicDifficultyID++)
			{
				_difficultyBackSpries[num] = Resources.Load<Sprite>("Process/MusicSelect/Sprites/MusicSelect/MusicPlate/UI_MSS_MBase_" + GetDifficultySortName(musicDifficultyID));
				_difficultyMiniBackSprites[num] = Resources.Load<Sprite>("Process/MusicSelect/Sprites/MusicSelect/MusicPlate/UI_MSS_MBase_" + GetDifficultySortName(musicDifficultyID) + "_Mini");
				_difficultyTagDeluxeSprite[num] = Resources.Load<Sprite>("Process/MusicSelect/Sprites/MusicSelect/MusicPlate/UI_MSS_MBase_" + GetDifficultySortName(musicDifficultyID) + "_Tab_01");
				_difficultyTagStandardSprite[num] = Resources.Load<Sprite>("Process/MusicSelect/Sprites/MusicSelect/MusicPlate/UI_MSS_MBase_" + GetDifficultySortName(musicDifficultyID) + "_Tab_02");
				num++;
			}
			for (MusicClearrankID musicClearrankID = MusicClearrankID.Rank_D; musicClearrankID < MusicClearrankID.End; musicClearrankID++)
			{
				string rankTagName = GetRankTagName(musicClearrankID);
				if (rankTagName != "")
				{
					_rankTagAddDeluxeSprite[(int)musicClearrankID] = Resources.Load<Sprite>("Process/MusicSelect/Sprites/MusicSelect/MusicPlate/UI_MSS_Clear_" + rankTagName + "_Tab_01");
					_rankTagAddStandardSprite[(int)musicClearrankID] = Resources.Load<Sprite>("Process/MusicSelect/Sprites/MusicSelect/MusicPlate/UI_MSS_Clear_" + rankTagName + "_Tab_02");
				}
				else
				{
					_rankTagAddDeluxeSprite[(int)musicClearrankID] = null;
					_rankTagAddStandardSprite[(int)musicClearrankID] = null;
				}
			}
			foreach (KeyValuePair<int, MusicGenreData> musicGenre in Singleton<DataManager>.Instance.GetMusicGenres())
			{
				_rankingGenre.Add(musicGenre.Key, Resources.Load<Sprite>("Common/Sprites/Tab/Title/" + musicGenre.Value.FileName));
			}
			base.AdvancedInitialize(selectProcess, manager, index);
		}

		public Texture2D GetJacket()
		{
			if (SpotArray[4] != null && SpotArray[4] is MusicChainCardObejct)
			{
				return ((MusicChainCardObejct)SpotArray[4]).GetJacket();
			}
			return null;
		}

		public override void SetScrollCard(bool isVisible)
		{
			if (isVisible)
			{
				MusicChainCardObejct musicChainCardObejct = (MusicChainCardObejct)ScrollChainCard;
				musicChainCardObejct.ChangeSize(isMainActive: true);
				SetChainData(musicChainCardObejct, 0, SelectProcess.IsMusicSeq());
			}
			base.SetScrollCard(isVisible);
		}

		public void SetChainData(MusicChainCardObejct card, int index, bool isKindDoubleView, int difficulty = -1, bool changeAnimation = false)
		{
			MusicSelectProcess.CombineMusicSelectData combineMusic = SelectProcess.GetCombineMusic(index);
			MusicSelectProcess.MusicSelectData musicSelectData = combineMusic.musicSelectData[(int)SelectProcess.ScoreType];
			MusicSelectProcess.MusicSelectDetailData msDetailData = combineMusic.msDetailData;
			card.ResetChain();
			bool flag = SelectProcess.IsExtraFolder(index);
			bool flag2 = SelectProcess.IsGhostFolder(index);
			bool flag3 = SelectProcess.IsRatingFolder(index);
			bool flag4 = SelectProcess.IsMapTaskFolder(index);
			bool flag5 = SelectProcess.IsChallengeFolder(index);
			bool flag6 = SelectProcess.IsTournamentFolder(index);
			bool flag7 = false;
			int scoreAttackRankingId = SelectProcess.GetTournamentRankingID(index);
			bool isRanking = false;
			int score = 0;
			int ranking = -1;
			if (flag6)
			{
				int count = Singleton<ScoreRankingManager>.Instance.getSrDataForMS(scoreAttackRankingId).MusicInfoList.Count;
				MusicSelectProcess.GenreSelectData genreSelectDataForMusicIndex = SelectProcess.GetGenreSelectDataForMusicIndex(MonitorIndex, index);
				if (genreSelectDataForMusicIndex != null && count == genreSelectDataForMusicIndex.totalMusicNum)
				{
					isRanking = true;
					UserScoreRanking userScoreRanking = Singleton<UserDataManager>.Instance.GetUserData(MonitorIndex).ScoreRankingList.Find((UserScoreRanking i) => i.tournamentId == scoreAttackRankingId);
					if (userScoreRanking != null)
					{
						score = (int)userScoreRanking.totalScore;
						ranking = userScoreRanking.ranking;
					}
				}
			}
			if (difficulty < 0)
			{
				difficulty = ((!flag) ? (SelectProcess.IsLevelTab(index) ? SelectProcess.GetDifficultyByLevel(index) : SelectProcess.GetCurrentDifficulty(MonitorIndex)) : musicSelectData.Difficulty);
			}
			if (combineMusic.GetID(SelectProcess.ScoreType) == 2)
			{
				SetWaitConnectData(card, index, difficulty);
				return;
			}
			if (!SelectProcess.IsRemasterEnable(index) && difficulty == 4)
			{
				difficulty--;
			}
			int num = -1;
			uint dxScore = 0u;
			uint dxScoreMax = 0u;
			MusicClearrankID musicClearrankID = MusicClearrankID.Invalid;
			PlaySyncflagID sync = PlaySyncflagID.None;
			PlayComboflagID scoreAchivment = PlayComboflagID.None;
			UserScore userScore = SelectProcess.GetUserScore(MonitorIndex, difficulty, musicSelectData.MusicData.name.id);
			if (userScore != null)
			{
				num = (int)userScore.achivement;
				musicClearrankID = GameManager.GetClearRank(num);
				dxScore = userScore.deluxscore;
				scoreAchivment = userScore.combo;
				sync = userScore.sync;
				dxScoreMax = (uint)(musicSelectData.ScoreData[difficulty].maxNotes * 3);
			}
			NotesWrapper notesWrapper = Singleton<NotesListManager>.Instance.GetNotesList()[musicSelectData.MusicData.GetID()];
			string thumbnailName = musicSelectData.MusicData.thumbnailName;
			int level = musicSelectData.ScoreData[difficulty].level;
			card.SetMusicData(musicSelectData.MusicData.name.str, musicSelectData.MusicData.artistName.str, musicSelectData.ScoreData[difficulty].notesDesigner.str, musicSelectData.MusicData.bpm, AssetManager.GetJacketThumbTexture2D(thumbnailName), difficulty);
			card.SetScoreAchivement(musicClearrankID, num, dxScore, dxScoreMax, scoreAchivment, sync);
			card.SetCopyright((string.Empty != musicSelectData.MusicData.rightFile) ? AssetManager.GetRightTexture2D(musicSelectData.MusicData.rightFile) : null);
			card.SetScoreAttackRankingInfo(flag6, isRanking, score, ranking);
			card.SetLevel(level, (MusicLevelID)musicSelectData.ScoreData[difficulty].musicLevelID, difficulty);
			card.SetDifficulty(_difficultyBackSpries[difficulty], _difficultyTagDeluxeSprite[difficulty], _difficultyTagStandardSprite[difficulty], _difficultyMiniBackSprites[difficulty]);
			int num2 = (int)((musicClearrankID != MusicClearrankID.Invalid) ? musicClearrankID : MusicClearrankID.Rank_D);
			card.SetTagAddRank(_rankTagAddDeluxeSprite[num2], _rankTagAddStandardSprite[num2]);
			int id = musicSelectData.MusicData.name.id;
			bool flag8 = false;
			bool isUnlock = notesWrapper.IsNeedUnlock[MonitorIndex][difficulty];
			bool isRating = notesWrapper.IsRating[MonitorIndex][difficulty];
			bool flag9 = id >= 10000 && id < 20000;
			bool deluxeExist = combineMusic.existDeluxeScore;
			bool standardExist = combineMusic.existStandardScore;
			bool isAnimation = changeAnimation;
			_ = notesWrapper.Ranking;
			int id2 = musicSelectData.MusicData.genreName.id;
			Color bgColor = Utility.ConvertColor(Singleton<DataManager>.Instance.GetMusicGenre(id2).Color);
			int startLife = msDetailData.startLife;
			for (int j = 0; j < notesWrapper.IsMapBonus.Count; j++)
			{
				flag8 |= notesWrapper.IsMapBonus[j];
			}
			if (!isKindDoubleView)
			{
				if (flag9)
				{
					standardExist = false;
				}
				else
				{
					deluxeExist = false;
				}
			}
			card.SetScoreKind(flag9, deluxeExist, standardExist, isAnimation);
			bool isExist = notesWrapper.IsEnable[4];
			bool isUnlock2 = notesWrapper.IsUnlock[4];
			if (difficulty == 3)
			{
				card.SetRemasterExistIcon(isExist, isUnlock2);
			}
			else
			{
				card.SetRemasterExistIcon(isExist: false, isUnlock: false);
			}
			if (SelectProcess.GetMusicCategoryNameFromMusicIndex(index).Contains(Singleton<DataManager>.Instance.GetMusicGenre(2).genreName))
			{
				card.SetRanking(_rankingGenre[id2], bgColor);
			}
			else
			{
				card.SetRanking(null, Color.white);
			}
			if (flag5)
			{
				card.SetChallengeView(active: true);
				card.SetChallengeLife(startLife);
			}
			else
			{
				card.SetChallengeView(active: false);
			}
			card.SetNewLable(notesWrapper.IsNewMusic);
			card.SetBonus(flag8);
			card.SetLabel(isUnlock, isRating);
			if (flag2)
			{
				int extraPlayer = SelectProcess.GetExtraPlayer(index);
				GhostManager.GhostType ghostType = SelectProcess.GetGhostType(index);
				bool flag10 = musicSelectData.Difficulty <= difficulty;
				card.SetGhost(flag10, (int)ghostType);
				card.SetPlayerPlate(flag10, extraPlayer);
				UserGhost ghostToEnum = Singleton<GhostManager>.Instance.GetGhostToEnum(musicSelectData.GhostTarget);
				if (ghostToEnum != null)
				{
					if (ghostType == GhostManager.GhostType.Map && flag10)
					{
						int iconId = ghostToEnum.IconId;
						card.SetGhost(Resources.Load<Sprite>("Process/MusicSelect/Sprites/MusicSelect/MusicPlate/NPC/UI_MSS_GhostB_Icon_" + iconId.ToString("000000")));
					}
					MusicClearrankID clearRank = GameManager.GetClearRank(ghostToEnum.Achievement);
					card.SetGhostRank(flag10, clearRank);
				}
				else
				{
					card.SetGhostRank(isVisisble: false, MusicClearrankID.Invalid);
				}
			}
			else if (flag3)
			{
				card.SetGhost(isVisisble: false, 0);
				card.SetGhostRank(isVisisble: false, MusicClearrankID.Invalid);
				card.SetPlayerPlate(difficulty == musicSelectData.Difficulty, musicSelectData.RatingPlayer);
			}
			else if (flag4 || flag5)
			{
				card.SetGhost(isVisisble: false, 0);
				card.SetGhostRank(isVisisble: false, MusicClearrankID.Invalid);
				int playerIndex = 0;
				int playerIndex2 = -1;
				if (flag4)
				{
					if (notesWrapper.IsMapTaskMusic[0])
					{
						if (notesWrapper.IsMapTaskMusic[1])
						{
							playerIndex2 = 1;
						}
					}
					else
					{
						playerIndex = 1;
					}
				}
				else if (flag5)
				{
					if (notesWrapper.ChallengeDetail[0].isEnable)
					{
						if (notesWrapper.ChallengeDetail[1].isEnable)
						{
							playerIndex2 = 1;
						}
					}
					else
					{
						playerIndex = 1;
					}
				}
				card.SetPlayerPlate(isVisible: true, playerIndex, playerIndex2);
				if (notesWrapper.ChallengeDetail[MonitorIndex].isEnable)
				{
					flag7 = true;
				}
			}
			else
			{
				card.SetGhost(isVisisble: false, 0);
				card.SetGhostRank(isVisisble: false, MusicClearrankID.Invalid);
				card.SetPlayerPlate(isVisible: false, 0);
			}
			if (!notesWrapper.IsEnable[difficulty])
			{
				card.SetDisableScore(2);
			}
			else if (flag5)
			{
				if (flag7)
				{
					card.SetLockScore(5);
				}
			}
			else
			{
				if (flag4 || notesWrapper.IsUnlock[difficulty])
				{
					return;
				}
				switch (difficulty)
				{
				case 3:
					if (flag2)
					{
						if (musicSelectData.Difficulty < difficulty)
						{
							card.SetLockScore(0);
						}
					}
					else
					{
						card.SetLockScore(0);
					}
					break;
				case 4:
					if (flag2)
					{
						if (musicSelectData.Difficulty < difficulty)
						{
							card.SetLockScore(0);
						}
					}
					else
					{
						card.SetLockScore(1);
					}
					break;
				}
			}
		}

		public void SetWaitConnectData(MusicChainCardObejct card, int index, int difficulty)
		{
			card.SetMusicData("", "", "", 0, null, 0);
			card.SetScoreAchivement(MusicClearrankID.Invalid, 0, 0u, 0u, PlayComboflagID.None, PlaySyncflagID.None);
			card.SetCopyright(null);
			card.SetScoreAttackRankingInfo(isEnable: false);
			card.SetLevel(0, MusicLevelID.None, difficulty);
			card.SetDifficulty(_difficultyBackSpries[difficulty], _difficultyTagDeluxeSprite[difficulty], _difficultyTagStandardSprite[difficulty], _difficultyMiniBackSprites[difficulty]);
			card.SetTagAddRank(_rankTagAddDeluxeSprite[0], _rankTagAddStandardSprite[0]);
			card.SetScoreKind(isDeluxe: false, deluxeExist: false, standardExist: false, isAnimation: false);
			card.SetRemasterExistIcon(isExist: false, isUnlock: false);
			card.SetRanking(null, Color.white);
			card.SetNewLable(isVisible: false);
			card.SetBonus(isVisible: false);
			card.SetChallengeView(active: false);
			card.SetGhost(isVisisble: false, 0);
			card.SetGhostRank(isVisisble: false, MusicClearrankID.Invalid);
			card.SetPlayerPlate(isVisible: false, 0);
			if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.gameSetting.MachineGroupID != MachineGroupID.OFF)
			{
				card.SetDisableScore(4);
			}
			else
			{
				card.SetDisableScore(3);
			}
		}

		public void DeployMusicList(bool isAnimation = true, bool isChangeAnimation = false)
		{
			_isDifficultySelect = false;
			if (CurrentCoroutine != null)
			{
				StopCoroutine(CurrentCoroutine);
				CurrentCoroutine = null;
			}
			if (_corutine != null)
			{
				StopCoroutine(_corutine);
				_corutine = null;
			}
			RemoveAll();
			int num = 0;
			int overCount;
			for (int i = 0; i < 4; i++)
			{
				int diffIndex = -4 + i;
				if (SelectProcess.IsMusicBoundary(diffIndex, out overCount))
				{
					num++;
				}
			}
			int num2 = -4 + num;
			for (int j = 0; j < 9; j++)
			{
				int diffIndex2 = -4 + j;
				if (SelectProcess.IsMusicBoundary(diffIndex2, out overCount))
				{
					if (overCount == -2)
					{
						overCount++;
					}
					string categoryName = SelectProcess.GetCategoryName(MonitorIndex, overCount - 1);
					string categoryName2 = SelectProcess.GetCategoryName(MonitorIndex, overCount);
					SetSpot(j, GetSeparate(categoryName2, categoryName));
				}
				else
				{
					MusicChainCardObejct chain = GetChain<MusicChainCardObejct>();
					SetChainData(chain, num2, isKindDoubleView: true, -1, isChangeAnimation);
					chain.ChangeSize(j == 4);
					SetSpot(j, chain);
					num2++;
				}
			}
			Positioning(isImmediate: true, isAnimation);
			base.IsListEnable = true;
		}

		public void DeployDifficulty(MusicDifficultyID difficulty)
		{
			if (_corutine == null)
			{
				_corutine = StartCoroutine(DifficultyCoroutine(difficulty));
			}
		}

		private IEnumerator DifficultyCoroutine(MusicDifficultyID difficulty)
		{
			yield return RemoveOutCoroutine();
			DeployDifficultyList(difficulty);
			yield return new WaitForSeconds(0.28f);
			Play();
			_corutine = null;
		}

		public void DeployDifficultyList(MusicDifficultyID centerDifficulty)
		{
			RemoveAll();
			_isDifficultySelect = true;
			int num = (int)(4 - centerDifficulty);
			for (MusicDifficultyID musicDifficultyID = MusicDifficultyID.Basic; musicDifficultyID <= MusicDifficultyID.ReMaster; musicDifficultyID++)
			{
				if (SelectProcess.IsRemasterEnable() || musicDifficultyID != MusicDifficultyID.ReMaster)
				{
					MusicChainCardObejct chain = GetChain<MusicChainCardObejct>();
					SetChainData(chain, 0, isKindDoubleView: false, (int)musicDifficultyID);
					chain.ChangeSize(num == 4);
					SetSpot(num, chain);
					num++;
				}
			}
			Positioning(isImmediate: true, isAnimation: true);
			base.IsListEnable = true;
		}

		protected override void Next(int targetIndex, Direction direction)
		{
			if (_isDifficultySelect)
			{
				return;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < 9; i++)
			{
				if (SpotArray[i] is SeparateChainObject)
				{
					if (i < 4)
					{
						num3++;
					}
					if (4 == i)
					{
						num2 = -1;
					}
					if (i < 4 && direction == Direction.Right)
					{
						num++;
					}
					else if (i > 4 && direction == Direction.Left)
					{
						num--;
					}
				}
			}
			int index = ((direction != Direction.Right) ? 8 : 0);
			if (SelectProcess.IsMusicBoundary((4 + num2) * (int)direction, out var overCount))
			{
				if (direction == Direction.Right && overCount <= -2)
				{
					overCount++;
				}
				string categoryName = SelectProcess.GetCategoryName(MonitorIndex, overCount - 1);
				string categoryName2 = SelectProcess.GetCategoryName(MonitorIndex, overCount);
				SetSpot(index, GetSeparate(categoryName2, categoryName));
			}
			else
			{
				MusicChainCardObejct chain = GetChain<MusicChainCardObejct>();
				SetChainData(chain, targetIndex + num, SelectProcess.IsMusicSeq());
				chain.ChangeSize(isMainActive: false);
				SetSpot(index, chain);
			}
		}

		public Sprite GetDifficultyBackSprite(int index)
		{
			return _difficultyBackSpries[index];
		}

		public Sprite GetDifficultyMiniBackSprite(int index)
		{
			return _difficultyMiniBackSprites[index];
		}

		public Sprite GetDifficultyTagDeluxeSprite(int index)
		{
			return _difficultyTagDeluxeSprite[index];
		}

		public Sprite GetDifficultyTagStandardSprite(int index)
		{
			return _difficultyTagStandardSprite[index];
		}

		public Sprite GetRankTagAddDeluxeSprite(int index)
		{
			if (index < 0)
			{
				index = 0;
			}
			return _rankTagAddDeluxeSprite[index];
		}

		public Sprite GetRankTagAddStandardSprite(int index)
		{
			if (index < 0)
			{
				index = 0;
			}
			return _rankTagAddStandardSprite[index];
		}

		public Sprite GetRankingGenreSprite(int index)
		{
			if (_rankingGenre.ContainsKey(index))
			{
				return _rankingGenre[index];
			}
			return null;
		}
	}
}
