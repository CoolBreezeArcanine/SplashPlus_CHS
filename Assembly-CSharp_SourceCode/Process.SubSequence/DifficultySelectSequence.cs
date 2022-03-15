using DB;
using IO;
using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_Partner_000001;
using Manager;
using Manager.Party.Party;
using Monitor;
using UI;
using UI.DaisyChainList;

namespace Process.SubSequence
{
	public class DifficultySelectSequence : SequenceBase
	{
		private bool[] _playableData;

		private bool _isGhostCategory;

		private bool _isConnectCategory;

		private bool _isChallengeCategory;

		private bool _isScoreAttackCategory;

		private bool _isLocalMusicBackConfirmFlag;

		private int MUSICSELECT_OUT_TIME = 500;

		private int MENU_OUT_TIME = 1200;

		private int _extraDifficulty = -1;

		private MusicDifficultyID difficultyIndex;

		public DifficultySelectSequence(int index, bool isValidity, IMusicSelectProcessProcessing processing)
			: base(index, isValidity, processing)
		{
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
			MusicDifficultyID musicDifficultyID = MusicDifficultyID.Basic;
			if (ProcessProcessing.DifficultySelectIndex[PlayerIndex] != -1)
			{
				musicDifficultyID = (MusicDifficultyID)ProcessProcessing.DifficultySelectIndex[PlayerIndex];
			}
			else
			{
				if (!ProcessProcessing.IsRemasterEnable() && ProcessProcessing.CurrentDifficulty[PlayerIndex] == MusicDifficultyID.ReMaster)
				{
					ProcessProcessing.CurrentDifficulty[PlayerIndex]--;
				}
				musicDifficultyID = ProcessProcessing.CurrentDifficulty[PlayerIndex];
				if (ProcessProcessing.IsLevelTab())
				{
					musicDifficultyID = (MusicDifficultyID)ProcessProcessing.GetMusic(0).Difficulty;
				}
				difficultyIndex = musicDifficultyID;
				ProcessProcessing.DifficultySelectIndex[PlayerIndex] = (int)difficultyIndex;
			}
			MusicSelectMonitor musicSelectMonitor = ProcessProcessing.MonitorArray[PlayerIndex];
			musicSelectMonitor.OnStartDifficultySelect(musicDifficultyID, CheckButton);
			MusicSelectProcess.CombineMusicSelectData combineMusic = ProcessProcessing.GetCombineMusic(0);
			MusicSelectProcess.MusicSelectData musicSelectData = combineMusic.musicSelectData[(int)ProcessProcessing.ScoreType];
			MusicSelectProcess.MusicSelectDetailData msDetailData = combineMusic.msDetailData;
			_playableData = ProcessProcessing.IsPlayableMusic(combineMusic.GetID(ProcessProcessing.ScoreType));
			_isGhostCategory = ProcessProcessing.IsGhostFolder(0);
			_isConnectCategory = ProcessProcessing.IsConnectionFolder();
			_isChallengeCategory = ProcessProcessing.IsChallengeFolder();
			_isScoreAttackCategory = ProcessProcessing.IsTournamentFolder();
			if (_isGhostCategory)
			{
				_extraDifficulty = musicSelectData.Difficulty;
				if (Singleton<UserDataManager>.Instance.GetUserData(PlayerIndex).IsGuest())
				{
					musicSelectMonitor.SetSideMessage(CommonMessageID.Scroll_Level_Select_Normal.GetName());
				}
				else
				{
					if (ProcessProcessing.CurrentDifficulty[PlayerIndex] == (MusicDifficultyID)_extraDifficulty)
					{
						musicSelectMonitor.SetSideMessage(CommonMessageID.Scroll_Level_Select_Otomodachi.GetName());
					}
					else if ((int)ProcessProcessing.CurrentDifficulty[PlayerIndex] > _extraDifficulty)
					{
						musicSelectMonitor.SetSideMessage(CommonMessageID.MusicSelectVsUpperDifficulty.GetName());
					}
					else
					{
						musicSelectMonitor.SetSideMessage(CommonMessageID.MusicSelectVsLowerDifficulty.GetName());
					}
					int num = (int)ProcessProcessing.CurrentDifficulty[PlayerIndex];
					musicSelectMonitor.SetGhostData(num, _playableData[num]);
					musicSelectMonitor.ActiveGhostData(isActive: true);
				}
				if (musicSelectData.Difficulty == 3)
				{
					_playableData[3] = true;
				}
				if (musicSelectData.Difficulty == 4)
				{
					_playableData[3] = true;
					_playableData[4] = true;
				}
				for (int i = 0; i <= 2; i++)
				{
					_playableData[i] = true;
				}
			}
			else if (_isConnectCategory)
			{
				for (int j = 0; j <= 2; j++)
				{
					_playableData[j] = true;
				}
				musicSelectMonitor.SetSideMessage(CommonMessageID.Scroll_Level_Select_Normal.GetName());
			}
			else if (_isChallengeCategory)
			{
				if (Singleton<NotesListManager>.Instance.GetNotesList()[combineMusic.GetID(ProcessProcessing.ScoreType)].ChallengeDetail[PlayerIndex].isEnable)
				{
					musicSelectMonitor.SetChallengeData(ExtraInfoController.EnumChallengeInfoSeq.Difficulty, msDetailData.startLife, msDetailData.challengeUnlockDiff, msDetailData.nextRelaxDay, msDetailData.infoEnable);
				}
				musicSelectMonitor.SetChallengeBGDiffSeq();
				musicSelectMonitor.SetSideMessage(CommonMessageID.Scroll_Level_Select_Normal.GetName());
			}
			else if (_isScoreAttackCategory)
			{
				musicSelectMonitor.ActiveScoreAttackRule(isActive: true);
				musicSelectMonitor.SetSideMessage(CommonMessageID.Scroll_Level_Select_Normal.GetName());
			}
			else
			{
				musicSelectMonitor.SetSideMessage(CommonMessageID.Scroll_Level_Select_Normal.GetName());
			}
			musicSelectMonitor.SetGhostJumpHide();
			if (!_playableData[ProcessProcessing.GetCurrentDifficulty(PlayerIndex)])
			{
				musicSelectMonitor.SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button04);
			}
			SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000099, PlayerIndex);
		}

		public override bool Update()
		{
			if (!base.IsValidity)
			{
				return false;
			}
			MusicSelectMonitor musicSelectMonitor = ProcessProcessing.MonitorArray[PlayerIndex];
			if (ProcessProcessing.GetCombineMusic(0).GetID(ProcessProcessing.ScoreType) == 2)
			{
				ProcessProcessing.IsForceMusicBack = true;
				ProcessProcessing.RecruitCancel = true;
			}
			if (ProcessProcessing.IsForceMusicBack)
			{
				ProcessProcessing.NotificationCharacterSelectProcess();
				if (!ProcessProcessing.IsLevelTab())
				{
					ProcessProcessing.ReCalcLevelSortData();
				}
				musicSelectMonitor.OnStartMusicSelect();
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CANCEL, PlayerIndex);
				musicSelectMonitor.OnBackDifficultySelect();
				musicSelectMonitor.ChangeButtonImage(InputManager.ButtonSetting.Button04, 5);
				musicSelectMonitor.SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button04);
				ProcessProcessing.CallCancelMessage(ProcessProcessing.IsForceMusicBackPlayer);
				ProcessProcessing.DifficultySelectIndex[PlayerIndex] = -1;
				if (Party.Get().IsHost() || Party.Get().IsClient())
				{
					ProcessProcessing.RecruitCancel = true;
				}
				if (_isGhostCategory || _isChallengeCategory || _isScoreAttackCategory)
				{
					musicSelectMonitor.ExtraInfoReset();
				}
				ProcessProcessing.SetInputLockInfo(PlayerIndex, MUSICSELECT_OUT_TIME);
				Next(PlayerIndex, MusicSelectProcess.SubSequence.Music);
				if (ProcessProcessing.IsForceMusicBackConfirm[PlayerIndex])
				{
					musicSelectMonitor.CloseConnectCancelWarning();
				}
				return false;
			}
			if (ProcessProcessing.IsInputLocking(PlayerIndex))
			{
				Timer += GameManager.GetGameMSecAdd();
				musicSelectMonitor.AnimationUpdate(Timer / 100f);
				if (Timer > 100f)
				{
					Timer = 0f;
				}
				return false;
			}
			if (_isLocalMusicBackConfirmFlag != ProcessProcessing.IsForceMusicBackConfirm[PlayerIndex])
			{
				_isLocalMusicBackConfirmFlag = ProcessProcessing.IsForceMusicBackConfirm[PlayerIndex];
				if (_isLocalMusicBackConfirmFlag)
				{
					MechaManager.LedIf[PlayerIndex].SetColor(2, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.Black));
					MechaManager.LedIf[PlayerIndex].SetColor(5, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.Black));
				}
				else
				{
					CheckButton(musicSelectMonitor);
					MechaManager.LedIf[PlayerIndex].SetColor(3, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.Red));
					MechaManager.LedIf[PlayerIndex].SetColor(4, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.Blue));
				}
			}
			if (ProcessProcessing.IsForceMusicBackConfirm[PlayerIndex])
			{
				return false;
			}
			if (InputManager.GetButtonDown(PlayerIndex, InputManager.ButtonSetting.Button04))
			{
				int num = (int)(ProcessProcessing.IsLevelTab() ? difficultyIndex : ProcessProcessing.CurrentDifficulty[PlayerIndex]);
				if (_playableData[num])
				{
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, PlayerIndex);
					Next(PlayerIndex, MusicSelectProcess.SubSequence.Menu);
					if (ProcessProcessing.IsLevelTab())
					{
						ProcessProcessing.DifficultySelectIndex[PlayerIndex] = (int)difficultyIndex;
					}
					else
					{
						ProcessProcessing.DifficultySelectIndex[PlayerIndex] = (int)ProcessProcessing.CurrentDifficulty[PlayerIndex];
					}
					if (Party.Get().IsHost() || Party.Get().IsClient())
					{
						ProcessProcessing.ChangeUserInfo();
					}
					ProcessProcessing.SetInputLockInfo(PlayerIndex, MENU_OUT_TIME);
				}
			}
			else if (InputManager.GetInputDown(PlayerIndex, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3))
			{
				AddDifficulty(musicSelectMonitor, InputManager.ButtonSetting.Button03);
			}
			else if (InputManager.GetInputDown(PlayerIndex, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6))
			{
				SubDifficulty(musicSelectMonitor, InputManager.ButtonSetting.Button06);
			}
			return false;
		}

		private void AddDifficulty(MusicSelectMonitor monitor, InputManager.ButtonSetting button)
		{
			if (ProcessProcessing.IsLevelTab())
			{
				if (difficultyIndex + 1 > MusicDifficultyID.ReMaster || (!ProcessProcessing.IsRemasterEnable() && difficultyIndex + 1 == MusicDifficultyID.ReMaster))
				{
					return;
				}
				difficultyIndex++;
				SoundManager.PlaySE(MusicSelectProcess.DifficultySe[(int)difficultyIndex], PlayerIndex);
				monitor.ScrollDifficultySelect(Direction.Left, button, difficultyIndex);
				ProcessProcessing.DifficultySelectIndex[PlayerIndex] = (int)difficultyIndex;
			}
			else
			{
				if (ProcessProcessing.CurrentDifficulty[PlayerIndex] + 1 > MusicDifficultyID.ReMaster || (!ProcessProcessing.IsRemasterEnable() && ProcessProcessing.CurrentDifficulty[PlayerIndex] + 1 == MusicDifficultyID.ReMaster))
				{
					return;
				}
				ProcessProcessing.CurrentDifficulty[PlayerIndex]++;
				SoundManager.PlaySE(MusicSelectProcess.DifficultySe[(int)ProcessProcessing.CurrentDifficulty[PlayerIndex]], PlayerIndex);
				monitor.ScrollDifficultySelect(Direction.Left, button, ProcessProcessing.CurrentDifficulty[PlayerIndex]);
				ProcessProcessing.DifficultySelectIndex[PlayerIndex] = (int)ProcessProcessing.CurrentDifficulty[PlayerIndex];
			}
			CheckButton(monitor);
			bool flag = Singleton<UserDataManager>.Instance.GetUserData(PlayerIndex).IsGuest();
			if (_isGhostCategory && !flag)
			{
				int num = (int)ProcessProcessing.CurrentDifficulty[PlayerIndex];
				monitor.SetGhostData(num, _playableData[num]);
				if (ProcessProcessing.CurrentDifficulty[PlayerIndex] == (MusicDifficultyID)_extraDifficulty)
				{
					monitor.SetSideMessage(CommonMessageID.Scroll_Level_Select_Otomodachi.GetName());
				}
				else if ((int)ProcessProcessing.CurrentDifficulty[PlayerIndex] > _extraDifficulty)
				{
					monitor.SetSideMessage(CommonMessageID.MusicSelectVsUpperDifficulty.GetName());
				}
				else
				{
					monitor.SetSideMessage(CommonMessageID.MusicSelectVsLowerDifficulty.GetName());
				}
			}
			else
			{
				monitor.SetSideMessage(CommonMessageID.Scroll_Level_Select_Normal.GetName());
			}
			ProcessProcessing.SetInputLockInfo(PlayerIndex, 100f);
		}

		private void SubDifficulty(MusicSelectMonitor monitor, InputManager.ButtonSetting button)
		{
			if (ProcessProcessing.IsLevelTab())
			{
				if (difficultyIndex - 1 < MusicDifficultyID.Basic)
				{
					return;
				}
				difficultyIndex--;
				SoundManager.PlaySE(MusicSelectProcess.DifficultySe[(int)difficultyIndex], PlayerIndex);
				monitor.ScrollDifficultySelect(Direction.Right, button, difficultyIndex);
				ProcessProcessing.DifficultySelectIndex[PlayerIndex] = (int)difficultyIndex;
			}
			else
			{
				if (ProcessProcessing.CurrentDifficulty[PlayerIndex] - 1 < MusicDifficultyID.Basic)
				{
					return;
				}
				ProcessProcessing.CurrentDifficulty[PlayerIndex]--;
				SoundManager.PlaySE(MusicSelectProcess.DifficultySe[(int)ProcessProcessing.CurrentDifficulty[PlayerIndex]], PlayerIndex);
				monitor.ScrollDifficultySelect(Direction.Right, button, ProcessProcessing.CurrentDifficulty[PlayerIndex]);
				ProcessProcessing.DifficultySelectIndex[PlayerIndex] = (int)ProcessProcessing.CurrentDifficulty[PlayerIndex];
			}
			CheckButton(monitor);
			bool flag = Singleton<UserDataManager>.Instance.GetUserData(PlayerIndex).IsGuest();
			if (_isGhostCategory && !flag)
			{
				int num = (int)ProcessProcessing.CurrentDifficulty[PlayerIndex];
				monitor.SetGhostData(num, _playableData[num]);
				if (ProcessProcessing.CurrentDifficulty[PlayerIndex] == (MusicDifficultyID)_extraDifficulty)
				{
					monitor.SetSideMessage(CommonMessageID.Scroll_Level_Select_Otomodachi.GetName());
				}
				else if ((int)ProcessProcessing.CurrentDifficulty[PlayerIndex] > _extraDifficulty)
				{
					monitor.SetSideMessage(CommonMessageID.MusicSelectVsUpperDifficulty.GetName());
				}
				else
				{
					monitor.SetSideMessage(CommonMessageID.MusicSelectVsLowerDifficulty.GetName());
				}
			}
			else
			{
				monitor.SetSideMessage(CommonMessageID.Scroll_Level_Select_Normal.GetName());
			}
			ProcessProcessing.SetInputLockInfo(PlayerIndex, 100f);
		}

		private void CheckButton(MusicSelectMonitor monitor)
		{
			MusicDifficultyID musicDifficultyID = (ProcessProcessing.IsLevelTab() ? difficultyIndex : ProcessProcessing.CurrentDifficulty[PlayerIndex]);
			MusicDifficultyID musicDifficultyID2 = musicDifficultyID;
			switch (musicDifficultyID)
			{
			case MusicDifficultyID.Basic:
				monitor.SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button06);
				monitor.SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button03);
				MechaManager.LedIf[PlayerIndex].SetColor(2, DifficultyContoroller.GetButtonLEDColor((int)(musicDifficultyID + 1)));
				break;
			case MusicDifficultyID.ReMaster:
				monitor.SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button03);
				monitor.SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button06);
				MechaManager.LedIf[PlayerIndex].SetColor(5, DifficultyContoroller.GetButtonLEDColor((int)(musicDifficultyID - 1)));
				break;
			default:
				if (!ProcessProcessing.IsRemasterEnable() && musicDifficultyID + 1 == MusicDifficultyID.ReMaster)
				{
					monitor.SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button03);
					monitor.SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button06);
					MechaManager.LedIf[PlayerIndex].SetColor(5, DifficultyContoroller.GetButtonLEDColor((int)(musicDifficultyID - 1)));
				}
				else
				{
					monitor.SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button06);
					monitor.SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button03);
					MechaManager.LedIf[PlayerIndex].SetColor(2, DifficultyContoroller.GetButtonLEDColor((int)(musicDifficultyID + 1)));
					MechaManager.LedIf[PlayerIndex].SetColor(5, DifficultyContoroller.GetButtonLEDColor((int)(musicDifficultyID - 1)));
				}
				break;
			}
			if (!_playableData[(int)musicDifficultyID2] && monitor.GetVisibleButton(InputManager.ButtonSetting.Button04))
			{
				monitor.SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button04);
			}
			else if (_playableData[(int)musicDifficultyID2] && !monitor.GetVisibleButton(InputManager.ButtonSetting.Button04))
			{
				monitor.SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button04);
			}
		}
	}
}
