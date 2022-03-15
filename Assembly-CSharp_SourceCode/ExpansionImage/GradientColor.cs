using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ExpansionImage
{
	[RequireComponent(typeof(Graphic))]
	public class GradientColor : BaseMeshEffect
	{
		private const int TextVert = 6;

		[SerializeField]
		private Color _y1 = Color.white;

		[SerializeField]
		private Color _y2 = Color.white;

		[SerializeField]
		private Color _x1 = Color.white;

		[SerializeField]
		private Color _x2 = Color.white;

		[SerializeField]
		[Range(-1f, 1f)]
		private float _gradientOffsetVertical;

		[SerializeField]
		[Range(-1f, 1f)]
		private float _gradientOffsetHorizontal;

		[SerializeField]
		private bool _splitTextGradient;

		public Color Top
		{
			get
			{
				return _y1;
			}
			set
			{
				if (!(_y1 == value))
				{
					_y1 = value;
					SetDirty();
				}
			}
		}

		public void SetColor(Color top, Color bottom)
		{
			_y1 = top;
			_y2 = bottom;
			SetDirty();
		}

		private void SetDirty()
		{
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}

		public override void ModifyMesh(VertexHelper vh)
		{
			if (IsActive())
			{
				List<UIVertex> list = CustomListPool<UIVertex>.Get();
				vh.GetUIVertexStream(list);
				ModVertices(list);
				vh.Clear();
				vh.AddUIVertexTriangleStream(list);
				CustomListPool<UIVertex>.Release(list);
			}
		}

		private void ModVertices(IList<UIVertex> vertexList)
		{
			if (!IsActive() || vertexList == null || vertexList.Count == 0)
			{
				return;
			}
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			for (int i = 0; i < vertexList.Count; i++)
			{
				if (i == 0 || (_splitTextGradient && i % 6 == 0))
				{
					num = vertexList[i].position.x;
					num2 = vertexList[i].position.y;
					float num5 = vertexList[i].position.x;
					float num6 = vertexList[i].position.y;
					int num7 = (_splitTextGradient ? (i + 6) : vertexList.Count);
					for (int j = i; j < num7 && j < vertexList.Count; j++)
					{
						UIVertex uIVertex = vertexList[j];
						num = Mathf.Min(num, uIVertex.position.x);
						num2 = Mathf.Min(num2, uIVertex.position.y);
						num5 = Mathf.Max(num5, uIVertex.position.x);
						num6 = Mathf.Max(num6, uIVertex.position.y);
					}
					num3 = num5 - num;
					num4 = num6 - num2;
				}
				UIVertex value = vertexList[i];
				Color color = value.color;
				Color color2 = Color.Lerp(_y2, _y1, ((num4 > 0f) ? ((value.position.y - num2) / num4) : 0f) + _gradientOffsetVertical);
				Color color3 = Color.Lerp(_x1, _x2, ((num3 > 0f) ? ((value.position.x - num) / num3) : 0f) + _gradientOffsetHorizontal * -1f);
				value.color = color * color2 * color3;
				vertexList[i] = value;
			}
		}
	}
}
