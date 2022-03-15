using UnityEngine;

namespace FX
{
	public class FX_CopyMat_UVAdd : MonoBehaviour
	{
		[SerializeField]
		private string TextureName = "_MainTex";

		[SerializeField]
		private float addSpeedX;

		[SerializeField]
		private float addSpeedY;

		private Material _material;

		private float _x;

		private float _y;

		private void Start()
		{
			Renderer component = GetComponent<Renderer>();
			_material = ((component != null) ? component.material : null);
			if (_material != null)
			{
				_material.SetTextureOffset(TextureName, Vector2.zero);
			}
		}

		private void Update()
		{
			if (_material != null)
			{
				_x = Mathf.Repeat(_x + 60f * Time.deltaTime * addSpeedX, 1f);
				_y = Mathf.Repeat(_y + 60f * Time.deltaTime * addSpeedY, 1f);
				Vector2 value = new Vector2(_x, _y);
				_material.SetTextureOffset(TextureName, value);
			}
		}
	}
}
