using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace ExpansionImage
{
	public class ExpansionImage : Image
	{
		[SerializeField]
		private Sprite _expantionSprite;

		[SerializeField]
		private Color _decalColor = Color.white;

		[SerializeField]
		private Rect _uv1Scale = new Rect(0f, 0f, 1f, 1f);

		private readonly List<UIVertex> _vlist = new List<UIVertex>();

		private Vector2 _sizeDelta;

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

		public Sprite ExpansionSprite
		{
			get
			{
				return _expantionSprite;
			}
			set
			{
				SetPropertyUtility.SetClass(ref _expantionSprite, value, delegate
				{
					SetMaterialParamaters();
					SetAllDirty();
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

		protected virtual void SetMaterialParamaters()
		{
			if (material == null || material.shader.name != "MAI2/UI/MultiImage")
			{
				Shader shader = Shader.Find("MAI2/UI/MultiImage");
				material = new Material(shader);
			}
			material.SetTexture("_MaskTex", (_expantionSprite == null) ? Graphic.s_WhiteTexture : _expantionSprite.texture);
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			base.OnPopulateMesh(vh);
			if (_expantionSprite != null && base.rectTransform.sizeDelta != _sizeDelta)
			{
				_uv1Scale.width = base.rectTransform.rect.width / _expantionSprite.rect.width;
				_uv1Scale.height = base.rectTransform.rect.height / _expantionSprite.rect.height;
				_sizeDelta = base.rectTransform.sizeDelta;
			}
			material.SetColor("_Color", _decalColor);
			GenerateUV1Normalize(vh);
			ApplyUVScroll(vh);
		}

		private void ApplyUVScroll(VertexHelper vh)
		{
			UIVertex vertex = default(UIVertex);
			int currentVertCount = vh.currentVertCount;
			for (int i = 0; i < currentVertCount; i++)
			{
				vh.PopulateUIVertex(ref vertex, i);
				vertex.uv1.x = vertex.uv1.x * _uv1Scale.width + _uv1Scale.x;
				vertex.uv1.y = vertex.uv1.y * _uv1Scale.height + _uv1Scale.y;
				vh.SetUIVertex(vertex, i);
			}
		}

		private void GenerateUV1Normalize(VertexHelper vh)
		{
			if (vh.currentVertCount <= 0 || !(_expantionSprite != null))
			{
				return;
			}
			vh.GetUIVertexStream(_vlist);
			int count = _vlist.Count;
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			float num3 = float.MinValue;
			float num4 = float.MinValue;
			for (int i = 0; i < count; i++)
			{
				UIVertex uIVertex = _vlist[i];
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
			Vector4 outerUV = DataUtility.GetOuterUV(_expantionSprite);
			for (int j = 0; j < count; j++)
			{
				UIVertex value = _vlist[j];
				value.uv1 = new Vector2(Mathf.Lerp(outerUV.x, outerUV.z, Mathf.InverseLerp(vector.x, vector.z, value.position.x)), Mathf.Lerp(outerUV.y, outerUV.w, Mathf.InverseLerp(vector.y, vector.w, value.position.y)));
				_vlist[j] = value;
			}
			vh.Clear();
			vh.AddUIVertexTriangleStream(_vlist);
		}

		public void CalculateWidthHeight()
		{
			if (!(ExpansionSprite == null))
			{
				Rect uV1Scale = UV1Scale;
				uV1Scale.width = base.rectTransform.rect.width / _expantionSprite.rect.width;
				uV1Scale.height = base.rectTransform.rect.height / _expantionSprite.rect.height;
				uV1Scale.x = (0f - (base.rectTransform.rect.width / 2f - _expantionSprite.rect.width / 2f)) / _expantionSprite.rect.width;
				uV1Scale.y = (0f - (base.rectTransform.rect.height / 2f - _expantionSprite.rect.height / 2f)) / _expantionSprite.rect.height;
				UV1Scale = uV1Scale;
			}
		}

		public void ResetWidthHeight()
		{
			if (!(ExpansionSprite == null))
			{
				UV1Scale = new Rect(0f, 0f, 1f, 1f);
			}
		}

		public void SetExpentionNativeSize()
		{
			if (_expantionSprite != null)
			{
				float x = _expantionSprite.rect.width / base.pixelsPerUnit;
				float y = _expantionSprite.rect.height / base.pixelsPerUnit;
				base.rectTransform.anchorMax = base.rectTransform.anchorMin;
				base.rectTransform.sizeDelta = new Vector2(x, y);
				SetAllDirty();
			}
		}
	}
}
