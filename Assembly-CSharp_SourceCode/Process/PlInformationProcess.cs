using System.Collections.Generic;
using System.Linq;
using DB;
using IO;
using MAI2.Util;
using MAI2System;
using Manager;
using Manager.Achieve;
using Manager.MaiStudio;
using Manager.UserDatas;
using Mecha;
using Monitor;
using Monitor.TakeOver;
using Net.VO.Mai2;
using Process.Information;
using UnityEngine;
using Util;

namespace Process
{
	public class PlInformationProcess : ProcessBase
	{
		public enum PlInformationSequence
		{
			Init,
			Wait,
			Disp,
			DownloadWait,
			DispEnd,
			DispErrorWait,
			Release
		}

		private const int NetworkErrorMessageDispTime = 4000;

		private const int NetworkWaitMessageDispTime = 2000;

		private PlInformationSequence _state = PlInformationSequence.Wait;

		private PlInformationMonitor[] _monitors;

		private long _delayMSec;

		private bool _dispDownloadMessage;

		public uint[] _rom_version = new uint[2];

		public TakeOverMonitor.MajorRomVersion[] _major_version = new TakeOverMonitor.MajorRomVersion[2];

		public bool _isArgument;

		public uint _current_rom_version;

		public TakeOverMajorVersion _convert = new TakeOverMajorVersion();

		public bool _isCountinue;

		public PlInformationProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public PlInformationProcess(bool isContinue, ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
			_isCountinue = isContinue;
		}

		public PlInformationProcess(ProcessDataContainer dataContainer, params object[] args)
			: base(dataContainer)
		{
			if (args == null)
			{
				return;
			}
			uint num = (uint)args.Length;
			if (num == 2)
			{
				for (int i = 0; i < 2; i++)
				{
					_rom_version[i] = (uint)args[i];
					_major_version[i] = _convert.GetMajorRomVersion(_rom_version[i]);
				}
				_isArgument = true;
			}
		}

		public override void OnAddProcess()
		{
		}

		public override void OnStart()
		{
			GameObject prefs = Resources.Load<GameObject>("Process/PlInformation/PlInformationProcess");
			_monitors = new PlInformationMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<PlInformationMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<PlInformationMonitor>()
			};
			for (int i = 0; i < _monitors.Length; i++)
			{
				_monitors[i].Initialize(i, Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry, _isCountinue);
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30001));
				MechaManager.LedIf[i].SetColorMultiFet(Bd15070_4IF.BodyBrightInGame);
				_monitors[i]._rom_version = _rom_version[i];
				_monitors[i]._major_version = _major_version[i];
			}
			if (!GameManager.IsSelectContinue[0] && !GameManager.IsSelectContinue[1])
			{
				container.processManager.NotificationFadeIn();
			}
			Singleton<GamePlayManager>.Instance.ClaerLog();
			GameManager.UpdateRandom();
			Singleton<CollectionAchieve>.Instance.Initialize();
			if (Singleton<UserDataManager>.Instance.GetUserData(0L).IsEntry && Singleton<UserDataManager>.Instance.GetUserData(1L).IsEntry)
			{
				GameManager.IsMaxTrack = true;
			}
			for (int j = 2; j < 4; j++)
			{
				Singleton<UserDataManager>.Instance.GetUserData(j).Initialize();
				Singleton<UserDataManager>.Instance.SetDefault(j);
			}
			_dispDownloadMessage = false;
			_current_rom_version = Singleton<SystemConfig>.Instance.config.romVersionInfo.versionNo.versionCode;
		}

		public override void OnUpdate()
		{
			PlInformationMonitor[] monitors;
			switch (_state)
			{
			case PlInformationSequence.Init:
				_state = PlInformationSequence.Wait;
				break;
			case PlInformationSequence.Wait:
			{
				_state = PlInformationSequence.Disp;
				List<Jvs.LedPwmFadeParam> list = new List<Jvs.LedPwmFadeParam>();
				list.Add(new Jvs.LedPwmFadeParam
				{
					StartFadeColor = Color.black,
					EndFadeColor = CommonScriptable.GetLedSetting().ButtonAttentionColor,
					FadeTime = 200L,
					NextIndex = 1
				});
				list.Add(new Jvs.LedPwmFadeParam
				{
					StartFadeColor = CommonScriptable.GetLedSetting().ButtonAttentionColor,
					EndFadeColor = Color.black,
					FadeTime = 1000L,
					NextIndex = -1
				});
				for (int l = 0; l < _monitors.Length; l++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(l).IsEntry)
					{
						if (GameManager.IsSelectContinue[l])
						{
							_monitors[l].Skip();
							continue;
						}
						MechaManager.LedIf[l].SetColorMultiAutoFade(list);
						_monitors[l].PlayPlInfo();
					}
				}
				break;
			}
			case PlInformationSequence.Disp:
			{
				if (!_monitors[0].IsPlayPlInfoEnd() || !_monitors[1].IsPlayPlInfoEnd())
				{
					break;
				}
				_state = PlInformationSequence.DownloadWait;
				_delayMSec = 0L;
				for (int k = 0; k < _monitors.Length; k++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(k).IsEntry)
					{
						_monitors[k].PlayPlLoop();
					}
				}
				break;
			}
			case PlInformationSequence.DownloadWait:
			{
				bool flag2 = ((bool?)container.processManager.SendMessage(new Message(ProcessType.NetworkProcess, 200))) ?? true;
				bool flag3 = ((bool?)container.processManager.SendMessage(new Message(ProcessType.NetworkProcess, 300))) ?? true;
				_delayMSec += GameManager.GetGameMSecAdd();
				if (_delayMSec >= 2000 && !_dispDownloadMessage)
				{
					monitors = _monitors;
					foreach (PlInformationMonitor plInformationMonitor in monitors)
					{
						if (Singleton<UserDataManager>.Instance.GetUserData(plInformationMonitor.MonitorIndex).IsEntry)
						{
							container.processManager.EnqueueMessage(plInformationMonitor.MonitorIndex, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
						}
					}
					_dispDownloadMessage = true;
				}
				if (flag2)
				{
					_state = PlInformationSequence.DispEnd;
				}
				else
				{
					if (!flag3)
					{
						break;
					}
					monitors = _monitors;
					foreach (PlInformationMonitor plInformationMonitor2 in monitors)
					{
						if (!Singleton<UserDataManager>.Instance.GetUserData(plInformationMonitor2.MonitorIndex).IsEntry)
						{
							continue;
						}
						int monitorIndex = plInformationMonitor2.MonitorIndex;
						container.processManager.ForcedCloseWindow(plInformationMonitor2.MonitorIndex);
						if (_monitors[monitorIndex]._rom_version != 0 && _monitors[monitorIndex]._rom_version < _current_rom_version)
						{
							TakeOverMonitor.MajorRomVersion majorRomVersion = _convert.GetMajorRomVersion(_current_rom_version);
							if ((uint)_monitors[monitorIndex]._major_version < (uint)majorRomVersion)
							{
								_monitors[monitorIndex]._isMajorVersionUp = true;
							}
							else
							{
								_monitors[monitorIndex]._isMinorVersionUp = true;
							}
						}
						if (Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).UserType == UserData.UserIDType.Inherit || _monitors[monitorIndex]._isMajorVersionUp || _monitors[monitorIndex]._isMinorVersionUp)
						{
							container.processManager.EnqueueMessage(plInformationMonitor2.MonitorIndex, WindowMessageID.TransferDx03);
						}
						else
						{
							container.processManager.EnqueueMessage(plInformationMonitor2.MonitorIndex, WindowMessageID.NetworkErrorToGuest);
						}
						Singleton<UserDataManager>.Instance.GetUserData(plInformationMonitor2.MonitorIndex).Initialize();
						Singleton<UserDataManager>.Instance.GetUserData(plInformationMonitor2.MonitorIndex).IsEntry = true;
					}
					_state = PlInformationSequence.DispErrorWait;
					_delayMSec = 0L;
				}
				break;
			}
			case PlInformationSequence.DispErrorWait:
				_delayMSec += GameManager.GetGameMSecAdd();
				if (_delayMSec >= 4000)
				{
					_state = PlInformationSequence.DispEnd;
				}
				break;
			case PlInformationSequence.DispEnd:
			{
				UpdateUserData();
				bool flag = false;
				uint num = 0u;
				for (int i = 0; i < _monitors.Length; i++)
				{
					if (_monitors[i]._isFinishedDataConversion)
					{
						num++;
					}
				}
				if (num == _monitors.Length)
				{
					flag = true;
				}
				if (flag)
				{
					for (int j = 0; j < 2; j++)
					{
						Manager.UserDatas.UserDetail detail = Singleton<UserDataManager>.Instance.GetUserData(j).Detail;
						Manager.UserDatas.UserOption option = Singleton<UserDataManager>.Instance.GetUserData(j).Option;
						MessageUserInformationData messageUserInformationData = new MessageUserInformationData(j, container.assetManager, detail, option.DispRate, isSubMonitor: true);
						container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20011, j, messageUserInformationData));
					}
					container.processManager.SendMessage(new Message(ProcessType.NetworkProcess, 100));
					if (GameManager.IsSelectContinue[0] || GameManager.IsSelectContinue[1])
					{
						container.processManager.ReleaseProcess(this);
						container.processManager.AddProcess(new InformationProcess(container), 50);
					}
					else
					{
						container.processManager.AddProcess(new FadeProcess(container, this, new InformationProcess(container)), 50);
					}
					_state = PlInformationSequence.Release;
				}
				break;
			}
			case PlInformationSequence.Release:
				return;
			}
			monitors = _monitors;
			for (int m = 0; m < monitors.Length; m++)
			{
				monitors[m].ViewUpdate();
			}
		}

		public override void OnLateUpdate()
		{
		}

		public override void OnRelease()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				Object.Destroy(_monitors[i].gameObject);
			}
		}

		private void UpdateUserData()
		{
			bool flag = false;
			for (int i = 0; i < _monitors.Length; i++)
			{
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(i);
				if (_monitors[i]._isDataConversion && _monitors[i]._isConvertedData && !_monitors[i]._isFinishedDataConversion)
				{
					switch (userData.UserType)
					{
					}
					Manager.UserDatas.UserExtend extend = userData.Extend;
					if (!flag)
					{
						GameManager.SortMusicSetting = extend.SortMusicSetting;
						GameManager.SortCategorySetting = extend.SortCategorySetting;
						GameManager.CategoryIndex = extend.CategoryIndex;
						GameManager.MusicIndex = extend.MusicIndex;
						GameManager.ExtraFlag = extend.ExtraFlag;
						GameManager.SelectScoreType = userData.Extend.SelectScoreType;
						GameManager.SortCategorySetting = userData.Extend.SortCategorySetting;
						GameManager.SortMusicSetting = userData.Extend.SortMusicSetting;
						flag = true;
					}
					GameManager.SelectDifficultyID[i] = extend.SelectDifficultyID;
					GameManager.SelectMusicID[i] = extend.SelectMusicID;
					GameManager.IsSelectResultDetails[i] = extend.SelectResultDetails;
					SoundManager.SetHeadPhoneVolume(i, Singleton<UserDataManager>.Instance.GetUserData(i).Option.HeadPhoneVolume.GetValue());
					_monitors[i]._isFinishedDataConversion = true;
				}
				if (_monitors[i]._isDataConversion && !_monitors[i]._isConvertedData)
				{
					switch (userData.UserType)
					{
					case UserData.UserIDType.Inherit:
						_monitors[i]._isDataConversion = true;
						_monitors[i]._isConvertedData = true;
						break;
					case UserData.UserIDType.Guest:
					case UserData.UserIDType.New:
						_monitors[i]._isDataConversion = true;
						_monitors[i]._isConvertedData = true;
						break;
					}
				}
				if (!_monitors[i]._isDataConversion || _monitors[i]._isConvertedData)
				{
					continue;
				}
				if (_monitors[i]._rom_version < _current_rom_version)
				{
					switch (_monitors[i]._major_version)
					{
					case TakeOverMonitor.MajorRomVersion.NOTHING:
						_monitors[i]._isConvertedData = true;
						_monitors[i]._rom_version = _current_rom_version;
						_monitors[i]._major_version = _convert.GetMajorRomVersion(_monitors[i]._rom_version);
						break;
					case TakeOverMonitor.MajorRomVersion.NONE:
						_ = _monitors[i]._rom_version;
						ResetCommonParam(i);
						userData.Option.InitGame();
						userData.Option.InitJudge();
						userData.Option.InitDesign();
						userData.Option.InitSound();
						userData.Option.InitOther();
						userData.Extend.ExtendContendBit.SetFlag(ExtendContentBitID.PhotoAgree, flag: false);
						userData.Extend.ExtendContendBit.SetFlag(ExtendContentBitID.GotoCodeRead, flag: true);
						userData.Extend.ExtendContendBit.SetFlag(ExtendContentBitID.GotoCharaSelect, flag: true);
						userData.Detail.EquipPartnerID = 17;
						foreach (KeyValuePair<int, PartnerData> item in from x in Singleton<DataManager>.Instance.GetPartners()
							where x.Value.isDefault
							select x)
						{
							userData.AddCollections(UserData.Collection.Partner, item.Value.GetID());
						}
						_monitors[i]._rom_version = 1010000u;
						_monitors[i]._major_version = _convert.GetMajorRomVersion(_monitors[i]._rom_version);
						break;
					case TakeOverMonitor.MajorRomVersion.NONE_PLUS:
					{
						uint rom_version = _monitors[i]._rom_version;
						_ = 1010000;
						ResetCommonParam(i);
						Singleton<UserDataManager>.Instance.SetDefault_SPLASH(i);
						ConvertMapRunMeterValue(i);
						ChangeDefaultPartner(i);
						ChangeDefaultFrame(i);
						ChangeDefaultCharacter(i);
						ChangeDefaultSelectMapID(i, 100001);
						_monitors[i]._rom_version = 1014000u;
						_monitors[i]._major_version = _convert.GetMajorRomVersion(_monitors[i]._rom_version);
						break;
					}
					case TakeOverMonitor.MajorRomVersion.SPLASH:
					{
						uint rom_version = _monitors[i]._rom_version;
						_ = 1014000;
						ResetCommonParam(i);
						Singleton<UserDataManager>.Instance.SetDefault_SPLASH_PLUS(i);
						userData.Extend.ExtendContendBit.SetFlag(ExtendContentBitID.PhotoAgree, flag: true);
						userData.Extend.ExtendContendBit.SetFlag(ExtendContentBitID.GotoCharaSelect, flag: true);
						userData.Extend.ExtendContendBit.SetFlag(ExtendContentBitID.GotoIconPhotoShoot, flag: true);
						ChangeDefaultSelectMapID(i, 150001);
						_monitors[i]._rom_version = _current_rom_version;
						_monitors[i]._major_version = _convert.GetMajorRomVersion(_monitors[i]._rom_version);
						break;
					}
					case TakeOverMonitor.MajorRomVersion.SPLASH_PLUS:
					{
						uint rom_version = _monitors[i]._rom_version;
						_ = 1014000;
						_monitors[i]._rom_version = _current_rom_version;
						_monitors[i]._major_version = _convert.GetMajorRomVersion(_monitors[i]._rom_version);
						break;
					}
					}
				}
				if (_monitors[i]._rom_version >= _current_rom_version)
				{
					_monitors[i]._isConvertedData = true;
					_monitors[i]._rom_version = _current_rom_version;
					_monitors[i]._major_version = _convert.GetMajorRomVersion(_monitors[i]._rom_version);
				}
			}
			for (int j = 0; j < _monitors.Length; j++)
			{
				UserData userData2 = Singleton<UserDataManager>.Instance.GetUserData(j);
				if (_monitors[j]._isDataConversion)
				{
					continue;
				}
				if (userData2.IsEntry)
				{
					_monitors[j]._isDataConversion = true;
					_monitors[j]._isConvertedData = false;
					_monitors[j]._isFinishedDataConversion = false;
					userData2.IsSave = Singleton<OperationManager>.Instance.IsAliveServer;
					switch (userData2.UserType)
					{
					case UserData.UserIDType.Exist:
						if (userData2.Detail.PlayCount == 0)
						{
							Singleton<UserDataManager>.Instance.SetDefault(j);
							ChangeDefaultParam(j);
						}
						userData2.Detail.IsNetMember = userData2.IsNetMember;
						RestoreRating(j);
						RestoreCharadata(j);
						RestoreLoginBonusdata(j);
						RestoreCourse(j);
						RestoreCharge(j);
						RestoreScoreRanking(j);
						RestoreActivity(j);
						RestoreGhost(j);
						userData2.CheckUserData();
						if (_monitors[j]._rom_version != 0 && _monitors[j]._rom_version < _current_rom_version)
						{
							TakeOverMonitor.MajorRomVersion majorRomVersion = _convert.GetMajorRomVersion(_current_rom_version);
							if ((uint)_monitors[j]._major_version < (uint)majorRomVersion)
							{
								_monitors[j]._isMajorVersionUp = true;
							}
							else
							{
								_monitors[j]._isMinorVersionUp = true;
							}
						}
						break;
					case UserData.UserIDType.Inherit:
						RestoreRating(j, isInherit: true);
						Singleton<UserDataManager>.Instance.SetDefault(j);
						ChangeDefaultParam(j);
						break;
					case UserData.UserIDType.New:
						Singleton<UserDataManager>.Instance.SetDefault(j);
						Singleton<UserDataManager>.Instance.SetDefault(j);
						ChangeDefaultParam(j);
						break;
					case UserData.UserIDType.Guest:
						Singleton<UserDataManager>.Instance.SetDefault(j);
						Singleton<UserDataManager>.Instance.SetGuestContentBit(j);
						break;
					}
				}
				else
				{
					_monitors[j]._isDataConversion = true;
					_monitors[j]._isConvertedData = true;
					_monitors[j]._isFinishedDataConversion = true;
				}
				userData2.Detail.PreLastLoginDate = userData2.Detail.LastLoginDate;
				userData2.Detail.LastLoginDate = TimeManager.GetDateString(TimeManager.PlayBaseTime);
				userData2.Detail.SetFirstOnDay(j);
				userData2.Activity.PlayMaimaiDx();
			}
		}

		private void RestoreCharadata(int index)
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

		private void RestoreLoginBonusdata(int index)
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

		private void RestoreRating(int index, bool isInherit = false)
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(index);
			Manager.UserDatas.UserRating ratingList = userData.RatingList;
			List<Manager.UserDatas.UserRate> ratingList2 = ratingList.RatingList;
			List<Manager.UserDatas.UserRate> newRatingList = ratingList.NewRatingList;
			List<Manager.UserDatas.UserRate> nextRatingList = ratingList.NextRatingList;
			List<Manager.UserDatas.UserRate> nextNewRatingList = ratingList.NextNewRatingList;
			Manager.UserDatas.UserUdemae udemae = ratingList.Udemae;
			Net.VO.Mai2.UserRating userRating = Singleton<NetDataManager>.Instance.GetUserRating(index);
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
			if (!isInherit)
			{
				Net.VO.Mai2.UserUdemae udemae2 = userRating.udemae;
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
			}
			Singleton<UserDataManager>.Instance.GetUserData(index).UpdateUserRate();
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
					if (music != null && Singleton<EventManager>.Instance.IsOpenEvent(music.eventName.id) && (int)Singleton<SystemConfig>.Instance.config.dataVersionInfo.versionNo.versionCode >= userGhost.version)
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
						Achievement = Random.Range(num6, num5),
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

		private void ResetGhostData(int index)
		{
			Singleton<GhostManager>.Instance.ResetGhostServerData(index);
		}

		private void ConvertMapRunMeterValue(int monitorID)
		{
			Singleton<MapMaster>.Instance.CreateConvertMapDistance(monitorID);
			List<UserMapData> obj = Singleton<MapMaster>.Instance.RefConvertUserMapList[monitorID];
			List<UserMapData> list = new List<UserMapData>();
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(monitorID);
			foreach (UserMapData item in obj)
			{
				if ((item.ID != 7 && (item.ID < 50001 || item.ID > 50009 || item.ID == 50007)) || item.Distance == 0)
				{
					continue;
				}
				MapData mapData = Singleton<DataManager>.Instance.GetMapData(item.ID);
				if (mapData == null)
				{
					continue;
				}
				int count = mapData.TreasureExDatas.Count;
				if (count <= 0)
				{
					continue;
				}
				if (item.IsComplete)
				{
					item.Distance = (uint)mapData.TreasureExDatas[count - 1].Distance;
					list.Add(item);
					continue;
				}
				bool flag = false;
				int num = -1;
				for (int i = 0; i < count; i++)
				{
					int id = mapData.TreasureExDatas[i].TreasureId.id;
					MapTreasureData mapTreasureData = Singleton<DataManager>.Instance.GetMapTreasureData(id);
					int item_id = -1;
					int num2 = -1;
					bool flag2 = false;
					switch (mapTreasureData.TreasureType)
					{
					case MapTreasureType.Character:
						item_id = mapTreasureData.CharacterId.id;
						if (item_id > 0)
						{
							num2 = userData.CharaList.FindIndex((UserChara m) => m.ID == item_id);
						}
						if (num2 < 0)
						{
							flag2 = true;
						}
						break;
					case MapTreasureType.MusicNew:
						item_id = mapTreasureData.MusicId.id;
						if (item_id > 0)
						{
							num2 = userData.MusicUnlockList.FindIndex((int m) => m == item_id);
						}
						if (num2 < 0)
						{
							flag2 = true;
						}
						break;
					case MapTreasureType.NamePlate:
						item_id = mapTreasureData.NamePlate.id;
						if (item_id > 0)
						{
							num2 = userData.PlateList.FindIndex((Manager.UserDatas.UserItem m) => m.itemId == item_id);
						}
						if (num2 < 0)
						{
							flag2 = true;
						}
						break;
					}
					if (flag2)
					{
						num = i;
						break;
					}
				}
				if (num == -1)
				{
					item.Distance = (uint)mapData.TreasureExDatas[count - 1].Distance;
				}
				else
				{
					if (num > 0)
					{
						if (mapData.TreasureExDatas[num - 1].Distance == 0)
						{
							item.Distance = (uint)mapData.TreasureExDatas[num - 1].Distance;
							flag = true;
						}
						else
						{
							item.Distance = (uint)mapData.TreasureExDatas[num - 1].Distance;
						}
					}
					else if (num == 0)
					{
						if (mapData.TreasureExDatas[0].Distance == 0)
						{
							item.Distance = (uint)mapData.TreasureExDatas[0].Distance;
						}
						else
						{
							item.Distance = 0u;
						}
						flag = true;
					}
					if (flag)
					{
						item.Distance += 1000u;
					}
				}
				list.Add(item);
			}
			Singleton<MapMaster>.Instance.RefConvertUserMapList[monitorID] = new List<UserMapData>();
			foreach (UserMapData item2 in list)
			{
				Singleton<MapMaster>.Instance.RefConvertUserMapList[monitorID].Add(item2);
			}
			Singleton<MapMaster>.Instance.IsConvertMapDistance[monitorID] = false;
			if (Singleton<MapMaster>.Instance.RefConvertUserMapList[monitorID].Count > 0)
			{
				Singleton<MapMaster>.Instance.IsConvertMapDistance[monitorID] = true;
			}
		}

		private void ResetClassValue(int monitorID)
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(monitorID);
			int num = 50000 + UdemaeID.Class_A5.GetStartPoint();
			if (userData.RatingList.Udemae.ClassValue >= userData.RatingList.Udemae.MaxClassValue)
			{
				userData.RatingList.Udemae.SetMaxClassValue(userData.RatingList.Udemae.ClassValue);
			}
			if (userData.RatingList.Udemae.MaxClassValue >= num)
			{
				userData.RatingList.Udemae.SetMaxClassValue(num);
			}
			if (userData.RatingList.Udemae.ClassValue >= num)
			{
				userData.RatingList.Udemae.SetClassValue(num);
			}
		}

		private void ChangeDefaultPartner(int monitorID, int id = 17)
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(monitorID);
			foreach (KeyValuePair<int, PartnerData> v in from x in Singleton<DataManager>.Instance.GetPartners()
				where x.Value.isDefault
				select x)
			{
				if (userData.PartnerList.FindIndex((Manager.UserDatas.UserItem m) => m.itemId == v.Value.GetID()) < 0)
				{
					userData.AddCollections(UserData.Collection.Partner, v.Value.GetID());
				}
			}
			int set_id = id;
			if (userData.PartnerList.FindIndex((Manager.UserDatas.UserItem m) => m.itemId == set_id) >= 0)
			{
				userData.Detail.EquipPartnerID = set_id;
				SoundManager.SetPartnerVoiceCue(monitorID, set_id);
			}
		}

		private void ChangeDefaultFrame(int monitorID, int id = 1)
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(monitorID);
			foreach (KeyValuePair<int, FrameData> v in from x in Singleton<DataManager>.Instance.GetFrames()
				where x.Value.isDefault
				select x)
			{
				if (userData.FrameList.FindIndex((Manager.UserDatas.UserItem m) => m.itemId == v.Value.GetID()) < 0)
				{
					userData.AddCollections(UserData.Collection.Frame, v.Value.GetID());
				}
			}
			int set_id = id;
			if (userData.PartnerList.FindIndex((Manager.UserDatas.UserItem m) => m.itemId == set_id) >= 0)
			{
				userData.Detail.EquipFrameID = set_id;
			}
		}

		private void ChangeDefaultCharacter(int monitorID)
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(monitorID);
			int[] member = new int[5] { 101, 102, 103, 104, 105 };
			bool[] array = new bool[5];
			int i;
			for (i = 0; i < member.Length; i++)
			{
				if (userData.CharaList.FindIndex((UserChara m) => m.ID == member[i]) >= 0)
				{
					continue;
				}
				userData.AddCollections(UserData.Collection.Chara, member[i]);
				int count = userData.NewCharaList.Count;
				if (count > 0)
				{
					userData.NewCharaList.RemoveAt(count - 1);
					if (count == 1)
					{
						userData.NewCharaList.Clear();
					}
				}
			}
			int[] charaSlot = userData.Detail.CharaSlot;
			for (int j = 0; j < charaSlot.Length; j++)
			{
				if (charaSlot[j] <= 0)
				{
					continue;
				}
				for (int k = 0; k < member.Length; k++)
				{
					if (charaSlot[j] == member[k])
					{
						array[k] = true;
						break;
					}
				}
			}
			for (int l = 0; l < charaSlot.Length; l++)
			{
				if (charaSlot[l] > 0)
				{
					continue;
				}
				for (int n = 0; n < member.Length; n++)
				{
					if (!array[n])
					{
						charaSlot[l] = member[n];
						array[n] = true;
						break;
					}
				}
			}
		}

		private void ChangeDefaultSelectMapID(int monitorID, int mapID)
		{
			if (Singleton<DataManager>.Instance.GetMapData(mapID) != null)
			{
				Singleton<UserDataManager>.Instance.GetUserData(monitorID).Detail.SelectMapID = mapID;
			}
		}

		private void ChangeDefaultParam(int monitorID)
		{
			ChangeDefaultCharacter(monitorID);
			ChangeDefaultSelectMapID(monitorID, 100001);
		}

		private void ResetCommonParam(int monitorID)
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(monitorID);
			userData.Extend.TakeOverReset();
			userData.Extend.ExtendContendBit.SetFlag(ExtendContentBitID.GotoCodeRead, flag: true);
			ResetClassValue(monitorID);
			userData.Detail.CourseRank = 0u;
			userData.UpdateUserRate();
			ResetGhostData(monitorID);
		}
	}
}
