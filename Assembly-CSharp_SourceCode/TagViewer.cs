using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TagViewer : MonoBehaviour
{
	[SerializeField]
	private CustomDropDown _dropDown;

	[SerializeField]
	private Button _insertTButton;

	[SerializeField]
	private Button _insertMButton;

	private List<TagTip> _tips = new List<TagTip>();

	private IFontOutViewer _fontout;

	private IColorViewer _color;

	public void Prepare(List<TagTip> tips, Action<string> insertTitle, Action<string> insertMessage, IFontOutViewer fontout, IColorViewer color)
	{
		_tips = tips;
		_fontout = fontout;
		_color = color;
		List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
		for (int i = 0; i < tips.Count; i++)
		{
			list.Add(new TMP_Dropdown.OptionData(tips[i].Name));
		}
		_dropDown.Prepare("タグ一覧", list);
		_insertTButton.onClick.RemoveAllListeners();
		_insertMButton.onClick.RemoveAllListeners();
		_insertTButton.onClick.AddListener(delegate
		{
			insertTitle(CreateMessage());
		});
		_insertMButton.onClick.AddListener(delegate
		{
			insertMessage(CreateMessage());
		});
	}

	private string CreateMessage()
	{
		TagTip tip = GetTip(_dropDown.GetCurrentOption().text);
		string text = "";
		if (tip.Category == TagTip.TagCategory.Picto)
		{
			return tip.OpeningTag.Replace("=", "=" + _fontout.GetName());
		}
		if (tip.Category == TagTip.TagCategory.Color)
		{
			return tip.OpeningTag + _color.GetColorCode() + tip.ClosingTag;
		}
		return tip.ActualTag();
	}

	private TagTip GetTip(string name)
	{
		return _tips.FirstOrDefault((TagTip tip) => tip.Name == name);
	}
}
