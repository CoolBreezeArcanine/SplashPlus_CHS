using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomGraphic : Graphic
{
	public List<Vector2> vertex = new List<Vector2>
	{
		new Vector2(-50f, 50f),
		new Vector2(50f, 50f),
		new Vector2(50f, -50f),
		new Vector2(-50f, -50f)
	};

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
		List<int> indices = new List<int>(new Triangulator(vertex.ToArray(), Camera.main.transform.position).Triangulate());
		List<UIVertex> list = new List<UIVertex>();
		for (int i = 0; i < vertex.Count; i++)
		{
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.position = new Vector3(vertex[i].x, vertex[i].y, 0f);
			simpleVert.color = color;
			list.Add(simpleVert);
		}
		vh.AddUIVertexStream(list, indices);
	}
}
