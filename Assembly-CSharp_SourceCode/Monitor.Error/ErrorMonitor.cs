using AMDaemon;
using DB;
using TMPro;
using UnityEngine;

namespace Monitor.Error
{
	public class ErrorMonitor : MonitorBase
	{
		[SerializeField]
		private TextMeshProUGUI ErrorID;

		[SerializeField]
		private TextMeshProUGUI ErrorMessage;

		[SerializeField]
		private TextMeshProUGUI ErrorDate;

		[SerializeField]
		private TextMeshProUGUI ErrorIDTitle;

		[SerializeField]
		private TextMeshProUGUI ErrorMessageTitle;

		[SerializeField]
		private TextMeshProUGUI ErrorDateTitle;

		private void Awake()
		{
			ErrorIDTitle.text = CommonMessageID.ErrorIDTitle.GetName();
			ErrorMessageTitle.text = CommonMessageID.ErrorMessageTitle.GetName();
			ErrorDateTitle.text = CommonMessageID.ErrorDateTitle.GetName();
		}

		public override void Initialize(int monIndex, bool active)
		{
			base.Initialize(monIndex, active);
			ErrorID.text = AMDaemon.Error.Number.ToString().PadLeft(4, '0');
			ErrorMessage.text = AMDaemon.Error.Message;
			ErrorDate.text = AMDaemon.Error.Time.ToString();
		}

		public override void ViewUpdate()
		{
		}
	}
}
