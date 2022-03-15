using UnityEngine;

namespace FX
{
	public class FX_ScaleCurve : MonoBehaviour
	{
		[SerializeField]
		private float keyValueMagnification = 1f;

		[SerializeField]
		private float keyTimeMagnification = 1f;

		private float duration;

		[SerializeField]
		private bool resetTime;

		private bool toggleResetTime;

		public AnimationCurve CurveX = new AnimationCurve(new Keyframe(0f, 0f, 3f, 3f), new Keyframe(1f, 1f));

		public AnimationCurve CurveY = new AnimationCurve(new Keyframe(0f, 0f, 3f, 3f), new Keyframe(1f, 1f));

		public AnimationCurve CurveZ = new AnimationCurve(new Keyframe(0f, 0f, 3f, 3f), new Keyframe(1f, 1f));

		private void Update()
		{
			if (!(resetTime & toggleResetTime))
			{
				if (resetTime & !toggleResetTime)
				{
					duration = 0f;
					toggleResetTime = true;
				}
				else
				{
					toggleResetTime = false;
				}
			}
			float x = keyValueMagnification * CurveX.Evaluate(duration);
			float y = keyValueMagnification * CurveY.Evaluate(duration);
			float z = keyValueMagnification * CurveZ.Evaluate(duration);
			duration += Time.deltaTime / keyTimeMagnification;
			base.transform.localScale = new Vector3(x, y, z);
		}
	}
}
