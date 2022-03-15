using Process.CodeRead;
using UI.DaisyChainList;
using UnityEngine;

namespace Monitor.CodeRead.Controller
{
	public class PartnerStatusController : CodeReadControllerBase
	{
		protected readonly int BlonzIn = Animator.StringToHash("Partner_Status_Blonz_in");

		protected readonly int SilverIn = Animator.StringToHash("Partner_Status_Silver_in");

		protected readonly int GoldIn = Animator.StringToHash("Partner_Status_Gold_in");

		[SerializeField]
		private GameObject _leftObject;

		[SerializeField]
		private GameObject _rightObject;

		[SerializeField]
		[Header("カードスロットターゲット")]
		private RectTransform _leftCardSlot;

		[SerializeField]
		private RectTransform _rightCardSlot;

		[SerializeField]
		[Header("カードスロットオリジナルオブジェクト")]
		private ReadCodeController _originalCode;

		[SerializeField]
		[Header("効果ダウンターゲット")]
		private RectTransform _parentLeftEffectDown;

		[SerializeField]
		[Header("効果ダウンターゲット")]
		private RectTransform _parentRightEffectDown;

		[SerializeField]
		[Header("効果ダウンオリジナル")]
		private EffectDownController _originalEffectDown;

		private EffectDownController _leftEffectDown;

		private EffectDownController _rightEffectDown;

		private ReadCodeController _leftCode;

		private ReadCodeController _rightCode;

		public void Initialize(int playerIndex, Direction direction)
		{
			Initialize(playerIndex);
			switch (direction)
			{
			case Direction.Left:
				_rightObject.SetActive(value: false);
				_leftCode = Object.Instantiate(_originalCode, _leftCardSlot);
				_leftCode.gameObject.SetActive(value: false);
				_leftEffectDown = Object.Instantiate(_originalEffectDown, _parentLeftEffectDown);
				_leftEffectDown.gameObject.SetActive(value: false);
				break;
			case Direction.Right:
				_leftObject.SetActive(value: false);
				_rightCode = Object.Instantiate(_originalCode, _rightCardSlot);
				_rightCode.gameObject.SetActive(value: false);
				_rightEffectDown = Object.Instantiate(_originalEffectDown, _parentRightEffectDown);
				_rightEffectDown.gameObject.SetActive(value: false);
				break;
			default:
				_rightObject.SetActive(value: false);
				_leftObject.SetActive(value: false);
				break;
			}
		}

		public void SetData(CodeReadProcess.ReadCard card, bool isDownEffect, CodeReadProcess.CardStatus status)
		{
			_leftCode?.gameObject.SetActive(value: true);
			_leftCode?.SetData(card);
			_rightCode?.gameObject.SetActive(value: true);
			_rightCode?.SetData(card);
			_leftEffectDown?.gameObject.SetActive(isDownEffect);
			_rightEffectDown?.gameObject.SetActive(isDownEffect);
			if (isDownEffect)
			{
				_leftEffectDown?.SetData(status);
				_rightEffectDown?.SetData(status);
			}
		}

		public override void Play(AnimationType type)
		{
			if (IsAnimationActive())
			{
				switch (type)
				{
				case AnimationType.In:
					MainAnimator.Play(In);
					break;
				case AnimationType.Out:
					MainAnimator.Play(Out);
					break;
				}
			}
		}

		public void PlayTypeIn(FusionType type)
		{
			if (IsAnimationActive())
			{
				switch (type)
				{
				case FusionType.Blonz:
					MainAnimator.Play(BlonzIn);
					break;
				case FusionType.Silver:
					MainAnimator.Play(SilverIn);
					break;
				case FusionType.Gold:
					MainAnimator.Play(GoldIn);
					break;
				default:
					MainAnimator.Play(In);
					break;
				}
			}
		}
	}
}
