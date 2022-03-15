using System;
using System.Collections;
using System.IO;
using Datas.DebugData;
using DB;
using IO;
using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_000001;
using MAI2System;
using Manager;
using Manager.MaiStudio;
using Manager.UserDatas;
using Mecha;
using Monitor;
using UI.DaisyChainList;
using UnityEngine;

namespace Process
{
	public class PhotoEditProcess : ProcessBase, IPhotoEditProcess
	{
		private enum EditSequence
		{
			Initialize,
			Staging,
			Release
		}

		private enum SubSequence
		{
			Init,
			FirstInfoInit,
			FirstInformation,
			Staging,
			Update,
			TimerWait,
			Release
		}

		public enum SelectMode
		{
			Unregistered,
			MainMenu,
			CustomEdit,
			ZoomPreview,
			Confirmation,
			UploadWait,
			Complete
		}

		private const int SinglePhotoShiftPosition = 160;

		private const string CustomTitlePath = "Process/PhotoEdit/Sprites/Txt_Option/";

		private EditSequence _sequence;

		private SelectMode[] _selectModes;

		private PhotoEditMonitor[] _monitors;

		private float _timer;

		private float[] _userTimer;

		private bool[] _isShowSkipButton;

		private bool[] _isUploadeds;

		private int[,] _settingSelectValues;

		private PhotoeditSettingID[] _currentSettingIndexs;

		private PhotoeditStampID[] _currentStampIndex;

		private int[] _undoStampPosIndex;

		private int[] _undoCurrentStampIndex;

		private MusicData[] _musicDatas;

		private Notes[,] _notesDatas;

		private UserData[] _userDatas;

		private Texture2D[,] _textureBuffers;

		private Rect[,][] _faceDetections;

		private UserScore[,] _userScores;

		private GameScoreList[,] _gameScoreLists;

		private uint _trackCount;

		private SubSequence[] _subSequences;

		private int[,] _difficulties;

		private int[] _currentTrackNumbers;

		private int[] _currentPhotoIndexs;

		private bool[] _isShowPhots;

		private bool[] _isRegisterMessage;

		private bool _isSinglePlay;

		private Coroutine _writeCoroutine;

		private Sprite _defaultSprite;

		private Sprite[] _customTitleSprites;

		public PhotoEditProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public override void OnAddProcess()
		{
		}

		public override void OnStart()
		{
			GameObject prefs = Resources.Load<GameObject>("Process/PhotoEdit/PhotoEditProcess");
			_monitors = new PhotoEditMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<PhotoEditMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<PhotoEditMonitor>()
			};
			_currentSettingIndexs = new PhotoeditSettingID[_monitors.Length];
			_selectModes = new SelectMode[_monitors.Length];
			_subSequences = new SubSequence[2];
			_currentTrackNumbers = new int[_monitors.Length];
			_currentPhotoIndexs = new int[_monitors.Length];
			_currentStampIndex = new PhotoeditStampID[_monitors.Length];
			_undoStampPosIndex = new int[_monitors.Length];
			_undoCurrentStampIndex = new int[_monitors.Length];
			_isShowPhots = new bool[_monitors.Length];
			_isShowSkipButton = new bool[_monitors.Length];
			_isUploadeds = new bool[2];
			_isRegisterMessage = new bool[_monitors.Length];
			_userTimer = new float[2];
			_settingSelectValues = new int[_monitors.Length, PhotoeditSettingID.End.GetEnd()];
			if (GameManager.IsCourseMode)
			{
				_trackCount = (uint)Singleton<GamePlayManager>.Instance.GetScoreListCount();
			}
			else
			{
				_trackCount = GameManager.GetMaxTrackCount();
			}
			_difficulties = new int[_monitors.Length, _trackCount];
			_gameScoreLists = new GameScoreList[_monitors.Length, _trackCount];
			_userScores = new UserScore[_monitors.Length, _trackCount];
			_userDatas = new UserData[_monitors.Length];
			_musicDatas = new MusicData[_trackCount];
			_notesDatas = new Notes[_monitors.Length, _trackCount];
			_textureBuffers = new Texture2D[_trackCount, 1];
			_faceDetections = new Rect[_trackCount, 1][];
			int num = 5;
			PhotoeditSettingID photoeditSettingID = PhotoeditSettingID.Layout;
			_customTitleSprites = new Sprite[num];
			for (int i = 0; i < num; i++)
			{
				_customTitleSprites[i] = Resources.Load<Sprite>(System.IO.Path.Combine("Process/PhotoEdit/Sprites/Txt_Option/", "UI_TTR_Option_" + photoeditSettingID++.GetEnumName()));
			}
			_isSinglePlay = false;
			for (int j = 0; j < _monitors.Length; j++)
			{
				MechaManager.LedIf[j].SetColorMultiFet(Bd15070_4IF.BodyBrightOutGame);
				_currentTrackNumbers[j] = 0;
				_currentPhotoIndexs[j] = 0;
				_undoStampPosIndex[j] = 0;
				_undoCurrentStampIndex[j] = 0;
				_currentSettingIndexs[j] = PhotoeditSettingID.Layout;
				_isShowPhots[j] = true;
				_selectModes[j] = SelectMode.MainMenu;
				for (int k = 0; k < PhotoeditSettingID.End.GetEnd(); k++)
				{
					if (k == 0)
					{
						if (Singleton<UserDataManager>.Instance.IsSingleUser())
						{
							if (GameManager.IsPhotoAgree)
							{
								_settingSelectValues[j, k] = 0;
							}
							else
							{
								_settingSelectValues[j, k] = 1;
							}
						}
						else
						{
							_settingSelectValues[j, k] = 2;
						}
					}
					else
					{
						_settingSelectValues[j, k] = 0;
					}
				}
				_userDatas[j] = Singleton<UserDataManager>.Instance.GetUserData(j);
				bool isEntry = _userDatas[j].IsEntry;
				_monitors[j].Initialize(j, isEntry);
				_monitors[j].AdvancedInitialize(this, Singleton<OperationManager>.Instance.ShopData.ShopNickName, DateTime.Now.ToShortDateString(), (PhotoeditLayoutID)_settingSelectValues[j, 0]);
				if (!isEntry)
				{
					_isSinglePlay = true;
					_selectModes[j] = SelectMode.UploadWait;
					continue;
				}
				if (!_userDatas[j].IsGuest() && !_userDatas[j].Detail.ContentBit.IsFlagOn(ContentBitID.FirstTotalResult))
				{
					_subSequences[j] = SubSequence.FirstInfoInit;
					Singleton<UserDataManager>.Instance.GetUserData(j).Detail.ContentBit.SetFlag(ContentBitID.FirstTotalResult, flag: true);
				}
				_isRegisterMessage[j] = _userDatas[j].Detail.IsNetMember != 0;
				DebugGameScoreList debugGameScore = Singleton<GamePlayManager>.Instance.GetDebugGameScore(j);
				if (debugGameScore == null)
				{
					for (int l = 0; l < _trackCount; l++)
					{
						_gameScoreLists[j, l] = Singleton<GamePlayManager>.Instance.GetGameScore(j, l);
						_musicDatas[l] = Singleton<DataManager>.Instance.GetMusic(_gameScoreLists[j, l].SessionInfo.musicId);
						_notesDatas[j, l] = _gameScoreLists[j, l].SessionInfo.notesData;
						_difficulties[j, l] = _gameScoreLists[j, l].SessionInfo.difficulty;
						_userScores[j, l] = new UserScore(_musicDatas[l].GetID())
						{
							achivement = Singleton<GamePlayManager>.Instance.GetAchivement(j, l),
							combo = Singleton<GamePlayManager>.Instance.GetComboType(j, l),
							sync = Singleton<GamePlayManager>.Instance.GetSyncType(j, l),
							deluxscore = Singleton<GamePlayManager>.Instance.GetDeluxeScore(j, l),
							scoreRank = Singleton<GamePlayManager>.Instance.GetClearRank(j, l)
						};
						_currentStampIndex[j] = PhotoeditStampID.Stamp00;
					}
				}
				else
				{
					for (int m = 0; m < _trackCount; m++)
					{
						_gameScoreLists[j, m] = debugGameScore.GetGameScoreList(j, m);
						_musicDatas[m] = Singleton<DataManager>.Instance.GetMusic(debugGameScore.GameScoreData[m].Score.id);
						DebugGameScoreListData debugGameScoreListData = debugGameScore.GameScoreData[m];
						_difficulties[j, m] = (int)debugGameScoreListData.Difficulty;
						_notesDatas[j, m] = Singleton<DataManager>.Instance.GetMusic(debugGameScoreListData.Score.id).notesData[_difficulties[j, m]];
						_userScores[j, m] = debugGameScore.GameScoreData[m].Score;
						_currentStampIndex[j] = PhotoeditStampID.Stamp00;
					}
				}
				_monitors[j].SetPhotoData(LoadPhotoData(0, 0), (j == 0) ? (-160) : 160);
				int num2 = ((_currentTrackNumbers[j] - 1 < 0) ? ((int)(_trackCount - 1)) : (_currentTrackNumbers[j] - 1));
				int num3 = ((_currentTrackNumbers[j] + 1 < (int)_trackCount) ? (_currentTrackNumbers[j] + 1) : 0);
				_monitors[j].SetTrackNumber(num2 + 1, _currentTrackNumbers[j] + 1, num3 + 1, _currentPhotoIndexs[j] + 1, WebCamManager.GetTakePictureNum(_currentTrackNumbers[j]));
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 50003, j, 1));
			}
			for (int n = 0; n < _monitors.Length; n++)
			{
				if (_userDatas[n].IsEntry)
				{
					FirstSetData(n);
					SetViewData(n, 0, jacketImmediate: true);
				}
			}
			container.processManager.PrepareTimer(60, 0, isEntry: false, OnTimeUp);
			if (!_isSinglePlay)
			{
				container.processManager.SetCompleteOneSide(OnTimerComplete);
			}
			container.processManager.NotificationFadeIn();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			switch (_sequence)
			{
			case EditSequence.Initialize:
			{
				_timer += GameManager.GetGameMSecAdd();
				_sequence = EditSequence.Staging;
				for (int j = 0; j < _monitors.Length; j++)
				{
					SoundManager.StopBGM(j);
					SoundManager.PlayBGM(Mai2.Mai2Cue.Cue.BGM_COLLECTION, j);
					if (Singleton<UserDataManager>.Instance.GetUserData(j).IsEntry)
					{
						container.processManager.IsTimeCounting(j, isTimeCount: false);
						container.processManager.SetVisibleTimer(j, isVisible: false);
						if (_subSequences[j] == SubSequence.FirstInfoInit)
						{
							_userTimer[j] = 0f;
							_monitors[j].SetFirstInformation();
						}
						else
						{
							_subSequences[j] = SubSequence.Staging;
							CheckButton(j);
						}
						_monitors[j].Play();
					}
				}
				_timer = 0f;
				break;
			}
			case EditSequence.Staging:
			{
				for (int i = 0; i < _monitors.Length; i++)
				{
					switch (_subSequences[i])
					{
					case SubSequence.FirstInfoInit:
						_userTimer[i] += GameManager.GetGameMSecAdd();
						if (_userTimer[i] >= 700f)
						{
							_subSequences[i] = SubSequence.FirstInformation;
							_userTimer[i] = 0f;
							_isShowSkipButton[i] = false;
							container.processManager.EnqueueMessage(i, WindowMessageID.TotalResultFirst);
							SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000223, i);
						}
						break;
					case SubSequence.FirstInformation:
						UpdateFirstInformation(i);
						break;
					case SubSequence.Staging:
						UpdateStaging(i);
						break;
					}
				}
				break;
			}
			}
			PhotoEditMonitor[] monitors = _monitors;
			for (int k = 0; k < monitors.Length; k++)
			{
				monitors[k].ViewUpdate();
			}
		}

		protected override void UpdateInput(int monitorId)
		{
			if (_sequence != EditSequence.Staging)
			{
				return;
			}
			if (_subSequences[monitorId] != SubSequence.Update)
			{
				if (_subSequences[monitorId] == SubSequence.FirstInformation)
				{
					InputFirstInformation(monitorId);
				}
				return;
			}
			switch (_selectModes[monitorId])
			{
			case SelectMode.MainMenu:
				MainMenuUpdate(monitorId);
				break;
			case SelectMode.CustomEdit:
				EditUpdate(monitorId);
				break;
			case SelectMode.ZoomPreview:
				ZoomPreviewUpdate(monitorId);
				break;
			case SelectMode.Confirmation:
				ConfirmationUpdate(monitorId);
				break;
			case SelectMode.Unregistered:
				UnregisteredUpdate(monitorId);
				break;
			case SelectMode.Complete:
				CompleteUpdate(monitorId);
				break;
			case SelectMode.UploadWait:
				break;
			}
		}

		public override void OnLateUpdate()
		{
		}

		public override void OnRelease()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				UnityEngine.Object.Destroy(_monitors[i].gameObject);
			}
			container.processManager.SetVisibleTimers(isVisible: false);
			Resources.UnloadUnusedAssets();
			GC.Collect();
		}

		private void OnTimeUp()
		{
			if (_sequence != EditSequence.Release)
			{
				if (GameManager.IsEventMode)
				{
					container.processManager.AddProcess(new FadeProcess(container, this, new DataSaveProcess(container)), 50);
				}
				else
				{
					container.processManager.AddProcess(new FadeProcess(container, this, new UnlockMusicProcess(container)), 50);
				}
				for (int i = 0; i < _monitors.Length; i++)
				{
					_subSequences[i] = SubSequence.Release;
					container.processManager.CloseWindow(i);
				}
				_sequence = EditSequence.Release;
			}
		}

		private void OnTimerComplete(int playerIndex)
		{
			container.processManager.EnqueueMessage(playerIndex, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
			_monitors[playerIndex].SetTimeUpWait();
			_subSequences[playerIndex] = SubSequence.TimerWait;
		}

		private void MainMenuUpdate(int i)
		{
			if (InputManager.GetInputDown(i, InputManager.ButtonSetting.Button02, InputManager.TouchPanelArea.A2))
			{
				if (_currentTrackNumbers[i] + 1 < _trackCount)
				{
					_currentTrackNumbers[i]++;
					_currentPhotoIndexs[i] = 0;
					SetStamp(i);
					_monitors[i].SetButtonPressed(InputManager.ButtonSetting.Button02);
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, i);
					int num = ((_currentTrackNumbers[i] - 1 < 0) ? ((int)(_trackCount - 1)) : (_currentTrackNumbers[i] - 1));
					int num2 = ((_currentTrackNumbers[i] + 1 < (int)_trackCount) ? (_currentTrackNumbers[i] + 1) : 0);
					_monitors[i].SetTrackNumber(num + 1, _currentTrackNumbers[i] + 1, num2 + 1, _currentPhotoIndexs[i] + 1, WebCamManager.GetTakePictureNum(_currentTrackNumbers[i]));
					SetViewData(i, _currentTrackNumbers[i]);
					_monitors[i].CardViewScroll(Direction.Right);
					CheckButton(i);
					int shiftPosition = ((i == 0) ? (-160) : 160);
					_monitors[i].SetPhotoData(LoadPhotoData(_currentTrackNumbers[i], _currentPhotoIndexs[i]), shiftPosition);
				}
				SetInputLockInfo(i, 200f);
			}
			else if (InputManager.GetInputDown(i, InputManager.ButtonSetting.Button07, InputManager.TouchPanelArea.A7))
			{
				if (_currentTrackNumbers[i] - 1 >= 0)
				{
					_currentTrackNumbers[i]--;
					_currentPhotoIndexs[i] = 0;
					_monitors[i].SetButtonPressed(InputManager.ButtonSetting.Button07);
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, i);
					int num3 = ((_currentTrackNumbers[i] - 1 < 0) ? ((int)(_trackCount - 1)) : (_currentTrackNumbers[i] - 1));
					int num4 = ((_currentTrackNumbers[i] + 1 < (int)_trackCount) ? (_currentTrackNumbers[i] + 1) : 0);
					_monitors[i].SetTrackNumber(num3 + 1, _currentTrackNumbers[i] + 1, num4 + 1, _currentPhotoIndexs[i] + 1, WebCamManager.GetTakePictureNum(_currentTrackNumbers[i]));
					SetViewData(i, _currentTrackNumbers[i]);
					_monitors[i].CardViewScroll(Direction.Left);
					CheckButton(i);
					int shiftPosition2 = ((i == 0) ? (-160) : 160);
					_monitors[i].SetPhotoData(LoadPhotoData(_currentTrackNumbers[i], _currentPhotoIndexs[i]), shiftPosition2);
				}
				SetInputLockInfo(i, 200f);
			}
			else if (InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button04))
			{
				container.processManager.DecrementTime(i, 60);
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_SKIP, i);
				_monitors[i].SetButtonPressed(InputManager.ButtonSetting.Button04);
			}
			else if (InputManager.GetTouchPanelAreaDown(i, InputManager.TouchPanelArea.B5, InputManager.TouchPanelArea.E6))
			{
				_selectModes[i] = SelectMode.ZoomPreview;
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, i);
				_monitors[i].SetZoomMode();
				SetInputLockInfo(i, 750f);
			}
			else if (InputManager.GetTouchPanelAreaDown(i, InputManager.TouchPanelArea.B4, InputManager.TouchPanelArea.E4))
			{
				if (_isRegisterMessage[i] || _userDatas[i].Detail.IsNetMember != 0)
				{
					_selectModes[i] = SelectMode.CustomEdit;
					SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000224, i);
					_monitors[i].SetCustomEditMode(_userDatas[i].Detail.IsNetMember != 0 && !GameManager.IsUploadPhoto[i]);
					_monitors[i].SetVisibleButton(_currentSettingIndexs[i] != PhotoeditSettingID.Layout, InputManager.ButtonSetting.Button06);
					_monitors[i].SetVisibleButton(_currentSettingIndexs[i] != PhotoeditSettingID.ShopName, InputManager.ButtonSetting.Button03);
				}
				else
				{
					container.processManager.EnqueueMessage(i, WindowMessageID.PhotoNotRegistNet);
					_selectModes[i] = SelectMode.Unregistered;
					_monitors[i].SetUnregisterd();
					SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000225, i);
					_isRegisterMessage[i] = true;
				}
				_userTimer[i] = 0f;
				SetInputLockInfo(i, 800f);
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, i);
			}
		}

		private void EditUpdate(int i)
		{
			if (InputManager.GetInputDown(i, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3))
			{
				int end = PhotoeditSettingID.End.GetEnd();
				if ((int)(_currentSettingIndexs[i] + 1) < end)
				{
					_currentSettingIndexs[i]++;
					_monitors[i].Scroll(Direction.Left);
					_monitors[i].SetButtonPressed(InputManager.ButtonSetting.Button03);
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, i);
					if (_currentSettingIndexs[i] + 1 >= PhotoeditSettingID.End)
					{
						_monitors[i].SetVisibleButton(false, InputManager.ButtonSetting.Button03);
					}
					if (_currentSettingIndexs[i] - 2 < PhotoeditSettingID.Layout)
					{
						_monitors[i].SetVisibleButton(true, InputManager.ButtonSetting.Button06);
					}
				}
			}
			else if (_userDatas[i].Detail.IsNetMember != 0 && !GameManager.IsUploadPhoto[i] && InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button04))
			{
				container.processManager.EnqueueMessage(i, WindowMessageID.PhotoUploadConfirm);
				container.processManager.EnqueueMessage(i, WindowMessageID.PhotoUploadContract);
				_selectModes[i] = SelectMode.Confirmation;
				_monitors[i].SetConfirmationMode();
				_userTimer[i] = 0f;
				_monitors[i].SetButtonPressed(InputManager.ButtonSetting.Button04);
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, i);
				SetInputLockInfo(i, 700f);
			}
			else if (InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button05))
			{
				_selectModes[i] = SelectMode.MainMenu;
				_monitors[i].SetCustom2MainMenu();
				_monitors[i].SetButtonPressed(InputManager.ButtonSetting.Button05);
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CANCEL, i);
				SetInputLockInfo(i, 1000f);
				CheckButton(i);
			}
			else if (InputManager.GetInputDown(i, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6))
			{
				if (_currentSettingIndexs[i] - 1 >= PhotoeditSettingID.Layout)
				{
					_currentSettingIndexs[i]--;
					_monitors[i].Scroll(Direction.Right);
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, i);
					_monitors[i].SetButtonPressed(InputManager.ButtonSetting.Button06);
					if (_currentSettingIndexs[i] + 2 >= PhotoeditSettingID.End)
					{
						_monitors[i].SetVisibleButton(true, InputManager.ButtonSetting.Button03);
					}
					if (_currentSettingIndexs[i] - 1 < PhotoeditSettingID.Layout)
					{
						_monitors[i].SetVisibleButton(false, InputManager.ButtonSetting.Button06);
					}
				}
			}
			else if (InputManager.GetTouchPanelAreaDown(i, InputManager.TouchPanelArea.B4))
			{
				int num = (int)_currentSettingIndexs[i];
				int switchValueLength = GetSwitchValueLength((PhotoeditSettingID)num);
				if (_settingSelectValues[i, num] + 1 < switchValueLength)
				{
					_settingSelectValues[i, num]++;
					_monitors[i].SetValue(Direction.Right, GetSwitchValueName((PhotoeditSettingID)num, _settingSelectValues[i, num]));
					ChangeDetailsShow(i);
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, i);
					_monitors[i].PressedTouchButton(Direction.Right, isLongTouch: false, _settingSelectValues[i, num] + 1 >= switchValueLength);
					if (_settingSelectValues[i, num] + 1 >= switchValueLength)
					{
						_monitors[i].SetVisibleCardButton(isVisible: false, Direction.Right);
					}
					if (_settingSelectValues[i, num] < switchValueLength)
					{
						_monitors[i].SetVisibleCardButton(isVisible: true, Direction.Left);
					}
				}
			}
			else
			{
				if (!InputManager.GetTouchPanelAreaDown(i, InputManager.TouchPanelArea.B5))
				{
					return;
				}
				int num2 = (int)_currentSettingIndexs[i];
				if (_settingSelectValues[i, num2] - 1 >= 0)
				{
					_settingSelectValues[i, num2]--;
					_monitors[i].SetValue(Direction.Left, GetSwitchValueName((PhotoeditSettingID)num2, _settingSelectValues[i, num2]));
					ChangeDetailsShow(i);
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, i);
					_monitors[i].PressedTouchButton(Direction.Left, isLongTouch: false, _settingSelectValues[i, num2] - 1 < 0);
					if (_settingSelectValues[i, num2] - 1 < 0)
					{
						_monitors[i].SetVisibleCardButton(isVisible: false, Direction.Left);
					}
					if (0 <= _settingSelectValues[i, num2])
					{
						_monitors[i].SetVisibleCardButton(isVisible: true, Direction.Right);
					}
				}
			}
		}

		private void ZoomPreviewUpdate(int i)
		{
			if (InputManager.IsButtonDown(i) || InputManager.IsTouchPanelDown(i))
			{
				_selectModes[i] = SelectMode.MainMenu;
				_monitors[i].SetZoom2Main();
				CheckButton(i);
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, i);
				SetInputLockInfo(i, 500f);
			}
		}

		private void ConfirmationUpdate(int i)
		{
			if (InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button04))
			{
				container.processManager.CloseWindow(i);
				if (_writeCoroutine == null)
				{
					_writeCoroutine = container.monoBehaviour.StartCoroutine(TakePhotograph(i, delegate
					{
						container.processManager.EnqueueMessage(i, WindowMessageID.PhotoUploadDone);
						_isUploadeds[i] = true;
						_monitors[i].SetVisibleButton(true, InputManager.ButtonSetting.Button04);
						_monitors[i].SetCompleteMode();
						_selectModes[i] = SelectMode.Complete;
						_userTimer[i] = 0f;
						GameManager.IsUploadPhoto[i] = true;
						GameManager.PhotoTrackNo[i] = _currentTrackNumbers[i] + 1;
					}));
					_monitors[i].SetVisibleButton(false, InputManager.ButtonSetting.Button04, InputManager.ButtonSetting.Button05);
					_selectModes[i] = SelectMode.UploadWait;
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, i);
					SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000226, i);
					_monitors[i].SetButtonPressed(InputManager.ButtonSetting.Button04);
					SetInputLockInfo(i, 200f);
				}
			}
			else if (InputManager.GetInputDown(i, InputManager.ButtonSetting.Button05, InputManager.TouchPanelArea.A5))
			{
				container.processManager.CloseWindow(i);
				_monitors[i].SetConfirmation2Edit();
				_selectModes[i] = SelectMode.CustomEdit;
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CANCEL, i);
				_monitors[i].SetButtonPressed(InputManager.ButtonSetting.Button05);
				_monitors[i].SetVisibleButton(_currentSettingIndexs[i] != PhotoeditSettingID.Layout, InputManager.ButtonSetting.Button06);
				_monitors[i].SetVisibleButton(_currentSettingIndexs[i] != PhotoeditSettingID.ShopName, InputManager.ButtonSetting.Button03);
			}
		}

		private void CompleteUpdate(int i)
		{
			if (_userTimer[i] >= 10000f || InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button04))
			{
				_selectModes[i] = SelectMode.MainMenu;
				if (_userTimer[i] < 10000f)
				{
					_monitors[i].SetButtonPressed(InputManager.ButtonSetting.Button04);
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, i);
					SetInputLockInfo(i, 2000f);
				}
				_monitors[i].SetVisibleButton(false, InputManager.ButtonSetting.Button04);
				container.processManager.CloseWindow(i);
				_monitors[i].SetConfirmation2Main();
				CheckButton(i);
			}
			_userTimer[i] += GameManager.GetGameMSecAdd();
		}

		private void UnregisteredUpdate(int i)
		{
			if (_userTimer[i] >= 10000f || InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button04))
			{
				_selectModes[i] = SelectMode.CustomEdit;
				if (_userTimer[i] < 10000f)
				{
					_monitors[i].SetButtonPressed(InputManager.ButtonSetting.Button04);
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, i);
					SetInputLockInfo(i, 200f);
				}
				_monitors[i].SetCustomEditMode(isUpload: false);
				_monitors[i].SetVisibleButton(_currentSettingIndexs[i] != PhotoeditSettingID.Layout, InputManager.ButtonSetting.Button06);
				_monitors[i].SetVisibleButton(_currentSettingIndexs[i] != PhotoeditSettingID.ShopName, InputManager.ButtonSetting.Button03);
				SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000224, i);
				container.processManager.ForcedCloseWindow(i);
			}
			_userTimer[i] += GameManager.GetGameMSecAdd();
		}

		private void UpdateFirstInformation(int i)
		{
			if (_userTimer[i] >= 10000f)
			{
				container.processManager.CloseWindow(i);
				_subSequences[i] = SubSequence.Staging;
				CheckButton(i);
				_userTimer[i] = 0f;
			}
			if (!_isShowSkipButton[i] && _userTimer[i] >= 3000f)
			{
				_isShowSkipButton[i] = true;
				_monitors[i].SetFirstInformationSkip(isActive: true);
			}
			_userTimer[i] += GameManager.GetGameMSecAdd();
		}

		private void UpdateStaging(int i)
		{
			if (_userTimer[i] >= 750f)
			{
				container.processManager.IsTimeCounting(i, isTimeCount: true);
				container.processManager.SetVisibleTimer(i, isVisible: true);
				SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000154, i);
				_monitors[i].AfterPlay();
				_subSequences[i] = SubSequence.Update;
			}
			_userTimer[i] += GameManager.GetGameMSecAdd();
		}

		private void InputFirstInformation(int i)
		{
			if (_userTimer[i] >= 3000f && InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button04))
			{
				_monitors[i].SetButtonPressed(InputManager.ButtonSetting.Button04);
				_monitors[i].SetFirstInformationSkip(isActive: false);
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, i);
				_userTimer[i] = 10000f;
			}
		}

		private void ChangeDetailsShow(int i)
		{
			PhotoeditSettingID photoeditSettingID = _currentSettingIndexs[i];
			int num = _settingSelectValues[i, (int)photoeditSettingID];
			switch (photoeditSettingID)
			{
			case PhotoeditSettingID.PlayerInfo:
				_monitors[i].SetVisibleUserInfomation(num == 0);
				break;
			case PhotoeditSettingID.Date:
				_monitors[i].SetVisibleShootingDate(num == 0);
				break;
			case PhotoeditSettingID.ShopName:
				_monitors[i].SetVisibleStoreName(num == 0);
				break;
			case PhotoeditSettingID.Stamp:
				_currentStampIndex[i] = (PhotoeditStampID)num;
				SetStamp(i);
				break;
			case PhotoeditSettingID.Layout:
				_monitors[i].ChangeLayout((PhotoeditLayoutID)num);
				break;
			}
		}

		private void SetStamp(int i)
		{
			_monitors[i].SetStamp(_currentStampIndex[i]);
		}

		private void FirstSetData(int playerIndex)
		{
			int num = ((playerIndex == 0) ? 1 : 0);
			UserDetail detail = _userDatas[playerIndex].Detail;
			UserOption option = _userDatas[playerIndex].Option;
			UserDetail detail2 = _userDatas[num].Detail;
			UserOption option2 = _userDatas[num].Option;
			GameManager.TargetID player = ((playerIndex == 0) ? GameManager.TargetID.Right : GameManager.TargetID.Left);
			GameManager.TargetID player2 = ((playerIndex != 0) ? GameManager.TargetID.Right : GameManager.TargetID.Left);
			_monitors[playerIndex].SetUserData(player, container.assetManager, detail, option, isMyself: true);
			_monitors[playerIndex].SetUserData(player2, container.assetManager, detail2, option2, isMyself: false);
		}

		private void SetData(int playerIndex, int targetIndex, int trackNumber, GameManager.TargetID player, bool isMyself)
		{
			GameScoreList gameScoreList = _gameScoreLists[targetIndex, trackNumber];
			UserScore userScore = _userScores[targetIndex, trackNumber];
			Notes notes = _notesDatas[targetIndex, trackNumber];
			int level = notes.level;
			MusicLevelID musicLevelID = (MusicLevelID)notes.musicLevelID;
			MusicDifficultyID difficulty = (MusicDifficultyID)_difficulties[targetIndex, trackNumber];
			_monitors[playerIndex].SetDifficulty(player, difficulty, isMyself);
			_monitors[playerIndex].SetLevel(player, level, musicLevelID, difficulty, isMyself);
			MusicClearrankID clearRank = GameManager.GetClearRank((int)userScore.achivement);
			_monitors[playerIndex].SetClearRank(player, clearRank, isMyself);
			_monitors[playerIndex].SetAchievement(player, userScore.achivement, isMyself);
			CharaData chara = Singleton<DataManager>.Instance.GetChara(Singleton<UserDataManager>.Instance.GetUserData(targetIndex).Detail.CharaSlot[0]);
			if (!chara.name.IsValid())
			{
				chara = Singleton<DataManager>.Instance.GetChara(101);
			}
			Sprite character;
			if (chara.isCopyright)
			{
				if (_defaultSprite == null)
				{
					Texture2D characterTexture2D = container.assetManager.GetCharacterTexture2D(Singleton<DataManager>.Instance.GetChara(101).imageFile);
					_defaultSprite = Sprite.Create(characterTexture2D, new Rect(0f, 0f, characterTexture2D.width, characterTexture2D.height), new Vector2(0.5f, 0.5f));
				}
				character = _defaultSprite;
			}
			else
			{
				string imageFile = chara.imageFile;
				Texture2D characterTexture2D2 = container.assetManager.GetCharacterTexture2D(imageFile);
				character = Sprite.Create(characterTexture2D2, new Rect(0f, 0f, characterTexture2D2.width, characterTexture2D2.height), new Vector2(0.5f, 0.5f));
			}
			_monitors[playerIndex].SetCharacterImage(player, character, isMyself);
			_monitors[playerIndex].SetMedal(player, userScore.combo, userScore.sync, isMyself);
			ConstParameter.ScoreKind scoreKind = GameManager.GetScoreKind(gameScoreList.SessionInfo.musicId);
			_monitors[playerIndex].SetGameScoreType(player, scoreKind, isMyself);
			_monitors[playerIndex].SetPerfectChallenge(player, gameScoreList.IsChallenge, (int)gameScoreList.Life, gameScoreList.Life != 0, isMyself);
			_monitors[playerIndex].SetPlayerRank(player, (gameScoreList.VsRank != 0) ? (gameScoreList.VsRank - 1) : 0u, isMyself);
		}

		private void SetViewData(int playerIndex, int trackNumber, bool jacketImmediate = false)
		{
			GameScoreList gameScoreList = _gameScoreLists[playerIndex, trackNumber];
			UserScore userScore = _userScores[playerIndex, trackNumber];
			Notes notes = _notesDatas[playerIndex, trackNumber];
			string str = _musicDatas[trackNumber].name.str;
			Texture2D jacketThumbTexture2D = container.assetManager.GetJacketThumbTexture2D(_musicDatas[trackNumber].thumbnailName);
			MusicClearrankID clearRank = GameManager.GetClearRank((int)userScore.achivement);
			_monitors[playerIndex].SetDetails(str, notes.notesType, clearRank >= MusicClearrankID.Rank_A, jacketThumbTexture2D, jacketImmediate);
			if (_userDatas[0].IsEntry)
			{
				SetData(playerIndex, 0, trackNumber, GameManager.TargetID.Right, playerIndex == 0);
			}
			if (_userDatas[1].IsEntry)
			{
				SetData(playerIndex, 1, trackNumber, GameManager.TargetID.Left, playerIndex == 1);
			}
			uint deluxscore = userScore.deluxscore;
			int num = Singleton<GamePlayManager>.Instance.GetGameScore(playerIndex, trackNumber).SessionInfo.notesData.maxNotes * 3;
			int percent = 0;
			if (num > 0)
			{
				percent = (int)(_userScores[playerIndex, trackNumber].deluxscore * 100) / num;
			}
			int deluxcoreRank = (int)GameManager.GetDeluxcoreRank(percent);
			_monitors[playerIndex].SetDxScore(deluxscore, (uint)num, deluxcoreRank);
			bool visibleCriticalPerfect = _gameScoreLists[playerIndex, trackNumber].UserOption.DispJudge.IsCritical();
			_monitors[playerIndex].SetVisibleCriticalPerfect(visibleCriticalPerfect);
			_monitors[playerIndex].SetVisibleFastLate(isVisible: true);
			_monitors[playerIndex].SetScore(gameScoreList.CriticalNum, gameScoreList.PerfectNum, gameScoreList.GreatNum, gameScoreList.GoodNum, gameScoreList.MissNum);
			DebugGameScoreList debugGameScore = Singleton<GamePlayManager>.Instance.GetDebugGameScore(playerIndex);
			NoteScore.EScoreType[] array = (NoteScore.EScoreType[])Enum.GetValues(typeof(NoteScore.EScoreType));
			foreach (NoteScore.EScoreType eScoreType in array)
			{
				if (eScoreType != NoteScore.EScoreType.End)
				{
					uint num2 = _gameScoreLists[playerIndex, trackNumber].GetJudgeTotalNum(eScoreType);
					uint num3 = _gameScoreLists[playerIndex, trackNumber].GetJudgeNum(eScoreType, NoteJudge.JudgeBox.Critical);
					uint num4 = _gameScoreLists[playerIndex, trackNumber].GetJudgeNum(eScoreType, NoteJudge.JudgeBox.Perfect);
					uint num5 = _gameScoreLists[playerIndex, trackNumber].GetJudgeNum(eScoreType, NoteJudge.JudgeBox.Great);
					uint num6 = _gameScoreLists[playerIndex, trackNumber].GetJudgeNum(eScoreType, NoteJudge.JudgeBox.Good);
					uint miss = _gameScoreLists[playerIndex, trackNumber].GetJudgeNum(eScoreType, NoteJudge.JudgeBox.Miss);
					float perfect = 0f;
					float critical = 0f;
					float great = 0f;
					float good = 0f;
					if (0 < num2)
					{
						perfect = (float)(num3 + num4) / (float)num2;
						critical = (float)num3 / (float)num2;
						great = (float)num5 / (float)num2;
						good = (float)num6 / (float)num2;
					}
					if (debugGameScore != null)
					{
						DebugGameScoreListData obj = debugGameScore.GameScoreData[trackNumber];
						critical = obj.GetNoteScore(eScoreType, NoteJudge.ETiming.Critical);
						perfect = obj.GetNoteScore(eScoreType, NoteJudge.ETiming.LatePerfect);
						great = obj.GetNoteScore(eScoreType, NoteJudge.ETiming.LateGreat);
						good = obj.GetNoteScore(eScoreType, NoteJudge.ETiming.LateGood);
						num2 = obj.GetCount(eScoreType);
						num3 = obj.GetScore(eScoreType, NoteJudge.JudgeBox.Critical);
						num4 = obj.GetScore(eScoreType, NoteJudge.JudgeBox.Perfect);
						num5 = obj.GetScore(eScoreType, NoteJudge.JudgeBox.Great);
						num6 = obj.GetScore(eScoreType, NoteJudge.JudgeBox.Good);
						miss = obj.GetScore(eScoreType, NoteJudge.JudgeBox.Miss);
					}
					if (eScoreType == NoteScore.EScoreType.Break)
					{
						num3 = _gameScoreLists[playerIndex, trackNumber].GetJudgeBreakNum(NoteJudge.JudgeBox.Critical);
						num4 = _gameScoreLists[playerIndex, trackNumber].GetJudgeBreakNum(NoteJudge.JudgeBox.Perfect);
					}
					_monitors[playerIndex].SetScoreData(eScoreType, num3, num4, num5, num6, miss);
					_monitors[playerIndex].SetScoreGauge(eScoreType, perfect, critical, great, good, num2);
				}
			}
			uint achivement = _userScores[playerIndex, trackNumber].achivement;
			decimal preAchivement = _gameScoreLists[playerIndex, trackNumber].PreAchivement;
			decimal num7 = preAchivement;
			_monitors[playerIndex].SetMyBestRecord(num7 < (decimal)achivement, (int)num7, (int)((decimal)achivement - preAchivement));
			_monitors[playerIndex].SetComboSyncData(gameScoreList.MaxCombo, gameScoreList.MaxChain);
			_monitors[playerIndex].SetVisibleSync(1 < Singleton<GamePlayManager>.Instance.GetPlayerIgnoreNpcNum(trackNumber));
			_monitors[playerIndex].SetFastRate(_gameScoreLists[playerIndex, trackNumber].Fast, _gameScoreLists[playerIndex, trackNumber].Late);
		}

		private void CheckButton(int i)
		{
			_monitors[i].SetVisibleButton(_currentTrackNumbers[i] - 1 >= 0, InputManager.ButtonSetting.Button07);
			_monitors[i].SetVisibleButton(_currentTrackNumbers[i] + 1 < _trackCount, InputManager.ButtonSetting.Button02);
		}

		private Texture2D LoadPhotoData(int trackNumber, int photoIndex)
		{
			if (_textureBuffers[trackNumber, photoIndex] != null)
			{
				return _textureBuffers[trackNumber, photoIndex];
			}
			return _textureBuffers[trackNumber, photoIndex] = WebCamManager.GetTakePicture(trackNumber, photoIndex);
		}

		public IEnumerator TakePhotograph(int i, Action completeAction)
		{
			yield return new WaitForSeconds(0.5f);
			yield return new WaitForEndOfFrame();
			Rect photoRect = _monitors[i].GetPhotoRect();
			RenderTexture.active = _monitors[i].OverlayCamera.targetTexture;
			Texture2D texture2D = new Texture2D((int)photoRect.size.x, (int)photoRect.size.y, TextureFormat.ARGB32, mipChain: false);
			texture2D.ReadPixels(new Rect(Vector2.zero, photoRect.size), 0, 0, recalculateMipMaps: false);
			texture2D.Apply();
			Write(texture2D, i, 100);
			_writeCoroutine = null;
			completeAction();
		}

		private void Write(Texture2D tex, int i, int quality)
		{
			byte[] buffer = tex.EncodeToJPG(quality);
			using (FileStream output = new FileStream(System.IO.Path.Combine(WebCamManager.GetUploadDataPath(), "UserGamePhoto_" + i + ".jpg"), FileMode.Create, FileAccess.Write))
			{
				using BinaryWriter binaryWriter = new BinaryWriter(output);
				binaryWriter.Write(buffer);
			}
			GameManager.PhotoTexture[i] = tex;
		}

		public Sprite GetSettingNameSprite(int playerIndex, int diff)
		{
			if (_selectModes[playerIndex] == SelectMode.CustomEdit)
			{
				int num = (int)(_currentSettingIndexs[playerIndex] + diff);
				int end = PhotoeditSettingID.End.GetEnd();
				while (num >= end)
				{
					num -= end;
				}
				while (num < 0)
				{
					num = end + num;
				}
				return _customTitleSprites[num];
			}
			int num2 = (int)(_currentStampIndex[playerIndex] + diff);
			int num3 = 13;
			while (num2 >= num3)
			{
				num2 -= num3;
			}
			while (num2 < 0)
			{
				num2 = num3 + num2;
			}
			return null;
		}

		public void IsCheckCategory(int playerIndex, int diff, out bool isLeftActive, out bool isRightActive)
		{
			int num = (int)(_currentSettingIndexs[playerIndex] + diff);
			int end = PhotoeditSettingID.End.GetEnd();
			while (num >= end)
			{
				num -= end;
			}
			while (num < 0)
			{
				num = end + num;
			}
			int switchValueLength = GetSwitchValueLength((PhotoeditSettingID)num);
			isLeftActive = _settingSelectValues[playerIndex, num] - 1 >= 0;
			isRightActive = _settingSelectValues[playerIndex, num] + 1 < switchValueLength;
		}

		public int GetSettingSwitchValue(int playerIndex, int diff, out string switchName)
		{
			int num = (int)(_currentSettingIndexs[playerIndex] + diff);
			if (_selectModes[playerIndex] == SelectMode.CustomEdit)
			{
				int end = PhotoeditSettingID.End.GetEnd();
				while (num >= end)
				{
					num -= end;
				}
				while (num < 0)
				{
					num = end + num;
				}
				int num2 = _settingSelectValues[playerIndex, num];
				switchName = GetSwitchValueName((PhotoeditSettingID)num, num2);
				return num2;
			}
			while (num >= PhotoeditStampID.End.GetEnd())
			{
				num -= PhotoeditStampID.End.GetEnd();
			}
			while (num < 0)
			{
				num = PhotoeditStampID.End.GetEnd() + num;
			}
			switchName = ((_currentStampIndex[playerIndex] == (PhotoeditStampID)num) ? "ON" : "OFF");
			return 0;
		}

		private static string GetSwitchValueName(PhotoeditSettingID id, int index)
		{
			return id switch
			{
				PhotoeditSettingID.PlayerInfo => ((PhotoeditPlayerinfoID)Enum.ToObject(typeof(PhotoeditPlayerinfoID), index)).GetName(), 
				PhotoeditSettingID.Date => ((PhotoeditDateID)Enum.ToObject(typeof(PhotoeditDateID), index)).GetName(), 
				PhotoeditSettingID.ShopName => ((PhotoeditShopnameID)Enum.ToObject(typeof(PhotoeditShopnameID), index)).GetName(), 
				PhotoeditSettingID.Stamp => ((PhotoeditStampID)Enum.ToObject(typeof(PhotoeditStampID), index)).GetName(), 
				PhotoeditSettingID.Layout => ((PhotoeditLayoutID)Enum.ToObject(typeof(PhotoeditLayoutID), index)).GetName(), 
				_ => null, 
			};
		}

		private int GetSwitchValueLength(PhotoeditSettingID id)
		{
			switch (id)
			{
			case PhotoeditSettingID.PlayerInfo:
				return PhotoeditPlayerinfoID.End.GetEnd();
			case PhotoeditSettingID.Date:
				return PhotoeditDateID.End.GetEnd();
			case PhotoeditSettingID.ShopName:
				return PhotoeditShopnameID.End.GetEnd();
			case PhotoeditSettingID.Stamp:
				return PhotoeditStampID.End.GetEnd();
			case PhotoeditSettingID.Layout:
				if (_isSinglePlay)
				{
					return PhotoeditLayoutID.End.GetEnd() - 1;
				}
				return PhotoeditLayoutID.End.GetEnd();
			default:
				return 0;
			}
		}

		public bool GetUploaded(int playerIndex)
		{
			return _isUploadeds[playerIndex];
		}

		public Rect[] GetCurrentFaceRects(int playerIndex)
		{
			return _faceDetections[_currentTrackNumbers[playerIndex], 0];
		}

		public PhotoeditSettingID GetCurrentSettingIndex(int playerIndex)
		{
			return _currentSettingIndexs[playerIndex];
		}
	}
}
