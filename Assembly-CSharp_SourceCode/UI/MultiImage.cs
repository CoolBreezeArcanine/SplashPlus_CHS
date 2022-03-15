using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace UI
{
	public class MultiImage : Image
	{
		[SerializeField]
		private MultiImageBlendType _blendType;

		[SerializeField]
		private Sprite _image2;

		[SerializeField]
		private Color _color2 = Color.white;

		[SerializeField]
		private MultiImageMultiMode _multiMode;

		[SerializeField]
		private MultiImageImage2UVType _image2UVType;

		[SerializeField]
		private Rect _uvScale = new Rect(0f, 0f, 1f, 1f);

		[SerializeField]
		private Rect _uv1Scale = new Rect(0f, 0f, 1f, 1f);

		private List<UIVertex> vlist = new List<UIVertex>();

		public MultiImageBlendType BlendType
		{
			get
			{
				return _blendType;
			}
			set
			{
				SetPropertyUtility.SetStruct(ref _blendType, value, delegate
				{
					SetMaterialParamaters();
				});
			}
		}

		public Sprite Image2
		{
			get
			{
				return _image2;
			}
			set
			{
				SetPropertyUtility.SetClass(ref _image2, value, delegate
				{
					SetMaterialParamaters();
					SetAllDirty();
				});
			}
		}

		public Texture Image2Texture
		{
			get
			{
				if (!(_image2 == null))
				{
					return _image2.texture;
				}
				return Graphic.s_WhiteTexture;
			}
		}

		public Color Color2
		{
			get
			{
				return _color2;
			}
			set
			{
				SetPropertyUtility.SetStruct(ref _color2, value, delegate
				{
					SetVerticesDirty();
				});
			}
		}

		public MultiImageMultiMode MultiMode
		{
			get
			{
				return _multiMode;
			}
			set
			{
				SetPropertyUtility.SetStruct(ref _multiMode, value, delegate
				{
					SetMaterialParamaters();
				});
			}
		}

		public MultiImageImage2UVType Image2UVType
		{
			get
			{
				return _image2UVType;
			}
			set
			{
				SetPropertyUtility.SetStruct(ref _image2UVType, value, delegate
				{
					SetVerticesDirty();
				});
			}
		}

		public Rect UVScale
		{
			get
			{
				return _uvScale;
			}
			set
			{
				SetPropertyUtility.SetStruct(ref _uvScale, value, delegate
				{
					SetVerticesDirty();
				});
			}
		}

		public Rect UV1Scale
		{
			get
			{
				return _uv1Scale;
			}
			set
			{
				SetPropertyUtility.SetStruct(ref _uv1Scale, value, delegate
				{
					SetVerticesDirty();
				});
			}
		}

		protected override void Awake()
		{
			base.Awake();
			material = null;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			SetMaterialParamaters();
		}

		private void SetMaterialParamaters()
		{
			if (material == null || material.shader.name != "MAI2/UI/MultiImage")
			{
				Shader shader = Shader.Find("MAI2/UI/MultiImage");
				material = new Material(shader);
			}
			material.SetTexture("_MaskTex", Image2Texture);
			switch (_blendType)
			{
			case MultiImageBlendType.Normal:
				material.SetInt("_SrcBlend", 5);
				material.SetInt("_DstBlend", 10);
				material.SetInt("_BlendOp", 0);
				break;
			case MultiImageBlendType.Add:
				material.SetInt("_SrcBlend", 5);
				material.SetInt("_DstBlend", 1);
				material.SetInt("_BlendOp", 0);
				break;
			case MultiImageBlendType.Subtract:
				material.SetInt("_SrcBlend", 5);
				material.SetInt("_DstBlend", 1);
				material.SetInt("_BlendOp", 2);
				break;
			}
			switch (_multiMode)
			{
			case MultiImageMultiMode.Mask:
				material.EnableKeyword("_MODE_MASK");
				material.DisableKeyword("_MODE_DECAL");
				material.DisableKeyword("_MODE_DEBUG");
				break;
			case MultiImageMultiMode.Decal:
				material.DisableKeyword("_MODE_MASK");
				material.EnableKeyword("_MODE_DECAL");
				material.DisableKeyword("_MODE_DEBUG");
				break;
			case MultiImageMultiMode.Debug:
				material.DisableKeyword("_MODE_MASK");
				material.DisableKeyword("_MODE_DECAL");
				material.EnableKeyword("_MODE_DEBUG");
				break;
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			base.OnPopulateMesh(vh);
			material.SetColor("_Color", _color2);
			switch (Image2UVType)
			{
			case MultiImageImage2UVType.Copy:
				GenerateUV1Copy(vh);
				break;
			case MultiImageImage2UVType.Normalize:
				GenerateUV1Normalize(vh);
				break;
			}
			ApplyUVScroll(vh);
		}

		private static void GenerateUV1Copy(VertexHelper vh)
		{
			UIVertex vertex = default(UIVertex);
			int currentVertCount = vh.currentVertCount;
			for (int i = 0; i < currentVertCount; i++)
			{
				vh.PopulateUIVertex(ref vertex, i);
				vertex.uv1 = vertex.uv0;
				vh.SetUIVertex(vertex, i);
			}
		}

		private void GenerateUV1Normalize(VertexHelper vh)
		{
			if (vh.currentVertCount <= 0 || !(_image2 != null))
			{
				return;
			}
			vh.GetUIVertexStream(vlist);
			int count = vlist.Count;
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			float num3 = float.MinValue;
			float num4 = float.MinValue;
			for (int i = 0; i < count; i++)
			{
				UIVertex uIVertex = vlist[i];
				if (num > uIVertex.position.x)
				{
					num = uIVertex.position.x;
				}
				if (num2 > uIVertex.position.y)
				{
					num2 = uIVertex.position.y;
				}
				if (num3 < uIVertex.position.x)
				{
					num3 = uIVertex.position.x;
				}
				if (num4 < uIVertex.position.y)
				{
					num4 = uIVertex.position.y;
				}
			}
			Vector4 vector = new Vector4(num, num2, num3, num4);
			Vector4 outerUV = DataUtility.GetOuterUV(_image2);
			for (int j = 0; j < count; j++)
			{
				UIVertex value = vlist[j];
				value.uv1 = new Vector2(Mathf.Lerp(outerUV.x, outerUV.z, Mathf.InverseLerp(vector.x, vector.z, value.position.x)), Mathf.Lerp(outerUV.y, outerUV.w, Mathf.InverseLerp(vector.y, vector.w, value.position.y)));
				vlist[j] = value;
			}
			vh.Clear();
			vh.AddUIVertexTriangleStream(vlist);
		}

		private void ApplyUVScroll(VertexHelper vh)
		{
			UIVertex vertex = default(UIVertex);
			int currentVertCount = vh.currentVertCount;
			for (int i = 0; i < currentVertCount; i++)
			{
				vh.PopulateUIVertex(ref vertex, i);
				vertex.uv0.x = vertex.uv0.x * _uvScale.width + _uvScale.x;
				vertex.uv0.y = vertex.uv0.y * _uvScale.height + _uvScale.y;
				vertex.uv1.x = vertex.uv1.x * _uv1Scale.width + _uv1Scale.x;
				vertex.uv1.y = vertex.uv1.y * _uv1Scale.height + _uv1Scale.y;
				vh.SetUIVertex(vertex, i);
			}
		}

		public void CalculateWH()
		{
			Rect uV1Scale = UV1Scale;
			uV1Scale.width = base.rectTransform.rect.width / _image2.rect.width;
			uV1Scale.height = base.rectTransform.rect.height / _image2.rect.height;
			UV1Scale = uV1Scale;
		}
	}
}
