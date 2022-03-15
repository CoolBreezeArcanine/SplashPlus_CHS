using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ExpansionImage
{
	[RequireComponent(typeof(Graphic))]
	public class VertexColor : BaseMeshEffect
	{
		[SerializeField]
		private Color _v1 = Color.white;

		[SerializeField]
		private Color _v2 = Color.white;

		[SerializeField]
		private Color _v3 = Color.white;

		[SerializeField]
		private Color _v4 = Color.white;

		public override void ModifyMesh(VertexHelper vh)
		{
			if (IsActive())
			{
				List<UIVertex> list = CustomListPool<UIVertex>.Get();
				vh.GetUIVertexStream(list);
				Vertices(list);
				vh.Clear();
				vh.AddUIVertexTriangleStream(list);
				CustomListPool<UIVertex>.Release(list);
			}
		}

		private void Vertices(IList<UIVertex> vertexList)
		{
			if (!IsActive() || vertexList == null || vertexList.Count == 0)
			{
				return;
			}
			for (int i = 0; i < vertexList.Count; i++)
			{
				Color color = Color.white;
				switch (i)
				{
				case 0:
				case 5:
					color = _v4;
					break;
				case 1:
					color = _v1;
					break;
				case 2:
				case 3:
					color = _v2;
					break;
				case 4:
					color = _v3;
					break;
				}
				UIVertex value = vertexList[i];
				value.color = color;
				vertexList[i] = value;
			}
		}
	}
}
