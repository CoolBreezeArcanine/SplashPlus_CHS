using System;
using DB;
using IO;
using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_Partner_000001;
using Manager;
using Manager.MaiStudio;
using Manager.Party.Party;
using Manager.UserDatas;
using Monitor;
using UI;
using UnityEngine;

namespace Process.SubSequence
{
	public class MenuSelectSequence : SequenceBase
	{
		public Action GameStart;

		private bool _startConfirm;

		private bool isHost;

		private bool _isBackEnable = true;

		private bool _courseFirstFadeIn;

		private int SEQUENCE_MOVE_TIME = 1200;

		private int RECRUIT_DECIDE_TIME = 100;

		public MenuSelectSequence(int index, bool isValidity, IMusicSelectProcessProcessing processing, Action gameStart)
			: base(index, isValidity, processing)
		{
			GameStart = gameStart;
		}

		public override void Initialize()
		{
		}

		public override void OnStartSequence()
		{
			if (!base.IsValidity)
			{
				return;
			}
			_startConfirm = false;
			MusicSelectMonitor musicSelectMonitor = ProcessProcessing.MonitorArray[PlayerIndex];
			musicSelectMonitor.OnStartMenuSelect(ButtonCheck);
			UserOption option = Singleton<UserDataManager>.Instance.GetUserData(PlayerIndex).Option;
			musicSelectMonitor.SetSpeed(option.GetNoteSpeed.GetName());
			musicSelectMonitor.SetDetils(option.MirrorMode.GetName(), option.TrackSkip.GetName());
			musicSelectMonitor.SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button02);
			musicSelectMonitor.SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button07);
			SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000101, PlayerIndex);
			musicSelectMonitor.SetConnectMember();
			ProcessProcessing.SetInputLockInfo(PlayerIndex, SEQUENCE_MOVE_TIME);
			musicSelectMonitor.SetChallengeBGDiffSeq();
			if (IsExtraInfoView())
			{
				musicSelectMonitor.ActiveGhostData(isActive: true);
			}
			CourseData courseData = Singleton<CourseManager>.Instance.GetCourseData();
			if (courseData != null)
			{
				_isBackEnable = false;
				CourseCardData courseInfo = new CourseCardData(Resources.Load<Sprite>(Singleton<CourseManager>.Instance.GetCourseImage(CourseManager.ImageType.Title)), life: Singleton<CourseManager>.Instance.GetBeforeRestLife(PlayerIndex), courseMode: courseData.courseMode.id, level: MusicLevelID.None, recover: courseData.recover, greatDamage: courseData.greatDamage, goodDamage: courseData.goodDamage, missDamage: courseData.missDamage, achievement: 0, restLife: 0, isPlay: false);
				musicSelectMonitor.SetCourseInfo(courseInfo);
				musicSelectMonitor.ActiveCourseInfo(isActive: true);
				if (!_courseFirstFadeIn)
				{
					_courseFirstFadeIn = true;
					musicSelectMonitor.SetCourseFadeIn(courseData.courseMode.id);
				}
			}
		}

		public override bool Update()
		{
			if (!base.IsValidity)
			{
				return false;
			}
			if (ProcessProcessing.ConnectMusicAllDecide)
			{
				GameStart();
			}
			MusicSelectMonitor musicSelectMonitor = ProcessProcessing.MonitorArray[PlayerIndex];
			if (ProcessProcessing.IsForceMusicBack)
			{
				musicSelectMonitor.OnBackMenuSelect();
				musicSelectMonitor.SetVisibleOptionSummary(isVisible: false);
				ProcessProcessing.CurrentSelectMenu[PlayerIndex] = MusicSelectProcess.MenuType.GameStart;
				musicSelectMonitor.SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button04);
				musicSelectMonitor.OnStartMusicSelect();
				musicSelectMonitor.ActiveGhostData(isActive: true);
				musicSelectMonitor.ExtraInfoReset();
				ProcessProcessing.SetInputLockInfo(PlayerIndex, 500f);
				ProcessProcessing.DifficultySelectIndex[PlayerIndex] = -1;
				Next(PlayerIndex, MusicSelectProcess.SubSequence.Music);
				return false;
			}
			if (isHost != Party.Get().IsHost())
			{
				isHost = Party.Get().IsHost();
				if (!ProcessProcessing.IsPreparationCompletes[PlayerIndex])
				{
					ButtonCheck(musicSelectMonitor);
				}
			}
			if (ProcessProcessing.IsInputLocking(PlayerIndex))
			{
				Timer += GameManager.GetGameMSecAdd();
				musicSelectMonitor.AnimationUpdate(Timer / 100f);
				if (Timer > 100f)
				{
					Timer = 0f;
				}
			}
			else if (_startConfirm)
			{
				if (InputManager.GetButtonDown(PlayerIndex, InputManager.ButtonSetting.Button04))
				{
					ProcessProcessing.PrepareFinish = true;
					ProcessProcessing.CloseWindow(PlayerIndex);
					musicSelectMonitor.CloseForceStartWarning();
					musicSelectMonitor.OnGameStartWait();
				}
				else if (InputManager.GetButtonDown(PlayerIndex, InputManager.ButtonSetting.Button05))
				{
					_startConfirm = false;
					ProcessProcessing.CloseWindow(PlayerIndex);
					musicSelectMonitor.CloseForceStartWarning();
					ProcessProcessing.IsPreparationCompletes[PlayerIndex] = false;
					MechaManager.LedIf[PlayerIndex].SetColor(2, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.White));
					MechaManager.LedIf[PlayerIndex].SetColor(5, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.White));
					MechaManager.LedIf[PlayerIndex].SetColor(3, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.Red));
					MechaManager.LedIf[PlayerIndex].SetColor(4, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.Blue));
				}
			}
			else
			{
				if (ProcessProcessing.IsPreparationCompletes[PlayerIndex])
				{
					return false;
				}
				if (InputManager.GetButtonDown(PlayerIndex, InputManager.ButtonSetting.Button04))
				{
					switch (ProcessProcessing.CurrentSelectMenu[PlayerIndex])
					{
					case MusicSelectProcess.MenuType.Matching:
						if (IsMatchingEnable() && !Party.Get().IsHost() && !Party.Get().IsClient())
						{
							ProcessProcessing.RecruitActive = true;
							musicSelectMonitor.SetConnectMember();
							ProcessProcessing.SetInputLockInfo(PlayerIndex, RECRUIT_DECIDE_TIME);
							SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, PlayerIndex);
						}
						break;
					case MusicSelectProcess.MenuType.GameStart:
						if (Singleton<UserDataManager>.Instance.GetUserData(0L).IsEntry && Singleton<UserDataManager>.Instance.GetUserData(1L).IsEntry)
						{
							ProcessProcessing.IsPreparationCompletes[PlayerIndex] = true;
							if (ProcessProcessing.IsPreparationCompletes[0] && ProcessProcessing.IsPreparationCompletes[1])
							{
								if (ProcessProcessing.IsClientFinishSetting())
								{
									ProcessProcessing.PrepareFinish = true;
									musicSelectMonitor.OnGameStartWait();
									break;
								}
								_startConfirm = true;
								MechaManager.LedIf[PlayerIndex].SetColor(2, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.Black));
								MechaManager.LedIf[PlayerIndex].SetColor(5, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.Black));
								musicSelectMonitor.SetForceStartWarning();
								SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_INFO_ATTENTION, PlayerIndex);
								ProcessProcessing.CallMessage(PlayerIndex, WindowMessageID.MusicSelectConfirmTrackStart);
							}
							else
							{
								ProcessProcessing.CallWaitMessage(PlayerIndex);
								SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, PlayerIndex);
								musicSelectMonitor.OnGameStartWait();
							}
						}
						else
						{
							ProcessProcessing.IsPreparationCompletes[PlayerIndex] = true;
							if (ProcessProcessing.IsClientFinishSetting())
							{
								ProcessProcessing.PrepareFinish = true;
								musicSelectMonitor.OnGameStartWait();
								break;
							}
							_startConfirm = true;
							musicSelectMonitor.SetForceStartWarning();
							SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_INFO_ATTENTION, PlayerIndex);
							ProcessProcessing.CallMessage(PlayerIndex, WindowMessageID.MusicSelectConfirmTrackStart);
						}
						break;
					case MusicSelectProcess.MenuType.Option:
						if (Singleton<UserDataManager>.Instance.GetUserData(PlayerIndex).Option.OptionKind == OptionKindID.Custom)
						{
							Next(PlayerIndex, MusicSelectProcess.SubSequence.Option);
							SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, PlayerIndex);
							ProcessProcessing.SetInputLockInfo(PlayerIndex, SEQUENCE_MOVE_TIME);
						}
						break;
					}
				}
				else if (InputManager.GetButtonDown(PlayerIndex, InputManager.ButtonSetting.Button05) && !GameManager.IsCourseMode)
				{
					musicSelectMonitor.OnBackMenuSelect();
					musicSelectMonitor.SetVisibleOptionSummary(isVisible: false);
					ProcessProcessing.CurrentSelectMenu[PlayerIndex] = MusicSelectProcess.MenuType.GameStart;
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CANCEL, PlayerIndex);
					musicSelectMonitor.SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button04);
					ProcessProcessing.SetInputLockInfo(PlayerIndex, SEQUENCE_MOVE_TIME);
					Next(PlayerIndex, MusicSelectProcess.SubSequence.Difficulty);
				}
				else if (InputManager.GetButtonDown(PlayerIndex, InputManager.ButtonSetting.Button03) || InputManager.GetTouchPanelAreaDown(PlayerIndex, InputManager.TouchPanelArea.A3))
				{
					if (ProcessProcessing.CurrentSelectMenu[PlayerIndex] + 1 < MusicSelectProcess.MenuType.Max)
					{
						ProcessProcessing.CurrentSelectMenu[PlayerIndex]++;
						musicSelectMonitor.ScrollLeft();
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, PlayerIndex);
						ProcessProcessing.SetInputLockInfo(PlayerIndex, 100f);
						ButtonCheck(musicSelectMonitor);
						VoiceCheck();
					}
					if (IsExtraInfoView())
					{
						musicSelectMonitor.ActiveGhostData(ProcessProcessing.CurrentSelectMenu[PlayerIndex] == MusicSelectProcess.MenuType.GameStart);
					}
					if (GameManager.IsCourseMode)
					{
						musicSelectMonitor.ActiveCourseInfo(ProcessProcessing.CurrentSelectMenu[PlayerIndex] == MusicSelectProcess.MenuType.GameStart);
					}
				}
				else if (InputManager.GetButtonDown(PlayerIndex, InputManager.ButtonSetting.Button06) || InputManager.GetTouchPanelAreaDown(PlayerIndex, InputManager.TouchPanelArea.A6))
				{
					if (ProcessProcessing.CurrentSelectMenu[PlayerIndex] - 1 >= MusicSelectProcess.MenuType.Matching)
					{
						ProcessProcessing.CurrentSelectMenu[PlayerIndex]--;
						musicSelectMonitor.ScrollRight();
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, PlayerIndex);
						ProcessProcessing.SetInputLockInfo(PlayerIndex, 100f);
						ButtonCheck(musicSelectMonitor);
						VoiceCheck();
					}
					if (IsExtraInfoView())
					{
						musicSelectMonitor.ActiveGhostData(ProcessProcessing.CurrentSelectMenu[PlayerIndex] == MusicSelectProcess.MenuType.GameStart);
					}
					if (GameManager.IsCourseMode)
					{
						musicSelectMonitor.ActiveCourseInfo(ProcessProcessing.CurrentSelectMenu[PlayerIndex] == MusicSelectProcess.MenuType.GameStart);
					}
				}
				else
				{
					switch (ProcessProcessing.CurrentSelectMenu[PlayerIndex])
					{
					case MusicSelectProcess.MenuType.Option:
						if (InputManager.GetTouchPanelAreaDown(PlayerIndex, InputManager.TouchPanelArea.B4))
						{
							UserOption option = Singleton<UserDataManager>.Instance.GetUserData(PlayerIndex).Option;
							if (option.OptionKind + 1 < OptionKindID.End)
							{
								option.OptionKind++;
								musicSelectMonitor.ChangeOptionCard(option.OptionKind);
								musicSelectMonitor.SetSpeed(option.GetNoteSpeed.GetName());
								musicSelectMonitor.SetDetils(option.MirrorMode.GetName(), option.TrackSkip.GetName());
								if (option.OptionKind == OptionKindID.Custom)
								{
									musicSelectMonitor.SetOptionSummary(option);
									musicSelectMonitor.ChangeButtonImage(InputManager.ButtonSetting.Button04, 13);
									musicSelectMonitor.SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button04);
									VoiceCheck();
									musicSelectMonitor.SetVisibleOptionSummary(isVisible: true);
								}
								ProcessProcessing.SetInputLockInfo(PlayerIndex, 100f);
								SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, PlayerIndex);
								musicSelectMonitor.PressedTouchPanel(InputManager.TouchPanelArea.B4);
							}
						}
						else
						{
							if (!InputManager.GetTouchPanelAreaDown(PlayerIndex, InputManager.TouchPanelArea.B5))
							{
								break;
							}
							UserOption option2 = Singleton<UserDataManager>.Instance.GetUserData(PlayerIndex).Option;
							if (option2.OptionKind - 1 >= OptionKindID.Basic)
							{
								Singleton<UserDataManager>.Instance.GetUserData(PlayerIndex).Option.OptionKind--;
								musicSelectMonitor.SetSpeed(option2.GetNoteSpeed.GetName());
								musicSelectMonitor.SetDetils(option2.MirrorMode.GetName(), option2.TrackSkip.GetName());
								musicSelectMonitor.ChangeOptionCard(option2.OptionKind);
								if (option2.OptionKind <= OptionKindID.Expert)
								{
									musicSelectMonitor.SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button04);
									musicSelectMonitor.SetVisibleOptionSummary(isVisible: false);
								}
								ProcessProcessing.SetInputLockInfo(PlayerIndex, 100f);
								SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, PlayerIndex);
								musicSelectMonitor.PressedTouchPanel(InputManager.TouchPanelArea.B5);
							}
						}
						break;
					case MusicSelectProcess.MenuType.Volume:
						if (InputManager.GetTouchPanelAreaDown(PlayerIndex, InputManager.TouchPanelArea.B4) || InputManager.GetTouchPanelAreaLongPush(PlayerIndex, InputManager.TouchPanelArea.B4, 200L))
						{
							OptionHeadphonevolumeID headPhoneVolume = Singleton<UserDataManager>.Instance.GetUserData(PlayerIndex).Option.HeadPhoneVolume;
							if (headPhoneVolume + 1 < OptionHeadphonevolumeID.End)
							{
								headPhoneVolume++;
								musicSelectMonitor.SetVolume(headPhoneVolume, headPhoneVolume.GetValue());
								SoundManager.SetHeadPhoneVolume(PlayerIndex, headPhoneVolume.GetValue());
								Singleton<UserDataManager>.Instance.GetUserData(PlayerIndex).Option.HeadPhoneVolume = headPhoneVolume;
								ProcessProcessing.SetInputLockInfo(PlayerIndex, 100f);
								SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, PlayerIndex);
								musicSelectMonitor.PressedTouchPanel(InputManager.TouchPanelArea.B4);
							}
						}
						else if (InputManager.GetTouchPanelAreaDown(PlayerIndex, InputManager.TouchPanelArea.B5) || InputManager.GetTouchPanelAreaLongPush(PlayerIndex, InputManager.TouchPanelArea.B5, 200L))
						{
							OptionHeadphonevolumeID headPhoneVolume2 = Singleton<UserDataManager>.Instance.GetUserData(PlayerIndex).Option.HeadPhoneVolume;
							if (headPhoneVolume2 - 1 >= OptionHeadphonevolumeID.Vol1)
							{
								headPhoneVolume2--;
								musicSelectMonitor.SetVolume(headPhoneVolume2, headPhoneVolume2.GetValue());
								SoundManager.SetHeadPhoneVolume(PlayerIndex, headPhoneVolume2.GetValue());
								Singleton<UserDataManager>.Instance.GetUserData(PlayerIndex).Option.HeadPhoneVolume = headPhoneVolume2;
								ProcessProcessing.SetInputLockInfo(PlayerIndex, 100f);
								SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, PlayerIndex);
								musicSelectMonitor.PressedTouchPanel(InputManager.TouchPanelArea.B5);
							}
						}
						break;
					}
				}
			}
			return false;
		}

		private void ButtonCheck(MusicSelectMonitor monitor)
		{
			monitor.SetVisibleButton(_isBackEnable, InputManager.ButtonSetting.Button05);
			switch (ProcessProcessing.CurrentSelectMenu[PlayerIndex])
			{
			case MusicSelectProcess.MenuType.Matching:
				if (IsMatchingEnable())
				{
					monitor.ChangeButtonImage(InputManager.ButtonSetting.Button04, 2);
				}
				monitor.SetVisibleButton(IsMatchingEnable(), InputManager.ButtonSetting.Button04);
				monitor.SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button03);
				monitor.SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button06);
				break;
			case MusicSelectProcess.MenuType.GameStart:
				monitor.ChangeButtonImage(InputManager.ButtonSetting.Button04, 14);
				monitor.SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button04);
				monitor.SetVisibleOptionSummary(isVisible: false);
				monitor.SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button03);
				monitor.SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button06);
				break;
			case MusicSelectProcess.MenuType.Option:
			{
				monitor.SetVisibleOptionSummary(Singleton<UserDataManager>.Instance.GetUserData(PlayerIndex).Option.OptionKind == OptionKindID.Custom);
				bool flag = Singleton<UserDataManager>.Instance.GetUserData(PlayerIndex).Option.OptionKind == OptionKindID.Custom;
				if (flag)
				{
					monitor.ChangeButtonImage(InputManager.ButtonSetting.Button04, 13);
				}
				monitor.SetVisibleButton(flag, InputManager.ButtonSetting.Button04);
				monitor.SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button03);
				monitor.SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button06);
				break;
			}
			case MusicSelectProcess.MenuType.Volume:
				monitor.SetVisibleOptionSummary(isVisible: false);
				monitor.SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button06);
				monitor.SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button04);
				monitor.SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button03);
				break;
			}
		}

		private void VoiceCheck()
		{
			switch (ProcessProcessing.CurrentSelectMenu[PlayerIndex])
			{
			case MusicSelectProcess.MenuType.Matching:
				if (IsMatchingEnable())
				{
					SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000206, PlayerIndex);
				}
				break;
			case MusicSelectProcess.MenuType.GameStart:
				SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000101, PlayerIndex);
				break;
			case MusicSelectProcess.MenuType.Option:
				SoundManager.PlayPartnerVoice((Singleton<UserDataManager>.Instance.GetUserData(PlayerIndex).Option.OptionKind == OptionKindID.Custom) ? Mai2.Voice_Partner_000001.Cue.VO_000104 : Mai2.Voice_Partner_000001.Cue.VO_000103, PlayerIndex);
				break;
			case MusicSelectProcess.MenuType.Volume:
				SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000109, PlayerIndex);
				break;
			}
		}

		private bool IsMatchingEnable()
		{
			if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.gameSetting.MachineGroupID != MachineGroupID.OFF && !Party.Get().IsHost() && !Party.Get().IsClient() && !GameManager.IsFreedomMode && !GameManager.IsCourseMode && !ProcessProcessing.IsGhostFolder(0))
			{
				return !ProcessProcessing.IsChallengeFolder();
			}
			return false;
		}

		private bool IsExtraInfoView()
		{
			if ((ProcessProcessing.IsGhostFolder(0) && !Singleton<UserDataManager>.Instance.GetUserData(PlayerIndex).IsGuest()) || ProcessProcessing.IsChallengeFolder() || ProcessProcessing.IsTournamentFolder())
			{
				return true;
			}
			return false;
		}

		public int GetCurrentCharactorListMax()
		{
			return 0;
		}

		public int GetCurrentCharactorIndex()
		{
			return 0;
		}
	}
}
