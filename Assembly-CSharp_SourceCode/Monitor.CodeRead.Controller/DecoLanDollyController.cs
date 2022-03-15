using System.Collections;
using Process.CodeRead;
using UnityEngine;

namespace Monitor.CodeRead.Controller
{
	public class DecoLanDollyController : CodeReadControllerBase
	{
		protected const string LanDecision = "Deco_Lan_Decision";

		protected const string DollyDecision = "Deco_Dolly_Decision";

		public override void Initialize(int playerIndex)
		{
			base.Initialize(playerIndex);
			for (int i = 1; i < 7; i++)
			{
				MainAnimator.SetLayerWeight(i, 0f);
			}
		}

		public override void Play(AnimationType type)
		{
			if (IsAnimationActive())
			{
				switch (type)
				{
				case AnimationType.In:
					PlayIntroduction();
					break;
				case AnimationType.Out:
					MainAnimator.Play(Out);
					break;
				}
			}
		}

		public void SetDecision(int targetPlayer)
		{
			string text = "";
			switch (targetPlayer)
			{
			default:
				return;
			case 0:
				text = "Deco_Lan_Decision";
				break;
			case 1:
				text = "Deco_Dolly_Decision";
				break;
			}
			if (IsAnimationActive())
			{
				int layerIndex = MainAnimator.GetLayerIndex(text);
				MainAnimator.SetLayerWeight(layerIndex, 1f);
			}
		}

		private void PlayIntroduction()
		{
			StartCoroutine(IntroductionCoroutine());
		}

		private IEnumerator IntroductionCoroutine()
		{
			MainAnimator.Play(Animator.StringToHash("In"));
			yield return new WaitForEndOfFrame();
			MainAnimator.Play(Animator.StringToHash("Idle"));
			yield return new WaitForEndOfFrame();
			MainAnimator.SetLayerWeight(1, 1f);
			MainAnimator.SetLayerWeight(2, 1f);
		}
	}
}
