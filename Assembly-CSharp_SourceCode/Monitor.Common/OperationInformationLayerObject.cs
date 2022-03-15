using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.Common
{
	public class OperationInformationLayerObject : MonoBehaviour
	{
		[SerializeField]
		private Image _base;

		[SerializeField]
		private TextMeshProUGUI _text;

		[SerializeField]
		private GameObject _button;

		private Animator _animator;

		private CanvasGroup _canvasGroup;

		private bool _delaySetting;

		private Sprite _delaySprite;

		private string _delayMessage;

		private bool _delayShowButton;

		public void Initialize()
		{
			_animator = GetComponent<Animator>();
			_canvasGroup = GetComponent<CanvasGroup>();
			_animator.PlayInFixedTime("Out", -1, float.MaxValue);
		}

		public void ViewUpdate()
		{
			if (_delaySetting && _canvasGroup.alpha < float.Epsilon)
			{
				_base.sprite = _delaySprite;
				_text.text = _delayMessage;
				_button.SetActive(_delayShowButton);
				_delaySetting = false;
			}
		}

		public void SetParameter(bool isDelay, Sprite sprite, string message, bool showButton)
		{
			if (isDelay)
			{
				_delaySetting = true;
				_delaySprite = sprite;
				_delayMessage = message;
				_delayShowButton = showButton;
			}
			else
			{
				_base.sprite = sprite;
				_text.text = message;
				_button.SetActive(showButton);
			}
		}

		public void Play(string stateName)
		{
			_animator.Play(stateName);
		}

		public bool IsPlaying()
		{
			return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f;
		}
	}
}
