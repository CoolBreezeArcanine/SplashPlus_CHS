using System;
using AMDaemon;
using DB;
using IO;
using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_Partner_000001;
using Manager;
using Mecha;
using Monitor;
using Net;
using Net.Packet.Helper;
using Net.Packet.Mai2;
using Net.VO.Mai2;
using UnityEngine;

namespace Process
{
	public class ContinueProcess : ProcessBase
	{
		public enum ContinueSequence
		{
			Init,
			Wait,
			Disp,
			DispEnd,
			Continue,
			DownloadWait,
			Error,
			Release
		}

		private ContinueSequence _state;

		private ContinueMonitor[] _monitors;

		private bool[] _isContinueReady;

		private float _timeCounter;

		public ContinueProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public override void OnAddProcess()
		{
		}

		public override void OnStart()
		{
			GameObject prefs = Resources.Load<GameObject>("Process/Continue/ContinueProcess");
			_monitors = new ContinueMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<ContinueMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<ContinueMonitor>()
			};
			_isContinueReady = new bool[_monitors.Length];
			for (int i = 0; i < _monitors.Length; i++)
			{
				_monitors[i].Initialize(i, Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry);
				SoundManager.StopBGM(i);
				GameManager.IsSelectContinue[i] = false;
				_isContinueReady[i] = false;
			}
			container.processManager.NotificationFadeIn();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			switch (_state)
			{
			case ContinueSequence.Init:
				_state = ContinueSequence.Wait;
				break;
			case ContinueSequence.Wait:
			{
				_timeCounter += Time.deltaTime;
				if (!(_timeCounter >= 1f))
				{
					break;
				}
				_timeCounter = 0f;
				_state = ContinueSequence.Disp;
				for (int i = 0; i < _monitors.Length; i++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry)
					{
						_monitors[i].WaitStart();
						SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000164, i);
					}
				}
				Action both = delegate
				{
					for (int n = 0; n < _monitors.Length; n++)
					{
						if (Singleton<UserDataManager>.Instance.GetUserData(n).IsEntry && !_monitors[n].IsSelected())
						{
							_monitors[n].SelectNotContinue();
							GameManager.IsSelectContinue[n] = false;
							container.processManager.SetVisibleTimers(isVisible: false);
						}
					}
				};
				container.processManager.PrepareTimer(5, 0, isEntry: false, both);
				break;
			}
			case ContinueSequence.Disp:
			{
				if ((!Singleton<UserDataManager>.Instance.GetUserData(0L).IsEntry || !_monitors[0].IsSelected()) && (!Singleton<UserDataManager>.Instance.GetUserData(1L).IsEntry || !_monitors[1].IsSelected()))
				{
					break;
				}
				_state = ContinueSequence.DispEnd;
				_timeCounter = 0f;
				if (GameManager.IsSelectContinue[0] || GameManager.IsSelectContinue[1])
				{
					container.processManager.SetVisibleTimers(isVisible: false);
					for (int k = 0; k < _monitors.Length; k++)
					{
						if (Singleton<UserDataManager>.Instance.GetUserData(k).IsEntry)
						{
							SoundManager.PlaySE(Mai2.Mai2Cue.Cue.JINGLE_CONTINUE, k);
							_monitors[k].PlayContinue();
						}
					}
					break;
				}
				for (int l = 0; l < _monitors.Length; l++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(l).IsEntry)
					{
						_monitors[l].PlayNotContinue();
					}
				}
				break;
			}
			case ContinueSequence.DispEnd:
				_timeCounter += Time.deltaTime;
				if (!(_timeCounter >= 3f))
				{
					break;
				}
				_timeCounter = 0f;
				if (GameManager.IsSelectContinue[0] || GameManager.IsSelectContinue[1])
				{
					_state = ContinueSequence.Continue;
					BackupBookkeep.EntryState[] array = new BackupBookkeep.EntryState[_monitors.Length];
					bool[] array2 = new bool[_monitors.Length];
					for (int j = 0; j < _monitors.Length; j++)
					{
						UserData userData = Singleton<UserDataManager>.Instance.GetUserData(j);
						array[j].entry = false;
						array2[j] = false;
						if (Singleton<UserDataManager>.Instance.GetUserData(j).IsEntry)
						{
							array[j].entry = true;
							array[j].type = (UserID.IsGuest(Singleton<UserDataManager>.Instance.GetUserData(j).Detail.UserID) ? BackupBookkeep.LoginType.Guest : BackupBookkeep.LoginType.Aime);
							array2[j] = Singleton<UserDataManager>.Instance.GetUserData(j).UserType == UserData.UserIDType.New;
							if (!userData.IsGuest())
							{
								SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.SendAimeLog(userData.AimeId, AimeLogStatus.Leave);
							}
						}
					}
					SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.bookkeep.endPlayTime(array, array2);
					ResetEntry();
					container.processManager.SendMessage(new Message(ProcessType.PleaseWaitProcess, 20000));
				}
				else
				{
					_state = ContinueSequence.Release;
					NextGameOverProcess();
					bool flag = Singleton<OperationManager>.Instance.IsAimeLoginDisable();
					SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.EnableRead(!flag);
				}
				container.processManager.SetVisibleTimers(isVisible: false);
				break;
			case ContinueSequence.Continue:
				if (_isContinueReady[0] && _isContinueReady[1])
				{
					container.processManager.AddProcess(new UserDataDLProcess(container), 50);
					_state = ContinueSequence.DownloadWait;
				}
				break;
			case ContinueSequence.DownloadWait:
				if ((bool)container.processManager.SendMessage(new Message(ProcessType.NetworkProcess, 300)))
				{
					_state = ContinueSequence.Error;
					container.processManager.EnqueueMessage(0, WindowMessageID.NetworkError);
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_ENTRY_AIME_ERROR, 0);
					container.processManager.EnqueueMessage(1, WindowMessageID.NetworkError);
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_ENTRY_AIME_ERROR, 1);
				}
				if ((bool)container.processManager.SendMessage(new Message(ProcessType.NetworkProcess, 200)))
				{
					NextContinueProcess();
					_state = ContinueSequence.Release;
				}
				break;
			case ContinueSequence.Error:
				_timeCounter += Time.deltaTime;
				if (3f < _timeCounter)
				{
					_timeCounter = 0f;
					container.processManager.ForcedCloseWindow(0);
					container.processManager.ForcedCloseWindow(1);
					NextGameOverProcess();
					_state = ContinueSequence.Release;
				}
				break;
			case ContinueSequence.Release:
				return;
			}
			ContinueMonitor[] monitors = _monitors;
			for (int m = 0; m < monitors.Length; m++)
			{
				monitors[m].ViewUpdate();
			}
		}

		protected override void UpdateInput(int monitorId)
		{
			switch (_state)
			{
			case ContinueSequence.Disp:
				DetailInputOnDisp(monitorId);
				break;
			case ContinueSequence.Init:
			case ContinueSequence.Wait:
			case ContinueSequence.DispEnd:
			case ContinueSequence.Continue:
			case ContinueSequence.DownloadWait:
			case ContinueSequence.Error:
			case ContinueSequence.Release:
				break;
			}
		}

		private void DetailInputOnDisp(int monitorId)
		{
			if (_monitors[monitorId].IsSelected() || !Singleton<UserDataManager>.Instance.GetUserData(monitorId).IsEntry)
			{
				return;
			}
			if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button04))
			{
				for (int i = 0; i < _monitors.Length; i++)
				{
					_monitors[i].SelectContinue();
					GameManager.IsSelectContinue[i] = true;
				}
				container.processManager.ForceTimeUp();
			}
			else if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button05))
			{
				for (int j = 0; j < _monitors.Length; j++)
				{
					_monitors[j].SelectNotContinue();
					GameManager.IsSelectContinue[j] = false;
				}
				container.processManager.ForceTimeUp();
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
		}

		private void ResetEntry()
		{
			SoundManager.ResetMasterVolume();
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.ClearPaydCost();
			container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30000));
			container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 50000));
			container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 50002, CommonMonitor.SkyDaylight.MorningNow));
			WebCamManager.ClearnUpFolder();
			SoundManager.PlayBGM(Mai2.Mai2Cue.Cue.BGM_ENTRY, 2);
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.SetCoinCostInit();
			bool[] array = new bool[2]
			{
				GameManager.IsSelectContinue[0],
				GameManager.IsSelectContinue[1]
			};
			GameManager.Initialize();
			GameManager.IsSelectContinue[0] = array[0];
			GameManager.IsSelectContinue[1] = array[1];
			Singleton<NetDataManager>.Instance.Initialize();
			Singleton<GhostManager>.Instance.ResetGhostServerData();
			Singleton<CourseManager>.Instance.Initialize();
			Singleton<TicketManager>.Instance.Initialize();
			GameManager.IsCourseMode = false;
			GameManager.SpecialKindNum = GameManager.SpecialKind.None;
			Bd15070_4IF[] ledIf = MechaManager.LedIf;
			foreach (Bd15070_4IF obj in ledIf)
			{
				obj.ButtonLedReset();
				obj.SetColorMultiFet(Bd15070_4IF.BodyBrightOutGame);
			}
			Singleton<OperationManager>.Instance.AutomaticDownload = false;
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.EnableRead(flag: false);
			TimeManager.MarkGameStartTime();
			Singleton<EventManager>.Instance.UpdateEvent();
			Singleton<ScoreRankingManager>.Instance.UpdateData();
			container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30001));
			ContinueMonitor[] monitors = _monitors;
			foreach (ContinueMonitor continueMonitor in monitors)
			{
				if (Singleton<UserDataManager>.Instance.GetUserData(continueMonitor.MonitorIndex).IsEntry)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(continueMonitor.MonitorIndex).IsGuest())
					{
						Singleton<UserDataManager>.Instance.GetUserData(continueMonitor.MonitorIndex).Initialize();
						Singleton<UserDataManager>.Instance.SetDefault(continueMonitor.MonitorIndex);
						Singleton<UserDataManager>.Instance.GetUserData(continueMonitor.MonitorIndex).IsEntry = true;
						_isContinueReady[continueMonitor.MonitorIndex] = true;
					}
					else
					{
						ReLogin(continueMonitor.MonitorIndex);
					}
				}
				else
				{
					_isContinueReady[continueMonitor.MonitorIndex] = true;
				}
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 50003, continueMonitor.MonitorIndex, 1));
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 45002));
			}
		}

		private void ReLogin(int id)
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(id);
			ulong userID = userData.Detail.UserID;
			string userName = userData.Detail.UserName;
			string accessCode = userData.Detail.AccessCode;
			string offlineID = userData.OfflineId;
			string dailyBonusDate = userData.Detail.DailyBonusDate;
			int netMember = userData.IsNetMember;
			UserData.UserIDType userType = userData.UserType;
			OptionHeadphonevolumeID headPhoneVol = userData.Option.HeadPhoneVolume;
			if (userType == UserData.UserIDType.New || userType == UserData.UserIDType.Inherit)
			{
				userType = UserData.UserIDType.Exist;
			}
			PacketHelper.StartPacket(new PacketUserLogin(userID, userData.Detail.AccessCode, delegate(UserLoginResponseVO vo)
			{
				if (vo.returnCode == 1)
				{
					UserData userData2 = Singleton<UserDataManager>.Instance.GetUserData(id);
					userData2.Initialize();
					userData2.IsEntry = true;
					if (!UserID.IsGuest(userID))
					{
						userData2.Detail.UserID = userID;
						userData2.Detail.UserName = userName;
						userData2.Detail.AccessCode = accessCode;
						userData2.Detail.DailyBonusDate = dailyBonusDate;
						userData2.OfflineId = offlineID;
						userData2.IsNetMember = netMember;
						userData2.UserType = userType;
						userData2.Option.HeadPhoneVolume = headPhoneVol;
					}
					Singleton<NetDataManager>.Instance.SetLoginVO(id, vo);
				}
				else
				{
					container.processManager.EnqueueMessage(id, WindowMessageID.EntryErrorAimeLogin);
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_ENTRY_AIME_ERROR, id);
					_state = ContinueSequence.Error;
				}
				_isContinueReady[id] = true;
			}, delegate
			{
				_state = ContinueSequence.Error;
				container.processManager.EnqueueMessage(id, WindowMessageID.NetworkError);
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_ENTRY_AIME_ERROR, id);
				_isContinueReady[id] = true;
			}));
		}

		private void NextGameOverProcess()
		{
			container.processManager.AddProcess(new FadeProcess(container, this, new GameOverProcess(container), FadeProcess.FadeType.Type3), 50);
		}

		private void NextContinueProcess()
		{
			container.processManager.AddProcess(new FadeProcess(container, this, new PlInformationProcess(isContinue: true, container)), 50);
			container.processManager.AddProcess(new PleaseWaitProcess(container), 50);
		}
	}
}
