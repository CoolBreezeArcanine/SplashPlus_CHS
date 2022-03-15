using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Mai2Data/MusicDifficultyTable", fileName = "ParameterTable")]
public class MusicDifficultySheet : ScriptableObject
{
	public const int LevelImageIndex = 14;

	[SerializeField]
	[Header("レベル画像")]
	private List<SpriteSheet> _musicLevelTexture = new List<SpriteSheet>(6);

	public List<SpriteSheet> MusicLevelSprites => _musicLevelTexture;
}
