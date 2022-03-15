using UI;
using UnityEngine;
using UnityEngine.UI;

public class FrameAdjustButton : CommonButtonObject
{
	[SerializeField]
	private Image _topImage;

	[SerializeField]
	private Image _baseImage;

	private readonly Animator _animator;

	private bool isNonActive;

	public Animator Animator => _animator ?? GetComponent<Animator>();

	public void SetTopSprite(bool isNormal)
	{
		if (isNormal)
		{
			if (isNonActive)
			{
				Animator.SetTrigger("Loop");
				isNonActive = false;
			}
			_symbolImage.color = Color.white;
			_topImage.color = Color.white;
			_baseImage.color = Color.white;
		}
		else
		{
			if (base.gameObject.activeSelf && base.gameObject.activeInHierarchy && !isNonActive)
			{
				Animator.SetTrigger("NonActive");
				isNonActive = true;
			}
			_symbolImage.color = Color.gray;
			_topImage.color = Color.gray;
			_baseImage.color = Color.gray;
		}
	}
}
