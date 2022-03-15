using UnityEngine;

namespace FX
{
	[RequireComponent(typeof(TrailRenderer))]
	public class FX_TrailRenderer_SetEnable : MonoBehaviour
	{
		[SerializeField]
		private float _delayFrame;

		private float count;

		private bool generated;

		private void Update()
		{
			if (count >= _delayFrame && !generated)
			{
				TrailRenderer component = base.gameObject.GetComponent<TrailRenderer>();
				component.enabled = true;
				component.Clear();
				generated = true;
			}
			else
			{
				count += Time.deltaTime * 60f;
			}
		}
	}
}
