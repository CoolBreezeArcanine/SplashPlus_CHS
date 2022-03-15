using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicCardObject : SelectCardObject
{
	public enum ScoreAchivment
	{
		None,
		FullCombo,
		GreatFullCombo,
		AllPerfect
	}

	private const float WAIT_SHOW_TIME_MSEC = 1000f;

	[SerializeField]
	[Header("UIパーツ")]
	private Image difficultyBackground;

	[SerializeField]
	[Header("楽曲名")]
	private RectTransform titleObject;

	[SerializeField]
	private TextMeshProUGUI[] titleNameTextArray;

	[SerializeField]
	[Header("アーティスト")]
	private RectTransform artistObject;

	[SerializeField]
	private TextMeshProUGUI[] artistNameTextArray;

	[SerializeField]
	[Header("ジャケット")]
	private RawImage jacketImage;

	[SerializeField]
	[Tooltip("ノーツデザイナー名")]
	private TextMeshProUGUI _noteDesignerNameText;

	[SerializeField]
	[Tooltip("ジャンル名")]
	private TextMeshProUGUI _genreNameText;

	[SerializeField]
	[Header("難易度")]
	private Image difficultyLevelBackground;

	[SerializeField]
	private Image difficultyLevelNumberImage;

	[SerializeField]
	private Image[] difficultyLevelNumbers;

	[SerializeField]
	private Image plusSymbolImage;

	[SerializeField]
	private TextMeshProUGUI rankText;

	[SerializeField]
	[Header("達成率")]
	private GameObject _achievementObject;

	[SerializeField]
	private TextMeshProUGUI _achievementIntegerText;

	[SerializeField]
	private TextMeshProUGUI _achievementDecimalText;

	[SerializeField]
	private TextMeshProUGUI BPMText;

	[SerializeField]
	[Header("称号")]
	private Image AP;

	[SerializeField]
	private Image FC;

	[SerializeField]
	private Image Sync100;

	[SerializeField]
	[Header("ラベル")]
	private Image _newImage;

	[SerializeField]
	private Image _taskImage;

	[SerializeField]
	private Image _lockImage;

	[SerializeField]
	private TextMeshProUGUI _dxScore;

	[SerializeField]
	[Header("ミニカード")]
	private MusicCardObject _miniCard;

	[SerializeField]
	private GameObject _mainGameObject;

	private bool isTitleScroll;

	private bool isArtistScroll;

	private int titleAnimationState;

	private int artistAnimationState;

	private float titleWidth;

	private float artistWidth;

	private float titleTimeCounter;

	private float artistTimeCounter;

	public void SetRankAchivment(string rank, int achivment, uint dxScore, ScoreAchivment socreAchivement, bool is100Sync)
	{
		if (_miniCard != null)
		{
			_miniCard.SetRankAchivment(rank, achivment, dxScore, socreAchivement, is100Sync);
		}
		if (achivment >= 0)
		{
			_newImage.gameObject.SetActive(value: false);
			_achievementObject.SetActive(value: true);
			string text = achivment.ToString("0000000").Substring(0, 3);
			string text2 = achivment.ToString("0000000").Substring(3, 4);
			_achievementIntegerText.text = text;
			_achievementDecimalText.text = "." + text2;
		}
		else
		{
			_achievementObject.SetActive(value: false);
		}
		_dxScore.text = dxScore.ToString();
		rankText.text = rank;
		AP.gameObject.SetActive(value: false);
		FC.gameObject.SetActive(value: false);
		switch (socreAchivement)
		{
		case ScoreAchivment.FullCombo:
		case ScoreAchivment.GreatFullCombo:
			FC.gameObject.SetActive(value: true);
			break;
		case ScoreAchivment.AllPerfect:
			AP.gameObject.SetActive(value: true);
			break;
		}
		Sync100.gameObject.SetActive(is100Sync);
	}

	public void SetMusicData(string title, string artist, string genre, string noteDesigner, int bpm, Texture2D jacket)
	{
		if (_miniCard != null)
		{
			_miniCard.SetMusicData(title, artist, genre, noteDesigner, bpm, jacket);
		}
		BPMText.text = "BPM " + bpm.ToString("000");
		_genreNameText.text = genre;
		titleNameTextArray[0].text = title;
		titleWidth = titleNameTextArray[0].preferredWidth;
		_noteDesignerNameText.text = noteDesigner;
		_taskImage.gameObject.SetActive(value: false);
		_lockImage.gameObject.SetActive(value: false);
		isTitleScroll = titleWidth >= 235f;
		if (!isTitleScroll)
		{
			titleWidth = 235f;
		}
		Vector2 sizeDelta = new Vector2(titleWidth, 35f);
		for (int i = 0; i < titleNameTextArray.Length; i++)
		{
			titleNameTextArray[i].text = title;
			titleNameTextArray[i].rectTransform.sizeDelta = sizeDelta;
		}
		artistNameTextArray[0].text = artist;
		artistWidth = artistNameTextArray[0].preferredWidth;
		isArtistScroll = artistWidth >= 236f;
		if (!isArtistScroll)
		{
			artistWidth = 236f;
		}
		sizeDelta = new Vector2(artistWidth, 25f);
		for (int j = 0; j < artistNameTextArray.Length; j++)
		{
			artistNameTextArray[j].text = artist;
			artistNameTextArray[j].rectTransform.sizeDelta = sizeDelta;
		}
		jacketImage.texture = jacket;
		ViewReset();
	}

	public Texture2D GetJacket()
	{
		return jacketImage.texture as Texture2D;
	}

	public void ChangeMode(bool isMini)
	{
		if (_miniCard != null)
		{
			if (isMini)
			{
				_mainGameObject.SetActive(value: false);
				difficultyBackground.enabled = false;
				_miniCard.gameObject.SetActive(value: true);
			}
			else
			{
				_mainGameObject.SetActive(value: true);
				difficultyBackground.enabled = true;
				_miniCard.gameObject.SetActive(value: false);
			}
		}
	}

	public override void ViewUpdate()
	{
		if (isTitleScroll)
		{
			titleTimeCounter += GameManager.GetGameMSecAdd();
			if (titleAnimationState == 0 && titleTimeCounter >= 1000f)
			{
				titleTimeCounter = 0f;
				titleAnimationState++;
			}
			else if (titleAnimationState == 1)
			{
				titleObject.anchoredPosition += Vector2.left * 0.5f;
				if (titleObject.anchoredPosition.x <= 0f - (titleWidth + 100f))
				{
					titleObject.anchoredPosition = Vector2.zero;
					titleTimeCounter = 0f;
					titleAnimationState = 0;
				}
			}
		}
		if (isArtistScroll)
		{
			artistTimeCounter += GameManager.GetGameMSecAdd();
			if (artistAnimationState == 0 && artistTimeCounter >= 1000f)
			{
				artistTimeCounter = 0f;
				artistAnimationState++;
			}
			else if (artistAnimationState == 1)
			{
				artistObject.anchoredPosition += Vector2.left * 0.5f;
				if (artistObject.anchoredPosition.x <= 0f - (artistWidth + 100f))
				{
					artistObject.anchoredPosition = Vector2.zero;
					artistTimeCounter = 0f;
					artistAnimationState = 0;
				}
			}
		}
		if (_miniCard != null)
		{
			_miniCard.ViewUpdate();
		}
	}

	public void ViewReset()
	{
		titleTimeCounter = (artistTimeCounter = 0f);
		titleAnimationState = (artistAnimationState = 0);
		Vector2 vector3 = (titleObject.anchoredPosition = (artistObject.anchoredPosition = Vector3.zero));
		if (_miniCard != null)
		{
			_miniCard.ViewReset();
		}
	}

	public void SetDifficulty(Sprite background, Sprite lvBackground, Sprite miniBackground)
	{
		if (difficultyBackground != null)
		{
			difficultyBackground.sprite = background;
		}
		if (difficultyLevelBackground != null)
		{
			difficultyLevelBackground.sprite = lvBackground;
		}
		if (_miniCard != null)
		{
			_miniCard.SetMiniCardBackground(miniBackground);
		}
	}

	public void SetMiniCardBackground(Sprite background)
	{
		difficultyBackground.sprite = background;
	}

	public void SetLevel(bool isPlush, Sprite plusSymbol, Sprite multiple1, Sprite multiple10 = null)
	{
		plusSymbolImage.gameObject.SetActive(value: false);
		Vector3 vector;
		if (multiple10 == null)
		{
			difficultyLevelNumbers[0].gameObject.SetActive(value: false);
			difficultyLevelNumbers[1].gameObject.SetActive(value: false);
			difficultyLevelNumberImage.gameObject.SetActive(value: true);
			difficultyLevelNumberImage.sprite = multiple1;
			vector = new Vector3(29f, 21f, 0f);
		}
		else
		{
			difficultyLevelNumbers[0].gameObject.SetActive(value: true);
			difficultyLevelNumbers[1].gameObject.SetActive(value: true);
			difficultyLevelNumberImage.gameObject.SetActive(value: false);
			difficultyLevelNumbers[0].sprite = multiple1;
			difficultyLevelNumbers[1].sprite = multiple10;
			vector = new Vector3(52f, 21f, 0f);
		}
		if (isPlush)
		{
			plusSymbolImage.gameObject.SetActive(value: true);
			plusSymbolImage.sprite = plusSymbol;
			plusSymbolImage.rectTransform.anchoredPosition = vector;
		}
		if (_miniCard != null)
		{
			_miniCard.SetLevel(isPlush, plusSymbol, multiple1, multiple10);
		}
	}
}
