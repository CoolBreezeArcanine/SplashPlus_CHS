using System.Collections.Generic;
using UnityEngine;

public class CharacterTabController : TabController
{
	private TabDataBase _favoriteStyle;

	[SerializeField]
	private Sprite _favoriteSprite;

	public override void Initialize(int monitorId)
	{
		base.Initialize(monitorId);
		_favoriteStyle = new TabDataBase(CommonScriptable.GetColorSetting().FavoriteColor, _favoriteSprite, "お気に入りセット");
	}

	public void SlotView2CharacterSelect(List<CharacterTabData> tabDatas, int categoryIndex)
	{
		Set(new List<TabDataBase>(tabDatas), categoryIndex);
	}

	public void Favorite2Collection(int categoryIndex)
	{
		SetActiveMiniPanels(categoryIndex);
	}

	public void Collection2Favorite()
	{
		SetOnlyMainPanel(_favoriteStyle);
	}
}
