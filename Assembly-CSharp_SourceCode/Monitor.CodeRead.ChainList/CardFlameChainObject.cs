using Monitor.CodeRead.Controller;
using Process.CodeRead;
using UI;
using UI.DaisyChainList;
using UnityEngine;

namespace Monitor.CodeRead.ChainList
{
	public class CardFlameChainObject : ChainObject
	{
		protected readonly int Decision = Animator.StringToHash("Card_Decision");

		protected readonly int Cancel = Animator.StringToHash("Card_Cancel");

		protected readonly int MainIn = Animator.StringToHash("Card_Main_In");

		protected readonly int MainOut = Animator.StringToHash("Card_Sub_in");

		[SerializeField]
		private RectTransform _cardSet;

		[SerializeField]
		private ReadCodeController _originalCode;

		[SerializeField]
		private MultipleImage[] _playerInfomationImages;

		[SerializeField]
		[Header("決定アイコンアニメーター")]
		private Animator _decisionAnimator;

		[SerializeField]
		[Header("効果ダウン演出")]
		private RectTransform _parentEffectDown;

		[SerializeField]
		private EffectDownController _originalEffectDown;

		private ReadCodeController _readCodeController;

		private EffectDownController _effectDown;

		public void Initialize()
		{
			_readCodeController = Object.Instantiate(_originalCode, _cardSet).GetComponent<ReadCodeController>();
			_effectDown = Object.Instantiate(_originalEffectDown, _parentEffectDown);
			_effectDown.gameObject.SetActive(value: false);
		}

		public void SetData(int playerIndex, CodeReadProcess.CardStatus status, Sprite background, Sprite character, Sprite frame)
		{
			MultipleImage[] playerInfomationImages = _playerInfomationImages;
			for (int i = 0; i < playerInfomationImages.Length; i++)
			{
				playerInfomationImages[i].ChangeSprite(playerIndex);
			}
			_readCodeController.SetData(background, character, frame);
			if ((int)status > 0)
			{
				_effectDown.gameObject.SetActive(value: true);
				_effectDown.SetData(status);
			}
			else
			{
				_effectDown.gameObject.SetActive(value: false);
			}
		}

		public void SetDecision(bool isDecision)
		{
			_decisionAnimator.Play(isDecision ? Decision : Cancel);
		}

		public override void OnCenterIn()
		{
			Animator.Play(MainIn);
		}

		public override void OnCenterOut()
		{
			Animator.Play(MainOut);
		}

		public override void SetChainActive(bool isActive)
		{
			if (isActive)
			{
				Animator.enabled = true;
				Animator.Rebind();
				Animator.Play(Animator.StringToHash("Idle"));
			}
			else
			{
				Animator.enabled = false;
			}
			base.SetChainActive(isActive);
		}
	}
}
