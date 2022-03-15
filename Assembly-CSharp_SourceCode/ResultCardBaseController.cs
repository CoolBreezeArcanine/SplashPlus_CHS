using System.Collections.Generic;
using DB;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class ResultCardBaseController : MonoBehaviour
{
	[SerializeField]
	[Header("楽曲名")]
	private TextMeshProUGUI _musicNameText;

	[SerializeField]
	[Header("撮影日")]
	private TextMeshProUGUI _dateTimeText;

	[SerializeField]
	[Header("店舗名")]
	private TextMeshProUGUI _storeNameText;

	[SerializeField]
	[Header("スタンプ")]
	private MultipleImage _stampImage;

	[SerializeField]
	[Header("写真")]
	private RawImage _photoImage;

	private CanvasGroup _canvasGroup;

	private Animator _stampAnimator;

	private readonly List<Image> _maskList = new List<Image>();

	public virtual void Initialize()
	{
		_canvasGroup = GetComponent<CanvasGroup>();
		_stampAnimator = _stampImage.GetComponent<Animator>();
		_stampImage.gameObject.SetActive(value: false);
	}

	public virtual void ViewUpdate()
	{
	}

	public void SetFaceMask(Rect[] faces, Sprite mask)
	{
		if (_maskList.Count > 0)
		{
			foreach (Image mask2 in _maskList)
			{
				Object.Destroy(mask2.gameObject);
			}
			_maskList.Clear();
		}
		if (faces != null)
		{
			for (int i = 0; i < faces.Length; i++)
			{
				Rect rect = faces[i];
				GameObject obj = new GameObject($"{rect.x}:{rect.y}", typeof(Image));
				obj.name = $"FaceMask{rect.x}";
				obj.tag = _photoImage.tag;
				obj.layer = _photoImage.gameObject.layer;
				obj.transform.SetParent(_photoImage.transform);
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localRotation = Quaternion.identity;
				obj.transform.localScale = Vector3.one;
				RectTransform component = obj.GetComponent<RectTransform>();
				component.anchoredPosition = new Vector2(rect.x, rect.y);
				component.sizeDelta = new Vector2(rect.width * 1.5f, rect.height * 1.5f);
				Image component2 = obj.GetComponent<Image>();
				component2.sprite = mask;
				_maskList.Add(component2);
			}
		}
	}

	public void RemoveMasks()
	{
		if (_maskList.Count <= 0)
		{
			return;
		}
		foreach (Image mask in _maskList)
		{
			mask.gameObject.SetActive(value: false);
		}
	}

	public void SetVisible(bool isVisible)
	{
		_canvasGroup.alpha = (isVisible ? 1 : 0);
	}

	public void SetBasicData(string storeName, string dateTime)
	{
		_storeNameText.text = storeName;
		_dateTimeText.text = dateTime;
	}

	public void SetMusicTitle(string musicTitle)
	{
		_musicNameText.text = musicTitle;
	}

	public void SetPhotoData(Texture2D texture, int shiftPosition)
	{
		_photoImage.texture = texture;
		_photoImage.rectTransform.anchoredPosition = new Vector2(shiftPosition, 0f);
	}

	public void SetVisibleStoreName(bool isVisible)
	{
		_storeNameText.transform.parent.gameObject.SetActive(isVisible);
	}

	public void SetVisibleShootingDate(bool isVisible)
	{
		_dateTimeText.transform.parent.gameObject.SetActive(isVisible);
	}

	public void ChangeStamp(PhotoeditStampID stampIndex)
	{
		_stampImage.ChangeSprite((int)(stampIndex - 1));
		_stampAnimator.Play(Animator.StringToHash("In"), 0, 0f);
	}

	public void SetVisibleStamp(bool isVisible)
	{
		_stampImage.gameObject.SetActive(isVisible);
	}

	public virtual void SetDisable()
	{
	}
}
