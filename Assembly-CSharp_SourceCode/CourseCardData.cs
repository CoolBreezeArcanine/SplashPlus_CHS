using Manager;
using UnityEngine;

public class CourseCardData
{
	public Sprite _sprite;

	public int _courseMode;

	public MusicLevelID _level;

	public int _life;

	public int _recover;

	public int _greatDamage;

	public int _goodDamage;

	public int _missDamage;

	public int _achievement;

	public int _restLife;

	public bool _isPlay;

	public CourseCardData()
	{
	}

	public CourseCardData(Sprite sprite, int courseMode, MusicLevelID level, int life, int recover, int greatDamage, int goodDamage, int missDamage, int achievement, int restLife, bool isPlay)
	{
		_sprite = sprite;
		_courseMode = courseMode;
		_level = level;
		_life = life;
		_recover = recover;
		_greatDamage = greatDamage;
		_goodDamage = goodDamage;
		_missDamage = missDamage;
		_achievement = achievement;
		_restLife = restLife;
		_isPlay = isPlay;
	}
}
