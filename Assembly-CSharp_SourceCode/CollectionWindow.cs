using DB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionWindow : EventWindowBase
{
	[SerializeField]
	private GameObject _namePlateRoot;

	[SerializeField]
	private GameObject _frameRoot;

	[SerializeField]
	private GameObject _iconRoot;

	[SerializeField]
	private Image _namePlate;

	[SerializeField]
	private Image _frame;

	[SerializeField]
	private Image _icon;

	[SerializeField]
	private TextMeshProUGUI _itemName;

	[SerializeField]
	private TextMeshProUGUI _infoText;

	private void Awake()
	{
		_infoText.text = CommonMessageID.GetWindowCollection.GetName();
	}

	public void SetNamePlate(Sprite collectSprite, string itemName)
	{
		_namePlateRoot.SetActive(value: true);
		_namePlate.sprite = collectSprite;
		_itemName.text = itemName;
	}

	public void SetFrame(Sprite collectSprite, string itemName)
	{
		_frameRoot.SetActive(value: true);
		_frame.sprite = collectSprite;
		_itemName.text = itemName;
	}

	public void SetIcon(Sprite collectSprite, string itemName)
	{
		_iconRoot.SetActive(value: true);
		_icon.sprite = collectSprite;
		_itemName.text = itemName;
	}
}
