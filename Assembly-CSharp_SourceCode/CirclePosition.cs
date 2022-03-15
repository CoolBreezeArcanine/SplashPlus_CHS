using System;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteAlways]
public class CirclePosition : UIBehaviour
{
	[SerializeField]
	private float radius;

	[SerializeField]
	private float angle;

	public void SetRadius(float radius)
	{
		this.radius = radius;
		Positioning();
	}

	public void SetAngle(float angle)
	{
		this.angle = angle;
		Positioning();
	}

	private void Positioning()
	{
		if (base.isActiveAndEnabled)
		{
			float x = radius * Mathf.Cos(angle * ((float)Math.PI / 180f));
			float y = radius * Mathf.Sin(angle * ((float)Math.PI / 180f));
			base.transform.localPosition = new Vector3(x, y, 0f);
		}
	}

	protected override void OnDidApplyAnimationProperties()
	{
		Positioning();
		base.OnDidApplyAnimationProperties();
	}
}
