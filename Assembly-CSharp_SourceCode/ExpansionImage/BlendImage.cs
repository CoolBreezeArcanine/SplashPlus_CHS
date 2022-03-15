using UnityEngine;
using UnityEngine.UI;

namespace ExpansionImage
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Image))]
	[ExecuteAlways]
	public class BlendImage : ExternalImage
	{
		[SerializeField]
		private MultiImageBlendType _blendType;

		[SerializeField]
		private Color _addColor = Color.black;

		public Color AddColor
		{
			get
			{
				return _addColor;
			}
			set
			{
				if (SetPropertyUtility.SetColor(ref _addColor, value))
				{
					_image.SetVerticesDirty();
				}
			}
		}

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
			switch (_blendType)
			{
			case MultiImageBlendType.Normal:
				_image.material.SetInt("_SrcBlend", 5);
				_image.material.SetInt("_DstBlend", 10);
				_image.material.SetInt("_BlendOp", 0);
				break;
			case MultiImageBlendType.Add:
				_image.material.SetInt("_SrcBlend", 5);
				_image.material.SetInt("_DstBlend", 1);
				_image.material.SetInt("_BlendOp", 0);
				break;
			case MultiImageBlendType.Subtract:
				_image.material.SetInt("_SrcBlend", 5);
				_image.material.SetInt("_DstBlend", 1);
				_image.material.SetInt("_BlendOp", 2);
				break;
			}
			_image.material.SetFloat("_AddOn", 1f);
			_image.material.SetColor("_AddColor", _addColor);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (GetComponents<ExternalImage>().Length > 1 || GetComponent<ExpansionImage>() != null)
			{
				if (_image != null && _image.material != null)
				{
					_image.material.SetInt("_SrcBlend", 5);
					_image.material.SetInt("_DstBlend", 10);
					_image.material.SetInt("_BlendOp", 0);
					_image.material.SetFloat("_AddOn", 0f);
					_image.material.SetColor("_AddColor", Color.black);
				}
			}
			else
			{
				_image.material = null;
			}
		}
	}
}
