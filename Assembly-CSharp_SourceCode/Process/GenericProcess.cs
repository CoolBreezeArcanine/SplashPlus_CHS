using System;
using System.Collections;
using System.Collections.Generic;
using DB;
using MAI2.Util;
using Manager;
using UnityEngine;

namespace Process
{
	public class GenericProcess : ProcessBase, IGenericManager, IGeneric, ITimer
	{
		private enum State : byte
		{
			Idle,
			Released
		}

		public struct WindowInfo
		{
			public WindowMessageID MessageId;

			public WindowPositionID PositionId;
		}

		private GenericMonitor[] _monitors;

		private State _state;

		private Queue[] _messageQue;

		private Queue[] _warningMessageQue;

		private TimerController[] _timerController;

		private bool _isEntryProcess;

		private Action<int> _onCompleteCallback;

		private Queue<Action> _both = new Queue<Action>();

		private short _isAllTimeUpFlag;

		public GenericProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public void Enqueue(int monitorId, WindowMessageID messageId)
		{
			WindowInfo windowInfo = default(WindowInfo);
			windowInfo.MessageId = messageId;
			windowInfo.PositionId = messageId.GetPosition();
			WindowInfo windowInfo2 = windowInfo;
			_messageQue[monitorId].Enqueue(windowInfo2);
		}

		public void Enqueue(int monitorId, WindowMessageID messageId, WindowPositionID positionId)
		{
			WindowInfo windowInfo = default(WindowInfo);
			windowInfo.MessageId = messageId;
			windowInfo.PositionId = positionId;
			WindowInfo windowInfo2 = windowInfo;
			_messageQue[monitorId].Enqueue(windowInfo2);
		}

		public void EnqueueWarning(WarningWindowInfo info)
		{
			_warningMessageQue[info.MonitorId].Enqueue(info);
		}

		public void AllInterrupt()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				_monitors[i].AllInterrupt();
				_monitors[i].WarningInterrupt();
			}
		}

		public void AllInterrupt(int monitorId)
		{
			_monitors[monitorId].AllInterrupt();
		}

		public void WarningInterrupt(int monitorId)
		{
			_monitors[monitorId].WarningInterrupt();
		}

		public void CheckQueue()
		{
			if (_warningMessageQue != null)
			{
				EpelQueue(_warningMessageQue);
			}
			if (_messageQue != null)
			{
				EpelQueue(_messageQue);
			}
		}

		private void EpelQueue(Queue[] que)
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				while (1 < que[i].Count)
				{
					que[i].Dequeue();
				}
			}
		}

		private void Execute()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				if (0 < _warningMessageQue[i].Count)
				{
					_monitors[i].WarningInterrupt();
					WarningWindowInfo warningWindowInfo = (WarningWindowInfo)_warningMessageQue[i].Dequeue();
					if (warningWindowInfo != null)
					{
						_monitors[i].SetWarningWindow(warningWindowInfo, Vector3.zero);
					}
				}
				if (0 < _messageQue[i].Count)
				{
					_monitors[i].AllInterrupt();
					WindowInfo messageWindow = (WindowInfo)_messageQue[i].Dequeue();
					_monitors[i].SetMessageWindow(messageWindow);
				}
			}
		}

		public bool IsOpening(int monitorId, WindowPositionID positionId)
		{
			return _monitors[monitorId].IsOpening(positionId);
		}

		public bool IsOpeningWarningWindow(int monitorId)
		{
			return _monitors[monitorId].IsOpeningWarningWindow();
		}

		public void Close(int monitorId)
		{
			_monitors[monitorId].AllClose();
		}

		public void ForcedClose(int monitorId)
		{
			_messageQue[monitorId].Clear();
			_monitors[monitorId].ForcedAllClose();
		}

		public void Close(int monitorId, WindowPositionID positionId)
		{
			_monitors[monitorId].Close(positionId);
		}

		public void CloseWarningWindow(int monitorId)
		{
			_monitors[monitorId].CloseWarnigWindow();
		}

		public override void OnAddProcess()
		{
		}

		public override void OnLateUpdate()
		{
		}

		public override void OnRelease()
		{
			if (_monitors != null)
			{
				for (int i = 0; i < _monitors.Length; i++)
				{
					UnityEngine.Object.Destroy(_monitors[i].gameObject);
				}
			}
		}

		public override void OnStart()
		{
			GameObject prefs = Resources.Load<GameObject>("Process/Generic/GenericProcess");
			_monitors = new GenericMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<GenericMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<GenericMonitor>()
			};
			_warningMessageQue = new Queue[_monitors.Length];
			_messageQue = new Queue[_monitors.Length];
			_timerController = new TimerController[_monitors.Length];
			for (int i = 0; i < _monitors.Length; i++)
			{
				_monitors[i].Initialize(i, active: true);
				CreateTimer(i);
				_warningMessageQue[i] = new Queue();
				_messageQue[i] = new Queue();
				_timerController[i] = new TimerController();
				_timerController[i].PrepareTimer(0);
			}
		}

		public override void OnUpdate()
		{
			if (_state != 0)
			{
				_ = 1;
				return;
			}
			CheckQueue();
			Execute();
			for (int i = 0; i < _monitors.Length; i++)
			{
				UpdateTimer(i);
				_monitors[i].ViewUpdate();
			}
		}

		private void CreateTimer(int monitorId)
		{
			_monitors[monitorId].CreateTimer();
		}

		public void PrepareTimer(int second, int delayCount, bool isEntryProcess, Action both, bool isVisible = true)
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				if (isEntryProcess || Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry)
				{
					_timerController[i].PrepareTimer(second);
					_timerController[i].DelayCount(delayCount);
					SetVisibleTimer(i, isVisible);
					_monitors[i].SetTimerSecond((uint)second);
				}
			}
			_isEntryProcess = isEntryProcess;
			_isAllTimeUpFlag = 0;
			_onCompleteCallback = null;
			_both.Clear();
			if (both != null)
			{
				_both.Enqueue(both);
			}
		}

		public void SetCompleteOneSide(Action<int> single)
		{
			_onCompleteCallback = single;
		}

		public void SetVisibleTimer(int monitorId, bool isVisible)
		{
			if (GameManager.IsFreedomMode)
			{
				int num = ((!Singleton<UserDataManager>.Instance.GetUserData(0L).IsEntry) ? 1 : 0);
				if (num == monitorId)
				{
					_monitors[num].SetVisibleTimer(isVisible);
				}
			}
			else
			{
				_monitors[monitorId].SetVisibleTimer(isVisible);
			}
		}

		public void SetVisibleTimer(bool isVisible)
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				_monitors[i].SetVisibleTimer(isVisible);
			}
		}

		private void UpdateTimer(int monitorId)
		{
			_timerController[monitorId].UpdateTimer(GameManager.GetGameMSecAdd(), monitorId);
			CheckCompleted();
			_monitors[monitorId].SetTimerSecond(_timerController[monitorId].CountDownSecond, _timerController[monitorId].IsInfinity);
		}

		private void CheckCompleted()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				if (!_isEntryProcess && !Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry)
				{
					_isAllTimeUpFlag |= (short)(1 << i);
				}
				else if (_timerController[i].IsCompleted && (_isAllTimeUpFlag & (short)(1 << i)) == 0)
				{
					_onCompleteCallback?.Invoke(i);
					_isAllTimeUpFlag |= (short)(1 << i);
				}
			}
			if (_isAllTimeUpFlag == 3)
			{
				if (0 < _both.Count)
				{
					_both.Dequeue()?.Invoke();
				}
				_onCompleteCallback = null;
			}
		}

		public void DecrementTime(int monitorId, int decrementValue)
		{
			if (_isEntryProcess || Singleton<UserDataManager>.Instance.GetUserData(monitorId).IsEntry)
			{
				_timerController[monitorId].DecrementTime(decrementValue);
			}
		}

		public void IsFastSkip(int monitorId, bool isFast)
		{
			if (_isEntryProcess || Singleton<UserDataManager>.Instance.GetUserData(monitorId).IsEntry)
			{
				_timerController[monitorId].IsFastSkip = isFast;
			}
		}

		public void IsLongSkip(int monitorId, bool isLongSkip)
		{
			if (_isEntryProcess || Singleton<UserDataManager>.Instance.GetUserData(monitorId).IsEntry)
			{
				_timerController[monitorId].IsLongSkip = isLongSkip;
			}
		}

		public void IsTimeCounting(bool isTimeCount)
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				if (_isEntryProcess || Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry)
				{
					_timerController[i].IsTimeCounting = isTimeCount;
				}
			}
		}

		public void IsTimeCounting(int monitorId, bool isTimeCount)
		{
			if (_isEntryProcess || Singleton<UserDataManager>.Instance.GetUserData(monitorId).IsEntry)
			{
				_timerController[monitorId].IsTimeCounting = isTimeCount;
			}
		}

		public bool IsTimeUp(int monitorId)
		{
			bool result = true;
			if (_timerController != null && _timerController[monitorId] != null)
			{
				result = _timerController[monitorId].IsTimeUp;
			}
			return result;
		}

		public void ForceTimeUp()
		{
			for (int i = 0; i < _timerController.Length; i++)
			{
				if (_isEntryProcess || Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry)
				{
					_timerController[i].SetTimeZero();
				}
			}
			_isAllTimeUpFlag = 3;
			_both.Clear();
			_onCompleteCallback = null;
		}

		public void ForceTimeUp(int monitorId)
		{
			if (_isEntryProcess || Singleton<UserDataManager>.Instance.GetUserData(monitorId).IsEntry)
			{
				_timerController[monitorId].SetTimeZero();
				_onCompleteCallback = null;
			}
		}

		public void SetTimerSecurity(int second, int delayCount, Action both)
		{
			for (int i = 0; i < _timerController.Length; i++)
			{
				if (_isEntryProcess || Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry)
				{
					_timerController[i].SetTimerSecurity(second);
					_timerController[i].DelayCount(delayCount);
				}
			}
			_isAllTimeUpFlag = 0;
			if (both != null && _both.Count == 0)
			{
				_both.Enqueue(both);
			}
		}

		public bool CheckZombiAction(ProcessBase process)
		{
			foreach (Action item in _both)
			{
				if (item.Target == process)
				{
					return true;
				}
			}
			return false;
		}

		public void ClearTimeoutAction()
		{
			_both.Clear();
		}
	}
}
