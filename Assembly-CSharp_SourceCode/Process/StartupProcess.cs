using System.Diagnostics;
using System.Text;
using AMDaemon;
using DB;
using IO;
using MAI2.Util;
using MAI2System;
using Manager;
using Manager.Achieve;
using Manager.Party.Party;
using Monitor;
using PartyLink;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

namespace Process
{
	public class StartupProcess : ProcessBase
	{
		private enum StartUpState : byte
		{
			WaitTouchPanel,
			WaitLed,
			WaitCam,
			WaitDataManager,
			WaitLinkDelivery,
			WaitLinkSetting,
			WaitLinkParty,
			WaitLinkAdvertise,
			Ready,
			Released,
			End
		}

		private enum CheckList
		{
			TouchPanel1P,
			TouchPanel2P,
			Led1P,
			Led2P,
			QrCam1P,
			QrCam2P,
			WebCam,
			DataManager,
			LinkSpace,
			LinkInfo,
			LinkServer,
			LinkGroup,
			End
		}

		private StartupMonitor[] _monitors;

		private StartUpState _state;

		private bool _isUiLoaded;

		private string[] _statusMsg = new string[12];

		private string[] _statusSubMsg = new string[12];

		private Stopwatch timer = new Stopwatch();

		private Stopwatch blinktimer = new Stopwatch();

		private StringBuilder _sb = new StringBuilder();

		private const int LastWaitTime = 2000;

		public StartupProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public override void OnAddProcess()
		{
		}

		public override void OnRelease()
		{
			if (_monitors != null)
			{
				for (int i = 0; i < _monitors.Length; i++)
				{
					Object.Destroy(_monitors[i].gameObject);
				}
			}
		}

		public override void OnStart()
		{
			SetupUi();
			Singleton<OperationManager>.Instance.SetCoinBlockerMode(CoinBlocker.Mode.Initialize);
			MechaManager.SetAllCuOff();
			GameManager.StreamingAssetsPath = Application.streamingAssetsPath;
			Singleton<DataManager>.Instance.Load();
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.EnableRead(flag: false);
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.StartTouchPanel();
			GenericProcess genericProcess = new GenericProcess(container);
			container.processManager.AddProcess(genericProcess, 51);
			container.processManager.SetMessageManager(genericProcess);
			Singleton<OperationManager>.Instance.Initialize();
			_statusMsg[0] = HardInitializeTextID.TouchPanel1P.GetName();
			_statusMsg[1] = HardInitializeTextID.TouchPanel2P.GetName();
			_statusMsg[2] = HardInitializeTextID.Led1P.GetName();
			_statusMsg[3] = HardInitializeTextID.Led2P.GetName();
			_statusMsg[6] = HardInitializeTextID.Camera.GetName();
			_statusMsg[4] = HardInitializeTextID.CodeReader1P.GetName();
			_statusMsg[5] = HardInitializeTextID.CodeReader2P.GetName();
			_statusMsg[7] = HardInitializeTextID.DataLoad.GetName();
			_statusMsg[8] = "";
			_statusMsg[9] = HardInitializeTextID.Link.GetName();
			_statusMsg[10] = (LanInstall.IsServer ? HardInitializeTextID.DeliveryServer.GetName() : HardInitializeTextID.DeliveryClient.GetName());
			_statusMsg[11] = (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.gameSetting.IsStandardSettingMachine ? HardInitializeTextID.LinkServer.GetName() : HardInitializeTextID.LinkClient.GetName());
			blinktimer.Restart();
		}

		private bool SetupUi()
		{
			if (!_isUiLoaded)
			{
				GameObject prefs = Resources.Load<GameObject>("Process/Startup/StartupProcess");
				_monitors = new StartupMonitor[2]
				{
					CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<StartupMonitor>(),
					CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<StartupMonitor>()
				};
				for (int i = 0; i < _monitors.Length; i++)
				{
					_monitors[i].Initialize(i, active: true);
				}
				container.processManager.NotificationFadeIn();
				_isUiLoaded = true;
			}
			return _isUiLoaded;
		}

		public override void OnUpdate()
		{
			if (!SetupUi())
			{
				return;
			}
			switch (_state)
			{
			case StartUpState.WaitTouchPanel:
			{
				for (int i = 0; i < 2; i++)
				{
					if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.NewTouchPanel[i].Status == NewTouchPanel.StatusEnum.Drive || Singleton<SystemConfig>.Instance.config.IsDummyTouchPanel)
					{
						_statusSubMsg[i] = ConstParameter.TestString_Good;
					}
					else if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.NewTouchPanel[i].Status == NewTouchPanel.StatusEnum.Error)
					{
						_statusSubMsg[i] = ConstParameter.TestString_Bad;
					}
					else if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.NewTouchPanel[i].Status != NewTouchPanel.StatusEnum.Drive)
					{
						_statusSubMsg[i] = (Utility.isBlinkDisp(blinktimer) ? ConstParameter.TestString_Check : "");
					}
				}
				if (((SingletonStateMachine<AmManager, AmManager.EState>.Instance.NewTouchPanel[0].Status != NewTouchPanel.StatusEnum.Drive && SingletonStateMachine<AmManager, AmManager.EState>.Instance.NewTouchPanel[0].Status != NewTouchPanel.StatusEnum.Error) || (SingletonStateMachine<AmManager, AmManager.EState>.Instance.NewTouchPanel[1].Status != NewTouchPanel.StatusEnum.Drive && SingletonStateMachine<AmManager, AmManager.EState>.Instance.NewTouchPanel[1].Status != NewTouchPanel.StatusEnum.Error)) && !Singleton<SystemConfig>.Instance.config.IsDummyTouchPanel)
				{
					break;
				}
				for (int j = 0; j < 2; j++)
				{
					_statusSubMsg[j] = ((SingletonStateMachine<AmManager, AmManager.EState>.Instance.NewTouchPanel[j].Status == NewTouchPanel.StatusEnum.Drive || Singleton<SystemConfig>.Instance.config.IsDummyTouchPanel) ? ConstParameter.TestString_Good : ConstParameter.TestString_Bad);
				}
				_state = StartUpState.WaitLed;
				for (int k = 0; k < 2; k++)
				{
					MechaManager.LedIf[k].Reset();
					if (!Singleton<SystemConfig>.Instance.config.IsDummyTouchPanel && SingletonStateMachine<AmManager, AmManager.EState>.Instance.NewTouchPanel[k].GetLastErrorPs() != 0)
					{
						AMDaemon.Error.Set((int)SingletonStateMachine<AmManager, AmManager.EState>.Instance.NewTouchPanel[k].GetLastErrorPs());
					}
				}
				break;
			}
			case StartUpState.WaitLed:
			{
				bool[] array = new bool[2];
				bool[] array2 = new bool[2];
				for (int l = 0; l < 2; l++)
				{
					if (!MechaManager.LedIf[l].IsInitialized(out array[l]))
					{
						_statusSubMsg[2 + l] = (Utility.isBlinkDisp(blinktimer) ? ConstParameter.TestString_Check : "");
					}
					else if (!array[l])
					{
						_statusSubMsg[2 + l] = ConstParameter.TestString_Bad;
						array2[l] = true;
					}
					else
					{
						_statusSubMsg[2 + l] = ConstParameter.TestString_Good;
						array2[l] = true;
					}
				}
				if (array2[0] && array2[1])
				{
					for (int m = 0; m < 2; m++)
					{
						_statusSubMsg[2 + m] = (array[m] ? ConstParameter.TestString_Good : ConstParameter.TestString_Bad);
					}
					_state = StartUpState.WaitCam;
					CameraManager.Initialize(container.monoBehaviour);
					WebCamManager.Reset();
					WebCamManager.Initialize(container.monoBehaviour);
					MechaManager.QrReader.Initialize(container.monoBehaviour);
				}
				break;
			}
			case StartUpState.WaitCam:
			{
				for (int n = 0; n < 3; n++)
				{
					_statusSubMsg[4 + n] = (CameraManager.IsAvailableCameras[n] ? ((CameraManager.CameraProcMode[n] == CameraManager.CameraProcEnum.Good) ? ConstParameter.TestString_Good : ConstParameter.TestString_Warn) : (Utility.isBlinkDisp(blinktimer) ? ConstParameter.TestString_Check : ""));
				}
				if (CameraManager.IsReady)
				{
					for (int num = 0; num < 3; num++)
					{
						_statusSubMsg[4 + num] = (CameraManager.IsAvailableCameras[num] ? ((CameraManager.CameraProcMode[num] == CameraManager.CameraProcEnum.Good) ? ConstParameter.TestString_Good : ConstParameter.TestString_Warn) : ConstParameter.TestString_Bad);
					}
					_state = StartUpState.WaitDataManager;
					timer.Reset();
					timer.Start();
					int errorNo = CameraManager.ErrorNo;
					if (errorNo != 0)
					{
						AMDaemon.Error.Set(errorNo);
					}
				}
				break;
			}
			case StartUpState.WaitDataManager:
				_statusSubMsg[7] = Singleton<DataManager>.Instance.GetLoadedTaskPer().ToString("0%");
				if (Singleton<DataManager>.Instance.IsLoaded())
				{
					Singleton<CollectionAchieve>.Instance.Configure();
					Singleton<MapMaster>.Instance.Initialize();
					_state = StartUpState.WaitLinkDelivery;
					BackupGameSetting gameSetting3 = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.gameSetting;
					_statusSubMsg[9] = gameSetting3.MachineGroupID.GetName();
					DeliveryChecker.IManager manager4 = DeliveryChecker.get();
					timer.Reset();
					timer.Start();
					if (manager4 == null)
					{
						_state = StartUpState.Ready;
						_statusSubMsg[10] = ConstParameter.TestString_NA;
						_statusSubMsg[11] = ConstParameter.TestString_NA;
					}
					else
					{
						bool isServer = LanInstall.IsServer;
						manager4.start(isServer);
					}
				}
				break;
			case StartUpState.WaitLinkDelivery:
			{
				DeliveryChecker.IManager manager2 = DeliveryChecker.get();
				string text2 = (Utility.isBlinkDisp(blinktimer) ? ConstParameter.TestString_Check : "");
				if (LanInstall.IsServer && !AMDaemon.Network.IsLanAvailable)
				{
					_statusSubMsg[10] = ConstParameter.TestString_Good;
					_state = StartUpState.WaitLinkSetting;
					break;
				}
				bool flag2 = false;
				if (manager2.isError())
				{
					if (manager2.getErrorID() == PartyDeliveryCheckerErrorID.Duplicate)
					{
						text2 = ConstParameter.TestString_Bad + CommonMessageID.SystemDupli.GetName();
						_state = StartUpState.WaitLinkSetting;
					}
					else
					{
						text2 = ConstParameter.TestString_Bad;
						_state = StartUpState.WaitLinkSetting;
					}
					flag2 = true;
				}
				if (manager2.isOk())
				{
					text2 = ConstParameter.TestString_Good;
					_state = StartUpState.WaitLinkSetting;
					flag2 = true;
				}
				_statusSubMsg[10] = text2;
				if (flag2)
				{
					_state = StartUpState.WaitLinkSetting;
					timer.Reset();
					timer.Stop();
					BackupGameSetting gameSetting2 = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.gameSetting;
					bool isStandardSettingMachine = gameSetting2.IsStandardSettingMachine;
					MachineGroupID machineGroupID2 = gameSetting2.MachineGroupID;
					Setting.IManager manager3 = Setting.get();
					manager3.retry();
					if (manager3.isStandardSettingMachine != isStandardSettingMachine || manager3.getGroup() != machineGroupID2)
					{
						manager3.start(isStandardSettingMachine, machineGroupID2);
					}
					if (isStandardSettingMachine)
					{
						Setting.Data data2 = new Setting.Data();
						data2.set(gameSetting2.IsEventMode, (int)gameSetting2.EventModeTrack);
						manager3.setData(data2);
					}
					manager3.setRetryEnable(isRetry: true);
					timer.Reset();
					timer.Start();
				}
				break;
			}
			case StartUpState.WaitLinkSetting:
			{
				BackupGameSetting gameSetting = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.gameSetting;
				Setting.IManager manager = Setting.get();
				string text = (Utility.isBlinkDisp(blinktimer) ? ConstParameter.TestString_Check : "");
				bool flag = false;
				if (manager.getGroup() == MachineGroupID.OFF)
				{
					_state = StartUpState.WaitLinkParty;
					text = ConstParameter.TestString_NA;
					flag = true;
				}
				if (manager.isDuplicate())
				{
					manager.terminate();
					AMDaemon.Error.Set(3201);
					text = ConstParameter.TestString_Bad;
					_state = StartUpState.WaitLinkParty;
					flag = true;
				}
				if (manager.isStandardSettingMachine)
				{
					if (Setting.getParam()._hostInfoSendInterval * 2 * 1000 < timer.ElapsedMilliseconds)
					{
						text = ConstParameter.TestString_Good;
						_state = StartUpState.WaitLinkParty;
						flag = true;
					}
				}
				else
				{
					if (manager.isNormal())
					{
						if (manager.isDataValid())
						{
							Setting.Data data = manager.getData();
							gameSetting.IsEventMode = data.isEventModeSettingAvailable;
							gameSetting.EventModeTrack = (EventModeMusicCountID)data.eventModeMusicCount;
						}
						text = ConstParameter.TestString_Good;
						_state = StartUpState.WaitLinkParty;
						flag = true;
					}
					if (Singleton<OperationManager>.Instance.IsRebootNeeded())
					{
						text = ConstParameter.TestString_NA;
						_state = StartUpState.WaitLinkParty;
						flag = true;
					}
				}
				_statusSubMsg[11] = text;
				if (flag)
				{
					_state = StartUpState.WaitLinkParty;
					timer.Reset();
					timer.Stop();
					Setting.get().setRetryEnable(isRetry: false);
					MachineGroupID machineGroupID = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.gameSetting.MachineGroupID;
					Manager.Party.Party.Party.Get().Start(machineGroupID);
					timer.Reset();
					timer.Start();
				}
				break;
			}
			case StartUpState.WaitLinkParty:
			{
				IManager manager6 = Manager.Party.Party.Party.Get();
				if (manager6.IsNormal() || manager6.IsError())
				{
					_state = StartUpState.WaitLinkAdvertise;
					timer.Reset();
					timer.Stop();
					MachineGroupID machineGroupID3 = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.gameSetting.MachineGroupID;
					Advertise.get().initialize(machineGroupID3);
					timer.Reset();
					timer.Start();
				}
				break;
			}
			case StartUpState.WaitLinkAdvertise:
			{
				IManager manager5 = Manager.Party.Party.Party.Get();
				if (manager5.IsNormal() || manager5.IsError())
				{
					_state = StartUpState.Ready;
					timer.Reset();
					timer.Start();
				}
				break;
			}
			case StartUpState.Ready:
				if (timer.ElapsedMilliseconds > 2000)
				{
					_state = StartUpState.Released;
					if (SceneManager.GetActiveScene().name == "DebugStartup")
					{
						container.monoBehaviour.GetComponent<DebugGameMainObject>().DebugAddProcess(this);
					}
					else
					{
						container.processManager.AddProcess(new WarningProcess(container), 50);
						container.processManager.ReleaseProcess(this);
					}
					Singleton<OperationManager>.Instance.SetCoinBlockerMode(CoinBlocker.Mode.Game);
					GameManager.Initialize();
					GameManager.IsInitializeEnd = true;
				}
				break;
			}
			string text3 = "";
			string text4 = "";
			_sb.Clear();
			string[] statusMsg = _statusMsg;
			foreach (string value in statusMsg)
			{
				_sb.Append(value);
				_sb.Append("\n");
			}
			text3 = _sb.ToString();
			_sb.Clear();
			statusMsg = _statusSubMsg;
			foreach (string value2 in statusMsg)
			{
				_sb.Append(value2);
				_sb.Append("\n");
			}
			text4 = _sb.ToString();
			for (int num3 = 0; num3 < 2; num3++)
			{
				_monitors[num3].SetMainMessage(text3, text4);
			}
		}

		public override void OnLateUpdate()
		{
		}
	}
}
