using TMPro;
using UnityEngine;

namespace Monitor
{
	public class PowerOnMonitor : MonitorBase
	{
		[SerializeField]
		private TextMeshProUGUI MainMessage;

		[SerializeField]
		private TextMeshProUGUI SubMessage;

		public override void ViewUpdate()
		{
		}

		public void SetMainMessage(string mmessage, string smessage)
		{
			MainMessage.text = mmessage;
			SubMessage.text = smessage;
		}
	}
}
