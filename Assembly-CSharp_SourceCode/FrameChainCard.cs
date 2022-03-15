using UI.DaisyChainList;
using UnityEngine;

public class FrameChainCard : ChainObject
{
	[SerializeField]
	[Header("フレームイメージ")]
	private CustomImage _image;

	[SerializeField]
	private CanvasGroup _miniCardGroup;

	[SerializeField]
	private CanvasGroup _mainCardGroup;

	[SerializeField]
	private FrameChainCard _miniCard;

	public void Prepare(Sprite sprite)
	{
		_image.sprite = sprite;
		if (_miniCard != null)
		{
			_miniCard.Prepare(sprite);
		}
	}

	public void ChangeSize(bool isMainActive)
	{
		if (_miniCard != null)
		{
			_miniCardGroup.alpha = ((!isMainActive) ? 1 : 0);
			_mainCardGroup.alpha = (isMainActive ? 1 : 0);
		}
	}

	public override void OnCenterIn()
	{
		ChangeSize(isMainActive: true);
	}

	public override void OnCenterOut()
	{
		ChangeSize(isMainActive: false);
	}
}
