using System;
using UnityEngine;

[Serializable]
public class SpriteCounterData
{
	public string Text;

	public float Scale;

	public float AnimationScale;

	public float AnimationYPos;

	public CustomUIVertex[] UiVertexs = new CustomUIVertex[4];

	public bool IsAnimated;

	public AnimationCurve PosYCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);

	public AnimationCurve ScaleCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);

	public Vector2 RelativePosition;

	public Vector2 DefaultScale;
}
