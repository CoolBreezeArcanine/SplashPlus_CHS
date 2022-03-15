using UnityEngine;

namespace FX
{
	public class FX_GenerateGameObjects : MonoBehaviour
	{
		public GameObject[] GenerateObjects;

		private void Start()
		{
			for (int i = 0; i < GenerateObjects.Length; i++)
			{
				Object.Instantiate(GenerateObjects[i]).transform.SetParent(base.transform, worldPositionStays: false);
			}
		}
	}
}
