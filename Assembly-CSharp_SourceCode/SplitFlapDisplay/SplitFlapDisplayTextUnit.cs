using TMPro;
using UnityEngine;

namespace SplitFlapDisplay
{
	public class SplitFlapDisplayTextUnit : SplitFlapDisplayUnit
	{
		[SerializeField]
		private StringDisplayElements _displayElements;

		[SerializeField]
		private TextMeshProUGUI _nextUpper;

		[SerializeField]
		private TextMeshProUGUI _currentUpper;

		[SerializeField]
		private TextMeshProUGUI _nextFotter;

		[SerializeField]
		private TextMeshProUGUI _currenFotter;

		private void Start()
		{
			Initialize(0);
		}

		protected override void SetCurrentElement(int index)
		{
			string element = _displayElements.GetElement(index);
			_currentUpper.text = element;
			_currenFotter.text = element;
		}

		protected override void SetNextElement(int index)
		{
			string element = _displayElements.GetElement(index);
			_nextUpper.text = element;
			_nextFotter.text = element;
		}

		public override int GetElementCount()
		{
			return _displayElements.GetCount();
		}
	}
}
