using UnityEngine;

namespace FX
{
	[RequireComponent(typeof(Camera))]
	public class FX_CameraCtrl : MonoBehaviour
	{
		[SerializeField]
		[Range(0.1f, 10f)]
		private float wheelSpeed = 1f;

		[SerializeField]
		[Range(0.1f, 10f)]
		private float moveSpeed = 0.3f;

		[SerializeField]
		[Range(0.1f, 10f)]
		private float rotateSpeed = 0.3f;

		private Vector3 preMousePos;

		private void Update()
		{
			MouseUpdate();
		}

		private void MouseUpdate()
		{
			float axis = Input.GetAxis("Mouse ScrollWheel");
			if (axis != 0f)
			{
				MouseWheel(axis);
			}
			if (DebugInput.GetMouseButtonDown(0) || DebugInput.GetMouseButtonDown(1) || DebugInput.GetMouseButtonDown(2))
			{
				preMousePos = Input.mousePosition;
			}
			MouseDrag(Input.mousePosition);
		}

		private void MouseWheel(float delta)
		{
			base.transform.position += base.transform.forward * delta * wheelSpeed;
		}

		private void MouseDrag(Vector3 mousePos)
		{
			Vector3 vector = mousePos - preMousePos;
			if (!(vector.magnitude < 1E-05f))
			{
				if (DebugInput.GetMouseButton(2))
				{
					base.transform.Translate(-vector * Time.deltaTime * moveSpeed);
				}
				else if (DebugInput.GetMouseButton(1))
				{
					CameraRotate(new Vector2(0f - vector.y, vector.x) * rotateSpeed);
				}
				preMousePos = mousePos;
			}
		}

		public void CameraRotate(Vector2 angle)
		{
			base.transform.RotateAround(base.transform.position, base.transform.right, angle.x);
			base.transform.RotateAround(base.transform.position, Vector3.up, angle.y);
		}
	}
}
