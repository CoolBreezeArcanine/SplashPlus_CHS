using System.Collections;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class CustomTextScroll : MonoBehaviour
{
	private enum ScrollState
	{
		Wait,
		Scroll
	}

	[SerializeField]
	[Header("長さに関係なくスクロールさせる")]
	private bool forceScroll;

	[SerializeField]
	private float scrollSpeed = 1f;

	[SerializeField]
	private float scrollWaitTimeMSec = 1000f;

	[SerializeField]
	private RectTransform scrollParent;

	[SerializeField]
	[Range(-1f, 1f)]
	private int Direction = -1;

	[SerializeField]
	private RectTransform maskRectTransform;

	private TextMeshProUGUI[] meshText;

	private Image[] _images;

	private ScrollState state;

	private HorizontalOrVerticalLayoutGroup layout;

	private ContentSizeFitter _contentSize;

	private float width;

	private float timer;

	private bool isScroll;

	private bool initialized;

	private float _spacing;

	protected void Awake()
	{
		Initialize();
	}

	protected void Initialize()
	{
		if (initialized)
		{
			return;
		}
		if (maskRectTransform == null)
		{
			maskRectTransform = GetComponent<RectTransform>();
		}
		if (scrollParent != null)
		{
			if (layout == null)
			{
				layout = scrollParent.GetComponent<HorizontalOrVerticalLayoutGroup>();
			}
			layout.enabled = true;
			if (_contentSize == null)
			{
				_contentSize = scrollParent.GetComponent<ContentSizeFitter>();
			}
			_contentSize.enabled = true;
			meshText = scrollParent.GetComponentsInChildren<TextMeshProUGUI>(includeInactive: true);
			_images = scrollParent.GetComponentsInChildren<Image>(includeInactive: true);
			_spacing = layout.spacing;
		}
		if (forceScroll)
		{
			isScroll = forceScroll;
		}
		initialized = true;
	}

	public void SetData(string text)
	{
		Initialize();
		if (_contentSize != null && !_contentSize.enabled)
		{
			_contentSize.enabled = true;
		}
		if (layout != null && !layout.enabled)
		{
			layout.enabled = true;
		}
		if (meshText != null && meshText.Length != 0)
		{
			meshText[0].text = text;
			width = meshText[0].preferredWidth;
			isScroll = width >= maskRectTransform.sizeDelta.x;
			if (!isScroll)
			{
				width = maskRectTransform.sizeDelta.x;
			}
			Vector2 sizeDelta = new Vector2(width, maskRectTransform.sizeDelta.y);
			for (int i = 0; i < meshText.Length; i++)
			{
				meshText[i].text = text;
				meshText[i].rectTransform.sizeDelta = sizeDelta;
			}
		}
		else if (_images != null && _images.Length != 0)
		{
			width = _images[0].preferredWidth;
			isScroll = width * 2f >= maskRectTransform.sizeDelta.x;
			if (!isScroll)
			{
				width = maskRectTransform.sizeDelta.x;
			}
		}
		if (forceScroll)
		{
			isScroll = forceScroll;
		}
		if (layout != null)
		{
			layout.CalculateLayoutInputHorizontal();
			layout.SetLayoutHorizontal();
		}
		if (base.isActiveAndEnabled && base.gameObject.activeInHierarchy)
		{
			StartCoroutine(WaitEnable());
		}
	}

	private IEnumerator WaitEnable()
	{
		yield return null;
		yield return new WaitForEndOfFrame();
		if (layout != null && layout.enabled)
		{
			if (_contentSize != null)
			{
				_contentSize.enabled = false;
			}
			layout.enabled = false;
		}
	}

	public void ResetPosition()
	{
		timer = 0f;
		state = ScrollState.Wait;
		scrollParent.anchoredPosition = Vector2.zero;
	}

	public void ViewUpdate()
	{
		if (!isScroll)
		{
			return;
		}
		timer += GameManager.GetGameMSecAdd();
		if (state == ScrollState.Wait && timer >= scrollWaitTimeMSec)
		{
			timer = 0f;
			state = ScrollState.Scroll;
		}
		else
		{
			if (state != ScrollState.Scroll)
			{
				return;
			}
			scrollParent.anchoredPosition += new Vector2(scrollSpeed, 0f) * Direction;
			if (Direction < 0)
			{
				if (scrollParent.anchoredPosition.x <= 0f - (width + _spacing))
				{
					scrollParent.anchoredPosition = Vector2.zero;
					timer = 0f;
					state = ScrollState.Wait;
				}
			}
			else if (scrollParent.anchoredPosition.x >= width + _spacing)
			{
				scrollParent.anchoredPosition = Vector2.zero;
				timer = 0f;
				state = ScrollState.Wait;
			}
		}
	}
}
