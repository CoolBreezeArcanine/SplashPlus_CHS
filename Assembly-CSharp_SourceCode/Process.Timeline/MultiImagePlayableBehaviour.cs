using UI;
using UnityEngine.Playables;

namespace Process.Timeline
{
	public class MultiImagePlayableBehaviour : PlayableBehaviour
	{
		public int PlayerIndex;

		public MultipleImage image;

		public int index;

		public override void OnGraphStart(Playable playable)
		{
		}

		public override void OnGraphStop(Playable playable)
		{
		}

		public override void OnBehaviourPlay(Playable playable, FrameData info)
		{
			if (image.MultiSprites.Count > index)
			{
				image.ChangeSprite(index);
			}
		}

		public override void OnBehaviourPause(Playable playable, FrameData info)
		{
		}

		public override void PrepareFrame(Playable playable, FrameData info)
		{
		}
	}
}
