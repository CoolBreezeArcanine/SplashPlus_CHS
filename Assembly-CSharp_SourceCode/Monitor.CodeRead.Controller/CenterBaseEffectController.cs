using Process.CodeRead;
using UnityEngine;

namespace Monitor.CodeRead.Controller
{
	public class CenterBaseEffectController : CodeReadControllerBase
	{
		protected readonly int EffectOut = Animator.StringToHash("Center_Base_Out");

		protected readonly int SilverIn = Animator.StringToHash("Center_Base_Silver_in");

		protected readonly int GoldIn = Animator.StringToHash("Center_Base_Gold_in");

		protected readonly int PlatinumIn = Animator.StringToHash("Center_Base_Platinum_in");

		private FusionType _currentPlayEfecType;

		public override void Play(AnimationType type)
		{
			if (IsAnimationActive() && type == AnimationType.Out && (int)_currentPlayEfecType > 0)
			{
				MainAnimator.Play(Out);
			}
		}

		public void PlayEffect(FusionType type)
		{
			_currentPlayEfecType = type;
			switch (type)
			{
			case FusionType.None:
				MainAnimator.Play(EffectOut);
				break;
			case FusionType.Silver:
				MainAnimator.Play(SilverIn);
				break;
			case FusionType.Gold:
				MainAnimator.Play(GoldIn);
				break;
			case FusionType.Platinum:
				MainAnimator.Play(PlatinumIn);
				break;
			case (FusionType)1:
			case FusionType.Blonz:
				break;
			}
		}
	}
}
