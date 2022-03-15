using UnityEngine;

namespace FX
{
	[RequireComponent(typeof(TrailRenderer))]
	public class FX_TrailRenderer_Clear : MonoBehaviour
	{
		[SerializeField]
		private float _delayFrame;

		private float count;

		private bool generated;

		private void Update()
		{
			if (count >= _delayFrame && !generated)
			{
				base.gameObject.GetComponent<TrailRenderer>().Clear();
				generated = true;
			}
			else
			{
				count += Time.deltaTime * 60f;
			}
		}
	}
}
