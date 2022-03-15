using Manager;
using UnityEngine;

namespace Process
{
	public abstract class ProcessBase
	{
		public ProcessType processType = ProcessType.General;

		public uint ProcessId;

		protected ProcessDataContainer container;

		private const int MONITOR_NUM = 2;

		private float[] _inputLockTimes;

		private bool[] _isInputLock;

		public ProcessBase(ProcessDataContainer dataContainer)
		{
			container = dataContainer;
			InputInput();
		}

		public ProcessBase(ProcessDataContainer dataContainer, ProcessType type)
		{
			container = dataContainer;
			processType = type;
			InputInput();
		}

		public abstract void OnAddProcess();

		public abstract void OnStart();

		public abstract void OnRelease();

		public virtual void OnUpdate()
		{
			for (int i = 0; i < 2; i++)
			{
				UpdateInputLock(i);
				if (!IsInputLock(i))
				{
					UpdateInput(i);
				}
			}
		}

		private void InputInput()
		{
			_inputLockTimes = new float[2];
			_isInputLock = new bool[2];
			for (int i = 0; i < 2; i++)
			{
				_inputLockTimes[i] = 0f;
				_isInputLock[i] = false;
			}
		}

		protected virtual void UpdateInput(int monitorId)
		{
		}

		protected void UpdateInputLock(int monitorId)
		{
			if (0f < _inputLockTimes[monitorId] - (float)GameManager.GetGameMSecAdd())
			{
				_inputLockTimes[monitorId] -= GameManager.GetGameMSecAdd();
			}
			else
			{
				_isInputLock[monitorId] = false;
			}
		}

		protected bool IsInputLock(int monitorId)
		{
			return _isInputLock[monitorId];
		}

		public void SetInputLockInfo(int monitorId, float time)
		{
			_inputLockTimes[monitorId] = time;
			_isInputLock[monitorId] = true;
		}

		public abstract void OnLateUpdate();

		public virtual object HandleMessage(Message message)
		{
			return null;
		}

		protected GameObject CreateInstanceAndSetParent(GameObject prefs, Transform parent)
		{
			if (prefs != null)
			{
				GameObject gameObject = Object.Instantiate(prefs, parent);
				gameObject.transform.localScale = Vector3.one;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localPosition = Vector3.zero;
				return gameObject;
			}
			return null;
		}

		public virtual void OnApplicationPause(bool isPause)
		{
		}
	}
}
