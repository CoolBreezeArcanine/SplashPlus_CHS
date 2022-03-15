using System.Collections.Generic;
using UnityEngine;

public class GenreSelectController : TabController
{
	[SerializeField]
	private Sprite _favoriteSprite;

	public override void Initialize(int monitorId)
	{
		base.Initialize(monitorId);
	}

	public void SortType2Genre(List<TabDataBase> tabDatas, int categoryIndex)
	{
		Set(new List<TabDataBase>(tabDatas), categoryIndex);
	}

	public void Favorite2Collection(int categoryIndex)
	{
		SetActiveMiniPanels(categoryIndex);
	}

	public void Collection2Favorite()
	{
	}
}
