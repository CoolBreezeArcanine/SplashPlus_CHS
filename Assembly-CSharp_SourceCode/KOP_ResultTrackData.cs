using MAI2System;
using Manager;
using Timeline;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class KOP_ResultTrackData : MonoBehaviour
{
	[SerializeField]
	private GameObject _resultDataObject;

	[SerializeField]
	private GameObject _trackDataObject;

	[SerializeField]
	[Header("楽曲情報")]
	private TextMeshProUGUI _musicName;

	[SerializeField]
	private RawImage _jacketImage;

	[SerializeField]
	private MultipleImage _gameTypeImage;

	[SerializeField]
	private MultipleImage[] _trackNoImages;

	[SerializeField]
	[Header("難易度")]
	private MultipleImage _difficultyImage;

	[SerializeField]
	private SpriteCounter _singleLevel;

	[SerializeField]
	private SpriteCounter _doubleLevel;

	[SerializeField]
	private Image _levelTextImage;

	[SerializeField]
	[Header("達成率")]
	private AchievementCounterObject _achievementCounter;

	[SerializeField]
	[Header("でらっくすスコア")]
	private Image[] _deluxeScoreImages;

	[SerializeField]
	private Sprite[] _numSprites;

	public void SetDisplay(bool isResultOpen)
	{
		_resultDataObject.SetActive(isResultOpen);
		_trackDataObject.SetActive(!isResultOpen);
	}

	public void SetMusicData(int trackNumber, string musicName, ConstParameter.ScoreKind kind, Texture2D jacket)
	{
		_trackNoImages[0].ChangeSprite(trackNumber);
		_musicName.text = musicName;
		_jacketImage.texture = jacket;
		_gameTypeImage.ChangeSprite((int)kind);
	}

	public void SetAchievementData(uint achievement)
	{
		_achievementCounter.SetAchievement(0u, achievement);
		_achievementCounter.OnClipTailEnd();
	}

	public void SetDeluxeScore(int score)
	{
		if (score > 0)
		{
			int num = (int)Mathf.Log10(score) + 1;
			int num2 = score;
			for (int i = 0; i < _deluxeScoreImages.Length; i++)
			{
				if (i < num)
				{
					_deluxeScoreImages[i].gameObject.SetActive(value: true);
					int num3 = num2 % 10;
					num2 /= 10;
					_deluxeScoreImages[i].sprite = _numSprites[num3];
				}
				else
				{
					_deluxeScoreImages[i].gameObject.SetActive(value: false);
				}
			}
		}
		else
		{
			_deluxeScoreImages[0].gameObject.SetActive(value: true);
			for (int j = 1; j < _deluxeScoreImages.Length; j++)
			{
				_deluxeScoreImages[j].gameObject.SetActive(value: false);
			}
		}
	}

	public void SetDifficultyLevel(int level, MusicLevelID levelId, int difficulty)
	{
		_difficultyImage.ChangeSprite(difficulty);
		Sprite[] musicLevelSprites = CommonPrefab.GetMusicLevelSprites(difficulty);
		_singleLevel.SetSpriteSheet(musicLevelSprites);
		_doubleLevel.SetSpriteSheet(musicLevelSprites);
		_levelTextImage.sprite = musicLevelSprites[14];
		if (level > 9)
		{
			_singleLevel.gameObject.SetActive(value: false);
			_doubleLevel.gameObject.SetActive(value: true);
			_doubleLevel.ChangeText(levelId.GetLevelNum().PadRight(3));
		}
		else
		{
			_singleLevel.gameObject.SetActive(value: true);
			_doubleLevel.gameObject.SetActive(value: false);
			_singleLevel.ChangeText(levelId.GetLevelNum().PadRight(2));
		}
	}
}
