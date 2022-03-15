using UI;
using UnityEngine;

public class CollectionTypeParts : MonoBehaviour
{
	[SerializeField]
	[Header("コレクション種別のパーツ")]
	private CanvasGroup _canvasGroup;

	[SerializeField]
	[Header("コレクション名")]
	private MultipleImage _collectionAcquisionTitleImage;

	public void SetParts(int typeIndex)
	{
		if (_collectionAcquisionTitleImage != null)
		{
			_collectionAcquisionTitleImage.ChangeSprite(typeIndex);
		}
	}

	public void SetVisible(bool isVisible)
	{
		_canvasGroup.alpha = (isVisible ? 1 : 0);
	}
}
