using Manager;
using UnityEngine;

public class MenuAnimationData
{
	private float time;

	public AnimationCurve curve;

	public Vector2 basePosition;

	public Vector2 toPosition;

	public float duration;

	public int nextImageIndex = -1;

	public bool Update(out Vector2 pos)
	{
		time += GameManager.GetGameMSecAdd();
		float t = curve.Evaluate(time / duration);
		pos = Vector2.Lerp(basePosition, toPosition, t);
		return time >= duration;
	}
}
