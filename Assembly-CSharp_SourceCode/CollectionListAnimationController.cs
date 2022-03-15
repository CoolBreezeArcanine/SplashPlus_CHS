using UnityEngine;

public class CollectionListAnimationController : MonoBehaviour
{
	public enum AnimationType
	{
		ToRight,
		ToLeft,
		ToUpper
	}

	private static readonly int HashValueSyncTimer = Animator.StringToHash("SyncTimer");

	private static readonly int HashValueSlideInToLeft = Animator.StringToHash("IsSlideInToLeft");

	private static readonly int HashValueSlideInToRight = Animator.StringToHash("IsSlideInToRight");

	private static readonly int HashValueSlideOutToLeft = Animator.StringToHash("IsSlideOutToLeft");

	private static readonly int HashValueSlideOutToRight = Animator.StringToHash("IsSlideOutToRight");

	private static readonly int HashValueSlideInToUpper = Animator.StringToHash("IsSlideInToUpper");

	private static readonly int HashValueSlideOutToLower = Animator.StringToHash("IsSlideOutToLower");

	[SerializeField]
	private Animator _listAnimator;

	public void Initialize()
	{
		_listAnimator.Rebind();
	}

	public void SetEnable(bool isActive)
	{
		_listAnimator.enabled = isActive;
	}

	public void SetFloat(float rate)
	{
		_listAnimator.SetFloat(HashValueSyncTimer, rate);
	}

	public float GetAnimationLength()
	{
		return _listAnimator.GetCurrentAnimatorStateInfo(0).length;
	}

	public void SetSlideOutAnim(AnimationType type)
	{
		switch (type)
		{
		case AnimationType.ToLeft:
			_listAnimator.SetTrigger(HashValueSlideOutToLeft);
			break;
		case AnimationType.ToRight:
			_listAnimator.SetTrigger(HashValueSlideOutToRight);
			break;
		case AnimationType.ToUpper:
			_listAnimator.SetTrigger(HashValueSlideOutToLower);
			break;
		}
	}

	public void SetSlideInAnim(AnimationType type)
	{
		switch (type)
		{
		case AnimationType.ToLeft:
			_listAnimator.SetTrigger(HashValueSlideInToLeft);
			break;
		case AnimationType.ToRight:
			_listAnimator.SetTrigger(HashValueSlideInToRight);
			break;
		case AnimationType.ToUpper:
			_listAnimator.SetTrigger(HashValueSlideInToUpper);
			break;
		}
	}
}
