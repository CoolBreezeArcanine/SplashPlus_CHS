using UnityEngine;
using UnityEngine.Playables;

namespace Timeline.CustomTrack
{
	public class CanvasGroupAlphaMixerBehaviour : PlayableBehaviour
	{
		private CanvasGroup _trackBinding;

		private float _initialValue;

		public override void OnGraphStop(Playable playable)
		{
			if (_trackBinding != null)
			{
				_trackBinding.alpha = _initialValue;
				_trackBinding = null;
			}
		}

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			CanvasGroup canvasGroup = playerData as CanvasGroup;
			if (canvasGroup == null)
			{
				return;
			}
			if (_trackBinding == null)
			{
				_trackBinding = canvasGroup;
				_initialValue = _trackBinding.alpha;
				return;
			}
			int inputCount = playable.GetInputCount();
			float num = 0f;
			for (int i = 0; i < inputCount; i++)
			{
				float inputWeight = playable.GetInputWeight(i);
				CanvasGroupAlphaBehaviour behaviour = ((ScriptPlayable<CanvasGroupAlphaBehaviour>)playable.GetInput(i)).GetBehaviour();
				num += behaviour.Alpha * inputWeight;
			}
			_trackBinding.alpha = num;
		}
	}
}
