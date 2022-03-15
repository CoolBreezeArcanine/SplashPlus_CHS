using System;
using System.Collections.Generic;
using Datas.DebugData;
using DB;
using Game;
using IO;
using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_Partner_000001;
using MAI2System;
using Manager;
using Manager.MaiStudio;
using Manager.UserDatas;
using Monitor;
using Monitor.Result;
using Process.GhostResult;
using UnityEngine;

namespace Process
{
	public class ResultProcess : ProcessBase
	{
		public enum ResultSequence
		{
			Init,
			Start,
			Staging,
			Convention,
			Update,
			GhostResult,
			Release
		}

		public enum ResultStagingSequence
		{
			Init,
			Staging,
			SyncWait,
			End
		}

		private readonly string[] _derakkumaMessages = new string[5]
		{
			CommonMessageID.ResultKumaMessage01.GetName(),
			CommonMessageID.ResultKumaMessage02.GetName(),
			CommonMessageID.ResultKumaMessage03.GetName(),
			CommonMessageID.ResultKumaMessage04.GetName(),
			CommonMessageID.ResultKumaMessage05.GetName()
		};

		private ResultSequence _sequence;

		private float _timer;

		private bool _isConvention;

		private ResultMonitor[] _monitors;

		private GameObject _leftInstance;

		private GameObject _rightInstance;

		private UserData[] _userData;

		private UserScore[] _userScores;

		private GameScoreList[] _gameScoreLists;

		private GameScoreList[,] _conventionScores;

		private DebugGameScoreListData[] _debugGameScore;

		private int _musicID;

		private bool _isSinglePlay;

		private float[] _waitTimers;

		private float[] _touchWaitTimers;

		private uint[] _rateAchivementScore;

		private int[] _touchCounter;

		private bool[] _isNewRecord;

		private bool[] _isDetilsShow;

		private bool[] _isButtonActive;

		private bool[] _isNextWait;

		private bool _isDebug;

		private int[] _prevHighestRating;

		private GhostResultProcess _ghostResultProcess;

		private FriendBattleResultProcess _battleResultProcess;

		private bool _isGhostResult;

		private bool _initiative;

		public ResultStagingSequence[] StagingSequence { get; private set; }

		public ResultProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public override void OnAddProcess()
		{
		}

		public ResultStagingSequence GetStagingSequence(int index)
		{
			return StagingSequence[index];
		}

		public override void OnStart()
		{
			GameObject prefs = Resources.Load<GameObject>("Process/Result/ResultProcess");
			_initiative = true;
			_leftInstance = CreateInstanceAndSetParent(prefs, container.LeftMonitor);
			_rightInstance = CreateInstanceAndSetParent(prefs, container.RightMonitor);
			DebugGameScoreList debugGameScoreList = null;
			for (int i = 0; i < 2; i++)
			{
				debugGameScoreList = Singleton<GamePlayManager>.Instance.GetDebugGameScore(i);
				if (debugGameScoreList != null)
				{
					break;
				}
			}
			if (debugGameScoreList == null)
			{
				_musicID = GameManager.SelectMusicID[0];
			}
			else
			{
				_musicID = debugGameScoreList.GameScoreData[GameManager.MusicTrackNumber - 1].Score.id;
			}
			MusicData music = Singleton<DataManager>.Instance.GetMusic(_musicID);
			Texture2D jacketThumbTexture2D = container.assetManager.GetJacketThumbTexture2D(music.thumbnailName);
			_monitors = new ResultMonitor[2]
			{
				_leftInstance.GetComponent<ResultMonitor>(),
				_rightInstance.GetComponent<ResultMonitor>()
			};
			_userData = new UserData[4];
			_userScores = new UserScore[4];
			_gameScoreLists = new GameScoreList[4];
			_debugGameScore = new DebugGameScoreListData[4];
			_conventionScores = new GameScoreList[_monitors.Length, 4];
			StagingSequence = new ResultStagingSequence[2];
			_rateAchivementScore = new uint[2];
			_isNewRecord = new bool[2];
			_isDetilsShow = new bool[2];
			_isGhostResult = false;
			_isNextWait = new bool[_monitors.Length];
			_prevHighestRating = new int[2];
			_isButtonActive = new bool[_monitors.Length];
			_touchCounter = new int[_monitors.Length];
			_waitTimers = new float[2];
			_touchWaitTimers = new float[2];
			for (int j = 0; j < 4; j++)
			{
				DebugGameScoreList debugGameScore = Singleton<GamePlayManager>.Instance.GetDebugGameScore(j);
				if (debugGameScore == null)
				{
					_userScores[j] = new UserScore(_musicID)
					{
						achivement = Singleton<GamePlayManager>.Instance.GetAchivement(j),
						combo = Singleton<GamePlayManager>.Instance.GetComboType(j),
						sync = Singleton<GamePlayManager>.Instance.GetSyncType(j),
						deluxscore = Singleton<GamePlayManager>.Instance.GetDeluxeScore(j),
						scoreRank = Singleton<GamePlayManager>.Instance.GetClearRank(j)
					};
					_gameScoreLists[j] = Singleton<GamePlayManager>.Instance.GetGameScore(j);
					continue;
				}
				_isDebug = true;
				int num = j;
				_gameScoreLists[num] = debugGameScore.GetGameScoreList(num, (int)(GameManager.MusicTrackNumber - 1));
				debugGameScore = Singleton<GamePlayManager>.Instance.GetDebugGameScore(num);
				DebugGameScoreListData debugGameScoreListData = debugGameScore.GameScoreData[GameManager.MusicTrackNumber - 1];
				GameManager.SelectMusicID[num] = debugGameScoreListData.Score.id;
				GameManager.SelectDifficultyID[num] = (int)debugGameScoreListData.Difficulty;
				_userScores[j] = debugGameScoreListData.Score;
				for (int k = 0; k < 4; k++)
				{
					DebugGameScoreListData debugGameScoreListData2 = debugGameScore.GameScoreData[k];
					_conventionScores[j, k] = debugGameScoreListData2.GetGameScoreList(j);
				}
				Singleton<GamePlayManager>.Instance.DebugSetGameScore(num, _gameScoreLists[num]);
				int id = debugGameScoreListData.Score.id;
				int difficulty = (int)debugGameScoreListData.Difficulty;
				Singleton<GamePlayManager>.Instance.GetGameScore(num).SessionInfo = new SessionInfo
				{
					notesData = Singleton<DataManager>.Instance.GetMusic(id).notesData[difficulty],
					musicId = debugGameScoreListData.Score.id,
					difficulty = (int)debugGameScoreListData.Difficulty
				};
				if (num < 2)
				{
					_isDetilsShow[num] = debugGameScore.IsDetaile;
				}
				_debugGameScore[num] = debugGameScoreListData;
			}
			_isConvention = GameManager.IsEventMode;
			_isSinglePlay = Singleton<GamePlayManager>.Instance.GetPlayerIgnoreNpcNum() <= 1;
			for (int l = 0; l < _monitors.Length; l++)
			{
				StagingSequence[l] = ResultStagingSequence.Init;
				_userData[l] = Singleton<UserDataManager>.Instance.GetUserData(l);
				_monitors[l].Initialize(l, _userData[l].IsEntry);
				_touchCounter[l] = 1;
				if (_debugGameScore[l] == null)
				{
					_isDetilsShow[l] = GameManager.IsSelectResultDetails[l];
				}
				_isDetilsShow[l] = _userData[l].Extend.SelectResultDetails;
				if (!_userData[l].IsEntry)
				{
					StagingSequence[l] = ResultStagingSequence.End;
					_isNextWait[l] = true;
					_monitors[l].SetDisable();
					continue;
				}
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20020, l, false));
				_isNextWait[l] = false;
				_monitors[l].SetDerakkumaMessages(_derakkumaMessages);
				int num2 = GameManager.SelectDifficultyID[l];
				Notes notes = null;
				MusicDifficultyID musicDifficultyID = (MusicDifficultyID)num2;
				if ((uint)musicDifficultyID <= 4u)
				{
					notes = Singleton<DataManager>.Instance.GetMusic(_musicID).notesData[num2];
				}
				int level = notes?.level ?? 0;
				int levelID = notes?.musicLevelID ?? 0;
				_monitors[l].SetMusicData(music.name.str, GameManager.MusicTrackNumber, jacketThumbTexture2D);
				_monitors[l].SetLevel(level, (MusicLevelID)levelID, num2);
				_rateAchivementScore[l] = 0u;
				uint rating = _userData[l].Detail.Rating;
				_ = _userData[l].Detail.GradeRating;
				_ = _userData[l].Detail.MusicRating;
				_ = Singleton<GamePlayManager>.Instance.GetGameScore(l).PreDan;
				uint preClassValue = Singleton<GamePlayManager>.Instance.GetGameScore(l).PreClassValue;
				_prevHighestRating[l] = (int)_userData[l].Detail.HighestRating;
				_userData[l].UpdateScore(_musicID, num2, _userScores[l].achivement, (uint)music.version);
				if (GameManager.SelectGhostID[l] != GhostManager.GhostTarget.End)
				{
					bool vsGhostWin = Singleton<GamePlayManager>.Instance.GetVsGhostWin(l);
					int classValue = (int)Singleton<GamePlayManager>.Instance.GetGhostScore().ClassValue;
					bool isBoss = (l == 0 && GameManager.SelectGhostID[l] == GhostManager.GhostTarget.BossGhost_1P) || (l == 1 && GameManager.SelectGhostID[l] == GhostManager.GhostTarget.BossGhost_2P);
					_userData[l].RatingList.Udemae.UpdateResult(vsGhostWin, classValue, Singleton<GamePlayManager>.Instance.GetGhostScore().IsNpc(), isBoss);
				}
				_userData[l].UpdateUserRate();
				uint rating2 = _userData[l].Detail.Rating;
				uint gradeRating = _userData[l].Detail.GradeRating;
				uint musicRating = _userData[l].Detail.MusicRating;
				int classValue2 = _userData[l].RatingList.Udemae.ClassValue;
				UserUdemae.GetRateToUdemaeID((int)preClassValue);
				UdemaeID rateToUdemaeID = UserUdemae.GetRateToUdemaeID(classValue2);
				Singleton<GamePlayManager>.Instance.GetGameScore(l).SetPlayAfterRate((int)musicRating, (int)gradeRating, rateToUdemaeID, classValue2);
				if (CheckGhostUpload(l))
				{
					_userData[l].CreateGhost(l);
				}
				int arrowState = ((rating2 <= rating) ? ((rating2 >= rating) ? 1 : 2) : 0);
				bool isColorChange = GetRateingColorID(rating) < GetRateingColorID(rating2);
				Sprite prevPlate = Resources.Load<Sprite>("Common/Sprites/DXRating/UI_CMN_DXRating_" + GetRatePlate(rating));
				Sprite currentPlate = Resources.Load<Sprite>("Common/Sprites/DXRating/UI_CMN_DXRating_" + GetRatePlate(rating2));
				_monitors[l].SetDxRate(rating2, (int)rating, (int)(rating2 - rating), arrowState, currentPlate, prevPlate, isColorChange);
				Resources.Load<Sprite>($"Common/Sprites/Dani/UI_CMN_MatchLevel_{(int)(rateToUdemaeID + 1):00}");
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30007, l, rating2));
				ConstParameter.ScoreKind scoreKind = GameManager.GetScoreKind(_musicID);
				_monitors[l].SetGameScoreType(scoreKind);
				UserScore userScore = _userData[l].ScoreList[num2].Find((UserScore item) => item.id == _musicID);
				uint bestScore = 0u;
				uint diffScore = 0u;
				int num3 = Singleton<GamePlayManager>.Instance.GetGameScore(l).SessionInfo.notesData.maxNotes * 3;
				int percent = 0;
				if (num3 > 0)
				{
					percent = (int)(_userScores[l].deluxscore * 100) / num3;
				}
				DeluxcorerankrateID deluxcoreRank = GameManager.GetDeluxcoreRank(percent);
				bool isChallenge = Singleton<GamePlayManager>.Instance.GetGameScore(l).IsChallenge;
				uint life = Singleton<GamePlayManager>.Instance.GetGameScore(l).Life;
				_monitors[l].SetActivePerfectChallenge(isChallenge);
				_monitors[l].SetPerfectChallenge((int)life, life != 0);
				if (userScore == null)
				{
					_monitors[l].SetDxScore(_userScores[l].deluxscore, (int)_userScores[l].deluxscore, num3, deluxcoreRank);
					_userScores[l].playcount++;
					_userData[l].ScoreList[num2].Add(_userScores[l]);
					if (_userScores[l].achivement != 0)
					{
						_isNewRecord[l] = true;
						bestScore = 0u;
						diffScore = _userScores[l].achivement;
					}
				}
				else
				{
					int deluxscore = (int)userScore.deluxscore;
					int fluctuation = (int)_userScores[l].deluxscore - deluxscore;
					if (_isDebug)
					{
						deluxscore = (int)_debugGameScore[l].DxScore;
						fluctuation = (int)_userScores[l].deluxscore - deluxscore;
					}
					_monitors[l].SetDxScore(_userScores[l].deluxscore, fluctuation, num3, deluxcoreRank);
					if (userScore.achivement < _userScores[l].achivement)
					{
						diffScore = _userScores[l].achivement - userScore.achivement;
						bestScore = userScore.achivement;
						userScore.achivement = _userScores[l].achivement;
						userScore.scoreRank = GameManager.GetClearRank((int)userScore.achivement);
						_isNewRecord[l] = true;
					}
					else
					{
						diffScore = userScore.achivement - _userScores[l].achivement;
						bestScore = userScore.achivement;
						_isNewRecord[l] = false;
					}
					if (userScore.combo < _userScores[l].combo)
					{
						userScore.combo = _userScores[l].combo;
					}
					if (userScore.sync < _userScores[l].sync)
					{
						userScore.sync = _userScores[l].sync;
					}
					if (userScore.deluxscore < _userScores[l].deluxscore)
					{
						userScore.deluxscore = _userScores[l].deluxscore;
					}
					userScore.playcount++;
				}
				_monitors[l].SetMyBestAchievement(bestScore, diffScore, _isNewRecord[l]);
				bool flag = _userData[l].Option.DispJudge.IsCritical();
				_monitors[l].SetActiveCriticalScore(flag);
				NoteScore.EScoreType[] array = (NoteScore.EScoreType[])Enum.GetValues(typeof(NoteScore.EScoreType));
				foreach (NoteScore.EScoreType eScoreType in array)
				{
					if (eScoreType != NoteScore.EScoreType.End)
					{
						uint num4 = _gameScoreLists[l].GetJudgeTotalNum(eScoreType);
						float perfect = 0f;
						float critical = 0f;
						float great = 0f;
						float good = 0f;
						uint num5 = _gameScoreLists[l].GetJudgeNum(eScoreType, NoteJudge.JudgeBox.Critical);
						uint num6 = _gameScoreLists[l].GetJudgeNum(eScoreType, NoteJudge.JudgeBox.Perfect);
						uint num7 = _gameScoreLists[l].GetJudgeNum(eScoreType, NoteJudge.JudgeBox.Great);
						uint num8 = _gameScoreLists[l].GetJudgeNum(eScoreType, NoteJudge.JudgeBox.Good);
						uint miss = _gameScoreLists[l].GetJudgeNum(eScoreType, NoteJudge.JudgeBox.Miss);
						if (0 < num4)
						{
							perfect = (float)(num5 + num6) / (float)num4;
							critical = (float)num5 / (float)num4;
							great = (float)num7 / (float)num4;
							good = (float)num8 / (float)num4;
						}
						if (_isDebug)
						{
							critical = _debugGameScore[l].GetNoteScore(eScoreType, NoteJudge.ETiming.Critical);
							perfect = _debugGameScore[l].GetNoteScore(eScoreType, NoteJudge.ETiming.LatePerfect);
							great = _debugGameScore[l].GetNoteScore(eScoreType, NoteJudge.ETiming.LateGreat);
							good = _debugGameScore[l].GetNoteScore(eScoreType, NoteJudge.ETiming.LateGood);
							num4 = _debugGameScore[l].GetCount(eScoreType);
							num5 = _debugGameScore[l].GetScore(eScoreType, NoteJudge.JudgeBox.Critical);
							num6 = _debugGameScore[l].GetScore(eScoreType, NoteJudge.JudgeBox.Perfect);
							num7 = _debugGameScore[l].GetScore(eScoreType, NoteJudge.JudgeBox.Great);
							num8 = _debugGameScore[l].GetScore(eScoreType, NoteJudge.JudgeBox.Good);
							miss = _debugGameScore[l].GetScore(eScoreType, NoteJudge.JudgeBox.Miss);
						}
						if (!flag)
						{
							num6 += num5;
							num5 = 0u;
						}
						if (eScoreType == NoteScore.EScoreType.Break)
						{
							num5 = _gameScoreLists[l].GetJudgeBreakNum(NoteJudge.JudgeBox.Critical);
							num6 = _gameScoreLists[l].GetJudgeBreakNum(NoteJudge.JudgeBox.Perfect);
						}
						_monitors[l].SetScoreData(eScoreType, num5, num6, num7, num8, miss);
						if (!flag)
						{
							critical = 0f;
						}
						_monitors[l].SetScoreGauge(eScoreType, perfect, critical, great, good, num4);
					}
				}
				if (_isDebug)
				{
					_monitors[l].SetTotalScoreDetils(_debugGameScore[l].Critical, _debugGameScore[l].Perfect, _debugGameScore[l].Great, _debugGameScore[l].Good, _debugGameScore[l].Miss);
				}
				else
				{
					_monitors[l].SetTotalScoreDetils(_gameScoreLists[l].CriticalNum, _gameScoreLists[l].PerfectNum, _gameScoreLists[l].GreatNum, _gameScoreLists[l].GoodNum, _gameScoreLists[l].MissNum);
				}
				_monitors[l].SetVisibleFastLate(isVisible: true);
				_monitors[l].SetFastLate(_gameScoreLists[l].Fast, _gameScoreLists[l].Late);
				_monitors[l].SetAchievementRate(_userScores[l].achivement);
				for (int n = 0; n < 4; n++)
				{
					if (n != l)
					{
						GameScoreList gameScore = Singleton<GamePlayManager>.Instance.GetGameScore(n);
						if (gameScore.IsEnable && gameScore.IsHuman())
						{
							Singleton<GamePlayManager>.Instance.GetGameScore(l).AddRival((int)gameScore.UserID, gameScore.UserName, gameScore.SessionInfo.difficulty);
						}
					}
				}
			}
			if (_isConvention)
			{
				int[] array2 = new int[_monitors.Length];
				int[] array3 = new int[_monitors.Length];
				int num9 = 0;
				for (int num10 = 0; num10 < _monitors.Length; num10++)
				{
					if (!_userData[num10].IsEntry)
					{
						continue;
					}
					_monitors[num10].InitConventionResultWindow((int)GameManager.GetMaxTrackCount());
					for (int num11 = 0; num11 < 4; num11++)
					{
						DebugGameScoreList debugGameScore2 = Singleton<GamePlayManager>.Instance.GetDebugGameScore(num10);
						if (num11 < Singleton<GamePlayManager>.Instance.GetScoreListCount() && debugGameScore2 == null)
						{
							_conventionScores[num10, num11] = Singleton<GamePlayManager>.Instance.GetGameScore(num10, num11);
						}
						else if (debugGameScore2 == null)
						{
							_conventionScores[num10, num11] = null;
						}
						if (_conventionScores[num10, num11] != null)
						{
							int musicId = _conventionScores[num10, num11].SessionInfo.musicId;
							MusicData music2 = Singleton<DataManager>.Instance.GetMusic(musicId);
							int difficulty2 = _conventionScores[num10, num11].SessionInfo.difficulty;
							Notes notes2 = music2.notesData[difficulty2];
							int num12 = (int)((debugGameScore2 == null) ? Singleton<GamePlayManager>.Instance.GetAchivement(num10, num11) : debugGameScore2.GameScoreData[num11].Score.achivement);
							ConstParameter.ScoreKind scoreKind2 = GameManager.GetScoreKind(musicId);
							int num13 = (int)((debugGameScore2 == null) ? Singleton<GamePlayManager>.Instance.GetDeluxeScore(num10, num11) : debugGameScore2.GameScoreData[num11].DxScore);
							int level2 = notes2.level;
							MusicLevelID musicLevelID = (MusicLevelID)notes2.musicLevelID;
							array2[num10] += num12;
							array3[num10] += num13;
							Texture2D jacketThumbTexture2D2 = container.assetManager.GetJacketThumbTexture2D(music2.thumbnailName);
							_monitors[num10].SetConventionData(num11, num12, num13, difficulty2, musicLevelID, level2, scoreKind2, music2.name.str, jacketThumbTexture2D2);
						}
						else
						{
							_monitors[num10].SetConventionTrack(num11);
						}
					}
					if (num9 < array2[num10])
					{
						num9 = array2[num10];
					}
					_monitors[num10].SetTotalConventionData((uint)array2[num10], array3[num10]);
				}
				for (int num14 = 0; num14 < _monitors.Length; num14++)
				{
					int num15 = 0;
					for (int num16 = 0; num16 < array2.Length; num16++)
					{
						if (num9 == array2[num14])
						{
							break;
						}
						if (array2[num14] < array2[num16])
						{
							num15++;
						}
					}
					_monitors[num14].SetTotalRankData(num15, array2[num14] - num9);
				}
			}
			for (int num17 = 0; num17 < _monitors.Length; num17++)
			{
				if (_gameScoreLists[num17] != null)
				{
					DebugGameScoreList debugGameScore3 = Singleton<GamePlayManager>.Instance.GetDebugGameScore(num17);
					int difficulty3 = GameManager.SelectDifficultyID[num17];
					if (_isDebug && debugGameScore3 != null)
					{
						difficulty3 = (int)debugGameScore3.GameScoreData[GameManager.MusicTrackNumber - 1].Difficulty;
					}
					MusicClearrankID clearRank = GameManager.GetClearRank((int)_userScores[num17].achivement);
					string name = clearRank.GetName();
					Animator clearRankOriginal = Resources.Load<Animator>("Process/Result/Prefabs/ClearRank/" + name);
					int equipPartnerID = Singleton<UserDataManager>.Instance.GetUserData(num17).Detail.EquipPartnerID;
					SoundManager.SetPartnerVoiceCue(num17, equipPartnerID);
					int id2 = Singleton<DataManager>.Instance.GetPartner(equipPartnerID).naviChara.id;
					_monitors[num17].SetStartData(clearRankOriginal, clearRank, difficulty3, _isDetilsShow[num17], _isSinglePlay, clearRank >= MusicClearrankID.Rank_A, id2);
					float rate = 0f;
					if (_gameScoreLists[num17].TheoryCombo != 0)
					{
						rate = (float)_gameScoreLists[num17].MaxCombo / (float)_gameScoreLists[num17].TheoryCombo;
					}
					_monitors[num17].SetCombo(_gameScoreLists[num17].MaxCombo, _gameScoreLists[num17].TheoryCombo, rate);
					MedalDisplayObject.MedalTarget medalTarget = ((!_isSinglePlay) ? MedalDisplayObject.MedalTarget.Sync : MedalDisplayObject.MedalTarget.Combo);
					uint toValue = (_isSinglePlay ? _gameScoreLists[num17].MaxCombo : _gameScoreLists[num17].MaxChain);
					_monitors[num17].SetMedalData(_userScores[num17].combo, _userScores[num17].sync, medalTarget, toValue);
					int level3 = music.notesData[GameManager.SelectDifficultyID[num17]].level * 10 + music.notesData[GameManager.SelectDifficultyID[num17]].levelDecimal;
					if (_userData[num17].Detail.HighestRating >= 1000)
					{
						int num18 = _prevHighestRating[num17] / 1000;
						int num19 = (int)_userData[num17].Detail.HighestRating / 1000;
						if (num19 - num18 > 0)
						{
							_userData[num17].Activity.DxRate(num19 * 1000);
						}
					}
					if (Singleton<GamePlayManager>.Instance.GetGameScore(num17).Dan > Singleton<GamePlayManager>.Instance.GetGameScore(num17).PreDan)
					{
						_userData[num17].Activity.ClassUp(Singleton<GamePlayManager>.Instance.GetGameScore(num17).Dan);
					}
					if (_userScores[num17].sync > PlaySyncflagID.None)
					{
						UserAct.ActivityCode code = (UserAct.ActivityCode)(_userScores[num17].sync + 39);
						switch (_userScores[num17].sync)
						{
						case PlaySyncflagID.ChainHi:
							code = UserAct.ActivityCode.FullSyncP;
							break;
						case PlaySyncflagID.SyncLow:
							code = UserAct.ActivityCode.FullSyncDx;
							break;
						case PlaySyncflagID.SyncHi:
							code = UserAct.ActivityCode.FullSyncDxP;
							break;
						}
						_userData[num17].Activity.MusicAchivement(_musicID, GameManager.SelectDifficultyID[num17], code, level3);
					}
					if (_userScores[num17].combo > PlayComboflagID.None)
					{
						UserAct.ActivityCode code2 = UserAct.ActivityCode.FullCombo;
						switch (_userScores[num17].combo)
						{
						case PlayComboflagID.Gold:
							code2 = UserAct.ActivityCode.FullComboP;
							break;
						case PlayComboflagID.AllPerfect:
							code2 = UserAct.ActivityCode.AllPerfect;
							break;
						case PlayComboflagID.AllPerfectPlus:
							code2 = UserAct.ActivityCode.AllPerfectP;
							break;
						}
						_userData[num17].Activity.MusicAchivement(_musicID, GameManager.SelectDifficultyID[num17], code2, level3);
					}
					if (clearRank >= MusicClearrankID.Rank_S)
					{
						_userData[num17].Activity.MusicAchivement(_musicID, GameManager.SelectDifficultyID[num17], clearRank switch
						{
							MusicClearrankID.Rank_S => UserAct.ActivityCode.RankS, 
							MusicClearrankID.Rank_SP => UserAct.ActivityCode.RankSP, 
							MusicClearrankID.Rank_SS => UserAct.ActivityCode.RankSS, 
							MusicClearrankID.Rank_SSP => UserAct.ActivityCode.RankSSP, 
							MusicClearrankID.Rank_SSS => UserAct.ActivityCode.RankSSS, 
							MusicClearrankID.Rank_SSSP => UserAct.ActivityCode.RankSSSP, 
							_ => UserAct.ActivityCode.RankS, 
						}, level3);
					}
					_userData[num17].Activity.PlayMusic(_musicID);
					int playerIgnoreNpcNum = Singleton<GamePlayManager>.Instance.GetPlayerIgnoreNpcNum();
					if (playerIgnoreNpcNum > 1)
					{
						SortedDictionary<int, List<int>> sortedDictionary = new SortedDictionary<int, List<int>>();
						_monitors[num17].InitMultiUserData(playerIgnoreNpcNum);
						for (int num20 = 0; num20 < 4; num20++)
						{
							GameScoreList gameScore2 = Singleton<GamePlayManager>.Instance.GetGameScore(num20);
							if (gameScore2.IsEnable && gameScore2.IsHuman())
							{
								int vsRank = (int)Singleton<GamePlayManager>.Instance.GetVsRank(num20);
								if (sortedDictionary.ContainsKey(vsRank))
								{
									sortedDictionary[vsRank].Add(num20);
									continue;
								}
								sortedDictionary.Add(vsRank, new List<int> { num20 });
							}
						}
						int num21 = 0;
						foreach (KeyValuePair<int, List<int>> item in sortedDictionary)
						{
							if (item.Value.Count > 1)
							{
								item.Value.Sort((int a, int b) => Singleton<GamePlayManager>.Instance.GetGameScore(a).PlayerIndex - Singleton<GamePlayManager>.Instance.GetGameScore(b).PlayerIndex);
							}
							foreach (int item2 in item.Value)
							{
								UserData userData = Singleton<UserDataManager>.Instance.GetUserData(item2);
								Texture2D iconTexture2D = container.assetManager.GetIconTexture2D(item2, userData.Detail.EquipIconID);
								Sprite userIcon = Sprite.Create(iconTexture2D, new Rect(0f, 0f, iconTexture2D.width, iconTexture2D.height), new Vector2(0.5f, 0.5f));
								MusicDifficultyID difficulty4 = (_isDebug ? Singleton<GamePlayManager>.Instance.GetDebugGameScore(item2).GameScoreData[GameManager.MusicTrackNumber - 1].Difficulty : ((MusicDifficultyID)GameManager.SelectDifficultyID[item2]));
								uint achivement = _userScores[item2].achivement;
								PlayComboflagID combo = _userScores[item2].combo;
								_monitors[num17].SetMultiUserData(num21, userData.Detail.UserName, userIcon, difficulty4, achivement, item.Key, combo, num17 == item2);
								num21++;
							}
						}
					}
					_monitors[num17].TimelineInterpretation();
				}
				if (_isSinglePlay)
				{
					_monitors[num17].SetActiveMultiPlayObject(isActive: false);
					continue;
				}
				_monitors[num17].SetActiveMultiPlayObject(isActive: true);
				_monitors[num17].SetRankOrder(Singleton<GamePlayManager>.Instance.GetVsRank(num17));
				int num22 = ((num17 != 0) ? 1 : 0);
				int num23 = ((num17 == 0) ? 1 : 0);
				float num24 = (float)_gameScoreLists[num22].MaxChain / (float)_gameScoreLists[num22].TheoryChain;
				float num25 = (float)_gameScoreLists[num23].MaxChain / (float)_gameScoreLists[num23].TheoryChain;
				_monitors[num17].SetSync(_gameScoreLists[num17].MaxChain, _gameScoreLists[num17].TheoryChain, num24 * 0.5f, num25 * 0.5f);
			}
			_isGhostResult = false;
			for (int num26 = 0; num26 < 2; num26++)
			{
				if (GameManager.SelectGhostID[num26] != GhostManager.GhostTarget.End)
				{
					_isGhostResult = true;
					break;
				}
			}
			if (_isGhostResult)
			{
				_battleResultProcess = new FriendBattleResultProcess(container, OnGhostResultProcessCallback);
				container.processManager.AddProcess(_battleResultProcess, 15);
			}
		}

		public override void OnUpdate()
		{
			LogicUpdate();
			ResultMonitor[] monitors = _monitors;
			for (int i = 0; i < monitors.Length; i++)
			{
				monitors[i].ViewUpdate();
			}
		}

		public override void OnLateUpdate()
		{
		}

		private void LogicUpdate()
		{
			switch (_sequence)
			{
			case ResultSequence.Init:
				if (_timer >= 100f)
				{
					for (int n = 0; n < _monitors.Length; n++)
					{
						if (!_userData[n].IsEntry || _gameScoreLists[n] == null)
						{
							continue;
						}
						MusicClearrankID clearRank = GameManager.GetClearRank((int)_userScores[n].achivement);
						SoundManager.StopBGM(n);
						if (!_isConvention)
						{
							if (clearRank >= MusicClearrankID.Rank_A)
							{
								SoundManager.PlayBGM(Mai2.Mai2Cue.Cue.BGM_RESULT_CLEAR, n);
							}
							else
							{
								SoundManager.PlayBGM(Mai2.Mai2Cue.Cue.BGM_RESULT, n);
							}
						}
					}
					_timer = 0f;
					container.processManager.NotificationFadeIn();
					if (_isGhostResult)
					{
						_initiative = false;
						_battleResultProcess.PlayStart();
						for (int num3 = 0; num3 < 2; num3++)
						{
							if (_userData[num3].IsEntry)
							{
								_monitors[num3].SetGhostResult();
							}
						}
						_sequence = ResultSequence.GhostResult;
					}
					else
					{
						_sequence = ResultSequence.Start;
					}
				}
				else
				{
					_timer += GameManager.GetGameMSecAdd();
				}
				break;
			case ResultSequence.Start:
			{
				_sequence = ResultSequence.Staging;
				for (int m = 0; m < _monitors.Length; m++)
				{
					if (_userData[m].IsEntry)
					{
						_isButtonActive[m] = false;
						_monitors[m].PlayStaging();
						_monitors[m].SetVisibleButton(InputManager.ButtonSetting.Button03, isVisisble: false);
						_monitors[m].SetVisibleButton(InputManager.ButtonSetting.Button04, isVisisble: false);
						StagingSequence[m] = ResultStagingSequence.Staging;
					}
				}
				_timer = 0f;
				break;
			}
			case ResultSequence.Staging:
			{
				for (int i = 0; i < _monitors.Length; i++)
				{
					if (_monitors[i].IsSkip() && !_isButtonActive[i] && !_isConvention)
					{
						_isButtonActive[i] = true;
						_monitors[i].SetVisibleButton(InputManager.ButtonSetting.Button03, isVisisble: true);
						_monitors[i].SetVisibleButton(InputManager.ButtonSetting.Button04, isVisisble: true);
					}
					if (_userData[i].IsEntry && _monitors[i].IsSkip() && StagingSequence[i] == ResultStagingSequence.Staging && !_isConvention)
					{
						if (InputManager.GetInputDown(i, InputManager.ButtonSetting.Button04, InputManager.TouchPanelArea.A4))
						{
							_monitors[i].PreseedButton(InputManager.ButtonSetting.Button04);
							SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_SKIP, i);
							_monitors[i].Skip();
							int num = ((i == 0) ? 1 : 0);
							if (_userData[num].IsEntry && StagingSequence[num] == ResultStagingSequence.Staging)
							{
								_monitors[i].SetVisibleButton(InputManager.ButtonSetting.Button04, isVisisble: false);
							}
							if (!_monitors[i].IsDXRatingUP)
							{
								StagingSequence[i] = ResultStagingSequence.SyncWait;
							}
							_waitTimers[i] = 100f;
							continue;
						}
						if (!_monitors[i].IsDXRatingUP && InputManager.GetInputDown(i, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3))
						{
							_monitors[i].PreseedButton(InputManager.ButtonSetting.Button03);
							_monitors[i].SetVisibleButton(InputManager.ButtonSetting.Button04, isVisisble: false);
							_monitors[i].Skip();
							StagingSequence[i] = ResultStagingSequence.SyncWait;
							_isDetilsShow[i] = !_isDetilsShow[i];
							_monitors[i].ChangeScoreBoard(_isDetilsShow[i]);
							_waitTimers[i] = 100f;
							continue;
						}
					}
					if (_monitors[i].IsStagingEnd() && StagingSequence[i] == ResultStagingSequence.Staging)
					{
						StagingSequence[i] = ResultStagingSequence.End;
					}
					if (StagingSequence[i] == ResultStagingSequence.SyncWait)
					{
						int num2 = ((i == 0) ? 1 : 0);
						if (StagingSequence[num2] != ResultStagingSequence.Staging)
						{
							StagingSequence[i] = ResultStagingSequence.End;
						}
					}
					if (_waitTimers[i] > 0f)
					{
						_waitTimers[i] -= GameManager.GetGameMSecAdd();
					}
					else if (StagingSequence[i] == ResultStagingSequence.SyncWait && InputManager.GetInputDown(i, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3))
					{
						_monitors[i].PreseedButton(InputManager.ButtonSetting.Button03);
						_isDetilsShow[i] = !_isDetilsShow[i];
						_monitors[i].ChangeScoreBoard(_isDetilsShow[i]);
						_waitTimers[i] = 100f;
					}
				}
				if (StagingSequence[0] != ResultStagingSequence.End || StagingSequence[1] != ResultStagingSequence.End)
				{
					break;
				}
				for (int j = 0; j < 2; j++)
				{
					if (!_isConvention)
					{
						_monitors[j].ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 5);
						_monitors[j].SetVisibleButton(InputManager.ButtonSetting.Button04, isVisisble: true);
						_monitors[j].SetVisibleButton(InputManager.ButtonSetting.Button03, isVisisble: true);
					}
					StagingSequence[j] = ResultStagingSequence.End;
				}
				if (!GameManager.IsEventMode)
				{
					if (GameManager.IsFreedomMode)
					{
						container.processManager.PrepareTimer(60, 0, isEntry: false, ToNextCheck, isVisible: false);
					}
					else
					{
						container.processManager.PrepareTimer(20, 0, isEntry: false, ToNextCheck, isVisible: false);
					}
				}
				if (_isConvention)
				{
					for (int k = 0; k < _monitors.Length; k++)
					{
						if (_userData[k].IsEntry)
						{
							_monitors[k].PlayConventionFadeIn((int)GameManager.MusicTrackNumber);
						}
					}
					_waitTimers[0] = 500f;
					_timer = 0f;
					_sequence = ResultSequence.Convention;
				}
				else
				{
					_sequence = ResultSequence.Update;
				}
				break;
			}
			case ResultSequence.Update:
			{
				for (int num4 = 0; num4 < _monitors.Length; num4++)
				{
					if (_waitTimers[num4] > 0f)
					{
						_waitTimers[num4] -= GameManager.GetGameMSecAdd();
					}
					else
					{
						if (!_userData[num4].IsEntry || _isNextWait[num4])
						{
							continue;
						}
						if (InputManager.GetButtonDown(num4, InputManager.ButtonSetting.Button04))
						{
							_isNextWait[num4] = true;
							bool flag = true;
							for (int num5 = 0; num5 < _monitors.Length; num5++)
							{
								if (!_isNextWait[num5])
								{
									flag = false;
									break;
								}
							}
							if (flag)
							{
								ToNextCheck();
							}
							else
							{
								_monitors[num4].SetNextWait();
								container.processManager.EnqueueMessage(num4, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
							}
							_monitors[num4].PreseedButton(InputManager.ButtonSetting.Button04);
							SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, num4);
						}
						else if (InputManager.GetInputDown(num4, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3))
						{
							_monitors[num4].PreseedButton(InputManager.ButtonSetting.Button03);
							_isDetilsShow[num4] = !_isDetilsShow[num4];
							_monitors[num4].ChangeScoreBoard(_isDetilsShow[num4]);
							_waitTimers[num4] = 100f;
						}
						if (_touchWaitTimers[num4] > 0f)
						{
							_touchWaitTimers[num4] -= GameManager.GetGameMSecAdd();
						}
						else if (InputManager.GetTouchPanelAreaDown(num4, InputManager.TouchPanelArea.B1, InputManager.TouchPanelArea.E2, InputManager.TouchPanelArea.B2, InputManager.TouchPanelArea.B3, InputManager.TouchPanelArea.E3, InputManager.TouchPanelArea.E4))
						{
							if (_touchCounter[num4] <= 0)
							{
								SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000252, num4);
								_monitors[num4].TouchPartnerCharacter();
								_waitTimers[num4] = 100f;
								_touchWaitTimers[num4] = 2500f;
								_touchCounter[num4] = 1;
							}
							else
							{
								_touchCounter[num4]--;
							}
						}
					}
				}
				break;
			}
			case ResultSequence.Convention:
				if (InputManager.GetButtonPush(0, InputManager.ButtonSetting.Select) && InputManager.GetButtonPush(1, InputManager.ButtonSetting.Select))
				{
					_timer += GameManager.GetGameMSecAdd();
					_waitTimers[0] += GameManager.GetGameMSecAdd();
					if (2000f <= _timer)
					{
						for (int l = 0; l < _monitors.Length; l++)
						{
							SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, l);
							_waitTimers[l] = 0f;
							if (_userData[l].IsEntry)
							{
								_monitors[l].PlayConventionFadeOut();
								_monitors[l].ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 5);
								_monitors[l].SetVisibleButton(InputManager.ButtonSetting.Button04, isVisisble: true);
								_monitors[l].SetVisibleButton(InputManager.ButtonSetting.Button03, isVisisble: true);
							}
						}
						_sequence = ResultSequence.Update;
					}
					else if (_waitTimers[0] >= 500f)
					{
						SoundManager.PlayGameSE(Mai2.Mai2Cue.Cue.SE_GAME_CLICK, 0, 1f);
						SoundManager.PlayGameSE(Mai2.Mai2Cue.Cue.SE_GAME_CLICK, 1, 1f);
						_waitTimers[0] = 0f;
					}
				}
				else
				{
					if (_timer > 0f)
					{
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CANCEL, 0);
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CANCEL, 1);
					}
					_timer = 0f;
				}
				break;
			case ResultSequence.GhostResult:
				if (_initiative)
				{
					_sequence = ResultSequence.Start;
				}
				break;
			case ResultSequence.Release:
				break;
			}
		}

		private void OnGhostResultProcessCallback()
		{
			_initiative = true;
		}

		public override void OnRelease()
		{
			UnityEngine.Object.Destroy(_leftInstance);
			UnityEngine.Object.Destroy(_rightInstance);
		}

		private void ToNextCheck()
		{
			if (_sequence != ResultSequence.Release)
			{
				container.processManager.ForceTimeUp();
				ToNextProcess();
				_sequence = ResultSequence.Release;
			}
		}

		private void ToNextProcess()
		{
			if (Singleton<SystemConfig>.Instance.config.IsMovieSupervision)
			{
				container.processManager.AddProcess(new FadeProcess(container, this, new MusicSelectProcess(container), FadeProcess.FadeType.Type2), 50);
			}
			else
			{
				container.processManager.AddProcess(new FadeProcess(container, this, new UnlockProcess(container)), 50);
			}
			if (_battleResultProcess != null)
			{
				container.processManager.ReleaseProcess(_battleResultProcess);
			}
			for (int i = 0; i < 2; i++)
			{
				UserDetail detail = Singleton<UserDataManager>.Instance.GetUserData(i).Detail;
				UserOption option = Singleton<UserDataManager>.Instance.GetUserData(i).Option;
				MessageUserInformationData messageUserInformationData = new MessageUserInformationData(i, container.assetManager, detail, option.DispRate, isSubMonitor: true);
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20011, i, messageUserInformationData));
			}
			for (int j = 0; j < 2; j++)
			{
				GameManager.IsSelectResultDetails[j] = _isDetilsShow[j];
				if (_monitors[j].IsActive())
				{
					MechaManager.LedIf[j].ButtonLedReset();
				}
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(j);
				if (userData.IsEntry)
				{
					userData.Extend.SelectResultDetails = GameManager.IsSelectResultDetails[j];
				}
			}
		}

		private bool CheckGhostUpload(int playerId)
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(playerId);
			GameScoreList gameScore = Singleton<GamePlayManager>.Instance.GetGameScore(playerId);
			int musicId = gameScore.SessionInfo.musicId;
			int difficulty = gameScore.SessionInfo.difficulty;
			if (difficulty > 4)
			{
				return false;
			}
			if (Singleton<GamePlayManager>.Instance.GetGameScore(playerId).IsTrackSkip)
			{
				return false;
			}
			if (!Singleton<GamePlayManager>.Instance.GetGameScore(playerId).IsClear)
			{
				return false;
			}
			if (GameManager.IsEventMode)
			{
				return false;
			}
			if (userData.Detail.PlayCount == 0)
			{
				return false;
			}
			UdemaeID rateToUdemaeID = UserUdemae.GetRateToUdemaeID(userData.RatingList.Udemae.ClassValue);
			int npcParamBoss = rateToUdemaeID.GetNpcParamBoss();
			UdemaeBossData udemaeBoss = Singleton<DataManager>.Instance.GetUdemaeBoss(npcParamBoss);
			if (udemaeBoss == null)
			{
				return false;
			}
			Notes notes = Singleton<DataManager>.Instance.GetMusic(musicId).notesData[difficulty];
			int num = notes.level * 10 + notes.levelDecimal;
			Notes notes2 = Singleton<DataManager>.Instance.GetMusic(udemaeBoss.music.id).notesData[udemaeBoss.difficulty.id];
			int achieve = udemaeBoss.achieve;
			int num2 = notes2.level * 10 + notes2.levelDecimal;
			int num3 = achieve - rateToUdemaeID.GetNpcFluctuateAchieveLower();
			int num4 = achieve + rateToUdemaeID.GetNpcFluctuateAchieveUpper();
			int num5 = num2 - rateToUdemaeID.GetNpcFluctuateLevelLower();
			int num6 = num2 + rateToUdemaeID.GetNpcFluctuateLevelUpper();
			int num7 = (int)(gameScore.Achivement * 10000m);
			if (num5 <= num && num <= num6 && num3 <= num7 && num7 <= num4)
			{
				return true;
			}
			return false;
		}

		public static string GetRatePlate(uint rating)
		{
			string result = "01";
			if (rating >= RateColorID.Rainbow1.GetRate())
			{
				result = "11";
			}
			else if (rating >= RateColorID.Platinum1.GetRate())
			{
				result = "10";
			}
			else if (rating >= RateColorID.Gold1.GetRate())
			{
				result = "09";
			}
			else if (rating >= RateColorID.Silver1.GetRate())
			{
				result = "08";
			}
			else if (rating >= RateColorID.Copper1.GetRate())
			{
				result = "07";
			}
			else if (rating >= RateColorID.Purple1.GetRate())
			{
				result = "06";
			}
			else if (rating >= RateColorID.Red1.GetRate())
			{
				result = "05";
			}
			else if (rating >= RateColorID.Yellow1.GetRate())
			{
				result = "04";
			}
			else if (rating >= RateColorID.Green1.GetRate())
			{
				result = "03";
			}
			else if (rating >= RateColorID.Blue1.GetRate())
			{
				result = "02";
			}
			return result;
		}

		public RateColorID GetRateingColorID(uint rating)
		{
			RateColorID result = RateColorID.White1;
			if (rating >= RateColorID.Rainbow1.GetRate())
			{
				result = RateColorID.Rainbow1;
			}
			else if (rating >= RateColorID.Platinum1.GetRate())
			{
				result = RateColorID.Platinum1;
			}
			else if (rating >= RateColorID.Gold1.GetRate())
			{
				result = RateColorID.Gold1;
			}
			else if (rating >= RateColorID.Silver1.GetRate())
			{
				result = RateColorID.Silver1;
			}
			else if (rating >= RateColorID.Copper1.GetRate())
			{
				result = RateColorID.Copper1;
			}
			else if (rating >= RateColorID.Purple1.GetRate())
			{
				result = RateColorID.Purple1;
			}
			else if (rating >= RateColorID.Red1.GetRate())
			{
				result = RateColorID.Red1;
			}
			else if (rating >= RateColorID.Yellow1.GetRate())
			{
				result = RateColorID.Yellow1;
			}
			else if (rating >= RateColorID.Green1.GetRate())
			{
				result = RateColorID.Green1;
			}
			else if (rating >= RateColorID.Blue1.GetRate())
			{
				result = RateColorID.Blue1;
			}
			return result;
		}
	}
}
