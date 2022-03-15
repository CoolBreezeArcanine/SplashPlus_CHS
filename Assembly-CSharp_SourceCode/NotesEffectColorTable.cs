using UnityEngine;

[CreateAssetMenu(menuName = "Mai2Data/NotesColorTable", fileName = "ParameterTable")]
public class NotesEffectColorTable : ScriptableObject
{
	[SerializeField]
	[Header("ノーツエフェクト色")]
	private Color tap_color;

	[SerializeField]
	private Color each_color;

	[SerializeField]
	private Color star_color;

	public Color TapColor
	{
		get
		{
			return tap_color;
		}
		private set
		{
		}
	}

	public Color EachColor
	{
		get
		{
			return each_color;
		}
		private set
		{
		}
	}

	public Color StarColor
	{
		get
		{
			return star_color;
		}
		private set
		{
		}
	}
}
