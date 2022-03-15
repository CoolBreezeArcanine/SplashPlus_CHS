using System.Collections.Generic;
using UnityEngine;

public class CollectionTabController : TabController
{
	private CollectionTabData FavoriteStyle;

	[SerializeField]
	private Sprite _favoriteSprite;

	public override void Initialize(int monitorId)
	{
		base.Initialize(monitorId);
		FavoriteStyle = new CollectionTabData(CommonScriptable.GetColorSetting().FavoriteColor, _favoriteSprite, "お気に入りセット");
	}

	public void CollectionType2Collection(List<CollectionTabData> tabDatas, int categoryIndex)
	{
		Set(new List<TabDataBase>(tabDatas), categoryIndex);
	}

	public void Favorite2Collection(int categoryIndex)
	{
		SetActiveMiniPanels(categoryIndex);
	}

	public void Collection2Favorite()
	{
		SetOnlyMainPanel(FavoriteStyle);
	}
}
