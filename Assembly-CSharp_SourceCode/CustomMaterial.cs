using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class CustomMaterial
{
	private class StencilMatEntry
	{
		public StencilOp operation;

		public CompareFunction compareFunction = CompareFunction.Always;

		public Material baseMaterial;

		public Material customMaterial;

		public int count;

		public int stencilId;

		public int readMask;

		public int writeMask;

		public bool useAlphaClip;

		public ColorWriteMask colorMask;
	}

	private class MaterialEntry : StencilMatEntry
	{
		public int id;

		public bool isScreenSpace;

		public Matrix4x4 matrix;

		public Vector4 tilingAndOffset;
	}

	private static List<MaterialEntry> list = new List<MaterialEntry>();

	private static bool CheckStencilValue(StencilMatEntry entry, int stencilId, StencilOp operation, CompareFunction compare, ColorWriteMask colorWriteMask, int readMask, int writeMask)
	{
		if (entry.stencilId == stencilId && entry.operation == operation && entry.compareFunction == compare)
		{
			if (entry.readMask == readMask && entry.writeMask == writeMask)
			{
				return entry.colorMask == colorWriteMask;
			}
			return false;
		}
		return false;
	}

	public static Material Add(int instanceId, CustomAlphaMask[] parentComponents, Material baseMaterial, Texture maskTexture, Matrix4x4 maskMatrix, Vector4 tilingOffset, bool isScreen, int stencilId, StencilOp operation, CompareFunction compare, ColorWriteMask colorWriteMask, int readMask = 255, int writeMask = 255)
	{
		for (int i = 0; i < list.Count; i++)
		{
			MaterialEntry materialEntry = list[i];
			if (materialEntry.id == instanceId)
			{
				SetMaterial(materialEntry, parentComponents, maskTexture, maskMatrix, tilingOffset, isScreen);
				materialEntry.count++;
				return materialEntry.customMaterial;
			}
		}
		MaterialEntry materialEntry2 = new MaterialEntry
		{
			id = instanceId,
			count = 1,
			baseMaterial = baseMaterial,
			customMaterial = new Material(baseMaterial),
			stencilId = stencilId,
			operation = operation,
			compareFunction = compare,
			readMask = readMask,
			writeMask = writeMask,
			colorMask = colorWriteMask,
			useAlphaClip = (operation != 0 && writeMask > 0),
			isScreenSpace = isScreen,
			matrix = maskMatrix,
			tilingAndOffset = tilingOffset
		};
		materialEntry2.customMaterial.hideFlags = HideFlags.HideAndDontSave;
		if (materialEntry2.customMaterial.HasProperty("_UseAlphaClip"))
		{
			materialEntry2.customMaterial.SetInt("_UseAlphaClip", materialEntry2.useAlphaClip ? 1 : 0);
		}
		if (materialEntry2.useAlphaClip)
		{
			materialEntry2.customMaterial.EnableKeyword("UNITY_UI_ALPHACLIP");
		}
		else
		{
			materialEntry2.customMaterial.DisableKeyword("UNITY_UI_ALPHACLIP");
		}
		SetMaterial(materialEntry2, parentComponents, maskTexture, maskMatrix, tilingOffset, isScreen);
		list.Add(materialEntry2);
		return materialEntry2.customMaterial;
	}

	public static void Remove(Material material)
	{
		if (material == null)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			MaterialEntry materialEntry = list[i];
			if (!(materialEntry.customMaterial == material))
			{
				continue;
			}
			if (--materialEntry.count == 0)
			{
				if (Application.isEditor)
				{
					Object.DestroyImmediate(materialEntry.customMaterial);
				}
				else
				{
					Object.Destroy(materialEntry.customMaterial);
				}
				materialEntry.baseMaterial = null;
				list.RemoveAt(i);
			}
			break;
		}
	}

	private static void SetMaterial(MaterialEntry entry, CustomAlphaMask[] parentComponents, Texture maskTexture, Matrix4x4 maskMatrix, Vector4 tilingOffset, bool isScreen)
	{
		if (isScreen)
		{
			entry.customMaterial.EnableKeyword("UI_WORLD_COORDINATE");
		}
		else
		{
			entry.customMaterial.DisableKeyword("UI_WORLD_COORDINATE");
		}
		entry.customMaterial.SetVector("_MaskTex_ST", tilingOffset);
		entry.customMaterial.SetTexture("_MaskTex", maskTexture);
		entry.customMaterial.SetMatrix("_World", maskMatrix);
		if (parentComponents.Length > 1)
		{
			CustomAlphaMask customAlphaMask = parentComponents[1];
			Texture2D value = (Texture2D)customAlphaMask.GetGraphic.mainTexture;
			entry.customMaterial.SetTexture("_ParentMaskTex", value);
			entry.customMaterial.SetMatrix("_Parent", customAlphaMask.maskMatrix);
			if (parentComponents.Length > 2)
			{
				entry.customMaterial.SetTexture("_Parent2MaskTex", parentComponents[2].GetGraphic.mainTexture);
				entry.customMaterial.SetMatrix("_Parent2", parentComponents[2].maskMatrix);
			}
		}
	}
}
