using System.Collections;
using Manager;
using TMPro;
using UI.DaisyChainList;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.PhotoEdit
{
	public class EditCardObject : ChainObject
	{
		[SerializeField]
		[Header("Advanced")]
		private Animator _cardAnimator;

		[SerializeField]
		[Header("タイトル画像")]
		private Image _titleImage;

		[SerializeField]
		[Header("タッチボタン")]
		private Animator _rightButtonAnimator;

		[SerializeField]
		private Animator _leftButtonAnimator;

		[SerializeField]
		private CanvasGroup _leftGroup;

		[SerializeField]
		private CanvasGroup _rightGroup;

		[SerializeField]
		[Header("設定値")]
		private TextMeshProUGUI _leftValueText;

		[SerializeField]
		private TextMeshProUGUI _rightValueText;

		private float _holdUpTime = 250f;

		private float _lastPressedTime;

		private float _buttonTimer;

		private bool _isHold;

		private readonly bool[] _isButtonActive = new bool[2] { true, true };

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

		public override void OnCenterIn()
		{
			_cardAnimator.Play(Animator.StringToHash("CenterActive"), 0, 0f);
		}

		public override void OnCenterOut()
		{
			_cardAnimator.Play(Animator.StringToHash("CenterDeactive"), 0, 0f);
		}

		public void SetData(Sprite titleSprite, string valueText, bool isLeftActive, bool isRightActive)
		{
			_titleImage.sprite = titleSprite;
			_leftValueText.text = valueText;
			_rightValueText.text = valueText;
			SetTrigger(Direction.Left, isLeftActive ? "In" : "Out");
			SetTrigger(Direction.Right, isRightActive ? "In" : "Out");
		}

		public void PressedButton(Direction direction, bool isLongTouch, bool toOut)
		{
			_lastPressedTime = GameManager.GetGameMSec();
			string key = (isLongTouch ? "Hold" : "Pressed");
			_isHold = isLongTouch;
			_rightButtonAnimator.SetBool("ToOut", value: false);
			_leftButtonAnimator.SetBool("ToOut", value: false);
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

		public override void ResetChain()
		{
			_cardAnimator.Play("Reset");
		}

		public void SetValue(Direction direction, string switchText)
		{
			if (direction == Direction.Right)
			{
				_rightValueText.text = switchText;
				_cardAnimator.Play(Animator.StringToHash("RightChange"), 0, 0f);
			}
			else
			{
				_leftValueText.text = switchText;
				_cardAnimator.Play(Animator.StringToHash("LeftChange"), 0, 0f);
			}
			StartCoroutine(CardAnimation(switchText));
		}

		private IEnumerator CardAnimation(string text)
		{
			yield return new WaitForSeconds(_cardAnimator.GetCurrentAnimatorStateInfo(0).length);
			_rightValueText.text = text;
			_leftValueText.text = text;
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
	}
}
