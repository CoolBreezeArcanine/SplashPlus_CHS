using UnityEngine;
using UnityEngine.Playables;

namespace Timeline
{
	public class AnimationTriggerBehaviour : PlayableBehaviour
	{
		[SerializeField]
		private Animator _targetAnimator;

		[SerializeField]
		private string _trigger;

		public string Trigger
		{
			get
			{
				return _trigger;
			}
			set
			{
				_trigger = value;
			}
		}

		public Animator Target
		{
			set
			{
				_targetAnimator = value;
			}
		}

		public override void OnBehaviourPlay(Playable playable, FrameData info)
		{
			if (!(_targetAnimator == null))
			{
				_targetAnimator.enabled = true;
				_targetAnimator.Play(_trigger);
			}
		}

		public override void OnGraphStart(Playable playable)
		{
			if (!(_targetAnimator == null))
			{
				_targetAnimator.enabled = false;
			}
		}
	}
}
