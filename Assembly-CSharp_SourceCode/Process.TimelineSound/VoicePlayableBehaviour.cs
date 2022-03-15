using System;
using Mai2.Voice_000001;
using Mai2.Voice_Partner_000001;
using Manager;
using UnityEngine.Playables;

namespace Process.TimelineSound
{
	public class VoicePlayableBehaviour : PlayableBehaviour
	{
		public int PlayerIndex;

		public string CueCode;

		public bool IsPartnerVoice;

		public override void OnGraphStart(Playable playable)
		{
		}

		public override void OnGraphStop(Playable playable)
		{
		}

		public override void OnBehaviourPlay(Playable playable, FrameData info)
		{
			if (string.IsNullOrEmpty(CueCode))
			{
				return;
			}
			try
			{
				if (IsPartnerVoice)
				{
					SoundManager.PlayPartnerVoice((Mai2.Voice_Partner_000001.Cue)Enum.Parse(typeof(Mai2.Voice_Partner_000001.Cue), CueCode), PlayerIndex);
				}
				else
				{
					SoundManager.PlayVoice((Mai2.Voice_000001.Cue)Enum.Parse(typeof(Mai2.Voice_000001.Cue), CueCode), PlayerIndex);
				}
			}
			catch
			{
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
