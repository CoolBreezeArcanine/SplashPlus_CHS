using DB;
using MAI2.Util;
using Manager;
using Monitor;
using UnityEngine;

namespace Process
{
	public class UnlockProcess : ProcessBase
	{
		public enum UnlockSequence
		{
			Init,
			GotoEnd,
			Wait,
			Disp,
			DispEnd,
			Release
		}

		private UnlockSequence _state = UnlockSequence.Wait;

		private UnlockMonitor[] _monitors;

		public UnlockProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public override void OnAddProcess()
		{
		}

		public override void OnStart()
		{
			GameObject prefs = Resources.Load<GameObject>("Process/Unlock/UnlockProcess");
			_monitors = new UnlockMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<UnlockMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<UnlockMonitor>()
			};
			for (int i = 0; i < _monitors.Length; i++)
			{
				_monitors[i].SetAssetManager(container.assetManager);
				_monitors[i].Initialize(i, Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry);
			}
			bool flag = false;
			for (int j = 0; j < _monitors.Length; j++)
			{
				flag |= _monitors[j].IsUnlock();
			}
			if (!flag)
			{
				_state = UnlockSequence.GotoEnd;
			}
			else
			{
				container.processManager.NotificationFadeIn();
			}
		}

		public override void OnUpdate()
		{
			switch (_state)
			{
			case UnlockSequence.GotoEnd:
				_state = UnlockSequence.Release;
				if (GameManager.IsEventMode)
				{
					container.processManager.AddProcess(new NextTrackProcess(container, this), 50);
				}
				else if (GameManager.IsFreedomMapSkip())
				{
					container.processManager.AddProcess(new NextTrackProcess(container, this), 50);
				}
				else if (GameManager.IsCourseMode)
				{
					container.processManager.AddProcess(new NextTrackProcess(container, this), 50);
				}
				else
				{
					container.processManager.AddProcess(new MapResultProcess(container), 50);
				}
				container.processManager.ReleaseProcess(this);
				break;
			case UnlockSequence.Init:
				_state = UnlockSequence.Wait;
				break;
			case UnlockSequence.Wait:
			{
				_state = UnlockSequence.Disp;
				for (int j = 0; j < _monitors.Length; j++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(j).IsEntry && !_monitors[j].IsUnlock())
					{
						container.processManager.EnqueueMessage(j, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
					}
				}
				break;
			}
			case UnlockSequence.Disp:
			{
				if (_monitors[0].IsPlaying() || _monitors[1].IsPlaying())
				{
					break;
				}
				_state = UnlockSequence.DispEnd;
				for (int i = 0; i < _monitors.Length; i++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry)
					{
						container.processManager.CloseWindow(i);
					}
				}
				break;
			}
			case UnlockSequence.DispEnd:
				if (GameManager.IsFreedomMapSkip())
				{
					container.processManager.AddProcess(new NextTrackProcess(container, this), 50);
				}
				else if (GameManager.IsCourseMode)
				{
					container.processManager.AddProcess(new NextTrackProcess(container, this), 50);
				}
				else
				{
					container.processManager.AddProcess(new FadeProcess(container, this, new MapResultProcess(container)), 50);
				}
				_state = UnlockSequence.Release;
				break;
			case UnlockSequence.Release:
				return;
			}
			UnlockMonitor[] monitors = _monitors;
			for (int k = 0; k < monitors.Length; k++)
			{
				monitors[k].ViewUpdate();
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
	}
}
