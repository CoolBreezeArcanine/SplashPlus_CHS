using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CustomImage : Image
{
	public int num;

	private int alphaValue;

	public override Material GetModifiedMaterial(Material baseMaterial)
	{
		Material material = baseMaterial;
		if (m_ShouldRecalculateStencil)
		{
			alphaValue = (base.maskable ? CustomMaskUtilities.GetStencilDepth<CustomAlphaMask>(base.transform, MaskUtilities.FindRootSortOverrideCanvas(base.transform)) : 0);
			m_StencilValue = (base.maskable ? MaskUtilities.GetStencilDepth(base.transform, MaskUtilities.FindRootSortOverrideCanvas(base.transform)) : 0);
			m_ShouldRecalculateStencil = false;
		}
		if ((m_StencilValue > 0 || alphaValue > 0) && baseMaterial.HasProperty("_MaskTex"))
		{
			CustomAlphaMask componentInParent = GetComponentInParent<CustomAlphaMask>();
			Texture2D maskTexture = (Texture2D)componentInParent.GetGraphic.mainTexture;
			Vector4 tilingOffset = new Vector4(componentInParent.Tiling.x, componentInParent.Tiling.y, componentInParent.Offset.x, componentInParent.Offset.y);
			CustomAlphaMask[] maskParentComponents = CustomMaskUtilities.GetMaskParentComponents<CustomAlphaMask>(base.transform, MaskUtilities.FindRootSortOverrideCanvas(base.transform));
			material = CustomMaterial.Add(base.gameObject.GetInstanceID(), maskParentComponents, baseMaterial, maskTexture, componentInParent.maskMatrix, tilingOffset, componentInParent.IsScreen, (1 << m_StencilValue) - 1, StencilOp.Keep, CompareFunction.Equal, ColorWriteMask.All, (1 << m_StencilValue) - 1, 0);
			CustomMaterial.Remove(m_MaskMaterial);
			m_MaskMaterial = material;
		}
		return material;
	}
}
