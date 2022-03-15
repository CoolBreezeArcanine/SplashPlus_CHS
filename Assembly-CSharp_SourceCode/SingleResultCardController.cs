using DB;
using MAI2System;
using Manager;
using Manager.UserDatas;
using Monitor.Result;
using Timeline;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class SingleResultCardController : ResultCardBaseController
{
	[SerializeField]
	[Header("写真オブジェクト")]
	private GameObject _photoObject;

	[SerializeField]
	[Header("ユーザー情報")]
	private UserInformationController _userinfomation;

	[SerializeField]
	[Header("クリアランク")]
	private MultipleImage _clearRankImage;

	[SerializeField]
	[Header("難易度表示")]
	private MultipleImage _difficultyNameImage;

	[SerializeField]
	[Header("クリア文字")]
	private GameObject _clearObject;

	[SerializeField]
	[Header("難易度レベル表示")]
	private RectTransform _levelObject;

	[SerializeField]
	private SpriteCounter _difficultySingle;

	[SerializeField]
	private SpriteCounter _difficultyDouble;

	[SerializeField]
	private Image _levelTextImage;

	[SerializeField]
	[Header("キャラクター")]
	private Image _characterImage;

	[SerializeField]
	[Header("達成率")]
	private AchievementCounterObject _counterObject;

	[SerializeField]
	[Header("自己ベスト達成率")]
	private SpriteCounter _bestCounter;

	[SerializeField]
	private SpriteCounter _bestDiffCounter;

	[SerializeField]
	[Header("NewRecord")]
	private GameObject _newRecordObject;

	[SerializeField]
	[Header("称号虹")]
	private GameObject _rainbowObject;

	[SerializeField]
	[Header("コンボ称号")]
	private GameObject _medalObject;

	[SerializeField]
	private MultipleImage _medalBase;

	[SerializeField]
	private MultipleImage _medalText;

	[SerializeField]
	[Header("シンク称号")]
	private GameObject _syncMedalObject;

	[SerializeField]
	private MultipleImage _syncMedalBase;

	[SerializeField]
	private MultipleImage _syncMedaltext;

	[SerializeField]
	[Header("MaxCombo")]
	private CounterObject _comboCounter;

	[SerializeField]
	[Header("MaxSync")]
	private CounterObject _syncCounter;

	[SerializeField]
	private TextMeshProUGUI _rankText;

	[SerializeField]
	[Header("FAST/LATEオブジェクト")]
	private GameObject _fastLateObject;

	[SerializeField]
	[Header("FAST")]
	private TextMeshProUGUI _fastText;

	[SerializeField]
	[Header("LATE")]
	private TextMeshProUGUI _lateText;

	[SerializeField]
	[Header("DXスコア")]
	private TextMeshProUGUI _dxCurrentScoreText;

	[SerializeField]
	[Header("DXスコア最大")]
	private TextMeshProUGUI _dxMaximumScoreText;

	[SerializeField]
	[Header("詳細スコア")]
	private TextMeshProUGUI _criticalScore;

	[SerializeField]
	private TextMeshProUGUI _perfectScore;

	[SerializeField]
	private TextMeshProUGUI _greatScore;

	[SerializeField]
	private TextMeshProUGUI _goodScore;

	[SerializeField]
	private TextMeshProUGUI _missScore;

	[SerializeField]
	[Header("クリティカルパーフェクト")]
	private GameObject _criticalScoreObject;

	[SerializeField]
	[Header("スコアゲージ")]
	private ScoreGaugeController _scoreGaugeController;

	[SerializeField]
	[Header("でらっくすスター")]
	private MultipleImage[] _dxStars;

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

	private Outline _difficultyNameOutline;

	private Shadow _difficultyNameShadow;

	public override void Initialize()
	{
		base.Initialize();
		_difficultyNameOutline = _difficultyNameImage.GetComponent<Outline>();
		_difficultyNameShadow = _difficultyNameImage.GetComponent<Shadow>();
	}

	public void SetSwitchPhotoCharacter(bool isPhoto)
	{
		_characterImage.gameObject.SetActive(!isPhoto);
		_photoObject.SetActive(isPhoto);
	}

	public void SetCharacter(Sprite character)
	{
		_characterImage.sprite = character;
	}

	public void SetVisibleClear(bool isVisible)
	{
		_clearObject.SetActive(isVisible);
	}

	public void SetClearRank(MusicClearrankID rank)
	{
		_clearRankImage.ChangeSprite((int)rank);
	}

	public void SetAchievement(uint score)
	{
		_counterObject.SetAchievement(0u, score);
		_counterObject.OnClipTailEnd();
	}

	public void SetMyBestRecord(bool isNewRecord, int best, int diff)
	{
		_newRecordObject.SetActive(isNewRecord);
		string text = ((isNewRecord || diff > 0) ? "+" : "");
		int num = 3 - (Mathf.Abs(diff) / 10000).ToString().Length;
		if (text != "")
		{
			num++;
		}
		if (diff == 0)
		{
			num++;
		}
		_bestCounter.ChangeText(((float)best / 10000f).ToString("##0.0000").PadLeft(8) + "%");
		_bestDiffCounter.ChangeText(text.PadLeft(num) + ((float)diff / 10000f).ToString("##0.0000") + "%");
		_bestDiffCounter.SetColor((diff > 0) ? CommonScriptable.GetColorSetting().RiseColor : CommonScriptable.GetColorSetting().DeclineColor);
	}

	public void SetUserData(int playerIndex, AssetManager manager, UserDetail data, UserOption option)
	{
		_userinfomation.SetUserDataParts(playerIndex, manager, data, option);
	}

	public void SetVisibleUserInformation(bool isVisible)
	{
		_userinfomation.gameObject.SetActive(isVisible);
	}

	public void SetDxScore(uint dxScore, uint maximum, int dxStarCount)
	{
		_dxCurrentScoreText.text = dxScore.ToString("0");
		_dxMaximumScoreText.text = maximum.ToString("0");
		int index = ((dxStarCount == 5) ? 2 : ((dxStarCount >= 3) ? 1 : 0));
		for (int i = 0; i < _dxStars.Length; i++)
		{
			if (i < dxStarCount)
			{
				_dxStars[i].gameObject.SetActive(value: true);
				_dxStars[i].ChangeSprite(index);
			}
			else
			{
				_dxStars[i].gameObject.SetActive(value: false);
			}
		}
	}

	public void SetFastLate(uint fast, uint late)
	{
		_fastText.text = fast.ToString();
		_lateText.text = late.ToString();
	}

	public void SetVisibleFastLate(bool isVisible)
	{
		_fastLateObject.SetActive(isVisible);
	}

	public void SetMedal(PlayComboflagID comboType, PlaySyncflagID syncType)
	{
		_rainbowObject.SetActive(comboType > PlayComboflagID.None || syncType > PlaySyncflagID.None);
		_medalObject.gameObject.SetActive(comboType > PlayComboflagID.None);
		switch (comboType)
		{
		case PlayComboflagID.Silver:
		case PlayComboflagID.Gold:
			_medalBase.ChangeSprite(0);
			_medalText.ChangeSprite((int)(comboType - 1));
			break;
		case PlayComboflagID.AllPerfect:
		case PlayComboflagID.AllPerfectPlus:
			_medalBase.ChangeSprite(1);
			_medalText.ChangeSprite((int)(comboType - 1));
			break;
		}
		bool flag = syncType > PlaySyncflagID.None;
		_syncMedalObject.SetActive(flag);
		_syncMedalBase.gameObject.SetActive(flag);
		if (flag)
		{
			switch (syncType)
			{
			case PlaySyncflagID.ChainLow:
			case PlaySyncflagID.ChainHi:
				_syncMedalBase.ChangeSprite(0);
				break;
			case PlaySyncflagID.SyncLow:
			case PlaySyncflagID.SyncHi:
				_syncMedalBase.ChangeSprite(1);
				break;
			}
			_syncMedaltext.ChangeSprite((int)(syncType - 1));
		}
	}

	public void SetMaxComboData(uint maxConbo)
	{
		_comboCounter.SetCountData(0u, maxConbo);
		_comboCounter.OnClipTailEnd();
	}

	public void SetMaxSyncData(uint maxSync)
	{
		_syncCounter.SetCountData(0u, maxSync);
		_syncCounter.OnClipTailEnd();
	}

	public void SetVisibleSync(bool isVisible)
	{
		_syncCounter.gameObject.SetActive(isVisible);
	}

	public void SetPlayerRank(uint rankOrder)
	{
		_rankText.text = rankOrder switch
		{
			0u => "1st", 
			1u => "2nd", 
			2u => "3rd", 
			3u => "4th", 
			_ => "err", 
		};
	}

	public void SetVisiblePlayerRank(bool isVisible)
	{
		_rankText.gameObject.SetActive(isVisible);
	}

	public void SetDifficulty(MusicDifficultyID difficulty, Color difficultyFrontColor, Color difficultyBackColor)
	{
		_difficultyNameImage.ChangeSprite((int)difficulty);
		Color color3 = (_difficultyNameShadow.effectColor = (_difficultyNameOutline.effectColor = difficultyBackColor));
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

	public void SetVisibleCritical(bool isVisible)
	{
		_criticalScoreObject.SetActive(isVisible);
		_scoreGaugeController.SetActiveCriticalScore(isVisible);
	}

	public void SetScore(uint critical, uint perfect, uint great, uint good, uint miss)
	{
		_criticalScore.text = critical.ToString();
		_perfectScore.text = perfect.ToString();
		_greatScore.text = great.ToString();
		_goodScore.text = good.ToString();
		_missScore.text = miss.ToString();
	}

	public void SetScoreGauge(NoteScore.EScoreType type, float perfect, float critical, float great, float good, uint max)
	{
		_scoreGaugeController.SetScoreGauge(type, perfect, critical, great, good, max);
		_scoreGaugeController.ShowGuages();
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
