using Mai2.Mai2Cue;
using Mai2.Voice_000001;
using Manager;
using Process.CodeRead;
using UnityEngine;

namespace Monitor.CodeRead.Controller
{
	public abstract class CodeReadControllerBase : MonoBehaviour
	{
		protected Animator MainAnimator;

		protected int PlayerIndex = -1;

		protected readonly int In = Animator.StringToHash("In");

		protected readonly int Out = Animator.StringToHash("Out");

		public virtual void Initialize(int playerIndex)
		{
			PlayerIndex = playerIndex;
			MainAnimator = GetComponent<Animator>();
		}

		public abstract void Play(AnimationType type);

		protected bool IsAnimationActive()
		{
			if (base.gameObject.activeSelf)
			{
				return base.gameObject.activeInHierarchy;
			}
			return false;
		}

		public virtual void PlaySE(Mai2.Mai2Cue.Cue cue)
		{
			SoundManager.PlaySE(cue, PlayerIndex);
		}

		public virtual void PlayVoice(Mai2.Voice_000001.Cue cue)
		{
			SoundManager.PlayVoice(cue, PlayerIndex);
		}
	}
}
