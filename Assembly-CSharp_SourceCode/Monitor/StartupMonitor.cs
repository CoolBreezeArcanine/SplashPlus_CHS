using TMPro;
using UnityEngine;

namespace Monitor
{
	public class StartupMonitor : MonitorBase
	{
		[SerializeField]
		private TextMeshProUGUI _mainMessage;

		[SerializeField]
		private TextMeshProUGUI _subMessage;

		public override void Initialize(int monIndex, bool active)
		{
			base.Initialize(monIndex, active);
		}

		public override void ViewUpdate()
		{
		}

		public void SetMainMessage(string message, string submessage)
		{
			_mainMessage.text = message;
			_subMessage.text = submessage;
		}
	}
}
