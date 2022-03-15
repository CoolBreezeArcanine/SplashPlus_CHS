using Process.CodeRead;
using UnityEngine;

namespace Monitor.CodeRead.Controller
{
	public class CenterBaseCircleController : CodeReadControllerBase
	{
		protected const string LayerSilver = "Silver";

		protected const string LayerGold = "Gold";

		protected const string LayerPlatinum = "Platinum";

		protected readonly int CircleLoop = Animator.StringToHash("Circle_Loop");

		protected readonly int CircleOut = Animator.StringToHash("Circle_Out");

		protected readonly int SilverIn = Animator.StringToHash("Circle_Silver_in");

		protected readonly int GoldIn = Animator.StringToHash("Circle_Gold_in");

		protected readonly int PlatinumIn = Animator.StringToHash("Circle_Platinum_in");

		[SerializeField]
		private GameObject _silverObject;

		[SerializeField]
		private GameObject _goldObject;

		[SerializeField]
		private GameObject _platinumObject;

		public override void Play(AnimationType type)
		{
			if (IsAnimationActive())
			{
				switch (type)
				{
				case AnimationType.In:
					MainAnimator.Play(In);
					break;
				case AnimationType.Loop:
					MainAnimator.Play(CircleLoop);
					break;
				case AnimationType.Out:
					MainAnimator.Play(Out);
					break;
				}
			}
		}

		public void SetData(FusionType type)
		{
			switch (type)
			{
			case FusionType.None:
				_silverObject.SetActive(value: false);
				_goldObject.SetActive(value: false);
				_platinumObject.SetActive(value: false);
				break;
			case FusionType.Silver:
				_silverObject.SetActive(value: true);
				_goldObject.SetActive(value: false);
				_platinumObject.SetActive(value: false);
				break;
			case FusionType.Gold:
				_silverObject.SetActive(value: false);
				_goldObject.SetActive(value: true);
				_platinumObject.SetActive(value: false);
				break;
			case FusionType.Platinum:
				_silverObject.SetActive(value: false);
				_goldObject.SetActive(value: false);
				_platinumObject.SetActive(value: true);
				break;
			case (FusionType)1:
			case FusionType.Blonz:
				break;
			}
		}

		public void PlayCircle(FusionType type)
		{
			SetData(type);
			if (IsAnimationActive())
			{
				switch (type)
				{
				case FusionType.None:
					MainAnimator.SetLayerWeight(MainAnimator.GetLayerIndex("Silver"), 0f);
					MainAnimator.SetLayerWeight(MainAnimator.GetLayerIndex("Gold"), 0f);
					MainAnimator.SetLayerWeight(MainAnimator.GetLayerIndex("Platinum"), 0f);
					MainAnimator.Play(CircleOut);
					break;
				case FusionType.Silver:
					MainAnimator.SetLayerWeight(MainAnimator.GetLayerIndex("Silver"), 1f);
					MainAnimator.SetLayerWeight(MainAnimator.GetLayerIndex("Gold"), 0f);
					MainAnimator.SetLayerWeight(MainAnimator.GetLayerIndex("Platinum"), 0f);
					MainAnimator.Play(SilverIn);
					break;
				case FusionType.Gold:
					MainAnimator.SetLayerWeight(MainAnimator.GetLayerIndex("Silver"), 0f);
					MainAnimator.SetLayerWeight(MainAnimator.GetLayerIndex("Gold"), 1f);
					MainAnimator.SetLayerWeight(MainAnimator.GetLayerIndex("Platinum"), 0f);
					MainAnimator.Play(GoldIn);
					break;
				case FusionType.Platinum:
					MainAnimator.SetLayerWeight(MainAnimator.GetLayerIndex("Silver"), 0f);
					MainAnimator.SetLayerWeight(MainAnimator.GetLayerIndex("Gold"), 0f);
					MainAnimator.SetLayerWeight(MainAnimator.GetLayerIndex("Platinum"), 1f);
					MainAnimator.Play(PlatinumIn);
					break;
				case (FusionType)1:
				case FusionType.Blonz:
					break;
				}
			}
		}
	}
}
