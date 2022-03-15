using Monitor.Error;
using UnityEngine;

namespace Process.Error
{
	public class ErrorProcess : ProcessBase
	{
		private enum ErrorState : byte
		{
			Wait,
			Released
		}

		private ErrorMonitor[] _monitors;

		private ErrorState _state;

		public ErrorProcess(ProcessDataContainer dataContainer)
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
			GameObject prefs = Resources.Load<GameObject>("Process/Error/ErrorProcess");
			_monitors = new ErrorMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<ErrorMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<ErrorMonitor>()
			};
			for (int i = 0; i < _monitors.Length; i++)
			{
				_monitors[i].Initialize(i, active: true);
			}
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (_state != 0)
			{
				_ = 1;
			}
		}

		protected override void UpdateInput(int monitorId)
		{
		}

		public override void OnLateUpdate()
		{
		}
	}
}
