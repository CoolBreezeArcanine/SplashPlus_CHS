using TMPro;
using UnityEngine;

namespace Monitor.Result
{
	public class ScoreRecordObject : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI _perfect;

		[SerializeField]
		private TextMeshProUGUI _critical;

		[SerializeField]
		private TextMeshProUGUI _great;

		[SerializeField]
		private TextMeshProUGUI _good;

		[SerializeField]
		private TextMeshProUGUI _miss;

		public void SetScore(uint perfect, uint critical, uint great, uint good, uint miss)
		{
			_perfect.text = perfect.ToString();
			_critical.text = "(" + critical + ")";
			_great.text = great.ToString();
			_good.text = good.ToString();
			_miss.text = miss.ToString();
		}
	}
}
