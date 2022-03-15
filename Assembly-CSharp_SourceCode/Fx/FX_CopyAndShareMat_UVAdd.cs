using UnityEngine;

namespace FX
{
	public class FX_CopyAndShareMat_UVAdd : MonoBehaviour
	{
		[SerializeField]
		private string TextureName = "_MainTex";

		[SerializeField]
		private float addSpeedX;

		[SerializeField]
		private float addSpeedY;

		[SerializeField]
		private Renderer[] _renderers;

		private Material _sharedMaterial;

		private float _x;

		private float _y;

		private void Start()
		{
			Renderer[] renderers = _renderers;
			foreach (Renderer renderer in renderers)
			{
				if (renderer != null && renderer.material != null)
				{
					if (_sharedMaterial == null)
					{
						_sharedMaterial = new Material(renderer.material);
					}
					renderer.material = _sharedMaterial;
				}
			}
			if (_sharedMaterial != null)
			{
				_sharedMaterial.SetTextureOffset(TextureName, Vector2.zero);
			}
		}

		private void Update()
		{
			if (_sharedMaterial != null)
			{
				_x = Mathf.Repeat(_x + 60f * Time.deltaTime * addSpeedX, 1f);
				_y = Mathf.Repeat(_y + 60f * Time.deltaTime * addSpeedY, 1f);
				Vector2 value = new Vector2(_x, _y);
				_sharedMaterial.SetTextureOffset(TextureName, value);
			}
		}
	}
}
