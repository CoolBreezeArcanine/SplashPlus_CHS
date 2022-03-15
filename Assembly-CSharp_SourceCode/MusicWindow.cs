using DB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicWindow : EventWindowBase
{
	[SerializeField]
	private Image _jacket;

	[SerializeField]
	private TextMeshProUGUI _musicName;

	[SerializeField]
	private TextMeshProUGUI _infoText;

	private void Awake()
	{
		_infoText.text = CommonMessageID.GetWindowMusic.GetName();
	}

	public void Set(Sprite jacket, string musicName)
	{
		_jacket.sprite = jacket;
		_musicName.text = musicName;
	}

	public void SetInfoText(string infoText)
	{
		_infoText.text = infoText;
	}
}
