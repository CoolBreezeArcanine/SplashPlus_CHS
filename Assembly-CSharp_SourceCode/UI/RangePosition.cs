using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
	public class RangePosition : UIBehaviour
	{
		[SerializeField]
		private Vector3 _position01;

		[SerializeField]
		private Vector3 _position02;

		[SerializeField]
		[Range(0f, 1f)]
		private float _progress;

		private RectTransform _rectTransform;

		private RectTransform RectTransform => _rectTransform ?? (_rectTransform = GetComponent<RectTransform>());

		public float Progress => _progress;

		private void UpdatePosition()
		{
			RectTransform.anchoredPosition = Vector3.Lerp(_position01, _position02, _progress);
		}
	}
}
