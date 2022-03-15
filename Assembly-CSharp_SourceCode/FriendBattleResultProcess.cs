using System;
using System.Collections.Generic;
using DB;
using IO;
using Mai2.Mai2Cue;
using MAI2.Util;
using Manager;
using Manager.MaiStudio;
using Manager.UserDatas;
using Process;
using UnityEngine;

public class FriendBattleResultProcess : ProcessBase
{
	private enum State
	{
		None,
		Init,
		Staging,
		Release
	}

	private enum StagingState
	{
		None,
		Init,
		SkipWait,
		Staging,
		Information,
		InputWait,
		Wait,
		End
	}

	private enum ExtraInfo
	{
		UnlockMusic
	}

	private enum InfoKind
	{
		Common = 0,
		Extra = 1,
		Invalid = -1
	}

	private readonly Action _completeCallback;

	private State _state;

	private InfoKind _infoKind;

	private StagingState[] _stagingStates;

	private Queue<FirstInformationData>[] _messageQueue;

	private Queue<ExtraInfo>[] _extraInfoQueue;

	private FriendBattleResultMonitor[] _monitors;

	private float[] _timer;

	private float[] _messageSkipTime;

	private bool[] _isBossBattle;

	private bool[] _isBossEntry;

	private bool[] _isBossAppearance;

	private bool[] _isMessageSkip;

	public FriendBattleResultProcess(ProcessDataContainer dataContainer)
		: base(dataContainer)
	{
	}

	public FriendBattleResultProcess(ProcessDataContainer dataContainer, Action completeCallback)
		: base(dataContainer)
	{
		_completeCallback = completeCallback;
	}

	public override void OnAddProcess()
	{
	}

	public override void OnStart()
	{
		_stagingStates = new StagingState[2];
		_messageQueue = new Queue<FirstInformationData>[2];
		_extraInfoQueue = new Queue<ExtraInfo>[2];
		_timer = new float[2];
		_messageSkipTime = new float[2];
		_isBossBattle = new bool[2];
		_isBossEntry = new bool[2];
		_isBossAppearance = new bool[2];
		_isMessageSkip = new bool[2];
		GameObject prefs = Resources.Load<GameObject>("Process/FriendBattleResult/FriendBattleResultProcess");
		_monitors = new FriendBattleResultMonitor[2]
		{
			CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<FriendBattleResultMonitor>(),
			CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<FriendBattleResultMonitor>()
		};
		for (int i = 0; i < 2; i++)
		{
			_timer[i] = 0f;
			_messageQueue[i] = new Queue<FirstInformationData>();
			_extraInfoQueue[i] = new Queue<ExtraInfo>();
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(i);
			_monitors[i].Initialize(i, userData.IsEntry);
			_monitors[i].SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button04);
			if (!userData.IsEntry)
			{
				_stagingStates[i] = StagingState.End;
				_monitors[i].SetDisable();
				continue;
			}
			if (GameManager.SelectGhostID[i] == GhostManager.GhostTarget.End || userData.IsGuest())
			{
				_stagingStates[i] = StagingState.End;
				container.processManager.EnqueueMessage(i, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
				_monitors[i].SetDisable();
				continue;
			}
			UserGhost ghostToEnum = Singleton<GhostManager>.Instance.GetGhostToEnum(GameManager.SelectGhostID[i]);
			if (ghostToEnum == null)
			{
				continue;
			}
			PartnerData partner = Singleton<DataManager>.Instance.GetPartner(userData.Detail.EquipPartnerID);
			SoundManager.SetPartnerVoiceCue(i, userData.Detail.EquipPartnerID);
			UserUdemae udemae = userData.RatingList.Udemae;
			MessageUserInformationData rivalData = null;
			GameScoreList gameScore = Singleton<GamePlayManager>.Instance.GetGameScore(i);
			MessageUserInformationData messageUserInformationData = new MessageUserInformationData(i, container.assetManager, userData.Detail, userData.Option.DispRate, isSubMonitor: false);
			uint achivement = Singleton<GamePlayManager>.Instance.GetAchivement(i);
			uint preClassValue = gameScore.PreClassValue;
			UdemaeID rateToUdemaeID = UserUdemae.GetRateToUdemaeID((int)preClassValue);
			int classPointToClassValue = UserUdemae.GetClassPointToClassValue((int)preClassValue);
			int rate = (int)Singleton<GamePlayManager>.Instance.GetGameScore(i).PreRating();
			messageUserInformationData.OverrideRateAndGrade((uint)rate, rateToUdemaeID);
			uint classValue = gameScore.ClassValue;
			UdemaeID rateToUdemaeID2 = UserUdemae.GetRateToUdemaeID((int)classValue);
			int classPointToClassValue2 = UserUdemae.GetClassPointToClassValue((int)classValue);
			bool vsGhostWin = Singleton<GamePlayManager>.Instance.GetVsGhostWin(i);
			switch (ghostToEnum.Type)
			{
			case UserGhost.GhostType.Player:
				rivalData = new MessageUserInformationData(2, container.assetManager, new UserDetail
				{
					UserName = ghostToEnum.Name,
					EquipIconID = ghostToEnum.IconId,
					EquipPlateID = ghostToEnum.PlateId,
					EquipTitleID = ghostToEnum.TitleId,
					Rating = (uint)ghostToEnum.Rate,
					ClassRank = (uint)UserUdemae.GetRateToUdemaeID(ghostToEnum.ClassValue),
					CourseRank = ghostToEnum.CourseRank
				}, OptionDisprateID.AllDisp, isSubMonitor: false);
				break;
			case UserGhost.GhostType.Boss:
				_isBossBattle[i] = (i == 0 && GameManager.SelectGhostID[i] == GhostManager.GhostTarget.BossGhost_1P) || (i == 1 && GameManager.SelectGhostID[i] == GhostManager.GhostTarget.BossGhost_2P);
				rivalData = new MessageUserInformationData(container.assetManager, new UserDetail
				{
					UserName = ghostToEnum.Name,
					EquipIconID = ghostToEnum.IconId,
					EquipPlateID = ghostToEnum.PlateId,
					EquipTitleID = ghostToEnum.TitleId,
					Rating = (uint)ghostToEnum.Rate,
					ClassRank = (uint)UserUdemae.GetRateToUdemaeID(ghostToEnum.ClassValue),
					CourseRank = ghostToEnum.CourseRank
				});
				break;
			case UserGhost.GhostType.MapNpc:
				rivalData = new MessageUserInformationData(container.assetManager, new UserGhost
				{
					Name = ghostToEnum.Name,
					IconId = ghostToEnum.IconId,
					PlateId = ghostToEnum.PlateId,
					TitleId = ghostToEnum.TitleId,
					Rate = ghostToEnum.Rate,
					ClassValue = ghostToEnum.ClassValue,
					ClassRank = (uint)UserUdemae.GetRateToUdemaeID(ghostToEnum.ClassValue),
					CourseRank = ghostToEnum.CourseRank
				});
				break;
			}
			UdemaeBossData udemaeBoss = Singleton<DataManager>.Instance.GetUdemaeBoss(rateToUdemaeID.GetClassBoss());
			int border = rateToUdemaeID.GetBorder();
			UdemaeID maxDan = Singleton<GamePlayManager>.Instance.GetGameScore(i).MaxDan;
			bool isBossBattleEnable = udemae.IsBossBattleEnable();
			bool flag = udemae.IsBossExist(rateToUdemaeID, maxDan);
			bool flag2 = udemae.IsBossBattleEnable(rateToUdemaeID, (int)preClassValue % 10000, maxDan);
			_isBossAppearance[i] = border == classValue % 10000u && border > preClassValue % 10000u && udemaeBoss != null && flag;
			bool flag3 = UserUdemae.IsBossSpecial((int)preClassValue) || UserUdemae.IsBossSpecial((int)classValue);
			bool isBossDeprivation = border == preClassValue % 10000u && border > classValue % 10000u && flag;
			_isBossEntry[i] = flag2 || _isBossBattle[i];
			if (_isBossAppearance[i] && udemaeBoss != null)
			{
				int classValue2 = userData.RatingList.Udemae.ClassValue + 1;
				MessageUserInformationData bossInformation = new MessageUserInformationData(container.assetManager, new UserDetail
				{
					UserName = udemaeBoss.notesDesigner.str,
					EquipIconID = udemaeBoss.silhouette.id,
					EquipPlateID = 11,
					EquipTitleID = 11,
					Rating = (uint)udemaeBoss.rating,
					ClassRank = (uint)UserUdemae.GetRateToUdemaeID(classValue2),
					CourseRank = (uint)udemaeBoss.daniId.id
				});
				_monitors[i].SetBossInformation(bossInformation);
			}
			if (userData.Detail.ContentBit.IsFlagOn(ContentBitID.FirstFriendBattleResult))
			{
				_messageQueue[i].Enqueue(new FirstInformationData(WindowMessageID.FBRFirst, 1000));
				userData.Detail.ContentBit.SetFlag(ContentBitID.FirstFriendBattleResult, flag: true);
			}
			if (!userData.Detail.ContentBit.IsFlagOn(ContentBitID.FirstBossAppear) && _isBossAppearance[i])
			{
				_messageQueue[i].Enqueue(new FirstInformationData(WindowMessageID.BossAppearFirst, 1000));
				userData.Detail.ContentBit.SetFlag(ContentBitID.FirstBossAppear, flag: true);
			}
			if (flag3 && _isBossAppearance[i])
			{
				_messageQueue[i].Enqueue(new FirstInformationData(WindowMessageID.SpecialBossAppear, 1000));
				userData.Detail.ContentBit.SetFlag(ContentBitID.FirstBossAppear, flag: true);
			}
			if ((_isBossBattle[i] && !vsGhostWin) || (!_isBossBattle[i] && flag2 && vsGhostWin))
			{
				_messageQueue[i].Enqueue(new FirstInformationData(WindowMessageID.BossStayHint, 1000));
			}
			if (!userData.Detail.ContentBit.IsFlagOn(ContentBitID.FirstA4Class) && rateToUdemaeID <= UdemaeID.Class_A5 && UdemaeID.Class_A4 <= rateToUdemaeID2)
			{
				_messageQueue[i].Enqueue(new FirstInformationData(WindowMessageID.FBRClassPoint, 1000));
				userData.Detail.ContentBit.SetFlag(ContentBitID.FirstA4Class, flag: true);
			}
			if (rateToUdemaeID < UdemaeID.Class_LEGEND && UdemaeID.Class_LEGEND <= rateToUdemaeID2)
			{
				_messageQueue[i].Enqueue(new FirstInformationData(WindowMessageID.FBRLegend, 1000));
			}
			if (flag3 && udemaeBoss != null)
			{
				MusicData music = Singleton<DataManager>.Instance.GetMusic(udemaeBoss.music.id);
				Texture2D jacketTexture2D = container.assetManager.GetJacketTexture2D(music.jacketFile);
				Sprite jacket = Sprite.Create(jacketTexture2D, new Rect(0f, 0f, jacketTexture2D.width, jacketTexture2D.height), new Vector2(0.5f, 0.5f));
				string str = music.name.str;
				_monitors[i].SetUnlockInfo(jacket, str);
			}
			if (flag3 && _isBossBattle[i] && vsGhostWin && UserUdemae.IsBossSpecial((int)preClassValue))
			{
				int id = udemaeBoss.music.id;
				if (!userData.IsUnlockMusic(UserData.MusicUnlock.Base, id))
				{
					userData.AddUnlockMusic(UserData.MusicUnlock.Base, id);
					userData.Activity.TransmissionMusicGet(id);
				}
				_extraInfoQueue[i].Enqueue(ExtraInfo.UnlockMusic);
			}
			_monitors[i].SetFlagData(vsGhostWin, _isBossBattle[i], _isBossEntry[i], _isBossAppearance[i], isBossDeprivation, flag3, isBossBattleEnable, flag);
			_monitors[i].SetUserData(messageUserInformationData, achivement, partner.naviChara.id, rivalData, (uint)ghostToEnum.Achievement);
			_monitors[i].SetClassData(classPointToClassValue, rateToUdemaeID, classPointToClassValue2, rateToUdemaeID2);
			_stagingStates[i] = StagingState.None;
		}
		_state = State.Init;
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		LogicUpdate();
		if (_state == State.Staging && _stagingStates[0] == StagingState.End && _stagingStates[1] == StagingState.End)
		{
			Cleanup();
		}
		for (int i = 0; i < _monitors.Length; i++)
		{
			_monitors[i].ViewUpdate();
		}
	}

	public override void OnLateUpdate()
	{
	}

	protected override void UpdateInput(int playerIndex)
	{
		base.UpdateInput(playerIndex);
		_ = _state;
		_ = 1;
		if (_state != State.Staging)
		{
			return;
		}
		switch (_stagingStates[playerIndex])
		{
		case StagingState.Information:
			if (!IsInputLock(playerIndex) && _isMessageSkip[playerIndex] && _timer[playerIndex] >= 1000f && InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button04))
			{
				_monitors[playerIndex].PressedButton(InputManager.ButtonSetting.Button04);
				SoundManager.PlaySE(Cue.SE_SYS_FIX, playerIndex);
				_timer[playerIndex] = 10000f;
				SetInputLockInfo(playerIndex, 500f);
			}
			break;
		case StagingState.InputWait:
			if (InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button04))
			{
				_monitors[playerIndex].PressedButton(InputManager.ButtonSetting.Button04);
				SoundManager.PlaySE(Cue.SE_SYS_FIX, playerIndex);
				_monitors[playerIndex].SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button04);
				_stagingStates[playerIndex] = StagingState.End;
				int num = ((playerIndex == 0) ? 1 : 0);
				if (Singleton<UserDataManager>.Instance.GetUserData(num).IsEntry)
				{
					container.processManager.EnqueueMessage(playerIndex, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
				}
			}
			break;
		case StagingState.Staging:
		case StagingState.Wait:
		case StagingState.End:
			break;
		}
	}

	private void LogicUpdate()
	{
		switch (_state)
		{
		case State.Staging:
		{
			for (int i = 0; i < 2; i++)
			{
				switch (_stagingStates[i])
				{
				case StagingState.Init:
					_stagingStates[i] = StagingState.SkipWait;
					_monitors[i].SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button04);
					break;
				case StagingState.SkipWait:
					if (_timer[i] < 100f)
					{
						_timer[i] += GameManager.GetGameMSecAdd();
						break;
					}
					if (!_isBossEntry[i] && _monitors[i].IsSkip)
					{
						_monitors[i].SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button04);
					}
					_timer[i] = 0f;
					_stagingStates[i] = StagingState.Staging;
					break;
				case StagingState.Staging:
					if (_monitors[i].IsEnd())
					{
						if (_messageQueue[i].Count > 0 || _extraInfoQueue[i].Count > 0)
						{
							_timer[i] = 0f;
							ShowMessage(i);
							_monitors[i].SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button04);
							_stagingStates[i] = StagingState.Information;
						}
						else
						{
							_stagingStates[i] = StagingState.Wait;
						}
					}
					break;
				case StagingState.Information:
					UpdateInformation(i);
					break;
				case StagingState.Wait:
				{
					int num = ((i == 0) ? 1 : 0);
					if (_stagingStates[num] == StagingState.Wait || _stagingStates[num] == StagingState.End || _stagingStates[num] == StagingState.InputWait)
					{
						_monitors[i].ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 1);
						_stagingStates[i] = StagingState.InputWait;
						_monitors[i].SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button04);
						if (_stagingStates[num] == StagingState.InputWait || _stagingStates[num] == StagingState.End)
						{
							container.processManager.PrepareTimer(10, 0, isEntry: false, Cleanup, isVisible: false);
						}
					}
					break;
				}
				}
			}
			break;
		}
		case State.Init:
		case State.Release:
			break;
		}
	}

	private void UpdateInformation(int playerIndex)
	{
		if (!_isMessageSkip[playerIndex] && _timer[playerIndex] > _messageSkipTime[playerIndex])
		{
			_isMessageSkip[playerIndex] = true;
			_monitors[playerIndex].ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 0);
			_monitors[playerIndex].SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button04);
		}
		if (_timer[playerIndex] >= 10000f)
		{
			switch (_infoKind)
			{
			case InfoKind.Common:
				container.processManager.CloseWindow(playerIndex);
				break;
			case InfoKind.Extra:
				_monitors[playerIndex].SkipUnlockInfo();
				break;
			}
			_timer[playerIndex] = 0f;
			if (_messageQueue[playerIndex].Count > 0 || _extraInfoQueue[playerIndex].Count > 0)
			{
				_monitors[playerIndex].SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button04);
				_isMessageSkip[playerIndex] = false;
				ShowMessage(playerIndex);
			}
			else
			{
				_stagingStates[playerIndex] = StagingState.Wait;
			}
		}
		else
		{
			_timer[playerIndex] += GameManager.GetGameMSecAdd();
		}
	}

	public void PlayStart()
	{
		for (int i = 0; i < 2; i++)
		{
			if (_stagingStates[i] == StagingState.None)
			{
				_stagingStates[i] = StagingState.Init;
				_monitors[i].Play();
			}
		}
		_state = State.Staging;
	}

	private void ShowMessage(int playerIndex)
	{
		if (_messageQueue[playerIndex].Count > 0)
		{
			FirstInformationData firstInformationData = _messageQueue[playerIndex].Dequeue();
			container.processManager.EnqueueMessage(playerIndex, firstInformationData.MessageID);
			_messageSkipTime[playerIndex] = firstInformationData.SkipButtonTime;
			_infoKind = InfoKind.Common;
		}
		else if (_extraInfoQueue[playerIndex].Count > 0)
		{
			if (_extraInfoQueue[playerIndex].Dequeue() == ExtraInfo.UnlockMusic)
			{
				_monitors[playerIndex].PlayUnlockInfo();
			}
			_infoKind = InfoKind.Extra;
		}
	}

	public void Cleanup()
	{
		for (int i = 0; i < _monitors.Length; i++)
		{
			if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry)
			{
				MechaManager.LedIf[i].ButtonLedReset();
			}
			_stagingStates[i] = StagingState.End;
			_monitors[i].Cleanup();
			container.processManager.ForcedCloseWindow(i);
		}
		_state = State.Release;
		_completeCallback?.Invoke();
	}

	public override void OnRelease()
	{
		FriendBattleResultMonitor[] monitors = _monitors;
		for (int i = 0; i < monitors.Length; i++)
		{
			UnityEngine.Object.Destroy(monitors[i].gameObject);
		}
		_monitors = null;
	}
}
