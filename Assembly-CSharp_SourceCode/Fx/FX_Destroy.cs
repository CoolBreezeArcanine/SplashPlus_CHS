using UnityEngine;

namespace FX
{
	public class FX_Destroy : MonoBehaviour
	{
		public float EndTime = 2f;

		private void Start()
		{
			Object.Destroy(base.gameObject, EndTime);
		}
	}
}
