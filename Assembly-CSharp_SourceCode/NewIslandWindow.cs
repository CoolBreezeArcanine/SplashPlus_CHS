using DB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewIslandWindow : EventWindowBase
{
	[SerializeField]
	private Image _islandImage;

	[SerializeField]
	private TextMeshProUGUI _message;

	[SerializeField]
	private TextMeshProUGUI _infoText;

	private GameObject _prefabNewIsland;

	private GameObject _objIsland;

	private void Awake()
	{
		_infoText.text = CommonMessageID.GetWindowIsland.GetName();
	}

	public void Set(Sprite uiIsland, string title)
	{
		_islandImage.sprite = uiIsland;
		_message.text = title;
	}
}
