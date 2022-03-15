using UnityEngine;

namespace FX
{
	public class FX_TurnCamera_high : MonoBehaviour
	{
		[SerializeField]
		private Camera targetCamera;

		public Vector3 localRot;

		public Vector3 worldRot;

		private void Start()
		{
			if (targetCamera == null)
			{
				targetCamera = Camera.main;
			}
		}

		private void Update()
		{
			base.transform.Rotate(localRot);
			base.transform.Rotate(worldRot, Space.World);
			base.transform.localRotation = new Quaternion(0f, 60f, 0f, 60f);
		}
	}
}
