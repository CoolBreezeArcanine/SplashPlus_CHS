using System;
using DB;
using MAI2System;
using Manager;
using Manager.UserDatas;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class TotalResultPlayer
{
	[SerializeField]
	[Header("クリアランク")]
	private MultipleImage _clearRankImage;

	[SerializeField]
	[Header("ユーザー情報")]
	private UserInformationController _userinfomation;

	[SerializeField]
	[Header("でらっくすこあ")]
	private SpriteCounter _dxScore;

	[SerializeField]
	[Header("個別達成率")]
	private TextMeshProUGUI _achivementText;

	[SerializeField]
	[Header("コンボミニ称号")]
	private MultipleImage _comboMiniImage;

	[SerializeField]
	[Header("シンクミニ称号")]
	private MultipleImage _syncMiniImage;

	[SerializeField]
	[Header("難易度")]
	private MultipleImage _difficultyMiniImage;

	[SerializeField]
	[Header("難易度レベル表示")]
	private RectTransform _levelObject;

	[SerializeField]
	private GameObject[] _levelBubbleObjects;

	[SerializeField]
	[Header("レベル")]
	private SpriteCounter _difficultySingle;

	[SerializeField]
	private SpriteCounter _difficultyDouble;

	[SerializeField]
	private Image _levelTextImage;

	[SerializeField]
	[Header("キャラクター")]
	private Image _characterImage;

	[SerializeField]
	[Header("譜面型表示")]
	private MultipleImage _gameTypeImage;

	[SerializeField]
	[Header("パーフェクトチャレンジ")]
	private GameObject _perfectChallengeObject;

	[SerializeField]
	private SpriteCounter _lifeCounter;

	[SerializeField]
	private GameObject _perfectChallengeClearObject;

	public void ViewUpdate()
	{
		_userinfomation.UpdateTextScroll();
	}

	public void SetCharacer(Sprite character)
	{
		_characterImage.sprite = character;
	}

	public void SetVisibleCharacter(bool isVisible)
	{
		_characterImage.gameObject.SetActive(isVisible);
	}

	public void SetClearRank(MusicClearrankID rank)
	{
		_clearRankImage?.ChangeSprite((int)rank);
	}

	public void SetMedal(PlayComboflagID comboType, PlaySyncflagID syncType)
	{
		if (comboType > PlayComboflagID.None)
		{
			_comboMiniImage.gameObject.SetActive(value: true);
			_comboMiniImage.ChangeSprite((int)(comboType - 1));
		}
		else
		{
			_comboMiniImage.gameObject.SetActive(value: false);
		}
		if (syncType > PlaySyncflagID.None)
		{
			_syncMiniImage.gameObject.SetActive(value: true);
			_syncMiniImage.ChangeSprite((int)(syncType - 1));
		}
		else
		{
			_syncMiniImage.gameObject.SetActive(value: false);
		}
	}

	public void SetUserData(int playerIndex, AssetManager manager, UserDetail data, UserOption option)
	{
		_userinfomation.SetUserDataParts(playerIndex, manager, data, option);
	}

	public void SetVisibleUserInformation(bool isVisible)
	{
		_userinfomation.gameObject.SetActive(isVisible);
	}

	public void SetDifficulty(MusicDifficultyID difficultyIndex)
	{
		_difficultyMiniImage?.ChangeSprite((int)difficultyIndex);
		for (int i = 0; i < _levelBubbleObjects.Length; i++)
		{
			_levelBubbleObjects[i].SetActive(i == (int)difficultyIndex);
		}
	}

	public void SetDxScore(uint dxScore)
	{
		if (_dxScore != null)
		{
			_dxScore.ChangeText($"{dxScore:00000}");
		}
	}

	public void SetAchievement(uint score)
	{
		if (_achivementText != null)
		{
			_achivementText.text = $"{(float)score / 10000f:##0.0000}%";
		}
	}

	public void SetLevel(int level, MusicLevelID levelID, int difficulty)
	{
		Sprite[] musicLevelSprites = CommonPrefab.GetMusicLevelSprites(difficulty);
		_difficultySingle.SetSpriteSheet(musicLevelSprites);
		_difficultyDouble.SetSpriteSheet(musicLevelSprites);
		_levelTextImage.sprite = musicLevelSprites[14];
		if (level > 9)
		{
			_difficultySingle.gameObject.SetActive(value: false);
			_difficultyDouble.gameObject.SetActive(value: true);
			_difficultyDouble.ChangeText(levelID.GetLevelNum().PadRight(3));
		}
		else
		{
			_difficultySingle.gameObject.SetActive(value: true);
			_difficultyDouble.gameObject.SetActive(value: false);
			_difficultySingle.ChangeText(levelID.GetLevelNum().PadRight(2));
		}
	}

	public void SetGameScoreType(ConstParameter.ScoreKind kind)
	{
		int index = ((kind != ConstParameter.ScoreKind.Deluxe) ? 1 : 0);
		_gameTypeImage.ChangeSprite(index);
	}

	public void SetPerfectChallenge(bool isActive, int life, bool isClear)
	{
		_perfectChallengeObject.SetActive(isActive);
		_perfectChallengeClearObject.SetActive(isClear);
		_lifeCounter.rectTransform.anchoredPosition = new Vector2(0f, 0f);
		string text;
		if (life < 10)
		{
			text = $" {life} ";
		}
		else if (life < 100)
		{
			text = $" {life}";
			_lifeCounter.rectTransform.anchoredPosition = new Vector2(-5.5f, 0f);
		}
		else
		{
			text = $"{life}";
		}
		_lifeCounter.ChangeText(text);
	}
}
