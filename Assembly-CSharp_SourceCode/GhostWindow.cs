using DB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GhostWindow : EventWindowBase
{
	[SerializeField]
	private Image _ghost;

	[SerializeField]
	private Image _jacket;

	[SerializeField]
	private TextMeshProUGUI _infoText;

	[SerializeField]
	private TextMeshProUGUI _titleText;

	private void Awake()
	{
		_titleText.text = CommonMessageID.GetWindowGhostTitle.GetName();
		_infoText.text = CommonMessageID.GetWindowGhost.GetName();
	}

	public void Set(Sprite ghost, Sprite jacket)
	{
		_ghost.sprite = ghost;
		_jacket.sprite = jacket;
	}
}
