using UnityEngine;

namespace FX
{
	public class FX_GenerateSingle : MonoBehaviour
	{
		public GameObject GenerateObject;

		private void Awake()
		{
			Object.Instantiate(GenerateObject).transform.SetParent(base.transform, worldPositionStays: false);
		}
	}
}
