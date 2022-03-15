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
using Manager.UserDatas;
using Process;
using UnityEngine;

public class PhotoShootProcess : ProcessBase, IPhotoShootProcess
{
	private enum MainSequence
	{
		Init,
		FadeIn,
		Update,
		Release
	}

	private enum PhotoShootSequence : byte
	{
		FirstInformation,
		Confirmation,
		FrameSelect,
		Preparation,
		Photographing,
		Adjustment,
		Cleanup,
		Cancel,
		None
	}

	private const string SHUTTER_MATERIAL_NAME = "Materials/CircleFadeMaterial";

	public const int FIRST_INFO_TIME = 10000;

	private const int MAX_VOICE_COUNT = 3;

	public const int PHOTO_GRAPHING_TIME = 2000;

	public const int PHOTO_COUNTDOWN_INTERVAL = 666;

	private const int ADJUST_MOVE_VALUE = 10;

	private MainSequence _mainSequence;

	private Vector2[] _restrictionSize;

	private Vector2[] _adjustPosition;

	private PhotoShootMonitor[] _monitors;

	private int[] _frameIndex;

	private int[] _voiceCount;

	private int[] _skipTimes;

	private int[] _countDawn;

	private float[] _userTimer;

	private float[] _photoGraphingTimeCounter;

	private readonly float[] _shootIntervalCounter = new float[2] { 666f, 666f };

	private bool[] _canInput;

	private bool[] _isPreview;

	private bool[] _isChangePhotographingSequence;

	private bool[] _isFirstSkipButton;

	private bool[] _isCountDownGrace;

	private PhotoShootSequence[] _photoSequences;

	private PhotoShootSequence[] _prevPhotoSequences;

	private Queue<FirstInformationData>[] _firstInformationQueues;

	private SlideScrollController[] _slideController;

	private Texture2D[] _resultTexture;

	private List<ReadOnlyCollection<PhotoFrameData>> _photoFrameDataList;

	private List<FramePart> _frameParts;

	private int _counter;

	public PhotoShootProcess(ProcessDataContainer dataContainer)
		: base(dataContainer)
	{
	}

	public override void OnAddProcess()
	{
	}

	public override void OnStart()
	{
		WebCamManager.Play();
		GameObject prefs = Resources.Load<GameObject>("Process/PhotoShoot/PhotoShootProcess");
		_monitors = new PhotoShootMonitor[2]
		{
			CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<PhotoShootMonitor>(),
			CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<PhotoShootMonitor>()
		};
		_frameIndex = new int[_monitors.Length];
		_voiceCount = new int[_monitors.Length];
		_skipTimes = new int[_monitors.Length];
		_countDawn = new int[_monitors.Length];
		_userTimer = new float[_monitors.Length];
		_photoGraphingTimeCounter = new float[_monitors.Length];
		_canInput = new bool[_monitors.Length];
		_isPreview = new bool[_monitors.Length];
		_isFirstSkipButton = new bool[_monitors.Length];
		_isCountDownGrace = new bool[_monitors.Length];
		_isChangePhotographingSequence = new bool[_monitors.Length];
		_slideController = new SlideScrollController[_monitors.Length];
		_resultTexture = new Texture2D[_monitors.Length];
		_adjustPosition = new Vector2[_monitors.Length];
		_restrictionSize = new Vector2[_monitors.Length];
		_photoSequences = new PhotoShootSequence[_monitors.Length];
		_prevPhotoSequences = new PhotoShootSequence[_monitors.Length];
		_firstInformationQueues = new Queue<FirstInformationData>[_monitors.Length];
		Material source = Resources.Load<Material>("Materials/CircleFadeMaterial");
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
			MechaManager.Jvs.SetPwmOutput((byte)j, CommonScriptable.GetLedSetting().BillboardPhotoColor);
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(j);
			_monitors[j].Initialize(j, userData.IsEntry);
			_monitors[j].SetData(this);
			_slideController[j] = new SlideScrollController();
			_slideController[j].SetAction(j, SlideScrollLeft, SlideScrollRight);
			_firstInformationQueues[j] = new Queue<FirstInformationData>();
			_countDawn[j] = 3;
			_skipTimes[j] = 0;
			_userTimer[j] = 0f;
			_isPreview[j] = false;
			_isFirstSkipButton[j] = false;
			_adjustPosition[j] = Vector2.zero;
			_monitors[j].SetMaterial(new Material(source));
			if (_frameParts.Count == 0)
			{
				_monitors[j].SetFrameVisible(isShow: false);
			}
			else
			{
				_monitors[j].SetFrameTexture(_frameParts[_frameIndex[j]].Tex);
			}
			if (userData.IsEntry)
			{
				UserDetail detail = userData.Detail;
				UserOption option = userData.Option;
				_monitors[j].SetUserData(null, detail.UserName, (int)detail.Rating, (int)option.DispRate, detail.TotalAwake);
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
				_canInput[j] = false;
			}
			_photoSequences[j] = PhotoShootSequence.None;
			_prevPhotoSequences[j] = PhotoShootSequence.None;
		}
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		switch (_mainSequence)
		{
		case MainSequence.Init:
			_counter = 0;
			_mainSequence = MainSequence.FadeIn;
			break;
		case MainSequence.FadeIn:
			if (3 < _counter)
			{
				_mainSequence = MainSequence.Update;
				container.processManager.NotificationFadeIn();
				for (int j = 0; j < _monitors.Length; j++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(j).IsEntry)
					{
						if (0 < _firstInformationQueues[j].Count)
						{
							ChangePhotographingSequence(PhotoShootSequence.FirstInformation, j);
							_monitors[j].SetFirstInformation();
							ShowFirstInformation(j);
						}
						else
						{
							ChangePhotographingSequence(PhotoShootSequence.Confirmation, j);
							_monitors[j].SetContract(isActive: true, MessageAction, OnFinishConfirmationIn);
							container.processManager.PrepareTimer(99, 0, isEntry: false, GotoNextProcess);
						}
					}
				}
			}
			else
			{
				_counter++;
			}
			break;
		case MainSequence.Update:
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				UpdatePhotographing(i);
				_monitors[i].ViewUpdate();
			}
			break;
		}
		case MainSequence.Release:
			break;
		}
	}

	public override void OnLateUpdate()
	{
	}

	public override void OnRelease()
	{
		WebCamManager.Pause();
		for (int i = 0; i < _monitors.Length; i++)
		{
			Object.Destroy(_monitors[i].gameObject);
		}
	}

	protected override void UpdateInput(int id)
	{
		base.UpdateInput(id);
		InputPhotographing(id);
	}

	private void InputPhotographing(int i)
	{
		if (_canInput[i])
		{
			switch (_photoSequences[i])
			{
			case PhotoShootSequence.FirstInformation:
				InputFirstInformation(i);
				break;
			case PhotoShootSequence.Confirmation:
				InputConfirmation(i);
				break;
			case PhotoShootSequence.FrameSelect:
				InputFrameSelect(i);
				break;
			case PhotoShootSequence.Preparation:
				InputPreparation(i);
				break;
			case PhotoShootSequence.Adjustment:
				InputAdjustment(i);
				break;
			case PhotoShootSequence.Cancel:
				InputCancelConfirm(i);
				break;
			case PhotoShootSequence.Photographing:
			case PhotoShootSequence.Cleanup:
				break;
			}
		}
	}

	private void UpdatePhotographing(int i)
	{
		EnterPhotographingSequence(_photoSequences[i], i);
		switch (_photoSequences[i])
		{
		case PhotoShootSequence.FirstInformation:
			UpdateFirstInformation(i);
			break;
		case PhotoShootSequence.FrameSelect:
			UpdateAutoScroll();
			break;
		case PhotoShootSequence.Photographing:
			UpdatePhotograph(i);
			break;
		case PhotoShootSequence.Cleanup:
			UpdateCleanUp(i);
			break;
		case PhotoShootSequence.Confirmation:
		case PhotoShootSequence.Preparation:
		case PhotoShootSequence.Adjustment:
			break;
		}
	}

	private void ShowFirstInformation(int playerIndex)
	{
		FirstInformationData firstInformationData = _firstInformationQueues[playerIndex].Dequeue();
		container.processManager.EnqueueMessage(playerIndex, firstInformationData.MessageID);
		SoundManager.PlayVoice(firstInformationData.VoiceCue, playerIndex);
		_skipTimes[playerIndex] = firstInformationData.SkipButtonTime;
	}

	private void MessageAction(int i)
	{
		container.processManager.EnqueueMessage(i, WindowMessageID.IconPhotoContract);
	}

	private void OnFinishConfirmationIn(int i)
	{
		_canInput[i] = true;
	}

	private void UpdateFirstInformation(int i)
	{
		if (!_isFirstSkipButton[i] && _userTimer[i] > (float)_skipTimes[i])
		{
			_isFirstSkipButton[i] = true;
			_monitors[i].SetFirstInformationSkip();
			OnFinishConfirmationIn(i);
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
			}
			else
			{
				ChangePhotographingSequence(PhotoShootSequence.Confirmation, i);
				container.processManager.PrepareTimer(99, 0, isEntry: false, GotoNextProcess);
				_monitors[i].SetContract(isActive: true, MessageAction, OnFinishConfirmationIn);
				SetInputLockInfo(i, 1000f);
			}
		}
		else
		{
			_userTimer[i] += GameManager.GetGameMSecAdd();
		}
	}

	private void InputFirstInformation(int i)
	{
		if (!IsInputLock(i) && _isFirstSkipButton[i] && _userTimer[i] >= 1000f && InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button04))
		{
			_monitors[i].PressedButton(InputManager.ButtonSetting.Button04);
			_userTimer[i] = 10000f;
			SetInputLockInfo(i, 1000f);
		}
	}

	private void InputConfirmation(int i)
	{
		if (!IsInputLock(i))
		{
			if (InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button05))
			{
				ChangePhotographingSequence(PhotoShootSequence.None, i);
				_monitors[i].PressedButton(InputManager.ButtonSetting.Button05);
				_monitors[i].SetContract(isActive: false, container.processManager.CloseWindow, OnFinishConfirmationIn);
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CANCEL, i);
				SetReady(i);
				SetInputLockInfo(i, 1000f);
			}
			else if (InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button04))
			{
				_monitors[i].SetFrameSelectActive(isActive: true);
				_monitors[i].PressedButton(InputManager.ButtonSetting.Button04);
				_monitors[i].ChangeFrameName(_frameParts[_frameIndex[i]].Name);
				ChangePhotographingSequence(PhotoShootSequence.FrameSelect, i);
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, i);
				container.processManager.CloseWindow(i);
				SetInputLockInfo(i, 1500f);
			}
		}
	}

	private void InputCancelConfirm(int i)
	{
		if (InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button05))
		{
			ChangePhotographingSequence(PhotoShootSequence.FrameSelect, i);
			_monitors[i].SetFrameSelectActive(isActive: true);
			_monitors[i].PressedButton(InputManager.ButtonSetting.Button05);
			SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CANCEL, i);
			container.processManager.CloseWindow(i);
			SetInputLockInfo(i, 1500f);
		}
		else if (InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button04))
		{
			ChangePhotographingSequence(PhotoShootSequence.None, i);
			_monitors[i].PressedButton(InputManager.ButtonSetting.Button04);
			SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, i);
			container.processManager.CloseWindow(i);
			SetReady(i);
			SetInputLockInfo(i, 1500f);
		}
	}

	private void InputFrameSelect(int i)
	{
		if (IsInputLock(i))
		{
			return;
		}
		if (InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button05))
		{
			ChangePhotographingSequence(PhotoShootSequence.Cancel, i);
			_monitors[i].PressedButton(InputManager.ButtonSetting.Button05);
			_monitors[i].SetFrameSelect2Cancel(toCancel: true, delegate(int index)
			{
				container.processManager.EnqueueMessage(index, WindowMessageID.PhotoShootCancel);
			});
			SetInputLockInfo(i, 1500f);
		}
		else if (InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button04))
		{
			_monitors[i].SetFrameSelect2PhotoPreparation(isActive: true);
			ChangePhotographingSequence(PhotoShootSequence.Preparation, i);
			_monitors[i].PressedButton(InputManager.ButtonSetting.Button04);
			SetInputLockInfo(i, 1500f);
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

	private void InputPreparation(int i)
	{
		if (!IsInputLock(i))
		{
			if (InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button05))
			{
				ChangePhotographingSequence(PhotoShootSequence.FrameSelect, i);
				_monitors[i].SetFrameSelect2PhotoPreparation(isActive: false);
				SetInputLockInfo(i, 2000f);
				_monitors[i].PressedButton(InputManager.ButtonSetting.Button05);
			}
			else if (InputManager.GetInputDown(i, InputManager.ButtonSetting.Button01, InputManager.TouchPanelArea.A1) || InputManager.GetInputDown(i, InputManager.ButtonSetting.Button08, InputManager.TouchPanelArea.A8))
			{
				ChangePhotographingSequence(PhotoShootSequence.Photographing, i);
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
			ChangePhotographingSequence(PhotoShootSequence.Preparation, i);
			_monitors[i].PressedButton(InputManager.ButtonSetting.Button05);
			ResetAdjustPosition(i);
			SetInputLockInfo(i, 2000f);
		}
		else if (InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button04))
		{
			CaptureDevelop(i);
			_userTimer[i] = 0f;
			_monitors[i].PressedButton(InputManager.ButtonSetting.Button04);
			ChangePhotographingSequence(PhotoShootSequence.Cleanup, i);
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
			_monitors[i].SetPhotoCountDown(--_countDawn[i]);
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
			ChangePhotographingSequence(PhotoShootSequence.Adjustment, i);
			_monitors[i].SetPhotographing2Adjustment();
			_countDawn[i] = 3;
			_isPreview[i] = false;
			_shootIntervalCounter[i] = 666f;
			_photoGraphingTimeCounter[i] = 0f;
			_monitors[i].SetPhotoCountDown(3);
			SetInputLockInfo(i, 1000f);
		}
	}

	private void UpdateCleanUp(int i)
	{
		_userTimer[i] += GameManager.GetGameMSecAdd();
		float num = _userTimer[i] / 2500f;
		if (num >= 1f)
		{
			_monitors[i].CleanupUpdate(1f);
			_userTimer[i] = 0f;
			ChangePhotographingSequence(PhotoShootSequence.None, i);
			_monitors[i].SetAdjust2Menu(_resultTexture[i]);
			SetReady(i);
			SendTextureToCommonProcess(i);
			Singleton<UserDataManager>.Instance.GetUserData(i).Detail.EquipIconID = 10;
		}
		else
		{
			_monitors[i].CleanupUpdate(num);
		}
	}

	private void EnterPhotographingSequence(PhotoShootSequence sequence, int monitorId)
	{
		if (_isChangePhotographingSequence[monitorId])
		{
			switch (sequence)
			{
			case PhotoShootSequence.Photographing:
				_voiceCount[monitorId] = 3;
				SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000029, monitorId);
				break;
			case PhotoShootSequence.Preparation:
				SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000028, monitorId);
				break;
			case PhotoShootSequence.Adjustment:
				SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000033, monitorId);
				break;
			case PhotoShootSequence.Cleanup:
				SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000034, monitorId);
				break;
			case PhotoShootSequence.FrameSelect:
				_slideController[0].Refresh();
				_slideController[1].Refresh();
				SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000027, monitorId);
				break;
			}
			_isChangePhotographingSequence[monitorId] = false;
		}
	}

	private void ChangePhotographingSequence(PhotoShootSequence next, int monitorId)
	{
		if (_prevPhotoSequences[monitorId] != next)
		{
			PhotoShootSequence[] photoSequences = _photoSequences;
			PhotoShootSequence photoShootSequence;
			_prevPhotoSequences[monitorId] = (photoShootSequence = next);
			photoSequences[monitorId] = photoShootSequence;
			_isChangePhotographingSequence[monitorId] = true;
		}
	}

	private void SetReady(int i)
	{
		ChangePhotographingSequence(PhotoShootSequence.None, i);
		if (_photoSequences[0] == PhotoShootSequence.None && _photoSequences[1] == PhotoShootSequence.None)
		{
			MechaManager.LedIf[i].ButtonLedReset();
			if (_mainSequence != MainSequence.Release)
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

	public Sprite GetFrameSpriteByAdjustIndex(int playerIndex, int diff)
	{
		int loadIndex = GetLoadIndex(playerIndex, diff);
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

	private void ResetAdjustPosition(int i)
	{
		_adjustPosition[i] = Vector2.zero;
		_monitors[i].ResetAdjustPosition();
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

	private void SendTextureToCommonProcess(int monitorId)
	{
		container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30002, monitorId, GameManager.FaceIconTexture[monitorId]));
	}

	private void CaptureDevelop(int i)
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

	public void GotoNextProcess()
	{
		if (!GameManager.IsCourseMode && !GameManager.IsFreedomMode)
		{
			container.processManager.AddProcess(new FadeProcess(container, this, new RegionalSelectProcess(container)), 50);
		}
		else
		{
			container.processManager.AddProcess(new FadeProcess(container, this, new GetMusicProcess(container)), 50);
		}
		container.processManager.SetVisibleTimers(isVisible: false);
		container.processManager.ClearTimeoutAction();
		_mainSequence = MainSequence.Release;
	}
}
