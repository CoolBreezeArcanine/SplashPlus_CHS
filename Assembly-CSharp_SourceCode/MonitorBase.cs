using UnityEngine;

public abstract class MonitorBase : MonoBehaviour
{
	protected int monitorIndex;

	protected bool isPlayerActive;

	[SerializeField]
	protected CanvasGroup Main;

	[SerializeField]
	protected CanvasGroup Sub;

	public int MonitorIndex => monitorIndex;

	public virtual void Initialize(int monIndex, bool isActive)
	{
		monitorIndex = monIndex;
		isPlayerActive = isActive;
		SetVisible(isActive);
	}

	public abstract void ViewUpdate();

	protected virtual void SetVisible(bool isActive)
	{
	}

	public virtual bool IsActive()
	{
		return isPlayerActive;
	}

	public virtual void Release()
	{
	}
}
