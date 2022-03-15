using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class BlurPlate : MonoBehaviour
	{
		private Image _blurImage;

		private Material _blurMaterial;

		public const string ShaderValueName = "_Rate";

		public void SetBlurRate(float progress)
		{
			_blurMaterial.SetFloat("_Rate", progress);
		}

		private void Awake()
		{
			_blurImage = GetComponent<Image>();
			_blurMaterial = _blurImage.GetComponent<Material>();
		}
	}
}
