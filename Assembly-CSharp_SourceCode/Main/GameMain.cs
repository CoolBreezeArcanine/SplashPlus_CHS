using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AMDaemon;
using DB;
using IO;
using MAI2;
using MAI2.Util;
using MAI2System;
using Manager;
using Manager.MaiStudio;
using Manager.Party.Party;
using Manager.UserDatas;
using Monitor;
using Net;
using Net.VO.Mai2;
using PartyLink;
using Process;
using Process.Error;
using UnityEngine;
using Util;

namespace Main
{
	public class GameMain
	{
		private readonly ProcessManager _processManager;

		private readonly List<string> _errorMessageList = new List<string>();

		private readonly ProcessDataContainer _container = new ProcessDataContainer();

		private readonly Dictionary<SoundManager.PlayerID, bool> _pauseList = new Dictionary<SoundManager.PlayerID, bool>();

		private bool _isInitialize;

		private bool _createError;

		public GameMain()
		{
			_processManager = new ProcessManager(ExceptionHandler);
		}

		public void Initialize(MonoBehaviour gameMainObject, Transform left, Transform right)
		{
			bool flag = false;
			bool flag2 = true;
			using (IniFile iniFile = new IniFile("mai2.ini"))
			{
				flag = iniFile.getValue("Player", "SinglePlayer", 0) == 1;
				flag2 = iniFile.getValue("Skin", "Mask", 1) == 1;
			}
			if (flag)
			{
				Camera.main.transform.position = new Vector3(-540f, 0f, -20f);
				right.localScale = Vector3.zero;
			}
			if (!flag2)
			{
				GameObject.Find("Mask").transform.localScale = Vector3.zero;
			}
			MAI2System.Path.CreateFolder();
			Singleton<OptionDataManager>.Instance.CheckStreamingAssets();
			SoundManager.Initialize();
			_container.monoBehaviour = gameMainObject;
			_container.processManager = _processManager;
			_container.assetManager = new AssetManager(gameMainObject);
			_container.LeftMonitor = left;
			_container.RightMonitor = right;
			_processManager.AddProcess(new PowerOnProcess(_container), 50);
			AssetManager.SetInstance(_container.assetManager);
			_isInitialize = true;
		}

		public void Update()
		{
			try
			{
				SingletonStateMachine<AmManager, AmManager.EState>.Instance.Execute();
				MechaManager.Execute();
				InputManager.UpdateTouchPanel();
				InputManager.UpdateAmInput();
				if (MechaManager.IsInitialized && AMDaemon.Network.IsLanAvailable)
				{
					Setting.get()?.execute();
					Advertise.get()?.execute();
					DeliveryChecker.get()?.execute();
					Manager.Party.Party.Party.Get()?.Execute();
				}
				if (GameManager.IsInitializeEnd && !GameManager.IsErrorMode && IsError())
				{
					NetPacketUtil.ForcedUserLogout();
					Manager.Party.Party.Party.Get()?.Terminate();
					_processManager.OnGotoErrorMode();
					_processManager.AddProcess(new ErrorProcess(_container), 50);
					GameManager.IsGameProcMode = false;
					GameManager.IsErrorMode = true;
					GameManager.IsInitializeEnd = false;
					SoundManager.StopAll();
					MechaManager.SetAllCuOff();
					SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.EnableRead(flag: false);
				}
				if (InputManager.GetSystemInputDown(InputManager.SystemButtonSetting.ButtonTest) && (GameManager.IsGameProcMode || GameManager.IsErrorMode))
				{
					NetPacketUtil.ForcedUserLogout();
					Manager.Party.Party.Party.Get()?.Terminate();
					_processManager.OnGotoTestMode();
					_processManager.AddProcess(new TestModeProcess(_container), 50);
					GameManager.IsGameProcMode = false;
					GameManager.IsErrorMode = false;
					GameManager.IsInitializeEnd = false;
					SoundManager.StopAll();
					MechaManager.SetAllCuOff();
					SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.bookkeep.updateRunningTime();
					SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.EnableRead(flag: false);
					Sequence.BeginTest();
				}
				_processManager.Update();
			}
			catch (Exception e)
			{
				ExceptionHandler(e);
			}
		}

		public void LateUpdate()
		{
			try
			{
				Singleton<SystemConfig>.Instance.execute();
				SoundManager.Execute();
				if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsReady)
				{
					Singleton<OperationManager>.Instance.Execute();
				}
				_processManager.LateUpdate();
			}
			catch (Exception e)
			{
				ExceptionHandler(e);
			}
		}

		public void OnApplicationQuit()
		{
			_processManager.OnApplicationQuit();
			Singleton<SystemConfig>.Instance.terminate();
			Singleton<OperationManager>.Instance.Terminate();
			MechaManager.SetAllCuOff();
			MechaManager.Execute();
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.Execute();
			MechaManager.Terminate();
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.Terminate();
		}

		public void OnApplicationPause(bool isPause)
		{
			if (!_isInitialize)
			{
				return;
			}
			SoundManager.PlayerID[] array = (SoundManager.PlayerID[])Enum.GetValues(typeof(SoundManager.PlayerID));
			foreach (SoundManager.PlayerID playerID in array)
			{
				if (playerID == SoundManager.PlayerID.End)
				{
					break;
				}
				if (isPause)
				{
					if (_pauseList.ContainsKey(playerID))
					{
						_pauseList[playerID] = SoundManager.IsPlaying(playerID);
					}
					else
					{
						_pauseList.Add(playerID, SoundManager.IsPlaying(playerID));
					}
					SoundManager.Pause(playerID, onOff: true);
				}
				else
				{
					SoundManager.Pause(playerID, !_pauseList[playerID]);
				}
			}
			_processManager.OnApplicationPause(isPause);
		}

		private void DebugLogMessageReceived(string condition, string stackTrace, LogType type)
		{
			string text = DateTime.Now.ToString("yyyyMMddHHmmss");
			string newLine = Environment.NewLine;
			string text2 = string.Concat("[", type, "]", text, "\t", condition, newLine, stackTrace, newLine);
			File.AppendAllText(System.IO.Path.Combine(MAI2System.Path.ErrorLogPath, "Debug_Log.log"), text2 + newLine);
			if (type == LogType.Error)
			{
				_errorMessageList.Add(condition + "\n" + stackTrace);
				if (UnityEngine.Debug.developerConsoleVisible)
				{
					UnityEngine.Debug.ClearDeveloperConsole();
					UnityEngine.Debug.developerConsoleVisible = false;
				}
			}
		}

		public void ExceptionHandler(Exception e)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string errorMessage in _errorMessageList)
			{
				stringBuilder.AppendLine(errorMessage);
			}
			string value = stringBuilder.ToString();
			stringBuilder.Clear();
			stringBuilder.Length = 0;
			stringBuilder.AppendLine("Monitor List");
			UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(MonitorBase));
			foreach (UnityEngine.Object @object in array)
			{
				if (@object.hideFlags != HideFlags.NotEditable || @object.hideFlags != HideFlags.HideAndDontSave)
				{
					stringBuilder.AppendLine(@object.name);
				}
			}
			string value2 = _processManager.Dump();
			GameManager.IsException = true;
			if (!_createError)
			{
				MAI2System.Path.DeleteOldErrorFiles(MAI2System.Path.ErrorLogPath, ".log", 4);
				MAI2System.Path.DeleteOldErrorFiles(MAI2System.Path.ErrorLogPath, ".png", 4);
				string text = DateTime.Now.ToString("yyyyMMddHHmmss");
				string errorLogPath = MAI2System.Path.ErrorLogPath;
				using (StreamWriter streamWriter = new StreamWriter(errorLogPath + text + ".log", append: false))
				{
					streamWriter.WriteLine(e.Message);
					streamWriter.WriteLine(value);
					streamWriter.Write(value2);
					streamWriter.Write("\n");
					streamWriter.Write(e);
					streamWriter.Write("\n");
					streamWriter.Write(stringBuilder.ToString());
					streamWriter.Flush();
					streamWriter.Close();
				}
				string finename = errorLogPath + text + ".png";
				_container.monoBehaviour.StartCoroutine(Log.takeCrashScreenShot(finename));
				_createError = true;
			}
		}

		public void DebugAddProcess(ProcessBase prevProcess, Type t)
		{
			if (_container != null)
			{
				_container.processManager.AddProcess(new CommonProcess(_container), 10);
				GameManager.SelectMusicID[0] = 17;
				if (t != null)
				{
					ProcessBase to = t.GetConstructor(new Type[1] { typeof(ProcessDataContainer) }).Invoke(new object[1] { _container }) as ProcessBase;
					_container.processManager.AddProcess(new FadeProcess(_container, prevProcess, to), 50);
				}
				else
				{
					_container.processManager.AddProcess(new FadeProcess(_container, prevProcess, new MusicSelectProcess(_container)), 50);
				}
				_container.processManager.AddProcess(new PleaseWaitProcess(_container), 50);
				_processManager.Update();
				_container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30001));
				_container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 50000));
			}
		}

		public IEnumerator DebugUserDataDownloadCoroutine()
		{
			_container.processManager.AddProcess(new UserDataDLProcess(_container), 50);
			yield return new WaitWhile(() => !(bool)_container.processManager.SendMessage(new Message(ProcessType.NetworkProcess, 200)));
			_container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 50000));
			_container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 50002, CommonMonitor.SkyDaylight.MorningNow));
			yield return new WaitForEndOfFrame();
			_container.processManager.SendMessage(new Message(ProcessType.NetworkProcess, 100));
		}

		public void DebugSetData(int index)
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(index);
			RestoreRating(index);
			RestoreCharaData(index);
			RestoreLoginBonusData(index);
			RestoreCourse(index);
			RestoreCharge(index);
			RestoreScoreRanking(index);
			RestoreActivity(index);
			RestoreGhost(index);
			userData.CheckUserData();
			NetUserData netUserData = Singleton<NetDataManager>.Instance.GetNetUserData(index);
			userData.Extend = netUserData.Extend.Export();
			userData.Detail = netUserData.Detail.Export(netUserData.UserId);
			userData.Option = netUserData.Option.Export();
			userData.CardList = netUserData.CardList.Export();
			userData.Option.CheckOverParam();
			List<UserChara> charaList = userData.CharaList;
			UserCharacter[] userCharacters = Singleton<NetDataManager>.Instance.GetUserCharacters(index);
			UserMap[] userMaps = Singleton<NetDataManager>.Instance.GetUserMaps(index);
			foreach (UserMap map in userMaps)
			{
				userData.MapList.Add(MapMaster.ConvertUserMap(map));
			}
			charaList.Clear();
			if (userCharacters != null)
			{
				UserCharacter[] array = userCharacters;
				for (int i = 0; i < array.Length; i++)
				{
					UserCharacter userCharacter = array[i];
					charaList.Add(new UserChara(userCharacter.characterId, userCharacter.useCount, userCharacter.level));
				}
			}
			userData.IsSave = false;
			GameManager.SelectDifficultyID[index] = userData.Extend.SelectDifficultyID;
			GameManager.SelectMusicID[index] = userData.Extend.SelectMusicID;
			GameManager.IsSelectResultDetails[index] = userData.Extend.SelectResultDetails;
			SoundManager.SetHeadPhoneVolume(index, Singleton<UserDataManager>.Instance.GetUserData(index).Option.HeadPhoneVolume.GetValue());
			GameManager.SortMusicSetting = userData.Extend.SortMusicSetting;
			GameManager.SortCategorySetting = userData.Extend.SortCategorySetting;
			GameManager.CategoryIndex = userData.Extend.CategoryIndex;
			GameManager.MusicIndex = userData.Extend.MusicIndex;
			GameManager.ExtraFlag = userData.Extend.ExtraFlag;
			GameManager.SelectScoreType = userData.Extend.SelectScoreType;
			_container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 50003, index, 1));
			_container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30001));
		}

		private void RestoreRating(int index)
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(index);
			Manager.UserDatas.UserRating ratingList = userData.RatingList;
			List<Manager.UserDatas.UserRate> ratingList2 = ratingList.RatingList;
			List<Manager.UserDatas.UserRate> newRatingList = ratingList.NewRatingList;
			List<Manager.UserDatas.UserRate> nextRatingList = ratingList.NextRatingList;
			List<Manager.UserDatas.UserRate> nextNewRatingList = ratingList.NextNewRatingList;
			Manager.UserDatas.UserUdemae udemae = ratingList.Udemae;
			Net.VO.Mai2.UserUdemae udemae2 = Singleton<NetDataManager>.Instance.GetUserRating(index).udemae;
			ratingList2.Clear();
			newRatingList.Clear();
			List<UserScore>[] scoreList = Singleton<UserDataManager>.Instance.GetUserData(index).ScoreList;
			for (int i = 0; i < scoreList.Length; i++)
			{
				if (scoreList[i] == null)
				{
					continue;
				}
				foreach (UserScore item in scoreList[i])
				{
					MusicData music = Singleton<DataManager>.Instance.GetMusic(item.id);
					if (music != null)
					{
						userData.UpdateScore(item.id, i, item.achivement, (uint)music.version);
					}
				}
			}
			userData.UpdateUserRate();
			nextRatingList.Clear();
			nextNewRatingList.Clear();
			udemae.Initialize();
			udemae.SetClassValue(udemae2.classValue);
			udemae.SetMaxClassValue(udemae2.maxClassValue);
			udemae.TotalWinNum = udemae2.totalWinNum;
			udemae.TotalLoseNum = udemae2.totalLoseNum;
			udemae.MaxWinNum = udemae2.maxWinNum;
			udemae.MaxLoseNum = udemae2.maxLoseNum;
			udemae.WinNum = udemae2.winNum;
			udemae.LoseNum = udemae2.loseNum;
			udemae.NpcTotalWinNum = udemae2.npcTotalWinNum;
			udemae.NpcTotalLoseNum = udemae2.npcTotalLoseNum;
			udemae.NpcMaxWinNum = udemae2.npcMaxWinNum;
			udemae.NpcMaxLoseNum = udemae2.npcMaxLoseNum;
			udemae.NpcWinNum = udemae2.npcWinNum;
			udemae.NpcLoseNum = udemae2.npcLoseNum;
			Singleton<UserDataManager>.Instance.GetUserData(index).UpdateUserRate();
		}

		private void RestoreLoginBonusData(int index)
		{
			List<UserLoginBonus> loginBonusList = Singleton<UserDataManager>.Instance.GetUserData(index).LoginBonusList;
			Net.VO.Mai2.UserLoginBonus[] userLoginBonuses = Singleton<NetDataManager>.Instance.GetUserLoginBonuses(index);
			loginBonusList.Clear();
			if (userLoginBonuses != null)
			{
				Net.VO.Mai2.UserLoginBonus[] array = userLoginBonuses;
				for (int i = 0; i < array.Length; i++)
				{
					Net.VO.Mai2.UserLoginBonus userLoginBonus = array[i];
					loginBonusList.Add(new UserLoginBonus(userLoginBonus.bonusId, userLoginBonus.point, userLoginBonus.isCurrent, userLoginBonus.isComplete));
				}
			}
			Singleton<UserDataManager>.Instance.GetUserData(index).UpdateTotalAwake();
		}

		private void RestoreCharaData(int index)
		{
			List<UserChara> charaList = Singleton<UserDataManager>.Instance.GetUserData(index).CharaList;
			UserCharacter[] userCharacters = Singleton<NetDataManager>.Instance.GetUserCharacters(index);
			charaList.Clear();
			if (userCharacters != null)
			{
				UserCharacter[] array = userCharacters;
				for (int i = 0; i < array.Length; i++)
				{
					UserCharacter userCharacter = array[i];
					charaList.Add(new UserChara(userCharacter.characterId, userCharacter.useCount, userCharacter.level));
				}
			}
			Singleton<UserDataManager>.Instance.GetUserData(index).UpdateTotalAwake();
		}

		private void RestoreCourse(int index)
		{
			List<Manager.UserDatas.UserCourse> courseList = Singleton<UserDataManager>.Instance.GetUserData(index).CourseList;
			Net.VO.Mai2.UserCourse[] userCourses = Singleton<NetDataManager>.Instance.GetUserCourses(index);
			courseList.Clear();
			if (userCourses != null)
			{
				Net.VO.Mai2.UserCourse[] array = userCourses;
				for (int i = 0; i < array.Length; i++)
				{
					Net.VO.Mai2.UserCourse userCourse = array[i];
					courseList.Add(new Manager.UserDatas.UserCourse(userCourse.courseId, userCourse.isLastClear, userCourse.totalRestlife, userCourse.totalAchievement, userCourse.totalDeluxscore, userCourse.playCount, userCourse.clearDate, userCourse.lastPlayDate, userCourse.bestAchievement, userCourse.bestAchievementDate, userCourse.bestDeluxscore, userCourse.bestDeluxscoreDate));
				}
			}
		}

		private void RestoreCharge(int index)
		{
			List<Manager.UserDatas.UserCharge> chargeList = Singleton<UserDataManager>.Instance.GetUserData(index).ChargeList;
			Net.VO.Mai2.UserCharge[] userCharges = Singleton<NetDataManager>.Instance.GetUserCharges(index);
			chargeList.Clear();
			if (userCharges != null)
			{
				Net.VO.Mai2.UserCharge[] array = userCharges;
				for (int i = 0; i < array.Length; i++)
				{
					Net.VO.Mai2.UserCharge userCharge = array[i];
					chargeList.Add(new Manager.UserDatas.UserCharge(userCharge.chargeId, userCharge.stock, userCharge.purchaseDate, userCharge.validDate));
				}
			}
		}

		private void RestoreScoreRanking(int index)
		{
			List<Manager.UserDatas.UserScoreRanking> scoreRankingList = Singleton<UserDataManager>.Instance.GetUserData(index).ScoreRankingList;
			Net.VO.Mai2.UserScoreRanking[] userScoreRankings = Singleton<NetDataManager>.Instance.GetUserScoreRankings(index);
			scoreRankingList.Clear();
			if (userScoreRankings != null)
			{
				Net.VO.Mai2.UserScoreRanking[] array = userScoreRankings;
				for (int i = 0; i < array.Length; i++)
				{
					Net.VO.Mai2.UserScoreRanking userScoreRanking = array[i];
					scoreRankingList.Add(new Manager.UserDatas.UserScoreRanking(userScoreRanking.tournamentId, userScoreRanking.totalScore, userScoreRanking.ranking, userScoreRanking.rankingDate));
				}
			}
		}

		private void RestoreActivity(int index)
		{
			Manager.UserDatas.UserActivity activity = Singleton<UserDataManager>.Instance.GetUserData(index).Activity;
			List<Manager.UserDatas.UserAct> playList = activity.PlayList;
			List<Manager.UserDatas.UserAct> musicList = activity.MusicList;
			Net.VO.Mai2.UserActivity userActivity = Singleton<NetDataManager>.Instance.GetUserActivity(index);
			Net.VO.Mai2.UserAct[] playList2 = userActivity.playList;
			Net.VO.Mai2.UserAct[] musicList2 = userActivity.musicList;
			playList.Clear();
			if (playList2 != null)
			{
				Net.VO.Mai2.UserAct[] array = playList2;
				for (int i = 0; i < array.Length; i++)
				{
					Net.VO.Mai2.UserAct userAct = array[i];
					playList.Add(new Manager.UserDatas.UserAct(userAct.kind, userAct.id, userAct.sortNumber, userAct.param1, userAct.param2, userAct.param3, userAct.param4));
				}
			}
			musicList.Clear();
			if (musicList2 != null)
			{
				Net.VO.Mai2.UserAct[] array = musicList2;
				for (int i = 0; i < array.Length; i++)
				{
					Net.VO.Mai2.UserAct userAct2 = array[i];
					musicList.Add(new Manager.UserDatas.UserAct(userAct2.kind, userAct2.id, userAct2.sortNumber, userAct2.param1, userAct2.param2, userAct2.param3, userAct2.param4));
				}
			}
		}

		private void RestoreGhost(int index)
		{
			Net.VO.Mai2.UserGhost[] userGhosts = Singleton<NetDataManager>.Instance.GetUserGhosts(index);
			GhostManager instance = Singleton<GhostManager>.Instance;
			instance.ResetGhostServerData(index);
			if (userGhosts != null)
			{
				Net.VO.Mai2.UserGhost[] array = userGhosts;
				for (int i = 0; i < array.Length; i++)
				{
					Net.VO.Mai2.UserGhost userGhost = array[i];
					MusicData music = Singleton<DataManager>.Instance.GetMusic(userGhost.musicId);
					if (music != null)
					{
						Manager.UserDatas.UserGhost userGhost2 = new Manager.UserDatas.UserGhost
						{
							Achievement = userGhost.achievement,
							Difficulty = userGhost.difficulty,
							IconId = userGhost.iconId,
							MusicId = userGhost.musicId,
							Name = userGhost.name,
							ShopId = userGhost.shopId,
							RegionCode = userGhost.regionCode,
							PlateId = userGhost.plateId,
							Rate = userGhost.rate,
							ClassRank = userGhost.classRank,
							ClassValue = userGhost.classValue,
							CourseRank = userGhost.courseRank,
							UnixTime = TimeManager.GetUnixTime(userGhost.playDatetime),
							ResultNum = userGhost.resultNum
						};
						userGhost2.ResultBitList.AddRange(userGhost.resultBitList);
						instance.AddGhostData(index, userGhost2);
						if (instance.GetGhostCount(index) >= 10)
						{
							break;
						}
					}
				}
			}
			if (instance.GetGhostCount(index) < 10)
			{
				Singleton<NotesListManager>.Instance.CreateScore();
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(index);
				int classValue = userData.RatingList.Udemae.ClassValue;
				UdemaeID rateToUdemaeID = Manager.UserDatas.UserUdemae.GetRateToUdemaeID(classValue);
				int npcParamBoss = rateToUdemaeID.GetNpcParamBoss();
				int npcFluctuateAchieveLower = rateToUdemaeID.GetNpcFluctuateAchieveLower();
				int npcFluctuateAchieveUpper = rateToUdemaeID.GetNpcFluctuateAchieveUpper();
				int npcFluctuateLevelLower = rateToUdemaeID.GetNpcFluctuateLevelLower();
				int npcFluctuateLevelUpper = rateToUdemaeID.GetNpcFluctuateLevelUpper();
				UdemaeBossData udemaeBoss = Singleton<DataManager>.Instance.GetUdemaeBoss(npcParamBoss);
				int id = udemaeBoss.music.id;
				int id2 = udemaeBoss.difficulty.id;
				int num = udemaeBoss.achieve - npcFluctuateAchieveLower;
				int num2 = udemaeBoss.achieve + npcFluctuateAchieveUpper;
				Notes notes = Singleton<DataManager>.Instance.GetMusic(id).notesData[id2];
				int num3 = notes.level * 10 + notes.levelDecimal;
				int levelLower = num3 - npcFluctuateLevelLower;
				int levelUpper = num3 + npcFluctuateLevelUpper;
				List<int> randomMusic = Singleton<NotesListManager>.Instance.GetRandomMusic(levelLower, levelUpper, MusicDifficultyID.Invalid, 10);
				for (int j = 0; j < randomMusic.Count; j++)
				{
					int num4 = randomMusic[j];
					int num5 = num2;
					int num6 = num;
					if (num5 > 1010000)
					{
						num5 = 1010000;
					}
					if (num6 < 0)
					{
						num6 = 0;
					}
					int num7 = num4 / 100;
					MusicDifficultyID musicDifficultyID = (MusicDifficultyID)(num4 % 100);
					if (musicDifficultyID == MusicDifficultyID.ReMaster)
					{
						MusicData music2 = Singleton<DataManager>.Instance.GetMusic(num7);
						if (music2 == null || !music2.notesData[4].isEnable || !Singleton<EventManager>.Instance.IsOpenEvent(music2.subEventName.id))
						{
							musicDifficultyID = MusicDifficultyID.Master;
						}
					}
					Manager.UserDatas.UserGhost data = new Manager.UserDatas.UserGhost
					{
						Id = 0uL,
						Type = Manager.UserDatas.UserGhost.GhostType.Player,
						Name = ((VsghostnpcID)(0 + j)).GetName(),
						IconId = 1,
						TitleId = 1,
						PlateId = 1,
						Rate = (int)(userData.Detail.Rating / 100u * 100),
						MusicId = num7,
						Difficulty = (int)musicDifficultyID,
						Achievement = UnityEngine.Random.Range(num6, num5),
						ClassValue = classValue,
						ClassRank = (uint)Manager.UserDatas.UserUdemae.GetRateToUdemaeID(classValue),
						CourseRank = (uint)udemaeBoss.daniId.id
					};
					instance.AddGhostData(index, data);
					if (instance.GetGhostCount(index) >= 10)
					{
						break;
					}
				}
			}
			instance.GhostRandomSet();
		}

		private bool IsError()
		{
			if (Singleton<SystemConfig>.Instance.config.IsIgnoreError)
			{
				return false;
			}
			if (!Core.IsReady)
			{
				return false;
			}
			return Error.IsOccurred;
		}
	}
}
