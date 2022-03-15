using System;
using Mai2.Mai2Cue;
using Manager;
using UnityEngine.Playables;

namespace Process.TimelineSound
{
	public class SeLoopPlayableBehaviour : SePlayableBehaviour
	{
		private bool _isPlay;

		public override void OnGraphStart(Playable playable)
		{
		}

		public override void OnGraphStop(Playable playable)
		{
			SoundManager.StopLoopSe(PlayerIndex);
		}

		public override void OnBehaviourPlay(Playable playable, FrameData info)
		{
			if (!string.IsNullOrEmpty(CueCode))
			{
				try
				{
					_isPlay = true;
					SoundManager.PlayLoopSE((Cue)Enum.Parse(typeof(Cue), CueCode), PlayerIndex);
				}
				catch
				{
				}
			}
		}

		public override void OnBehaviourPause(Playable playable, FrameData info)
		{
			if (_isPlay)
			{
				_isPlay = false;
				SoundManager.StopLoopSe(PlayerIndex);
			}
		}

		public override void PrepareFrame(Playable playable, FrameData info)
		{
			if (_isPlay)
			{
				double num = (double)(float)playable.GetDuration() - 0.1;
				if ((double)(float)playable.GetTime() >= num)
				{
					_isPlay = false;
					SoundManager.StopLoopSe(PlayerIndex);
				}
			}
		}
	}
}
