using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using DB;
using IO;
using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_000001;
using Manager;
using Manager.MaiStudio;
using Manager.MaiStudio.CardTypeName;
using Manager.UserDatas;
using Process.CodeRead;
using Process.CourseSelect;
using Process.LoginBonus;
using UnityEngine;

namespace Process
{
	public class SimpleSettingProcess : ProcessBase, ISimpleSettingProcess
	{
		private enum SimpleSettingSequence : byte
		{
			Init,
			FadeWait,
			StagingWait,
			Update,
			MessageWait,
			Release
		}

		private enum SubSequence : byte
		{
			FirstInformation,
			Menu,
			Photographing,
			Collection,
			Team,
			InputWait,
			Wait,
			None
		}

		private enum PhotographingSequence : byte
		{
			Confirmation,
			FrameSelect,
			Preparation,
			Photographing,
			Adjustment,
			Cleanup,
			None
		}

		private const string SHUTTER_MATERIAL_NAME = "Materials/CircleFadeMaterial";

		public const int FIRST_INFO_TIME = 10000;

		public const int PHOTO_PREVIEW_TIME = 2000;

		public const int PHOTO_GRAPHING_TIME = 2000;

		public const int PHOTO_COUNTDOWN_INTERVAL = 666;

		private const int ADJUST_MOVE_VALUE = 10;

		private const int MAX_VOICE_COUNT = 3;

		private bool[] _isPreview;

		private bool[] _isCountDownGrace;

		private bool[] _isAgreed;

		private bool[] _isCodeReadUse;

		private Queue<FirstInformationData>[] _firstInformationQueues;

		private int[] _skipTimes;

		private bool[] _isFirstSkipButton;

		private float[] _userTimer;

		private SimpleSettingSequence _sequence;

		private SubSequence[] _subSequence;

		private PhotographingSequence[] _photoSequences;

		private PhotographingSequence[] _prevPhotoSequences;

		private bool[] _isChangePhotographingSuquence;

		private SimpleSettingMonitor[] _monitors;

		private readonly float[] _timers = new float[2];

		private readonly float[] _shootIntervalCounter = new float[2] { 666f, 666f };

		private float _initialTimer;

		private Vector2[] _restrictionSize;

		private List<FramePart> _frameParts;

		private int[] _frameIndex;

		private Vector2[] _adjustPosition;

		private float[] _photoGraphingTimeCounter;

		private int[] _contdowns;

		private int[] _voiceCount;

		private int[] _dxCardTypes;

		private bool[] _photoAggrees;

		private bool[] _isToCodeReadProcess;

		private List<ReadOnlyCollection<PhotoFrameData>> _photoFrameDataList;

		private Texture2D[] _resultTexture;

		private SlideScrollController[] _slideController;

		public SimpleSettingProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public override void OnAddProcess()
		{
		}

		public override void OnStart()
		{
			GameObject prefs = Resources.Load<GameObject>("Process/SimpleSetting/SimpleSettingProcess");
			_monitors = new SimpleSettingMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<SimpleSettingMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<SimpleSettingMonitor>()
			};
			_isAgreed = new bool[_monitors.Length];
			_isPreview = new bool[_monitors.Length];
			_frameIndex = new int[_monitors.Length];
			_contdowns = new int[_monitors.Length];
			_skipTimes = new int[_monitors.Length];
			_dxCardTypes = new int[_monitors.Length];
			_slideController = new SlideScrollController[_monitors.Length];
			_userTimer = new float[_monitors.Length];
			_photoGraphingTimeCounter = new float[_monitors.Length];
			_adjustPosition = new Vector2[_monitors.Length];
			_restrictionSize = new Vector2[_monitors.Length];
			_isFirstSkipButton = new bool[_monitors.Length];
			_subSequence = new SubSequence[_monitors.Length];
			_photoSequences = new PhotographingSequence[_monitors.Length];
			_prevPhotoSequences = new PhotographingSequence[_monitors.Length];
			_isChangePhotographingSuquence = new bool[_monitors.Length];
			_voiceCount = new int[_monitors.Length];
			_isCountDownGrace = new bool[_monitors.Length];
			_resultTexture = new Texture2D[_monitors.Length];
			_frameIndex = new int[_monitors.Length];
			_firstInformationQueues = new Queue<FirstInformationData>[_monitors.Length];
			Material source = Resources.Load<Material>("Materials/CircleFadeMaterial");
			_photoAggrees = new bool[_monitors.Length];
			_isToCodeReadProcess = new bool[_monitors.Length];
			_isCodeReadUse = new bool[_monitors.Length];
			_photoFrameDataList = new List<ReadOnlyCollection<PhotoFrameData>>();
			SortedDictionary<int, List<PhotoFrameData>> sortedDictionary = new SortedDictionary<int, List<PhotoFrameData>>();
			foreach (KeyValuePair<int, PhotoFrameData> photoFrame in Singleton<DataManager>.Instance.GetPhotoFrames())
			{
				int iD = photoFrame.Value.GetID();
				if (Singleton<EventManager>.Instance.IsOpenEvent(photoFrame.Value.eventName.id))
				{
					List<PhotoFrameData> value = new List<PhotoFrameData> { photoFrame.Value };
					sortedDictionary.Add(iD, value);
				}
			}
			foreach (int key in sortedDictionary.Keys)
			{
				ReadOnlyCollection<PhotoFrameData> item = new ReadOnlyCollection<PhotoFrameData>(sortedDictionary[key]);
				_photoFrameDataList.Add(item);
			}
			_frameParts = new List<FramePart>();
			for (int i = 0; i < _photoFrameDataList.Count; i++)
			{
				Texture2D photoFrameTexture2D = container.assetManager.GetPhotoFrameTexture2D(_photoFrameDataList[i][0].imageFile);
				Sprite sprite = Sprite.Create(photoFrameTexture2D, new Rect(0f, 0f, photoFrameTexture2D.width, photoFrameTexture2D.height), new Vector2(0.5f, 0.5f));
				_frameParts.Add(new FramePart(_photoFrameDataList[i][0].name.str, sprite, photoFrameTexture2D));
			}
			for (int j = 0; j < _monitors.Length; j++)
			{
				bool isEntry = Singleton<UserDataManager>.Instance.GetUserData(j).IsEntry;
				_monitors[j].PreInitialize(this);
				_monitors[j].Initialize(j, isEntry);
				_firstInformationQueues[j] = new Queue<FirstInformationData>();
				_userTimer[j] = 0f;
				_isFirstSkipButton[j] = false;
				_dxCardTypes[j] = 0;
				_frameIndex[j] = 0;
				_isPreview[j] = false;
				_isAgreed[j] = false;
				_isCodeReadUse[j] = false;
				_adjustPosition[j] = Vector2.zero;
				_prevPhotoSequences[j] = PhotographingSequence.None;
				_isChangePhotographingSuquence[j] = false;
				_voiceCount[j] = 3;
				_slideController[j] = new SlideScrollController();
				_slideController[j].SetAction(j, SlideScrollLeft, SlideScrollRight);
				_contdowns[j] = 3;
				_subSequence[j] = SubSequence.Menu;
				if (_frameParts.Count == 0)
				{
					_monitors[j].SetFrameVisible(isShow: false);
				}
				else
				{
					_monitors[j].SetFrameTexture(_frameParts[_frameIndex[j]].Tex);
				}
				_monitors[j].SetMaterial(new Material(source));
				if (isEntry)
				{
					UserData userData = Singleton<UserDataManager>.Instance.GetUserData(j);
					UserDetail detail = userData.Detail;
					UserOption option = userData.Option;
					_photoAggrees[j] = userData.Extend.ExtendContendBit.IsFlagOn(ExtendContentBitID.PhotoAgree);
					_isToCodeReadProcess[j] = userData.Extend.ExtendContendBit.IsFlagOn(ExtendContentBitID.GotoCodeRead) && !userData.IsGuest();
					Texture2D iconTexture2D = container.assetManager.GetIconTexture2D(j, detail.EquipIconID);
					bool flag = false;
					container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20010, j, true, flag));
					MessageUserInformationData messageUserInformationData = new MessageUserInformationData(j, container.assetManager, detail, option.DispRate, isSubMonitor: true);
					container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20011, j, messageUserInformationData));
					container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20030, j, true));
					container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30008, j, option.SubmonitorAppeal));
					_monitors[j].SetUserData(iconTexture2D, detail.UserName, (int)detail.Rating, (int)option.DispRate, detail.TotalAwake);
					List<int> selectedCardList = userData.Extend.SelectedCardList;
					if (selectedCardList.Count > 0)
					{
						if (selectedCardList[0] == 0)
						{
							_monitors[j].SetUserDxPathStatus(CodeReadProcess.CardStatus.Unowned);
						}
						else
						{
							CodeReadProcess.CardStatus status;
							UserCard userCard = CodeReadProcess.GetUserCard(j, selectedCardList[0], out status);
							string dxCardTypeText = CodeReadProcess.GetDxCardTypeText((Table)selectedCardList[0]);
							_dxCardTypes[j] = selectedCardList[0];
							Sprite frame = Resources.Load<Sprite>("Process/Entry/Sprites/DXPass/UI_ENT_DXPass_" + dxCardTypeText);
							Material material = Resources.Load<Material>("CMN_Card/FX_CMN_Card_" + dxCardTypeText + "_Pattern");
							bool flag2 = CodeReadProcess.IsInPeriod(userCard.startDate, userCard.endDataDate) && status == CodeReadProcess.CardStatus.Normal;
							_monitors[j].SetUserDxPathData(frame, material);
							_monitors[j].SetUserDxPathStatus((!flag2) ? CodeReadProcess.CardStatus.Expired : CodeReadProcess.CardStatus.Normal);
						}
					}
					_monitors[j].SetCodeReadState(_isToCodeReadProcess[j]);
					_monitors[j].SetPhotoAggreesState(_photoAggrees[j]);
					_monitors[j].IsCodeReadUse = (_isCodeReadUse[j] = !userData.IsGuest() && !GameManager.IsEventMode);
				}
				else
				{
					_subSequence[j] = SubSequence.Wait;
				}
				MechaManager.Jvs.SetPwmOutput((byte)j, CommonScriptable.GetLedSetting().BillboardPhotoColor);
			}
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			switch (_sequence)
			{
			case SimpleSettingSequence.Init:
				_sequence = SimpleSettingSequence.FadeWait;
				break;
			case SimpleSettingSequence.FadeWait:
			{
				_initialTimer += Time.deltaTime;
				if (!(_initialTimer > 1f))
				{
					break;
				}
				_initialTimer = 0f;
				container.processManager.NotificationFadeIn();
				for (int j = 0; j < _monitors.Length; j++)
				{
					UserData userData = Singleton<UserDataManager>.Instance.GetUserData(j);
					if (!userData.IsEntry)
					{
						continue;
					}
					SetInputLockInfo(j, 550f);
					if (!userData.Detail.ContentBit.IsFlagOn(ContentBitID.FirstPassportDxPlus))
					{
						_firstInformationQueues[j].Enqueue(new FirstInformationData(WindowMessageID.PassportCamera, Mai2.Voice_000001.Cue.VO_000228, 3000));
						userData.Detail.ContentBit.SetFlag(ContentBitID.FirstPassportDxPlus, flag: true);
					}
					bool flag = false;
					long unixTime = TimeManager.GetUnixTime(Singleton<UserDataManager>.Instance.GetUserData(j).Detail.EventWatchedDate);
					foreach (ReadOnlyCollection<PhotoFrameData> photoFrameData in _photoFrameDataList)
					{
						if (Singleton<EventManager>.Instance.IsDateNewAndOpen(photoFrameData[0].eventName.id, unixTime))
						{
							flag = true;
							break;
						}
					}
					if (!userData.IsGuest() && flag)
					{
						_firstInformationQueues[j].Enqueue(new FirstInformationData(WindowMessageID.SinmpeSettingNewFrame, Mai2.Voice_000001.Cue.VO_000176, 1000));
					}
					if (_firstInformationQueues[j].Count > 0)
					{
						_subSequence[j] = SubSequence.FirstInformation;
						_monitors[j].SetFirstInformation();
						ShowFirstInformation(j);
						continue;
					}
					_subSequence[j] = SubSequence.InputWait;
					_monitors[j].Play(delegate(int index)
					{
						_subSequence[index] = SubSequence.Menu;
						CheckTimer();
					});
				}
				_sequence = SimpleSettingSequence.StagingWait;
				break;
			}
			case SimpleSettingSequence.StagingWait:
				if (_initialTimer >= 1000f)
				{
					_initialTimer = 0f;
					_sequence = SimpleSettingSequence.Update;
				}
				else
				{
					_initialTimer += GameManager.GetGameMSecAdd();
				}
				break;
			case SimpleSettingSequence.Update:
			{
				for (int i = 0; i < _monitors.Length; i++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry)
					{
						switch (_subSequence[i])
						{
						case SubSequence.FirstInformation:
							InputFirstInformation(i);
							break;
						case SubSequence.Menu:
							InputMenu(i);
							break;
						}
						UpdateDetail(i);
					}
				}
				break;
			}
			case SimpleSettingSequence.MessageWait:
				UpdateMessageWait();
				break;
			}
			for (int k = 0; k < _monitors.Length; k++)
			{
				if (Singleton<UserDataManager>.Instance.GetUserData(k).IsEntry)
				{
					_monitors[k].ViewUpdate();
				}
			}
		}

		public override void OnLateUpdate()
		{
		}

		private void ShowFirstInformation(int playerIndex)
		{
			FirstInformationData firstInformationData = _firstInformationQueues[playerIndex].Dequeue();
			container.processManager.EnqueueMessage(playerIndex, firstInformationData.MessageID);
			SoundManager.PlayVoice(firstInformationData.VoiceCue, playerIndex);
			_skipTimes[playerIndex] = firstInformationData.SkipButtonTime;
		}

		private void UpdateDetail(int i)
		{
			switch (_subSequence[i])
			{
			case SubSequence.FirstInformation:
				UpdateFirstInformation(i);
				break;
			case SubSequence.Menu:
			case SubSequence.Photographing:
			case SubSequence.Collection:
			case SubSequence.Team:
			case SubSequence.InputWait:
			case SubSequence.Wait:
				break;
			}
		}

		private void UpdateMessageWait()
		{
			if (_initialTimer >= 3f)
			{
				_initialTimer = 0f;
				GotoNextProcess();
			}
			_initialTimer += Time.deltaTime;
		}

		private void MessageAction(int i)
		{
			container.processManager.EnqueueMessage(i, WindowMessageID.IconPhotoContract);
		}

		private void InputFirstInformation(int i)
		{
			if (!IsInputLock(i) && _isFirstSkipButton[i] && _userTimer[i] >= 1000f && InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button04))
			{
				_monitors[i].PressedButton(InputManager.ButtonSetting.Button04);
				_userTimer[i] = 10000f;
				SetInputLockInfo(i, 2000f);
			}
		}

		private void UpdateFirstInformation(int i)
		{
			if (!_isFirstSkipButton[i] && _userTimer[i] > (float)_skipTimes[i])
			{
				_isFirstSkipButton[i] = true;
				_monitors[i].SetFirstInformationSkip();
			}
			if (_userTimer[i] >= 10000f)
			{
				container.processManager.CloseWindow(i);
				_monitors[i].SetFirstInformation();
				if (_firstInformationQueues[i].Count > 0)
				{
					_userTimer[i] = 0f;
					_isFirstSkipButton[i] = false;
					ShowFirstInformation(i);
					return;
				}
				_subSequence[i] = SubSequence.InputWait;
				_monitors[i].Play(delegate(int index)
				{
					_subSequence[index] = SubSequence.Menu;
					CheckTimer();
				});
			}
			else
			{
				_userTimer[i] += GameManager.GetGameMSecAdd();
			}
		}

		private void CheckTimer()
		{
			if ((_subSequence[0] == SubSequence.Menu || _subSequence[0] == SubSequence.Wait) && (_subSequence[1] == SubSequence.Menu || _subSequence[1] == SubSequence.Wait))
			{
				container.processManager.PrepareTimer(99, 0, isEntry: false, GotoNextProcess);
			}
		}

		private void InputMenu(int i)
		{
			if (IsInputLock(i))
			{
				return;
			}
			if (WebCamManager.IsAvailableCamera() && InputManager.GetInputDown(i, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6))
			{
				_monitors[i].PressedButton(InputManager.ButtonSetting.Button06);
				if (_isAgreed[i])
				{
					_monitors[i].SetMenu2FrameSelectActive(isActive: true);
					_monitors[i].ChangeFrameName(_frameParts[_frameIndex[i]].Name);
					ChangePhotographingSequence(PhotographingSequence.FrameSelect, i);
					SetInputLockInfo(i, 2000f);
				}
				else
				{
					_monitors[i].SetMenu2Contract(isActive: true, MessageAction);
					ChangePhotographingSequence(PhotographingSequence.Confirmation, i);
					SetInputLockInfo(i, 2000f);
				}
				_subSequence[i] = SubSequence.Photographing;
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, i);
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20010, i, false, false));
			}
			else if (InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button04))
			{
				if (_subSequence[i] == SubSequence.Wait)
				{
					return;
				}
				_subSequence[i] = SubSequence.Wait;
				_monitors[i].PressedButton(InputManager.ButtonSetting.Button04);
				if (_subSequence[0] == SubSequence.Wait && _subSequence[1] == SubSequence.Wait)
				{
					if (_sequence != SimpleSettingSequence.Release)
					{
						GotoNextProcess();
					}
				}
				else
				{
					container.processManager.EnqueueMessage(i, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
					_monitors[i].SetMenu2Wait();
				}
			}
			else if (InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button02) || InputManager.GetButtonLongPush(i, InputManager.ButtonSetting.Button02, 200L))
			{
				OptionHeadphonevolumeID headPhoneVolume = Singleton<UserDataManager>.Instance.GetUserData(i).Option.HeadPhoneVolume;
				if (headPhoneVolume + 1 < OptionHeadphonevolumeID.End)
				{
					headPhoneVolume++;
					_monitors[i].SetVolume(headPhoneVolume);
					SoundManager.SetHeadPhoneVolume(i, headPhoneVolume.GetValue());
					Singleton<UserDataManager>.Instance.GetUserData(i).Option.HeadPhoneVolume = headPhoneVolume;
					SetInputLockInfo(i, 100f);
					_monitors[i].PressedButton(InputManager.ButtonSetting.Button02);
					CheckVolumeButton(i, headPhoneVolume);
				}
			}
			else if (InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button03) || InputManager.GetButtonLongPush(i, InputManager.ButtonSetting.Button03, 200L))
			{
				OptionHeadphonevolumeID headPhoneVolume2 = Singleton<UserDataManager>.Instance.GetUserData(i).Option.HeadPhoneVolume;
				if (headPhoneVolume2 - 1 >= OptionHeadphonevolumeID.Vol1)
				{
					headPhoneVolume2--;
					_monitors[i].SetVolume(headPhoneVolume2);
					SoundManager.SetHeadPhoneVolume(i, headPhoneVolume2.GetValue());
					Singleton<UserDataManager>.Instance.GetUserData(i).Option.HeadPhoneVolume = headPhoneVolume2;
					SetInputLockInfo(i, 100f);
					_monitors[i].PressedButton(InputManager.ButtonSetting.Button03);
					CheckVolumeButton(i, headPhoneVolume2);
				}
			}
			else if (_isCodeReadUse[i] && InputManager.GetInputDown(i, InputManager.ButtonSetting.Button01, InputManager.TouchPanelArea.A1))
			{
				_isToCodeReadProcess[i] = !_isToCodeReadProcess[i];
				SetInputLockInfo(i, 200f);
				_monitors[i].SetCodeReadToggleButton(_isToCodeReadProcess[i]);
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, i);
			}
			else if (CameraManager.IsAvailableCamera && InputManager.GetTouchPanelAreaDown(i, InputManager.TouchPanelArea.B4, InputManager.TouchPanelArea.E4))
			{
				_photoAggrees[i] = !_photoAggrees[i];
				SetInputLockInfo(i, 200f);
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, i);
				_monitors[i].SetPhotoAgreesToggleButton(_photoAggrees[i]);
			}
		}

		private void CheckVolumeButton(int playerIndex, OptionHeadphonevolumeID volume)
		{
			switch (volume)
			{
			case OptionHeadphonevolumeID.Vol1:
				_monitors[playerIndex].SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button03);
				break;
			case OptionHeadphonevolumeID.Vol20:
				_monitors[playerIndex].SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button02);
				break;
			default:
				_monitors[playerIndex].SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button02);
				_monitors[playerIndex].SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button03);
				break;
			}
		}

		private void InputPhotographing(int i)
		{
			switch (_photoSequences[i])
			{
			case PhotographingSequence.Confirmation:
				InputConfirmation(i);
				break;
			case PhotographingSequence.FrameSelect:
				InputFrameSelect(i);
				break;
			case PhotographingSequence.Preparation:
				InputPreparation(i);
				break;
			case PhotographingSequence.Adjustment:
				InputAdjustment(i);
				break;
			case PhotographingSequence.Photographing:
			case PhotographingSequence.Cleanup:
				break;
			}
		}

		private void GotoNextProcess()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(i);
				userData.Extend.ExtendContendBit.SetFlag(ExtendContentBitID.PhotoAgree, _photoAggrees[i]);
				userData.Extend.ExtendContendBit.SetFlag(ExtendContentBitID.GotoCodeRead, _isToCodeReadProcess[i]);
			}
			GameManager.IsPhotoAgree = (GameManager.IsGotoPhotoShoot = _photoAggrees[0] || _photoAggrees[1]);
			GameManager.IsGotoCodeRead = _isToCodeReadProcess[0] || _isToCodeReadProcess[1];
			bool flag2 = false;
			for (int j = 0; j < 2; j++)
			{
				UserData userData2 = Singleton<UserDataManager>.Instance.GetUserData(j);
				if (userData2.IsEntry && !userData2.IsGuest() && userData2.Detail.PlayCount >= 1)
				{
					flag2 = true;
				}
			}
			if (GameManager.IsEventMode)
			{
				flag2 = false;
			}
			GameManager.SetMaxTrack();
			for (int k = 0; k < _monitors.Length; k++)
			{
				CodeReadProcess.CardStatus status;
				UserCard userCard = CodeReadProcess.GetUserCard(k, _dxCardTypes[k], out status);
				if (userCard != null)
				{
					if (CodeReadProcess.IsInPeriod(userCard.startDate, userCard.endDataDate))
					{
						_ = status == CodeReadProcess.CardStatus.Normal;
					}
					else
						_ = 0;
				}
				if (Singleton<UserDataManager>.Instance.GetUserData(k).Detail.CardType > 1)
				{
					string dxCardTypeText = CodeReadProcess.GetDxCardTypeText((Table)Singleton<UserDataManager>.Instance.GetUserData(k).Detail.CardType);
					Sprite sprite = Resources.Load<Sprite>("Process/Common/Sprites/UpperMonitor/UI_CMN_DXPass_" + dxCardTypeText);
					container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30005, k, sprite));
				}
				CardTypeData cardType = Singleton<DataManager>.Instance.GetCardType(Singleton<UserDataManager>.Instance.GetUserData(k).Detail.CardType);
				if (cardType != null)
				{
					GameManager.SetCardEffect(cardType.extendBitParameter, k);
				}
			}
			if (GameManager.IsEventMode)
			{
				if (GameManager.IsCourseMode)
				{
					container.processManager.AddProcess(new FadeProcess(container, this, new CourseSelectProcess(container)), 50);
				}
				else
				{
					container.processManager.AddProcess(new FadeProcess(container, this, new MusicSelectProcess(container)), 50);
				}
			}
			if (flag2)
			{
				container.processManager.AddProcess(new FadeProcess(container, this, new LoginBonusProcess(container)), 50);
			}
			else if (GameManager.IsGotoCodeRead)
			{
				container.processManager.AddProcess(new FadeProcess(container, this, new CodeReadProcess(container), FadeProcess.FadeType.Type3), 50);
			}
			else if (GameManager.IsGotoPhotoShoot)
			{
				container.processManager.AddProcess(new FadeProcess(container, this, new PhotoShootProcess(container)), 50);
			}
			else
			{
				container.processManager.AddProcess(new FadeProcess(container, this, new RegionalSelectProcess(container)), 50);
			}
			SoundManager.PlayBGM(Mai2.Mai2Cue.Cue.BGM_ENTRY, 2);
			container.processManager.SetVisibleTimers(isVisible: false);
			container.processManager.ClearTimeoutAction();
			_sequence = SimpleSettingSequence.Release;
		}

		private void UpdatePhotographing(int i)
		{
			EnterPhotographingSequence(_photoSequences[i], i);
			switch (_photoSequences[i])
			{
			case PhotographingSequence.Photographing:
				UpdatePhotograph(i);
				break;
			case PhotographingSequence.Cleanup:
				UpdateCleanUp(i);
				break;
			case PhotographingSequence.FrameSelect:
				UpdateAutoScroll();
				break;
			case PhotographingSequence.Preparation:
			case PhotographingSequence.Adjustment:
				break;
			}
		}

		private void InputConfirmation(int i)
		{
			if (!IsInputLock(i))
			{
				if (InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button05))
				{
					ChangePhotographingSequence(PhotographingSequence.None, i);
					_subSequence[i] = SubSequence.Menu;
					_monitors[i].PressedButton(InputManager.ButtonSetting.Button05);
					_monitors[i].SetMenu2Contract(isActive: false, container.processManager.CloseWindow);
					container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20010, i, true, false));
					SetInputLockInfo(i, 1500f);
				}
				else if (InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button04))
				{
					_isAgreed[i] = true;
					_monitors[i].SetMenu2FrameSelectActive(isActive: true);
					_monitors[i].PressedButton(InputManager.ButtonSetting.Button04);
					_monitors[i].ChangeFrameName(_frameParts[_frameIndex[i]].Name);
					ChangePhotographingSequence(PhotographingSequence.FrameSelect, i);
					container.processManager.CloseWindow(i);
					SetInputLockInfo(i, 1500f);
				}
			}
		}

		private void InputFrameSelect(int i)
		{
			if (!IsInputLock(i))
			{
				if (InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button05))
				{
					_monitors[i].SetMenu2FrameSelectActive(isActive: false);
					ChangePhotographingSequence(PhotographingSequence.None, i);
					_subSequence[i] = SubSequence.Menu;
					_monitors[i].PressedButton(InputManager.ButtonSetting.Button05);
					container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20010, i, true, false));
					SetInputLockInfo(i, 1000f);
				}
				else if (InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button04))
				{
					_monitors[i].SetFrameSelect2PhotoPreparation(isActive: true);
					ChangePhotographingSequence(PhotographingSequence.Preparation, i);
					_monitors[i].PressedButton(InputManager.ButtonSetting.Button04);
					SetInputLockInfo(i, 2000f);
				}
				else if (InputManager.GetInputDown(i, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3) || InputManager.GetInputLongPush(i, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L))
				{
					ScrollLeft(i);
					SetInputLockInfo(i, 100f);
				}
				else if (InputManager.GetInputDown(i, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6) || InputManager.GetInputLongPush(i, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L))
				{
					ScrollRight(i);
					SetInputLockInfo(i, 100f);
				}
				else if (InputManager.SlideAreaLr(i))
				{
					_slideController[i].StartSlideScroll(isToRight: true, 10);
					SetInputLockInfo(i, 100f);
				}
				else if (InputManager.SlideAreaRl(i))
				{
					_slideController[i].StartSlideScroll(isToRight: false, 10);
					SetInputLockInfo(i, 100f);
				}
			}
		}

		private void InputPreparation(int i)
		{
			if (!IsInputLock(i))
			{
				if (InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button05))
				{
					ChangePhotographingSequence(PhotographingSequence.FrameSelect, i);
					_monitors[i].SetFrameSelect2PhotoPreparation(isActive: false);
					SetInputLockInfo(i, 2000f);
					_monitors[i].PressedButton(InputManager.ButtonSetting.Button05);
				}
				else if (InputManager.GetInputDown(i, InputManager.ButtonSetting.Button01, InputManager.TouchPanelArea.A1) || InputManager.GetInputDown(i, InputManager.ButtonSetting.Button08, InputManager.TouchPanelArea.A8))
				{
					ChangePhotographingSequence(PhotographingSequence.Photographing, i);
					_monitors[i].SetPreparation2Photographing();
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, i);
					_monitors[i].SetPhotoCountDown();
					_isCountDownGrace[i] = false;
					SoundManager.StopBGM(2);
				}
			}
		}

		private void InputAdjustment(int i)
		{
			if (IsInputLock(i))
			{
				return;
			}
			if (InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button05))
			{
				_monitors[i].SetAdjust2Preparation();
				ChangePhotographingSequence(PhotographingSequence.Preparation, i);
				_monitors[i].PressedButton(InputManager.ButtonSetting.Button05);
				ResetAdjustPosition(i);
				SetInputLockInfo(i, 2000f);
			}
			else if (InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button04))
			{
				Capture(i);
				_monitors[i].PressedButton(InputManager.ButtonSetting.Button04);
				ChangePhotographingSequence(PhotographingSequence.Cleanup, i);
				_monitors[i].SetPhotographing2Cleanup();
				_isCountDownGrace[i] = false;
				SetInputLockInfo(i, 2000f);
			}
			else if (InputManager.GetTouchPanelAreaDown(i, InputManager.TouchPanelArea.E1, InputManager.TouchPanelArea.B1, InputManager.TouchPanelArea.B8))
			{
				if (_adjustPosition[i].y + 10f <= _restrictionSize[i].y)
				{
					_adjustPosition[i] += Vector2.up * 10f;
					_monitors[i].UpdateAdjustCapture(_adjustPosition[i], 0);
				}
			}
			else if (InputManager.GetTouchPanelAreaDown(i, InputManager.TouchPanelArea.E3, InputManager.TouchPanelArea.B2, InputManager.TouchPanelArea.B3))
			{
				if (_adjustPosition[i].x + 10f <= _restrictionSize[i].x)
				{
					_adjustPosition[i] += Vector2.right * 10f;
					_monitors[i].UpdateAdjustCapture(_adjustPosition[i], 1);
				}
			}
			else if (InputManager.GetTouchPanelAreaDown(i, InputManager.TouchPanelArea.E5, InputManager.TouchPanelArea.B4, InputManager.TouchPanelArea.B5))
			{
				if (_adjustPosition[i].y - 10f >= 0f - _restrictionSize[i].y)
				{
					_adjustPosition[i] += Vector2.down * 10f;
					_monitors[i].UpdateAdjustCapture(_adjustPosition[i], 2);
				}
			}
			else if (InputManager.GetTouchPanelAreaDown(i, InputManager.TouchPanelArea.E7, InputManager.TouchPanelArea.B6, InputManager.TouchPanelArea.B7) && _adjustPosition[i].x - 10f >= 0f - _restrictionSize[i].x)
			{
				_adjustPosition[i] += Vector2.left * 10f;
				_monitors[i].UpdateAdjustCapture(_adjustPosition[i], 3);
			}
		}

		private void UpdatePhotograph(int i)
		{
			_photoGraphingTimeCounter[i] += GameManager.GetGameMSecAdd();
			if (!_isCountDownGrace[i])
			{
				if (_photoGraphingTimeCounter[i] > 1000f)
				{
					_monitors[i].SetPhotoCountDown(3);
					ChangeCountDownSound(i);
					_isCountDownGrace[i] = true;
					_photoGraphingTimeCounter[i] = 0f;
				}
			}
			else if (_photoGraphingTimeCounter[i] < 2000f && _photoGraphingTimeCounter[i] >= _shootIntervalCounter[i])
			{
				_shootIntervalCounter[i] += 666f;
				ChangeCountDownSound(i);
				_monitors[i].SetPhotoCountDown(--_contdowns[i]);
			}
			else if (_photoGraphingTimeCounter[i] >= 2000f && !_isPreview[i])
			{
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_CAMERA, i);
				SoundManager.PlayBGM(Mai2.Mai2Cue.Cue.BGM_ENTRY, 2);
				_isPreview[i] = true;
				_restrictionSize[i] = new Vector2((float)CameraManager.GameCameraParam.Width * 0.625f / 4f - 160f, (float)CameraManager.GameCameraParam.Height * 0.625f / 2f - 160f);
				_monitors[i].Shoot();
				_monitors[i].SetAdjustLength((int)_restrictionSize[i].x / 10, (int)_restrictionSize[i].y / 10);
				_monitors[i].SetPhotoCountDown(0);
			}
			else if (_photoGraphingTimeCounter[i] >= 2500f)
			{
				ChangePhotographingSequence(PhotographingSequence.Adjustment, i);
				_monitors[i].SetPhotographing2Adjustment();
				_contdowns[i] = 3;
				_isPreview[i] = false;
				_shootIntervalCounter[i] = 666f;
				_photoGraphingTimeCounter[i] = 0f;
				_monitors[i].SetPhotoCountDown(3);
				SetInputLockInfo(i, 1000f);
			}
		}

		private void UpdateCleanUp(int i)
		{
			_timers[i] += GameManager.GetGameMSecAdd();
			if (!_isCountDownGrace[i])
			{
				if (_timers[i] <= 500f)
				{
					_timers[i] = 0f;
					_isCountDownGrace[i] = true;
				}
				return;
			}
			float num = _timers[i] / 2000f;
			if (num >= 1f)
			{
				_monitors[i].CleanupUpdate(1f);
				_timers[i] = 0f;
				ChangePhotographingSequence(PhotographingSequence.None, i);
				_subSequence[i] = SubSequence.Menu;
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20010, i, true, false));
				_monitors[i].SetAdjust2Menu(_resultTexture[i]);
				SendTextureToCommonProcess(i);
				Singleton<UserDataManager>.Instance.GetUserData(i).Detail.EquipIconID = 10;
			}
			else
			{
				_monitors[i].CleanupUpdate(num);
			}
		}

		private void EnterPhotographingSequence(PhotographingSequence sequence, int monitorId)
		{
			if (_isChangePhotographingSuquence[monitorId])
			{
				switch (sequence)
				{
				case PhotographingSequence.Photographing:
					_voiceCount[monitorId] = 3;
					SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000029, monitorId);
					break;
				case PhotographingSequence.Preparation:
					SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000028, monitorId);
					break;
				case PhotographingSequence.Adjustment:
					SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000033, monitorId);
					break;
				case PhotographingSequence.Cleanup:
					SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000034, monitorId);
					break;
				case PhotographingSequence.FrameSelect:
					_slideController[0].Refresh();
					_slideController[1].Refresh();
					SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000027, monitorId);
					break;
				}
				_isChangePhotographingSuquence[monitorId] = false;
			}
		}

		private void ChangeCountDownSound(int monitorId)
		{
			switch (_voiceCount[monitorId])
			{
			case 3:
				SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000030, monitorId);
				break;
			case 2:
				SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000031, monitorId);
				break;
			case 1:
				SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000032, monitorId);
				break;
			}
			SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_CAMERA_COUNT, monitorId);
			_voiceCount[monitorId]--;
		}

		private void ChangePhotographingSequence(PhotographingSequence next, int monitorId)
		{
			if (_prevPhotoSequences[monitorId] != next)
			{
				PhotographingSequence[] photoSequences = _photoSequences;
				PhotographingSequence photographingSequence;
				_prevPhotoSequences[monitorId] = (photographingSequence = next);
				photoSequences[monitorId] = photographingSequence;
				_isChangePhotographingSuquence[monitorId] = true;
			}
		}

		private void Capture(int i)
		{
			Texture2D tex = _frameParts[_frameIndex[i]].Tex;
			_resultTexture[i] = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, mipChain: false);
			int width = _resultTexture[i].width;
			int height = _resultTexture[i].height;
			Color[] array = new Color[width * height];
			int x = (int)((float)((i == 0) ? (CameraManager.GameCameraParam.Width - CameraManager.GameCameraParam.Width / 4) : (CameraManager.GameCameraParam.Width / 4)) - ((float)width / 2f + _adjustPosition[i].x / 0.625f));
			int y = (int)((float)CameraManager.GameCameraParam.Height / 2f - ((float)height / 2f + _adjustPosition[i].y / 0.625f));
			Color[] pixels = _monitors[i].GetCaptureTexture().GetPixels(x, y, width, height);
			Color[] pixels2 = tex.GetPixels();
			for (int j = 0; j < height; j++)
			{
				for (int k = 0; k < width; k++)
				{
					int num = k + j * width;
					array[num] = pixels2[num] * pixels2[num].a + pixels[num] * (1f - pixels2[num].a);
				}
			}
			_resultTexture[i].SetPixels(array);
			_resultTexture[i].Apply();
			_monitors[i].SetResultTexture(new Vector2(width, height), _resultTexture[i]);
			ResetAdjustPosition(i);
			Texture2D texture2D = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, mipChain: false);
			texture2D.SetPixels(array);
			texture2D.Apply();
			TextureScale.Scale(texture2D, 256, 256);
			GameManager.FaceIconTexture[i] = new Texture2D(texture2D.width, texture2D.height, TextureFormat.ARGB32, mipChain: false);
			GameManager.FaceIconTexture[i].LoadRawTextureData(texture2D.GetRawTextureData());
			GameManager.FaceIconTexture[i].Apply();
			GameManager.IsTakeFaceIcon[i] = true;
			byte[] buffer = texture2D.EncodeToJPG(50);
			using FileStream output = new FileStream(Path.Combine(WebCamManager.GetDataPath(), "UserIcon_" + i + ".jpg"), FileMode.Create, FileAccess.Write);
			using BinaryWriter binaryWriter = new BinaryWriter(output);
			binaryWriter.Write(buffer);
		}

		private void SendTextureToCommonProcess(int monitorId)
		{
			container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30002, monitorId, GameManager.FaceIconTexture[monitorId]));
		}

		public override void OnRelease()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				Object.Destroy(_monitors[i].gameObject);
				MechaManager.Jvs.SetPwmOutput((byte)i, CommonScriptable.GetLedSetting().BillboardMainColor);
				MechaManager.LedIf[i].ButtonLedReset();
			}
			Resources.UnloadUnusedAssets();
		}

		private void ResetAdjustPosition(int i)
		{
			_adjustPosition[i] = Vector2.zero;
			_monitors[i].ResetAdjustPosition();
		}

		public Sprite GetFrameSpriteByAdjustIndex(int monitorIndex, int diff)
		{
			int loadIndex = GetLoadIndex(monitorIndex, diff);
			return _frameParts[loadIndex].Sprite;
		}

		private int GetLoadIndex(int monitorIndex, int diff)
		{
			int num = _frameIndex[monitorIndex] + diff;
			int count = _frameParts.Count;
			while (num >= count)
			{
				num -= count;
			}
			while (num < 0)
			{
				num = count + num;
			}
			return num;
		}

		private void ScrollLeft(int monitorId)
		{
			if (_frameIndex[monitorId] + 1 >= _frameParts.Count)
			{
				_frameIndex[monitorId] = 0;
			}
			else
			{
				_frameIndex[monitorId]++;
			}
			_monitors[monitorId].Scroll(0, _frameParts[_frameIndex[monitorId]].Tex, _frameParts[_frameIndex[monitorId]].Name);
		}

		private void ScrollRight(int monitorId)
		{
			if (_frameIndex[monitorId] - 1 < 0)
			{
				_frameIndex[monitorId] = _frameParts.Count - 1;
			}
			else
			{
				_frameIndex[monitorId]--;
			}
			_monitors[monitorId].Scroll(1, _frameParts[_frameIndex[monitorId]].Tex, _frameParts[_frameIndex[monitorId]].Name);
		}

		private void SlideScrollLeft(int monitorId)
		{
			if (_frameIndex[monitorId] + 1 >= _frameParts.Count)
			{
				_frameIndex[monitorId] = 0;
			}
			else
			{
				_frameIndex[monitorId]++;
			}
			_monitors[monitorId].SlideScroll(0, _frameParts[_frameIndex[monitorId]].Tex, _frameParts[_frameIndex[monitorId]].Name);
		}

		private void SlideScrollRight(int monitorId)
		{
			if (_frameIndex[monitorId] - 1 < 0)
			{
				_frameIndex[monitorId] = _frameParts.Count - 1;
			}
			else
			{
				_frameIndex[monitorId]--;
			}
			_monitors[monitorId].SlideScroll(1, _frameParts[_frameIndex[monitorId]].Tex, _frameParts[_frameIndex[monitorId]].Name);
		}

		private void UpdateAutoScroll()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry)
				{
					_slideController[i].UpdateAutoScroll();
				}
			}
		}

		public void SetTimerVisible(int playerIndex, bool isVisible)
		{
			container.processManager.SetVisibleTimer(playerIndex, isVisible);
		}
	}
}
