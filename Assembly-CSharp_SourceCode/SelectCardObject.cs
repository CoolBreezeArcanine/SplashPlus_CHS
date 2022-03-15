using UnityEngine;

public class SelectCardObject : MonoBehaviour
{
	private Vector3 fromPosition;

	private Vector3 toPosition;

	private Vector3 fromScale;

	private Vector3 toScale;

	private bool isActive;

	public bool IsActive
	{
		get
		{
			return isActive;
		}
		set
		{
			isActive = value;
			base.gameObject.SetActive(isActive);
			toPosition = Vector3.zero;
			toScale = Vector3.zero;
		}
	}

	public void SetTranslationPosition(Vector3 toPos, Vector3 toScl)
	{
		fromPosition = base.transform.localPosition;
		toPosition = toPos;
		fromScale = base.transform.localScale;
		toScale = toScl;
	}

	public virtual void ViewUpdate()
	{
	}

	public void ViewAnimationUpdate(float progress)
	{
		base.transform.localPosition = Vector3.Lerp(fromPosition, toPosition, progress);
		base.transform.localScale = Vector3.Lerp(fromScale, toScale, progress);
	}

	public void SetAnimationComplete()
	{
		fromPosition = toPosition;
		fromScale = toScale;
	}

	public virtual void ResetData()
	{
	}
}
