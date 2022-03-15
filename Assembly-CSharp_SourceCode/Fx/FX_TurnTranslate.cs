using UnityEngine;

namespace FX
{
	public class FX_TurnTranslate : MonoBehaviour
	{
		private Vector3 prePos;

		private Vector3 newPos;

		private void OnEnable()
		{
			prePos = base.transform.position;
		}

		private void Update()
		{
			Transform transform = base.transform;
			Vector3 position = transform.position;
			newPos = position - prePos;
			if (0.0001f < newPos.sqrMagnitude)
			{
				transform.rotation = Quaternion.LookRotation(newPos);
			}
			prePos = position;
		}
	}
}
