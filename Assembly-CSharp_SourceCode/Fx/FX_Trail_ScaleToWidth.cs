using UnityEngine;

namespace FX
{
	public class FX_Trail_ScaleToWidth : MonoBehaviour
	{
		private TrailRenderer tRenderer;

		private void Start()
		{
			tRenderer = GetComponent<TrailRenderer>();
		}

		private void Update()
		{
			tRenderer.widthMultiplier = base.transform.lossyScale.x;
			if (base.transform.lossyScale.x == 0f)
			{
				tRenderer.Clear();
			}
		}
	}
}
