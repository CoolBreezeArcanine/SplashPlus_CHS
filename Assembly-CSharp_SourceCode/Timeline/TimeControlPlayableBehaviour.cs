using UnityEngine;
using UnityEngine.Playables;

namespace Timeline
{
	public class TimeControlPlayableBehaviour : PlayableBehaviour
	{
		[SerializeField]
		private TimeControlBaseObject _target;

		private bool _isPlaying;

		public TimeControlBaseObject Target
		{
			get
			{
				return _target;
			}
			set
			{
				_target = value;
			}
		}

		public override void OnGraphStart(Playable playable)
		{
			if (!(_target == null))
			{
				_target.OnGraphStart();
			}
		}

		public override void OnGraphStop(Playable playable)
		{
			if (!(_target == null))
			{
				_target.OnGraphStop();
			}
		}

		public override void OnBehaviourPlay(Playable playable, FrameData info)
		{
			if (!(_target == null))
			{
				_target.OnBehaviourPlay();
			}
		}

		public override void OnBehaviourPause(Playable playable, FrameData info)
		{
			if (_target == null)
			{
				return;
			}
			_target.OnBehaviourPause();
			if (_isPlaying)
			{
				_isPlaying = false;
				if (playable.GetTime() > playable.GetDuration() / 2.0)
				{
					_target.OnClipTailEnd();
				}
				else
				{
					_target.OnClipHeadEnd();
				}
			}
		}

		public override void PrepareFrame(Playable playable, FrameData info)
		{
			if (!(_target == null))
			{
				if (!_isPlaying)
				{
					_isPlaying = true;
					_target.OnClipPlay();
				}
				_target.PrepareFrame(playable.GetTime() / playable.GetDuration());
			}
		}

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
		}

		public override void PrepareData(Playable playable, FrameData info)
		{
		}
	}
}
