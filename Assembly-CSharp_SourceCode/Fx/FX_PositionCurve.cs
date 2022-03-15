using UnityEngine;

namespace FX
{
	public class FX_PositionCurve : MonoBehaviour
	{
		[SerializeField]
		private float keyValueMagnification = 1f;

		[SerializeField]
		private float keyTimeMagnification = 1f;

		private float duration;

		private Vector3 preState = new Vector3(0f, 0f, 0f);

		private Vector3 animState;

		public AnimationCurve CurveX = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0f, 0f));

		public AnimationCurve CurveY = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0f, 0f));

		public AnimationCurve CurveZ = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0f, 0f));

		private void Update()
		{
			float x = keyValueMagnification * CurveX.Evaluate(duration);
			float y = keyValueMagnification * CurveY.Evaluate(duration);
			float z = keyValueMagnification * CurveZ.Evaluate(duration);
			animState = new Vector3(x, y, z);
			base.transform.Translate(animState - preState);
			preState = animState;
			duration += Time.deltaTime / keyTimeMagnification;
		}
	}
}
