using Monitor.CoursePrepare;
using UnityEngine;

namespace Process.CoursePrepare
{
	public class CoursePrepareProcess : ProcessBase
	{
		private enum ProcessState : byte
		{
			Wait,
			Released
		}

		private CoursePrepareMonitor[] _monitors;

		private ProcessState _state;

		public CoursePrepareProcess(ProcessDataContainer dataContainer)
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
			GameObject prefs = Resources.Load<GameObject>("Process/CoursePrepare/CoursePrepareProcess");
			_monitors = new CoursePrepareMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<CoursePrepareMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<CoursePrepareMonitor>()
			};
			for (int i = 0; i < _monitors.Length; i++)
			{
				_monitors[i].Initialize(i, active: true);
			}
			container.processManager.NotificationFadeIn();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (_state != 0)
			{
				_ = 1;
				return;
			}
			_state = ProcessState.Released;
			container.processManager.AddProcess(new FadeProcess(container, this, new AdvertiseProcess(container)), 50);
			container.processManager.SetVisibleTimers(isVisible: false);
		}

		protected override void UpdateInput(int monitorId)
		{
		}

		public override void OnLateUpdate()
		{
		}
	}
}
