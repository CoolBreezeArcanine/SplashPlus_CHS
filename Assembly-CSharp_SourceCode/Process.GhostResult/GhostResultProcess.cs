using System;
using System.Collections.Generic;
using DB;
using IO;
using Mai2.Mai2Cue;
using MAI2.Util;
using Manager;
using Manager.MaiStudio;
using Manager.UserDatas;
using Monitor.GhostResult;
using UnityEngine;

namespace Process.GhostResult
{
	public class GhostResultProcess : ProcessBase
	{
		private enum GhostResultState
		{
			Init,
			Staging,
			Release
		}

		private enum StagingState
		{
			Init,
			Wait,
			SkipWait,
			Staging,
			Information,
			InputWait,
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

		private GhostResultState _state;

		private StagingState[] _states;

		private GhostResultMonitor[] _monitors;

		private float[] _timer;

		private float[] _messageSkipTime;

		private bool[] _isBossBattle;

		private bool[] _isBossEntry;

		private bool[] _isBossAppearance;

		private bool[] _isMessageSkip;

		private Queue<FirstInformationData>[] _messageQueue;

		private Queue<ExtraInfo>[] _extraInfoQueue;

		private InfoKind _infoKind;

		public GhostResultProcess(ProcessDataContainer dataContainer, Action completeCallback)
			: base(dataContainer, ProcessType.CommonProcess)
		{
			_completeCallback = completeCallback;
		}

		public override void OnAddProcess()
		{
		}

		public override void OnStart()
		{
			_state = GhostResultState.Init;
			_infoKind = InfoKind.Invalid;
			GameObject prefs = Resources.Load<GameObject>("Process/GhostResult/GhostResultProcess");
			_monitors = new GhostResultMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<GhostResultMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<GhostResultMonitor>()
			};
			_states = new StagingState[2];
			_timer = new float[2];
			_messageSkipTime = new float[2];
			_isBossBattle = new bool[2];
			_isBossEntry = new bool[2];
			_isBossAppearance = new bool[2];
			_isMessageSkip = new bool[2];
			_messageQueue = new Queue<FirstInformationData>[2];
			_extraInfoQueue = new Queue<ExtraInfo>[2];
			for (int i = 0; i < 2; i++)
			{
				_timer[i] = 0f;
				_isBossEntry[i] = false;
				_isBossAppearance[i] = false;
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(i);
				_monitors[i].Initialize(i, userData.IsEntry);
				if (!userData.IsEntry)
				{
					_states[i] = StagingState.End;
					continue;
				}
				if (GameManager.SelectGhostID[i] == GhostManager.GhostTarget.End)
				{
					_states[i] = StagingState.End;
					container.processManager.EnqueueMessage(i, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
					_monitors[i].SetWait();
					continue;
				}
				MessageUserInformationData messageUserInformationData = new MessageUserInformationData(i, container.assetManager, userData.Detail, userData.Option.DispRate, isSubMonitor: false);
				_messageQueue[i] = new Queue<FirstInformationData>();
				_extraInfoQueue[i] = new Queue<ExtraInfo>();
				UserGhost ghostToEnum = Singleton<GhostManager>.Instance.GetGhostToEnum(GameManager.SelectGhostID[i]);
				if (ghostToEnum == null)
				{
					break;
				}
				uint achivement = Singleton<GamePlayManager>.Instance.GetAchivement(i);
				UdemaeID rateToUdemaeID = UserUdemae.GetRateToUdemaeID(ghostToEnum.ClassValue);
				int rate = (int)Singleton<GamePlayManager>.Instance.GetGameScore(i).PreRating();
				int danRate = (int)Singleton<GamePlayManager>.Instance.GetGameScore(i).DanRate;
				int preDanRate = (int)Singleton<GamePlayManager>.Instance.GetGameScore(i).PreDanRate;
				int preClassValue = (int)Singleton<GamePlayManager>.Instance.GetGameScore(i).PreClassValue;
				int classValue = (int)Singleton<GamePlayManager>.Instance.GetGameScore(i).ClassValue;
				UdemaeID rateToUdemaeID2 = UserUdemae.GetRateToUdemaeID(preClassValue);
				UdemaeID rateToUdemaeID3 = UserUdemae.GetRateToUdemaeID(classValue);
				UdemaeID maxDan = Singleton<GamePlayManager>.Instance.GetGameScore(i).MaxDan;
				int classPointToClassValue = UserUdemae.GetClassPointToClassValue(preClassValue);
				int classPointToClassValue2 = UserUdemae.GetClassPointToClassValue(classValue);
				messageUserInformationData.OverrideRateAndGrade((uint)rate, rateToUdemaeID2);
				if (ghostToEnum.Type == UserGhost.GhostType.Player)
				{
					MessageUserInformationData rivalData = new MessageUserInformationData(2, container.assetManager, new UserDetail
					{
						UserName = ghostToEnum.Name,
						EquipIconID = ghostToEnum.IconId,
						EquipPlateID = ghostToEnum.PlateId,
						EquipTitleID = ghostToEnum.TitleId,
						Rating = (uint)ghostToEnum.Rate,
						ClassRank = (uint)UserUdemae.GetRateToUdemaeID(ghostToEnum.ClassValue),
						CourseRank = ghostToEnum.CourseRank
					}, OptionDisprateID.AllDisp, isSubMonitor: false);
					_monitors[i].SetUserData(messageUserInformationData, rivalData, achivement, (uint)ghostToEnum.Achievement, rateToUdemaeID2, rateToUdemaeID);
				}
				else if (ghostToEnum.Type == UserGhost.GhostType.MapNpc)
				{
					MessageUserInformationData rivalData2 = new MessageUserInformationData(container.assetManager, new UserGhost
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
					_monitors[i].SetUserData(messageUserInformationData, rivalData2, achivement, (uint)ghostToEnum.Achievement, rateToUdemaeID2, rateToUdemaeID);
				}
				else if (ghostToEnum != null && ghostToEnum.Type == UserGhost.GhostType.Boss)
				{
					_isBossBattle[i] = (i == 0 && GameManager.SelectGhostID[i] == GhostManager.GhostTarget.BossGhost_1P) || (i == 1 && GameManager.SelectGhostID[i] == GhostManager.GhostTarget.BossGhost_2P);
					MessageUserInformationData rivalData3 = new MessageUserInformationData(container.assetManager, new UserDetail
					{
						UserName = ghostToEnum.Name,
						EquipIconID = ghostToEnum.IconId,
						EquipPlateID = ghostToEnum.PlateId,
						EquipTitleID = ghostToEnum.TitleId,
						Rating = (uint)ghostToEnum.Rate,
						ClassRank = (uint)UserUdemae.GetRateToUdemaeID(ghostToEnum.ClassValue),
						CourseRank = ghostToEnum.CourseRank
					});
					_monitors[i].SetUserData(messageUserInformationData, rivalData3, achivement, (uint)ghostToEnum.Achievement, rateToUdemaeID2, rateToUdemaeID);
				}
				bool vsGhostWin = Singleton<GamePlayManager>.Instance.GetVsGhostWin(i);
				UserUdemae udemae = userData.RatingList.Udemae;
				UdemaeBossData udemaeBoss = Singleton<DataManager>.Instance.GetUdemaeBoss(rateToUdemaeID2.GetClassBoss());
				_isBossAppearance[i] = udemae.IsBossBattleEnable(rateToUdemaeID3, classPointToClassValue2, maxDan);
				bool flag = udemae.IsBossBattleEnable(rateToUdemaeID2, classPointToClassValue, maxDan);
				bool isBossDeprivation = !vsGhostWin && flag && !_isBossAppearance[i];
				_isBossEntry[i] = flag || _isBossBattle[i];
				bool flag2 = _isBossAppearance[i] && !flag;
				bool flag3 = UserUdemae.IsBossSpecial(preClassValue) || UserUdemae.IsBossSpecial(classValue);
				_monitors[i].SetData(vsGhostWin, preClassValue, rateToUdemaeID2, classValue, rateToUdemaeID3, danRate - preDanRate, _isBossBattle[i], _isBossEntry[i], flag2, isBossDeprivation, flag3);
				int classValue2 = userData.RatingList.Udemae.ClassValue + 1;
				if (flag2 && udemaeBoss != null)
				{
					MessageUserInformationData bossInfo = new MessageUserInformationData(container.assetManager, new UserDetail
					{
						UserName = udemaeBoss.notesDesigner.str,
						EquipIconID = udemaeBoss.silhouette.id,
						EquipPlateID = 11,
						EquipTitleID = 11,
						Rating = (uint)udemaeBoss.rating,
						ClassRank = (uint)UserUdemae.GetRateToUdemaeID(classValue2)
					});
					_monitors[i].SetBossInfo(bossInfo);
				}
				if (flag3 && udemaeBoss != null)
				{
					MusicData music = Singleton<DataManager>.Instance.GetMusic(udemaeBoss.music.id);
					Texture2D jacketTexture2D = container.assetManager.GetJacketTexture2D(music.jacketFile);
					Sprite jacket = Sprite.Create(jacketTexture2D, new Rect(0f, 0f, jacketTexture2D.width, jacketTexture2D.height), new Vector2(0.5f, 0.5f));
					string str = music.name.str;
					_monitors[i].SetUnlockInfo(jacket, str);
				}
				_states[i] = StagingState.Init;
				_monitors[i].InitializeTimeline();
				_state = GhostResultState.Staging;
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
				if ((_isBossBattle[i] && !vsGhostWin) || (!_isBossBattle[i] && flag && _isBossAppearance[i]))
				{
					_messageQueue[i].Enqueue(new FirstInformationData(WindowMessageID.BossStayHint, 1000));
				}
				if (flag3 && _isBossBattle[i] && vsGhostWin)
				{
					int id = udemaeBoss.music.id;
					if (!userData.IsUnlockMusic(UserData.MusicUnlock.Base, id))
					{
						userData.AddUnlockMusic(UserData.MusicUnlock.Base, id);
						userData.Activity.TransmissionMusicGet(id);
					}
					_extraInfoQueue[i].Enqueue(ExtraInfo.UnlockMusic);
				}
			}
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			for (int i = 0; i < 2; i++)
			{
				LogicUpdate(i);
			}
			if (_state == GhostResultState.Staging && _states[0] == StagingState.End && _states[1] == StagingState.End)
			{
				Cleanup();
			}
			GhostResultMonitor[] monitors = _monitors;
			for (int j = 0; j < monitors.Length; j++)
			{
				monitors[j].ViewUpdate();
			}
		}

		public override void OnLateUpdate()
		{
		}

		public void ResultStart()
		{
			for (int i = 0; i < 2; i++)
			{
				if (_states[i] == StagingState.Init)
				{
					if (_monitors[i].IsActive())
					{
						MechaManager.LedIf[i].ButtonLedReset();
					}
					_states[i] = StagingState.SkipWait;
					SetInputLockInfo(i, 1000f);
					_monitors[i].Play();
				}
			}
		}

		private void LogicUpdate(int playerIndex)
		{
			switch (_states[playerIndex])
			{
			case StagingState.SkipWait:
				if (_timer[playerIndex] < 100f)
				{
					_timer[playerIndex] += GameManager.GetGameMSecAdd();
					break;
				}
				if (!_isBossEntry[playerIndex])
				{
					_monitors[playerIndex].SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button04);
				}
				_timer[playerIndex] = 0f;
				_states[playerIndex] = StagingState.Staging;
				break;
			case StagingState.Staging:
				if (_timer[playerIndex] <= 0f)
				{
					if (_monitors[playerIndex].IsSkip && !_isBossEntry[playerIndex] && InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button04))
					{
						_monitors[playerIndex].Skip();
						_monitors[playerIndex].PreseedButton(InputManager.ButtonSetting.Button04);
						SoundManager.PlaySE(Cue.SE_SYS_SKIP, playerIndex);
						_timer[playerIndex] = 100f;
					}
				}
				else
				{
					_timer[playerIndex] -= GameManager.GetGameMSecAdd();
				}
				if (_monitors[playerIndex].IsEnd())
				{
					if (_messageQueue[playerIndex].Count > 0 || _extraInfoQueue[playerIndex].Count > 0)
					{
						_timer[playerIndex] = 0f;
						ShowMessage(playerIndex);
						_monitors[playerIndex].SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button04);
						_states[playerIndex] = StagingState.Information;
					}
					else
					{
						_states[playerIndex] = StagingState.Wait;
					}
				}
				break;
			case StagingState.Information:
				if (!IsInputLock(playerIndex) && _isMessageSkip[playerIndex] && _timer[playerIndex] >= 1000f && InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button04))
				{
					_monitors[playerIndex].PressedButton(InputManager.ButtonSetting.Button04);
					SoundManager.PlaySE(Cue.SE_SYS_FIX, playerIndex);
					_timer[playerIndex] = 10000f;
					SetInputLockInfo(playerIndex, 2000f);
				}
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
						_states[playerIndex] = StagingState.Wait;
					}
				}
				else
				{
					_timer[playerIndex] += GameManager.GetGameMSecAdd();
				}
				break;
			case StagingState.Wait:
			{
				int num2 = ((playerIndex == 0) ? 1 : 0);
				_monitors[playerIndex].ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 1);
				_states[playerIndex] = StagingState.InputWait;
				_monitors[playerIndex].SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button04);
				if (_states[num2] == StagingState.InputWait || _states[num2] == StagingState.End)
				{
					container.processManager.PrepareTimer(10, 0, isEntry: false, Cleanup, isVisible: false);
				}
				break;
			}
			case StagingState.InputWait:
				if (InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button04))
				{
					_monitors[playerIndex].PreseedButton(InputManager.ButtonSetting.Button04);
					SoundManager.PlaySE(Cue.SE_SYS_FIX, playerIndex);
					_monitors[playerIndex].SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button04);
					_states[playerIndex] = StagingState.End;
					_monitors[playerIndex].SetVisibleFrontBlur(isVisible: true);
					int num = ((playerIndex == 0) ? 1 : 0);
					if (Singleton<UserDataManager>.Instance.GetUserData(num).IsEntry)
					{
						container.processManager.EnqueueMessage(playerIndex, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
					}
				}
				break;
			case StagingState.Init:
			case StagingState.End:
				break;
			}
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

		private bool Check(int playerIndex)
		{
			int num = ((playerIndex == 0) ? 1 : 0);
			if (_states[num] != StagingState.InputWait)
			{
				return _states[num] == StagingState.End;
			}
			return true;
		}

		private void Cleanup()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				MechaManager.LedIf[i].ButtonLedReset();
				_states[i] = StagingState.End;
				_monitors[i].CleanUp();
				container.processManager.ForcedCloseWindow(i);
				_monitors[i].SkipUnlockInfo();
			}
			_state = GhostResultState.Release;
			_completeCallback();
		}

		public override void OnRelease()
		{
			GhostResultMonitor[] monitors = _monitors;
			for (int i = 0; i < monitors.Length; i++)
			{
				UnityEngine.Object.Destroy(monitors[i].gameObject);
			}
			_monitors = null;
		}
	}
}
