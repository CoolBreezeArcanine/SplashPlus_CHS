using System;
using UnityEngine;
using UnityEngine.UI;

public class NamePlateWindow : EventWindowBase
{
	private readonly int _hashIn = Animator.StringToHash("In_NamePlate");

	[SerializeField]
	private Image _namePlateImage;

	public override void Play(Action onAction)
	{
		_animator.Play(_hashIn, 0, 0f);
		IsCanSkip = true;
	}

	public override bool Skip()
	{
		if (IsCanSkip && _animator.GetCurrentAnimatorStateInfo(0).fullPathHash == _hashIn)
		{
			_animator.Play(HashSkip, 0, 0f);
			IsCanSkip = false;
			return true;
		}
		return false;
	}

	public void Set(Sprite namePlateSprite)
	{
		_namePlateImage.sprite = namePlateSprite;
	}
}
