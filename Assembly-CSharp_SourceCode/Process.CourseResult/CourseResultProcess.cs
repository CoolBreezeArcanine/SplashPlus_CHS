using Datas.DebugData;
using DB;
using Mai2.Mai2Cue;
using MAI2.Util;
using MAI2System;
using Manager;
using Manager.MaiStudio;
using Manager.UserDatas;
using Monitor.CourseResult;
using UnityEngine;

namespace Process.CourseResult
{
	public class CourseResultProcess : ProcessBase
	{
		private enum ProcessState : byte
		{
			Wait,
			Play,
			Release,
			Released
		}

		private enum SubProcessState
		{
			First_In,
			CourseResult_In,
			CourseResult_Wait,
			TrackResult_In,
			Clear_In,
			RankUp_In,
			RankUp_Wait,
			RankUp_Out,
			ItemGet_In,
			ItemGet_Wait,
			ItemGet_Out,
			ResultWait,
			OutWait,
			EndCheck,
			EndWait,
			End
		}

		public class CourseResultMusic
		{
			public int trackNo = -1;

			public bool isPlay;

			public string musicName = "";

			public Texture2D jacket;

			public ConstParameter.ScoreKind scoreType;

			public int difficultyId = -1;

			public int level = -1;

			public MusicLevelID levelId;

			public int achievement = -1;

			public int dxscore = -1;

			public int life = -1;

			public int totalAchievement = -1;

			public int totalDxscore = -1;
		}

		public class CourseResultTotal
		{
			public int courseMode = -1;

			public Sprite daniImage;

			public Sprite daniPlate;

			public int achievement = -1;

			public int dxscore = -1;

			public int startLife = -1;

			public int life = -1;

			public bool isRankUp;
		}

		private CourseResultMonitor[] _monitors;

		private ProcessState _state;

		private float[] _timer;

		private SubProcessState[] _subState;

		private bool[] _isRankUp;

		private bool[] _isTicketGet;

		public CourseResultProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public override void OnAddProcess()
		{
		}

		public override void OnRelease()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				Object.Destroy(_monitors[i].gameObject);
			}
		}

		public override void OnStart()
		{
			if (Singleton<GamePlayManager>.Instance.GetScoreListCount() == 0)
			{
				Singleton<CourseManager>.Instance.SetCourseId(2301);
			}
			GameObject prefs = Resources.Load<GameObject>("Process/Course/CourseResultProcess");
			_monitors = new CourseResultMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<CourseResultMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<CourseResultMonitor>()
			};
			_timer = new float[_monitors.Length];
			_subState = new SubProcessState[_monitors.Length];
			_isRankUp = new bool[_monitors.Length];
			_isTicketGet = new bool[_monitors.Length];
			for (int i = 0; i < _monitors.Length; i++)
			{
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(i);
				_monitors[i].Initialize(i, userData.IsEntry);
			}
			container.processManager.NotificationFadeIn();
			SetResultData();
		}

		public void SetResultData()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				int num = 0;
				int num2 = 0;
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(i);
				if (!userData.IsEntry)
				{
					continue;
				}
				CourseResultTotal courseResultTotal = new CourseResultTotal();
				for (int j = 0; (long)j < 4L; j++)
				{
					CourseResultMusic courseResultMusic = new CourseResultMusic();
					if (j < Singleton<GamePlayManager>.Instance.GetScoreListCount())
					{
						GameScoreList gameScore = Singleton<GamePlayManager>.Instance.GetGameScore(i, j);
						int musicId = gameScore.SessionInfo.musicId;
						MusicData music = Singleton<DataManager>.Instance.GetMusic(musicId);
						int difficulty = gameScore.SessionInfo.difficulty;
						Notes notes = music.notesData[difficulty];
						courseResultMusic.trackNo = j;
						courseResultMusic.isPlay = true;
						courseResultMusic.musicName = music.name.str;
						courseResultMusic.jacket = container.assetManager.GetJacketThumbTexture2D(music.thumbnailName);
						courseResultMusic.scoreType = GameManager.GetScoreKind(musicId);
						courseResultMusic.difficultyId = difficulty;
						courseResultMusic.level = notes.level;
						courseResultMusic.levelId = (MusicLevelID)notes.musicLevelID;
						courseResultMusic.achievement = (int)Singleton<GamePlayManager>.Instance.GetAchivement(i, j);
						courseResultMusic.dxscore = (int)gameScore.DxScore;
						courseResultMusic.life = Singleton<CourseManager>.Instance.GetTrackLife(i, j);
						num += courseResultMusic.achievement;
						num2 += courseResultMusic.dxscore;
						courseResultMusic.totalAchievement = num;
						courseResultMusic.totalDxscore = num2;
					}
					else if (Singleton<GamePlayManager>.Instance.GetDebugGameScore(i) != null)
					{
						int musicId2 = Singleton<CourseManager>.Instance.GetMusicId(j);
						MusicData music2 = Singleton<DataManager>.Instance.GetMusic(musicId2);
						int difficultyId = Singleton<CourseManager>.Instance.GetDifficultyId(j);
						Notes notes2 = music2.notesData[difficultyId];
						DebugGameScoreList debugGameScore = Singleton<GamePlayManager>.Instance.GetDebugGameScore(i);
						int perfect = (int)debugGameScore.GameScoreData[j].Perfect;
						int great = (int)debugGameScore.GameScoreData[j].Great;
						int good = (int)debugGameScore.GameScoreData[j].Good;
						int miss = (int)debugGameScore.GameScoreData[j].Miss;
						Singleton<CourseManager>.Instance.SetLifeCalc(i, perfect, great, good, miss, j + 1);
						courseResultMusic.trackNo = j;
						courseResultMusic.isPlay = true;
						courseResultMusic.musicName = music2.name.str;
						courseResultMusic.jacket = container.assetManager.GetJacketThumbTexture2D(music2.thumbnailName);
						courseResultMusic.scoreType = GameManager.GetScoreKind(musicId2);
						courseResultMusic.difficultyId = difficultyId;
						courseResultMusic.level = notes2.level;
						courseResultMusic.levelId = (MusicLevelID)notes2.musicLevelID;
						courseResultMusic.life = Singleton<CourseManager>.Instance.GetTrackLife(i, j);
						courseResultMusic.achievement = (int)debugGameScore.GameScoreData[j].Score.achivement;
						courseResultMusic.dxscore = (int)debugGameScore.GameScoreData[j].DxScore;
						num += courseResultMusic.achievement;
						num2 += courseResultMusic.dxscore;
						courseResultMusic.totalAchievement = num;
						courseResultMusic.totalDxscore = num2;
					}
					else
					{
						courseResultMusic.trackNo = j;
						courseResultMusic.isPlay = false;
					}
					_monitors[i].SetResultTrackData(j, courseResultMusic);
				}
				courseResultTotal.achievement = num;
				courseResultTotal.dxscore = num2;
				CourseData courseData = Singleton<CourseManager>.Instance.GetCourseData();
				courseResultTotal.courseMode = courseData.courseMode.id;
				string courseImage = Singleton<CourseManager>.Instance.GetCourseImage(CourseManager.ImageType.Title);
				string courseImage2 = Singleton<CourseManager>.Instance.GetCourseImage(CourseManager.ImageType.Plate);
				if (courseImage != "")
				{
					courseResultTotal.daniImage = Resources.Load<Sprite>(courseImage);
				}
				if (courseImage2 != "")
				{
					courseResultTotal.daniPlate = Resources.Load<Sprite>(courseImage2);
				}
				courseResultTotal.startLife = Singleton<CourseManager>.Instance.GetCourseData().life;
				courseResultTotal.life = Singleton<CourseManager>.Instance.GetRestLife(i);
				int courseRank = (int)userData.Detail.CourseRank;
				int id = courseData.baseDaniId.id;
				courseResultTotal.isRankUp = courseResultTotal.life != 0 && courseRank < id;
				if (courseResultTotal.isRankUp)
				{
					userData.Detail.CourseRank = (uint)id;
					userData.Activity.RankUp(id);
				}
				_isRankUp[i] = courseResultTotal.isRankUp;
				_monitors[i].SetResultTotalData(courseResultTotal);
				_isTicketGet[i] = courseResultTotal.life != 0 && Singleton<TicketManager>.Instance.GetCoursePlayTicketId(i) != -1;
				if (_isTicketGet[i])
				{
					Singleton<TicketManager>.Instance.SaveCourseTicketGet(i);
				}
				UserCourse userCourse = userData.CourseList.Find((UserCourse t) => t.courseId == Singleton<CourseManager>.Instance.GetCourseData().GetID());
				if (userCourse != null)
				{
					userCourse.playCount++;
					userCourse.lastPlayDate = TimeManager.GetNowDateString();
					userCourse.isLastClear = courseResultTotal.life != 0;
					if (courseResultTotal.life != 0 && userCourse.clearDate == "")
					{
						userCourse.clearDate = TimeManager.GetNowDateString();
					}
					bool flag = userCourse.totalRestlife != 0;
					bool flag2 = courseResultTotal.life != 0;
					bool flag3 = userCourse.totalAchievement < (uint)num;
					bool flag4 = false;
					if (!flag && flag2)
					{
						flag4 = true;
					}
					else if (flag2 && flag3)
					{
						flag4 = true;
					}
					else if (!flag && !flag2 && flag3)
					{
						flag4 = true;
					}
					if (flag4)
					{
						userCourse.totalAchievement = (uint)num;
						userCourse.totalDeluxscore = (uint)num2;
						userCourse.totalRestlife = (uint)courseResultTotal.life;
					}
					if (userCourse.bestAchievement < (uint)num)
					{
						userCourse.bestAchievement = (uint)num;
						userCourse.bestAchievementDate = TimeManager.GetNowDateString();
					}
					if (userCourse.bestDeluxscore < (uint)num2)
					{
						userCourse.bestDeluxscore = (uint)num2;
						userCourse.bestDeluxscoreDate = TimeManager.GetNowDateString();
					}
				}
				else
				{
					UserCourse userCourse2 = new UserCourse(Singleton<CourseManager>.Instance.GetCourseData().GetID());
					userCourse2.playCount = 1u;
					userCourse2.isLastClear = courseResultTotal.life != 0;
					userCourse2.lastPlayDate = TimeManager.GetNowDateString();
					if (courseResultTotal.life != 0)
					{
						userCourse2.clearDate = TimeManager.GetNowDateString();
					}
					else
					{
						userCourse2.clearDate = "";
					}
					userCourse2.totalAchievement = (uint)num;
					userCourse2.totalDeluxscore = (uint)num2;
					userCourse2.totalRestlife = (uint)courseResultTotal.life;
					userCourse2.bestAchievement = (uint)num;
					userCourse2.bestAchievementDate = TimeManager.GetNowDateString();
					userCourse2.bestDeluxscore = (uint)num2;
					userCourse2.bestDeluxscoreDate = TimeManager.GetNowDateString();
					userData.CourseList.Add(userCourse2);
				}
			}
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			switch (_state)
			{
			case ProcessState.Wait:
				SoundManager.PlayBGM(Cue.BGM_DANI_01, 2);
				_state = ProcessState.Play;
				break;
			case ProcessState.Play:
			{
				bool flag = true;
				for (int i = 0; i < _monitors.Length; i++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry)
					{
						SubUpdate(i);
						if (_subState[i] != SubProcessState.End)
						{
							flag = false;
						}
					}
				}
				if (flag)
				{
					_state = ProcessState.Release;
				}
				break;
			}
			case ProcessState.Release:
				if (Singleton<GamePlayManager>.Instance.IsPlayLog())
				{
					container.processManager.AddProcess(new FadeProcess(container, this, new PhotoEditProcess(container)), 50);
				}
				else if (GameManager.IsEventMode)
				{
					container.processManager.AddProcess(new FadeProcess(container, this, new DataSaveProcess(container)), 50);
				}
				else
				{
					container.processManager.AddProcess(new FadeProcess(container, this, new UnlockMusicProcess(container)), 50);
				}
				container.processManager.SetVisibleTimers(isVisible: false);
				_state = ProcessState.Released;
				break;
			case ProcessState.Released:
				break;
			}
		}

		private void SubUpdate(int playerIndex)
		{
			if (_timer[playerIndex] > 0f)
			{
				_timer[playerIndex] -= GameManager.GetGameMSecAdd();
			}
			switch (_subState[playerIndex])
			{
			case SubProcessState.First_In:
				_monitors[playerIndex].FirstFadeIn();
				_subState[playerIndex] = SubProcessState.CourseResult_In;
				_timer[playerIndex] = 2000f;
				break;
			case SubProcessState.CourseResult_In:
				if (_timer[playerIndex] <= 0f)
				{
					_monitors[playerIndex].MainResultFadeIn();
					_subState[playerIndex] = SubProcessState.TrackResult_In;
					_timer[playerIndex] = 2000f;
				}
				break;
			case SubProcessState.CourseResult_Wait:
				if (_timer[playerIndex] <= 0f)
				{
					_subState[playerIndex] = SubProcessState.TrackResult_In;
					_timer[playerIndex] = 1000f;
				}
				break;
			case SubProcessState.TrackResult_In:
				if (_timer[playerIndex] <= 0f)
				{
					_monitors[playerIndex].TrackFadeIn();
					_subState[playerIndex] = SubProcessState.Clear_In;
					_timer[playerIndex] = 2000f;
				}
				break;
			case SubProcessState.Clear_In:
				if (_timer[playerIndex] <= 0f)
				{
					_monitors[playerIndex].ClearFadeIn();
					if (_isRankUp[playerIndex])
					{
						_subState[playerIndex] = SubProcessState.RankUp_In;
						_timer[playerIndex] = 2000f;
					}
					else if (_isTicketGet[playerIndex])
					{
						_subState[playerIndex] = SubProcessState.ItemGet_In;
						_timer[playerIndex] = 1000f;
					}
					else
					{
						_subState[playerIndex] = SubProcessState.ResultWait;
						_timer[playerIndex] = 1000f;
					}
				}
				break;
			case SubProcessState.RankUp_In:
				if (_timer[playerIndex] <= 0f)
				{
					_monitors[playerIndex].RankUpEffectIn();
					_subState[playerIndex] = SubProcessState.RankUp_Wait;
					_timer[playerIndex] = 2000f;
				}
				break;
			case SubProcessState.RankUp_Wait:
				if (_timer[playerIndex] <= 0f)
				{
					_subState[playerIndex] = SubProcessState.RankUp_Out;
					_timer[playerIndex] = 20000f;
					_monitors[playerIndex].SetButtonIn();
				}
				break;
			case SubProcessState.RankUp_Out:
				if (_timer[playerIndex] <= 0f)
				{
					_monitors[playerIndex].RankUpEffectOut();
					_monitors[playerIndex].SetButtonOut();
					UserData userData = Singleton<UserDataManager>.Instance.GetUserData(playerIndex);
					container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30010, playerIndex, userData.Detail.CourseRank));
					if (_isTicketGet[playerIndex])
					{
						_subState[playerIndex] = SubProcessState.ItemGet_In;
					}
					else
					{
						_subState[playerIndex] = SubProcessState.ResultWait;
					}
					_timer[playerIndex] = 1000f;
				}
				break;
			case SubProcessState.ItemGet_In:
				if (_timer[playerIndex] <= 0f)
				{
					_monitors[playerIndex].GetTicketIn();
					_subState[playerIndex] = SubProcessState.ItemGet_Wait;
					_timer[playerIndex] = 500f;
				}
				break;
			case SubProcessState.ItemGet_Wait:
				if (_timer[playerIndex] <= 0f)
				{
					_subState[playerIndex] = SubProcessState.ItemGet_Out;
					_timer[playerIndex] = 10000f;
					_monitors[playerIndex].SetButtonIn();
				}
				break;
			case SubProcessState.ItemGet_Out:
				if (_timer[playerIndex] <= 0f)
				{
					_monitors[playerIndex].GetTicketOut();
					_subState[playerIndex] = SubProcessState.ResultWait;
					_timer[playerIndex] = 1000f;
					_monitors[playerIndex].SetButtonOut();
				}
				break;
			case SubProcessState.ResultWait:
				if (_timer[playerIndex] <= 0f)
				{
					_subState[playerIndex] = SubProcessState.OutWait;
					_timer[playerIndex] = 15000f;
					_monitors[playerIndex].SetButtonIn();
				}
				break;
			case SubProcessState.OutWait:
				if (_timer[playerIndex] <= 0f)
				{
					_subState[playerIndex] = SubProcessState.EndCheck;
					_monitors[playerIndex].SetButtonOut();
				}
				break;
			case SubProcessState.EndCheck:
			{
				bool flag2 = true;
				for (int j = 0; j < _monitors.Length; j++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(j).IsEntry && _subState[j] < SubProcessState.EndCheck)
					{
						flag2 = false;
					}
				}
				if (!flag2)
				{
					container.processManager.EnqueueMessage(_monitors[playerIndex].MonitorIndex, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
					_subState[playerIndex] = SubProcessState.EndWait;
				}
				else
				{
					_subState[playerIndex] = SubProcessState.End;
				}
				break;
			}
			case SubProcessState.EndWait:
			{
				bool flag = true;
				for (int i = 0; i < _monitors.Length; i++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry && _subState[playerIndex] < SubProcessState.EndCheck)
					{
						flag = false;
					}
				}
				if (flag)
				{
					_subState[playerIndex] = SubProcessState.End;
				}
				break;
			}
			case SubProcessState.End:
				break;
			}
		}

		protected override void UpdateInput(int monitorId)
		{
			if (!Singleton<UserDataManager>.Instance.GetUserData(monitorId).IsEntry)
			{
				return;
			}
			switch (_subState[monitorId])
			{
			case SubProcessState.TrackResult_In:
			case SubProcessState.RankUp_Out:
			case SubProcessState.ItemGet_Out:
			case SubProcessState.OutWait:
				if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button04))
				{
					_timer[monitorId] = 0f;
					_monitors[monitorId].SetButtonSkip();
				}
				break;
			}
		}

		public override void OnLateUpdate()
		{
		}
	}
}
