using TMPro;
using UnityEngine;

namespace Monitor.Result
{
	public class ScoreBoardColumnObject : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI _numberText;

		[SerializeField]
		private GameObject _cloasBoxObject;

		[SerializeField]
		private bool _isUseComma;

		public void SetScore(uint score)
		{
			_numberText.text = (_isUseComma ? $"{score:#,0}" : $"{score}");
		}

		public void SetVisibleCloseBox(bool isVisible)
		{
			_cloasBoxObject.SetActive(isVisible);
		}
	}
}
