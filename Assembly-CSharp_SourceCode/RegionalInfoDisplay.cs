using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DB;
using MAI2.Util;
using Manager;
using Manager.MaiStudio;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class RegionalInfoDisplay : MonoBehaviour
{
	[SerializeField]
	[Header("Treasure")]
	private TreasureSignage _treasureSignage;

	[SerializeField]
	[Header("Regular Region")]
	private CanvasGroup _regionalGroup;

	[SerializeField]
	private TextMeshProUGUI _regionalTitleText;

	[SerializeField]
	private Image _regionalIslandImage;

	[SerializeField]
	[Header("Collabo Region")]
	private CanvasGroup _collaboGroup;

	[SerializeField]
	private TextMeshProUGUI _collaboTitleText;

	[SerializeField]
	private TextMeshProUGUI _collaboDateText;

	[SerializeField]
	private Image _collaboIslandImage;

	[SerializeField]
	[Header("Deluxe Region")]
	private Image _deluxIslandImage;

	[SerializeField]
	[Header("Lock")]
	private TextMeshProUGUI _lockTitleText;

	[SerializeField]
	[Header("パーフェクトチャレンジ")]
	private GameObject _perfectChallengeObject;

	[SerializeField]
	private MultipleImage _perfectChallengeDifficultyImage;

	[SerializeField]
	private SpriteCounter _lifeCounter;

	[SerializeField]
	[Header("コンプリート")]
	private GameObject _completeDisplayObject;

	[SerializeField]
	private GameObject _completeImage;

	[SerializeField]
	private Image _completeIslandImage;

	[SerializeField]
	private TextMeshProUGUI _completeText;

	private Animator _displayAnimator;

	private AssetManager _assetManager;

	public void Initialize(List<UserMapData> mapList, AssetManager assetManager)
	{
		_assetManager = assetManager;
		_displayAnimator = GetComponent<Animator>();
		_perfectChallengeObject.SetActive(value: false);
		_completeDisplayObject.SetActive(value: false);
	}

	public void SetPerfectChallenge(int life, MusicDifficultyID difficulty, Sprite jacket, bool reached = true)
	{
		_perfectChallengeObject.SetActive(reached);
		_lifeCounter.rectTransform.anchoredPosition = new Vector2(-30f, 0f);
		string text;
		if (life < 10)
		{
			text = $" {life} ";
		}
		else if (life < 100)
		{
			text = $" {life}";
			_lifeCounter.rectTransform.anchoredPosition = new Vector2(-35f, 0f);
		}
		else
		{
			text = $"{life}";
		}
		_lifeCounter.ChangeText(text);
		_perfectChallengeDifficultyImage.ChangeSprite((int)difficulty);
		_treasureSignage.SePerfectChallengeData(life, jacket, reached);
	}

	public void SetPerfectChallengeClose()
	{
		_perfectChallengeObject.SetActive(value: false);
	}

	public void SetData(UserMapData userMapData, int monIndex)
	{
		_displayAnimator.Play(Animator.StringToHash("In"), 0, 0f);
		MapData mapData = Singleton<DataManager>.Instance.GetMapData(userMapData.ID);
		if (userMapData.IsLock)
		{
			_completeImage.SetActive(value: false);
			_completeDisplayObject.SetActive(value: false);
			_collaboGroup.alpha = 0f;
			_regionalIslandImage.gameObject.SetActive(value: false);
			_collaboIslandImage.gameObject.SetActive(value: false);
			_deluxIslandImage.gameObject.SetActive(value: false);
			SetPerfectChallengeClose();
			string text = "";
			foreach (StringID item in mapData.ReleaseConditionIds.list)
			{
				text += item.str;
			}
			_treasureSignage.SetSecret();
			_treasureSignage.SetDistanceMeterClose();
			_treasureSignage.SetProgress(-1f);
			string text2 = CommonMessageID.MapSelect_CloseMap.GetName().Replace(CommonMessageID.MapSelect_CloseReplaceName.GetName(), text);
			_lockTitleText.text = ((text.Length > 0) ? text2 : "???");
			_regionalTitleText.text = "???";
			return;
		}
		if (!mapData.IsCollabo)
		{
			_regionalGroup.alpha = 1f;
			if (userMapData.IsDeluxe)
			{
				_collaboGroup.alpha = 0f;
				_regionalTitleText.text = mapData.name.str;
				_regionalIslandImage.gameObject.SetActive(value: false);
				_collaboIslandImage.gameObject.SetActive(value: false);
				_deluxIslandImage.gameObject.SetActive(value: true);
				_treasureSignage.SetDistanceMeter(TreasureSignage.MeterType.Deluxe, (int)userMapData.Distance);
				_deluxIslandImage.sprite = _assetManager.GetMapBgSprite(mapData.name.id, "UI_Island") ?? _assetManager.GetMapBgSprite(999999, "UI_Island");
			}
			else
			{
				_collaboGroup.alpha = 0f;
				_regionalTitleText.text = mapData.name.str;
				_regionalIslandImage.gameObject.SetActive(value: true);
				_collaboIslandImage.gameObject.SetActive(value: false);
				_deluxIslandImage.gameObject.SetActive(value: false);
				_treasureSignage.SetDistanceMeterClose();
				Sprite sprite = _assetManager.GetMapBgSprite(mapData.name.id, "UI_Island") ?? _assetManager.GetMapBgSprite(999999, "UI_Island");
				_regionalIslandImage.sprite = sprite;
			}
		}
		else
		{
			_regionalGroup.alpha = 0f;
			_regionalIslandImage.gameObject.SetActive(value: false);
			_collaboIslandImage.gameObject.SetActive(value: true);
			_deluxIslandImage.gameObject.SetActive(value: false);
			_collaboIslandImage.sprite = _assetManager.GetMapBgSprite(mapData.name.id, "UI_Island");
			_collaboGroup.alpha = 1f;
			_collaboTitleText.text = mapData.name.str;
			long eventEndUnixTime = Singleton<EventManager>.Instance.GetEventEndUnixTime(mapData.OpenEventId.id);
			if (1861887600 < eventEndUnixTime)
			{
				_collaboDateText.text = "";
			}
			else
			{
				DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(eventEndUnixTime).LocalDateTime - new TimeSpan(7, 0, 0);
				_collaboDateText.text = $"{dateTime:yyyy/MM/dd}まで";
			}
		}
		if (userMapData.IsComplete)
		{
			_completeImage.SetActive(value: true);
			_completeDisplayObject.SetActive(value: true);
			_completeIslandImage.sprite = _assetManager.GetMapBgSprite(mapData.name.id, "UI_IslandComp");
			_completeText.text = mapData.name.str;
		}
		else
		{
			_completeImage.SetActive(value: false);
			_completeDisplayObject.SetActive(value: false);
		}
		ReadOnlyCollection<MapTreasureExData> treasureExDatas = mapData.TreasureExDatas;
		_treasureSignage.ResetSignage();
		bool flag = false;
		int num = 0;
		int num2 = 0;
		int num3 = 10;
		int num4 = 0;
		foreach (MapTreasureExData item2 in treasureExDatas)
		{
			MapTreasureData mapTreasureData = Singleton<DataManager>.Instance.GetMapTreasureData(item2.TreasureId.id);
			if (userMapData.IsDeluxe)
			{
				_treasureSignage.SetDeluxePin(item2.Distance <= userMapData.Distance);
			}
			else if (num4 == treasureExDatas.Count - 1 && mapTreasureData.TreasureType == MapTreasureType.Character && num < num3)
			{
				TreasurePinObject.PinType pinType = TreasurePinObject.PinType.Goal;
				int frameType = 2;
				int treasureType = 0;
				_treasureSignage.SetPin(num++, pinType, frameType, treasureType);
				if (userMapData.Distance >= item2.Distance)
				{
					num2++;
					if (userMapData.Distance == item2.Distance && (mapTreasureData.TreasureType == MapTreasureType.MapTaskMusic || mapTreasureData.TreasureType == MapTreasureType.Challenge))
					{
						UserData userData = Singleton<UserDataManager>.Instance.GetUserData(monIndex);
						ChallengeData challengeData = null;
						int item_id2 = -1;
						int num5 = -1;
						switch (mapTreasureData.TreasureType)
						{
						case MapTreasureType.MapTaskMusic:
							item_id2 = mapTreasureData.MusicId.id;
							break;
						case MapTreasureType.Challenge:
							challengeData = Singleton<DataManager>.Instance.GetChallengeData(mapTreasureData.Challenge.id);
							if (challengeData != null)
							{
								item_id2 = challengeData.Music.id;
							}
							break;
						}
						if (item_id2 > 0)
						{
							num5 = userData.MusicUnlockList.FindIndex((int m) => m == item_id2);
						}
						if (num5 < 0)
						{
							num2--;
						}
					}
				}
			}
			else if (mapTreasureData.TreasureType != MapTreasureType.Character && num < num3)
			{
				TreasurePinObject.PinType pinType2 = ((num != 0) ? TreasurePinObject.PinType.Normal : TreasurePinObject.PinType.Start);
				int frameType2 = ((mapTreasureData.TreasureType != MapTreasureType.Frame) ? ((mapTreasureData.TreasureType == MapTreasureType.MusicNew) ? 1 : 2) : 0);
				int treasureType2 = ((mapTreasureData.TreasureType == MapTreasureType.MusicNew || mapTreasureData.TreasureType == MapTreasureType.MapTaskMusic || mapTreasureData.TreasureType == MapTreasureType.Challenge) ? 1 : 0);
				_treasureSignage.SetPin(num++, pinType2, frameType2, treasureType2);
				if (userMapData.Distance >= item2.Distance)
				{
					num2++;
					if (userMapData.Distance == item2.Distance && (mapTreasureData.TreasureType == MapTreasureType.MapTaskMusic || mapTreasureData.TreasureType == MapTreasureType.Challenge))
					{
						UserData userData2 = Singleton<UserDataManager>.Instance.GetUserData(monIndex);
						ChallengeData challengeData2 = null;
						int item_id = -1;
						int num6 = -1;
						switch (mapTreasureData.TreasureType)
						{
						case MapTreasureType.MapTaskMusic:
							item_id = mapTreasureData.MusicId.id;
							break;
						case MapTreasureType.Challenge:
							challengeData2 = Singleton<DataManager>.Instance.GetChallengeData(mapTreasureData.Challenge.id);
							if (challengeData2 != null)
							{
								item_id = challengeData2.Music.id;
							}
							break;
						}
						if (item_id > 0)
						{
							num6 = userData2.MusicUnlockList.FindIndex((int m) => m == item_id);
						}
						if (num6 < 0)
						{
							num2--;
						}
					}
				}
			}
			if ((!flag && userMapData.Distance < item2.Distance && (mapTreasureData.TreasureType != MapTreasureType.Character || num4 == treasureExDatas.Count - 1)) || ((mapTreasureData.TreasureType == MapTreasureType.MapTaskMusic || mapTreasureData.TreasureType == MapTreasureType.Challenge) && item2.Distance - userMapData.Distance == 0L))
			{
				flag = true;
				SetPerfectChallengeClose();
				int num7 = (int)(item2.Distance - userMapData.Distance);
				if (userMapData.IsDeluxe)
				{
					num7 = (int)userMapData.Distance;
					_treasureSignage.SetSecret();
					_treasureSignage.SetDistanceMeter(TreasureSignage.MeterType.Deluxe, num7);
				}
				else
				{
					switch (mapTreasureData.TreasureType)
					{
					case MapTreasureType.MusicNew:
					{
						Texture2D jacketTexture2D3 = _assetManager.GetJacketTexture2D(mapTreasureData.MusicId.id);
						_treasureSignage.SetJacketData(Sprite.Create(jacketTexture2D3, new Rect(0f, 0f, jacketTexture2D3.width, jacketTexture2D3.height), Vector2.zero));
						_treasureSignage.SetDistanceMeter(TreasureSignage.MeterType.Music, num7);
						break;
					}
					case MapTreasureType.NamePlate:
					{
						Texture2D plateTexture2D = _assetManager.GetPlateTexture2D(mapTreasureData.NamePlate.id);
						_treasureSignage.SetPlateData(Sprite.Create(plateTexture2D, new Rect(0f, 0f, plateTexture2D.width, plateTexture2D.height), Vector2.zero));
						_treasureSignage.SetDistanceMeter(TreasureSignage.MeterType.Collection, num7);
						break;
					}
					case MapTreasureType.Frame:
					{
						_treasureSignage.SetDistanceMeter(TreasureSignage.MeterType.Collection, num7);
						Texture2D frameTexture2D = _assetManager.GetFrameTexture2D(mapTreasureData.Frame.id);
						_treasureSignage.SetFrameData(Sprite.Create(frameTexture2D, new Rect(0f, 0f, frameTexture2D.width, frameTexture2D.height), Vector2.zero));
						break;
					}
					case MapTreasureType.MapTaskMusic:
					{
						Texture2D jacketTexture2D2 = _assetManager.GetJacketTexture2D(mapTreasureData.MusicId.id);
						_treasureSignage.SetJacketData(Sprite.Create(jacketTexture2D2, new Rect(0f, 0f, jacketTexture2D2.width, jacketTexture2D2.height), new Vector2(0.5f, 0.5f)));
						_treasureSignage.SetDistanceMeter(TreasureSignage.MeterType.AssignmentSong, num7);
						break;
					}
					case MapTreasureType.Character:
					{
						_treasureSignage.SetDistanceMeter(TreasureSignage.MeterType.Collection, num7);
						Texture2D characterTexture2D = _assetManager.GetCharacterTexture2D(mapTreasureData.CharacterId.id);
						_treasureSignage.SetCharacter(Sprite.Create(characterTexture2D, new Rect(0f, 0f, characterTexture2D.width, characterTexture2D.height), Vector2.zero));
						break;
					}
					case MapTreasureType.Challenge:
					{
						ChallengeData challengeData3 = Singleton<DataManager>.Instance.GetChallengeData(mapTreasureData.Challenge.id);
						ChallengeDetail challengeDetail = ChallengeManager.GetChallengeDetail(mapTreasureData.Challenge.id);
						Texture2D jacketTexture2D = _assetManager.GetJacketTexture2D(challengeData3.Music.id);
						Sprite jacket = Sprite.Create(jacketTexture2D, new Rect(0f, 0f, jacketTexture2D.width, jacketTexture2D.height), new Vector2(0.5f, 0.5f));
						int startLife = challengeDetail.startLife;
						MusicDifficultyID unlockDifficulty = challengeDetail.unlockDifficulty;
						bool reached = 0 >= num7;
						SetPerfectChallenge(startLife, unlockDifficulty, jacket, reached);
						_treasureSignage.SetDistanceMeter(TreasureSignage.MeterType.Boss, num7);
						break;
					}
					default:
						_treasureSignage.SetSecret();
						_treasureSignage.SetDistanceMeterClose();
						break;
					}
				}
			}
			num4++;
		}
		if (!flag && userMapData.IsDeluxe)
		{
			int distance = (int)userMapData.Distance;
			_treasureSignage.SetSecret();
			_treasureSignage.SetDistanceMeter(TreasureSignage.MeterType.Deluxe, distance);
			SetPerfectChallengeClose();
		}
		if (!userMapData.IsDeluxe)
		{
			_treasureSignage.SetPinPosition(num);
		}
		int count = treasureExDatas.Count;
		List<MapTreasureExData> list = new List<MapTreasureExData>(count);
		for (int i = 0; i < count; i++)
		{
			list.Add(mapData.TreasureExDatas[i]);
		}
		if (userMapData.IsDeluxe)
		{
			_treasureSignage.SetGuage(1, 0, isProgress: true);
		}
		else if (0 < list.Count)
		{
			_treasureSignage.SetGuage(num, num2, userMapData.Distance == 0);
		}
	}

	public void ViewUpdate()
	{
	}

	private void PlayWaitForAnimation(Animator animator, int playAnimationHash, int nextAnimationHash, int layer = 0)
	{
		StartCoroutine(WaitForAnimationCoroutine(animator, playAnimationHash, nextAnimationHash, layer));
	}

	private IEnumerator WaitForAnimationCoroutine(Animator animator, int playAnimationHash, int nextAnimationHash, int layer)
	{
		animator.Play(playAnimationHash, layer, 0f);
		yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(layer).length);
		animator.Play(nextAnimationHash, layer, 0f);
	}
}
