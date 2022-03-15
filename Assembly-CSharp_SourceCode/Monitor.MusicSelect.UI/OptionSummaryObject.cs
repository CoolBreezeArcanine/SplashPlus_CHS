using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Monitor.MusicSelect.UI
{
	public class OptionSummaryObject : MonoBehaviour
	{
		[SerializeField]
		private List<TextMeshProUGUI> _titleList;

		[SerializeField]
		private List<TextMeshProUGUI> _valueList;

		public void SetOptionData(int index, string title, string value)
		{
			_titleList[index].text = title;
			_valueList[index].text = value;
		}
	}
}
