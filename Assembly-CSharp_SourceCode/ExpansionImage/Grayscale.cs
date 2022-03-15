using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ExpansionImage
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Image))]
	public class Grayscale : UIBehaviour, IMaterialModifier
	{
		private const string ShaderName = "Custom/UI/Grayscale";

		private Image _image;

		protected override void OnEnable()
		{
			base.OnEnable();
			_image.material.EnableKeyword("CUSTOM_GRAYSCALE");
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			_image.material.DisableKeyword("CUSTOM_GRAYSCALE");
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			_image.material = null;
		}

		public Material GetModifiedMaterial(Material baseMaterial)
		{
			return baseMaterial;
		}
	}
}
