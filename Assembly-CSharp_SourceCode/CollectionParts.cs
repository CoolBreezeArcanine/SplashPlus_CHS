using Manager;
using UnityEngine;
using UnityEngine.UI;

public class CollectionParts : MonoBehaviour
{
	[SerializeField]
	[Header("コレクションパーツ群")]
	private CanvasGroup _canvasGroup;

	[SerializeField]
	[Header("称号")]
	private CanvasGroup _titleObj;

	[SerializeField]
	private Image _titleBaseImage;

	[SerializeField]
	private CustomTextScroll _titleScrollText;

	[SerializeField]
	[Header("Icon")]
	private RawImage _iconImage;

	[SerializeField]
	[Header("Plate")]
	private CanvasGroup _plateObj;

	[SerializeField]
	private CustomImage _plateImage;

	[SerializeField]
	[Header("Partner")]
	private RawImage _partnerImage;

	[SerializeField]
	[Header("Frame")]
	private CanvasGroup _frameObj;

	[SerializeField]
	private RawImage _frameImage;

	[SerializeField]
	[Header("キラ属性")]
	private CanvasGroup _setSpecialObj;

	public void SetIconTexture(Texture2D icon)
	{
		if (_iconImage != null)
		{
			_iconImage.texture = icon;
		}
	}

	public void SetPlateSprite(Sprite plate)
	{
		if (_plateImage != null)
		{
			_plateImage.sprite = plate;
		}
	}

	public void SetTitle(string title, Sprite titleBaseSprite)
	{
		if (_titleScrollText != null)
		{
			_titleScrollText.SetData(title);
			_titleScrollText.ResetPosition();
			_titleBaseImage.sprite = titleBaseSprite;
		}
	}

	public void SetPartnerTexture(Texture2D partner)
	{
		if (_partnerImage != null)
		{
			_partnerImage.texture = partner;
		}
	}

	public void SetFrameTexture(Texture2D frame)
	{
		if (_frameImage != null)
		{
			_frameImage.texture = frame;
		}
	}

	public void SetVisibleCollectionParts(CollectionGenreID type, bool isSpecial)
	{
		if (_setSpecialObj != null)
		{
			_setSpecialObj.alpha = 0f;
		}
		switch (type)
		{
		case CollectionGenreID.Title:
			_frameImage.gameObject.SetActive(value: false);
			_iconImage.gameObject.SetActive(value: false);
			_partnerImage.gameObject.SetActive(value: false);
			_plateObj.alpha = 0f;
			_titleObj.alpha = 1f;
			_frameObj.alpha = 0f;
			break;
		case CollectionGenreID.Icon:
			_frameImage.gameObject.SetActive(value: false);
			_plateObj.alpha = 0f;
			_titleObj.alpha = 0f;
			_frameObj.alpha = 0f;
			_partnerImage.gameObject.SetActive(value: false);
			_iconImage.gameObject.SetActive(value: true);
			break;
		case CollectionGenreID.Plate:
			_frameImage.gameObject.SetActive(value: false);
			_iconImage.gameObject.SetActive(value: false);
			_partnerImage.gameObject.SetActive(value: false);
			_titleObj.alpha = 0f;
			_plateObj.alpha = 1f;
			_frameObj.alpha = 0f;
			break;
		case CollectionGenreID.Partner:
			_frameImage.gameObject.SetActive(value: false);
			_iconImage.gameObject.SetActive(value: false);
			_titleObj.alpha = 0f;
			_plateObj.alpha = 0f;
			_frameObj.alpha = 0f;
			_partnerImage.gameObject.SetActive(value: true);
			break;
		case CollectionGenreID.Frame:
			_frameImage.gameObject.SetActive(value: true);
			_partnerImage.gameObject.SetActive(value: false);
			_titleObj.alpha = 0f;
			_plateObj.alpha = 0f;
			_frameObj.alpha = 1f;
			_iconImage.gameObject.SetActive(value: false);
			if (_setSpecialObj != null && isSpecial)
			{
				_setSpecialObj.alpha = 1f;
				_setSpecialObj.gameObject.SetActive(value: true);
			}
			break;
		case CollectionGenreID.Exit:
			_frameImage.gameObject.SetActive(value: false);
			_plateObj.alpha = 0f;
			_titleObj.alpha = 0f;
			_frameObj.alpha = 0f;
			_partnerImage.gameObject.SetActive(value: false);
			_iconImage.gameObject.SetActive(value: true);
			break;
		}
	}

	public void UpdateView(float syncTimer)
	{
		if (_titleScrollText != null)
		{
			_titleScrollText.ViewUpdate();
		}
	}

	public void ResetScrollText()
	{
		if (_titleObj.alpha != 0f)
		{
			_titleScrollText.ResetPosition();
		}
	}

	public bool IsActive()
	{
		return _canvasGroup.alpha != 0f;
	}

	public void SetVisible(bool isVisible)
	{
		_canvasGroup.alpha = (isVisible ? 1 : 0);
	}
}
