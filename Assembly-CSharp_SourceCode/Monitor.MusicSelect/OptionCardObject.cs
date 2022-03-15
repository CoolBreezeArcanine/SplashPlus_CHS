using Manager;
using TMPro;
using UI.DaisyChainList;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.MusicSelect
{
	public class OptionCardObject : ChainObject
	{
		[SerializeField]
		[Header("Advanced")]
		private Animator _cardAnimator;

		[SerializeField]
		private Image _optionValueImage;

		[SerializeField]
		private RectTransform _optionPreview;

		[SerializeField]
		private TextMeshProUGUI _titleText;

		[SerializeField]
		private TextMeshProUGUI _detailsText;

		[SerializeField]
		private TextMeshProUGUI _valueText;

		[SerializeField]
		private TextMeshProUGUI _valueDetailsText;

		[SerializeField]
		[Header("タッチボタン")]
		private GameObject _rightButtonObject;

		[SerializeField]
		private GameObject _leftButtonObject;

		[SerializeField]
		private Animator _rightButtonAnimator;

		[SerializeField]
		private Animator _leftButtonAnimator;

		[SerializeField]
		private CanvasGroup _leftGroup;

		[SerializeField]
		private CanvasGroup _rightGroup;

		private NoteSpeedPreviewObject _previewObject;

		private float _holdUpTime;

		private float _lastPressedTime;

		private bool _isHold;

		private bool[] _isButtonActive = new bool[2] { true, true };

		private float _buttonTimer;

		public void Initialize(float holdUpTIme)
		{
			_holdUpTime = holdUpTIme;
			_isButtonActive = new bool[2] { true, true };
			SetTrigger(Direction.Left, "Loop");
			SetTrigger(Direction.Right, "Loop");
			_rightGroup.alpha = 0f;
			_leftGroup.alpha = 0f;
		}

		public override void OnCenterIn()
		{
			_cardAnimator.SetTrigger("OnCenterIn");
			SetTrigger(Direction.Left, "In");
			SetTrigger(Direction.Right, "In");
		}

		public override void OnCenterOut()
		{
			_cardAnimator.SetTrigger("OnCenterOut");
		}

		public override void ViewUpdate(float syncTimer)
		{
			if (_buttonTimer >= 1000f)
			{
				_buttonTimer = 0f;
			}
			else
			{
				_buttonTimer += GameManager.GetGameMSecAdd();
			}
			_rightButtonAnimator.SetFloat("SyncTimer", _buttonTimer / 1000f);
			_leftButtonAnimator.SetFloat("SyncTimer", _buttonTimer / 1000f);
			if (_isHold && (float)GameManager.GetGameMSec() - _lastPressedTime > _holdUpTime)
			{
				_isHold = false;
				SetTrigger(Direction.Left, "Loop");
				SetTrigger(Direction.Right, "Loop");
			}
			base.ViewUpdate(syncTimer);
		}

		public void PressedButton(Direction direction, bool isLongTouch, bool toOut)
		{
			_lastPressedTime = GameManager.GetGameMSec();
			string key = (isLongTouch ? "Hold" : "Pressed");
			_isHold = isLongTouch;
			if (direction == Direction.Right)
			{
				_rightButtonAnimator.SetBool("ToOut", toOut);
			}
			else
			{
				_leftButtonAnimator.SetBool("ToOut", toOut);
			}
			if (_isButtonActive[(direction != Direction.Right) ? 1u : 0u])
			{
				SetTrigger(direction, key);
				if (toOut)
				{
					_isButtonActive[(direction != Direction.Right) ? 1u : 0u] = false;
				}
			}
		}

		public void SetVisibleButton(bool isVisible, Direction direction)
		{
			if (direction == Direction.Right)
			{
				_rightGroup.alpha = (isVisible ? 1 : 0);
			}
			else
			{
				_leftGroup.alpha = (isVisible ? 1 : 0);
			}
			SetTrigger(direction, isVisible ? "In" : "Out");
			_isButtonActive[(direction != Direction.Right) ? 1u : 0u] = isVisible;
		}

		public override void ResetChain()
		{
			_cardAnimator.Play("Reset");
		}

		private void SetTrigger(Direction direction, string key)
		{
			if (direction == Direction.Right)
			{
				_rightButtonAnimator.SetTrigger(key);
			}
			else
			{
				_leftButtonAnimator.SetTrigger(key);
			}
		}

		public void SetData(string title, string value, string details, string valueDetails, Sprite optionSprite, bool isLeftButtonActive, bool isRightButtonActive)
		{
			_titleText.text = title;
			_detailsText.text = details;
			SetValue(value, valueDetails, optionSprite);
			if (isLeftButtonActive)
			{
				SetTrigger(Direction.Left, "In");
				_leftGroup.alpha = 1f;
			}
			if (isRightButtonActive)
			{
				SetTrigger(Direction.Right, "In");
				_rightGroup.alpha = 1f;
			}
		}

		public void SetValue(string value, string details, Sprite optionSprite)
		{
			_valueText.text = value;
			_optionValueImage.sprite = optionSprite;
			_optionValueImage.SetNativeSize();
		}

		public void SetPreviewPrefab(GameObject obj)
		{
			obj.transform.SetParent(_optionPreview);
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localScale = Vector3.one;
		}
	}
}
