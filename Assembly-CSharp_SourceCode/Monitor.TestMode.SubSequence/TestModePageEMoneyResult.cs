using System;
using AMDaemon;
using TMPro;
using UnityEngine;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageEMoneyResult : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI _textDate;

		[SerializeField]
		private TextMeshProUGUI _textResult;

		[SerializeField]
		private TextMeshProUGUI _textDealNumber;

		[SerializeField]
		private TextMeshProUGUI _textCardNumber;

		[SerializeField]
		private TextMeshProUGUI _textBrand;

		[SerializeField]
		private TextMeshProUGUI _textAmount;

		[SerializeField]
		private TextMeshProUGUI _textBalanceBefore;

		[SerializeField]
		private TextMeshProUGUI _textBalanceAfter;

		private readonly string[] dummyBrand = new string[5] { "nanaco", "iD", "SAPICA", "交通系", "PASERI" };

		public void Set(EMoneyResult result)
		{
			_textDate.text = result.Time.ToString("yyyy/MM/dd HH:mm:ss");
			switch (result.Status)
			{
			case EMoneyResultStatus.Fail:
				_textResult.text = "失敗";
				break;
			case EMoneyResultStatus.Success:
				_textResult.text = "成功";
				break;
			case EMoneyResultStatus.Unconfirm:
				_textResult.text = "未了";
				break;
			case EMoneyResultStatus.Incomplete:
				_textResult.text = "受渡中";
				break;
			}
			_textDealNumber.text = result.DealNumber;
			_textCardNumber.text = result.CardNumber;
			_textBrand.text = result.Brand.Name;
			_textAmount.text = result.Amount.ToString();
			_textBalanceBefore.text = result.BalanceBefore.ToString();
			_textBalanceAfter.text = result.BalanceAfter.ToString();
		}

		public void SetDummy(int index)
		{
			_textDate.text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
			switch (index)
			{
			default:
				_textResult.text = "失敗";
				break;
			case 1:
				_textResult.text = "成功";
				break;
			case 2:
				_textResult.text = "未了";
				break;
			case 3:
				_textResult.text = "受渡中";
				break;
			}
			_textDealNumber.text = "0123-000000000" + index;
			_textCardNumber.text = "0123 4567 8901 000" + index;
			_textBrand.text = dummyBrand[index];
			_textAmount.text = (100 * index).ToString();
			_textBalanceBefore.text = (100000 * index).ToString();
			_textBalanceAfter.text = (100000 * index).ToString();
		}
	}
}
