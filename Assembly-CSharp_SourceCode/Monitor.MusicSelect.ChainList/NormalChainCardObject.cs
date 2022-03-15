using TMPro;
using UI.DaisyChainList;
using UnityEngine;

namespace Monitor.MusicSelect.ChainList
{
	public class NormalChainCardObject : ChainObject
	{
		[SerializeField]
		private TextMeshProUGUI titleText;

		[SerializeField]
		private TextMeshProUGUI valueText;

		[SerializeField]
		private TextMeshProUGUI detailText;

		public void SetData(string title, string value, string detail)
		{
			titleText.text = title;
			valueText.text = value;
			detailText.text = detail;
		}

		public void SetValue(string value)
		{
			valueText.text = value;
		}
	}
}
