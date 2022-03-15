using AMDaemon;
using TMPro;
using UnityEngine;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageEMoneyReport : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI _textDate;

		[SerializeField]
		private TextMeshProUGUI _textResult;

		[SerializeField]
		private TextMeshProUGUI _textCount;

		[SerializeField]
		private TextMeshProUGUI _textAmount;

		[SerializeField]
		private TextMeshProUGUI _textAlarmCount;

		[SerializeField]
		private TextMeshProUGUI _textAlarmAmount;

		public void Set(EMoneyReport report)
		{
			_textDate.text = report.Time.ToString("yyyy/MM/dd HH:mm:ss");
			_textResult.text = (report.IsSucceeded ? "成功" : "失敗");
			_textCount.text = report.Count.ToString();
			_textAmount.text = report.Amount.ToString();
			_textAlarmCount.text = report.AlarmCount.ToString();
			_textAlarmAmount.text = report.AlarmAmount.ToString();
		}
	}
}
