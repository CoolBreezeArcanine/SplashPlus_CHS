using System;
using System.Collections.Generic;
using DB;
using Process;

namespace Manager
{
	public class ProcessManager
	{
		public enum ProcessState : byte
		{
			Start,
			Update,
			Expried,
			Released
		}

		public class ProcessControle
		{
			public ProcessBase Process;

			public byte Priority;

			public ProcessState State;
		}

		private uint _currentProcessId;

		private uint _fadeProcessId;

		private bool _isException;

		private readonly LinkedList<ProcessControle> _processList = new LinkedList<ProcessControle>();

		private readonly Action<Exception> _errorHandler;

		private IGenericManager _genericManager;

		public ProcessManager(Action<Exception> handler)
		{
			_errorHandler = handler;
			_isException = false;
		}

		public void SetMessageManager(IGenericManager genericManager)
		{
			_genericManager = genericManager;
		}

		public void EnqueueMessage(int monitorId, WindowMessageID messageId)
		{
			_genericManager?.Enqueue(monitorId, messageId);
		}

		public void EnqueueMessage(int monitorId, WindowMessageID messageId, WindowPositionID positionId)
		{
			_genericManager?.Enqueue(monitorId, messageId, positionId);
		}

		public void EnqueueWarningMessage(WarningWindowInfo info)
		{
			_genericManager?.EnqueueWarning(info);
		}

		public bool IsOpening(int monitorId, WindowPositionID positionId)
		{
			return _genericManager?.IsOpening(monitorId, positionId) ?? false;
		}

		public void CloseWindow(int monitorId)
		{
			_genericManager?.Close(monitorId);
		}

		public void ForcedCloseWindow(int monitorId)
		{
			_genericManager?.ForcedClose(monitorId);
		}

		public void CloseWindow(int monitorId, WindowPositionID positionId)
		{
			_genericManager?.Close(monitorId, positionId);
		}

		public bool IsOpeningWarningWindow(int monitorId)
		{
			return _genericManager?.IsOpeningWarningWindow(monitorId) ?? false;
		}

		public void CloseWarningWindow(int monitorId)
		{
			_genericManager?.CloseWarningWindow(monitorId);
		}

		private void Interrupt()
		{
			_genericManager?.AllInterrupt();
		}

		public void PrepareTimer(int second, int delayCount, bool isEntry, Action both, bool isVisible = true)
		{
			_genericManager?.PrepareTimer(second, delayCount, isEntry, both, isVisible);
		}

		public void SetCompleteOneSide(Action<int> single)
		{
			_genericManager?.SetCompleteOneSide(single);
		}

		public void DecrementTime(int monitorId, int decrementValue)
		{
			_genericManager?.DecrementTime(monitorId, decrementValue);
		}

		public void IsFastSkip(int monitorId, bool isFast)
		{
			_genericManager?.IsFastSkip(monitorId, isFast);
		}

		public void IsLongSkip(int monitorId, bool isLongSkip)
		{
			_genericManager?.IsLongSkip(monitorId, isLongSkip);
		}

		public void IsTimeCounting(bool isTimeCount)
		{
			_genericManager?.IsTimeCounting(isTimeCount);
		}

		public void IsTimeCounting(int monitorId, bool isTimeCount)
		{
			_genericManager?.IsTimeCounting(monitorId, isTimeCount);
		}

		public void SetVisibleTimer(int monitorId, bool isVisible)
		{
			_genericManager?.SetVisibleTimer(monitorId, isVisible);
		}

		public void SetVisibleTimers(bool isVisible)
		{
			_genericManager?.SetVisibleTimer(isVisible);
		}

		public bool IsTimeUp(int monitorId)
		{
			bool result = true;
			if (_genericManager != null)
			{
				result = _genericManager.IsTimeUp(monitorId);
			}
			return result;
		}

		public void ForceTimeUp()
		{
			_genericManager?.ForceTimeUp();
		}

		public void ForceTimeUp(int monitorId)
		{
			_genericManager?.ForceTimeUp(monitorId);
		}

		public void ClearTimeoutAction()
		{
			_genericManager?.ClearTimeoutAction();
		}

		public void SetTimerSecurity(int second, int delayCount, Action both)
		{
			_genericManager?.SetTimerSecurity(second, delayCount, both);
		}

		public void Update()
		{
			if (_isException)
			{
				return;
			}
			LinkedListNode<ProcessControle> linkedListNode = _processList.First;
			try
			{
				while (linkedListNode != null)
				{
					switch (linkedListNode.Value.State)
					{
					case ProcessState.Start:
						linkedListNode.Value.Process.OnStart();
						linkedListNode.Value.State = ProcessState.Update;
						linkedListNode.Value.Process.OnUpdate();
						break;
					case ProcessState.Update:
						linkedListNode.Value.Process.OnUpdate();
						break;
					}
					linkedListNode = linkedListNode.Next;
				}
			}
			catch (Exception obj)
			{
				_isException = true;
				_errorHandler(obj);
			}
		}

		public void LateUpdate()
		{
			if (_isException)
			{
				return;
			}
			LinkedListNode<ProcessControle> linkedListNode = _processList.First;
			try
			{
				while (linkedListNode != null)
				{
					LinkedListNode<ProcessControle> next = linkedListNode.Next;
					switch (linkedListNode.Value.State)
					{
					case ProcessState.Update:
						linkedListNode.Value.Process.OnLateUpdate();
						break;
					case ProcessState.Released:
						linkedListNode.Value.Process.OnRelease();
						_processList.Remove(linkedListNode);
						if (_genericManager != null)
						{
							_genericManager.CheckZombiAction(linkedListNode.Value.Process);
						}
						break;
					}
					linkedListNode = next;
				}
			}
			catch (Exception obj)
			{
				_isException = true;
				_errorHandler(obj);
			}
		}

		public uint AddProcess(ProcessBase process, byte priority = 50)
		{
			foreach (ProcessControle process2 in _processList)
			{
				if (process2.Process.ToString() == process.ToString() && process.processType != ProcessType.FadeProcess && process.processType != ProcessType.NetworkProcess && process2.State != ProcessState.Released)
				{
					return 0u;
				}
			}
			ProcessControle processControle = new ProcessControle();
			process.ProcessId = ++_currentProcessId;
			processControle.Process = process;
			processControle.State = ProcessState.Start;
			processControle.Priority = priority;
			if (process is FadeProcess)
			{
				foreach (ProcessControle process3 in _processList)
				{
					if (process3.Process.ProcessId == _fadeProcessId)
					{
						((FadeProcess)process3.Process).ProcessingProcess();
						_fadeProcessId = 0u;
						Interrupt();
						process3.State = ProcessState.Released;
						break;
					}
				}
				_fadeProcessId = process.ProcessId;
			}
			for (LinkedListNode<ProcessControle> linkedListNode = _processList.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (linkedListNode.Value.Priority > processControle.Priority)
				{
					_processList.AddBefore(linkedListNode, processControle);
					process.OnAddProcess();
					return _currentProcessId;
				}
			}
			_processList.AddLast(processControle);
			process.OnAddProcess();
			return _currentProcessId;
		}

		public void ReleaseProcess(ProcessBase process)
		{
			foreach (ProcessControle process2 in _processList)
			{
				if (process2.Process == process)
				{
					if (process2.Process.ProcessId == _fadeProcessId)
					{
						_fadeProcessId = 0u;
					}
					if (process2.Process.GetType() != typeof(FadeProcess))
					{
						Interrupt();
					}
					process2.State = ProcessState.Released;
					break;
				}
			}
		}

		public void OnGotoTestMode()
		{
			OnApplicationQuit();
			_genericManager = null;
		}

		public void OnGotoErrorMode()
		{
			OnApplicationQuit();
			_genericManager = null;
		}

		public void OnApplicationQuit()
		{
			LinkedListNode<ProcessControle> linkedListNode = _processList.First;
			while (linkedListNode != null)
			{
				LinkedListNode<ProcessControle> next = linkedListNode.Next;
				linkedListNode.Value.Process.OnRelease();
				_processList.Remove(linkedListNode);
				linkedListNode = next;
			}
		}

		public void OnApplicationPause(bool isPause)
		{
			LinkedListNode<ProcessControle> linkedListNode = _processList.First;
			while (linkedListNode != null)
			{
				LinkedListNode<ProcessControle> next = linkedListNode.Next;
				linkedListNode.Value.Process.OnApplicationPause(isPause);
				linkedListNode = next;
			}
		}

		public void NotificationFadeIn()
		{
			foreach (ProcessControle process in _processList)
			{
				if (process.Process.ProcessId == _fadeProcessId)
				{
					((FadeProcess)process.Process).StartFadeIn();
				}
			}
		}

		public bool IsNotificationFadeIn()
		{
			foreach (ProcessControle process in _processList)
			{
				if (process.Process.ProcessId == _fadeProcessId)
				{
					return false;
				}
			}
			return true;
		}

		public object SendMessage(Message message)
		{
			object result = null;
			foreach (ProcessControle process in _processList)
			{
				if (message.TargetId != 0)
				{
					if (message.TargetId == process.Process.ProcessId)
					{
						result = process.Process.HandleMessage(message);
					}
				}
				else if (message.TargetType == ProcessType.Broadcast || message.TargetType == process.Process.processType)
				{
					result = process.Process.HandleMessage(message);
				}
			}
			return result;
		}

		public string Dump()
		{
			string text = "ProcessManager Dump\nNum\tPri\tName\t\t\t\t\tState\n";
			int num = 0;
			foreach (ProcessControle process in _processList)
			{
				text = string.Concat(text, num++, " \t", process.Priority, "\t", process.Process, "\t", process.State, "\n");
			}
			return text;
		}
	}
}
