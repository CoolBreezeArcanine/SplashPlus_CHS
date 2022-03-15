using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Mai2Data/AchieveNumTable", fileName = "ParameterTable")]
public class AchieveNumSheet : ScriptableObject
{
	public const int AchieveImageIndex = 14;

	public const int AchievePattern = 3;

	[SerializeField]
	[Header("達成率整数画像")]
	private List<SpriteSheet> _achieveIntSprite = new List<SpriteSheet>(3);

	[SerializeField]
	[Header("達成率画少数像")]
	private List<SpriteSheet> _achieveDecimalSprite = new List<SpriteSheet>(3);

	public List<SpriteSheet> AchieveIntSprites => _achieveIntSprite;

	public List<SpriteSheet> AchieveDecimalSprites => _achieveDecimalSprite;
}
