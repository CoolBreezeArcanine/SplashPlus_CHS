using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ExpansionImage
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(MaskableGraphic))]
	public class MeshSubdivide : BaseMeshEffect
	{
		[SerializeField]
		[Range(1f, 100f)]
		private int _divideX = 1;

		[SerializeField]
		[Range(1f, 100f)]
		private int _divideY = 1;

		[NonSerialized]
		private RectTransform _rectTransform;

		public int DivideX
		{
			get
			{
				return _divideX;
			}
			set
			{
				_divideX = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty();
				}
			}
		}

		public int DivideY
		{
			get
			{
				return _divideY;
			}
			set
			{
				_divideY = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty();
				}
			}
		}

		public RectTransform RectTransform
		{
			get
			{
				if (_rectTransform == null)
				{
					_rectTransform = GetComponent<RectTransform>();
				}
				return _rectTransform;
			}
		}

		protected MeshSubdivide()
		{
		}

		public override void ModifyMesh(VertexHelper vh)
		{
			if (IsActive())
			{
				List<UIVertex> list = CustomListPool<UIVertex>.Get();
				vh.GetUIVertexStream(list);
				Verts(list, _divideX, _divideY);
				vh.Clear();
				vh.AddUIVertexTriangleStream(list);
				CustomListPool<UIVertex>.Release(list);
			}
		}

		private void Verts(List<UIVertex> verts, int x, int y)
		{
			int count = verts.Count;
			int num = count * x * y;
			if (verts.Capacity < num)
			{
				verts.Capacity = num;
			}
			Vector2 size = RectTransform.rect.size;
			Vector2 vector = new Vector2(size.x / (float)x, size.y / (float)y);
			Vector2 vector2 = new Vector2(verts[4].uv0.x - verts[0].uv0.x, verts[1].uv0.y - verts[0].uv0.y);
			Vector2 vector3 = RectTransform.pivot - new Vector2(0.5f, 0.5f);
			Vector2 vector4 = (vector - size) / 2f + new Vector2(vector3.x * vector.x, vector3.y * vector.y) - new Vector2(size.x * vector3.x, size.y * vector3.y);
			Vector3 vector5 = Vector3.zero;
			for (int i = 0; i < count; i++)
			{
				UIVertex value = verts[i];
				Vector3 position = value.position;
				position.x /= x;
				position.y /= y;
				position.x += vector4.x;
				position.y += vector4.y;
				value.position = position;
				Vector3 vector6 = value.uv0;
				switch (i % count)
				{
				case 0:
					vector6 = value.uv0;
					vector5 = vector6;
					break;
				case 1:
					vector6 = vector5 + new Vector3(0f, 1f / (float)y * vector2.y, 0f);
					break;
				case 2:
				case 3:
					vector6 = vector5 + new Vector3(1f / (float)x * vector2.x, 1f / (float)y * vector2.y, 0f);
					break;
				case 4:
					vector6 = vector5 + new Vector3(1f / (float)x * vector2.x, 0f, 0f);
					break;
				case 5:
					vector6 = vector5;
					break;
				}
				value.uv0 = vector6;
				verts[i] = value;
			}
			for (int j = count; j < num; j++)
			{
				int index = j % count;
				int num2 = j / count;
				UIVertex value = verts[index];
				verts.Add(value);
				Vector3 position2 = value.position;
				position2.x += (float)(num2 % x) * vector.x;
				position2.y += (float)(num2 / x) * vector.y;
				value.position = position2;
				Vector3 vector7 = value.uv0;
				vector7 += new Vector3((float)(num2 % x) / (float)x * vector2.x, (float)(num2 / x) / (float)y * vector2.y, 0f);
				value.uv0 = vector7;
				verts[j] = value;
			}
		}
	}
}
