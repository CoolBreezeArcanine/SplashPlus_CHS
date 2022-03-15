using UnityEngine;
using UnityEngine.UI;

public class RegionalListNode : MonoBehaviour
{
	public enum RegionNodeState
	{
		Open,
		Lock,
		Complete
	}

	[SerializeField]
	private RectTransform _mainNode;

	[SerializeField]
	private RectTransform _separatorNode;

	[SerializeField]
	[Header("地方オブジェクト")]
	private Image _baseImage;

	[SerializeField]
	private Image _islandImage;

	[SerializeField]
	private Image _collaboImage;

	[SerializeField]
	private CustomTextScroll _textScroll;

	[SerializeField]
	[Header("表示切替オブジェクト")]
	private CanvasRenderer _lockIconObject;

	[SerializeField]
	private CanvasRenderer _clearFrameObject;

	[SerializeField]
	private CanvasRenderer _frameObject;

	private CanvasGroup _miniNodeGroup;

	private CanvasGroup _mainNodeGroup;

	private CanvasGroup _separatorGroup;

	private RectTransform _rectTransform;

	private LayoutElement _element;

	public Vector2 GetAnchoredPosition()
	{
		return _rectTransform?.anchoredPosition ?? new Vector2(0f, 0f);
	}

	public void Initialize()
	{
		_rectTransform = GetComponent<RectTransform>();
		_element = GetComponent<LayoutElement>();
		_mainNodeGroup = _mainNode.GetComponent<CanvasGroup>();
		_separatorGroup = _separatorNode.GetComponent<CanvasGroup>();
		_separatorGroup.alpha = 0f;
		SetNodeType(isMain: false);
	}

	public void SetData(RegionNodeState state, string mapName, Sprite baseSprite, Sprite islandSprite, Sprite collaboSprite = null)
	{
		for (int i = 0; i < 2; i++)
		{
			_baseImage.sprite = baseSprite;
			_islandImage.sprite = islandSprite;
			_textScroll.SetData(mapName);
			_textScroll.ResetPosition();
			if (collaboSprite != null)
			{
				_collaboImage.color = Color.white;
				_collaboImage.sprite = collaboSprite;
			}
			else
			{
				_collaboImage.color = Color.clear;
			}
			switch (state)
			{
			case RegionNodeState.Open:
				_frameObject.SetAlpha(1f);
				_lockIconObject.SetAlpha(0f);
				_clearFrameObject.SetAlpha(0f);
				break;
			case RegionNodeState.Lock:
				_frameObject.SetAlpha(0f);
				_lockIconObject.SetAlpha(1f);
				_clearFrameObject.SetAlpha(0f);
				break;
			case RegionNodeState.Complete:
				_frameObject.SetAlpha(0f);
				_lockIconObject.SetAlpha(0f);
				_clearFrameObject.SetAlpha(1f);
				break;
			}
		}
	}

	public void SetNodeType(bool isMain)
	{
		_element.preferredWidth = _mainNode.sizeDelta.x;
		_rectTransform.sizeDelta = _mainNode.sizeDelta;
		_mainNodeGroup.alpha = 1f;
		if (isMain)
		{
			_mainNode.localScale = Vector3.one;
		}
		else
		{
			_mainNode.localScale = new Vector3(0.7f, 0.7f, 0.7f);
		}
	}

	public void InitializeSeparator()
	{
		_mainNodeGroup.alpha = 0f;
		_separatorGroup.alpha = 1f;
		_element.preferredWidth = 550f;
		_rectTransform.sizeDelta = new Vector2(550f, _separatorNode.sizeDelta.y);
	}

	public void ViewUpdate()
	{
		for (int i = 0; i < 2; i++)
		{
			_textScroll.ViewUpdate();
		}
	}
}
