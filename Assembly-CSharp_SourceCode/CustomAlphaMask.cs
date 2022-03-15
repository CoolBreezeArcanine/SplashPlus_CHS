using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[DisallowMultipleComponent]
[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class CustomAlphaMask : Mask
{
	[SerializeField]
	private Vector2 tiling = new Vector2(1f, 1f);

	[SerializeField]
	private Vector2 offset = new Vector2(0f, 0f);

	[NonSerialized]
	private Material stencilMaskMaterial;

	[NonSerialized]
	private Material stencilUnmaskMaterial;

	[NonSerialized]
	public Matrix4x4 maskMatrix = Matrix4x4.identity;

	[NonSerialized]
	private RectTransform maskRectTransform;

	[NonSerialized]
	private Graphic maskGraphic;

	private Matrix4x4 maskQuadMatrix = Matrix4x4.identity;

	private Matrix4x4 prevMatrix = Matrix4x4.identity;

	public bool IsScreen { get; private set; }

	public Vector2 Tiling => tiling;

	public Vector2 Offset => offset;

	public Graphic GetGraphic => maskGraphic ?? (maskGraphic = GetComponent<Graphic>());

	public RectTransform GetRectTransform => maskRectTransform ?? (maskRectTransform = GetComponent<RectTransform>());

	public override bool MaskEnabled()
	{
		if (IsActive())
		{
			return GetGraphic != null;
		}
		return false;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (GetGraphic != null)
		{
			GetGraphic.canvasRenderer.hasPopInstruction = true;
			GetGraphic.SetMaterialDirty();
		}
		UpdateMask();
		MaskUtilities.NotifyStencilStateChanged(this);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if (GetGraphic != null)
		{
			GetGraphic.SetMaterialDirty();
			GetGraphic.canvasRenderer.hasPopInstruction = false;
			GetGraphic.canvasRenderer.popMaterialCount = 0;
		}
		StencilMaterial.Remove(stencilMaskMaterial);
		stencilMaskMaterial = null;
		StencilMaterial.Remove(stencilUnmaskMaterial);
		stencilUnmaskMaterial = null;
		UpdateMask();
		MaskUtilities.NotifyStencilStateChanged(this);
	}

	private void Update()
	{
	}

	private void LateUpdate()
	{
		UpdateMask();
	}

	private void OnRenderObject()
	{
		RectTransform component = GetComponent<RectTransform>();
		Vector3 vector = base.transform.lossyScale;
		if (component != null)
		{
			vector = Vector3.Scale(vector, component.rect.size);
		}
		vector.z = 0.1f;
		Vector3 b = new Vector3(1f, 1f, 1f);
		b.x = 1f / b.x;
		b.y = 1f / b.y;
		Vector3 vector2 = Vector3.Scale(vector, b);
		Matrix4x4 matrix4x = Matrix4x4.TRS(base.transform.position, base.transform.rotation, vector2);
		Vector3 vector3 = base.transform.rotation * -vector * 0.5f;
		Vector3 vector4 = Vector3.Scale(new Vector3(0f, 0f, 0f), -vector2);
		Matrix4x4 matrix4x2 = Matrix4x4.TRS(vector3 + vector4, Quaternion.identity, Vector3.one);
		maskQuadMatrix = matrix4x2 * matrix4x;
		UpdateMask();
	}

	private void OnDrawGizmos()
	{
		Gizmos.matrix = maskQuadMatrix;
		Vector3 one = Vector3.one;
		one.z = 0f;
		Gizmos.color = new Color(0f, 0f, 0f, 0f);
		Gizmos.DrawCube(new Vector3(0.5f, 0.5f, 0.5f), one);
	}

	public override Material GetModifiedMaterial(Material baseMaterial)
	{
		if (!MaskEnabled())
		{
			return baseMaterial;
		}
		int stencilDepth = MaskUtilities.GetStencilDepth(base.transform, MaskUtilities.FindRootSortOverrideCanvas(base.transform));
		if (stencilDepth >= 8)
		{
			return baseMaterial;
		}
		int num = 1 << stencilDepth;
		if (num == 1)
		{
			Material material = StencilMaterial.Add(baseMaterial, 1, StencilOp.Replace, CompareFunction.Always, (ColorWriteMask)0);
			StencilMaterial.Remove(stencilMaskMaterial);
			stencilMaskMaterial = material;
			Material material2 = StencilMaterial.Add(baseMaterial, 1, StencilOp.Zero, CompareFunction.Always, (ColorWriteMask)0);
			StencilMaterial.Remove(stencilUnmaskMaterial);
			stencilUnmaskMaterial = material2;
			GetGraphic.canvasRenderer.popMaterialCount = 1;
			GetGraphic.canvasRenderer.SetPopMaterial(stencilUnmaskMaterial, 0);
		}
		else
		{
			Material material3 = StencilMaterial.Add(baseMaterial, num | (num - 1), StencilOp.Replace, CompareFunction.Equal, (ColorWriteMask)0, num - 1, num | (num - 1));
			StencilMaterial.Remove(stencilMaskMaterial);
			stencilMaskMaterial = material3;
			GetGraphic.canvasRenderer.hasPopInstruction = true;
			Material material4 = StencilMaterial.Add(baseMaterial, num - 1, StencilOp.Replace, CompareFunction.Equal, (ColorWriteMask)0, num - 1, num | (num - 1));
			StencilMaterial.Remove(stencilUnmaskMaterial);
			stencilUnmaskMaterial = material4;
			GetGraphic.canvasRenderer.popMaterialCount = 1;
			GetGraphic.canvasRenderer.SetPopMaterial(stencilUnmaskMaterial, 0);
		}
		return stencilMaskMaterial;
	}

	private void UpdateMask()
	{
		if (GetGraphic == null)
		{
			return;
		}
		maskMatrix = base.transform.worldToLocalMatrix;
		Vector3 a = GetRectTransform.rect.size;
		a.z = 1f;
		a = Vector3.Scale(a, base.transform.lossyScale);
		maskMatrix.SetTRS(base.transform.position, base.transform.rotation, a);
		maskMatrix = Matrix4x4.Inverse(maskMatrix);
		if (maskMatrix != prevMatrix)
		{
			Matrix4x4 identity = Matrix4x4.identity;
			Canvas canvas = GetGraphic.canvas;
			IsScreen = canvas != null && (canvas.renderMode == RenderMode.ScreenSpaceOverlay || (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null));
			if (IsScreen)
			{
				identity = Matrix4x4.TRS(canvas.GetComponent<RectTransform>().rect.size / 2f * canvas.scaleFactor, Quaternion.identity, Vector3.one * canvas.scaleFactor);
				maskMatrix *= identity;
			}
			MaskUtilities.NotifyStencilStateChanged(this);
		}
		prevMatrix = maskMatrix;
	}
}
