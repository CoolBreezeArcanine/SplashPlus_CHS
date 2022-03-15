using System;

namespace Timeline
{
	public class EventTriggerControlObject : TimeControlBaseObject
	{
		public Action TriggerAction;

		public override void OnClipPlay()
		{
			Trigger();
		}

		private void Trigger()
		{
			TriggerAction?.Invoke();
		}
	}
}
