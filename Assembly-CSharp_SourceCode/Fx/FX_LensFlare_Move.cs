using UnityEngine;

namespace FX
{
	public class FX_LensFlare_Move : MonoBehaviour
	{
		[SerializeField]
		private Transform _targetPosition;

		private Transform _centerPosition;

		private Vector3 OriginalScale;

		private void Start()
		{
			if (_centerPosition == null)
			{
				_centerPosition = base.transform;
			}
			OriginalScale = new Vector3(1f / base.transform.lossyScale.x, 1f / base.transform.lossyScale.y, 1f / base.transform.lossyScale.z);
		}

		private void Update()
		{
			float x = _targetPosition.position.x - _centerPosition.position.x;
			float num = Mathf.Atan2(_targetPosition.position.y - _centerPosition.position.y, x) * 57.29578f;
			base.transform.localEulerAngles = new Vector3(0f, 0f, num - 90f);
			float num2 = Vector3.Distance(_centerPosition.position, _targetPosition.position);
			base.transform.localScale = new Vector3(num2 * OriginalScale.x, num2 * OriginalScale.y, num2 * OriginalScale.z);
		}
	}
}
