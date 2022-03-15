using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class UVImage : Image
	{
		[SerializeField]
		private Rect _uvScale = new Rect(0f, 0f, 1f, 1f);

		public Rect UVScale
		{
			get
			{
				return _uvScale;
			}
			set
			{
				if (_uvScale != value)
				{
					_uvScale = value;
					SetVerticesDirty();
				}
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			base.OnPopulateMesh(vh);
			UIVertex vertex = default(UIVertex);
			int currentVertCount = vh.currentVertCount;
			for (int i = 0; i < currentVertCount; i++)
			{
				vh.PopulateUIVertex(ref vertex, i);
				vertex.uv0.x = vertex.uv0.x * _uvScale.width + _uvScale.x;
				vertex.uv0.y = vertex.uv0.y * _uvScale.height + _uvScale.y;
				vh.SetUIVertex(vertex, i);
			}
		}
	}
}
