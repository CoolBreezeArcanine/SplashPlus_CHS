using UnityEngine;

namespace FX
{
	public class FX_PositionGoal_Ease : MonoBehaviour
	{
		[SerializeField]
		private Vector3 startPosition;

		[SerializeField]
		private Vector3 middlePosition;

		[SerializeField]
		private Vector3 goalPosition;

		[SerializeField]
		private float middleRadius;

		[SerializeField]
		[Range(0f, 1f)]
		private float middleRadiusThickness = 1f;

		private float randomRot;

		private float randomDst;

		[SerializeField]
		private float keyTimeMagnification = 1f;

		private float duration;

		public AnimationCurve _curve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0f, 0f));

		private void Start()
		{
			randomRot = Random.Range(0f, 1f);
			randomDst = Random.Range(0f, 1f);
			middlePosition += Quaternion.Euler(0f, 0f, 360f * randomRot) * (Vector3.up * middleRadius - Vector3.up * middleRadius * randomDst * middleRadiusThickness);
			float z = base.transform.localEulerAngles.z;
			startPosition = Quaternion.Euler(0f, 0f, z) * startPosition;
			goalPosition = Quaternion.Euler(0f, 0f, z) * goalPosition;
			middlePosition = Quaternion.Euler(0f, 0f, z) * middlePosition;
			base.transform.localPosition = startPosition;
		}

		private void Update()
		{
			float num = _curve.Evaluate(duration);
			if (num < 0.5f)
			{
				base.transform.localPosition = GetCatmullRomPosition(num * 2f, startPosition, startPosition, middlePosition, goalPosition);
			}
			else
			{
				base.transform.localPosition = GetCatmullRomPosition((num - 0.5f) * 2f, startPosition, middlePosition, goalPosition, goalPosition);
			}
			duration += Time.deltaTime / keyTimeMagnification;
		}

		private Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
		{
			Vector3 vector = 2f * p1;
			Vector3 vector2 = p2 - p0;
			Vector3 vector3 = 2f * p0 - 5f * p1 + 4f * p2 - p3;
			Vector3 vector4 = -p0 + 3f * p1 - 3f * p2 + p3;
			return 0.5f * (vector + vector2 * t + vector3 * t * t + vector4 * t * t * t);
		}
	}
}
