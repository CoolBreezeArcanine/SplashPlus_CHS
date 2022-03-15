using Manager;
using TMPro;
using UnityEngine;

public class CollectionName : MonoBehaviour
{
	[SerializeField]
	private GameObject _offTitleObj;

	[SerializeField]
	private TextMeshProUGUI _captionMiddleText;

	[SerializeField]
	private CanvasGroup _splitSet;

	[SerializeField]
	private CustomTextScroll _collectionNameText;

	[SerializeField]
	private CustomTextScroll _acquitionText;

	public void Prepare(CollectionGenreID type, string collectionName, string acquitionText)
	{
		SetVisibleSplitBg(type == CollectionGenreID.Title || type == CollectionGenreID.Exit);
		SetVisibleSplitSet(type != 0 && type != CollectionGenreID.Exit);
		if (type == CollectionGenreID.Title || type == CollectionGenreID.Exit)
		{
			SetCaptionMiddleText(acquitionText);
			SetCollectionName(acquitionText);
		}
		else
		{
			SetCollectionName(collectionName);
			SetAcquitionText(acquitionText);
		}
	}

	private void SetVisibleSplitBg(bool isActive)
	{
		if (!(_offTitleObj == null))
		{
			_offTitleObj.SetActive(isActive);
		}
	}

	private void SetVisibleSplitSet(bool isActive)
	{
		if (_splitSet != null)
		{
			_splitSet.alpha = (isActive ? 1 : 0);
		}
	}

	private void SetCollectionName(string collectionName)
	{
		if (!(_collectionNameText == null))
		{
			_collectionNameText.SetData(collectionName);
			_collectionNameText.ResetPosition();
		}
	}

	private void SetCaptionMiddleText(string conditionText)
	{
		if (!(_captionMiddleText == null))
		{
			_captionMiddleText.text = conditionText;
		}
	}

	private void SetAcquitionText(string conditionText)
	{
		if (!(_acquitionText == null))
		{
			_acquitionText.SetData(conditionText);
			_acquitionText.ResetPosition();
		}
	}

	public void ViewUpdate()
	{
		if (_collectionNameText != null)
		{
			_collectionNameText.ViewUpdate();
		}
		if (_acquitionText != null)
		{
			_acquitionText.ViewUpdate();
		}
	}

	public void ResetPosition()
	{
		if (_collectionNameText != null)
		{
			_collectionNameText.ResetPosition();
		}
		if (_acquitionText != null)
		{
			_acquitionText.ResetPosition();
		}
	}
}
