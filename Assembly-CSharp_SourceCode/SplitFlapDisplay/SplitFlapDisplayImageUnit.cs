using UnityEngine;
using UnityEngine.UI;

namespace SplitFlapDisplay
{
	public class SplitFlapDisplayImageUnit : SplitFlapDisplayUnit
	{
		[SerializeField]
		private SpriteDisplayElements _displayElements;

		[SerializeField]
		private Image _nextUpper;

		[SerializeField]
		private Image _currentUpper;

		[SerializeField]
		private Image _nextFooter;

		[SerializeField]
		private Image _currentFooter;

		private Color _originalColor;

		private void Awake()
		{
			_originalColor = _currentUpper.color;
		}

		protected override void SetCurrentElement(int index)
		{
			Sprite sprite = _displayElements?.GetElement(index) ?? null;
			if (sprite != null)
			{
				_currentUpper.color = _originalColor;
				_currentFooter.color = _originalColor;
				_currentUpper.sprite = sprite;
				_currentFooter.sprite = sprite;
			}
			else
			{
				_currentUpper.color = Color.clear;
				_currentFooter.color = Color.clear;
			}
		}

		protected override void SetNextElement(int index)
		{
			Sprite element = _displayElements.GetElement(index);
			if (element != null)
			{
				_nextUpper.color = _originalColor;
				_nextFooter.color = _originalColor;
				_nextUpper.sprite = element;
				_nextFooter.sprite = element;
			}
			else
			{
				_nextUpper.color = Color.clear;
				_nextFooter.color = Color.clear;
			}
		}

		public override int GetElementCount()
		{
			return _displayElements.GetCount();
		}
	}
}
