using UnityEngine;
using UnityEngine.UI;

namespace ExpansionImage
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Image))]
	[ExecuteAlways]
	public class FillImage : ExternalImage
	{
		[SerializeField]
		private Color _fillColor = Color.white;

		protected override void SetMaterialParamaters()
		{
			if (_image == null)
			{
				_image = GetComponent<Image>();
			}
			if (_image.material == null || _image.material.shader.name != "MAI2/UI/MultiImage")
			{
				Shader shader = Shader.Find("MAI2/UI/MultiImage");
				_image.material = new Material(shader);
			}
			_image.material.SetColor("_FillColor", _fillColor);
			_image.material.SetFloat("_FillOn", 1f);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (GetComponents<ExternalImage>().Length > 1 || GetComponent<ExpansionImage>() != null)
			{
				if (_image != null && _image.material != null)
				{
					_image.material.SetFloat("_FillOn", 0f);
					_image.material.SetColor("_FillColor", Color.clear);
				}
			}
			else
			{
				_image.material = null;
			}
		}
	}
}
