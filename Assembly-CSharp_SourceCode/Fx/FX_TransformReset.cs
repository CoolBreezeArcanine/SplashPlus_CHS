using UnityEngine;

namespace FX
{
	public class FX_TransformReset : MonoBehaviour
	{
		[SerializeField]
		private bool _resetLocalPosition = true;

		[SerializeField]
		private bool _resetLocalRotation = true;

		[SerializeField]
		private bool _resetLocalScale = true;

		private void Start()
		{
			if (_resetLocalPosition)
			{
				base.transform.localPosition = Vector3.zero;
			}
			if (_resetLocalRotation)
			{
				base.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
			}
			if (_resetLocalScale)
			{
				base.transform.localScale = Vector3.one;
			}
		}
	}
}
