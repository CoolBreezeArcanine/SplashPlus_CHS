using System.Collections.Generic;
using System.Linq;
using DB;
using MAI2.Util;
using Manager;
using Manager.MaiStudio;
using Monitor.MapResult.Common;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.MapResult
{
	public class Phase2CommonMember : CommonBase
	{
		public List<CommonValue> _valueListRainbow = new List<CommonValue>();

		public List<CommonValueUint> _valueListRainbowUint = new List<CommonValueUint>();

		public List<uint> _valueListRainbowCountUpRate = new List<uint>();

		public List<int> _rainbowPinIcon = new List<int>();

		public int _rainbowID;

		public int _rainbowStart;

		public int _rainbowEnd;

		public int _rainbowReleasePin;

		public bool _isJustRainbow;

		public int _isJustNextRainbowID;

		public bool _rainbowChallengeStart;

		public bool _isChallenge;

		public bool _isCommonStart = true;

		public bool _isChallengeDifficultyOK;

		public int _challengeInfoType;

		public bool _isChallengeClear;

		public int _bonusIconDispCount;

		public int _bonusIconMax;

		public List<Animator> _animListBonusIcon = new List<Animator>();

		public List<List<BonusNumber>> _listListBonusIconPoint = new List<List<BonusNumber>>();

		public PassBonusIconColor _passBonusIconColor = new PassBonusIconColor();

		public int _restPinMeterChildMax;

		public List<OdometerNumber> _objListRestPinMeter = new List<OdometerNumber>();

		public int _restPinMeterValue;

		public CommonValueUint _remainUntilPinCommonValue = new CommonValueUint();

		public int _totalMapRunMeterChildMax;

		public List<OdometerNumber> _objListTotalMapRunMeter = new List<OdometerNumber>();

		public uint _totalMapRunMeterValue;

		public int _toatalBonusMeterChildMax;

		public List<OdometerNumber> _objListTotalBonusMeter = new List<OdometerNumber>();

		public uint _toatalBonusMeterValue;

		public CommonValueUint _toatalBonusCommonValue = new CommonValueUint();

		private GameObject _prefabPrize;

		public int _prizeMax;

		public int _prizeWindowType;

		public List<GameObject> _nullListPrize = new List<GameObject>();

		public List<GameObject> _objListPrize = new List<GameObject>();

		public Animator _prize;

		public Animator _lime;

		public Animator _lemon;

		public Animator _otohime;

		private GameObject _prefabChallengeInfo;

		public List<GameObject> _nullListChallengeInfo = new List<GameObject>();

		public List<GameObject> _objListChallengeInfo = new List<GameObject>();

		public Animator _challengeInfo;

		public List<BounusIconDispInfo> _dispInfo = new List<BounusIconDispInfo>();

		public CommonValue _nearPinX = new CommonValue();

		public CommonValue _farPinX = new CommonValue();

		public bool _isSetPosNearPin;

		public bool _isSetPosFarPin;

		public bool _isUpdateNearPin;

		public bool _isUpdateFarPin;

		private GameObject _prefabNearPin;

		private GameObject _prefabNearPinGhost;

		private GameObject _prefabNearPinHand;

		public GameObject _objNearPin;

		public GameObject _objNearPinGhost;

		public GameObject _objNearPinHand;

		public GameObject _objFarPin;

		public GameObject _objFarPinGhost;

		public GameObject _objFarPinHand;

		public float _scroll1;

		public float _scroll2;

		public float _scrollBase1;

		public float _scrollBase2;

		public Vector3[] _posBG_Base = new Vector3[4];

		public Vector3[] _posBG = new Vector3[4];

		public Vector2 _texSizeBG;

		public GameObject _Derakkuma2;

		public GameObject _DerakkumaFire;

		public GameObject _DerakkumaSmoke01;

		public GameObject _DerakkumaSmoke02;

		public GameObject _DerakkumaSmoke03;

		public GameObject _DerakkumaSmoke;

		public CommonValue _DerakkumaPosX;

		public float _DerakkumaPosY;

		public int _RoadSpeedType;

		public List<int> _distanceBase = new List<int>();

		public List<uint> _distanceStart = new List<uint>();

		public List<uint> _distanceEnd = new List<uint>();

		public uint _paramTotalTime;

		public uint _paramMinWaitTime0;

		public uint _paramMinWaitTime1;

		public uint _paramMinWaitTime2;

		public uint _paramAddFrame;

		public uint _paramDispPercent;

		public float _paramRainbowMinDiff = 1E-08f;

		public float _paramNearPinStart = 500f;

		public float _paramNearPinReached = 70f;

		public float _paramNearPinUnreached = 150f;

		public bool _isReleasePinOver;

		public bool _isEndPinClear;

		public bool _isGetNewCharacter;

		public bool _isInfinityLimitOver;

		public bool _isInfinityLastPinOver;

		public uint _limitDistanceMax = 2147483647u;

		public uint _nextOtomodachiDistance;

		public int _nextOtomodachiInsertID = -1;

		public int _nextOtomodachiOriginalMapTresureID = -1;

		public bool _nextOtomodachiNothing;

		public List<int> _releaseIslandID = new List<int>();

		private GameObject _prefabNewIsland;

		public GameObject _objIsland;

		public List<Animator> _animAwakeStarRoot = new List<Animator>();

		public List<Animator> _animAwakeStar = new List<Animator>();

		public List<Animator> _animAwakeStarEffect = new List<Animator>();

		public List<Animator> _animAwakeStarSpark = new List<Animator>();

		public int _monitorID;

		public void InitializeNearPin(GameObject obj)
		{
			_prefabNearPin = Resources.Load<GameObject>("Process/MapResult/Prefabs/Pin_Parts/UI_Pin_Landmark_All");
			GameObject gameObject = new GameObject("EmptyPinParent");
			gameObject.transform.SetParent(obj.transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			gameObject.transform.localScale = Vector3.one;
			float num = 0f;
			GameObject gameObject2 = new GameObject("EmptyPinChild");
			gameObject2.transform.SetParent(gameObject.transform);
			num = -1f * _prefabNearPin.transform.localPosition.y;
			num += -40f;
			gameObject2.transform.localPosition = new Vector3(0f, num, 0f);
			gameObject2.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			gameObject2.transform.localScale = Vector3.one;
			_objNearPin = Object.Instantiate(_prefabNearPin);
			GameObject objNearPin = _objNearPin;
			objNearPin.transform.SetParent(gameObject2.transform);
			objNearPin.transform.localPosition = _prefabNearPin.transform.localPosition;
			objNearPin.transform.localRotation = _prefabNearPin.transform.localRotation;
			objNearPin.transform.localScale = _prefabNearPin.transform.localScale;
			_prefabNearPinGhost = Resources.Load<GameObject>("Process/MapResult/Prefabs/Pin_Parts/UI_Pin_Ghost");
			GameObject gameObject3 = new GameObject("EmptyGhostChild");
			gameObject3.transform.SetParent(gameObject.transform);
			gameObject3.transform.localPosition = new Vector3(0f, 0f, 0f);
			gameObject3.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			gameObject3.transform.localScale = Vector3.one;
			_objNearPinGhost = Object.Instantiate(_prefabNearPinGhost);
			GameObject objNearPinGhost = _objNearPinGhost;
			objNearPinGhost.transform.SetParent(gameObject3.transform);
			objNearPinGhost.transform.localPosition = _prefabNearPinGhost.transform.localPosition;
			objNearPinGhost.transform.localRotation = _prefabNearPinGhost.transform.localRotation;
			objNearPinGhost.transform.localScale = _prefabNearPinGhost.transform.localScale;
			objNearPinGhost.transform.localPosition = new Vector3(0f, 70f, 0f);
			int index = 0;
			int index2 = 0;
			GameObject gameObject4 = objNearPinGhost.transform.GetChild(index).gameObject.transform.GetChild(index2).gameObject;
			_prefabNearPinHand = Resources.Load<GameObject>("Process/MapResult/Prefabs/UI_WalkFriend");
			_objNearPinHand = Object.Instantiate(_prefabNearPinHand);
			GameObject objNearPinHand = _objNearPinHand;
			objNearPinHand.transform.SetParent(gameObject4.transform);
			objNearPinHand.transform.localPosition = _prefabNearPinHand.transform.localPosition;
			objNearPinHand.transform.localRotation = _prefabNearPinHand.transform.localRotation;
			objNearPinHand.transform.localScale = _prefabNearPinHand.transform.localScale;
			gameObject4.transform.localPosition = new Vector3(0f, -70f, 0f);
			objNearPin.SetActive(value: false);
			for (int i = 0; i < objNearPin.transform.childCount; i++)
			{
				objNearPin.transform.GetChild(i).transform.gameObject.SetActive(value: false);
			}
			objNearPinGhost.SetActive(value: false);
		}

		public void InitializeFarPin(GameObject obj)
		{
			_prefabNearPin = Resources.Load<GameObject>("Process/MapResult/Prefabs/Pin_Parts/UI_Pin_Landmark_All");
			GameObject gameObject = new GameObject("EmptyPinParent");
			gameObject.transform.SetParent(obj.transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			gameObject.transform.localScale = Vector3.one;
			float num = 0f;
			GameObject gameObject2 = new GameObject("EmptyPinChild");
			gameObject2.transform.SetParent(gameObject.transform);
			num = -1f * _prefabNearPin.transform.localPosition.y;
			num += -40f;
			gameObject2.transform.localPosition = new Vector3(0f, num, 0f);
			gameObject2.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			gameObject2.transform.localScale = Vector3.one;
			_objFarPin = Object.Instantiate(_prefabNearPin);
			GameObject objFarPin = _objFarPin;
			objFarPin.transform.SetParent(gameObject2.transform);
			objFarPin.transform.localPosition = _prefabNearPin.transform.localPosition;
			objFarPin.transform.localRotation = _prefabNearPin.transform.localRotation;
			objFarPin.transform.localScale = _prefabNearPin.transform.localScale;
			_prefabNearPinGhost = Resources.Load<GameObject>("Process/MapResult/Prefabs/Pin_Parts/UI_Pin_Ghost");
			GameObject gameObject3 = new GameObject("EmptyGhostChild");
			gameObject3.transform.SetParent(gameObject.transform);
			gameObject3.transform.localPosition = new Vector3(0f, 0f, 0f);
			gameObject3.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			gameObject3.transform.localScale = Vector3.one;
			_objFarPinGhost = Object.Instantiate(_prefabNearPinGhost);
			GameObject objFarPinGhost = _objFarPinGhost;
			objFarPinGhost.transform.SetParent(gameObject3.transform);
			objFarPinGhost.transform.localPosition = _prefabNearPinGhost.transform.localPosition;
			objFarPinGhost.transform.localRotation = _prefabNearPinGhost.transform.localRotation;
			objFarPinGhost.transform.localScale = _prefabNearPinGhost.transform.localScale;
			objFarPinGhost.transform.localPosition = new Vector3(0f, 70f, 0f);
			int index = 0;
			int index2 = 0;
			GameObject gameObject4 = objFarPinGhost.transform.GetChild(index).gameObject.transform.GetChild(index2).gameObject;
			_prefabNearPinHand = Resources.Load<GameObject>("Process/MapResult/Prefabs/UI_WalkFriend");
			_objFarPinHand = Object.Instantiate(_prefabNearPinHand);
			GameObject objFarPinHand = _objFarPinHand;
			objFarPinHand.transform.SetParent(gameObject4.transform);
			objFarPinHand.transform.localPosition = _prefabNearPinHand.transform.localPosition;
			objFarPinHand.transform.localRotation = _prefabNearPinHand.transform.localRotation;
			objFarPinHand.transform.localScale = _prefabNearPinHand.transform.localScale;
			gameObject4.transform.localPosition = new Vector3(0f, -70f, 0f);
			objFarPin.SetActive(value: false);
			for (int i = 0; i < objFarPin.transform.childCount; i++)
			{
				objFarPin.transform.GetChild(i).transform.gameObject.SetActive(value: false);
			}
			objFarPinGhost.SetActive(value: false);
		}

		public void InitializeDistanceInfo(GameObject obj, bool isEndless)
		{
			int num = 0;
			int num2 = 0;
			num = 2;
			MultipleImage component = obj.transform.GetChild(num).gameObject.transform.GetChild(1).gameObject.GetComponent<MultipleImage>();
			num2 = 0;
			if (isEndless)
			{
				num2 = 1;
			}
			component.ChangeSprite(num2);
		}

		public void InitializeRoad(int mapID, GameObject obj, float diff1, float diff2)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			num = 1;
			GameObject gameObject = obj.transform.GetChild(num).gameObject;
			Image image = null;
			Transform transform = null;
			Sprite sprite = null;
			num2 = 0;
			GameObject gameObject2 = gameObject.transform.GetChild(num2).gameObject;
			num3 = 0;
			image = gameObject2.transform.GetChild(num3).gameObject.GetComponent<Image>();
			_texSizeBG = image.rectTransform.sizeDelta;
			num3 = 1;
			num4 = 0;
			transform = gameObject2.transform.GetChild(num3).gameObject.transform;
			_posBG_Base[num4] = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
			image = transform.GetComponent<Image>();
			sprite = AssetManager.Instance().GetMapBgSprite(mapID, "UI_RunBG_01");
			image.sprite = Object.Instantiate(sprite);
			num3 = 0;
			num4 = 1;
			transform = gameObject2.transform.GetChild(num3).gameObject.transform;
			_posBG_Base[num4] = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
			image = transform.GetComponent<Image>();
			sprite = AssetManager.Instance().GetMapBgSprite(mapID, "UI_RunBG_01");
			image.sprite = Object.Instantiate(sprite);
			num2 = 1;
			GameObject gameObject3 = gameObject.transform.GetChild(num2).gameObject;
			num3 = 0;
			num4 = 2;
			transform = gameObject3.transform.GetChild(num3).gameObject.transform;
			_posBG_Base[num4] = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
			image = transform.GetComponent<Image>();
			sprite = AssetManager.Instance().GetMapBgSprite(mapID, "UI_RunBG_02");
			image.sprite = Object.Instantiate(sprite);
			num3 = 1;
			num4 = 3;
			transform = gameObject3.transform.GetChild(num3).gameObject.transform;
			_posBG_Base[num4] = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
			image = transform.GetComponent<Image>();
			sprite = AssetManager.Instance().GetMapBgSprite(mapID, "UI_RunBG_02");
			image.sprite = Object.Instantiate(sprite);
			_scrollBase1 = diff1;
			_scrollBase2 = diff2;
			_scroll1 = 0f;
			_scroll2 = 0f;
		}

		public void ScrollRoad(GameObject obj)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			num = 1;
			GameObject gameObject = obj.transform.GetChild(num).gameObject;
			float[] array = new float[4];
			for (int i = 0; i < 2; i++)
			{
				array[i] = _posBG_Base[i].x - 1f * _scroll1;
			}
			for (int j = 2; j < 4; j++)
			{
				array[j] = _posBG_Base[j].x - 1f * _scroll2;
			}
			num2 = 0;
			GameObject gameObject2 = gameObject.transform.GetChild(num2).gameObject;
			num3 = 1;
			Image component = gameObject2.transform.GetChild(num3).gameObject.GetComponent<Image>();
			num4 = 0;
			component.transform.localPosition = new Vector3(array[num4], _posBG_Base[num4].y, 0f);
			num3 = 0;
			Image component2 = gameObject2.transform.GetChild(num3).gameObject.GetComponent<Image>();
			num4 = 1;
			component2.transform.localPosition = new Vector3(array[num4], _posBG_Base[num4].y, 0f);
			num2 = 1;
			GameObject gameObject3 = gameObject.transform.GetChild(num2).gameObject;
			num3 = 0;
			Image component3 = gameObject3.transform.GetChild(num3).gameObject.GetComponent<Image>();
			num4 = 2;
			component3.transform.localPosition = new Vector3(array[num4], _posBG_Base[num4].y, 0f);
			num3 = 1;
			Image component4 = gameObject3.transform.GetChild(num3).gameObject.GetComponent<Image>();
			num4 = 3;
			component4.transform.localPosition = new Vector3(array[num4], _posBG_Base[num4].y, 0f);
			_scroll1 += _scrollBase1;
			_scroll2 += _scrollBase2;
			if (_scroll1 >= _texSizeBG.x)
			{
				_scroll1 -= _texSizeBG.x;
			}
			if (_scroll2 >= _texSizeBG.x)
			{
				_scroll2 -= _texSizeBG.x;
			}
		}

		public void InitializeRainbow(AssetManager manager, List<MapTreasureExData> list, uint current, uint total, bool isEndless)
		{
			_paramTotalTime = 120u;
			_paramMinWaitTime0 = 60u;
			_paramMinWaitTime1 = 45u;
			_paramMinWaitTime2 = 30u;
			_paramAddFrame = 12u;
			_paramDispPercent = 80u;
			_paramRainbowMinDiff = 1E-08f;
			_paramNearPinStart = 500f;
			_paramNearPinReached = 70f;
			_paramNearPinUnreached = 150f;
			int num = 0;
			List<MapTreasureType> list2 = new List<MapTreasureType>();
			int num2 = 0;
			foreach (MapTreasureExData item4 in list)
			{
				num = item4.TreasureId.id;
				if (_nextOtomodachiInsertID != -1 && num2 == _nextOtomodachiInsertID)
				{
					list2.Add(MapTreasureType.Otomodachi);
				}
				else if (Singleton<DataManager>.Instance.GetMapTreasureData(num) == null)
				{
					list2.Add((MapTreasureType)(-1));
				}
				else
				{
					list2.Add(Singleton<DataManager>.Instance.GetMapTreasureData(num).TreasureType);
				}
				num2++;
			}
			List<uint> list3 = new List<uint>();
			List<uint> list4 = new List<uint>();
			if (list[0].Distance == 0)
			{
				list4.Add((uint)list[0].Distance);
			}
			else
			{
				list4.Add(0u);
			}
			foreach (var item5 in list.Select((MapTreasureExData value, int index) => new { value, index }))
			{
				int index2 = item5.index;
				uint distance = (uint)item5.value.Distance;
				distance = GetArrangedDistance(distance, index2);
				if (list2[index2] == MapTreasureType.Otomodachi)
				{
					if (_nextOtomodachiInsertID != -1 && index2 == _nextOtomodachiInsertID && index2 == list.Count - 1)
					{
						list3.Add(distance);
					}
					else
					{
						list3.Add(0u);
					}
					list4.Add(0u);
					_rainbowPinIcon.Add(-1);
				}
				else
				{
					list3.Add(distance);
					list4.Add(distance);
					_rainbowPinIcon.Add(index2);
				}
			}
			List<uint> list5 = null;
			List<int> list6 = null;
			list5 = list3;
			uint value2 = list5[0];
			list5.Reverse();
			uint value3 = list5[0];
			for (int i = 0; i < list3.Count; i++)
			{
				if (list5[i] == 0)
				{
					list5[i] = value3;
				}
				else
				{
					value3 = list5[i];
				}
			}
			list5.Reverse();
			list5[0] = value2;
			list5 = list4;
			value3 = list5[1];
			for (int j = 1; j < list5.Count; j++)
			{
				if (list5[j] == 0)
				{
					list5[j] = value3;
				}
				else
				{
					value3 = list5[j];
				}
			}
			list6 = _rainbowPinIcon;
			int value4 = list6[0];
			list6.Reverse();
			int value5 = list6[0];
			for (int k = 0; k < list3.Count; k++)
			{
				if (list6[k] == -1)
				{
					list6[k] = value5;
				}
				else
				{
					value5 = list6[k];
				}
			}
			list6.Reverse();
			list6[0] = value4;
			list5 = null;
			list6 = null;
			value3 = 0u;
			value5 = 0;
			for (int l = 0; l < list3.Count; l++)
			{
				int item = (int)(list3[l] - list4[l]);
				_distanceBase.Add(item);
			}
			for (int m = 0; m < list3.Count; m++)
			{
				_distanceStart.Add(list4[m]);
				_distanceEnd.Add(list3[m]);
			}
			uint num3 = total - current;
			uint num4 = num3 / (_paramTotalTime - 1);
			uint num5 = num4;
			foreach (var item6 in list.Select((MapTreasureExData value, int index) => new { value, index }))
			{
				int index3 = item6.index;
				uint distance2 = (uint)item6.value.Distance;
				CommonValue item2 = new CommonValue();
				_valueListRainbow.Add(item2);
				_valueListRainbow[index3].start = 0f;
				_valueListRainbow[index3].end = 100f;
				CommonValueUint item3 = new CommonValueUint();
				_valueListRainbowUint.Add(item3);
				_valueListRainbowUint[index3].start = 0u;
				_valueListRainbowUint[index3].end = (uint)_distanceBase[index3];
				distance2 = GetArrangedDistance(distance2, index3);
				_valueListRainbowCountUpRate.Add(0u);
				uint num6 = 0u;
				num6 = list4[index3];
				uint num7 = list3[index3];
				uint num8 = distance2 - num6;
				uint num9 = num7 - num6;
				float num10 = num8;
				float num11 = num9;
				_valueListRainbow[index3].diff = 100f / (float)_paramTotalTime;
				if (list2[index3] == MapTreasureType.Otomodachi && num11 != 0f)
				{
					_valueListRainbow[index3].end = num10 / num11 * 100f;
					_valueListRainbowUint[index3].end = num8;
				}
			}
			foreach (var item7 in list.Select((MapTreasureExData value, int index) => new { value, index }))
			{
				int index4 = item7.index;
				if (list2[index4] == MapTreasureType.Otomodachi && index4 != 0)
				{
					if (_nextOtomodachiInsertID != -1 && index4 == _nextOtomodachiInsertID && index4 == list.Count - 1)
					{
						_valueListRainbow[index4].start = 0f;
						_valueListRainbowUint[index4].start = 0u;
					}
					else if (index4 < list2.Count)
					{
						_valueListRainbow[index4 + 1].start = _valueListRainbow[index4].end;
						_valueListRainbowUint[index4 + 1].start = _valueListRainbowUint[index4].end;
					}
				}
			}
			_isSetPosNearPin = false;
			_isSetPosFarPin = false;
			_isUpdateNearPin = false;
			_isUpdateFarPin = false;
			_rainbowEnd = 0;
			foreach (var item8 in list.Select((MapTreasureExData value, int index) => new { value, index }))
			{
				int index5 = item8.index;
				uint distance3 = (uint)item8.value.Distance;
				distance3 = GetArrangedDistance(distance3, index5);
				if (total > distance3)
				{
					_rainbowEnd = index5 + 1;
				}
			}
			num = _rainbowEnd;
			uint num12 = 0u;
			bool flag = false;
			if (num >= list3.Count && list3.Count > 0 && total >= list3[list3.Count - 1])
			{
				flag = true;
			}
			if (flag)
			{
				_valueListRainbow[list3.Count - 1].end = 100f;
				_valueListRainbowUint[list3.Count - 1].end = (uint)_distanceBase[list3.Count - 1];
			}
			else
			{
				num12 = list4[num];
				uint num13 = list3[num];
				uint num14 = total - num12;
				uint num15 = num13 - num12;
				float num16 = num14;
				float num17 = num15;
				if (num17 != 0f)
				{
					_valueListRainbow[num].end = num16 / num17 * 100f;
					if (num16 == num17 && num14 != num15)
					{
						_valueListRainbow[num].end = 99.9f;
					}
					_valueListRainbowUint[num].end = num14;
				}
			}
			_rainbowEnd++;
			_rainbowChallengeStart = false;
			_isChallenge = false;
			_isCommonStart = true;
			_isChallengeDifficultyOK = false;
			_isChallengeClear = false;
			_rainbowStart = 0;
			foreach (var item9 in list.Select((MapTreasureExData value, int index) => new { value, index }))
			{
				int index6 = item9.index;
				uint distance4 = (uint)item9.value.Distance;
				distance4 = GetArrangedDistance(distance4, index6);
				if (current == 0 && distance4 == 0)
				{
					_rainbowStart = 0;
				}
				else
				{
					if (current < distance4)
					{
						continue;
					}
					bool flag2 = true;
					if (current == distance4)
					{
						MapTreasureData mapTreasureData = Singleton<DataManager>.Instance.GetMapTreasureData(item9.value.TreasureId.id);
						if (mapTreasureData != null && (mapTreasureData.TreasureType == MapTreasureType.MapTaskMusic || mapTreasureData.TreasureType == MapTreasureType.Challenge))
						{
							_rainbowChallengeStart = true;
							_rainbowStart = index6 + 1;
							_rainbowStart = index6;
							flag2 = false;
						}
					}
					if (flag2)
					{
						_rainbowStart = index6 + 1;
					}
				}
			}
			if (_rainbowStart == 0)
			{
				_rainbowStart++;
			}
			num = _rainbowStart;
			uint num18 = 0u;
			bool flag3 = false;
			if (num >= list3.Count && list3.Count > 0 && current >= list3[list3.Count - 1])
			{
				flag3 = true;
				_isInfinityLastPinOver = true;
			}
			if (flag3)
			{
				_valueListRainbow[list3.Count - 1].start = 100f;
				_valueListRainbowUint[list3.Count - 1].start = (uint)_distanceBase[list3.Count - 1];
			}
			else
			{
				num18 = list4[num];
				uint num19 = list3[num];
				uint num20 = current - num18;
				uint num21 = num19 - num18;
				float num22 = num20;
				float num23 = num21;
				if (num23 != 0f)
				{
					_valueListRainbow[num].start = num22 / num23 * 100f;
					if (num22 == num23 && num20 != num21)
					{
						_valueListRainbow[num].start = 99.9f;
					}
					_valueListRainbowUint[num].start = num20;
				}
			}
			for (int n = 0; n < _valueListRainbow.Count; n++)
			{
				_valueListRainbow[n].current = _valueListRainbow[n].start;
				_valueListRainbowUint[n].current = _valueListRainbowUint[n].start;
			}
			foreach (var item10 in list.Select((MapTreasureExData value, int index) => new { value, index }))
			{
				int index7 = item10.index;
				float num24 = _valueListRainbow[index7].end - _valueListRainbow[index7].start;
				uint num25 = 0u;
				num25 = list4[index7];
				num3 = list3[index7] - num25;
				float num26 = (float)num3 * (num24 / 100f);
				float num27 = 1f;
				if (num4 == 0)
				{
					num4 = 1u;
				}
				num27 = ((num26 != 0f) ? (num26 / (float)num4) : 1f);
				uint num28 = 0u;
				num28 = _paramMinWaitTime2;
				if (num24 >= 30f)
				{
					num28 = _paramMinWaitTime1;
				}
				if (num24 >= 60f)
				{
					num28 = _paramMinWaitTime0;
				}
				if (num27 < (float)num28)
				{
					num5 = num3 / num28;
					if (num5 == 0)
					{
						num5 = 1u;
					}
					num27 = ((num26 != 0f) ? (num26 / (float)num5) : ((float)num28));
					if (num27 < (float)num28)
					{
						num27 = num28;
					}
				}
				else if (index7 >= _rainbowStart && num27 > (float)num28)
				{
					num27 = num28;
				}
				uint num29 = 0u;
				num29 = (uint)item10.value.Distance;
				num29 = GetArrangedDistance(num29, index7);
				if (total >= num29 && num27 < (float)num28)
				{
					num27 = num28;
				}
				_valueListRainbow[index7].diff = 100f / num27;
				uint num30 = _valueListRainbowUint[index7].end - _valueListRainbowUint[index7].start;
				uint num31 = 0u;
				_valueListRainbowUint[index7].diff = num30 / (uint)num27;
				if (_valueListRainbowUint[index7].diff == 0)
				{
					_valueListRainbowUint[index7].diff = 1u;
					if (num30 != 0 && num30 < (uint)num27)
					{
						num31 = (uint)num27 / num30;
						if (num31 * num30 < (uint)num27)
						{
							num31++;
						}
						_valueListRainbowCountUpRate[index7] = num31;
					}
				}
				_valueListRainbow[index7].diff = num24 / num27;
				if (_valueListRainbow[index7].diff <= _paramRainbowMinDiff)
				{
					_valueListRainbow[index7].diff = _paramRainbowMinDiff;
				}
			}
			_rainbowID = _rainbowStart;
			if (_rainbowID >= _valueListRainbow.Count)
			{
				_rainbowID = _valueListRainbow.Count - 1;
				_rainbowStart = _rainbowID;
				_rainbowEnd = _rainbowID + 1;
			}
			_rainbowReleasePin = list.FindIndex((MapTreasureExData data) => data.Flag == MapTreasureFlag.ReleaseFlag);
			_isJustRainbow = false;
			_isJustNextRainbowID = -1;
			if (isEndless)
			{
				return;
			}
			foreach (var item11 in list3.Select((uint value, int index) => new { value, index }))
			{
				int index8 = item11.index;
				if (item11.value != 0 && item11.value == total && index8 < list3.Count - 1)
				{
					_isJustRainbow = true;
					_isJustNextRainbowID = index8 + 1;
					break;
				}
			}
		}

		public void SetBonusIcon(AssetManager manager, GameObject obj, List<Animator> _animList, List<List<BonusNumber>> _listList)
		{
			GameObject gameObject = obj.transform.GetChild(2).gameObject;
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				Animator component = gameObject.transform.GetChild(i).GetChild(0).gameObject.GetComponent<Animator>();
				_animList.Add(component);
				GameObject gameObject2 = _animList[i].gameObject.transform.GetChild(0).gameObject;
				SetActiveChildren(gameObject2, active: false);
				int index = i;
				if (i == 3)
				{
					index = 4;
				}
				gameObject2.transform.GetChild(index).gameObject.SetActive(value: true);
				_animList[i].gameObject.transform.GetChild(1).gameObject.SetActive(value: false);
				GameObject gameObject3 = null;
				List<BonusNumber> list = new List<BonusNumber>();
				int index2 = 0;
				gameObject3 = gameObject2.transform.GetChild(index2).GetChild(2).GetChild(1)
					.GetChild(0)
					.gameObject;
				InitializeBonusIconPoint(gameObject3, list);
				index2 = 1;
				gameObject3 = gameObject2.transform.GetChild(index2).GetChild(2).GetChild(1)
					.GetChild(0)
					.gameObject;
				InitializeBonusIconPoint(gameObject3, list);
				index2 = 2;
				gameObject3 = gameObject2.transform.GetChild(index2).GetChild(3).GetChild(1)
					.GetChild(0)
					.gameObject;
				InitializeBonusIconPoint(gameObject3, list);
				index2 = 4;
				gameObject3 = gameObject2.transform.GetChild(index2).GetChild(3).GetChild(2)
					.GetChild(0)
					.gameObject;
				InitializeBonusIconPoint(gameObject3, list);
				_listList.Add(list);
			}
			for (int j = 0; j < gameObject.transform.childCount; j++)
			{
				int iD = _dispInfo[j].ID;
				Image image = null;
				Texture2D texture2D = null;
				MultiImage multiImage = null;
				Sprite sprite = null;
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				switch (_dispInfo[j].bonusKind)
				{
				case BounusIconDispInfo.BonusKind.BonusKindChara:
				{
					GameObject gameObject4 = _animList[j].gameObject.transform.GetChild(0).GetChild(0).gameObject;
					num3 = Singleton<DataManager>.Instance.GetChara(iD).genre.id;
					multiImage = gameObject4.transform.GetChild(0).GetChild(0).gameObject.GetComponent<MultiImage>();
					sprite = AssetManager.Instance().GetMapBgSprite(num3, "UI_Chara_Base_S");
					if (sprite != null)
					{
						multiImage.sprite = Object.Instantiate(sprite);
					}
					image = gameObject4.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Image>();
					int mapId = 1;
					sprite = AssetManager.Instance().GetMapBgSprite(mapId, "UI_Chara_Base_S");
					if (sprite != null)
					{
						image.sprite = Object.Instantiate(sprite);
					}
					multiImage = gameObject4.transform.GetChild(0).GetChild(1).GetChild(1)
						.gameObject.GetComponent<MultiImage>();
					num = iD;
					texture2D = manager.GetCharacterTexture2D(num);
					sprite = (multiImage.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero));
					multiImage = gameObject4.transform.GetChild(0).GetChild(1).GetChild(0)
						.gameObject.GetComponent<MultiImage>();
					multiImage.Image2 = sprite;
					num2 = Singleton<DataManager>.Instance.GetChara(iD).color.id;
					Color24 colorDark = Singleton<DataManager>.Instance.GetMapColorData(num2).ColorDark;
					Color color2 = (multiImage.color = new Color((float)(int)colorDark.R / 255f, (float)(int)colorDark.G / 255f, (float)(int)colorDark.B / 255f));
					image = gameObject4.transform.GetChild(0).GetChild(2).gameObject.GetComponent<Image>();
					sprite = AssetManager.Instance().GetMapBgSprite(num3, "UI_Chara_Frame_S");
					if (sprite != null)
					{
						image.sprite = Object.Instantiate(sprite);
					}
					break;
				}
				case BounusIconDispInfo.BonusKind.BonusKindPlayBonusMusic:
					image = _animList[j].gameObject.transform.GetChild(0).GetChild(2).gameObject.transform.GetChild(1).gameObject.GetComponent<Image>();
					num = iD;
					texture2D = manager.GetJacketTexture2D(num);
					image.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
					break;
				case BounusIconDispInfo.BonusKind.BonusKindCleaRank:
				{
					string text2 = "UI_CHR_PlayBonus_Sync";
					switch (iD)
					{
					case 0:
						text2 = "UI_CHR_PlayBonus_AP";
						break;
					case 1:
						text2 = "UI_CHR_PlayBonus_FSD";
						break;
					case 2:
						text2 = "UI_CHR_PlayBonus_FSDp";
						break;
					case 3:
						text2 = "UI_CHR_PlayBonus_FS";
						break;
					case 4:
						text2 = "UI_CHR_PlayBonus_FSp";
						break;
					case 5:
						text2 = "UI_CHR_PlayBonus_FC";
						break;
					case 6:
						text2 = "UI_CHR_PlayBonus_FCp";
						break;
					case 7:
						text2 = "UI_CHR_PlayBonus_AP";
						break;
					case 8:
						text2 = "UI_CHR_PlayBonus_APp";
						break;
					case 9:
						text2 = "UI_CHR_PlayBonus_Sync";
						break;
					}
					texture2D = Resources.Load<Sprite>("Process/MapResult/Sprites/Bonus/PlayBonus/" + text2).texture;
					sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
					image = _animList[j].gameObject.transform.GetChild(0).GetChild(1).gameObject.transform.GetChild(0).GetComponent<Image>();
					image.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
					break;
				}
				case BounusIconDispInfo.BonusKind.BonusKindTicketGrade:
				{
					bool flag = false;
					TicketData ticketData = null;
					string text = "UI_CMN_Tix_Icon_NoTicket_S";
					if ((uint)(iD - -1) <= 1u)
					{
						flag = true;
					}
					if (!flag)
					{
						ticketData = Singleton<DataManager>.Instance.GetTicket(iD);
						if (ticketData != null)
						{
							switch (ConvertTicketKind(ticketData))
							{
							case TicketKind.Free:
								text = "UI_CMN_Tix_Icon_Free_S";
								break;
							case TicketKind.Event:
								text = "UI_CMN_Tix_Icon_Event_S";
								break;
							case TicketKind.Paid:
								text = "UI_CMN_Tix_Icon_Paid_01_S";
								switch (ticketData.areaPercent)
								{
								case 200:
									text = "UI_CMN_Tix_Icon_Paid_01_S";
									break;
								case 300:
									text = "UI_CMN_Tix_Icon_Paid_02_S";
									break;
								}
								break;
							}
						}
					}
					image = _animList[j].gameObject.transform.GetChild(0).GetChild(4).gameObject.transform.GetChild(1).gameObject.GetComponent<Image>();
					texture2D = Resources.Load<Sprite>("Common/Sprites/Ticket/" + text).texture;
					sprite = (image.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero));
					break;
				}
				}
			}
		}

		public void InitializeRestPinMeter(GameObject meter)
		{
			_restPinMeterChildMax = meter.transform.childCount;
			for (int i = 0; i < _restPinMeterChildMax; i++)
			{
				Transform transform = meter.transform;
				int tex_type = i;
				if (i == 3)
				{
					tex_type = 2;
				}
				OdometerNumber item = new OdometerNumber(transform.GetChild(i).gameObject, another: false, tex_type);
				_objListRestPinMeter.Add(item);
			}
			_restPinMeterValue = 0;
		}

		public void InitializeTotalMapRunMeter(GameObject meter)
		{
			_totalMapRunMeterChildMax = meter.transform.childCount;
			for (int i = 0; i < _totalMapRunMeterChildMax; i++)
			{
				Transform transform = meter.transform;
				int tex_type = i;
				if (i == 0)
				{
					tex_type = 2;
				}
				OdometerNumber item = new OdometerNumber(transform.GetChild(i).gameObject, another: false, tex_type);
				_objListTotalMapRunMeter.Add(item);
			}
			_totalMapRunMeterValue = 66798776u;
		}

		public void InitializeTotalBonus(GameObject meter)
		{
			_toatalBonusMeterChildMax = meter.transform.childCount;
			for (int i = 0; i < _toatalBonusMeterChildMax; i++)
			{
				Transform transform = meter.transform;
				int tex_type = i + 1 + 210;
				OdometerNumber item = new OdometerNumber(transform.GetChild(i).gameObject, another: false, tex_type);
				_objListTotalBonusMeter.Add(item);
			}
			_toatalBonusMeterValue = 0u;
		}

		public void InitializeBonusIconPoint(GameObject point, List<BonusNumber> list)
		{
			BonusNumber item = new BonusNumber(point.gameObject);
			list.Add(item);
		}

		public string GetWindowPrefabName(int type)
		{
			string result = "";
			switch (type)
			{
			case 0:
				result = "UI_CharaAwakeWindow";
				break;
			case 1:
				result = "UI_CharaWindow";
				break;
			case 2:
				result = "UI_ClearWindow";
				break;
			case 3:
				result = "UI_CollectionWindow";
				break;
			case 4:
				result = "UI_IslandWindow";
				break;
			case 5:
				result = "UI_MusicWindow";
				break;
			case 6:
				result = "UI_UnlockMusicWindow";
				break;
			case 7:
				result = "UI_UnlockMusicWindow_PFC";
				break;
			}
			return result;
		}

		public void InitializePrize(GameObject parent, List<Animator> _root, List<Animator> _star, List<Animator> _star_eff, List<Animator> _star_spark)
		{
			string text = "";
			for (int i = 0; i < 8; i++)
			{
				text = GetWindowPrefabName(i);
				_prefabPrize = Resources.Load<GameObject>("Common/Prefabs/GetWindow/" + text);
				SetEmptyWithPrefab(parent.transform, _nullListPrize, _objListPrize, _prefabPrize);
				SetActiveChildren(_objListPrize[i], active: false);
				if (i == 0)
				{
					GameObject gameObject = null;
					GameObject gameObject2 = null;
					GameObject gameObject3 = null;
					gameObject2 = _objListPrize[i].transform.GetChild(6).gameObject;
					gameObject3 = Resources.Load<GameObject>("Common/Prefabs/UI_CMN_CharaStar");
					gameObject = Object.Instantiate(gameObject3);
					gameObject.transform.SetParent(gameObject2.transform);
					_root.Add(gameObject.GetComponent<Animator>());
					_root[0].gameObject.transform.localPosition = Vector3.zero;
					for (int j = 1; j <= 5; j++)
					{
						_star.Add(gameObject.transform.GetChild(j).gameObject.GetComponent<Animator>());
					}
					gameObject2 = gameObject2.transform.GetChild(0).gameObject.transform.GetChild(6).gameObject;
					gameObject3 = Resources.Load<GameObject>("Common/Prefabs/EFF_CharaStar_GetStar");
					for (int k = 0; k < gameObject2.transform.childCount; k++)
					{
						gameObject = Object.Instantiate(gameObject3);
						gameObject.transform.SetParent(gameObject2.transform.GetChild(k).gameObject.transform);
						_star_eff.Add(gameObject.GetComponent<Animator>());
						_star_eff[k].gameObject.transform.localPosition = Vector3.zero;
						gameObject.SetActive(value: false);
					}
					int index = gameObject2.transform.childCount - 1;
					gameObject3 = Resources.Load<GameObject>("Common/Prefabs/EFF_CharaStar_Spark");
					gameObject = Object.Instantiate(gameObject3);
					gameObject.transform.SetParent(gameObject2.transform.GetChild(index).gameObject.transform);
					_star_spark.Add(gameObject.GetComponent<Animator>());
					_star_spark[0].gameObject.transform.localPosition = Vector3.zero;
					gameObject.SetActive(value: false);
				}
			}
		}

		public string GetChallengeInfoPrefabName(int type)
		{
			string result = "";
			switch (type)
			{
			case 0:
				result = "UI_PFC_Dialog_01";
				break;
			case 1:
				result = "UI_PFC_Dialog_02";
				break;
			}
			return result;
		}

		public void ReplaceText(int type)
		{
			switch (type)
			{
			case 0:
			{
				Transform child2 = _objListChallengeInfo[type].transform.GetChild(1).GetChild(0);
				child2.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>()
					.text = CommonMessageID.ChallengeInfoAssignmentTitle.GetName();
				child2.GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>()
					.text = CommonMessageID.ChallengeInfoAssignmentMessage01.GetName();
				child2.GetChild(6).GetComponent<TextMeshProUGUI>().text = CommonMessageID.ChallengeInfoAssignmentMessage02.GetName();
				break;
			}
			case 1:
			{
				Transform child = _objListChallengeInfo[type].transform.GetChild(1).GetChild(0);
				child.GetChild(3).GetChild(3).GetComponent<TextMeshProUGUI>()
					.text = CommonMessageID.ChallengeInfoPerfectTitle.GetName();
				child.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>()
					.text = CommonMessageID.ChallengeInfoPerfectMessage01.GetName();
				child.GetChild(5).GetComponent<TextMeshProUGUI>().text = CommonMessageID.ChallengeInfoPerfectMessage02.GetName();
				break;
			}
			}
		}

		public void InitializeChallengeInfo(GameObject parent)
		{
			string text = "";
			for (int i = 0; i < 2; i++)
			{
				text = GetChallengeInfoPrefabName(i);
				_prefabChallengeInfo = Resources.Load<GameObject>("Common/Prefabs/PerfectChallenge/" + text);
				SetEmptyWithPrefab(parent.transform, _nullListChallengeInfo, _objListChallengeInfo, _prefabChallengeInfo);
				float y = 420f;
				_objListChallengeInfo[i].transform.localPosition = new Vector3(0f, y, 0f);
				SetActiveChildren(_objListChallengeInfo[i], active: false);
				ReplaceText(i);
			}
		}

		public int GetChallengeID(MapData mapData, List<MapTreasureExData> list, int current)
		{
			int result = -1;
			int convertedTreasureID = GetConvertedTreasureID(mapData, list, current, otomodachi: true);
			MapTreasureData mapTreasureData = Singleton<DataManager>.Instance.GetMapTreasureData(convertedTreasureID);
			if (mapTreasureData != null)
			{
				MapTreasureType treasureType = mapTreasureData.TreasureType;
				if ((uint)(treasureType - 7) <= 1u)
				{
					int challengeInfoType = _challengeInfoType;
					if (challengeInfoType != 0 && challengeInfoType == 1)
					{
						result = mapTreasureData.Challenge.id;
					}
				}
			}
			return result;
		}

		public int GetChallengeMusicID(MapData mapData, List<MapTreasureExData> list, int current)
		{
			int result = -1;
			int convertedTreasureID = GetConvertedTreasureID(mapData, list, current, otomodachi: true);
			MapTreasureData mapTreasureData = Singleton<DataManager>.Instance.GetMapTreasureData(convertedTreasureID);
			if (mapTreasureData != null)
			{
				ChallengeData challengeData = null;
				MapTreasureType treasureType = mapTreasureData.TreasureType;
				if ((uint)(treasureType - 7) <= 1u)
				{
					switch (_challengeInfoType)
					{
					case 0:
						result = mapTreasureData.MusicId.id;
						break;
					case 1:
						challengeData = Singleton<DataManager>.Instance.GetChallengeData(mapTreasureData.Challenge.id);
						if (challengeData != null)
						{
							result = challengeData.Music.id;
						}
						break;
					}
				}
			}
			return result;
		}

		public void SetChallengeAppearedInfo(AssetManager manager, MapData mapData, List<MapTreasureExData> list, GameObject info, int current)
		{
			int convertedTreasureID = GetConvertedTreasureID(mapData, list, current, otomodachi: true);
			MapTreasureData mapTreasureData = Singleton<DataManager>.Instance.GetMapTreasureData(convertedTreasureID);
			if (mapTreasureData == null)
			{
				return;
			}
			Image image = null;
			Texture2D texture2D = null;
			ChallengeData challengeData = null;
			int num = 0;
			MapTreasureType treasureType = mapTreasureData.TreasureType;
			if ((uint)(treasureType - 7) > 1u)
			{
				return;
			}
			switch (_challengeInfoType)
			{
			case 0:
				image = info.transform.GetChild(1).GetChild(0).GetChild(7)
					.GetChild(1)
					.gameObject.GetComponent<Image>();
				num = mapTreasureData.MusicId.id;
				texture2D = manager.GetJacketTexture2D(num);
				image.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
				break;
			case 1:
				image = info.transform.GetChild(1).GetChild(0).GetChild(6)
					.GetChild(1)
					.gameObject.GetComponent<Image>();
				challengeData = Singleton<DataManager>.Instance.GetChallengeData(mapTreasureData.Challenge.id);
				if (challengeData != null)
				{
					num = challengeData.Music.id;
					texture2D = manager.GetJacketTexture2D(num);
					image.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
				}
				break;
			}
		}

		public void SetChallengeLockedInfo(AssetManager manager, MapData mapData, List<MapTreasureExData> list, GameObject info, int current)
		{
			int convertedTreasureID = GetConvertedTreasureID(mapData, list, current, otomodachi: true);
			MapTreasureData mapTreasureData = Singleton<DataManager>.Instance.GetMapTreasureData(convertedTreasureID);
			if (mapTreasureData == null)
			{
				return;
			}
			Image image = null;
			Texture2D texture2D = null;
			ChallengeData challengeData = null;
			int num = 0;
			MapTreasureType treasureType = mapTreasureData.TreasureType;
			if ((uint)(treasureType - 7) > 1u)
			{
				return;
			}
			switch (_challengeInfoType)
			{
			case 0:
				info.transform.GetChild(0).GetChild(5).GetChild(0)
					.gameObject.GetComponent<MultipleImage>().ChangeSprite(0);
				image = info.transform.GetChild(0).GetChild(5).GetChild(1)
					.gameObject.GetComponent<Image>();
				num = mapTreasureData.MusicId.id;
				texture2D = manager.GetJacketTexture2D(num);
				image.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
				break;
			case 1:
				info.transform.GetChild(0).GetChild(5).GetChild(0)
					.gameObject.GetComponent<MultipleImage>().ChangeSprite(1);
				image = info.transform.GetChild(0).GetChild(5).GetChild(1)
					.gameObject.GetComponent<Image>();
				challengeData = Singleton<DataManager>.Instance.GetChallengeData(mapTreasureData.Challenge.id);
				if (challengeData != null)
				{
					num = challengeData.Music.id;
					texture2D = manager.GetJacketTexture2D(num);
					image.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
				}
				break;
			}
		}

		public void SetOtomodachiMusic(MapData mapData, List<MapTreasureExData> list, uint total, int monitor, List<int> music)
		{
			for (int i = _rainbowStart; i < _rainbowEnd; i++)
			{
				uint num = 0u;
				if (i < list.Count)
				{
					num = (uint)list[i].Distance;
					num = GetArrangedDistance(num, i);
				}
				if (i < list.Count && total >= num)
				{
					int convertedTreasureID = GetConvertedTreasureID(mapData, list, i, otomodachi: true);
					MapTreasureData mapTreasureData = Singleton<DataManager>.Instance.GetMapTreasureData(convertedTreasureID);
					music.Add(-1);
					if (mapTreasureData != null)
					{
						MapTreasureType treasureType = mapTreasureData.TreasureType;
						_ = 4;
					}
				}
			}
		}

		public void SetPin(AssetManager manager, MapData mapData, List<MapTreasureExData> list, int current, int otomodachi_music_id, bool reached, bool near)
		{
			int num = 0;
			num = GetConvertedTreasureID(mapData, list, current, otomodachi: true);
			MapTreasureData mapTreasureData = Singleton<DataManager>.Instance.GetMapTreasureData(num);
			GameObject gameObject = null;
			GameObject gameObject2 = null;
			if (near)
			{
				gameObject = _objNearPin;
				gameObject2 = _objNearPinGhost;
			}
			else
			{
				gameObject = _objFarPin;
				gameObject2 = _objFarPinGhost;
			}
			gameObject.SetActive(value: false);
			gameObject2.SetActive(value: false);
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			GameObject gameObject3 = null;
			Texture2D texture2D = null;
			int num5 = 0;
			if (mapTreasureData == null)
			{
				return;
			}
			switch (mapTreasureData.TreasureType)
			{
			case MapTreasureType.Character:
			{
				num2 = 2;
				gameObject3 = gameObject.transform.GetChild(num2).gameObject;
				for (int k = 0; k < gameObject3.transform.childCount; k++)
				{
					gameObject3.transform.GetChild(k).gameObject.SetActive(value: false);
				}
				num3 = 1;
				GameObject gameObject6 = gameObject3.transform.GetChild(num3).gameObject;
				num4 = 0;
				Image component2 = gameObject6.transform.GetChild(num4).gameObject.GetComponent<Image>();
				num5 = mapTreasureData.CharacterId.id;
				texture2D = manager.GetCharacterTexture2D(num5);
				component2.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
				if (!reached)
				{
					gameObject3.gameObject.GetComponent<MultipleImage>().ChangeSprite(1);
				}
				else
				{
					gameObject3.gameObject.GetComponent<MultipleImage>().ChangeSprite(0);
				}
				gameObject.SetActive(value: true);
				gameObject3.SetActive(value: true);
				gameObject6.SetActive(value: true);
				break;
			}
			case MapTreasureType.MusicNew:
			{
				num2 = 2;
				gameObject3 = gameObject.transform.GetChild(num2).gameObject;
				for (int j = 0; j < gameObject3.transform.childCount; j++)
				{
					gameObject3.transform.GetChild(j).gameObject.SetActive(value: false);
				}
				num3 = 0;
				GameObject gameObject5 = gameObject3.transform.GetChild(num3).gameObject;
				Image component = gameObject5.GetComponent<Image>();
				num5 = mapTreasureData.MusicId.id;
				texture2D = manager.GetJacketTexture2D(num5);
				component.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
				if (!reached)
				{
					gameObject3.gameObject.GetComponent<MultipleImage>().ChangeSprite(3);
				}
				else
				{
					gameObject3.gameObject.GetComponent<MultipleImage>().ChangeSprite(2);
				}
				gameObject.SetActive(value: true);
				gameObject3.SetActive(value: true);
				gameObject5.SetActive(value: true);
				break;
			}
			case MapTreasureType.NamePlate:
			case MapTreasureType.Frame:
			{
				num2 = 2;
				gameObject3 = gameObject.transform.GetChild(num2).gameObject;
				for (int i = 0; i < gameObject3.transform.childCount; i++)
				{
					gameObject3.transform.GetChild(i).gameObject.SetActive(value: false);
				}
				num3 = 2;
				GameObject gameObject4 = gameObject3.transform.GetChild(num3).gameObject;
				if (!reached)
				{
					gameObject3.gameObject.GetComponent<MultipleImage>().ChangeSprite(5);
				}
				else
				{
					gameObject3.gameObject.GetComponent<MultipleImage>().ChangeSprite(4);
				}
				gameObject.SetActive(value: true);
				gameObject3.SetActive(value: true);
				gameObject4.SetActive(value: true);
				break;
			}
			case MapTreasureType.LevelUpAll:
			case MapTreasureType.Otomodachi:
				break;
			}
		}

		public void SetNearPin(AssetManager manager, MapData mapData, List<MapTreasureExData> list, int current, int otomodachi_music_id, bool reached)
		{
			bool near = true;
			SetPin(manager, mapData, list, current, otomodachi_music_id, reached, near);
		}

		public void SetFarPin(AssetManager manager, MapData mapData, List<MapTreasureExData> list, int current, int otomodachi_music_id, bool reached)
		{
			bool near = false;
			SetPin(manager, mapData, list, current, otomodachi_music_id, reached, near);
		}

		public void SetRainbowPin(AssetManager manager, List<MapTreasureExData> list, GameObject pin, int current)
		{
			int num = _rainbowPinIcon[current];
			if (num < 0 || num >= list.Count)
			{
				return;
			}
			int id = list[num].TreasureId.id;
			MapTreasureData mapTreasureData = Singleton<DataManager>.Instance.GetMapTreasureData(id);
			int index = 0;
			int index2 = 0;
			if (mapTreasureData != null)
			{
				switch (mapTreasureData.TreasureType)
				{
				case MapTreasureType.Character:
					index = 0;
					index2 = 1;
					break;
				case MapTreasureType.MusicNew:
					index = 1;
					index2 = 1;
					break;
				case MapTreasureType.MapTaskMusic:
				case MapTreasureType.Challenge:
					index = 1;
					index2 = 1;
					break;
				case MapTreasureType.NamePlate:
				case MapTreasureType.Frame:
					index = 2;
					break;
				}
			}
			GameObject gameObject = null;
			for (int i = 0; i < pin.transform.childCount; i++)
			{
				gameObject = pin.transform.GetChild(i).gameObject;
				gameObject.SetActive(value: false);
				for (int j = 0; j < gameObject.transform.childCount; j++)
				{
					gameObject.transform.GetChild(j).gameObject.SetActive(value: false);
				}
			}
			pin.transform.GetChild(index).gameObject.SetActive(value: true);
			gameObject = pin.transform.GetChild(index).gameObject;
			Image image = null;
			Texture2D texture2D = null;
			ChallengeData challengeData = null;
			int num2 = 0;
			if (mapTreasureData == null)
			{
				return;
			}
			switch (mapTreasureData.TreasureType)
			{
			case MapTreasureType.Character:
			{
				image = gameObject.transform.GetChild(index2).GetComponent<Image>();
				num2 = mapTreasureData.CharacterId.id;
				texture2D = manager.GetCharacterTexture2D(num2);
				image.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
				gameObject.SetActive(value: true);
				for (int l = 0; l < gameObject.transform.childCount; l++)
				{
					gameObject.transform.GetChild(l).gameObject.SetActive(value: true);
				}
				gameObject.transform.GetChild(3).gameObject.SetActive(value: false);
				break;
			}
			case MapTreasureType.MusicNew:
			{
				image = gameObject.transform.GetChild(index2).gameObject.GetComponent<Image>();
				num2 = mapTreasureData.MusicId.id;
				texture2D = manager.GetJacketTexture2D(num2);
				image.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
				gameObject.SetActive(value: true);
				for (int m = 0; m < gameObject.transform.childCount; m++)
				{
					gameObject.transform.GetChild(m).gameObject.SetActive(value: true);
				}
				gameObject.transform.GetChild(2).gameObject.SetActive(value: false);
				break;
			}
			case MapTreasureType.MapTaskMusic:
			{
				image = gameObject.transform.GetChild(index2).gameObject.GetComponent<Image>();
				num2 = mapTreasureData.MusicId.id;
				texture2D = manager.GetJacketTexture2D(num2);
				image.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
				gameObject.SetActive(value: true);
				for (int num3 = 0; num3 < gameObject.transform.childCount; num3++)
				{
					gameObject.transform.GetChild(num3).gameObject.SetActive(value: true);
				}
				gameObject.transform.GetChild(2).gameObject.SetActive(value: false);
				break;
			}
			case MapTreasureType.Challenge:
			{
				image = gameObject.transform.GetChild(index2).gameObject.GetComponent<Image>();
				challengeData = Singleton<DataManager>.Instance.GetChallengeData(mapTreasureData.Challenge.id);
				if (challengeData != null)
				{
					num2 = challengeData.Music.id;
					texture2D = manager.GetJacketTexture2D(num2);
					image.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
				}
				gameObject.SetActive(value: true);
				for (int n = 0; n < gameObject.transform.childCount; n++)
				{
					gameObject.transform.GetChild(n).gameObject.SetActive(value: true);
				}
				gameObject.transform.GetChild(2).gameObject.SetActive(value: false);
				break;
			}
			case MapTreasureType.NamePlate:
			case MapTreasureType.Frame:
			{
				gameObject.SetActive(value: true);
				for (int k = 0; k < gameObject.transform.childCount; k++)
				{
					gameObject.transform.GetChild(k).gameObject.SetActive(value: true);
				}
				gameObject.transform.GetChild(3).gameObject.SetActive(value: false);
				break;
			}
			case MapTreasureType.LevelUpAll:
			case MapTreasureType.Otomodachi:
				break;
			}
		}

		public bool IsPrizeCharacter(List<MapTreasureExData> list, int current, bool otomodachi)
		{
			bool result = false;
			int index = _rainbowPinIcon[current];
			if (otomodachi)
			{
				index = current;
			}
			int id = list[index].TreasureId.id;
			MapTreasureData mapTreasureData = Singleton<DataManager>.Instance.GetMapTreasureData(id);
			if (mapTreasureData != null)
			{
				MapTreasureType treasureType = mapTreasureData.TreasureType;
				if (treasureType == MapTreasureType.Character)
				{
					result = true;
				}
			}
			return result;
		}

		public int GetConvertedTreasureID(MapData mapData, List<MapTreasureExData> list, int current, bool otomodachi)
		{
			bool flag = false;
			int num = _rainbowPinIcon[current];
			if (otomodachi)
			{
				num = current;
				if (_nextOtomodachiInsertID != -1 && num == _nextOtomodachiInsertID && _nextOtomodachiOriginalMapTresureID != -1)
				{
					num = _nextOtomodachiOriginalMapTresureID;
					flag = true;
				}
			}
			int num2 = 0;
			if (flag)
			{
				return mapData.TreasureExDatas[num].TreasureId.id;
			}
			return list[num].TreasureId.id;
		}

		public uint GetArrangedDistance(uint d, int i)
		{
			uint num = d;
			if (i == 0 && num != 0)
			{
				num = 0u;
			}
			if (_nextOtomodachiInsertID != -1 && i == _nextOtomodachiInsertID)
			{
				num = _nextOtomodachiDistance;
			}
			return num;
		}

		public uint GetPrizeDistance(List<MapTreasureExData> list, int i)
		{
			uint result = 0u;
			if (i < list.Count)
			{
				result = (uint)list[i].Distance;
				result = GetArrangedDistance(result, i);
			}
			return result;
		}

		public bool IsGetPrize(List<MapTreasureExData> list, int i, uint current)
		{
			bool result = false;
			uint num = 0u;
			if (i < list.Count)
			{
				num = GetPrizeDistance(list, i);
				if (current >= num)
				{
					result = true;
				}
			}
			return result;
		}

		public int GetPrizeWindowType(MapData mapData, List<MapTreasureExData> list, int current, bool otomodachi)
		{
			int num = 0;
			num = GetConvertedTreasureID(mapData, list, current, otomodachi);
			MapTreasureData mapTreasureData = Singleton<DataManager>.Instance.GetMapTreasureData(num);
			int result = -1;
			if (mapTreasureData != null)
			{
				switch (mapTreasureData.TreasureType)
				{
				case MapTreasureType.Character:
					result = 1;
					break;
				case MapTreasureType.MusicNew:
					result = 5;
					break;
				case MapTreasureType.MapTaskMusic:
					result = 6;
					break;
				case MapTreasureType.Challenge:
					result = 7;
					break;
				case MapTreasureType.NamePlate:
				case MapTreasureType.Frame:
					result = 3;
					break;
				}
			}
			if (mapTreasureData == null)
			{
				result = -1;
			}
			return result;
		}

		public int SetPrize(AssetManager manager, MapData mapData, List<MapTreasureExData> list, GameObject prize, int current, bool otomodachi, int otomodachi_music_id)
		{
			int num = 0;
			num = GetConvertedTreasureID(mapData, list, current, otomodachi);
			MapTreasureData mapTreasureData = Singleton<DataManager>.Instance.GetMapTreasureData(num);
			int num2 = 0;
			int index = 0;
			int index2 = 0;
			if (mapTreasureData != null)
			{
				switch (mapTreasureData.TreasureType)
				{
				case MapTreasureType.Character:
					index = 3;
					index2 = 5;
					break;
				case MapTreasureType.MusicNew:
					index = 4;
					index2 = 3;
					break;
				case MapTreasureType.MapTaskMusic:
					index = 4;
					index2 = 3;
					break;
				case MapTreasureType.Challenge:
					index = 7;
					index2 = 6;
					break;
				case MapTreasureType.Otomodachi:
					index = 3;
					index2 = 4;
					break;
				case MapTreasureType.NamePlate:
				case MapTreasureType.Frame:
					index = 3;
					break;
				}
			}
			num2 = GetPrizeWindowType(mapData, list, current, otomodachi);
			for (int i = 0; i < prize.transform.childCount; i++)
			{
				prize.transform.GetChild(i).gameObject.SetActive(value: false);
			}
			if (num2 == -1)
			{
				return num2;
			}
			prize.SetActive(value: true);
			GameObject gameObject = prize.gameObject;
			Image image = null;
			Texture2D texture2D = null;
			GameObject gameObject2 = null;
			Sprite sprite = null;
			ChallengeData challengeData = null;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			string text = null;
			Color color = new Color(1f, 1f, 1f, 1f);
			Color color2 = new Color(1f, 1f, 1f, 1f);
			if (mapTreasureData != null)
			{
				switch (mapTreasureData.TreasureType)
				{
				case MapTreasureType.Character:
				{
					gameObject2 = gameObject.transform.GetChild(index).gameObject;
					MultiImage component = gameObject2.transform.GetChild(1).GetComponent<MultiImage>();
					num3 = mapTreasureData.CharacterId.id;
					texture2D = manager.GetCharacterTexture2D(num3);
					sprite = (component.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero));
					MultiImage component2 = gameObject2.transform.GetChild(0).GetComponent<MultiImage>();
					component2.Image2 = sprite;
					num4 = Singleton<DataManager>.Instance.GetChara(mapTreasureData.CharacterId.id).color.id;
					Color24 colorDark = Singleton<DataManager>.Instance.GetMapColorData(num4).ColorDark;
					color2 = (component2.color = new Color((float)(int)colorDark.R / 255f, (float)(int)colorDark.G / 255f, (float)(int)colorDark.B / 255f));
					gameObject2.transform.GetChild(0).gameObject.SetActive(value: false);
					gameObject2.transform.GetChild(0).gameObject.SetActive(value: true);
					gameObject2 = gameObject.transform.GetChild(index2).gameObject;
					color = gameObject2.GetComponent<Image>().color;
					color2 = new Color(color.r, color.g, color.b, 255f);
					gameObject2.GetComponent<Image>().color = color2;
					gameObject2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Singleton<DataManager>.Instance.GetChara(mapTreasureData.CharacterId.id).name.str;
					text = Singleton<DataManager>.Instance.GetChara(mapTreasureData.CharacterId.id).genre.id.ToString("000000");
					gameObject2 = gameObject.transform.GetChild(1).gameObject;
					image = gameObject2.GetComponent<Image>();
					sprite = Resources.Load<Sprite>("Process/MapResult/Sprites/Window/BG/UI_CMN_Window_BG_" + text);
					if (sprite != null)
					{
						image.sprite = Object.Instantiate(sprite);
						break;
					}
					sprite = Resources.Load<Sprite>("Process/MapResult/Sprites/Window/BG/UI_CMN_Window_BG_000003");
					if (sprite != null)
					{
						image.sprite = Object.Instantiate(sprite);
					}
					break;
				}
				case MapTreasureType.MusicNew:
					text = mapData.GetID().ToString("000000");
					gameObject2 = gameObject.transform.GetChild(1).gameObject;
					image = gameObject2.GetComponent<Image>();
					sprite = Resources.Load<Sprite>("Process/MapResult/Sprites/Window/BG/UI_CMN_Window_BG_" + text);
					if (sprite != null)
					{
						image.sprite = Object.Instantiate(sprite);
					}
					else
					{
						sprite = Resources.Load<Sprite>("Process/MapResult/Sprites/Window/BG/UI_CMN_Window_BG_000003");
						if (sprite != null)
						{
							image.sprite = Object.Instantiate(sprite);
						}
					}
					gameObject2 = gameObject.transform.GetChild(index).gameObject;
					image = gameObject2.transform.GetChild(1).GetComponent<Image>();
					num3 = mapTreasureData.MusicId.id;
					texture2D = manager.GetJacketTexture2D(num3);
					image.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
					gameObject2 = gameObject.transform.GetChild(index2).gameObject;
					color = gameObject2.GetComponent<Image>().color;
					color2 = new Color(color.r, color.g, color.b, 255f);
					gameObject2.GetComponent<Image>().color = color2;
					gameObject2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Singleton<DataManager>.Instance.GetMusic(mapTreasureData.MusicId.id).name.str;
					break;
				case MapTreasureType.MapTaskMusic:
					gameObject2 = gameObject.transform.GetChild(index).gameObject;
					image = gameObject2.transform.GetChild(1).GetComponent<Image>();
					num3 = mapTreasureData.MusicId.id;
					texture2D = manager.GetJacketTexture2D(num3);
					image.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
					gameObject2 = gameObject.transform.GetChild(index2).gameObject;
					color = gameObject2.GetComponent<Image>().color;
					color2 = new Color(color.r, color.g, color.b, 255f);
					gameObject2.GetComponent<Image>().color = color2;
					gameObject2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Singleton<DataManager>.Instance.GetMusic(mapTreasureData.MusicId.id).name.str;
					break;
				case MapTreasureType.Challenge:
					gameObject2 = gameObject.transform.GetChild(index).gameObject;
					image = gameObject2.transform.GetChild(1).GetComponent<Image>();
					challengeData = Singleton<DataManager>.Instance.GetChallengeData(mapTreasureData.Challenge.id);
					if (challengeData != null)
					{
						num3 = challengeData.Music.id;
						texture2D = manager.GetJacketTexture2D(num3);
						image.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
					}
					gameObject2 = gameObject.transform.GetChild(index2).gameObject;
					color = gameObject2.GetComponent<Image>().color;
					color2 = new Color(color.r, color.g, color.b, 255f);
					gameObject2.GetComponent<Image>().color = color2;
					if (challengeData != null)
					{
						gameObject2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Singleton<DataManager>.Instance.GetMusic(challengeData.Music.id).name.str;
					}
					break;
				case MapTreasureType.NamePlate:
				{
					GameObject gameObject4 = gameObject;
					gameObject = gameObject4.transform.GetChild(3).gameObject;
					gameObject2 = gameObject.transform.GetChild(0).gameObject;
					SetActiveChildren(gameObject2, active: false);
					gameObject = gameObject2.transform.GetChild(1).gameObject;
					gameObject.SetActive(value: true);
					gameObject2 = gameObject.transform.GetChild(1).gameObject;
					image = gameObject2.GetComponent<Image>();
					num3 = mapTreasureData.NamePlate.id;
					texture2D = manager.GetPlateTexture2D(num3);
					image.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
					gameObject = gameObject4.transform.GetChild(4).gameObject;
					gameObject2 = gameObject.transform.GetChild(0).gameObject;
					gameObject2.GetComponent<TextMeshProUGUI>().text = Singleton<DataManager>.Instance.GetPlate(mapTreasureData.NamePlate.id).name.str;
					break;
				}
				case MapTreasureType.Frame:
				{
					GameObject gameObject3 = gameObject;
					gameObject = gameObject3.transform.GetChild(3).gameObject;
					gameObject2 = gameObject.transform.GetChild(0).gameObject;
					SetActiveChildren(gameObject2, active: false);
					gameObject = gameObject2.transform.GetChild(0).gameObject;
					gameObject.SetActive(value: true);
					gameObject2 = gameObject.transform.GetChild(1).gameObject;
					image = gameObject2.GetComponent<Image>();
					num3 = mapTreasureData.Frame.id;
					texture2D = manager.GetFrameTexture2D(num3);
					image.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
					gameObject = gameObject3.transform.GetChild(4).gameObject;
					gameObject2 = gameObject.transform.GetChild(0).gameObject;
					gameObject2.GetComponent<TextMeshProUGUI>().text = Singleton<DataManager>.Instance.GetFrame(mapTreasureData.Frame.id).name.str;
					break;
				}
				}
			}
			return num2;
		}

		public int SetMapClear(int mapID, GameObject prize)
		{
			int num = 0;
			int num2 = 0;
			num = 2;
			num2 = 4;
			for (int i = 0; i < prize.transform.childCount; i++)
			{
				prize.transform.GetChild(i).gameObject.SetActive(value: false);
			}
			prize.SetActive(value: true);
			GameObject gameObject = prize.gameObject.transform.GetChild(num2).gameObject;
			gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = AssetManager.Instance().GetMapBgSprite(mapID, "UI_IslandComp");
			gameObject.transform.GetChild(1).gameObject.SetActive(value: false);
			gameObject.transform.GetChild(0).gameObject.SetActive(value: false);
			gameObject.transform.GetChild(0).gameObject.SetActive(value: true);
			return num;
		}

		public int SetMapRelease(int mapID, GameObject prize, int select_id)
		{
			int num = 0;
			num = 4;
			for (int i = 0; i < prize.transform.childCount; i++)
			{
				prize.transform.GetChild(i).gameObject.SetActive(value: false);
			}
			prize.gameObject.SetActive(value: true);
			GameObject gameObject = prize.gameObject;
			GameObject gameObject2 = gameObject.transform.GetChild(3).gameObject;
			foreach (Transform item in gameObject2.transform)
			{
				Object.Destroy(item.gameObject);
			}
			_prefabNewIsland = AssetManager.Instance().GetIslandPrefab(mapID);
			_objIsland = Object.Instantiate(_prefabNewIsland);
			_objIsland.transform.SetParent(gameObject2.transform);
			_objIsland.transform.localPosition = Vector3.zero;
			MapData mapData = Singleton<DataManager>.Instance.GetMapData(mapID);
			if (mapData.IsInfinity)
			{
				_objIsland.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
			}
			GameObject gameObject3 = gameObject.transform.GetChild(5).transform.gameObject;
			Color color = gameObject3.GetComponent<Image>().color;
			Color color2 = new Color(color.r, color.g, color.b, 1f);
			gameObject3.GetComponent<Image>().color = color2;
			string text = (gameObject3.transform.GetChild(0).transform.gameObject.GetComponent<TextMeshProUGUI>().text = mapData.name.str);
			return num;
		}
	}
}
