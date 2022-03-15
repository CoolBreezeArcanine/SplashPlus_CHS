using System;
using UnityEngine;

[Serializable]
public class CustomUIVertex
{
	public Vector2 UV;

	public Vector3 Position;

	public Color32 Color;

	public UIVertex Get()
	{
		UIVertex result = default(UIVertex);
		result.position = Position;
		result.color = Color;
		result.uv0 = UV;
		return result;
	}
}
