using UI;
using UnityEngine;

public class TreasurePinObject : MonoBehaviour
{
	public enum PinType
	{
		Start,
		Normal,
		Goal
	}

	[SerializeField]
	private MultipleImage _frameImage;

	[SerializeField]
	private MultipleImage _treasureImage;

	[SerializeField]
	private GameObject _goalObject;

	[SerializeField]
	private GameObject _startObject;

	[SerializeField]
	private CanvasGroup _group;

	private RectTransform _rectTransform;

	public CanvasGroup CanvasGroup => _group;

	public void SetData(PinType pinType, int frameType, int trueasureType)
	{
		_group.alpha = 1f;
		_frameImage.ChangeSprite(frameType);
		_treasureImage.ChangeSprite(trueasureType);
		_goalObject.SetActive(value: false);
		_startObject.SetActive(value: false);
		switch (pinType)
		{
		case PinType.Goal:
			_goalObject.SetActive(value: true);
			break;
		case PinType.Start:
			_startObject.SetActive(value: true);
			break;
		}
	}

	public void SetPosition(int x)
	{
		if (_rectTransform == null)
		{
			_rectTransform = GetComponent<RectTransform>();
		}
		_rectTransform.anchoredPosition = new Vector2(x, _rectTransform.anchoredPosition.y);
	}

	public void SetPinType(PinType type)
	{
		_goalObject.SetActive(value: false);
		_startObject.SetActive(value: false);
		switch (type)
		{
		case PinType.Goal:
			_goalObject.SetActive(value: true);
			break;
		case PinType.Start:
			_startObject.SetActive(value: true);
			break;
		}
	}
}
