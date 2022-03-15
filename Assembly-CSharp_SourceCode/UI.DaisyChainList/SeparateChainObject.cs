using TMPro;
using UnityEngine;

namespace UI.DaisyChainList
{
	public class SeparateChainObject : SeparateBaseChainObject
	{
		[SerializeField]
		private GameObject _leftObject;

		[SerializeField]
		private GameObject _rightObject;

		[SerializeField]
		private TextMeshProUGUI _leftTitle;

		[SerializeField]
		private TextMeshProUGUI _rightTitle;

		[SerializeField]
		private TextMeshProUGUI _left;

		[SerializeField]
		private TextMeshProUGUI _right;

		public override void SetData(string left, string right)
		{
			_left.text = left;
			_right.text = right;
		}

		public override void SetTitle(string left, string right)
		{
			_leftTitle.text = left;
			_rightTitle.text = right;
		}

		public override void SetVisible(bool isVisible, Direction direction)
		{
			switch (direction)
			{
			case Direction.Left:
				if (_leftObject.activeSelf != isVisible)
				{
					_leftObject.SetActive(isVisible);
				}
				break;
			case Direction.Right:
				if (_rightObject.activeSelf != isVisible)
				{
					_rightObject.SetActive(isVisible);
				}
				break;
			}
		}

		public override void SetScrollDirection(Direction direction)
		{
		}

		public override void ScrollUpdate(float progress)
		{
			if (progress <= 0.1f)
			{
				SetVisible(isVisible: false, Direction.Right);
				SetVisible(isVisible: false, Direction.Left);
			}
			if (progress >= 0.5f)
			{
				SetVisible(isVisible: true, Direction.Right);
				SetVisible(isVisible: true, Direction.Left);
			}
			base.ScrollUpdate(progress);
		}
	}
}
