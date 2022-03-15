using System;
using System.Diagnostics;
using System.IO.Ports;
using AMDaemon;
using AMDaemon.Allnet;
using DB;
using IO;
using MAI2.Util;
using MAI2System;
using Manager;
using Monitor;
using UnityEngine;
using Util;

namespace Process
{
	public class PowerOnProcess : ProcessBase
	{
		private enum PowerOnState : byte
		{
			WaitAmDaemon,
			AmDaemonReady,
			WaitAsset,
			AssetReady,
			WaitAime,
			AimeReady,
			WaitNetWork,
			NetWorkReady,
			WaitTitleServer,
			TitleServerReady,
			Released
		}

		private const int NetworkTimeout = 15000;

		private PowerOnMonitor[] _monitors;

		private PowerOnState _state;

		private float _timeCounter;

		private bool _isUiLoaded;

		private string _statusMsg = "";

		private string _statusSubMsg = "";

		private string _preStatusMsg = "";

		private string _preStatusSubMsg = "";

		private float _waitTime = 2f;

		private Stopwatch blinktimer = new Stopwatch();

		private Stopwatch networkTimer = new Stopwatch();

		private readonly string[] _usb4Com = new string[2];

		public PowerOnProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public override void OnAddProcess()
		{
		}

		public override void OnRelease()
		{
			for (int i = 0; i < _monitors?.Length; i++)
			{
				UnityEngine.Object.Destroy(_monitors[i]?.gameObject);
			}
		}

		public override void OnStart()
		{
			SetupUi();
			Singleton<OperationManager>.Instance.SetCoinBlockerMode(CoinBlocker.Mode.Initialize);
			for (int i = 0; i < 2; i++)
			{
				_usb4Com[i] = "";
			}
			blinktimer.Restart();
			networkTimer.Restart();
		}

		private bool SetupUi()
		{
			if (!_isUiLoaded)
			{
				GameObject prefs = Resources.Load<GameObject>("Process/PowerOn/PowerOnProcess");
				_monitors = new PowerOnMonitor[2]
				{
					CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<PowerOnMonitor>(),
					CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<PowerOnMonitor>()
				};
				for (int i = 0; i < _monitors.Length; i++)
				{
					_monitors[i].Initialize(i, isActive: true);
				}
				container.assetManager.Initialize();
				Singleton<SlideManager>.Instance.Initialize(Application.streamingAssetsPath + "/Table/slide/");
				GameNoteImageContainer.Initialize();
				GameParticleContainer.Initialize();
				GameNotePrefabContainer.Initialize();
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
			_timeCounter += Time.deltaTime;
			switch (_state)
			{
			case PowerOnState.WaitAmDaemon:
				_statusMsg = SystemInitializeTextID.SystemCheck.GetName();
				_statusSubMsg = (Core.IsReady ? ConstParameter.TestString_Good : (Utility.isBlinkDisp(blinktimer) ? SystemInitializeTextID.SystemWait.GetName() : ""));
				if (Core.IsReady)
				{
					SystemBaseCheck();
					_state = PowerOnState.AmDaemonReady;
					InputManager.Initialize();
					Singleton<SystemConfig>.Instance.initializeAfterAMDaemonReady();
					Singleton<OperationManager>.Instance.Initialize();
					MechaManager.InitParam initParam = new MechaManager.InitParam();
					string[] portNames = SerialPort.GetPortNames();
					int num = Array.IndexOf(portNames, "COM20");
					int num2 = Array.IndexOf(portNames, "COM24");
					int num3 = Array.IndexOf(portNames, "COM28");
					int num4 = Array.IndexOf(portNames, "COM32");
					if (-1 != num)
					{
						_usb4Com[0] = "COM21";
						_usb4Com[1] = "COM23";
					}
					else if (-1 != num2)
					{
						_usb4Com[0] = "COM25";
						_usb4Com[1] = "COM27";
					}
					else if (-1 != num3)
					{
						_usb4Com[0] = "COM29";
						_usb4Com[1] = "COM31";
					}
					else if (-1 != num4)
					{
						_usb4Com[0] = "COM33";
						_usb4Com[1] = "COM35";
					}
					else
					{
						_usb4Com[0] = "COM21";
						_usb4Com[1] = "COM23";
					}
					for (int i = 0; i < 2; i++)
					{
						initParam.LedParam[i].ComName = _usb4Com[i];
						initParam.LedParam[i].Dummy = Singleton<SystemConfig>.Instance.config.IsDummyLed;
						initParam.LedParam[i].index = i;
					}
					MechaManager.Initialize(initParam);
					for (int j = 0; j < 2; j++)
					{
						MechaManager.Jvs.SetPwmOutput((byte)j, Color.black);
					}
					_timeCounter = 0f;
				}
				break;
			case PowerOnState.AmDaemonReady:
				_preStatusMsg = _statusMsg;
				_preStatusSubMsg = _statusSubMsg;
				_state = PowerOnState.WaitAsset;
				break;
			case PowerOnState.WaitAsset:
			{
				bool flag3 = container.assetManager.IsDone() && !Singleton<SlideManager>.Instance.IsActiveThread();
				_statusMsg = _preStatusMsg + "\n" + SystemInitializeTextID.DataCheck.GetName();
				_statusSubMsg = _preStatusSubMsg + "\n";
				_statusSubMsg += (flag3 ? ConstParameter.TestString_Good : (Utility.isBlinkDisp(blinktimer) ? SystemInitializeTextID.DataWait.GetName() : ""));
				if (flag3)
				{
					_timeCounter = 0f;
					_state = PowerOnState.AssetReady;
				}
				break;
			}
			case PowerOnState.AssetReady:
				if (_timeCounter > _waitTime)
				{
					_state = PowerOnState.WaitAime;
				}
				break;
			case PowerOnState.WaitAime:
				_statusMsg = SystemInitializeTextID.AimeReader.GetName();
				_statusSubMsg = "";
				if (Aime.IsFirmUpdating)
				{
					_statusSubMsg = Aime.FirmUpdateProgress.ToString("###");
					break;
				}
				_state = PowerOnState.AimeReady;
				_timeCounter = 0f;
				_statusSubMsg = "100";
				break;
			case PowerOnState.AimeReady:
				if (_timeCounter > _waitTime)
				{
					_state = PowerOnState.WaitNetWork;
					networkTimer.Restart();
				}
				break;
			case PowerOnState.WaitNetWork:
			{
				NetworkTestInfo powerOnTestInfo = AMDaemon.Network.PowerOnTestInfo;
				bool flag2 = false;
				_statusMsg = "";
				_statusSubMsg = "";
				foreach (NetworkTestItem availableItem in powerOnTestInfo.AvailableItems)
				{
					_statusMsg += availableItem.ToText();
					_statusMsg += "\n";
					if (!powerOnTestInfo.IsBusy(availableItem) && !powerOnTestInfo.IsDone(availableItem))
					{
						_statusSubMsg += (flag2 ? ConstParameter.TestString_NA : "");
					}
					else if (powerOnTestInfo.IsBusy(availableItem))
					{
						if (powerOnTestInfo.GetResult(availableItem) == TestResult.Bad)
						{
							flag2 = true;
							_statusSubMsg += ConstParameter.TestString_Bad;
						}
						else if (availableItem == NetworkTestItem.IpAddress && networkTimer.ElapsedMilliseconds > 15000)
						{
							flag2 = true;
							_statusSubMsg += ConstParameter.TestString_Bad;
						}
						else if (availableItem == NetworkTestItem.Aime && isAimeError())
						{
							flag2 = true;
							_statusSubMsg += ConstParameter.TestString_Bad;
						}
						else
						{
							_statusSubMsg += (Utility.isBlinkDisp(blinktimer) ? ConstParameter.TestString_Check : "");
						}
					}
					else if (!powerOnTestInfo.IsBusy(availableItem) && powerOnTestInfo.IsDone(availableItem))
					{
						switch (powerOnTestInfo.GetResult(availableItem))
						{
						case TestResult.NA:
							_statusSubMsg += ConstParameter.TestString_NA;
							break;
						case TestResult.Good:
							_statusSubMsg += ((availableItem == NetworkTestItem.Hops) ? powerOnTestInfo.Hops.ToString() : ConstParameter.TestString_Good);
							break;
						case TestResult.Bad:
							_statusSubMsg += ConstParameter.TestString_Bad;
							break;
						}
					}
					_statusSubMsg += "\n";
				}
				_statusMsg += TestmodeNetworkID.Label08.GetName();
				if (powerOnTestInfo.IsCompleted || flag2)
				{
					_state = PowerOnState.NetWorkReady;
					_timeCounter = 0f;
					Singleton<OperationManager>.Instance.StartTest();
				}
				_state = PowerOnState.NetWorkReady;
				_timeCounter = 0f;
				Singleton<OperationManager>.Instance.StartTest();
				break;
			}
			case PowerOnState.NetWorkReady:
				_preStatusMsg = _statusMsg;
				_preStatusSubMsg = _statusSubMsg;
				_state = PowerOnState.WaitTitleServer;
				break;
			case PowerOnState.WaitTitleServer:
			{
				bool flag = false;
				_statusMsg = _preStatusMsg;
				_statusSubMsg = _preStatusSubMsg;
				if (!Auth.IsGood)
				{
					_statusSubMsg += ConstParameter.TestString_NA;
					flag = true;
				}
				else if (Singleton<OperationManager>.Instance.IsAliveServer)
				{
					_statusSubMsg += ConstParameter.TestString_Good;
					flag = true;
				}
				else if (Singleton<OperationManager>.Instance.IsBusy())
				{
					_statusSubMsg += (Utility.isBlinkDisp(blinktimer) ? ConstParameter.TestString_Check : "");
				}
				else
				{
					_statusSubMsg += ConstParameter.TestString_Bad;
					flag = true;
				}
				if (flag)
				{
					_timeCounter = 0f;
					_state = PowerOnState.TitleServerReady;
				}
				break;
			}
			case PowerOnState.TitleServerReady:
				if (_timeCounter > _waitTime)
				{
					_state = PowerOnState.Released;
				}
				break;
			case PowerOnState.Released:
				container.processManager.AddProcess(new StartupProcess(container), 50);
				container.processManager.ReleaseProcess(this);
				GameManager.IsGameProcMode = true;
				break;
			}
			for (int k = 0; k < 2; k++)
			{
				PowerOnMonitor powerOnMonitor = _monitors[k];
				if (powerOnMonitor != null)
				{
					powerOnMonitor.SetMainMessage(_statusMsg, _statusSubMsg);
				}
			}
		}

		public override void OnLateUpdate()
		{
		}

		protected void SystemBaseCheck()
		{
			ErrorInfo info = AMDaemon.Error.Info;
			if (info != null && info.IsOccurred && info.Number == 912)
			{
				GameManager.IsGotoSystemError = true;
			}
		}

		private bool isAimeError()
		{
			bool result = false;
			if (AMDaemon.Error.IsOccurred)
			{
				int number = AMDaemon.Error.Number;
				if (number == 6501 || number == 6506)
				{
					result = true;
				}
			}
			return result;
		}
	}
}
