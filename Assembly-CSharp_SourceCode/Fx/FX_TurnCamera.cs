using UnityEngine;

namespace FX
{
	public class FX_TurnCamera : MonoBehaviour
	{
		[SerializeField]
		private Camera targetCamera;

		private void Start()
		{
			if (targetCamera == null)
			{
				targetCamera = Camera.main;
			}
		}

		private void Update()
		{
			base.transform.LookAt(targetCamera.transform.position);
		}
	}
}
