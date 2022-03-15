using Monitor.Template;
using UnityEngine;

namespace Process.Template
{
	public class TemplateProcess : ProcessBase
	{
		private enum TemplateState : byte
		{
			Wait,
			Released
		}

		private TemplateMonitor[] _monitors;

		private TemplateState _state;

		public TemplateProcess(ProcessDataContainer dataContainer)
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
			GameObject prefs = Resources.Load<GameObject>("Process/Template/TemplateProcess");
			_monitors = new TemplateMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<TemplateMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<TemplateMonitor>()
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
			_state = TemplateState.Released;
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
