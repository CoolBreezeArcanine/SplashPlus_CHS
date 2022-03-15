using UnityEngine;

namespace UI.Common
{
	public class SelectorBackgroundController : MonoBehaviour
	{
		public enum AnimationType
		{
			In,
			InSelect,
			Out,
			OutSelect,
			OutBack,
			Disabled
		}

		private const string In = "In";

		private const string InSelect = "In_Select";

		private const string Out = "Out";

		private const string OutSelect = "Out_Select";

		private const string OutBack = "Out_Back";

		private const string Disabled = "Disabled";

		[SerializeField]
		[Header("背景色関連")]
		private ImageColorGroup _backgroundColorGroup;

		[SerializeField]
		private CustomTextScroll _scrollText;

		[SerializeField]
		private CustomIndicator _indicator;

		private Animator _animator;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
		}

		public void Play(AnimationType type)
		{
			string text = "";
			switch (type)
			{
			default:
				return;
			case AnimationType.In:
				text = "In";
				break;
			case AnimationType.InSelect:
				text = "In_Select";
				break;
			case AnimationType.Out:
				text = "Out";
				break;
			case AnimationType.OutSelect:
				text = "Out_Select";
				break;
			case AnimationType.OutBack:
				text = "Out_Back";
				break;
			case AnimationType.Disabled:
				text = "Disabled";
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				_animator.Play(Animator.StringToHash(text));
			}
		}

		public void SetBackgroundColor(int index)
		{
			_backgroundColorGroup.SetColor(index);
		}

		public void SetBackgroundColor(Color color)
		{
			_backgroundColorGroup.SetColor(color);
		}

		public void SetScrollMessage(string message)
		{
			if (_scrollText != null)
			{
				_scrollText.SetData(message);
				_scrollText.ResetPosition();
			}
		}

		public void PrepareIndicator(int collectionIndex, int collectionNum)
		{
			if (_indicator != null)
			{
				_indicator.Preapre(collectionIndex + 1, collectionNum);
			}
		}

		public void UpdateIndicator(int index)
		{
			if (_indicator != null && _indicator.gameObject.activeSelf)
			{
				_indicator.ViewUpdate(index, index + 1);
			}
		}

		public void SetVisibleIndicator(bool isActive)
		{
			if (_indicator != null)
			{
				_indicator.SetVisible(isActive);
			}
		}

		public void SetActiveIndicator(bool isActive)
		{
			if (_indicator != null)
			{
				_indicator.gameObject.SetActive(isActive);
			}
		}

		public void UpdateScroll()
		{
			if (_scrollText != null)
			{
				_scrollText.ViewUpdate();
			}
		}
	}
}
