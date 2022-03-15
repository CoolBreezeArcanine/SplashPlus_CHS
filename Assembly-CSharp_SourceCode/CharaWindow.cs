using DB;
using MAI2.Util;
using Manager;
using Manager.MaiStudio;
using TMPro;
using UI;
using UnityEngine;

public class CharaWindow : EventWindowBase
{
	[SerializeField]
	private MultiImage _chara;

	[SerializeField]
	private MultiImage _shadow;

	[SerializeField]
	private TextMeshProUGUI _charaName;

	[SerializeField]
	private TextMeshProUGUI _infoText;

	private void Awake()
	{
		_infoText.text = CommonMessageID.GetWindowChara.GetName();
	}

	public void Set(Sprite chara, string charaName, int id)
	{
		int id2 = Singleton<DataManager>.Instance.GetChara(id).color.id;
		Color24 colorDark = Singleton<DataManager>.Instance.GetMapColorData(id2).ColorDark;
		Color color = new Color((float)(int)colorDark.R / 255f, (float)(int)colorDark.G / 255f, (float)(int)colorDark.B / 255f);
		_chara.sprite = chara;
		_shadow.Image2 = chara;
		_shadow.color = color;
		_charaName.text = charaName;
	}
}
