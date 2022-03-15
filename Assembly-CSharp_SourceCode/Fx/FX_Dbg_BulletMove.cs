using UnityEngine;

namespace FX
{
	public class FX_Dbg_BulletMove : MonoBehaviour
	{
		public float amount = 1f;

		public float speed = 2f;

		private float countingTime;

		private float leftOrRight = -1f;

		public AnimationCurve CurveX = new AnimationCurve(new Keyframe(0f, 0f, 0f, 7f), new Keyframe(0.5f, 1.5f), new Keyframe(1f, 0f, -7f, 0f));

		public AnimationCurve CurveY = new AnimationCurve(new Keyframe(0f, 0f, 0f, 15f), new Keyframe(0.5f, 3.5f), new Keyframe(1f, 0f, -15f, 0f));

		public AnimationCurve CurveZ = new AnimationCurve(new Keyframe(0f, 0f, 0f, 25f), new Keyframe(1f, 25f, 25f, 0f));

		private void Start()
		{
			if (Random.value <= 0.5f)
			{
				leftOrRight = 1f;
			}
			if (Random.value <= 0.5f)
			{
				leftOrRight *= 3f;
			}
		}

		private void Update()
		{
			float x = amount * CurveX.Evaluate(countingTime) * leftOrRight;
			float y = amount * CurveY.Evaluate(countingTime);
			float z = amount * CurveZ.Evaluate(countingTime);
			countingTime += Time.deltaTime * speed;
			base.transform.localPosition = new Vector3(x, y, z);
		}
	}
}
