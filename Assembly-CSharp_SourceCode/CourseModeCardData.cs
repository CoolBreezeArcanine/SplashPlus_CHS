using UnityEngine;

public class CourseModeCardData
{
	public Sprite _sprite;

	public int _courseMode;

	public CourseModeCardData()
	{
	}

	public CourseModeCardData(Sprite sprite, int courseMode, Color color)
	{
		_sprite = sprite;
		_courseMode = courseMode;
	}
}
