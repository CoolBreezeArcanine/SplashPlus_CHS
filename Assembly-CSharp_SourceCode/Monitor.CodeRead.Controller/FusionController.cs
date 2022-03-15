using Process.CodeRead;
using UnityEngine;

namespace Monitor.CodeRead.Controller
{
	public class FusionController : CodeReadControllerBase
	{
		protected readonly int Normal = Animator.StringToHash("Fusion_Normal");

		protected readonly int Platinum = Animator.StringToHash("Fusion_Platinum");

		protected readonly int DerakkumaFusion = Animator.StringToHash("Fusion_01");

		protected const string LayerSilver = "Color_Silver";

		protected const string LayerGold = "Color_Gold";

		protected const string LayerPlatinum = "Color_Platinum";

		[SerializeField]
		[Header("でらっくま")]
		private RectTransform _derakkuma;

		[SerializeField]
		private GameObject _originalDerakkuma;

		[SerializeField]
		[Header("カード")]
		private RectTransform _cardSet;

		[SerializeField]
		private ReadCodeController _originalCodeSet;

		private Animator _derakkumaAnimator;

		private ReadCodeController _readCodeController;

		public override void Initialize(int playerIndex)
		{
			base.Initialize(playerIndex);
			_derakkumaAnimator = Object.Instantiate(_originalDerakkuma, _derakkuma).GetComponent<Animator>();
			_readCodeController = Object.Instantiate(_originalCodeSet, _cardSet);
		}

		public void SetData(CodeReadProcess.ReadCard card)
		{
			_readCodeController.SetData(card);
		}

		public override void Play(AnimationType type)
		{
		}

		public void PlayFusion(FusionType type)
		{
			MainAnimator.Rebind();
			switch (type)
			{
			default:
				return;
			case FusionType.Silver:
				MainAnimator.SetLayerWeight(MainAnimator.GetLayerIndex("Color_Silver"), 1f);
				MainAnimator.SetLayerWeight(MainAnimator.GetLayerIndex("Color_Gold"), 0f);
				MainAnimator.SetLayerWeight(MainAnimator.GetLayerIndex("Color_Platinum"), 0f);
				MainAnimator.Play(Normal);
				break;
			case FusionType.Gold:
				MainAnimator.SetLayerWeight(MainAnimator.GetLayerIndex("Color_Silver"), 0f);
				MainAnimator.SetLayerWeight(MainAnimator.GetLayerIndex("Color_Gold"), 1f);
				MainAnimator.SetLayerWeight(MainAnimator.GetLayerIndex("Color_Platinum"), 0f);
				MainAnimator.Play(Normal);
				break;
			case FusionType.Platinum:
				MainAnimator.SetLayerWeight(MainAnimator.GetLayerIndex("Color_Silver"), 0f);
				MainAnimator.SetLayerWeight(MainAnimator.GetLayerIndex("Color_Gold"), 0f);
				MainAnimator.SetLayerWeight(MainAnimator.GetLayerIndex("Color_Platinum"), 1f);
				MainAnimator.Play(Platinum);
				break;
			}
			_derakkumaAnimator.SetLayerWeight(1, 0f);
			_derakkumaAnimator.SetLayerWeight(2, 0f);
			_derakkumaAnimator.SetLayerWeight(3, 0f);
			_derakkumaAnimator.Play(DerakkumaFusion);
		}
	}
}
