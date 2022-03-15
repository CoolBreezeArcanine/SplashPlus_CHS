using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FontOutViewer : MonoBehaviour, IFontOutViewer
{
	private struct SpriteInfo
	{
		public Sprite Sp;

		public string Name;
	}

	[SerializeField]
	private CustomDropDown _dropDown;

	[SerializeField]
	private Image _view;

	[SerializeField]
	private TMP_SpriteAsset _asset;

	private List<SpriteInfo> _spriteInfos;

	private void Awake()
	{
		_spriteInfos = new List<SpriteInfo>();
		List<Sprite> source = Resources.LoadAll<Sprite>("TextView/END_SPACE_v100").ToList();
		List<TMP_Sprite> infos = _asset.spriteInfoList;
		int i;
		for (i = 0; i < infos.Count; i++)
		{
			Sprite sp = source.FirstOrDefault((Sprite sprite) => sprite.name == infos[i].sprite.name);
			_spriteInfos.Add(new SpriteInfo
			{
				Name = infos[i].name,
				Sp = sp
			});
		}
		List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
		for (int j = 0; j < _spriteInfos.Count; j++)
		{
			list.Add(new TMP_Dropdown.OptionData(_spriteInfos[j].Name));
		}
		_dropDown.Prepare("外字フォント一覧", list);
		ChangeSprite(0);
		_dropDown.AddListener(ChangeSprite);
	}

	private void ChangeSprite(int index)
	{
		_view.sprite = _spriteInfos[index].Sp;
	}

	public string GetName()
	{
		return _dropDown.GetCurrentOption().text;
	}
}
