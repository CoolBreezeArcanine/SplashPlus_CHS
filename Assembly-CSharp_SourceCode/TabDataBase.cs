using UnityEngine;

public class TabDataBase
{
	public Color BaseColor { get; }

	public Sprite TitleSprite { get; }

	public string Title { get; }

	public string SubTitle { get; }

	public TabDataBase()
	{
	}

	public TabDataBase(Color baseColor, Sprite sprite, string title, string subTitle = "")
	{
		BaseColor = baseColor;
		TitleSprite = sprite;
		Title = title;
		SubTitle = subTitle;
	}
}
