using DB;
using MAI2.Util;
using Manager;
using Monitor.CardMakerInfo;
using UnityEngine;

namespace Process.CardMakerInfo
{
	public class CardMakerInfoProcess : ProcessBase
	{
		private enum CardMakerInfoState : byte
		{
			Wait,
			Disp,
			DispEnd,
			GotoEnd,
			Released
		}

		private CardMakerInfoMonitor[] _monitors;

		private CardMakerInfoState _state;

		private float _timeCounter;

		public const uint WindowTimeOut = 8000u;

		public const uint WindowInputWait = 1000u;

		public CardMakerInfoProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public override void OnAddProcess()
		{
		}

		public override void OnRelease()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				Object.Destroy(_monitors[i].gameObject);
			}
		}

		public override void OnStart()
		{
			GameObject prefs = Resources.Load<GameObject>("Process/CardMakerInfo/CardMakerInfoProcess");
			_monitors = new CardMakerInfoMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<CardMakerInfoMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<CardMakerInfoMonitor>()
			};
			for (int i = 0; i < _monitors.Length; i++)
			{
				_monitors[i].Initialize(i, Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry);
			}
			_state = CardMakerInfoState.Wait;
			if (_monitors[0].IsFinish() && _monitors[1].IsFinish())
			{
				_state = CardMakerInfoState.GotoEnd;
			}
			else
			{
				container.processManager.NotificationFadeIn();
			}
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			switch (_state)
			{
			case CardMakerInfoState.GotoEnd:
				_state = CardMakerInfoState.Released;
				container.processManager.AddProcess(new DataSaveProcess(container), 50);
				container.processManager.ReleaseProcess(this);
				break;
			case CardMakerInfoState.Wait:
			{
				_timeCounter += Time.deltaTime;
				if (!(_timeCounter >= 1f))
				{
					break;
				}
				_state = CardMakerInfoState.Disp;
				for (int j = 0; j < _monitors.Length; j++)
				{
					if (!Singleton<UserDataManager>.Instance.GetUserData(j).IsEntry)
					{
						continue;
					}
					if (_monitors[j].IsFinish())
					{
						if (!Singleton<UserDataManager>.Instance.IsSingleUser())
						{
							container.processManager.EnqueueMessage(j, WindowMessageID.PlayPreparationWait);
						}
						_monitors[j].SetStatus(CardMakerInfoMonitor.InfoStatus.Finish);
					}
					else
					{
						CardMakerInfoMonitor.WindowDataSt nowMessageId = _monitors[j].GetNowMessageId();
						_monitors[j].Play();
						container.processManager.EnqueueMessage(j, nowMessageId.windowId);
						SoundManager.PlayVoice(nowMessageId.cue, j);
						SetInputLockInfo(j, 1000f);
					}
				}
				break;
			}
			case CardMakerInfoState.Disp:
			{
				if (!IsNext())
				{
					break;
				}
				_state = CardMakerInfoState.DispEnd;
				_timeCounter = 0f;
				for (int k = 0; k < _monitors.Length; k++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(k).IsEntry)
					{
						container.processManager.CloseWindow(k);
					}
				}
				break;
			}
			case CardMakerInfoState.DispEnd:
				_timeCounter += Time.deltaTime;
				if (_timeCounter >= 1f)
				{
					_timeCounter = 0f;
					_state = CardMakerInfoState.Released;
					container.processManager.AddProcess(new FadeProcess(container, this, new DataSaveProcess(container), FadeProcess.FadeType.Type3), 50);
					container.processManager.SetVisibleTimers(isVisible: false);
					for (int i = 0; i < _monitors.Length; i++)
					{
						SoundManager.StopVoice(i);
					}
				}
				break;
			case CardMakerInfoState.Released:
				break;
			}
		}

		protected override void UpdateInput(int monitorId)
		{
			if ((int)_state < 1 || _monitors[monitorId].IsFinish() || !_monitors[monitorId].IsActive() || (!InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button04) && !_monitors[monitorId].TimeUp()))
			{
				return;
			}
			_monitors[monitorId].Next();
			if (_monitors[monitorId].IsFinish())
			{
				if (!Singleton<UserDataManager>.Instance.IsSingleUser())
				{
					container.processManager.EnqueueMessage(monitorId, WindowMessageID.PlayPreparationWait);
				}
				_monitors[monitorId].Stop();
			}
			else
			{
				CardMakerInfoMonitor.WindowDataSt nowMessageId = _monitors[monitorId].GetNowMessageId();
				container.processManager.EnqueueMessage(monitorId, nowMessageId.windowId);
				SoundManager.PlayVoice(nowMessageId.cue, monitorId);
				_monitors[monitorId].PushButton();
				SetInputLockInfo(monitorId, 1000f);
			}
		}

		public override void OnLateUpdate()
		{
		}

		public bool IsNext()
		{
			if (_monitors[0].IsFinish())
			{
				return _monitors[1].IsFinish();
			}
			return false;
		}
	}
}
