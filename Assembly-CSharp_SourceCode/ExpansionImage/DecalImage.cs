using UnityEngine;

namespace ExpansionImage
{
	public class DecalImage : ExpansionImage
	{
		[SerializeField]
		private bool _isDecalMask;

		protected override void SetMaterialParamaters()
		{
			base.SetMaterialParamaters();
			if (base.ExpansionSprite != null)
			{
				material.SetFloat("_DecalMaskOn", _isDecalMask ? 1f : 0f);
				material.DisableKeyword("_MODE_MASK");
				material.EnableKeyword("_MODE_DECAL");
				material.DisableKeyword("_MODE_DEBUG");
			}
		}
	}
}
