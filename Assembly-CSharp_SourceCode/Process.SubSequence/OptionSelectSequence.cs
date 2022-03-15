using DB;
using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_Partner_000001;
using Manager;
using Manager.UserDatas;
using Monitor;
using UI.DaisyChainList;

namespace Process.SubSequence
{
	public class OptionSelectSequence : SequenceBase
	{
		private UserOption _userOption;

		private float _waitTimer;

		private int SEQUENCE_MOVE_TIME = 1200;

		private bool isStartCheck;

		public OptionCategoryID CurrentOptionCategory { get; private set; }

		public int CurrentOption { get; private set; }

		public string GetCategoryName(int diffIndex)
		{
			OptionCategoryID optionCategoryID = CurrentOptionCategory + diffIndex;
			if (optionCategoryID >= OptionCategoryID.End)
			{
				optionCategoryID -= 5;
			}
			if (optionCategoryID < OptionCategoryID.SpeedSetting)
			{
				optionCategoryID = 5 + optionCategoryID;
			}
			return optionCategoryID.GetName();
		}

		public bool IsOptionBoundary(int diffIndex, out int overCount)
		{
			int num = CurrentOption + diffIndex;
			OptionCategoryID optionCategoryID = CurrentOptionCategory;
			overCount = 0;
			if (num >= optionCategoryID.GetCategoryMax())
			{
				while (num >= optionCategoryID.GetCategoryMax() - 1)
				{
					num = num - optionCategoryID.GetCategoryMax() - 1;
					optionCategoryID = ((optionCategoryID + 1 < OptionCategoryID.End) ? (optionCategoryID + 1) : OptionCategoryID.SpeedSetting);
					overCount++;
				}
			}
			else if (num < -1)
			{
				while (num < -1)
				{
					overCount--;
					optionCategoryID = ((optionCategoryID - 1 >= OptionCategoryID.SpeedSetting) ? (optionCategoryID - 1) : OptionCategoryID.SoundSetting);
					num = optionCategoryID.GetCategoryMax() + 1 + num;
				}
			}
			if (num != optionCategoryID.GetCategoryMax())
			{
				return num == -1;
			}
			return true;
		}

		public string GetCategory(int diffIndex, out OptionCategoryID category, out string value, out string detail, out string valueDetails, out string spriteKey, out bool isLeftButtonActive, out bool isRightButtonActive)
		{
			int num = CurrentOption + diffIndex;
			category = CurrentOptionCategory;
			while (num >= category.GetCategoryMax())
			{
				num = num - category.GetCategoryMax() - 1;
				category = ((category + 1 < OptionCategoryID.End) ? (category + 1) : OptionCategoryID.SpeedSetting);
			}
			while (num < 0)
			{
				category = ((category - 1 >= OptionCategoryID.SpeedSetting) ? (category - 1) : OptionCategoryID.SoundSetting);
				num = category.GetCategoryMax() + 1 + num;
			}
			string optionName = _userOption.GetOptionName(category, num);
			value = _userOption.GetOptionValue(category, num);
			detail = _userOption.GetOptionDetail(category, num);
			valueDetails = _userOption.GetValueDetails(category, num);
			spriteKey = _userOption.GetFilePath(category, num);
			int optionValueIndex = _userOption.GetOptionValueIndex(category, num);
			int optionMax = _userOption.GetOptionMax(category, num);
			isLeftButtonActive = optionValueIndex - 1 > 0;
			isRightButtonActive = optionValueIndex + 1 < optionMax;
			return optionName;
		}

		public OptionSelectSequence(int index, bool isValidity, IMusicSelectProcessProcessing processing, UserOption option)
			: base(index, isValidity, processing)
		{
			_userOption = new UserOption(option);
		}

		public override void OnStartSequence()
		{
			if (base.IsValidity)
			{
				_userOption = new UserOption(Singleton<UserDataManager>.Instance.GetUserData(PlayerIndex).Option);
				MusicSelectMonitor musicSelectMonitor = ProcessProcessing.MonitorArray[PlayerIndex];
				musicSelectMonitor.SetOptionCard(PreviewUpdate);
				CheckCardButton(musicSelectMonitor);
				musicSelectMonitor.SetVisibleOptionSummary(isVisible: false);
				musicSelectMonitor.ChangeButtonImage(InputManager.ButtonSetting.Button04, 2);
				SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000112, PlayerIndex);
				isStartCheck = false;
			}
		}

		public override void OnGameStart()
		{
			if (base.IsValidity)
			{
				Singleton<UserDataManager>.Instance.GetUserData(PlayerIndex).Option = new UserOption(_userOption);
				MusicSelectMonitor obj = ProcessProcessing.MonitorArray[PlayerIndex];
				obj.UpdateDispRate(_userOption.DispRate);
				obj.UpdateAppealFrame(_userOption.SubmonitorAppeal);
			}
		}

		public override void Initialize()
		{
			CurrentOptionCategory = OptionCategoryID.SpeedSetting;
			CurrentOption = 0;
		}

		public override bool Update()
		{
			if (!base.IsValidity)
			{
				return false;
			}
			MusicSelectMonitor musicSelectMonitor = ProcessProcessing.MonitorArray[PlayerIndex];
			if (ProcessProcessing.IsForceMusicBack)
			{
				Singleton<UserDataManager>.Instance.GetUserData(PlayerIndex).Option = new UserOption(_userOption);
				ProcessProcessing.CurrentSelectMenu[PlayerIndex] = MusicSelectProcess.MenuType.GameStart;
				CurrentOption = 0;
				CurrentOptionCategory = OptionCategoryID.SpeedSetting;
				musicSelectMonitor.PressedButton(InputManager.ButtonSetting.Button04);
				musicSelectMonitor.SetSpeed(_userOption.NoteSpeed.GetName());
				musicSelectMonitor.SetDetils(_userOption.MirrorMode.GetName(), _userOption.TrackSkip.GetName());
				musicSelectMonitor.UpdateDispRate(_userOption.DispRate);
				musicSelectMonitor.UpdateAppealFrame(_userOption.SubmonitorAppeal);
				musicSelectMonitor.SetScrollOptionCard(isVisible: false);
				musicSelectMonitor.OnStartMusicSelect();
				Next(PlayerIndex, MusicSelectProcess.SubSequence.Music);
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, PlayerIndex);
				ProcessProcessing.SetInputLockInfo(PlayerIndex, 500f);
				ProcessProcessing.DifficultySelectIndex[PlayerIndex] = -1;
			}
			else if (_waitTimer > 0f)
			{
				_waitTimer -= GameManager.GetGameMSecAdd();
			}
			else if (ProcessProcessing.IsInputLocking(PlayerIndex))
			{
				Timer += GameManager.GetGameMSecAdd();
				musicSelectMonitor.AnimationUpdate(Timer / 100f);
				if (Timer > 100f)
				{
					Timer = 0f;
				}
			}
			else
			{
				if (!isStartCheck)
				{
					isStartCheck = true;
					CheckCardButton(musicSelectMonitor);
				}
				if (InputManager.GetButtonDown(PlayerIndex, InputManager.ButtonSetting.Button04))
				{
					Singleton<UserDataManager>.Instance.GetUserData(PlayerIndex).Option = new UserOption(_userOption);
					ProcessProcessing.CurrentSelectMenu[PlayerIndex] = MusicSelectProcess.MenuType.GameStart;
					CurrentOption = 0;
					CurrentOptionCategory = OptionCategoryID.SpeedSetting;
					musicSelectMonitor.PressedButton(InputManager.ButtonSetting.Button04);
					musicSelectMonitor.SetSpeed(_userOption.NoteSpeed.GetName());
					musicSelectMonitor.SetDetils(_userOption.MirrorMode.GetName(), _userOption.TrackSkip.GetName());
					musicSelectMonitor.UpdateDispRate(_userOption.DispRate);
					musicSelectMonitor.UpdateAppealFrame(_userOption.SubmonitorAppeal);
					Next(PlayerIndex, MusicSelectProcess.SubSequence.Menu);
					ProcessProcessing.SetInputLockInfo(PlayerIndex, SEQUENCE_MOVE_TIME);
					musicSelectMonitor.SetScrollOptionCard(isVisible: false);
					musicSelectMonitor.OutGenreTab();
				}
				else if (InputManager.GetInputDown(PlayerIndex, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3) || InputManager.GetInputLongPush(PlayerIndex, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L))
				{
					if (CurrentOption + 1 >= CurrentOptionCategory.GetCategoryMax())
					{
						CurrentOptionCategory = ((CurrentOptionCategory + 1 < OptionCategoryID.End) ? (CurrentOptionCategory + 1) : OptionCategoryID.SpeedSetting);
						CurrentOption = 0;
					}
					else
					{
						CurrentOption++;
					}
					musicSelectMonitor.ScrollOptionLeft((int)CurrentOptionCategory);
					musicSelectMonitor.SetScrollOptionCard(InputManager.GetInputLongPush(PlayerIndex, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L));
					_waitTimer = 100f;
					CheckCardButton(musicSelectMonitor);
					PreviewUpdate();
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, PlayerIndex);
					ProcessProcessing.SetInputLockInfo(PlayerIndex, 100f);
				}
				else if (InputManager.GetInputDown(PlayerIndex, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6) || InputManager.GetInputLongPush(PlayerIndex, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L))
				{
					if (CurrentOption - 1 < 0)
					{
						CurrentOptionCategory = ((CurrentOptionCategory - 1 >= OptionCategoryID.SpeedSetting) ? (CurrentOptionCategory - 1) : OptionCategoryID.SoundSetting);
						CurrentOption = CurrentOptionCategory.GetCategoryMax() - 1;
					}
					else
					{
						CurrentOption--;
					}
					musicSelectMonitor.ScrollOptionRight((int)CurrentOptionCategory);
					musicSelectMonitor.SetScrollOptionCard(InputManager.GetInputLongPush(PlayerIndex, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L));
					_waitTimer = 100f;
					CheckCardButton(musicSelectMonitor);
					PreviewUpdate();
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, PlayerIndex);
					ProcessProcessing.SetInputLockInfo(PlayerIndex, 100f);
				}
				else if (InputManager.GetTouchPanelAreaDown(PlayerIndex, InputManager.TouchPanelArea.E2))
				{
					if (CurrentOptionCategory + 1 >= OptionCategoryID.End)
					{
						CurrentOptionCategory = OptionCategoryID.SpeedSetting;
					}
					else
					{
						CurrentOptionCategory++;
					}
					CurrentOption = 0;
					musicSelectMonitor.ScrollOptionCategory(Direction.Left, (int)CurrentOptionCategory);
					CheckCardButton(musicSelectMonitor);
					PreviewUpdate();
				}
				else if (InputManager.GetTouchPanelAreaDown(PlayerIndex, InputManager.TouchPanelArea.E8))
				{
					if (CurrentOptionCategory - 1 < OptionCategoryID.SpeedSetting)
					{
						CurrentOptionCategory = OptionCategoryID.SoundSetting;
					}
					else
					{
						CurrentOptionCategory--;
					}
					CurrentOption = CurrentOptionCategory.GetCategoryMax() - 1;
					musicSelectMonitor.ScrollOptionCategory(Direction.Right, (int)CurrentOptionCategory);
					CheckCardButton(musicSelectMonitor);
					PreviewUpdate();
				}
				else if (InputManager.GetTouchPanelAreaPush(PlayerIndex, InputManager.TouchPanelArea.B4))
				{
					int optionValueIndex = _userOption.GetOptionValueIndex(CurrentOptionCategory, CurrentOption);
					int optionMax = _userOption.GetOptionMax(CurrentOptionCategory, CurrentOption);
					if (optionValueIndex + 1 >= optionMax)
					{
						return false;
					}
					_waitTimer = 200f;
					_userOption.AddOption(CurrentOptionCategory, CurrentOption);
					string filePath = _userOption.GetFilePath(CurrentOptionCategory, CurrentOption);
					string optionValue = _userOption.GetOptionValue(CurrentOptionCategory, CurrentOption);
					string valueDetails = _userOption.GetValueDetails(CurrentOptionCategory, CurrentOption);
					musicSelectMonitor.SetValueChange(optionValue, valueDetails, ProcessProcessing.GetOptionValueSprite(filePath));
					musicSelectMonitor.SetOptionSummary(_userOption);
					musicSelectMonitor.PressedTouchPanel(InputManager.TouchPanelArea.B4, InputManager.GetTouchPanelAreaLongPush(InputManager.TouchPanelArea.B4, 250L), optionValueIndex + 2 >= optionMax);
					SoundPreview();
					PreviewUpdate();
					if (optionValueIndex + 2 >= optionMax)
					{
						musicSelectMonitor.SetVisibleCardTouchButton(isVisible: false, InputManager.TouchPanelArea.B4);
					}
					if (optionValueIndex < optionMax)
					{
						musicSelectMonitor.SetVisibleCardTouchButton(isVisible: true, InputManager.TouchPanelArea.B5);
					}
				}
				else if (InputManager.GetTouchPanelAreaPush(PlayerIndex, InputManager.TouchPanelArea.B5))
				{
					int optionValueIndex2 = _userOption.GetOptionValueIndex(CurrentOptionCategory, CurrentOption);
					if (optionValueIndex2 - 1 < 0)
					{
						return false;
					}
					_waitTimer = 200f;
					_userOption.SubOption(CurrentOptionCategory, CurrentOption);
					string filePath2 = _userOption.GetFilePath(CurrentOptionCategory, CurrentOption);
					string optionValue2 = _userOption.GetOptionValue(CurrentOptionCategory, CurrentOption);
					string valueDetails2 = _userOption.GetValueDetails(CurrentOptionCategory, CurrentOption);
					musicSelectMonitor.SetValueChange(optionValue2, valueDetails2, ProcessProcessing.GetOptionValueSprite(filePath2));
					musicSelectMonitor.SetOptionSummary(_userOption);
					musicSelectMonitor.PressedTouchPanel(InputManager.TouchPanelArea.B5, InputManager.GetTouchPanelAreaLongPush(InputManager.TouchPanelArea.B5, 250L), optionValueIndex2 - 2 < 0);
					SoundPreview();
					PreviewUpdate();
					if (optionValueIndex2 - 2 < 0)
					{
						musicSelectMonitor.SetVisibleCardTouchButton(isVisible: false, InputManager.TouchPanelArea.B5);
					}
					if (0 < optionValueIndex2)
					{
						musicSelectMonitor.SetVisibleCardTouchButton(isVisible: true, InputManager.TouchPanelArea.B4);
					}
				}
				else if (InputManager.GetTouchPanelArea_C_Down(PlayerIndex))
				{
					musicSelectMonitor.OptionPreviewPressed(CurrentOptionCategory, CurrentOption);
				}
				else
				{
					musicSelectMonitor.SetScrollOptionCard(isVisible: false);
				}
			}
			return false;
		}

		private void CheckCardButton(MusicSelectMonitor monitor)
		{
			int optionValueIndex = _userOption.GetOptionValueIndex(CurrentOptionCategory, CurrentOption);
			int optionMax = _userOption.GetOptionMax(CurrentOptionCategory, CurrentOption);
			monitor.SetVisibleCardTouchButton(isVisible: true, InputManager.TouchPanelArea.B4);
			monitor.SetVisibleCardTouchButton(isVisible: true, InputManager.TouchPanelArea.B5);
			if (optionValueIndex == 0)
			{
				monitor.SetVisibleCardTouchButton(isVisible: false, InputManager.TouchPanelArea.B5);
			}
			if (optionValueIndex == optionMax - 1)
			{
				monitor.SetVisibleCardTouchButton(isVisible: false, InputManager.TouchPanelArea.B4);
			}
		}

		private void SoundPreview()
		{
			if (CurrentOptionCategory == OptionCategoryID.SoundSetting)
			{
				switch (CurrentOption)
				{
				case 0:
					SoundManager.PlayGameSE(Mai2.Mai2Cue.Cue.SE_GAME_ANSWER, PlayerIndex, _userOption.AnsVolume.GetValue());
					break;
				case 2:
					SoundManager.PlayGameSE(Mai2.Mai2Cue.Cue.SE_GAME_PERFECT, PlayerIndex, _userOption.TapHoldVolume.GetValue());
					break;
				case 3:
				case 4:
					SoundManager.PlayGameSingleSe(_userOption.BreakSe.GetBreakGoodCue(), PlayerIndex, SoundManager.PlayerID.BreakSe, _userOption.BreakVolume.GetValue());
					break;
				case 7:
				case 8:
					SoundManager.PlayGameSingleSe(_userOption.SlideSe.GetSlideCue(), PlayerIndex, SoundManager.PlayerID.SlideSe, _userOption.SlideVolume.GetValue());
					break;
				case 5:
				case 6:
					SoundManager.PlayGameSingleSe(_userOption.ExSe.GetExCue(), PlayerIndex, SoundManager.PlayerID.ExSe, _userOption.ExVolume.GetValue());
					break;
				default:
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, PlayerIndex);
					break;
				}
			}
			else
			{
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, PlayerIndex);
				OptionCategoryID currentOptionCategory = CurrentOptionCategory;
				if (currentOptionCategory == OptionCategoryID.JudgeSetting)
				{
					_ = CurrentOption;
				}
			}
		}

		private void PreviewUpdate()
		{
			MusicSelectMonitor musicSelectMonitor = ProcessProcessing.MonitorArray[PlayerIndex];
			float speed = -1f;
			switch (CurrentOptionCategory)
			{
			case OptionCategoryID.SpeedSetting:
				switch (CurrentOption)
				{
				case 0:
					speed = GameManager.GetNoteSpeed((int)_userOption.NoteSpeed);
					break;
				case 2:
					speed = GameManager.GetNoteSpeed((int)_userOption.NoteSpeed);
					musicSelectMonitor.SetOptionSlideSpeed((int)_userOption.SlideSpeed);
					break;
				case 1:
					speed = GameManager.GetTouchSpeed((int)_userOption.TouchSpeed);
					break;
				}
				break;
			case OptionCategoryID.GameSetting:
			{
				int currentOption = CurrentOption;
				if (currentOption == 2)
				{
					speed = ((_userOption.StarRotate == OptionStarrotateID.On) ? 250 : 0);
				}
				break;
			}
			}
			musicSelectMonitor.SetOptionPreview(CurrentOptionCategory, CurrentOption, speed);
		}
	}
}
