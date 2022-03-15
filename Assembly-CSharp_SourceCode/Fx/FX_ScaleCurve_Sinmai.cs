using UnityEngine;

namespace FX
{
	public class FX_ScaleCurve_Sinmai : MonoBehaviour
	{
		[SerializeField]
		private float keyValueMagnification = 1f;

		[SerializeField]
		private float keyTimeMagnification = 1f;

		private float duration;

		[SerializeField]
		private bool resetTime;

		private bool toggleResetTime;

		public AnimationCurve Curve = new AnimationCurve(new Keyframe(0f, 0f, 3f, 3f), new Keyframe(1f, 1f));

		private void OnEnable()
		{
			duration = 0f;
		}

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
			float num = keyValueMagnification * Curve.Evaluate(duration);
			duration += Time.deltaTime / keyTimeMagnification;
			base.transform.localScale = new Vector3(num, num, num);
		}
	}
}
