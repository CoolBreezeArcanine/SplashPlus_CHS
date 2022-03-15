using System.Collections;
using AMDaemon;
using DB;
using Mai2.Mai2Cue;
using MAI2.Util;
using Manager;
using Monitor;
using UnityEngine;

namespace Process
{
	public class PleaseWaitProcess : ProcessBase
	{
		public enum RemainingState
		{
			Normal = 600,
			OneMinute = 60,
			TenSecond = 11,
			TimeUp = 0
		}

		private enum FreedomModeState : byte
		{
			Init,
			CountDonw,
			TimeUp,
			TerminationCheck
		}

		private enum ReaderState
		{
			Wait,
			Start,
			Polling,
			Cancelling,
			Finish
		}

		private ReaderState _readerState;

		public const ushort MessageID_Kill = 20000;

		public const ushort MessageID_FreedomModeEnd = 20001;

		public const ushort MessageID_FreedomModePause = 20002;

		public const ushort MessageID_FreedomModeRainbowAlpha = 20003;

		public const ushort MessageID_FreedomModeIntroduction = 20004;

		public const ushort MessageID_RestartPleaseWait1P = 20005;

		public const ushort MessageID_RestartPleaseWait2P = 20006;

		public const int FreedomModeMessageTime = 5000;

		private PleaseWaitMonitor[] _monitors;

		private bool[] _isNotEntrys;

		private int _beforeSeconds;

		private int _beforeMinutes;

		private AimeUnit _unit;

		private float _timer;

		private RemainingState _remaining = RemainingState.Normal;

		private FreedomModeState _freedomModeState;

		public PleaseWaitProcess(ProcessDataContainer dataContainer)
			: base(dataContainer, ProcessType.PleaseWaitProcess)
		{
		}

		public override void OnAddProcess()
		{
		}

		public override void OnStart()
		{
			GameObject prefs = Resources.Load<GameObject>("Process/PleaseWait/PleaseWaitProcess");
			_monitors = new PleaseWaitMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<PleaseWaitMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<PleaseWaitMonitor>()
			};
			_isNotEntrys = new bool[_monitors.Length];
			for (int i = 0; i < _monitors.Length; i++)
			{
				_isNotEntrys[i] = !Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry;
				_monitors[i].Initialize(i, _isNotEntrys[i]);
			}
			_remaining = RemainingState.Normal;
		}

		public override void OnUpdate()
		{
			double num = (double)GameManager.GetFreedomModeMSec() * 0.001;
			int num2 = (int)(num % 60.0);
			int num3 = (int)(num / 60.0);
			PleaseWaitMonitor[] monitors = _monitors;
			foreach (PleaseWaitMonitor pleaseWaitMonitor in monitors)
			{
				if (!(null != pleaseWaitMonitor))
				{
					continue;
				}
				if (_isNotEntrys[pleaseWaitMonitor.MonitorIndex] && GameManager.IsFreedomMode && GameManager.IsFreedomCountDown)
				{
					FreedomModeInputUpdate(pleaseWaitMonitor);
					if (_freedomModeState == FreedomModeState.Init)
					{
						_beforeSeconds = num2;
						_beforeMinutes = num3;
						pleaseWaitMonitor.SetOneMinute(isOneMinute: false);
						pleaseWaitMonitor.SetTimerColor(Color.white);
						_remaining = RemainingState.Normal;
						_unit = Aime.Units[0];
						_readerState = ReaderState.Start;
						_freedomModeState = FreedomModeState.CountDonw;
					}
					if (_freedomModeState == FreedomModeState.TerminationCheck)
					{
						int monitorIndex = pleaseWaitMonitor.MonitorIndex;
						if (!IsInputLock(monitorIndex))
						{
							if (InputManager.GetButtonDown(monitorIndex, InputManager.ButtonSetting.Button04))
							{
								pleaseWaitMonitor.PressedButton(InputManager.ButtonSetting.Button04);
								pleaseWaitMonitor.SetTime(0, 0);
								pleaseWaitMonitor.SetButtonColor(0);
								pleaseWaitMonitor.PlayTimeUp();
								container.processManager.CloseWindow(monitorIndex);
								GameManager.ForcedTerminationForFreedomMode();
								_freedomModeState = FreedomModeState.TimeUp;
								_timer = 0f;
								pleaseWaitMonitor.SetCountDown();
								_unit.Cancel();
								_unit.SetLed(onR: false, onG: false, onB: false);
								_unit = null;
								_readerState = ReaderState.Finish;
							}
							else if (InputManager.GetButtonDown(monitorIndex, InputManager.ButtonSetting.Button05))
							{
								pleaseWaitMonitor.PressedButton(InputManager.ButtonSetting.Button05);
								container.monoBehaviour.StartCoroutine(ResetAimeReaderCoroutine());
								_freedomModeState = FreedomModeState.CountDonw;
								container.processManager.CloseWindow(monitorIndex);
								_timer = 0f;
								pleaseWaitMonitor.SetCountDown();
							}
						}
						if (_timer >= 5000f)
						{
							pleaseWaitMonitor.SetCountDown();
							container.monoBehaviour.StartCoroutine(ResetAimeReaderCoroutine());
							_freedomModeState = FreedomModeState.CountDonw;
							container.processManager.CloseWindow(monitorIndex);
							_timer = 0f;
						}
						else
						{
							_timer += GameManager.GetGameMSecAdd();
						}
					}
					if (num <= 0.0 && _remaining == RemainingState.TenSecond)
					{
						_freedomModeState = FreedomModeState.TimeUp;
						_remaining = RemainingState.TimeUp;
						if (!GameManager.IsInGame)
						{
							for (int j = 0; j < _monitors.Length; j++)
							{
								SoundManager.PlaySE(Cue.SE_FREEDOM_TIMEUP, j);
							}
						}
						container.processManager.CloseWindow(pleaseWaitMonitor.MonitorIndex);
						pleaseWaitMonitor.PlayTimeUp();
						if (_unit != null)
						{
							_unit.Cancel();
							_unit.SetLed(onR: false, onG: false, onB: false);
							_unit = null;
						}
					}
					if (_beforeSeconds != num2 && _freedomModeState != FreedomModeState.TimeUp)
					{
						_beforeSeconds = num2;
						if (_remaining == RemainingState.OneMinute && num <= 11.0)
						{
							_remaining = RemainingState.TenSecond;
							pleaseWaitMonitor.SetTimerColor(Color.red);
						}
						else if (_remaining == RemainingState.Normal && num <= 60.0)
						{
							_remaining = RemainingState.OneMinute;
							pleaseWaitMonitor.SetOneMinute(isOneMinute: true);
						}
						if (_remaining == RemainingState.TenSecond)
						{
							pleaseWaitMonitor.Play10CountDown();
						}
						pleaseWaitMonitor.SetRainbow(num3, num2);
						pleaseWaitMonitor.SetTime(num3, num2);
						if (_beforeMinutes != num3)
						{
							_beforeMinutes = num3;
							pleaseWaitMonitor.SetButtonColor(num3);
						}
						pleaseWaitMonitor.SetSeconHand((float)num2 / 60f * 360f);
						pleaseWaitMonitor.SetMinuteHand((float)num3 / 10f * 360f);
					}
				}
				if (_isNotEntrys[pleaseWaitMonitor.MonitorIndex] && GameManager.IsFreedomMode && GameManager.IsFreedomTimeUp && pleaseWaitMonitor._isDispButton)
				{
					pleaseWaitMonitor.SetCountDown();
				}
				pleaseWaitMonitor.ViewUpdate();
			}
			base.OnUpdate();
		}

		private void FreedomModeInputUpdate(PleaseWaitMonitor t)
		{
			if (_unit == null || _freedomModeState != FreedomModeState.CountDonw)
			{
				return;
			}
			switch (_readerState)
			{
			case ReaderState.Start:
				_unit.Start(AimeCommand.ScanOffline);
				_unit.SetLedStatus(AimeLedStatus.Scanning);
				_readerState = ReaderState.Polling;
				break;
			case ReaderState.Polling:
				if (_unit.IsBusy)
				{
					break;
				}
				if (_unit.HasError)
				{
					_unit.SetLedStatus(AimeLedStatus.Error);
					AimeErrorInfo errorInfo = _unit.ErrorInfo;
					_ = "[Error]\n" + $"ID          : {errorInfo.Id}\n" + $"Category    : {errorInfo.Category}\n" + $"Number      : {errorInfo.Number}\n" + "Message     : " + errorInfo.Message;
				}
				else
				{
					_unit.SetLedStatus(AimeLedStatus.Success);
					if (_unit.HasResult)
					{
						AimeOfflineId offlineId = _unit.Result.OfflineId;
						string text = "";
						switch (offlineId.Type)
						{
						case AimeOfflineIdType.AccessCode:
							text = offlineId.GetAccessCodeData().ToString();
							break;
						case AimeOfflineIdType.FeliCaId:
							text = offlineId.GetFeliCaIdData().ToString();
							break;
						}
						int num = ((t.MonitorIndex == 0) ? 1 : 0);
						string offlineId2 = Singleton<UserDataManager>.Instance.GetUserData(num).OfflineId;
						if (text == offlineId2)
						{
							t.SetTerminationCheck();
							container.processManager.EnqueueMessage(t.MonitorIndex, WindowMessageID.FreedomModeTerminationMessage);
							SetInputLockInfo(t.MonitorIndex, 500f);
							_timer = 0f;
							_freedomModeState = FreedomModeState.TerminationCheck;
						}
						else
						{
							_unit.SetLedStatus(AimeLedStatus.Error);
						}
					}
				}
				_unit.Cancel();
				_readerState = ReaderState.Wait;
				container.monoBehaviour.StartCoroutine(ResetAimeReaderCoroutine());
				break;
			case ReaderState.Finish:
				if (_unit != null)
				{
					_unit.Cancel();
					_unit.SetLed(onR: false, onG: false, onB: false);
					_unit = null;
				}
				break;
			case ReaderState.Wait:
			case ReaderState.Cancelling:
				break;
			}
		}

		private IEnumerator ResetAimeReaderCoroutine()
		{
			yield return new WaitForSeconds(0.5f);
			if (_readerState == ReaderState.Wait)
			{
				_readerState = ReaderState.Start;
			}
		}

		public override void OnLateUpdate()
		{
		}

		public override void OnRelease()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				Object.Destroy(_monitors[i].gameObject);
			}
		}

		public override object HandleMessage(Message message)
		{
			switch (message.Id)
			{
			case 20000:
				container.processManager.ReleaseProcess(this);
				break;
			case 20001:
			{
				PleaseWaitMonitor[] monitors = _monitors;
				for (int i = 0; i < monitors.Length; i++)
				{
					monitors[i].PlayFreedomModeTimerOut();
				}
				break;
			}
			case 20002:
			{
				PleaseWaitMonitor[] monitors = _monitors;
				for (int i = 0; i < monitors.Length; i++)
				{
					monitors[i].PauseTimer((bool)message.Param[0]);
				}
				break;
			}
			case 20003:
			{
				PleaseWaitMonitor[] monitors = _monitors;
				for (int i = 0; i < monitors.Length; i++)
				{
					monitors[i].SetVisibleRainbow((bool)message.Param[0]);
				}
				break;
			}
			case 20004:
			{
				PleaseWaitMonitor[] monitors = _monitors;
				for (int i = 0; i < monitors.Length; i++)
				{
					monitors[i].PlayFreedomModeTimerIntroduction();
				}
				break;
			}
			case 20005:
			{
				int num2 = 0;
				_isNotEntrys[num2] = !Singleton<UserDataManager>.Instance.GetUserData(num2).IsEntry;
				_monitors[num2].ReInitialize(num2, _isNotEntrys[num2]);
				_remaining = RemainingState.Normal;
				break;
			}
			case 20006:
			{
				int num = 1;
				_isNotEntrys[num] = !Singleton<UserDataManager>.Instance.GetUserData(num).IsEntry;
				_monitors[num].ReInitialize(num, _isNotEntrys[num]);
				_remaining = RemainingState.Normal;
				break;
			}
			}
			return null;
		}
	}
}
