using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class TreasureSignage : MonoBehaviour
{
	public enum MeterType
	{
		AssignmentSong,
		Boss,
		Music,
		Collection,
		Deluxe
	}

	private const float GuageMax = 450f;

	private const float GuageMin = 0f;

	private const float CursorMax = 437f;

	private const float CursorMin = 13f;

	[SerializeField]
	[Header("進行度表示")]
	private GameObject _progressObject;

	[SerializeField]
	private RectTransform _progressGuage;

	[SerializeField]
	private RectTransform _cursor;

	[SerializeField]
	private Image _backgroundImage;

	[SerializeField]
	[Header("報酬目安標")]
	private TreasurePinObject[] _pinObjects;

	[SerializeField]
	[Header("次回報酬標識")]
	private GameObject _rewardSignageObject;

	[SerializeField]
	[Header("シークレット表示")]
	private GameObject _secretImageObject;

	[SerializeField]
	[Header("ジャケット")]
	private Image _jacketImage;

	[SerializeField]
	[Header("ネームプレート")]
	private GameObject _namePlateSignageObject;

	[SerializeField]
	private MultiImage _namePlateImage;

	[SerializeField]
	[Header("フレーム")]
	private Image _frameImage;

	[SerializeField]
	[Header("ボス曲")]
	private GameObject _bossJacketSignageObject;

	[SerializeField]
	private GameObject _bossSignageObject2;

	[SerializeField]
	private Image _bossJacketImage;

	[SerializeField]
	private SpriteCounter _lifeCounter;

	[SerializeField]
	[Header("残り距離表示")]
	private GameObject _musicMeterObject;

	[SerializeField]
	private TextMeshProUGUI _musicRemainingMeter;

	[SerializeField]
	[Header("コレクション 残獲得処理")]
	private GameObject _collectionMeterObject;

	[SerializeField]
	private TextMeshProUGUI _collectionRemainingDistance;

	[SerializeField]
	[Header("でらっくす地方 総距離")]
	private GameObject _deluxMeterObject;

	[SerializeField]
	private TextMeshProUGUI _deluxTotalDistance;

	[SerializeField]
	[Header("課題曲標識")]
	private GameObject _assignmentSongSignage;

	[SerializeField]
	[Header("キャラクター報酬（特殊）")]
	private Image _characterImage;

	public void SetProgress(float progress)
	{
		if (progress < 0f)
		{
			_progressObject.SetActive(value: false);
			return;
		}
		_progressObject.SetActive(value: true);
		float num = 450f * progress;
		float y = _progressGuage.sizeDelta.y;
		if (num < 0f)
		{
			num = 0f;
		}
		_progressGuage.sizeDelta = new Vector2(num, y);
		float num2 = 437f * progress;
		float y2 = _cursor.anchoredPosition.y;
		if (num2 < 13f)
		{
			num2 = 13f;
		}
		_cursor.anchoredPosition = new Vector2(num2, y2);
	}

	public void SetGuage(int sectionCount, int pos, bool isProgress)
	{
		sectionCount--;
		if (sectionCount < 0)
		{
			return;
		}
		_progressObject.SetActive(value: true);
		float y = _cursor.anchoredPosition.y;
		if (isProgress)
		{
			_progressGuage.sizeDelta = new Vector2(0f, y);
		}
		else
		{
			_progressObject.SetActive(value: true);
			if (pos == 0)
			{
				_progressGuage.sizeDelta = new Vector2(60f, y);
			}
			else if (sectionCount <= 1)
			{
				_progressGuage.sizeDelta = new Vector2((pos == 2) ? 450 : 270, y);
			}
			else
			{
				float num = 300f / (float)sectionCount;
				float num2 = 120f + num * (float)pos - num / 2f;
				if (450f < num2)
				{
					num2 = 450f;
				}
				_progressGuage.sizeDelta = new Vector2(num2, y);
			}
		}
		_cursor.anchoredPosition = new Vector2(_progressGuage.sizeDelta.x + 6f, _cursor.anchoredPosition.y);
	}

	public void SetDistanceMeter(MeterType meterType, int distance = 0)
	{
		string text = $"{distance / 1000}km";
		SetDistanceMeterClose();
		switch (meterType)
		{
		case MeterType.AssignmentSong:
			if (distance == 0)
			{
				_assignmentSongSignage.SetActive(value: true);
				break;
			}
			_musicMeterObject.SetActive(value: true);
			_musicRemainingMeter.text = text;
			break;
		case MeterType.Boss:
			if (distance == 0)
			{
				_bossSignageObject2.SetActive(value: true);
				break;
			}
			_musicMeterObject.SetActive(value: true);
			_musicRemainingMeter.text = text;
			break;
		case MeterType.Music:
			_musicMeterObject.SetActive(value: true);
			_musicRemainingMeter.text = text;
			break;
		case MeterType.Collection:
			_collectionMeterObject.SetActive(value: true);
			_collectionRemainingDistance.text = text;
			break;
		case MeterType.Deluxe:
			_deluxMeterObject.SetActive(value: true);
			_deluxTotalDistance.text = text;
			break;
		}
	}

	public void SetDistanceMeterClose()
	{
		_assignmentSongSignage.SetActive(value: false);
		_bossSignageObject2.SetActive(value: false);
		_musicMeterObject.SetActive(value: false);
		_collectionMeterObject.SetActive(value: false);
		_deluxMeterObject.SetActive(value: false);
	}

	public void SetJacketData(Sprite jacketeSprite)
	{
		_bossJacketSignageObject.SetActive(value: false);
		_jacketImage.sprite = jacketeSprite;
		_secretImageObject.SetActive(value: false);
		_jacketImage.gameObject.SetActive(value: true);
		_namePlateSignageObject.SetActive(value: false);
		_frameImage.gameObject.SetActive(value: false);
		_characterImage.gameObject.SetActive(value: false);
	}

	public void SetPlateData(Sprite plateSprite)
	{
		_bossJacketSignageObject.SetActive(value: false);
		_namePlateImage.Image2 = plateSprite;
		_secretImageObject.SetActive(value: false);
		_jacketImage.gameObject.SetActive(value: false);
		_namePlateSignageObject.SetActive(value: true);
		_frameImage.gameObject.SetActive(value: false);
		_characterImage.gameObject.SetActive(value: false);
	}

	public void SetFrameData(Sprite frameSprite)
	{
		_bossJacketSignageObject.SetActive(value: false);
		_frameImage.sprite = frameSprite;
		_secretImageObject.SetActive(value: false);
		_jacketImage.gameObject.SetActive(value: false);
		_namePlateSignageObject.SetActive(value: false);
		_frameImage.gameObject.SetActive(value: true);
		_characterImage.gameObject.SetActive(value: false);
	}

	public void SetCharacter(Sprite character)
	{
		_characterImage.gameObject.SetActive(value: true);
		_characterImage.sprite = character;
		_secretImageObject.SetActive(value: false);
		_jacketImage.gameObject.SetActive(value: false);
		_namePlateSignageObject.SetActive(value: false);
		_frameImage.gameObject.SetActive(value: false);
	}

	public void SetSecret()
	{
		_bossJacketSignageObject.SetActive(value: false);
		_secretImageObject.SetActive(value: true);
		_jacketImage.gameObject.SetActive(value: false);
		_namePlateSignageObject.SetActive(value: false);
		_frameImage.gameObject.SetActive(value: false);
		_characterImage.gameObject.SetActive(value: false);
	}

	public void SePerfectChallengeData(int life, Sprite bossSprite, bool reached = true)
	{
		_bossJacketSignageObject.SetActive(value: true);
		_bossJacketImage.sprite = bossSprite;
		_bossJacketSignageObject.transform.GetChild(1).gameObject.SetActive(reached);
		_lifeCounter.rectTransform.anchoredPosition = Vector2.zero;
		string text;
		if (life < 10)
		{
			text = $" {life} ";
		}
		else if (life < 100)
		{
			text = $" {life}";
			_lifeCounter.rectTransform.anchoredPosition = new Vector2(-6f, 0f);
		}
		else
		{
			text = $"{life}";
		}
		_lifeCounter.ChangeText(text);
	}

	public void SetPin(int pinIndex, TreasurePinObject.PinType pinType, int frameType, int treasureType)
	{
		_pinObjects[pinIndex].SetData(pinType, frameType, treasureType);
	}

	public void SetPinPosition(int pinCount)
	{
		ResetPin();
		switch (pinCount)
		{
		case 0:
			return;
		case 1:
			_pinObjects[0].CanvasGroup.alpha = 1f;
			_pinObjects[0].SetPosition(300);
			_pinObjects[0].SetPinType(TreasurePinObject.PinType.Goal);
			return;
		case 2:
		{
			for (int i = 0; i < 2; i++)
			{
				_pinObjects[i].CanvasGroup.alpha = 1f;
				_pinObjects[i].SetPosition(300 * i);
			}
			_pinObjects[1].SetPinType(TreasurePinObject.PinType.Goal);
			return;
		}
		}
		int num = 300 / (pinCount - 1);
		for (int j = 0; j < pinCount; j++)
		{
			_pinObjects[j].CanvasGroup.alpha = 1f;
			_pinObjects[j].SetPosition(num * j);
		}
		_pinObjects[pinCount - 1].SetPinType(TreasurePinObject.PinType.Goal);
	}

	public void SetDeluxePin(bool isComplete)
	{
		ResetPin();
		if (!isComplete)
		{
			_pinObjects[0].CanvasGroup.alpha = 1f;
			_pinObjects[0].SetPosition(300);
			_pinObjects[0].SetData(TreasurePinObject.PinType.Goal, 1, 2);
		}
	}

	public void ResetPin()
	{
		TreasurePinObject[] pinObjects = _pinObjects;
		for (int i = 0; i < pinObjects.Length; i++)
		{
			pinObjects[i].CanvasGroup.alpha = 0f;
		}
	}

	public void ResetSignage()
	{
		_jacketImage.gameObject.SetActive(value: false);
		_namePlateSignageObject.SetActive(value: false);
		_frameImage.gameObject.SetActive(value: false);
		_bossJacketSignageObject.SetActive(value: false);
		_characterImage.gameObject.SetActive(value: false);
	}
}
