using UnityEngine;

namespace FX
{
	public class FX_GenerateSingle_Delay : MonoBehaviour
	{
		public GameObject GenerateObject;

		public float _delayFrame;

		private float count;

		private bool generated;

		private void Update()
		{
			if (count >= _delayFrame && !generated)
			{
				Object.Instantiate(GenerateObject).transform.SetParent(base.transform, worldPositionStays: false);
				generated = true;
			}
			else
			{
				count += Time.deltaTime * 60f;
			}
		}
	}
}
