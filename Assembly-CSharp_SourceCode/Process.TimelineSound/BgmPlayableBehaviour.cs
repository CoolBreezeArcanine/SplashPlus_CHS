using System;
using Mai2.Mai2Cue;
using Manager;
using UnityEngine.Playables;

namespace Process.TimelineSound
{
	public class BgmPlayableBehaviour : PlayableBehaviour
	{
		public int PlayerIndex;

		public string CueCode;

		public override void OnGraphStart(Playable playable)
		{
		}

		public override void OnGraphStop(Playable playable)
		{
		}

		public override void OnBehaviourPlay(Playable playable, FrameData info)
		{
			if (!string.IsNullOrEmpty(CueCode))
			{
				SoundManager.PlayBGM((Cue)Enum.Parse(typeof(Cue), CueCode), PlayerIndex);
			}
		}

		public override void OnBehaviourPause(Playable playable, FrameData info)
		{
			if (!string.IsNullOrEmpty(CueCode))
			{
				SoundManager.StopBGM(PlayerIndex);
			}
		}

		public override void PrepareFrame(Playable playable, FrameData info)
		{
		}
	}
}
