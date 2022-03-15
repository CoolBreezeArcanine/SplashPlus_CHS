using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Sxc.Unity.App
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Image))]
	public class GradationMap : UIBehaviour, IMaterialModifier
	{
		public enum BlendControl
		{
			Normal,
			Add
		}

		private const string ShaderName = "WR/GradationMap";

		private static readonly int GradationTextureId;

		private const int TextureW = 256;

		private const int TextureH = 1;

		[SerializeField]
		private bool _isEnable = true;

		[SerializeField]
		private Gradient _gradient = new Gradient();

		[SerializeField]
		private Image _image;

		[SerializeField]
		private BlendControl _blendControl;

		private Texture2D _gradationTexture;

		private Graphic _graphic;

		public Graphic graphic => _graphic ?? (_graphic = GetComponent<Graphic>());

		static GradationMap()
		{
			GradationTextureId = Shader.PropertyToID("_GradationMap");
		}

		protected override void Start()
		{
			base.Start();
			_image = GetComponent<Image>();
			_image.material = new Material(Shader.Find("WR/GradationMap"));
			GetModifiedMaterial(_image.material);
			_gradationTexture = new Texture2D(256, 1)
			{
				wrapMode = TextureWrapMode.Clamp,
				filterMode = FilterMode.Point
			};
			ApplyGradient();
		}

		public void ApplyGradient()
		{
			Texture2D gradationTexture = _gradationTexture;
			for (int i = 0; i < 256; i++)
			{
				Color color = _gradient.Evaluate((float)i / 255f);
				gradationTexture.SetPixel(i, 0, color);
			}
			gradationTexture.Apply();
			_gradationTexture = gradationTexture;
			_image.material.SetTexture(GradationTextureId, _gradationTexture);
			_image.material.SetFloat(UIShaderPropertyID.On, _isEnable ? 1f : 0f);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			Object.DestroyImmediate(_gradationTexture);
			_image.material = null;
		}

		public Material GetModifiedMaterial(Material baseMaterial)
		{
			BlendControl blendControl = _blendControl;
			BlendMode value;
			BlendMode value2;
			if (blendControl != BlendControl.Add)
			{
				value = BlendMode.SrcAlpha;
				value2 = BlendMode.OneMinusSrcAlpha;
			}
			else
			{
				value = BlendMode.SrcAlpha;
				value2 = BlendMode.One;
			}
			baseMaterial.SetInt(UIShaderPropertyID.SrcBlend, (int)value);
			baseMaterial.SetInt(UIShaderPropertyID.DstBlend, (int)value2);
			baseMaterial.SetFloat(UIShaderPropertyID.On, _isEnable ? 1f : 0f);
			return baseMaterial;
		}

		protected override void OnDidApplyAnimationProperties()
		{
			ApplyGradient();
			base.OnDidApplyAnimationProperties();
		}
	}
}
