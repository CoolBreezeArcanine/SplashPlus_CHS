using System;
using System.Collections.Generic;
using System.Linq;
using DB;
using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_000001;
using Manager;
using Manager.MaiStudio;
using Manager.MaiStudio.CardTypeName;
using Manager.UserDatas;
using Monitor.MapResult;
using Monitor.MapResult.Common;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Monitor
{
	public class MapResultMonitor : MonitorBase
	{
		[Serializable]
		public class Phase2
		{
			[SerializeField]
			public Animator _earth02;

			[SerializeField]
			public Animator _travelGauge01;

			[SerializeField]
			public Image _imgRainbow01;

			[SerializeField]
			public Animator _lockRainbow01;

			[SerializeField]
			public GameObject _objRainbowPin01;

			[SerializeField]
			public GameObject _objRainbowReached01;

			[SerializeField]
			public Animator _travelGauge02;

			[SerializeField]
			public Image _imgRainbow02;

			[SerializeField]
			public Animator _lockRainbow02;

			[SerializeField]
			public GameObject _objRainbowPin02;

			[SerializeField]
			public GameObject _objRainbowReached02;

			[SerializeField]
			public Animator _ground;

			[SerializeField]
			public Animator _restPinMeter;

			[SerializeField]
			public Animator _challengeInfo01;

			[SerializeField]
			private SpriteCounter _lifeNum;

			[SerializeField]
			private GameObject _challengeLevel;

			[SerializeField]
			private TextMeshProUGUI _challengeText01;

			[SerializeField]
			private TextMeshProUGUI _challengeText02;

			[SerializeField]
			private TextMeshProUGUI _stageFailedText;

			[SerializeField]
			private TextMeshProUGUI _taskText01;

			[SerializeField]
			private TextMeshProUGUI _taskText02;

			[SerializeField]
			private TextMeshProUGUI _mapCounterText;

			[SerializeField]
			public Animator _challengeInfo02;

			[SerializeField]
			public GameObject _objTotalMapRunMeter;

			[SerializeField]
			public GameObject _objWalk;

			[SerializeField]
			public GameObject _nullBonusWindowBG;

			[SerializeField]
			public Animator _bonusWindowBG;

			[SerializeField]
			public Animator _totalBonusMeter;

			[SerializeField]
			public GameObject _objTotalBonusMeter;

			public Phase2CommonMember m = new Phase2CommonMember();

			public void Initialize(AssetManager manager, List<MapTreasureExData> list, uint distance, uint total)
			{
				GameObject gameObject = null;
				GameObject gameObject2 = null;
				int num = 0;
				int num2 = 0;
				gameObject = _objWalk;
				gameObject2 = Resources.Load<GameObject>("Common/Prefabs/Derakkuma/UI_CMN_Derakkuma");
				m._Derakkuma2 = UnityEngine.Object.Instantiate(gameObject2);
				m.SetEmpty2Prefab(gameObject, gameObject2, m._Derakkuma2);
				gameObject = m._Derakkuma2.transform.parent.gameObject;
				gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 1f);
				num = 1;
				num2 = 3;
				gameObject = m._Derakkuma2.transform.GetChild(num).gameObject.transform.GetChild(num2).gameObject;
				gameObject2 = Resources.Load<GameObject>("Process/MapResult/Prefabs/Eff_DXFire");
				m._DerakkumaFire = UnityEngine.Object.Instantiate(gameObject2);
				m.SetEmptyPrefab(gameObject, gameObject2, m._DerakkumaFire);
				gameObject = m._DerakkumaFire.transform.parent.gameObject;
				gameObject.transform.localPosition = new Vector3(0f, 300f, 0f);
				gameObject = m._Derakkuma2.transform.parent.gameObject;
				gameObject = gameObject.transform.parent.gameObject;
				gameObject2 = Resources.Load<GameObject>("Process/MapResult/Prefabs/Eff_Derakkuma_Run_01");
				m._DerakkumaSmoke01 = UnityEngine.Object.Instantiate(gameObject2);
				m.SetEmptyPrefab(gameObject, gameObject2, m._DerakkumaSmoke01);
				gameObject2 = Resources.Load<GameObject>("Process/MapResult/Prefabs/Eff_Derakkuma_Run_02");
				m._DerakkumaSmoke02 = UnityEngine.Object.Instantiate(gameObject2);
				m.SetEmptyPrefab(gameObject, gameObject2, m._DerakkumaSmoke02);
				gameObject2 = Resources.Load<GameObject>("Process/MapResult/Prefabs/Eff_Derakkuma_Run_03");
				m._DerakkumaSmoke03 = UnityEngine.Object.Instantiate(gameObject2);
				m.SetEmptyPrefab(gameObject, gameObject2, m._DerakkumaSmoke03);
				m._DerakkumaSmoke01.SetActive(value: false);
				m._DerakkumaSmoke02.SetActive(value: false);
				m._DerakkumaSmoke03.SetActive(value: false);
				m._DerakkumaSmoke = null;
				m._isEndPinClear = false;
				m._isReleasePinOver = false;
				m._isGetNewCharacter = false;
				m._isInfinityLimitOver = false;
				m._isInfinityLastPinOver = false;
				_challengeText01.text = CommonMessageID.MapResultInfoChallenge01.GetName();
				_challengeText02.text = CommonMessageID.MapResultInfoChallenge02.GetName();
				_stageFailedText.text = CommonMessageID.MapResultInfoStageFailed.GetName();
				_taskText01.text = CommonMessageID.MapResultInfoTask01.GetName();
				_taskText02.text = CommonMessageID.MapResultInfoTask02.GetName();
				_mapCounterText.text = CommonMessageID.RegionalSelectTotalDistancce.GetName() + " :";
			}

			public void SetLifeInfo(int challenge_id)
			{
				uint num = 100u;
				uint index = 3u;
				int num2 = 3;
				bool flag = false;
				if (challenge_id != -1)
				{
					ChallengeDetail challengeDetail = ChallengeManager.GetChallengeDetail(challenge_id);
					if (challengeDetail.isEnable)
					{
						num = (uint)challengeDetail.startLife;
						index = (uint)challengeDetail.unlockDifficulty;
						num2 = challengeDetail.nextRelaxDay;
						flag = challengeDetail.infoEnable;
					}
				}
				string text = num.ToString("D000");
				if (num > 999)
				{
					text = "999";
				}
				_lifeNum.ChangeText(text);
				if (num < 100)
				{
					_lifeNum.FrameList[2].Scale = 0f;
					_lifeNum.FrameList[0].RelativePosition.x = 20f;
					_lifeNum.FrameList[1].RelativePosition.x = 12f;
					if (num < 10)
					{
						_lifeNum.FrameList[1].Scale = 0f;
						_lifeNum.FrameList[0].RelativePosition.x = 33f;
					}
				}
				MultipleImage component = _challengeLevel.GetComponent<MultipleImage>();
				if (component != null)
				{
					component.ChangeSprite((int)index);
				}
				if (_challengeText02 != null)
				{
					if (flag)
					{
						_challengeText02.text = CommonMessageID.MapResultInfoChallenge02.GetName().Replace(CommonMessageID.MapResultInfoChallenge02Replace.GetName(), num2.ToString());
					}
					else
					{
						_challengeText02.text = "";
					}
				}
			}
		}

		[Serializable]
		public class Phase3
		{
			[SerializeField]
			public Animator _party;

			[SerializeField]
			public GameObject _levelUpInfo;

			public Phase3CommonMember m = new Phase3CommonMember();

			public void Initialize(AssetManager manager, GameObject obj, List<UserChara> info, Table card_type, float ticket_rate)
			{
				m.Initialize(manager, obj, info, card_type, ticket_rate);
			}
		}

		private enum MonitorState
		{
			None,
			Phase2FadeIn,
			Phase2DerakkumaFadeIn,
			Phase2DerakkumaFadeInWait,
			Phase2RainbowReachedChallengePreJudge,
			Phase2RainbowReachedChallengeClearUnlockRainbow,
			Phase2RainbowReachedChallengeClearFadeIn,
			Phase2RainbowReachedChallengeClearFadeInWait,
			Phase2RainbowReachedChallengeClear,
			Phase2RainbowReachedChallengeClearWait,
			Phase2RainbowReachedChallengeClearFadeOut,
			Phase2RainbowReachedChallengeClearFadeOutWait,
			Phase2RainbowReachedChallengeFailedFadeIn,
			Phase2RainbowReachedChallengeFailedFadeInWait,
			Phase2RainbowReachedChallengeFailedFadeOut,
			Phase2RainbowReachedChallengeFailedFadeOutWait,
			Phase2GetBonusFadeIn,
			Phase2GetBonusFadeInWait,
			Phase2BonusMeterFadeIn,
			Phase2BonusMeterFadeInWait,
			Phase2GetBonusIconFadeInPreInit,
			Phase2GetBonusIconFadeIn,
			Phase2GetBonusIconFadeInWait,
			Phase2GetBonusIconFadeOut,
			Phase2GetBonusIconFadeOutWait,
			Phase2MoveBonusMiniIcon,
			Phase2MoveBonusMiniIconWait,
			Phase2GetBonusIconJudge,
			Phase2GetBonusFadeOut,
			Phase2GetBonusFadeOutWait,
			Phase2MeterFadeIn,
			Phase2MeterFadeInWait,
			Phase2AddRainbow,
			Phase2AddRainbowWait,
			Phase2RainbowReachedOtomodachiFadeIn,
			Phase2RainbowReachedOtomodachiFadeInWait,
			Phase2RainbowReachedOtomodachiFadeOut,
			Phase2RainbowReachedOtomodachiFadeOutWait,
			Phase2RainbowReached,
			Phase2RainbowReachedWait,
			Phase2RainbowReachedChallengePostJudge,
			Phase2RainbowReachedChallengeFadeIn,
			Phase2RainbowReachedChallengeFadeInWait,
			Phase2RainbowReachedChallengeInfo01FadeIn,
			Phase2RainbowReachedChallengeInfo01FadeInWait,
			Phase2RainbowReachedChallengeInfo01FadeOut,
			Phase2RainbowReachedChallengeInfo01FadeOutWait,
			Phase2RainbowReachedChallengeInfo02FadeIn,
			Phase2RainbowReachedChallengeInfo02FadeInWait,
			Phase2RainbowReachedChallengeInfo02FadeOut,
			Phase2RainbowReachedChallengeInfo02FadeOutWait,
			Phase2RainbowReachedGetPrizeFadeIn,
			Phase2RainbowReachedGetPrizeFadeInWait,
			Phase2RainbowReachedGetPrizeFadeOut,
			Phase2RainbowReachedGetPrizeFadeOutWait,
			Phase2NextRainbow,
			Phase2NextRainbowWait,
			Phase2NextRainbowJudge,
			Phase2MapClearFadeIn,
			Phase2MapClearFadeInWait,
			Phase2MapClearFadeOut,
			Phase2MapClearFadeOutWait,
			Phase2MapClearCollectionFadeIn,
			Phase2MapClearCollectionFadeInWait,
			Phase2MapClearCollectionGet,
			Phase2MapClearCollectionGetWait,
			Phase2MapClearCollectionFadeOut,
			Phase2MapClearCollectionFadeOutWait,
			Phase2MapReleaseFadeIn,
			Phase2MapReleaseFadeInWait,
			Phase2MapReleaseFadeOut,
			Phase2MapReleaseFadeOutWait,
			Phase2MapReleaseJudge,
			Phase2RainbowFadeOut,
			Phase2RainbowFadeOutWait,
			Phase2TotalMapRunFadeIn,
			Phase2TotalMapRunFadeInWait,
			Phase2TotalMapRunFadeInWaitJudge,
			Phase2TotalMapRunFadeInWaitJudgeWait,
			Phase2TotalMapRunWait,
			Phase2FadeOut,
			Phase2FadeOutWait,
			Phase3FadeIn,
			Phase3FadeInWait,
			Phase3LevelUpInfoFadeIn,
			Phase3LevelUpInfoFadeInWait,
			Phase3LevelUpInfoFadeOut,
			Phase3LevelUpInfoFadeOutWait,
			Phase3AddAwake,
			Phase3AddAwakeWait,
			Phase3AwakeReached,
			Phase3AwakeReachedWait,
			Phase3NextAwake,
			Phase3NextAwakeWait,
			Phase3NextAwakeJudge,
			Phase3AwakeInfo,
			Phase3AwakeInfoFadeIn,
			Phase3AwakeInfoFadeInWait,
			Phase3AwakeInfoFadeOut,
			Phase3AwakeInfoFadeOutWait,
			Phase3AwakeInfoJudge,
			Phase3PreFadeOutJudge,
			Phase3PreFadeOutJudgeWait,
			Phase3PreFadeOut,
			Phase3FadeOut,
			Phase3FadeOutWait,
			Finish,
			End
		}

		public enum InfoWindowState
		{
			None,
			Open,
			OpenWait,
			Wait,
			Close,
			CloseWait,
			End
		}

		[SerializeField]
		private MapResultButtonController _buttonController;

		[SerializeField]
		private Phase2 _phase2;

		[SerializeField]
		private Phase3 _phase3;

		private MonitorState _state = MonitorState.End;

		private int _timer;

		private int _sub_count;

		private bool _isBlockingSE1;

		private bool _isBlockingSE2;

		private bool _isBlockingSE3;

		private SoundManager.PlayerID _loopSE1;

		private SoundManager.PlayerID _loopSE2;

		private bool _isSetLoopSE1;

		private bool _isSetLoopSE2;

		public AssetManager _assetManager;

		public bool _isEntry;

		public bool _isGuest;

		public bool _isNeedAwake;

		private List<UserMapData> _dataListUserMap = new List<UserMapData>();

		private List<StringID> _dataListBonusMusicID = new List<StringID>();

		private List<MapTreasureExData> _listPinInfo = new List<MapTreasureExData>();

		private int _mapID;

		private int _musicID;

		private UserMapData _userMap;

		private MapData _mapData;

		private List<UserChara> _charaInfo = new List<UserChara>();

		private List<uint> _charaBaseV = new List<uint>();

		private UserData _userData;

		private int _monitorID;

		private List<int> _otomodachiMusicID = new List<int>();

		public bool _isCheckSkip;

		public bool _isSkip;

		private uint _count;

		private bool _isEnableSkip;

		private int _DispGetWindowTime = 300;

		private int _DispAwakeTime = 270;

		private int _DispMapClearTime = 600;

		private int _DispTotalMapRunTime = 30;

		private uint _current;

		private uint _total;

		private bool _isCardPass;

		private Table _cardPassType;

		private bool _isEnableRainbow;

		public bool _isDispInfoWindow1;

		public bool _isDispInfoWindow2;

		public bool _isCallVoice;

		public bool _isInfoWindowVoice;

		public InfoWindowState _info_state;

		public uint _info_timer;

		private bool _isTicketBonus;

		private int _ticketID;

		private float _ticketLevelUpRate = 1f;

		public void SkipAnim(InputManager.ButtonSetting button)
		{
			if (_isCheckSkip && !_isSkip)
			{
				_buttonController.SetAnimationActive(0);
				_isSkip = true;
			}
		}

		public void SetAssetManager(AssetManager manager)
		{
			_assetManager = manager;
		}

		public GameObject GetBlurObject()
		{
			return _phase2._earth02.gameObject.transform.parent.gameObject.gameObject.transform.parent.gameObject.gameObject.transform.parent.gameObject.transform.GetChild(2).gameObject;
		}

		public void SetBlur()
		{
			GameObject blurObject = GetBlurObject();
			blurObject.SetActive(value: true);
			CanvasGroup component = blurObject.GetComponent<CanvasGroup>();
			if (component != null)
			{
				component.alpha = 1f;
			}
		}

		public void ResetBlur()
		{
			GameObject blurObject = GetBlurObject();
			blurObject.SetActive(value: false);
			CanvasGroup component = blurObject.GetComponent<CanvasGroup>();
			if (component != null)
			{
				component.alpha = 0f;
			}
		}

		public void SetLastBlur()
		{
			GameObject blurObject = GetBlurObject();
			int siblingIndex = blurObject.transform.GetSiblingIndex();
			blurObject.transform.SetSiblingIndex(siblingIndex + 2);
			blurObject.SetActive(value: true);
			CanvasGroup component = blurObject.GetComponent<CanvasGroup>();
			if (component != null)
			{
				component.alpha = 1f;
			}
		}

		public override void Initialize(int monIndex, bool active)
		{
			_monitorID = monIndex;
			base.Initialize(_monitorID, active);
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(_monitorID);
			int num = 0;
			num = userData.MapList.Count;
			for (int i = 0; i < num; i++)
			{
				UserMapData userMapData = new UserMapData();
				userMapData = userData.MapList[i];
				_dataListUserMap.Add(userMapData);
			}
			_isEntry = userData.IsEntry;
			_isGuest = userData.IsGuest();
			_mapID = userData.Detail.SelectMapID;
			_musicID = 0;
			if (Singleton<GamePlayManager>.Instance.GetDebugGameScore(_monitorID) == null)
			{
				_musicID = GameManager.SelectMusicID[_monitorID];
			}
			else
			{
				_musicID = Singleton<GamePlayManager>.Instance.GetDebugGameScore(_monitorID).GameScoreData[GameManager.MusicTrackNumber - 1].Score.id;
			}
			if (_dataListUserMap.Count == 0 || !_isEntry || _isGuest)
			{
				GameObject obj = _phase2._earth02.gameObject.transform.parent.gameObject.gameObject.transform.parent.gameObject.gameObject.transform.parent.gameObject;
				obj.transform.GetChild(0).gameObject.SetActive(value: false);
				obj.transform.GetChild(1).gameObject.SetActive(value: false);
				_state = MonitorState.End;
				return;
			}
			int num2 = 0;
			foreach (KeyValuePair<int, MapData> mapData3 in Singleton<DataManager>.Instance.GetMapDatas())
			{
				int key = mapData3.Key;
				_ = mapData3.Value;
				num2 += key;
			}
			num2 *= 2;
			int num3 = userData.MapList.FindIndex((UserMapData m) => m.ID == _mapID);
			if (_dataListUserMap.Count == 0 || num3 < 0)
			{
				KeyValuePair<int, MapData> keyValuePair = Singleton<DataManager>.Instance.GetMapDatas().First();
				_mapID = keyValuePair.Key;
				userData.Detail.SelectMapID = keyValuePair.Key;
				MapData mapData = Singleton<DataManager>.Instance.GetMapData(_mapID);
				List<int> list = new List<int>();
				for (int j = 0; j < mapData.ReleaseConditionIds.list.Count; j++)
				{
					list.Add(mapData.ReleaseConditionIds.list[j].id);
				}
				UserMapData userMapData2 = new UserMapData();
				userMapData2.ID = _mapID;
				userMapData2.CharaColorKey = mapData.ColorId.id;
				Color24 color = Singleton<DataManager>.Instance.GetMapColorData(mapData.ColorId.id).Color;
				float r = (float)(int)color.R / 255f;
				float g = (float)(int)color.G / 255f;
				float b = (float)(int)color.B / 255f;
				userMapData2.MapColor = new Color(r, g, b, 1f);
				userMapData2.ReleaseIds = list;
				userMapData2.Name = mapData.name.str;
				_dataListUserMap.Add(userMapData2);
				num3 = 0;
			}
			_userMap = _dataListUserMap[num3];
			_mapData = Singleton<DataManager>.Instance.GetMapData(_mapID);
			_isEnableRainbow = true;
			if (_mapData.IsInfinity)
			{
				_isEnableRainbow = false;
			}
			_userData = Singleton<UserDataManager>.Instance.GetUserData(_monitorID);
			int num4 = _userData.MapList.FindIndex((UserMapData m) => m.ID == _mapID);
			if (num4 < 0)
			{
				num4 = 0;
			}
			_current = _userData.MapList[num4].Distance;
			_phase2.m._nextOtomodachiDistance = 0u;
			_phase2.m._nextOtomodachiInsertID = -1;
			_phase2.m._nextOtomodachiOriginalMapTresureID = -1;
			_phase2.m._nextOtomodachiNothing = false;
			_phase2.m._monitorID = _monitorID;
			List<int> list2 = new List<int>();
			List<uint> list3 = new List<uint>();
			uint num5 = 0u;
			foreach (var item in _mapData.TreasureExDatas.Select((MapTreasureExData value, int index) => new { value, index }))
			{
				if (item.value.SubParam1 > 0 || item.value.SubParam2 > 0)
				{
					list2.Add(item.index);
				}
			}
			if (list2.Count > 0)
			{
				foreach (var item2 in list2.Select((int value, int index) => new { value, index }))
				{
					int value2 = item2.value;
					uint distance = (uint)_mapData.TreasureExDatas[value2].Distance;
					uint num6 = (uint)_mapData.TreasureExDatas[value2].SubParam1;
					uint num7 = (uint)_mapData.TreasureExDatas[value2].SubParam2;
					uint num8 = 0u;
					if ((int)num6 <= 0)
					{
						num6 = 0u;
					}
					if ((int)num7 <= 0)
					{
						num7 = 0u;
					}
					uint num9 = 0u;
					uint num10 = 0u;
					num9 = num6;
					num10 = 0u;
					if (num9 != 0 && _current >= distance)
					{
						uint num11 = _current - distance;
						num10 = num11 / num9;
						num10 = ((num11 % num9 == 0) ? (num10 + 1) : (num10 + 1));
					}
					num8 = num10 * num9;
					if (num8 != 0)
					{
						int num12 = -1;
						foreach (var item3 in _mapData.TreasureExDatas.Select((MapTreasureExData value, int index) => new { value, index }))
						{
							if (item3.value.Distance == num8 + distance)
							{
								num12 = item3.index;
								break;
							}
						}
						while (num12 >= 0)
						{
							num10++;
							num8 = num10 * num9;
							if (num8 > _phase2.m._limitDistanceMax || num8 + distance > _phase2.m._limitDistanceMax)
							{
								num12 = -1;
								num8 = 0u;
								break;
							}
							num12 = -1;
							foreach (var item4 in _mapData.TreasureExDatas.Select((MapTreasureExData value, int index) => new { value, index }))
							{
								if (item4.value.Distance == num8 + distance)
								{
									num12 = item4.index;
									break;
								}
							}
						}
					}
					if (num8 != 0)
					{
						num8 += distance;
						if (num8 >= _phase2.m._limitDistanceMax)
						{
							num8 = 0u;
						}
						if (num7 != 0 && num8 > num7)
						{
							num8 = 0u;
						}
					}
					int count = _userData.MapList.Count;
					count--;
					if (count >= 0 && !_mapData.IsInfinity)
					{
						uint distance2 = _userData.MapList[count].Distance;
						if (num8 >= distance2)
						{
							num8 = 0u;
						}
					}
					if (_current < (uint)_mapData.TreasureExDatas[value2].Distance)
					{
						num8 = 0u;
					}
					list3.Add(num8);
				}
			}
			int num13 = -1;
			if (list2.Count > 0)
			{
				list2.Reverse();
				list3.Reverse();
				uint num14 = _phase2.m._limitDistanceMax;
				foreach (var item5 in list2.Select((int value, int index) => new { value, index }))
				{
					_ = item5.value;
					int index2 = item5.index;
					uint num15 = list3[index2];
					if (num15 != 0 && num15 < num14 && num15 > _current)
					{
						num14 = num15;
						num13 = index2;
					}
				}
				if (num14 != 0 && num14 <= _phase2.m._limitDistanceMax)
				{
					num5 = num14;
				}
				else
				{
					num13 = -1;
				}
			}
			if (num5 != 0)
			{
				_phase2.m._nextOtomodachiDistance = num5;
				if (num13 != -1)
				{
					_phase2.m._nextOtomodachiOriginalMapTresureID = list2[num13];
				}
				else
				{
					_phase2.m._nextOtomodachiDistance = 0u;
				}
			}
			if (num5 == 0)
			{
				_phase2.m._nextOtomodachiDistance = 0u;
			}
			num = _mapData.TreasureExDatas.Count;
			if (num > 0 && _mapData.TreasureExDatas[0].Distance != 0)
			{
				MapTreasureExData mapTreasureExData = new MapTreasureExData();
				mapTreasureExData = _mapData.TreasureExDatas[0];
				_listPinInfo.Add(mapTreasureExData);
			}
			if (_phase2.m._nextOtomodachiDistance != 0)
			{
				for (int k = 0; k < num; k++)
				{
					if (_mapData.TreasureExDatas[k].Distance < _phase2.m._nextOtomodachiDistance)
					{
						MapTreasureExData mapTreasureExData2 = new MapTreasureExData();
						mapTreasureExData2 = _mapData.TreasureExDatas[k];
						_listPinInfo.Add(mapTreasureExData2);
					}
					else if (_phase2.m._nextOtomodachiInsertID == -1)
					{
						if ((uint)_mapData.TreasureExDatas[k].Distance < _phase2.m._nextOtomodachiDistance)
						{
							MapTreasureExData mapTreasureExData3 = new MapTreasureExData();
							mapTreasureExData3 = _mapData.TreasureExDatas[k];
							_listPinInfo.Add(mapTreasureExData3);
							continue;
						}
						MapTreasureExData mapTreasureExData4 = new MapTreasureExData();
						mapTreasureExData4 = _mapData.TreasureExDatas[0];
						_listPinInfo.Add(mapTreasureExData4);
						_phase2.m._nextOtomodachiInsertID = _listPinInfo.Count - 1;
						MapTreasureExData mapTreasureExData5 = new MapTreasureExData();
						mapTreasureExData5 = _mapData.TreasureExDatas[k];
						_listPinInfo.Add(mapTreasureExData5);
					}
					else
					{
						MapTreasureExData mapTreasureExData6 = new MapTreasureExData();
						mapTreasureExData6 = _mapData.TreasureExDatas[k];
						_listPinInfo.Add(mapTreasureExData6);
					}
				}
				if (_phase2.m._nextOtomodachiInsertID == -1)
				{
					MapTreasureExData mapTreasureExData7 = new MapTreasureExData();
					mapTreasureExData7 = _mapData.TreasureExDatas[0];
					_listPinInfo.Add(mapTreasureExData7);
					_phase2.m._nextOtomodachiInsertID = _listPinInfo.Count - 1;
				}
			}
			else
			{
				for (int l = 0; l < num; l++)
				{
					MapTreasureExData mapTreasureExData8 = new MapTreasureExData();
					mapTreasureExData8 = _mapData.TreasureExDatas[l];
					_listPinInfo.Add(mapTreasureExData8);
				}
				if (_mapData.IsInfinity && _phase2.m._nextOtomodachiInsertID == -1)
				{
					MapTreasureExData mapTreasureExData9 = new MapTreasureExData();
					mapTreasureExData9 = _mapData.TreasureExDatas[0];
					_listPinInfo.Add(mapTreasureExData9);
					_phase2.m._nextOtomodachiInsertID = _listPinInfo.Count - 1;
					_phase2.m._nextOtomodachiNothing = true;
					_phase2.m._nextOtomodachiDistance = _phase2.m._limitDistanceMax;
				}
			}
			MapBonusMusicData mapBonusMusicData = Singleton<DataManager>.Instance.GetMapBonusMusicData(_mapData.BonusMusicId.id);
			num = mapBonusMusicData.MusicIds.list.Count;
			for (int n = 0; n < num; n++)
			{
				StringID stringID = new StringID();
				stringID = mapBonusMusicData.MusicIds.list[n];
				_dataListBonusMusicID.Add(stringID);
			}
			int num16 = -1;
			switch (_monitorID)
			{
			case 0:
				num16 = 1;
				break;
			case 1:
				num16 = 0;
				break;
			}
			if (num16 >= 0)
			{
				UserData userData2 = Singleton<UserDataManager>.Instance.GetUserData(num16);
				if (userData2.IsEntry && !userData2.IsGuest())
				{
					int selectMapID = userData2.Detail.SelectMapID;
					MapData mapData2 = Singleton<DataManager>.Instance.GetMapData(selectMapID);
					MapBonusMusicData mapBonusMusicData2 = Singleton<DataManager>.Instance.GetMapBonusMusicData(mapData2.BonusMusicId.id);
					num = mapBonusMusicData2.MusicIds.list.Count;
					for (int num17 = 0; num17 < num; num17++)
					{
						StringID stringID2 = new StringID();
						stringID2 = mapBonusMusicData2.MusicIds.list[num17];
						_dataListBonusMusicID.Add(stringID2);
					}
				}
			}
			int num18 = _dataListBonusMusicID.FindIndex((StringID m) => m.id == _musicID);
			_state = MonitorState.None;
			int[] charaSlot = userData.Detail.CharaSlot;
			int num19 = 0;
			for (int num20 = 0; num20 < charaSlot.Length; num20++)
			{
				if (charaSlot[num20] > 0)
				{
					CharaData data = Singleton<DataManager>.Instance.GetChara(charaSlot[num20]);
					UserChara userChara = userData.CharaList.Find((UserChara a2) => a2.ID == data.GetID());
					bool matchColor = userData.Detail.IsMatchColor(_monitorID, userChara.ID);
					bool leader = false;
					if (num19 == 0)
					{
						leader = true;
					}
					uint movementParam = userChara.GetMovementParam(matchColor, leader);
					movementParam *= 1000;
					UserChara userChara2 = new UserChara();
					userChara2 = userChara;
					_charaInfo.Add(userChara2);
					_charaBaseV.Add(movementParam);
					num19++;
				}
			}
			_cardPassType = Table.OutOfEffect;
			UserDetail detail = userData.Detail;
			_cardPassType = (Table)detail.CardType;
			if (Singleton<MapMaster>.Instance.TicketID != null)
			{
				_ticketID = Singleton<MapMaster>.Instance.TicketID[_monitorID];
			}
			if (_ticketID > 0)
			{
				bool flag = false;
				TicketData ticketData = null;
				int ticketID = _ticketID;
				if ((uint)(ticketID - -1) <= 1u)
				{
					flag = true;
				}
				if (!flag)
				{
					ticketData = Singleton<DataManager>.Instance.GetTicket(_ticketID);
					if (ticketData != null)
					{
						switch (_phase2.m.ConvertTicketKind(ticketData))
						{
						case TicketKind.Invalid:
						case TicketKind.None:
							_ticketLevelUpRate = 1f;
							break;
						case TicketKind.Paid:
						case TicketKind.Event:
						case TicketKind.Free:
							if ((float)ticketData.charaMagnification < 1f)
							{
								_ticketLevelUpRate = 1f;
							}
							else
							{
								_ticketLevelUpRate = (float)ticketData.charaMagnification / 100f;
							}
							break;
						}
					}
				}
			}
			_phase3.Initialize(_assetManager, _phase3._party.gameObject, _charaInfo, _cardPassType, _ticketLevelUpRate);
			uint num21 = 0u;
			foreach (var item6 in _charaBaseV.Select((uint value, int index) => new { value, index }))
			{
				num21 += item6.value;
			}
			if (_charaInfo.Count > 0)
			{
				BounusIconDispInfo bounusIconDispInfo = new BounusIconDispInfo();
				bounusIconDispInfo.ID = _charaInfo[0].ID;
				bounusIconDispInfo.miniIconType = BounusIconDispInfo.MiniIconType.MiniIconChara;
				bounusIconDispInfo.bonusKind = BounusIconDispInfo.BonusKind.BonusKindChara;
				bounusIconDispInfo.point = num21;
				bounusIconDispInfo.addPoint = num21;
				_phase2.m._dispInfo.Add(bounusIconDispInfo);
			}
			else
			{
				BounusIconDispInfo bounusIconDispInfo2 = new BounusIconDispInfo();
				bounusIconDispInfo2.ID = _charaInfo[0].ID;
				bounusIconDispInfo2.miniIconType = BounusIconDispInfo.MiniIconType.MiniIconChara;
				bounusIconDispInfo2.bonusKind = BounusIconDispInfo.BonusKind.BonusKindChara;
				bounusIconDispInfo2.point = num21;
				bounusIconDispInfo2.addPoint = num21;
				_phase2.m._dispInfo.Add(bounusIconDispInfo2);
			}
			if (Singleton<GamePlayManager>.Instance.GetDebugGameScore(monitorIndex) == null)
			{
				Singleton<GamePlayManager>.Instance.GetAchivement(monitorIndex);
			}
			else
			{
				_ = Singleton<GamePlayManager>.Instance.GetDebugGameScore(monitorIndex).GameScoreData[GameManager.MusicTrackNumber - 1].Score.achivement;
			}
			BounusIconDispInfo.RankKind iD = BounusIconDispInfo.RankKind.RankKindNone;
			int num22 = 0;
			bool flag2 = false;
			if (!flag2)
			{
				switch (Singleton<GamePlayManager>.Instance.GetGameScore(_monitorID).SyncType)
				{
				case PlaySyncflagID.ChainLow:
					iD = BounusIconDispInfo.RankKind.RankKindFullSync;
					num22 = 4000;
					flag2 = true;
					break;
				case PlaySyncflagID.ChainHi:
					iD = BounusIconDispInfo.RankKind.RankKindFullSyncPlus;
					num22 = 4000;
					flag2 = true;
					break;
				case PlaySyncflagID.SyncLow:
					iD = BounusIconDispInfo.RankKind.RankKindFullSyncDX;
					num22 = 4000;
					flag2 = true;
					break;
				case PlaySyncflagID.SyncHi:
					iD = BounusIconDispInfo.RankKind.RankKindFullSyncDXPlus;
					num22 = 4000;
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				switch (Singleton<GamePlayManager>.Instance.GetGameScore(_monitorID).ComboType)
				{
				case PlayComboflagID.Silver:
					iD = BounusIconDispInfo.RankKind.RankKindFullCombo;
					num22 = 2000;
					flag2 = true;
					break;
				case PlayComboflagID.Gold:
					iD = BounusIconDispInfo.RankKind.RankKindFullComboPlus;
					num22 = 2000;
					flag2 = true;
					break;
				case PlayComboflagID.AllPerfect:
					iD = BounusIconDispInfo.RankKind.RankKindAllPerfect;
					num22 = 3000;
					flag2 = true;
					break;
				case PlayComboflagID.AllPerfectPlus:
					iD = BounusIconDispInfo.RankKind.RankKindAllPerfectPlus;
					num22 = 3000;
					flag2 = true;
					break;
				}
			}
			if (!flag2 && Singleton<GamePlayManager>.Instance.GetPlayerIgnoreNpcNum() > 1)
			{
				iD = BounusIconDispInfo.RankKind.RankKindSyncPlay;
				num22 = 1000;
				flag2 = true;
			}
			if (num22 > 0)
			{
				BounusIconDispInfo bounusIconDispInfo3 = new BounusIconDispInfo();
				bounusIconDispInfo3.ID = (int)iD;
				bounusIconDispInfo3.miniIconType = BounusIconDispInfo.MiniIconType.MiniIconChara;
				bounusIconDispInfo3.bonusKind = BounusIconDispInfo.BonusKind.BonusKindCleaRank;
				bounusIconDispInfo3.point = (uint)num22;
				bounusIconDispInfo3.addPoint = num21 + (uint)num22;
				_phase2.m._dispInfo.Add(bounusIconDispInfo3);
			}
			else
			{
				num22 = 0;
				BounusIconDispInfo bounusIconDispInfo4 = new BounusIconDispInfo();
				bounusIconDispInfo4.ID = (int)iD;
				bounusIconDispInfo4.miniIconType = BounusIconDispInfo.MiniIconType.MiniIconChara;
				bounusIconDispInfo4.bonusKind = BounusIconDispInfo.BonusKind.BonusKindCleaRank;
				bounusIconDispInfo4.point = (uint)num22;
				bounusIconDispInfo4.addPoint = num21 + (uint)num22;
				_phase2.m._dispInfo.Add(bounusIconDispInfo4);
			}
			int num23 = 0;
			if (num18 >= 0)
			{
				num23 = _mapData.BonusMusicMagnification;
			}
			if (num23 > 0)
			{
				BounusIconDispInfo bounusIconDispInfo5 = new BounusIconDispInfo();
				bounusIconDispInfo5.ID = _musicID;
				bounusIconDispInfo5.miniIconType = BounusIconDispInfo.MiniIconType.MiniIconChara;
				bounusIconDispInfo5.bonusKind = BounusIconDispInfo.BonusKind.BonusKindPlayBonusMusic;
				bounusIconDispInfo5.point = (uint)num23;
				bounusIconDispInfo5.addPoint = (uint)((int)num21 + num22 + num23);
				_phase2.m._dispInfo.Add(bounusIconDispInfo5);
			}
			else
			{
				num23 = 0;
				BounusIconDispInfo bounusIconDispInfo6 = new BounusIconDispInfo();
				bounusIconDispInfo6.ID = _musicID;
				bounusIconDispInfo6.miniIconType = BounusIconDispInfo.MiniIconType.MiniIconChara;
				bounusIconDispInfo6.bonusKind = BounusIconDispInfo.BonusKind.BonusKindPlayBonusMusic;
				bounusIconDispInfo6.point = (uint)num23;
				bounusIconDispInfo6.addPoint = (uint)((int)num21 + num22 + num23);
				_phase2.m._dispInfo.Add(bounusIconDispInfo6);
			}
			_isCardPass = false;
			int num24 = 0;
			float num25 = 1f;
			_isTicketBonus = false;
			if (Singleton<MapMaster>.Instance.TicketID != null)
			{
				_ticketID = Singleton<MapMaster>.Instance.TicketID[_monitorID];
			}
			if (_ticketID > 0)
			{
				bool flag3 = false;
				TicketData ticketData2 = null;
				int ticketID = _ticketID;
				if ((uint)(ticketID - -1) <= 1u)
				{
					flag3 = true;
				}
				if (!flag3)
				{
					ticketData2 = Singleton<DataManager>.Instance.GetTicket(_ticketID);
					if (ticketData2 != null)
					{
						switch (_phase2.m.ConvertTicketKind(ticketData2))
						{
						case TicketKind.Free:
							num24 = ticketData2.areaPercent;
							num25 = (float)ticketData2.areaPercent / 100f;
							break;
						case TicketKind.Event:
							num24 = ticketData2.areaPercent;
							num25 = (float)ticketData2.areaPercent / 100f;
							break;
						case TicketKind.Paid:
							ticketID = ticketData2.areaPercent;
							if (ticketID != 200)
							{
								_ = 300;
							}
							num24 = ticketData2.areaPercent;
							num25 = (float)ticketData2.areaPercent / 100f;
							break;
						}
					}
				}
			}
			if (num24 > 0)
			{
				_isTicketBonus = true;
				_isCardPass = true;
			}
			num21 = 0u;
			foreach (var item7 in _charaBaseV.Select((uint value, int index) => new { value, index }))
			{
				num21 += item7.value;
			}
			if ((uint)num24 > 100u)
			{
				num25 = (float)num24 / 100f;
			}
			uint num26 = (uint)((float)(uint)((int)num21 + num22 + num23) * num25);
			uint num27 = 0u;
			num27 = num26 % 10u;
			if (num27 != 0)
			{
				num26 -= num27;
			}
			num27 = num26 % 1000u;
			if (num27 != 0)
			{
				num26 -= num27;
				num26 += 1000;
			}
			_total = _current + num26;
			if (num24 > 0)
			{
				BounusIconDispInfo bounusIconDispInfo7 = new BounusIconDispInfo();
				bounusIconDispInfo7.ID = _ticketID;
				bounusIconDispInfo7.miniIconType = BounusIconDispInfo.MiniIconType.MiniIconChara;
				bounusIconDispInfo7.bonusKind = BounusIconDispInfo.BonusKind.BonusKindTicketGrade;
				bounusIconDispInfo7.point = (uint)num24;
				bounusIconDispInfo7.addPoint = num26;
				_phase2.m._dispInfo.Add(bounusIconDispInfo7);
			}
			else
			{
				num24 = 0;
				BounusIconDispInfo bounusIconDispInfo8 = new BounusIconDispInfo();
				bounusIconDispInfo8.ID = _ticketID;
				bounusIconDispInfo8.miniIconType = BounusIconDispInfo.MiniIconType.MiniIconChara;
				bounusIconDispInfo8.bonusKind = BounusIconDispInfo.BonusKind.BonusKindTicketGrade;
				bounusIconDispInfo8.point = (uint)num24;
				bounusIconDispInfo8.addPoint = num26;
				_phase2.m._dispInfo.Add(bounusIconDispInfo8);
			}
			int siblingIndex = _buttonController.gameObject.transform.GetSiblingIndex();
			_buttonController.gameObject.transform.SetSiblingIndex(siblingIndex + 1);
			_buttonController.Initialize(_monitorID);
			_timer = 0;
			_sub_count = 0;
			_phase2.m.InitializeRainbow(_assetManager, _listPinInfo, _current, _total, _mapData.IsInfinity);
			_phase2.m._bonusIconDispCount = 0;
			_phase2.m._bonusIconMax = _phase2.m._dispInfo.Count;
			_phase2.m.SetBonusIcon(_assetManager, _phase2._bonusWindowBG.gameObject, _phase2.m._animListBonusIcon, _phase2.m._listListBonusIconPoint);
			_phase2.m.InitializeRestPinMeter(_phase2._restPinMeter.gameObject);
			_phase2.m.InitializeTotalMapRunMeter(_phase2._objTotalMapRunMeter);
			_phase2.m.InitializeTotalBonus(_phase2._objTotalBonusMeter);
			_phase2.m._RoadSpeedType = 0;
			if (_total - _current >= 2001)
			{
				_phase2.m._RoadSpeedType = 2;
			}
			else if (_total - _current >= 501)
			{
				_phase2.m._RoadSpeedType = 1;
			}
			else
			{
				_phase2.m._RoadSpeedType = 0;
			}
			float diff = 10f;
			float diff2 = 50f;
			switch (_phase2.m._RoadSpeedType)
			{
			case 0:
				diff2 = 2.5f;
				diff = 2f;
				break;
			case 1:
				diff2 = 10f;
				diff = 7f;
				break;
			case 2:
				diff2 = 25f;
				diff = 18f;
				break;
			}
			_phase2.m.InitializeRoad(_mapID, _phase2._ground.gameObject, diff, diff2);
			_phase2.m.InitializeDistanceInfo(_phase2._ground.gameObject, _mapData.IsInfinity);
			_phase2.m.InitializeNearPin(_phase2._objWalk);
			_phase2.m.InitializeFarPin(_phase2._objWalk);
			_phase2.Initialize(_assetManager, _listPinInfo, _current, _total);
			GameObject gameObject = _buttonController.gameObject.transform.parent.gameObject;
			GameObject gameObject2 = new GameObject("EmptyPrize");
			gameObject2.transform.SetParent(gameObject.transform);
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			gameObject2.transform.localScale = Vector3.one;
			gameObject2.transform.SetSiblingIndex(3);
			_phase2.m.InitializePrize(gameObject2, _phase2.m._animAwakeStarRoot, _phase2.m._animAwakeStar, _phase2.m._animAwakeStarEffect, _phase2.m._animAwakeStarSpark);
			_phase2.m.InitializeChallengeInfo(gameObject2);
			_phase2.m.SetOtomodachiMusic(_mapData, _listPinInfo, _total, _monitorID, _otomodachiMusicID);
			_isCheckSkip = false;
			_isSkip = false;
			_count = 0u;
			_isEnableSkip = false;
			_buttonController.SetVisible(false, default(int));
			_phase3.m.InitializeLevelUpInfo(_phase3._levelUpInfo);
			_phase3.m.InitializeAmakeCharaDistance(_phase2.m._objListPrize[0]);
			_isDispInfoWindow1 = false;
			_isDispInfoWindow2 = false;
			_info_state = InfoWindowState.None;
			_info_timer = 0u;
		}

		private void CheckChallengeStatus()
		{
			_phase2.m._isChallenge = false;
			_phase2.m._isCommonStart = true;
			_phase2.m._isChallengeDifficultyOK = false;
			_phase2.m._challengeInfoType = 0;
			int convertedTreasureID = _phase2.m.GetConvertedTreasureID(_mapData, _listPinInfo, _phase2.m._rainbowID, otomodachi: true);
			MapTreasureData mapTreasureData = Singleton<DataManager>.Instance.GetMapTreasureData(convertedTreasureID);
			if (mapTreasureData == null)
			{
				return;
			}
			int num = 0;
			bool flag = false;
			ChallengeData challengeData = null;
			switch (mapTreasureData.TreasureType)
			{
			case MapTreasureType.MapTaskMusic:
				_phase2.m._challengeInfoType = 0;
				flag = true;
				num = mapTreasureData.MusicId.id;
				break;
			case MapTreasureType.Challenge:
				_phase2.m._challengeInfoType = 1;
				flag = true;
				challengeData = Singleton<DataManager>.Instance.GetChallengeData(mapTreasureData.Challenge.id);
				if (challengeData != null)
				{
					num = challengeData.Music.id;
				}
				break;
			}
			if (!flag)
			{
				return;
			}
			_phase2.m._isChallenge = true;
			_phase2.m._isCommonStart = false;
			if (!_phase2.m._rainbowChallengeStart)
			{
				_phase2.m._isCommonStart = false;
				return;
			}
			_phase2.m._isChallengeClear = false;
			if (GameManager.SelectMusicID[_monitorID] == num)
			{
				switch (_phase2.m._challengeInfoType)
				{
				case 0:
					if (GameManager.GetClearRank((int)Singleton<GamePlayManager>.Instance.GetAchivement(monitorIndex)) >= MusicClearrankID.Rank_A)
					{
						_phase2.m._isChallengeClear = true;
					}
					break;
				case 1:
				{
					bool flag2 = false;
					bool flag3 = false;
					NotesWrapper notesWrapper = Singleton<NotesListManager>.Instance.GetNotesList()[num];
					for (int i = 0; i < notesWrapper.ChallengeDetail.Length; i++)
					{
						if (notesWrapper.ChallengeDetail[i].isEnable && notesWrapper.ChallengeDetail[i].startLife > 0)
						{
							flag2 = true;
							break;
						}
					}
					flag3 = GameManager.IsPerfectChallenge;
					if (flag2 && flag3 && GameManager.SelectDifficultyID[_monitorID] >= (int)notesWrapper.ChallengeDetail[_monitorID].unlockDifficulty)
					{
						_phase2.m._isChallengeDifficultyOK = true;
						if (Singleton<GamePlayManager>.Instance.GetGameScore(_monitorID).Life != 0)
						{
							_phase2.m._isChallengeClear = true;
						}
					}
					break;
				}
				}
			}
			else
			{
				_phase2.m._isCommonStart = false;
			}
			if (_phase2.m._isChallengeClear)
			{
				_phase2.m._isCommonStart = true;
			}
			else
			{
				_phase2.m._isCommonStart = false;
			}
		}

		private MapTreasureData GetChallengeTresureData()
		{
			int convertedTreasureID = _phase2.m.GetConvertedTreasureID(_mapData, _listPinInfo, _phase2.m._rainbowID, otomodachi: true);
			return Singleton<DataManager>.Instance.GetMapTreasureData(convertedTreasureID);
		}

		public void isSkipedTimerZero()
		{
			if (_isSkip)
			{
				_timer = 0;
			}
		}

		public void SkipStart()
		{
			if (!_isEnableSkip)
			{
				_isEnableSkip = true;
				_buttonController.SetVisible(true, default(int));
				_isCheckSkip = true;
			}
		}

		public void CheckSkipStart()
		{
			_count++;
			if (_count >= 60)
			{
				SkipStart();
			}
		}

		public void CheckRainbowSkipStart()
		{
			_count++;
			if (_count >= 60)
			{
				SkipStart();
			}
		}

		public void CheckGetWindowSkipStart()
		{
			_count++;
			if (_count >= _DispGetWindowTime - 120)
			{
				SkipStart();
			}
		}

		public void CheckAwakeSkipStart()
		{
			_count++;
			if (_count >= 30)
			{
				SkipStart();
			}
		}

		public void CheckMapClearSkipStart()
		{
			_count++;
			if (_count >= _DispMapClearTime - 120)
			{
				SkipStart();
			}
		}

		public void CheckTotalMapRunSkipStart()
		{
			_count++;
			if (_count >= _DispTotalMapRunTime - 120)
			{
				SkipStart();
			}
		}

		public void ResetSkipStart()
		{
			_count = 0u;
			if (_isEnableSkip)
			{
				_buttonController.SetVisible(false, default(int));
			}
			_isEnableSkip = false;
			_isCheckSkip = false;
			_isSkip = false;
		}

		public override void ViewUpdate()
		{
			if (_timer > 0)
			{
				_timer--;
			}
			switch (_state)
			{
			case MonitorState.None:
			{
				Sprite sprite = null;
				Image image = null;
				image = _phase2._earth02.gameObject.transform.GetChild(0).GetComponent<Image>();
				sprite = AssetManager.Instance().GetMapBgSprite(_mapID, "UI_CMN_Earth");
				if (sprite != null)
				{
					image.sprite = UnityEngine.Object.Instantiate(sprite);
				}
				_phase2._earth02.gameObject.SetActive(value: false);
				_phase2._travelGauge01.gameObject.SetActive(value: false);
				_phase2._lockRainbow01.gameObject.SetActive(value: false);
				_phase2._travelGauge02.gameObject.SetActive(value: false);
				_phase2._lockRainbow02.gameObject.SetActive(value: false);
				_phase2._ground.gameObject.SetActive(value: false);
				_phase2._restPinMeter.gameObject.SetActive(value: false);
				_phase2._challengeInfo01.gameObject.SetActive(value: false);
				_phase2._challengeInfo02.gameObject.SetActive(value: false);
				_phase2._nullBonusWindowBG.gameObject.SetActive(value: false);
				_phase2._bonusWindowBG.gameObject.SetActive(value: false);
				_phase2._totalBonusMeter.gameObject.SetActive(value: false);
				_phase2.m._Derakkuma2.SetActive(value: false);
				_phase3._party.gameObject.SetActive(value: false);
				_isNeedAwake = false;
				if (_isEntry)
				{
					_state = MonitorState.Phase2FadeIn;
					if (GameManager.IsFreedomMode && Singleton<MapMaster>.Instance.IsCallAwake != null && Singleton<MapMaster>.Instance.IsNeedAwake != null && !Singleton<MapMaster>.Instance.IsCallAwake[_monitorID] && Singleton<MapMaster>.Instance.IsNeedAwake[_monitorID])
					{
						_total = _current;
						_isNeedAwake = true;
					}
				}
				_isSetLoopSE1 = false;
				_isSetLoopSE2 = false;
				_count = 0u;
				break;
			}
			case MonitorState.Phase2FadeIn:
			{
				if (_timer != 0)
				{
					break;
				}
				_phase2._earth02.gameObject.SetActive(value: true);
				_phase2._earth02.Play("In", 0, 0f);
				if (_isNeedAwake)
				{
					_phase2._restPinMeter.gameObject.SetActive(value: true);
					_phase2._restPinMeter.gameObject.SetActive(value: true);
					_phase2._restPinMeter.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
					for (int num56 = 0; num56 < _phase2.m._restPinMeterChildMax; num56++)
					{
						_phase2._restPinMeter.gameObject.transform.GetChild(num56).gameObject.SetActive(value: true);
					}
					_phase2._ground.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).gameObject.SetActive(value: true);
					_ = _phase2.m._valueListRainbow[_phase2.m._rainbowID];
					int num57 = _phase2.m._distanceBase[_phase2.m._rainbowID];
					CommonValueUint commonValueUint3 = _phase2.m._valueListRainbowUint[_phase2.m._rainbowID];
					uint start5 = commonValueUint3.start;
					uint start6 = (uint)num57 - start5;
					_phase2.m._remainUntilPinCommonValue.start = start6;
					_ = commonValueUint3.end;
					CommonValueUint remainUntilPinCommonValue2 = _phase2.m._remainUntilPinCommonValue;
					if (_mapData.IsInfinity)
					{
						_phase2.m._remainUntilPinCommonValue.start = _phase2.m._distanceStart[_phase2.m._rainbowID] + start5;
					}
					_phase2.m._objListRestPinMeter[0].SetNumber(remainUntilPinCommonValue2.start);
					_phase2.m._objListRestPinMeter[1].SetNumber(remainUntilPinCommonValue2.start);
					_phase2.m._objListRestPinMeter[2].SetNumber(remainUntilPinCommonValue2.start);
					_phase2.m._objListRestPinMeter[3].SetNumber(remainUntilPinCommonValue2.start);
				}
				else
				{
					if (_isEnableRainbow)
					{
						_phase2._travelGauge01.gameObject.SetActive(value: true);
						_phase2._travelGauge01.Play("Fade_In", 0, 0f);
					}
					if (_phase2.m._rainbowChallengeStart)
					{
						_phase2._lockRainbow01.gameObject.SetActive(value: true);
						_phase2._lockRainbow01.Play("In", 0, 0f);
						_phase2._objRainbowReached01.SetActive(value: false);
					}
				}
				float start7 = _phase2.m._valueListRainbow[_phase2.m._rainbowID].start;
				_phase2._imgRainbow01.fillAmount = start7 / 100f;
				float z3 = 90f - 1.8f * start7;
				_phase2._objRainbowReached01.transform.localRotation = Quaternion.Euler(0f, 0f, z3);
				int num58 = _phase2.m._rainbowID + 1;
				bool num59 = num58 >= _phase2.m._rainbowEnd || num58 >= _listPinInfo.Count;
				bool flag29 = false;
				if (num59 && start7 >= 100f)
				{
					flag29 = true;
					if (!_mapData.IsInfinity && num58 >= _listPinInfo.Count)
					{
						_phase2.m._isEndPinClear = true;
					}
				}
				else
				{
					_phase2.m.SetRainbowPin(_assetManager, _listPinInfo, _phase2._objRainbowPin01, _phase2.m._rainbowID);
				}
				if (flag29)
				{
					_phase2._objRainbowPin01.SetActive(value: false);
				}
				_phase2._ground.gameObject.SetActive(value: true);
				_phase2._ground.Play("Ground_In", 0, 0f);
				_phase2.m._totalMapRunMeterValue = _current;
				_phase2.m._objListTotalMapRunMeter[0].SetNumber(_phase2.m._totalMapRunMeterValue);
				_state = MonitorState.Phase2DerakkumaFadeIn;
				_timer = 0;
				_isSetLoopSE1 = false;
				break;
			}
			case MonitorState.Phase2DerakkumaFadeIn:
			{
				if (_timer != 0)
				{
					break;
				}
				_phase2.m._Derakkuma2.SetActive(value: true);
				_phase2.m._DerakkumaPosX = new CommonValue();
				Vector3 localPosition = _phase2.m._Derakkuma2.transform.localPosition;
				_phase2.m._DerakkumaPosY = localPosition.y;
				_phase2.m._DerakkumaPosX.start = -2100f;
				_phase2.m._DerakkumaPosX.current = _phase2.m._DerakkumaPosX.start;
				_phase2.m._DerakkumaPosX.end = 0f;
				_phase2.m._DerakkumaPosX.diff = (_phase2.m._DerakkumaPosX.end - _phase2.m._DerakkumaPosX.start) / 60f;
				_phase2.m._Derakkuma2.transform.localPosition = new Vector3(_phase2.m._DerakkumaPosX.start, _phase2.m._DerakkumaPosY, 0f);
				if (!_isCardPass)
				{
					_phase2.m._DerakkumaFire.transform.parent.gameObject.SetActive(value: false);
				}
				Animator component8 = _phase2.m._Derakkuma2.GetComponent<Animator>();
				int layerIndex16 = component8.GetLayerIndex("Result");
				component8.SetLayerWeight(layerIndex16, 0f);
				layerIndex16 = component8.GetLayerIndex("Side");
				component8.SetLayerWeight(layerIndex16, 1f);
				component8.Play("S_Walk_R", layerIndex16, 0f);
				if (_isSetLoopSE1)
				{
					SoundManager.StopSE(_loopSE1);
					_isSetLoopSE1 = false;
				}
				_loopSE1 = SoundManager.PlayLoopSE(Mai2.Mai2Cue.Cue.SE_MAP_MOVE_WALK_01, _monitorID);
				_isSetLoopSE1 = true;
				if (!_isNeedAwake)
				{
					bool flag33 = true;
					CheckChallengeStatus();
					if (_phase2.m._isChallenge && _phase2.m._rainbowChallengeStart)
					{
						flag33 = false;
					}
					if (flag33)
					{
						SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000144, _monitorID);
					}
				}
				_state = MonitorState.Phase2DerakkumaFadeInWait;
				_timer = 60;
				float start11 = _phase2.m._valueListRainbow[_phase2.m._rainbowID].start;
				int num84 = _phase2.m._rainbowID + 1;
				bool num85 = num84 >= _phase2.m._rainbowEnd || num84 >= _listPinInfo.Count;
				bool flag34 = false;
				if (num85 && start11 >= 100f)
				{
					flag34 = true;
				}
				bool num86 = num85 && _mapData.IsInfinity;
				int num87 = _phase2.m._rainbowID;
				bool flag35 = false;
				if (_phase2.m._nextOtomodachiInsertID != -1 && num87 == _phase2.m._nextOtomodachiInsertID && num87 == _listPinInfo.Count - 1 && _phase2.m._valueListRainbow[num87].end >= 100f)
				{
					flag35 = true;
				}
				if (!num86)
				{
					_ = _phase2.m._valueListRainbow[num87];
				}
				else if (flag35)
				{
					_ = _phase2.m._valueListRainbow[num87];
				}
				else if (num87 >= _listPinInfo.Count - 1)
				{
					int num88 = _phase2.m._valueListRainbow.Count - 1;
					_ = _phase2.m._valueListRainbow[num88];
					num87 = num88;
				}
				else
				{
					_ = _phase2.m._valueListRainbow[num87];
				}
				uint num89 = 0u;
				uint prizeDistance5 = _phase2.m.GetPrizeDistance(_listPinInfo, _phase2.m._rainbowID);
				uint num90 = 0u;
				if (_phase2.m._rainbowID > 0)
				{
					num90 = _phase2.m.GetPrizeDistance(_listPinInfo, _phase2.m._rainbowID - 1);
				}
				uint num91 = prizeDistance5 - num90;
				if (num91 > 10000)
				{
					num91 /= 100u;
					num91 *= _phase2.m._paramDispPercent;
					num91 = num90 + num91;
				}
				else
				{
					float num92 = (float)num91 / 100f;
					num92 *= (float)_phase2.m._paramDispPercent;
					num91 = num90 + (uint)num92;
				}
				bool flag36 = num86 && _phase2.m.GetPrizeDistance(_listPinInfo, num87) == _total;
				bool flag37 = num86 && _phase2.m.GetPrizeDistance(_listPinInfo, num87) <= _total && num87 >= _listPinInfo.Count - 1;
				bool flag38 = !num86 || flag35 || flag36 || flag37 || _current >= num91;
				bool flag39 = _phase2.m._nextOtomodachiNothing;
				if (_phase2.m._nextOtomodachiNothing && num87 < _listPinInfo.Count - 1)
				{
					flag39 = false;
				}
				if (!flag34 && !_phase2.m._isUpdateNearPin && _phase2.m._rainbowID <= _listPinInfo.Count - 1 && _current >= num91 && flag38 && !flag39 && num91 != 0 && _current >= num91)
				{
					_phase2.m._isUpdateNearPin = true;
					_phase2.m._nearPinX.start = _phase2.m._paramNearPinStart;
					if (_phase2.m.IsGetPrize(_listPinInfo, num87, _current))
					{
						_phase2.m._nearPinX.end = _phase2.m._paramNearPinReached;
					}
					else
					{
						_phase2.m._nearPinX.end = _phase2.m._paramNearPinUnreached;
					}
					_phase2.m._nearPinX.diff = 0f;
					_phase2.m._nearPinX.diff = (_phase2.m._nearPinX.end - _phase2.m._nearPinX.start) / (100f - (float)_phase2.m._paramDispPercent);
					uint num93 = (prizeDistance5 - num90) / 100u;
					uint num94 = _current - num90;
					if (num93 == 0)
					{
						num89 = 0u;
					}
					else if (num91 > 10000)
					{
						num89 = num94 / num93;
						num89 -= _phase2.m._paramDispPercent;
					}
					else
					{
						num89 = (uint)((float)num94 / (float)num93 - (float)_phase2.m._paramDispPercent);
					}
					_phase2.m._nearPinX.current = _phase2.m._nearPinX.start;
					_phase2.m._nearPinX.current = _phase2.m._nearPinX.start;
					int otomodachi_music_id5 = -1;
					if (_otomodachiMusicID != null && _otomodachiMusicID.Count > 0 && num87 - _phase2.m._rainbowStart < _otomodachiMusicID.Count)
					{
						otomodachi_music_id5 = _otomodachiMusicID[num87 - _phase2.m._rainbowStart];
					}
					_phase2.m.SetNearPin(_assetManager, _mapData, _listPinInfo, num87, otomodachi_music_id5, reached: false);
				}
				if (_phase2.m._isUpdateNearPin)
				{
					for (int num95 = 0; num95 < num89; num95++)
					{
						_phase2.m._nearPinX.UpdateValue();
					}
					float current9 = _phase2.m._nearPinX.current;
					float num96 = 0f;
					num96 = _phase2.m._objNearPin.transform.localPosition.y;
					_phase2.m._objNearPin.transform.localPosition = new Vector3(current9, num96, 0f);
					num96 = _phase2.m._objNearPinGhost.transform.localPosition.y;
					_phase2.m._objNearPinGhost.transform.localPosition = new Vector3(current9, num96, 0f);
					_phase2.m._isSetPosNearPin = true;
				}
				_phase2.m._isUpdateNearPin = false;
				break;
			}
			case MonitorState.Phase2DerakkumaFadeInWait:
			{
				bool num12 = _phase2.m._DerakkumaPosX.UpdateValue();
				_phase2.m._Derakkuma2.transform.localPosition = new Vector3(_phase2.m._DerakkumaPosX.current, _phase2.m._DerakkumaPosY, 0f);
				if (_isNeedAwake)
				{
					_timer = 0;
				}
				if (num12 || _timer == 0)
				{
					_phase2.m._Derakkuma2.transform.localPosition = new Vector3(_phase2.m._DerakkumaPosX.end, _phase2.m._DerakkumaPosY, 0f);
					_state = MonitorState.Phase2RainbowReachedChallengePreJudge;
					Animator component = _phase2.m._Derakkuma2.GetComponent<Animator>();
					int layerIndex2 = component.GetLayerIndex("Result");
					component.SetLayerWeight(layerIndex2, 0f);
					layerIndex2 = component.GetLayerIndex("Side");
					component.SetLayerWeight(layerIndex2, 1f);
					component.Play("S_Default_R", layerIndex2, 0f);
					if (_isSetLoopSE1)
					{
						SoundManager.StopSE(_loopSE1);
						_isSetLoopSE1 = false;
					}
					_timer = 0;
				}
				if (_isNeedAwake)
				{
					_state = MonitorState.Phase2RainbowFadeOut;
				}
				break;
			}
			case MonitorState.Phase2RainbowReachedChallengePreJudge:
				CheckChallengeStatus();
				if (_phase2.m._isChallenge)
				{
					if (_phase2.m._isCommonStart)
					{
						_phase2._lockRainbow01.Play("Out_Unlock", 0, 0f);
						_phase2._objRainbowReached01.SetActive(value: true);
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_MAP_KEY_OPEN, _monitorID);
						_state = MonitorState.Phase2RainbowReachedChallengeClearUnlockRainbow;
						break;
					}
					if (_total >= _phase2.m._distanceEnd[_phase2.m._rainbowID])
					{
						_total = _phase2.m._distanceEnd[_phase2.m._rainbowID];
					}
					if (_phase2.m._rainbowChallengeStart)
					{
						_state = MonitorState.Phase2RainbowReachedChallengeFailedFadeIn;
					}
					else
					{
						_state = MonitorState.Phase2GetBonusFadeIn;
					}
				}
				else
				{
					_state = MonitorState.Phase2GetBonusFadeIn;
				}
				break;
			case MonitorState.Phase2RainbowReachedChallengeClearUnlockRainbow:
				if (_phase2.m.IsEndAnim(_phase2._lockRainbow01))
				{
					_phase2._lockRainbow01.gameObject.SetActive(value: false);
					_state = MonitorState.Phase2RainbowReachedChallengeClearFadeIn;
				}
				break;
			case MonitorState.Phase2RainbowReachedChallengeClearFadeIn:
				if (_timer == 0)
				{
					_phase2._restPinMeter.gameObject.SetActive(value: false);
					_phase2._totalBonusMeter.gameObject.SetActive(value: false);
					_phase2.m.SetChallengeLockedInfo(_assetManager, _mapData, _listPinInfo, _phase2._challengeInfo02.gameObject, _phase2.m._rainbowID);
					_phase2.m.SetActiveChildren(_phase2._challengeInfo02.gameObject, active: false);
					_phase2._challengeInfo02.gameObject.SetActive(value: true);
					_phase2._challengeInfo02.gameObject.transform.GetChild(0).gameObject.SetActive(value: true);
					int layerIndex15 = _phase2._challengeInfo02.GetLayerIndex("Base Layer");
					_phase2._challengeInfo02.SetLayerWeight(layerIndex15, 1f);
					switch (_phase2.m._challengeInfoType)
					{
					case 1:
						_phase2._challengeInfo02.gameObject.transform.GetChild(1).gameObject.SetActive(value: true);
						_phase2._challengeInfo02.Play("CompAnm_PerfectChallenge", 0, 0f);
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_MAP_SONG_CLEAR_02, _monitorID);
						SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000258, _monitorID);
						break;
					case 0:
						_phase2._challengeInfo02.gameObject.transform.GetChild(2).gameObject.SetActive(value: true);
						_phase2._challengeInfo02.Play("CompAnm_Task", 0, 0f);
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_MAP_SONG_CLEAR_01, _monitorID);
						SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000259, _monitorID);
						break;
					}
					layerIndex15 = _phase2._challengeInfo02.GetLayerIndex("Color Layer");
					_phase2._challengeInfo02.SetLayerWeight(layerIndex15, 1f);
					switch (_phase2.m._challengeInfoType)
					{
					case 1:
						_phase2._challengeInfo02.Play("Color_PerfectChallenge", layerIndex15, 0f);
						break;
					case 0:
						_phase2._challengeInfo02.Play("Color_Task", layerIndex15, 0f);
						break;
					}
					_state = MonitorState.Phase2RainbowReachedChallengeClearFadeInWait;
					_timer = 60;
				}
				break;
			case MonitorState.Phase2RainbowReachedChallengeClearFadeInWait:
				if (_phase2.m.IsEndAnim(_phase2._challengeInfo02) || _timer == 0)
				{
					_state = MonitorState.Phase2RainbowReachedChallengeClear;
					_timer = 60;
				}
				break;
			case MonitorState.Phase2RainbowReachedChallengeClear:
			{
				if (_timer != 0)
				{
					break;
				}
				_phase2._challengeInfo02.Play("Out", 0, 0f);
				int num9 = 0;
				int index2 = 6;
				switch (_phase2.m._challengeInfoType)
				{
				case 0:
					index2 = 6;
					break;
				case 1:
					index2 = 7;
					break;
				}
				num9 = _phase2.m.SetPrize(_assetManager, _mapData, _listPinInfo, _phase2.m._objListPrize[index2], _phase2.m._rainbowID, otomodachi: false, -1);
				_phase2.m._prizeWindowType = num9;
				if (num9 != -1)
				{
					_phase2.m._objListPrize[index2].SetActive(value: true);
					_phase2.m.SetActiveChildren(_phase2.m._objListPrize[index2], active: true);
					GameObject gameObject = _phase2.m._objListPrize[index2].gameObject;
					gameObject.SetActive(value: true);
					_phase2.m._prize = gameObject.GetComponent<Animator>();
					_phase2.m._prize.Play("In", 0, 0f);
					switch (_phase2.m._challengeInfoType)
					{
					case 0:
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.JINGLE_MAP_GET, _monitorID);
						break;
					case 1:
					{
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.JINGLE_MAP_GET, _monitorID);
						GameObject gameObject2 = _phase2.m._prize.gameObject;
						_phase2.m._lime = gameObject2.transform.GetChild(3).GetChild(0).gameObject.GetComponent<Animator>();
						_phase2.m._lime.Play("Lime_fun_Window_01", 0, 0f);
						_phase2.m._lemon = gameObject2.transform.GetChild(4).GetChild(0).gameObject.GetComponent<Animator>();
						_phase2.m._lemon.Play("Lemon_fun_Window_01", 0, 0f);
						_phase2.m._otohime = gameObject2.transform.GetChild(5).GetChild(0).gameObject.GetComponent<Animator>();
						_phase2.m._otohime.Play("fun_Window_01", 0, 0f);
						break;
					}
					}
				}
				_state = MonitorState.Phase2RainbowReachedChallengeClearWait;
				_timer = _DispGetWindowTime;
				ResetSkipStart();
				break;
			}
			case MonitorState.Phase2RainbowReachedChallengeClearWait:
			{
				isSkipedTimerZero();
				if (_timer == 0)
				{
					_state = MonitorState.Phase2RainbowReachedChallengeClearFadeOut;
				}
				int ticketID = _phase2.m._challengeInfoType;
				if (ticketID != 0 && ticketID == 1)
				{
					if (_phase2.m.IsEndAnim(_phase2.m._lime))
					{
						_phase2.m._lime.Play("Lime_fun_Window_01", 0, 0f);
					}
					if (_phase2.m.IsEndAnim(_phase2.m._lemon))
					{
						_phase2.m._lemon.Play("Lemon_fun_Window_01", 0, 0f);
					}
				}
				CheckGetWindowSkipStart();
				break;
			}
			case MonitorState.Phase2RainbowReachedChallengeClearFadeOut:
				if (_timer == 0)
				{
					if (_phase2.m._prize != null)
					{
						_phase2.m._prize.Play("Out", 0, 0f);
					}
					_state = MonitorState.Phase2RainbowReachedChallengeClearFadeOutWait;
				}
				break;
			case MonitorState.Phase2RainbowReachedChallengeClearFadeOutWait:
			{
				if (!_phase2.m.IsEndAnim(_phase2.m._prize) && _timer != 0)
				{
					break;
				}
				int prizeWindowType3 = _phase2.m._prizeWindowType;
				int num19 = 6;
				num19 = prizeWindowType3;
				if (prizeWindowType3 != -1)
				{
					_phase2.m._objListPrize[num19].gameObject.SetActive(value: false);
					_phase2.m.SetActiveChildren(_phase2.m._objListPrize[num19], active: false);
					_phase2.m._objListPrize[num19].SetActive(value: false);
				}
				num19 = _phase2.m._challengeInfoType;
				_phase2.m.SetActiveChildren(_phase2._challengeInfo01.gameObject.transform.GetChild(0).gameObject, active: false);
				_phase2._challengeInfo01.gameObject.SetActive(value: false);
				_phase2.m.SetActiveChildren(_phase2._challengeInfo02.gameObject, active: false);
				_phase2._challengeInfo02.gameObject.SetActive(value: false);
				int convertedTreasureID = _phase2.m.GetConvertedTreasureID(_mapData, _listPinInfo, _phase2.m._rainbowID, otomodachi: true);
				MapTreasureData mapTreasureData2 = Singleton<DataManager>.Instance.GetMapTreasureData(convertedTreasureID);
				if (mapTreasureData2 != null)
				{
					ChallengeData challengeData = null;
					int num20 = 0;
					switch (mapTreasureData2.TreasureType)
					{
					case MapTreasureType.MapTaskMusic:
						_phase2.m._challengeInfoType = 0;
						_userData.AddUnlockMusic(UserData.MusicUnlock.Base, mapTreasureData2.MusicId.id);
						num20 = GameManager.SelectDifficultyID[_monitorID];
						_userData.Activity.TaskMusicClear(mapTreasureData2.MusicId.id, num20);
						break;
					case MapTreasureType.Challenge:
						_phase2.m._challengeInfoType = 1;
						challengeData = Singleton<DataManager>.Instance.GetChallengeData(mapTreasureData2.Challenge.id);
						if (challengeData != null)
						{
							_userData.AddUnlockMusic(UserData.MusicUnlock.Base, challengeData.Music.id);
							num20 = GameManager.SelectDifficultyID[_monitorID];
							_userData.Activity.ChallengeMusicClear(challengeData.Music.id, num20);
						}
						break;
					}
				}
				_state = MonitorState.Phase2GetBonusFadeIn;
				_timer = 10;
				break;
			}
			case MonitorState.Phase2RainbowReachedChallengeFailedFadeIn:
				if (_timer == 0)
				{
					_phase2._restPinMeter.gameObject.SetActive(value: false);
					_phase2._totalBonusMeter.gameObject.SetActive(value: false);
					_phase2._challengeInfo01.gameObject.SetActive(value: true);
					_phase2.m.SetActiveChildren(_phase2._challengeInfo01.gameObject.transform.GetChild(0).gameObject, active: false);
					switch (_phase2.m._challengeInfoType)
					{
					case 1:
					{
						_phase2._challengeInfo01.gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(value: true);
						int challengeID2 = _phase2.m.GetChallengeID(_mapData, _listPinInfo, _phase2.m._rainbowID);
						_phase2.SetLifeInfo(challengeID2);
						break;
					}
					case 0:
						_phase2._challengeInfo01.gameObject.transform.GetChild(0).GetChild(1).gameObject.SetActive(value: true);
						break;
					}
					if (_phase2.m._isChallengeDifficultyOK)
					{
						_phase2._challengeInfo01.Play("StageFailed", 0, 0f);
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_MAP_SONG_FAILED, _monitorID);
					}
					else
					{
						_phase2._challengeInfo01.Play("In", 0, 0f);
					}
					SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000257, _monitorID);
					_phase2.m.SetChallengeLockedInfo(_assetManager, _mapData, _listPinInfo, _phase2._challengeInfo02.gameObject, _phase2.m._rainbowID);
					_phase2.m.SetActiveChildren(_phase2._challengeInfo02.gameObject, active: false);
					_phase2._challengeInfo02.gameObject.SetActive(value: true);
					_phase2._challengeInfo02.gameObject.transform.GetChild(0).gameObject.SetActive(value: true);
					int layerIndex7 = _phase2._challengeInfo02.GetLayerIndex("Base Layer");
					_phase2._challengeInfo02.SetLayerWeight(layerIndex7, 1f);
					_phase2._challengeInfo02.Play("In", 0, 0f);
					layerIndex7 = _phase2._challengeInfo02.GetLayerIndex("Color Layer");
					_phase2._challengeInfo02.SetLayerWeight(layerIndex7, 1f);
					switch (_phase2.m._challengeInfoType)
					{
					case 1:
						_phase2._challengeInfo02.Play("Color_PerfectChallenge", layerIndex7, 0f);
						break;
					case 0:
						_phase2._challengeInfo02.Play("Color_Task", layerIndex7, 0f);
						break;
					}
					_state = MonitorState.Phase2RainbowReachedChallengeFailedFadeInWait;
					_timer = 180;
					ResetSkipStart();
				}
				break;
			case MonitorState.Phase2RainbowReachedChallengeFailedFadeInWait:
			{
				isSkipedTimerZero();
				bool flag28 = false;
				if ((!_phase2.m._isChallengeDifficultyOK) ? (_timer == 0) : (_timer == 0))
				{
					_state = MonitorState.Phase2RainbowReachedChallengeFailedFadeOut;
					ResetSkipStart();
				}
				else
				{
					CheckSkipStart();
				}
				break;
			}
			case MonitorState.Phase2RainbowReachedChallengeFailedFadeOut:
				if (_timer == 0)
				{
					_phase2._challengeInfo01.Play("Out", 0, 0f);
					_phase2._challengeInfo02.Play("Out", 0, 0f);
					_state = MonitorState.Phase2RainbowReachedChallengeInfo02FadeOutWait;
					_timer = 30;
				}
				break;
			case MonitorState.Phase2RainbowReachedChallengeFailedFadeOutWait:
				if (_phase2.m.IsEndAnim(_phase2._challengeInfo01) || _timer == 0)
				{
					_phase2.m.SetActiveChildren(_phase2._challengeInfo01.gameObject.transform.GetChild(0).gameObject, active: false);
					_phase2._challengeInfo01.gameObject.SetActive(value: false);
					_phase2.m.SetActiveChildren(_phase2._challengeInfo02.gameObject, active: false);
					_phase2._challengeInfo02.gameObject.SetActive(value: false);
					_state = MonitorState.Phase2RainbowFadeOut;
					_timer = 30;
				}
				break;
			case MonitorState.Phase2GetBonusFadeIn:
				if (_timer == 0)
				{
					_phase2._bonusWindowBG.gameObject.SetActive(value: true);
					_phase2._bonusWindowBG.Play("In", 0, 0f);
					for (int num63 = 0; num63 < _phase2.m._animListBonusIcon.Count; num63++)
					{
						_phase2.m._animListBonusIcon[num63].gameObject.SetActive(value: false);
					}
					_phase2._totalBonusMeter.gameObject.transform.parent.gameObject.SetActive(value: true);
					_phase2._totalBonusMeter.gameObject.SetActive(value: true);
					_phase2._totalBonusMeter.Play("In", 0, 0f);
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_MAP_BONUS, _monitorID);
					for (int num64 = 0; num64 < _phase2.m._toatalBonusMeterChildMax; num64++)
					{
						_phase2._totalBonusMeter.gameObject.transform.GetChild(num64).gameObject.SetActive(value: true);
					}
					_phase2.m._objListTotalBonusMeter[0].SetNumber(_phase2.m._toatalBonusCommonValue.current);
					_phase2.m._objListTotalBonusMeter[1].SetNumber(_phase2.m._toatalBonusCommonValue.current);
					_state = MonitorState.Phase2GetBonusFadeInWait;
					_timer = 60;
				}
				break;
			case MonitorState.Phase2GetBonusFadeInWait:
				if (_phase2.m.IsEndAnim(_phase2._bonusWindowBG) || _timer == 0)
				{
					_state = MonitorState.Phase2BonusMeterFadeIn;
					_timer = 0;
				}
				break;
			case MonitorState.Phase2BonusMeterFadeIn:
				if (_timer == 0)
				{
					_state = MonitorState.Phase2BonusMeterFadeInWait;
					_timer = 0;
				}
				break;
			case MonitorState.Phase2BonusMeterFadeInWait:
				if (_timer == 0)
				{
					_state = MonitorState.Phase2GetBonusIconFadeInPreInit;
					_timer = 30;
				}
				break;
			case MonitorState.Phase2GetBonusIconFadeInPreInit:
			{
				if (_timer != 0)
				{
					break;
				}
				_phase2.m._animListBonusIcon[_phase2.m._bonusIconDispCount].gameObject.SetActive(value: true);
				bool flag11 = false;
				if (_phase2.m._dispInfo[_phase2.m._bonusIconDispCount].bonusKind == BounusIconDispInfo.BonusKind.BonusKindTicketGrade)
				{
					if (_phase2.m._dispInfo[_phase2.m._bonusIconDispCount].point > 100)
					{
						flag11 = true;
					}
				}
				else if (_phase2.m._dispInfo[_phase2.m._bonusIconDispCount].point != 0)
				{
					flag11 = true;
				}
				if (flag11)
				{
					_phase2.m._animListBonusIcon[_phase2.m._bonusIconDispCount].Play("State_Active", 0, 0f);
				}
				else
				{
					_phase2.m._animListBonusIcon[_phase2.m._bonusIconDispCount].Play("State_NoActive", 0, 0f);
				}
				_state = MonitorState.Phase2GetBonusIconFadeIn;
				_timer = 1;
				break;
			}
			case MonitorState.Phase2GetBonusIconFadeIn:
			{
				isSkipedTimerZero();
				if (_timer != 0)
				{
					break;
				}
				bool flag4 = false;
				if (_phase2.m._dispInfo[_phase2.m._bonusIconDispCount].bonusKind == BounusIconDispInfo.BonusKind.BonusKindTicketGrade)
				{
					if (_phase2.m._dispInfo[_phase2.m._bonusIconDispCount].point > 100)
					{
						flag4 = true;
					}
				}
				else if (_phase2.m._dispInfo[_phase2.m._bonusIconDispCount].point != 0)
				{
					flag4 = true;
				}
				if (flag4)
				{
					int num13 = (int)_phase2.m._dispInfo[_phase2.m._bonusIconDispCount].point;
					int index3 = 0;
					switch (_phase2.m._dispInfo[_phase2.m._bonusIconDispCount].bonusKind)
					{
					case BounusIconDispInfo.BonusKind.BonusKindChara:
						index3 = 0;
						num13 /= 1000;
						break;
					case BounusIconDispInfo.BonusKind.BonusKindCleaRank:
						index3 = 1;
						num13 /= 1000;
						break;
					case BounusIconDispInfo.BonusKind.BonusKindPlayBonusMusic:
						index3 = 2;
						num13 /= 1000;
						break;
					case BounusIconDispInfo.BonusKind.BonusKindTicketGrade:
						index3 = 3;
						break;
					}
					_phase2.m._listListBonusIconPoint[_phase2.m._bonusIconDispCount][index3].SetNumber(num13);
					_phase2.m._animListBonusIcon[_phase2.m._bonusIconDispCount].Play("In_Active", 0, 0f);
					_ = _phase2.m._dispInfo[_phase2.m._bonusIconDispCount].bonusKind;
					_ = 4;
					if (_phase2.m._dispInfo[_phase2.m._bonusIconDispCount].bonusKind == BounusIconDispInfo.BonusKind.BonusKindChara)
					{
						GameObject obj = _phase2.m._animListBonusIcon[_phase2.m._bonusIconDispCount].gameObject.transform.transform.GetChild(0).GetChild(0).GetChild(0)
							.GetChild(1)
							.gameObject;
						obj.SetActive(value: false);
						obj.SetActive(value: true);
					}
				}
				_phase2.m._toatalBonusMeterValue = _phase2.m._dispInfo[_phase2.m._bonusIconDispCount].addPoint;
				for (int l = 0; l < _phase2.m._toatalBonusMeterChildMax; l++)
				{
					_phase2.m._objListTotalBonusMeter[l].SetNumber(_phase2.m._toatalBonusMeterValue);
				}
				if (flag4)
				{
					_phase2._totalBonusMeter.Play("Action", 0, 0f);
					if (_phase2.m._dispInfo[_phase2.m._bonusIconDispCount].bonusKind == BounusIconDispInfo.BonusKind.BonusKindTicketGrade)
					{
						if (_ticketID > 0)
						{
							bool flag5 = false;
							bool flag6 = false;
							bool flag7 = false;
							TicketData ticketData = null;
							int ticketID = _ticketID;
							if ((uint)(ticketID - -1) <= 1u)
							{
								flag5 = true;
								flag7 = true;
							}
							if (!flag5)
							{
								ticketData = Singleton<DataManager>.Instance.GetTicket(_ticketID);
								if (ticketData != null)
								{
									switch (_phase2.m.ConvertTicketKind(ticketData))
									{
									case TicketKind.Invalid:
									case TicketKind.None:
										flag7 = true;
										break;
									case TicketKind.Free:
										SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_MAP_GET, _monitorID);
										flag6 = true;
										break;
									case TicketKind.Event:
										SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_MAP_GET_DX, _monitorID);
										flag6 = true;
										break;
									case TicketKind.Paid:
										SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_MAP_GET_DX, _monitorID);
										flag6 = true;
										ticketID = ticketData.areaPercent;
										if (ticketID != 200)
										{
											_ = 300;
										}
										break;
									}
								}
							}
							if (!flag7 && !flag6)
							{
								SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_MAP_GET, _monitorID);
							}
						}
					}
					else
					{
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_MAP_GET, _monitorID);
					}
				}
				_state = MonitorState.Phase2GetBonusIconFadeInWait;
				_timer = 0;
				break;
			}
			case MonitorState.Phase2GetBonusIconFadeInWait:
			{
				isSkipedTimerZero();
				bool flag3 = false;
				if (_phase2.m._dispInfo[_phase2.m._bonusIconDispCount].bonusKind == BounusIconDispInfo.BonusKind.BonusKindTicketGrade)
				{
					if (_timer == 0)
					{
						flag3 = true;
					}
				}
				else
				{
					flag3 = _timer == 0;
				}
				if (flag3)
				{
					_phase2._totalBonusMeter.Play("Action", 0, 1f);
					_state = MonitorState.Phase2GetBonusIconFadeOut;
					_timer = 0;
				}
				break;
			}
			case MonitorState.Phase2GetBonusIconFadeOut:
				isSkipedTimerZero();
				if (_timer == 0)
				{
					_state = MonitorState.Phase2GetBonusIconFadeOutWait;
					_timer = 0;
					SkipStart();
				}
				break;
			case MonitorState.Phase2GetBonusIconFadeOutWait:
				isSkipedTimerZero();
				if (_timer == 0)
				{
					_phase2.m._bonusIconDispCount++;
					if (_phase2.m._bonusIconDispCount == _phase2.m._bonusIconMax)
					{
						_state = MonitorState.Phase2GetBonusFadeOut;
						_timer = 60;
					}
					else
					{
						_state = MonitorState.Phase2MoveBonusMiniIcon;
					}
				}
				break;
			case MonitorState.Phase2MoveBonusMiniIcon:
				isSkipedTimerZero();
				if (_timer == 0)
				{
					_state = MonitorState.Phase2MoveBonusMiniIconWait;
					_timer = 10;
				}
				break;
			case MonitorState.Phase2MoveBonusMiniIconWait:
				if (_phase2.m._dispInfo[_phase2.m._bonusIconDispCount].bonusKind == BounusIconDispInfo.BonusKind.BonusKindTicketGrade)
				{
					ResetSkipStart();
				}
				else
				{
					isSkipedTimerZero();
				}
				if (_timer == 0)
				{
					_state = MonitorState.Phase2GetBonusIconJudge;
				}
				break;
			case MonitorState.Phase2GetBonusIconJudge:
				isSkipedTimerZero();
				if (_phase2.m._bonusIconDispCount >= _phase2.m._bonusIconMax)
				{
					_state = MonitorState.Phase2GetBonusFadeOut;
					break;
				}
				_state = MonitorState.Phase2GetBonusIconFadeInPreInit;
				if (_phase2.m._dispInfo[_phase2.m._bonusIconDispCount].bonusKind == BounusIconDispInfo.BonusKind.BonusKindTicketGrade && _phase2.m._dispInfo[_phase2.m._bonusIconDispCount].point > 100)
				{
					_timer = 30;
				}
				break;
			case MonitorState.Phase2GetBonusFadeOut:
				if (_timer == 0)
				{
					_phase2._bonusWindowBG.Play("Out", 0, 0f);
					_state = MonitorState.Phase2GetBonusFadeOutWait;
					_timer = 60;
				}
				break;
			case MonitorState.Phase2GetBonusFadeOutWait:
				if (_phase2.m.IsEndAnim(_phase2._bonusWindowBG) || _timer == 0)
				{
					_phase2._bonusWindowBG.gameObject.SetActive(value: false);
					_state = MonitorState.Phase2MeterFadeIn;
					_timer = 0;
					ResetSkipStart();
				}
				break;
			case MonitorState.Phase2MeterFadeIn:
			{
				if (_timer != 0)
				{
					break;
				}
				_phase2._totalBonusMeter.Play("Move", 0, 0f);
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_MAP_ZOOM, _monitorID);
				_phase2._restPinMeter.gameObject.SetActive(value: true);
				_phase2._restPinMeter.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
				for (int num17 = 0; num17 < _phase2.m._restPinMeterChildMax; num17++)
				{
					_phase2._restPinMeter.gameObject.transform.GetChild(num17).gameObject.SetActive(value: true);
				}
				_phase2._ground.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).gameObject.SetActive(value: true);
				_ = _phase2.m._valueListRainbow[_phase2.m._rainbowID];
				int num18 = _phase2.m._distanceBase[_phase2.m._rainbowID];
				CommonValueUint commonValueUint = _phase2.m._valueListRainbowUint[_phase2.m._rainbowID];
				uint start = commonValueUint.start;
				uint start2 = (uint)num18 - start;
				_phase2.m._remainUntilPinCommonValue.start = start2;
				uint end = commonValueUint.end;
				if ((uint)num18 > end)
				{
					_phase2.m._remainUntilPinCommonValue.end = (uint)num18 - end;
				}
				else
				{
					_phase2.m._remainUntilPinCommonValue.end = 0u;
				}
				_phase2.m._remainUntilPinCommonValue.current = _phase2.m._remainUntilPinCommonValue.start;
				if (!_mapData.IsInfinity)
				{
					_phase2.m._remainUntilPinCommonValue.diff = _phase2.m._valueListRainbowUint[_phase2.m._rainbowID].diff;
					_phase2.m._remainUntilPinCommonValue.sub = true;
				}
				else
				{
					_phase2.m._remainUntilPinCommonValue.diff = _phase2.m._valueListRainbowUint[_phase2.m._rainbowID].diff;
					_phase2.m._remainUntilPinCommonValue.sub = false;
				}
				if (_mapData.IsInfinity)
				{
					_phase2.m._remainUntilPinCommonValue.start = _phase2.m._distanceStart[_phase2.m._rainbowID] + start;
					_phase2.m._remainUntilPinCommonValue.current = _phase2.m._remainUntilPinCommonValue.start;
					_phase2.m._remainUntilPinCommonValue.end = _phase2.m._distanceStart[_phase2.m._rainbowID] + end;
					int i2 = _listPinInfo.Count - 1;
					uint prizeDistance = _phase2.m.GetPrizeDistance(_listPinInfo, i2);
					if (_current >= prizeDistance)
					{
						_phase2.m._remainUntilPinCommonValue.start = _current;
						_phase2.m._remainUntilPinCommonValue.current = _phase2.m._remainUntilPinCommonValue.start;
						_phase2.m._remainUntilPinCommonValue.end = _total;
					}
				}
				uint current = _phase2.m._remainUntilPinCommonValue.current;
				_phase2.m._objListRestPinMeter[0].SetNumber(current);
				_phase2.m._objListRestPinMeter[1].SetNumber(current);
				_phase2.m._objListRestPinMeter[2].SetNumber(current);
				_phase2.m._objListRestPinMeter[3].SetNumber(current);
				_phase2._restPinMeter.Play("in");
				_phase2.m._totalMapRunMeterValue = _current;
				_phase2.m._objListTotalMapRunMeter[0].SetNumber(_phase2.m._totalMapRunMeterValue);
				_state = MonitorState.Phase2MeterFadeInWait;
				_timer = 60;
				break;
			}
			case MonitorState.Phase2MeterFadeInWait:
			{
				if (!_phase2.m.IsEndAnim(_phase2._totalBonusMeter) && _timer != 0)
				{
					break;
				}
				_state = MonitorState.Phase2AddRainbow;
				uint num33 = _total - _current;
				_ = _phase2.m._valueListRainbow[_phase2.m._rainbowID];
				_ = _phase2.m._distanceBase[_phase2.m._rainbowID];
				CommonValueUint commonValueUint2 = _phase2.m._valueListRainbowUint[_phase2.m._rainbowID];
				uint end2 = commonValueUint2.end;
				uint start3 = commonValueUint2.start;
				uint num34 = end2 - start3;
				_phase2.m._toatalBonusCommonValue.start = num33;
				if (num33 > num34)
				{
					_phase2.m._toatalBonusCommonValue.end = num33 - num34;
				}
				else
				{
					_phase2.m._toatalBonusCommonValue.end = 0u;
				}
				_phase2.m._toatalBonusCommonValue.current = _phase2.m._toatalBonusCommonValue.start;
				_phase2.m._toatalBonusCommonValue.diff = _phase2.m._valueListRainbowUint[_phase2.m._rainbowID].diff;
				_phase2.m._toatalBonusCommonValue.sub = true;
				if (!_mapData.IsInfinity)
				{
					break;
				}
				int i3 = _listPinInfo.Count - 1;
				uint prizeDistance2 = _phase2.m.GetPrizeDistance(_listPinInfo, i3);
				if (_current >= prizeDistance2)
				{
					num34 = num33;
					_phase2.m._toatalBonusCommonValue.start = num33;
					if (num33 > num34)
					{
						_phase2.m._toatalBonusCommonValue.end = num33 - num34;
					}
					else
					{
						_phase2.m._toatalBonusCommonValue.end = 0u;
					}
					_phase2.m._toatalBonusCommonValue.current = _phase2.m._toatalBonusCommonValue.start;
				}
				break;
			}
			case MonitorState.Phase2AddRainbow:
				isSkipedTimerZero();
				if (_timer == 0)
				{
					Animator component6 = _phase2.m._Derakkuma2.GetComponent<Animator>();
					int layerIndex10 = component6.GetLayerIndex("Side");
					component6.SetLayerWeight(layerIndex10, 1f);
					if (_isSetLoopSE1)
					{
						SoundManager.StopSE(_loopSE1);
						_isSetLoopSE1 = false;
					}
					if (_isCardPass)
					{
						SoundManager.StopSE(_loopSE2);
						_isSetLoopSE2 = false;
					}
					switch (_phase2.m._RoadSpeedType)
					{
					case 0:
						component6.Play("S_Run_R_01", layerIndex10, 0f);
						_phase2.m._DerakkumaSmoke01.SetActive(value: true);
						_phase2.m._DerakkumaSmoke = _phase2.m._DerakkumaSmoke01;
						_loopSE1 = SoundManager.PlayLoopSE(Mai2.Mai2Cue.Cue.SE_MAP_MOVE_WALK_01, _monitorID);
						break;
					case 1:
						component6.Play("S_Run_R_02", layerIndex10, 0f);
						_phase2.m._DerakkumaSmoke02.SetActive(value: true);
						_phase2.m._DerakkumaSmoke = _phase2.m._DerakkumaSmoke02;
						_loopSE1 = SoundManager.PlayLoopSE(Mai2.Mai2Cue.Cue.SE_MAP_MOVE_WALK_02, _monitorID);
						break;
					case 2:
						component6.Play("S_Run_R_03", layerIndex10, 0f);
						_phase2.m._DerakkumaSmoke03.SetActive(value: true);
						_phase2.m._DerakkumaSmoke = _phase2.m._DerakkumaSmoke03;
						_loopSE1 = SoundManager.PlayLoopSE(Mai2.Mai2Cue.Cue.SE_MAP_MOVE_WALK_03, _monitorID);
						break;
					}
					_isSetLoopSE1 = true;
					if (_isCardPass)
					{
						_loopSE2 = SoundManager.PlayLoopSE(Mai2.Mai2Cue.Cue.SE_MAP_MOVE_BOOST_01, _monitorID, 1);
						_isSetLoopSE2 = true;
					}
					if (_isCardPass)
					{
						component6 = _phase2.m._DerakkumaFire.GetComponent<Animator>();
						component6.Play("Loop", 0, 0f);
					}
					_state = MonitorState.Phase2AddRainbowWait;
					_timer = 120;
					int num54 = 0;
					num54 = ((_phase2.m._rainbowID < _listPinInfo.Count - 1) ? _phase2.m._rainbowID : (_phase2.m._valueListRainbow.Count - 1));
					float num55 = _timer;
					if (_phase2.m._valueListRainbow[num54].diff != 0f)
					{
						bool flag26 = false;
						if (_phase2.m._valueListRainbow[num54].diff > _phase2.m._paramRainbowMinDiff)
						{
							num55 = (_phase2.m._valueListRainbow[num54].end - _phase2.m._valueListRainbow[num54].start) / _phase2.m._valueListRainbow[num54].diff;
							if (num55 >= (float)(_phase2.m._paramTotalTime * 2))
							{
								flag26 = true;
							}
						}
						else
						{
							flag26 = true;
						}
						if (flag26)
						{
							num55 = ((!_mapData.IsInfinity) ? ((float)((_phase2.m._valueListRainbowUint[num54].end - _phase2.m._valueListRainbowUint[num54].start) / _phase2.m._valueListRainbowUint[num54].diff)) : ((float)((_phase2.m._valueListRainbowUint[num54].end - _phase2.m._valueListRainbowUint[num54].start) / _phase2.m._valueListRainbowUint[num54].diff)));
						}
						if (_mapData.IsInfinity)
						{
							uint prizeDistance4 = _phase2.m.GetPrizeDistance(_listPinInfo, _listPinInfo.Count - 1);
							if (_phase2.m._remainUntilPinCommonValue.current >= prizeDistance4)
							{
								_phase2.m._valueListRainbowUint[num54].start = _phase2.m._remainUntilPinCommonValue.current;
								_phase2.m._valueListRainbowUint[num54].end = _phase2.m._remainUntilPinCommonValue.current + _phase2.m._toatalBonusCommonValue.current;
								_phase2.m._valueListRainbowUint[num54].current = _phase2.m._valueListRainbowUint[num54].start;
								_phase2.m._valueListRainbowUint[num54].diff = (_phase2.m._valueListRainbowUint[num54].end - _phase2.m._valueListRainbowUint[num54].start) / _phase2.m._paramMinWaitTime2;
								_phase2.m._remainUntilPinCommonValue.diff = _phase2.m._valueListRainbowUint[num54].diff;
								_phase2.m._toatalBonusCommonValue.diff = _phase2.m._valueListRainbowUint[num54].diff;
							}
						}
						num55 += 1f;
					}
					_timer = (int)num55 + (int)_phase2.m._paramAddFrame;
					_sub_count = 0;
				}
				CheckRainbowSkipStart();
				break;
			case MonitorState.Phase2AddRainbowWait:
			{
				isSkipedTimerZero();
				bool flag15 = false;
				int num35 = _phase2.m._rainbowID + 1;
				bool flag16 = num35 >= _phase2.m._rainbowEnd || num35 >= _listPinInfo.Count;
				bool flag17 = false;
				bool flag18 = false;
				flag18 = flag16 && _mapData.IsInfinity;
				int num36 = _phase2.m._rainbowID;
				bool flag19 = false;
				if (_phase2.m._nextOtomodachiInsertID != -1 && num36 == _phase2.m._nextOtomodachiInsertID && num36 == _listPinInfo.Count - 1 && _phase2.m._valueListRainbow[num36].end >= 100f && !_phase2.m._nextOtomodachiNothing)
				{
					flag19 = true;
				}
				CommonValue commonValue2 = null;
				if (!flag18)
				{
					commonValue2 = _phase2.m._valueListRainbow[num36];
				}
				else if (flag19)
				{
					commonValue2 = _phase2.m._valueListRainbow[num36];
				}
				else if (num36 >= _listPinInfo.Count - 1)
				{
					int num37 = _phase2.m._valueListRainbow.Count - 1;
					commonValue2 = _phase2.m._valueListRainbow[num37];
					num36 = num37;
				}
				else
				{
					commonValue2 = _phase2.m._valueListRainbow[num36];
				}
				bool flag20 = false;
				if (_phase2.m._valueListRainbow[num36].diff > _phase2.m._paramRainbowMinDiff)
				{
					if ((_phase2.m._valueListRainbow[num36].end - _phase2.m._valueListRainbow[num36].start) / _phase2.m._valueListRainbow[num36].diff >= (float)(_phase2.m._paramTotalTime * 2))
					{
						flag20 = true;
					}
				}
				else
				{
					flag20 = true;
				}
				bool flag21 = false;
				if (_isSkip || _phase2.m._valueListRainbowCountUpRate[num36] == 0 || (_sub_count >= _phase2.m._valueListRainbowCountUpRate[num36] && (long)_sub_count % (long)_phase2.m._valueListRainbowCountUpRate[num36] == 0L))
				{
					flag21 = true;
				}
				if (!flag18)
				{
					if (flag21)
					{
						if (_isSkip)
						{
							_phase2.m._toatalBonusCommonValue.current = _phase2.m._toatalBonusCommonValue.end;
						}
						_phase2.m._toatalBonusCommonValue.UpdateValue();
					}
				}
				else if (flag21)
				{
					if (_isSkip)
					{
						_phase2.m._toatalBonusCommonValue.current = _phase2.m._toatalBonusCommonValue.end;
					}
					flag15 = _phase2.m._toatalBonusCommonValue.UpdateValue();
				}
				_phase2.m._objListTotalBonusMeter[0].SetNumber(_phase2.m._toatalBonusCommonValue.current);
				_phase2.m._objListTotalBonusMeter[1].SetNumber(_phase2.m._toatalBonusCommonValue.current);
				CommonValueUint remainUntilPinCommonValue = _phase2.m._remainUntilPinCommonValue;
				if (flag21)
				{
					if (_isSkip)
					{
						remainUntilPinCommonValue.current = remainUntilPinCommonValue.end;
					}
					remainUntilPinCommonValue.UpdateValue();
				}
				_phase2.m._objListRestPinMeter[0].SetNumber(remainUntilPinCommonValue.current);
				_phase2.m._objListRestPinMeter[1].SetNumber(remainUntilPinCommonValue.current);
				_phase2.m._objListRestPinMeter[2].SetNumber(remainUntilPinCommonValue.current);
				_phase2.m._objListRestPinMeter[3].SetNumber(remainUntilPinCommonValue.current);
				if (_mapData.IsInfinity)
				{
					uint current2 = remainUntilPinCommonValue.current;
					_phase2.m._totalMapRunMeterValue = current2;
					_phase2.m._objListTotalMapRunMeter[0].SetNumber(_phase2.m._totalMapRunMeterValue);
				}
				else
				{
					uint num38 = _phase2.m._distanceStart[num36];
					uint current3 = remainUntilPinCommonValue.current;
					uint num39 = 0u;
					if ((uint)_phase2.m._distanceBase[num36] >= current3)
					{
						num39 = (uint)_phase2.m._distanceBase[num36] - current3;
					}
					uint totalMapRunMeterValue = num38 + num39;
					_phase2.m._totalMapRunMeterValue = totalMapRunMeterValue;
					_phase2.m._objListTotalMapRunMeter[0].SetNumber(_phase2.m._totalMapRunMeterValue);
				}
				float start4 = commonValue2.start;
				if (flag16 && start4 >= 100f)
				{
					flag17 = true;
					if (!_mapData.IsInfinity && num35 >= _listPinInfo.Count)
					{
						_phase2.m._isEndPinClear = true;
					}
				}
				uint prizeDistance3 = _phase2.m.GetPrizeDistance(_listPinInfo, _phase2.m._rainbowID);
				uint num40 = 0u;
				if (_phase2.m._rainbowID > 0)
				{
					num40 = _phase2.m.GetPrizeDistance(_listPinInfo, _phase2.m._rainbowID - 1);
				}
				uint num41 = prizeDistance3 - num40;
				if (num41 > 10000)
				{
					num41 /= 100u;
					num41 *= _phase2.m._paramDispPercent;
					num41 = num40 + num41;
				}
				else
				{
					float num42 = (float)num41 / 100f;
					num42 *= (float)_phase2.m._paramDispPercent;
					num41 = num40 + (uint)num42;
				}
				bool flag22 = flag18 && _phase2.m.GetPrizeDistance(_listPinInfo, num36) == _total;
				bool flag23 = flag18 && _phase2.m.GetPrizeDistance(_listPinInfo, num36) <= _total && num36 >= _listPinInfo.Count - 1;
				bool flag24 = !flag18 || flag19 || flag22 || flag23;
				bool flag25 = _phase2.m._nextOtomodachiNothing;
				if (_phase2.m._nextOtomodachiNothing && num36 < _listPinInfo.Count - 1)
				{
					flag25 = false;
				}
				if (!flag17 && !_phase2.m._isUpdateNearPin && _phase2.m._rainbowID <= _listPinInfo.Count - 1 && _total >= num41 && flag24 && !flag25 && _timer <= _phase2.m._paramMinWaitTime2 + _phase2.m._paramAddFrame)
				{
					_phase2.m._isSetPosFarPin = false;
					_phase2.m._isUpdateFarPin = false;
					_phase2.m._objFarPin.SetActive(value: false);
					_phase2.m._objFarPinGhost.SetActive(value: false);
					_phase2.m._isUpdateNearPin = true;
					if (!_phase2.m._isSetPosNearPin)
					{
						_phase2.m._nearPinX.start = _phase2.m._paramNearPinStart;
					}
					else
					{
						_phase2.m._nearPinX.start = _phase2.m._nearPinX.current;
					}
					if (_phase2.m.IsGetPrize(_listPinInfo, num36, _total))
					{
						_phase2.m._nearPinX.end = _phase2.m._paramNearPinReached;
					}
					else
					{
						_phase2.m._nearPinX.end = _phase2.m._paramNearPinUnreached;
					}
					_phase2.m._nearPinX.diff = 0f;
					_phase2.m._nearPinX.diff = (_phase2.m._nearPinX.end - _phase2.m._nearPinX.start) / (float)_phase2.m._paramMinWaitTime2;
					_phase2.m._nearPinX.current = _phase2.m._nearPinX.start;
					int otomodachi_music_id2 = -1;
					if (_otomodachiMusicID != null && _otomodachiMusicID.Count > 0 && num36 - _phase2.m._rainbowStart < _otomodachiMusicID.Count)
					{
						otomodachi_music_id2 = _otomodachiMusicID[num36 - _phase2.m._rainbowStart];
					}
					_phase2.m.SetNearPin(_assetManager, _mapData, _listPinInfo, num36, otomodachi_music_id2, reached: false);
				}
				if (!flag18)
				{
					if (flag20)
					{
						if (_isSkip)
						{
							_phase2.m._valueListRainbowUint[num36].current = _phase2.m._valueListRainbowUint[num36].end;
						}
						flag15 = _phase2.m._valueListRainbowUint[num36].UpdateValue();
					}
					else
					{
						if (_isSkip)
						{
							commonValue2.current = commonValue2.end;
						}
						flag15 = commonValue2.UpdateValue();
					}
				}
				else if (flag19)
				{
					if (flag20)
					{
						if (_isSkip)
						{
							_phase2.m._valueListRainbowUint[num36].current = _phase2.m._valueListRainbowUint[num36].end;
						}
						flag15 = _phase2.m._valueListRainbowUint[num36].UpdateValue();
					}
					else
					{
						if (_isSkip)
						{
							commonValue2.current = commonValue2.end;
						}
						flag15 = commonValue2.UpdateValue();
					}
				}
				if (_phase2.m._isUpdateNearPin)
				{
					if (_isSkip)
					{
						_phase2.m._nearPinX.current = _phase2.m._nearPinX.end;
					}
					_phase2.m._nearPinX.UpdateValue();
					float current4 = _phase2.m._nearPinX.current;
					float num43 = 0f;
					num43 = _phase2.m._objNearPin.transform.localPosition.y;
					_phase2.m._objNearPin.transform.localPosition = new Vector3(current4, num43, 0f);
					num43 = _phase2.m._objNearPinGhost.transform.localPosition.y;
					_phase2.m._objNearPinGhost.transform.localPosition = new Vector3(current4, num43, 0f);
				}
				_phase2._imgRainbow01.fillAmount = commonValue2.current / 100f;
				float z2 = 90f - 1.8f * commonValue2.current;
				_phase2._objRainbowReached01.transform.localRotation = Quaternion.Euler(0f, 0f, z2);
				_phase2.m.ScrollRoad(_phase2._ground.gameObject);
				if (flag15 || _timer == 0)
				{
					Animator component5 = _phase2.m._Derakkuma2.GetComponent<Animator>();
					int layerIndex9 = component5.GetLayerIndex("Result");
					component5.SetLayerWeight(layerIndex9, 0f);
					layerIndex9 = component5.GetLayerIndex("Side");
					component5.SetLayerWeight(layerIndex9, 1f);
					component5.Play("S_Default_R", layerIndex9, 0f);
					if (_phase2.m._DerakkumaSmoke != null)
					{
						_phase2.m._DerakkumaSmoke.SetActive(value: false);
					}
					if (_isSetLoopSE1)
					{
						SoundManager.StopSE(_loopSE1);
						_isSetLoopSE1 = false;
					}
					if (_isCardPass && _isSetLoopSE2)
					{
						SoundManager.StopSE(_loopSE2);
						_isSetLoopSE2 = false;
					}
					_phase2.m._isSetPosNearPin = false;
					_phase2.m._isUpdateNearPin = false;
					uint num44 = 0u;
					num44 = _phase2.m._toatalBonusCommonValue.end;
					_phase2.m._objListTotalBonusMeter[0].SetNumber(num44);
					_phase2.m._objListTotalBonusMeter[1].SetNumber(num44);
					num44 = _phase2.m._remainUntilPinCommonValue.end;
					_phase2.m._objListRestPinMeter[0].SetNumber(num44);
					_phase2.m._objListRestPinMeter[1].SetNumber(num44);
					_phase2.m._objListRestPinMeter[2].SetNumber(num44);
					_phase2.m._objListRestPinMeter[3].SetNumber(num44);
					if (_mapData.IsInfinity)
					{
						uint totalMapRunMeterValue2 = num44;
						_phase2.m._totalMapRunMeterValue = totalMapRunMeterValue2;
						_phase2.m._objListTotalMapRunMeter[0].SetNumber(_phase2.m._totalMapRunMeterValue);
					}
					else
					{
						uint num45 = _phase2.m._distanceStart[num36];
						uint num46 = num44;
						uint num47 = 0u;
						if ((uint)_phase2.m._distanceBase[num36] >= num46)
						{
							num47 = (uint)_phase2.m._distanceBase[num36] - num46;
						}
						uint totalMapRunMeterValue3 = num45 + num47;
						_phase2.m._totalMapRunMeterValue = totalMapRunMeterValue3;
						_phase2.m._objListTotalMapRunMeter[0].SetNumber(_phase2.m._totalMapRunMeterValue);
					}
					if (!flag18)
					{
						if (commonValue2.end >= 100f)
						{
							_state = MonitorState.Phase2RainbowReached;
							if (_phase2.m._rainbowReleasePin >= 0 && _phase2.m._rainbowID == _phase2.m._rainbowReleasePin && !_phase2.m._isReleasePinOver)
							{
								_phase2.m._isReleasePinOver = true;
								if (_phase2.m._isReleasePinOver)
								{
									List<UserMapData> list = new List<UserMapData>();
									List<UserMapData> list2 = Singleton<MapMaster>.Instance.RefUserMapList[_monitorID];
									foreach (var item in list2.Select((UserMapData value, int index) => new { value, index }))
									{
										list.Add(item.value);
										if (list[list.Count - 1].ID == _mapID)
										{
											list[list.Count - 1].Distance = _phase2.m._distanceEnd[_phase2.m._rainbowID];
										}
									}
									for (int num48 = 0; num48 < list.Count; num48++)
									{
										UserMapData userMapData = list[num48];
										UserMapData[] source = list.ToArray();
										foreach (int id in userMapData.ReleaseIds)
										{
											UserMapData userMapData2 = source.FirstOrDefault((UserMapData m) => m.ID == id);
											if (userMapData2 == null)
											{
												continue;
											}
											if (!Singleton<MapMaster>.Instance.IsRelease(userMapData2.ID, userMapData2.Distance))
											{
												list[num48].IsLock = true;
											}
											else
											{
												list[num48].IsLock = false;
											}
											if (list2[num48].IsEvent || list[num48].IsLock)
											{
												continue;
											}
											int select_id = _mapID;
											int mapID = list[num48].ID;
											int num49 = _userData.MapList.FindIndex((UserMapData m) => m.ID == mapID);
											int num50 = list[num48].ReleaseIds.FindIndex((int m) => m == select_id);
											if (num49 >= 0 || num50 < 0)
											{
												continue;
											}
											MapData mapData = Singleton<DataManager>.Instance.GetMapData(mapID);
											int num51 = 0;
											if (mapData != null)
											{
												num51 = mapData.OpenEventId.id;
												if (Singleton<EventManager>.Instance.IsOpenEvent(num51))
												{
													_phase2.m._releaseIslandID.Add(list[num48].ID);
												}
											}
										}
									}
								}
							}
							if (flag17)
							{
								_state = MonitorState.Phase2RainbowFadeOut;
							}
						}
						else
						{
							_state = MonitorState.Phase2NextRainbowJudge;
							int id3 = _listPinInfo[_phase2.m._rainbowID].TreasureId.id;
							if (Singleton<DataManager>.Instance.GetMapTreasureData(id3) != null && Singleton<DataManager>.Instance.GetMapTreasureData(id3).TreasureType == MapTreasureType.Otomodachi)
							{
								_state = MonitorState.Phase2RainbowReachedOtomodachiFadeIn;
							}
							int num52 = _phase2.m._rainbowID + 1;
							if (num52 >= _phase2.m._rainbowEnd)
							{
								if (num52 >= _listPinInfo.Count)
								{
									num52 = _listPinInfo.Count - 1;
								}
								if (_total < _phase2.m.GetPrizeDistance(_listPinInfo, num52))
								{
									_state = MonitorState.Phase2NextRainbowJudge;
								}
							}
						}
					}
					else
					{
						_state = MonitorState.Phase2NextRainbowJudge;
						if (_phase2.m._toatalBonusCommonValue.current == 0)
						{
							_state = MonitorState.Phase2RainbowFadeOut;
						}
						if (_phase2.m._remainUntilPinCommonValue.end >= _phase2.m._limitDistanceMax)
						{
							_phase2.m._toatalBonusCommonValue.current = 0u;
							_phase2.m._toatalBonusCommonValue.end = 0u;
							num44 = _phase2.m._toatalBonusCommonValue.end;
							_phase2.m._objListTotalBonusMeter[0].SetNumber(num44);
							_phase2.m._objListTotalBonusMeter[1].SetNumber(num44);
							_state = MonitorState.Phase2RainbowFadeOut;
						}
						if (_phase2.m.IsGetPrize(_listPinInfo, _phase2.m._rainbowID, _total) && !flag25)
						{
							if (flag19)
							{
								if (!_phase2.m._isInfinityLastPinOver)
								{
									_phase2.m._isInfinityLastPinOver = true;
									_state = MonitorState.Phase2RainbowReachedOtomodachiFadeIn;
								}
							}
							else if (commonValue2.start < 100f && commonValue2.end >= 100f && !_phase2.m._isInfinityLastPinOver)
							{
								_phase2.m._isInfinityLastPinOver = true;
								_state = MonitorState.Phase2RainbowReached;
							}
							else
							{
								int id4 = _listPinInfo[_phase2.m._rainbowID].TreasureId.id;
								if (Singleton<DataManager>.Instance.GetMapTreasureData(id4) != null && Singleton<DataManager>.Instance.GetMapTreasureData(id4).TreasureType == MapTreasureType.Otomodachi)
								{
									_state = MonitorState.Phase2RainbowReachedOtomodachiFadeIn;
								}
							}
						}
					}
					_timer = 60;
				}
				if (_phase2.m._isUpdateFarPin)
				{
					if (_isSkip)
					{
						_phase2.m._farPinX.current = _phase2.m._farPinX.end;
					}
					flag15 = _phase2.m._farPinX.UpdateValue();
					float current6 = _phase2.m._farPinX.current;
					float num53 = 0f;
					num53 = _phase2.m._objFarPin.transform.localPosition.y;
					_phase2.m._objFarPin.transform.localPosition = new Vector3(current6, num53, 0f);
					num53 = _phase2.m._objFarPinGhost.transform.localPosition.y;
					_phase2.m._objFarPinGhost.transform.localPosition = new Vector3(current6, num53, 0f);
					if (flag15)
					{
						_phase2.m._objFarPin.SetActive(value: false);
						_phase2.m._objFarPinGhost.SetActive(value: false);
					}
				}
				_sub_count++;
				CheckRainbowSkipStart();
				break;
			}
			case MonitorState.Phase2RainbowReachedOtomodachiFadeIn:
				isSkipedTimerZero();
				if (_timer == 0)
				{
					int num32 = 0;
					int index12 = 0;
					int otomodachi_music_id = -1;
					if (_otomodachiMusicID != null && _otomodachiMusicID.Count > 0 && _phase2.m._rainbowID - _phase2.m._rainbowStart < _otomodachiMusicID.Count)
					{
						otomodachi_music_id = _otomodachiMusicID[_phase2.m._rainbowID - _phase2.m._rainbowStart];
					}
					num32 = _phase2.m.SetPrize(_assetManager, _mapData, _listPinInfo, _phase2.m._objListPrize[index12], _phase2.m._rainbowID, otomodachi: true, otomodachi_music_id);
					_phase2.m._prizeWindowType = num32;
					_phase2.m._objListPrize[index12].SetActive(value: true);
					_phase2.m.SetActiveChildren(_phase2.m._objListPrize[index12], active: true);
					if (num32 != -1)
					{
						GameObject gameObject7 = _phase2.m._objListPrize[index12].transform.GetChild(num32).gameObject;
						_phase2.m._prize = gameObject7.GetComponent<Animator>();
						_phase2.m._prize.Play("Idle", 0, 0f);
						_phase2.m._prize.Play("In", 0, 0f);
					}
					_state = MonitorState.Phase2RainbowReachedOtomodachiFadeInWait;
					_timer = _DispGetWindowTime;
					ResetSkipStart();
					_state = MonitorState.Phase2RainbowReachedOtomodachiFadeOutWait;
					_timer = 0;
				}
				else
				{
					CheckRainbowSkipStart();
				}
				break;
			case MonitorState.Phase2RainbowReachedOtomodachiFadeInWait:
			{
				isSkipedTimerZero();
				bool flag9 = false;
				if ((_phase2.m._prizeWindowType != -1) ? (_phase2.m.IsEndAnim(_phase2.m._prize) || _timer == 0) : (_timer == 0))
				{
					_state = MonitorState.Phase2RainbowReachedOtomodachiFadeOut;
					_timer = 30;
				}
				CheckGetWindowSkipStart();
				break;
			}
			case MonitorState.Phase2RainbowReachedOtomodachiFadeOut:
				isSkipedTimerZero();
				if (_timer == 0)
				{
					_state = MonitorState.Phase2RainbowReachedOtomodachiFadeOutWait;
					_timer = 30;
				}
				CheckGetWindowSkipStart();
				break;
			case MonitorState.Phase2RainbowReachedOtomodachiFadeOutWait:
				isSkipedTimerZero();
				if (_phase2.m.IsEndAnim(_phase2.m._prize) || _timer == 0)
				{
					int prizeWindowType6 = _phase2.m._prizeWindowType;
					int index15 = 0;
					if (prizeWindowType6 != -1)
					{
						_phase2.m._objListPrize[index15].transform.GetChild(prizeWindowType6).gameObject.SetActive(value: false);
					}
					_phase2.m.SetActiveChildren(_phase2.m._objListPrize[index15], active: false);
					_phase2.m._objListPrize[index15].SetActive(value: false);
					if (_phase2.m._valueListRainbow[_phase2.m._rainbowID].end >= 100f)
					{
						_state = MonitorState.Phase2NextRainbow;
						if (_phase2.m._isReleasePinOver)
						{
							_state = MonitorState.Phase2MapReleaseFadeIn;
							ResetSkipStart();
						}
					}
					else
					{
						_state = MonitorState.Phase2NextRainbowJudge;
					}
					_phase2.m._objNearPin.SetActive(value: false);
					_phase2.m._objNearPinGhost.SetActive(value: false);
					int num60 = _phase2.m._rainbowID;
					if (num60 >= _listPinInfo.Count - 1)
					{
						num60 = _phase2.m._valueListRainbow.Count - 1;
					}
					int otomodachi_music_id3 = -1;
					if (_otomodachiMusicID != null && _otomodachiMusicID.Count > 0 && num60 - _phase2.m._rainbowStart < _otomodachiMusicID.Count)
					{
						otomodachi_music_id3 = _otomodachiMusicID[num60 - _phase2.m._rainbowStart];
					}
					_phase2.m._isUpdateFarPin = true;
					_phase2.m._farPinX.start = _phase2.m._paramNearPinReached;
					_phase2.m._farPinX.end = -1f * _phase2.m._paramNearPinStart;
					_phase2.m._farPinX.diff = 0f;
					_phase2.m._farPinX.diff = (_phase2.m._farPinX.end - _phase2.m._farPinX.start) / (float)_phase2.m._paramMinWaitTime2;
					_phase2.m._farPinX.current = _phase2.m._farPinX.start;
					float current7 = _phase2.m._farPinX.current;
					float num61 = 0f;
					num61 = _phase2.m._objFarPin.transform.localPosition.y;
					_phase2.m._objFarPin.transform.localPosition = new Vector3(current7, num61, 0f);
					num61 = _phase2.m._objFarPinGhost.transform.localPosition.y;
					_phase2.m._objFarPinGhost.transform.localPosition = new Vector3(current7, num61, 0f);
					_phase2.m._isSetPosFarPin = true;
					_phase2.m._isUpdateFarPin = true;
					_phase2.m.SetFarPin(_assetManager, _mapData, _listPinInfo, num60, otomodachi_music_id3, reached: true);
					_timer = 30;
					ResetSkipStart();
				}
				else
				{
					CheckGetWindowSkipStart();
				}
				break;
			case MonitorState.Phase2RainbowReached:
			{
				isSkipedTimerZero();
				CheckChallengeStatus();
				if (_isEnableRainbow)
				{
					if (_phase2.m._isChallenge)
					{
						if (!_phase2.m._isCommonStart && _phase2.m._rainbowChallengeStart)
						{
						}
					}
					else
					{
						_phase2._travelGauge01.Play("MaxGauge", 0, 0f);
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_MAP_RESULT_RAINBOW_MAX, _monitorID);
					}
				}
				int num80 = _phase2.m._rainbowID + 1;
				bool flag32 = num80 >= _phase2.m._rainbowEnd || num80 >= _listPinInfo.Count;
				if (!_mapData.IsInfinity)
				{
					if (num80 >= _listPinInfo.Count)
					{
						flag32 = true;
					}
				}
				else
				{
					flag32 = false;
				}
				int num81 = _phase2.m._rainbowID;
				if (num81 >= _listPinInfo.Count - 1)
				{
					num81 = _phase2.m._valueListRainbow.Count - 1;
				}
				int otomodachi_music_id4 = -1;
				if (_otomodachiMusicID != null && _otomodachiMusicID.Count > 0 && num81 - _phase2.m._rainbowStart < _otomodachiMusicID.Count)
				{
					otomodachi_music_id4 = _otomodachiMusicID[num81 - _phase2.m._rainbowStart];
				}
				int num82 = 0;
				num82 = _phase2.m.GetPrizeWindowType(_mapData, _listPinInfo, _phase2.m._rainbowID, otomodachi: true);
				if (flag32)
				{
					if (num82 != 3)
					{
						_phase2.m.SetNearPin(_assetManager, _mapData, _listPinInfo, num81, otomodachi_music_id4, reached: true);
					}
					else
					{
						_phase2.m.SetNearPin(_assetManager, _mapData, _listPinInfo, num81, otomodachi_music_id4, reached: true);
					}
				}
				else
				{
					_phase2.m.SetNearPin(_assetManager, _mapData, _listPinInfo, num81, otomodachi_music_id4, reached: true);
				}
				_phase2.m._isUpdateFarPin = true;
				_phase2.m._farPinX.start = _phase2.m._paramNearPinReached;
				_phase2.m._farPinX.end = -1f * _phase2.m._paramNearPinStart;
				_phase2.m._farPinX.diff = 0f;
				_phase2.m._farPinX.diff = (_phase2.m._farPinX.end - _phase2.m._farPinX.start) / (float)_phase2.m._paramMinWaitTime2;
				_phase2.m._farPinX.current = _phase2.m._farPinX.start;
				float current8 = _phase2.m._farPinX.current;
				float num83 = 0f;
				num83 = _phase2.m._objFarPin.transform.localPosition.y;
				_phase2.m._objFarPin.transform.localPosition = new Vector3(current8, num83, 0f);
				num83 = _phase2.m._objFarPinGhost.transform.localPosition.y;
				_phase2.m._objFarPinGhost.transform.localPosition = new Vector3(current8, num83, 0f);
				_phase2.m._isSetPosFarPin = true;
				_phase2.m._isUpdateFarPin = true;
				if (flag32)
				{
					if (num82 != 3)
					{
						_phase2.m.SetFarPin(_assetManager, _mapData, _listPinInfo, num81, otomodachi_music_id4, reached: true);
					}
					else
					{
						_phase2.m.SetFarPin(_assetManager, _mapData, _listPinInfo, num81, otomodachi_music_id4, reached: true);
					}
				}
				else
				{
					_phase2.m.SetFarPin(_assetManager, _mapData, _listPinInfo, num81, otomodachi_music_id4, reached: true);
				}
				_state = MonitorState.Phase2RainbowReachedWait;
				_timer = 30;
				CheckRainbowSkipStart();
				break;
			}
			case MonitorState.Phase2RainbowReachedWait:
			{
				isSkipedTimerZero();
				bool flag27 = false;
				if (_isEnableRainbow)
				{
					flag27 = ((!_phase2.m._isCommonStart) ? _phase2.m.IsEndAnim(_phase2._lockRainbow01) : _phase2.m.IsEndAnim(_phase2._travelGauge01));
				}
				if (flag27 || _timer == 0)
				{
					if (_phase2.m._isChallenge)
					{
						_state = MonitorState.Phase2RainbowReachedChallengePostJudge;
					}
					else
					{
						_state = MonitorState.Phase2RainbowReachedGetPrizeFadeIn;
						_timer = 60;
					}
				}
				CheckRainbowSkipStart();
				break;
			}
			case MonitorState.Phase2RainbowReachedChallengePostJudge:
				if (_timer != 0)
				{
					break;
				}
				if (_phase2.m._isCommonStart)
				{
					_state = MonitorState.Phase2NextRainbow;
				}
				else
				{
					_total = _phase2.m._totalMapRunMeterValue;
					if (_phase2.m._rainbowChallengeStart)
					{
						_state = MonitorState.Phase2RainbowFadeOut;
					}
					else
					{
						_state = MonitorState.Phase2RainbowReachedChallengeFadeIn;
					}
				}
				if (_phase2.m._isChallenge)
				{
					if (_phase2.m._rainbowChallengeStart)
					{
						_phase2.m._rainbowChallengeStart = false;
					}
					_phase2.m._isChallenge = false;
					_phase2.m._isCommonStart = true;
					_phase2.m._isChallengeDifficultyOK = false;
				}
				_timer = 10;
				break;
			case MonitorState.Phase2RainbowReachedChallengeFadeIn:
				if (_timer == 0)
				{
					_state = MonitorState.Phase2RainbowReachedChallengeFadeInWait;
				}
				break;
			case MonitorState.Phase2RainbowReachedChallengeFadeInWait:
				if (_timer == 0)
				{
					_state = MonitorState.Phase2RainbowReachedChallengeInfo01FadeIn;
				}
				break;
			case MonitorState.Phase2RainbowReachedChallengeInfo01FadeIn:
				if (_timer == 0)
				{
					int challengeInfoType = _phase2.m._challengeInfoType;
					_phase2.m.SetChallengeAppearedInfo(_assetManager, _mapData, _listPinInfo, _phase2.m._objListChallengeInfo[challengeInfoType], _phase2.m._rainbowID);
					int challengeMusicID = _phase2.m.GetChallengeMusicID(_mapData, _listPinInfo, _phase2.m._rainbowID);
					if (challengeMusicID != -1 && !GameManager.IsFreedomMode)
					{
						_userData.Extend.SelectMusicID = challengeMusicID;
						GameManager.IsForceChangeMusic = true;
					}
					_phase2.m._objListChallengeInfo[challengeInfoType].SetActive(value: true);
					_phase2.m.SetActiveChildren(_phase2.m._objListChallengeInfo[challengeInfoType], active: true);
					_phase2.m._challengeInfo = _phase2.m._objListChallengeInfo[challengeInfoType].GetComponent<Animator>();
					_phase2.m._challengeInfo.Play("In", 0, 0f);
					switch (_phase2.m._challengeInfoType)
					{
					case 1:
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_MAP_NEW_CHALLENGE_02, _monitorID);
						SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000255, _monitorID);
						break;
					case 0:
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_MAP_NEW_CHALLENGE_01, _monitorID);
						SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000256, _monitorID);
						break;
					}
					_state = MonitorState.Phase2RainbowReachedChallengeInfo01FadeInWait;
					_timer = 30;
				}
				break;
			case MonitorState.Phase2RainbowReachedChallengeInfo01FadeInWait:
				if (_phase2.m.IsEndAnim(_phase2.m._challengeInfo) || _timer == 0)
				{
					_state = MonitorState.Phase2RainbowReachedChallengeInfo01FadeOut;
					_timer = _DispGetWindowTime;
					ResetSkipStart();
				}
				break;
			case MonitorState.Phase2RainbowReachedChallengeInfo01FadeOut:
				isSkipedTimerZero();
				if (_timer == 0)
				{
					_phase2.m._challengeInfo.Play("Out", 0, 0f);
					_state = MonitorState.Phase2RainbowReachedChallengeInfo01FadeOutWait;
					ResetSkipStart();
				}
				else
				{
					CheckGetWindowSkipStart();
				}
				break;
			case MonitorState.Phase2RainbowReachedChallengeInfo01FadeOutWait:
				if (_timer == 0)
				{
					_state = MonitorState.Phase2RainbowReachedChallengeInfo02FadeIn;
				}
				break;
			case MonitorState.Phase2RainbowReachedChallengeInfo02FadeIn:
				if (_timer == 0)
				{
					_phase2._lockRainbow01.gameObject.SetActive(value: true);
					_phase2._lockRainbow01.Play("In", 0, 0f);
					_phase2._objRainbowReached01.SetActive(value: false);
					_phase2._restPinMeter.gameObject.SetActive(value: false);
					_phase2._totalBonusMeter.gameObject.SetActive(value: false);
					_phase2._challengeInfo01.gameObject.SetActive(value: true);
					_phase2.m.SetActiveChildren(_phase2._challengeInfo01.gameObject.transform.GetChild(0).gameObject, active: false);
					switch (_phase2.m._challengeInfoType)
					{
					case 1:
					{
						_phase2._challengeInfo01.gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(value: true);
						int challengeID = _phase2.m.GetChallengeID(_mapData, _listPinInfo, _phase2.m._rainbowID);
						_phase2.SetLifeInfo(challengeID);
						break;
					}
					case 0:
						_phase2._challengeInfo01.gameObject.transform.GetChild(0).GetChild(1).gameObject.SetActive(value: true);
						break;
					}
					_phase2._challengeInfo01.Play("In", 0, 0f);
					_phase2.m.SetChallengeLockedInfo(_assetManager, _mapData, _listPinInfo, _phase2._challengeInfo02.gameObject, _phase2.m._rainbowID);
					_phase2.m.SetActiveChildren(_phase2._challengeInfo02.gameObject, active: false);
					_phase2._challengeInfo02.gameObject.SetActive(value: true);
					_phase2._challengeInfo02.gameObject.transform.GetChild(0).gameObject.SetActive(value: true);
					int layerIndex = _phase2._challengeInfo02.GetLayerIndex("Base Layer");
					_phase2._challengeInfo02.SetLayerWeight(layerIndex, 1f);
					_phase2._challengeInfo02.Play("In", 0, 0f);
					layerIndex = _phase2._challengeInfo02.GetLayerIndex("Color Layer");
					_phase2._challengeInfo02.SetLayerWeight(layerIndex, 1f);
					switch (_phase2.m._challengeInfoType)
					{
					case 1:
						_phase2._challengeInfo02.Play("Color_PerfectChallenge", layerIndex, 0f);
						break;
					case 0:
						_phase2._challengeInfo02.Play("Color_Task", layerIndex, 0f);
						break;
					}
					_state = MonitorState.Phase2RainbowReachedChallengeInfo02FadeInWait;
					_timer = 30;
				}
				break;
			case MonitorState.Phase2RainbowReachedChallengeInfo02FadeInWait:
				if (_phase2.m.IsEndAnim(_phase2._challengeInfo01) || _timer == 0)
				{
					_state = MonitorState.Phase2RainbowReachedChallengeInfo02FadeOut;
					_timer = 300;
				}
				break;
			case MonitorState.Phase2RainbowReachedChallengeInfo02FadeOut:
				isSkipedTimerZero();
				if (_timer == 0)
				{
					_phase2._challengeInfo01.Play("Out", 0, 0f);
					_phase2._challengeInfo02.Play("Out", 0, 0f);
					_state = MonitorState.Phase2RainbowReachedChallengeInfo02FadeOutWait;
					_timer = 30;
					ResetSkipStart();
				}
				else
				{
					CheckGetWindowSkipStart();
				}
				break;
			case MonitorState.Phase2RainbowReachedChallengeInfo02FadeOutWait:
				if (_phase2.m.IsEndAnim(_phase2._challengeInfo01) || _timer == 0)
				{
					_phase2.m.SetActiveChildren(_phase2._challengeInfo01.gameObject.transform.GetChild(0).gameObject, active: false);
					_phase2._challengeInfo01.gameObject.SetActive(value: false);
					_phase2.m.SetActiveChildren(_phase2._challengeInfo02.gameObject, active: false);
					_phase2._challengeInfo02.gameObject.SetActive(value: false);
					_state = MonitorState.Phase2RainbowFadeOut;
					_timer = 30;
				}
				break;
			case MonitorState.Phase2RainbowReachedGetPrizeFadeIn:
				isSkipedTimerZero();
				if (_timer == 0)
				{
					int num21 = 0;
					int num22 = _phase2.m.GetPrizeWindowType(_mapData, _listPinInfo, _phase2.m._rainbowID, otomodachi: false);
					if (num22 < 0)
					{
						num22 = 0;
					}
					num21 = _phase2.m.SetPrize(_assetManager, _mapData, _listPinInfo, _phase2.m._objListPrize[num22], _phase2.m._rainbowID, otomodachi: false, -1);
					_phase2.m._prizeWindowType = num21;
					if (_phase2.m.IsPrizeCharacter(_listPinInfo, _phase2.m._rainbowID, otomodachi: false))
					{
						_phase2.m._isGetNewCharacter = true;
					}
					int index10 = _phase2.m._rainbowPinIcon[_phase2.m._rainbowID];
					int id2 = _listPinInfo[index10].TreasureId.id;
					MapTreasureData mapTreasureData3 = Singleton<DataManager>.Instance.GetMapTreasureData(id2);
					if (mapTreasureData3 != null)
					{
						switch (mapTreasureData3.TreasureType)
						{
						case MapTreasureType.Character:
						{
							SoundManager.PlaySE(Mai2.Mai2Cue.Cue.JINGLE_MAP_GET, _monitorID);
							int num23 = 0;
							num23 = _phase2.m.GetConvertedTreasureID(_mapData, _listPinInfo, _phase2.m._rainbowID, otomodachi: false);
							MapTreasureData mapTreasureData4 = Singleton<DataManager>.Instance.GetMapTreasureData(num23);
							if (mapTreasureData4 != null)
							{
								switch (mapTreasureData4.CharacterId.id)
								{
								case 102:
									SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000239, _monitorID);
									break;
								case 103:
									SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000241, _monitorID);
									break;
								case 104:
									SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000240, _monitorID);
									break;
								case 105:
									SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000242, _monitorID);
									break;
								default:
									SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000145, _monitorID);
									break;
								}
							}
							break;
						}
						case MapTreasureType.MusicNew:
							SoundManager.PlaySE(Mai2.Mai2Cue.Cue.JINGLE_MAP_GET, _monitorID);
							SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000146, _monitorID);
							break;
						}
					}
					_phase2.m._objListPrize[num22].SetActive(value: true);
					_phase2.m.SetActiveChildren(_phase2.m._objListPrize[num22], active: true);
					int num24 = _phase2.m._rainbowID + 1;
					bool num25 = num24 >= _phase2.m._rainbowEnd || num24 >= _listPinInfo.Count;
					bool flag13 = false;
					flag13 = !num25 || _mapData.IsInfinity || (!_mapData.IsInfinity && num24 < _listPinInfo.Count);
					switch (num21)
					{
					default:
					{
						GameObject gameObject6 = _phase2.m._objListPrize[num22].gameObject;
						gameObject6.SetActive(value: true);
						_phase2.m._prize = gameObject6.GetComponent<Animator>();
						_phase2.m._prize.Play("Idle", 0, 0f);
						_phase2.m._prize.Play("In", 0, 0f);
						break;
					}
					case 3:
						if (flag13)
						{
							SoundManager.PlaySE(Mai2.Mai2Cue.Cue.JINGLE_MAP_GET_02, _monitorID);
							GameObject gameObject5 = _phase2.m._objListPrize[num22].gameObject;
							gameObject5.SetActive(value: true);
							_phase2.m._prize = gameObject5.GetComponent<Animator>();
							_phase2.m._prize.Play("Idle", 0, 0f);
							_phase2.m._prize.Play("In", 0, 0f);
						}
						break;
					case -1:
						break;
					}
					if (num21 != 3)
					{
						_state = MonitorState.Phase2RainbowReachedGetPrizeFadeInWait;
						_timer = _DispGetWindowTime;
					}
					else if (flag13)
					{
						_state = MonitorState.Phase2RainbowReachedGetPrizeFadeInWait;
						_timer = _DispGetWindowTime;
					}
					else
					{
						_state = MonitorState.Phase2RainbowReachedGetPrizeFadeOutWait;
						_timer = 0;
					}
					ResetSkipStart();
				}
				else
				{
					CheckRainbowSkipStart();
				}
				break;
			case MonitorState.Phase2RainbowReachedGetPrizeFadeInWait:
			{
				isSkipedTimerZero();
				bool flag10 = false;
				if ((_phase2.m._prizeWindowType != -1) ? (_phase2.m.IsEndAnim(_phase2.m._prize) || _timer == 0) : (_timer == 0))
				{
					if (_phase2.m._prize != null)
					{
						_phase2.m._prize.Play("Out", 0, 0f);
					}
					_state = MonitorState.Phase2RainbowReachedGetPrizeFadeOut;
					_timer = 30;
					ResetSkipStart();
				}
				else
				{
					CheckGetWindowSkipStart();
				}
				break;
			}
			case MonitorState.Phase2RainbowReachedGetPrizeFadeOut:
				isSkipedTimerZero();
				if (_timer == 0)
				{
					_state = MonitorState.Phase2RainbowReachedGetPrizeFadeOutWait;
					ResetSkipStart();
				}
				break;
			case MonitorState.Phase2RainbowReachedGetPrizeFadeOutWait:
				if (_phase2.m.IsEndAnim(_phase2.m._prize) || _timer == 0)
				{
					int prizeWindowType2 = _phase2.m._prizeWindowType;
					int num16 = _phase2.m.GetPrizeWindowType(_mapData, _listPinInfo, _phase2.m._rainbowID, otomodachi: false);
					if (num16 < 0)
					{
						num16 = 0;
					}
					if (prizeWindowType2 != -1)
					{
						_phase2.m._objListPrize[num16].gameObject.SetActive(value: false);
					}
					_phase2.m.SetActiveChildren(_phase2.m._objListPrize[num16], active: false);
					_phase2.m._objListPrize[num16].SetActive(value: false);
					_state = MonitorState.Phase2NextRainbow;
					num16 = _listPinInfo[_phase2.m._rainbowID].TreasureId.id;
					bool flag12 = false;
					if (Singleton<DataManager>.Instance.GetMapTreasureData(num16) != null && Singleton<DataManager>.Instance.GetMapTreasureData(num16).TreasureType == MapTreasureType.Otomodachi)
					{
						flag12 = true;
					}
					if (flag12)
					{
						_state = MonitorState.Phase2RainbowReachedOtomodachiFadeIn;
					}
					else if (_phase2.m._isReleasePinOver)
					{
						_state = MonitorState.Phase2MapReleaseFadeIn;
						ResetSkipStart();
					}
					_timer = 30;
					ResetSkipStart();
				}
				break;
			case MonitorState.Phase2NextRainbow:
				isSkipedTimerZero();
				if (_timer == 0)
				{
					_phase2.m._objNearPin.SetActive(value: false);
					_phase2.m._objNearPinGhost.SetActive(value: false);
					int num76 = _phase2.m._rainbowID + 1;
					if ((num76 < _phase2.m._rainbowEnd && num76 < _listPinInfo.Count) || (!_mapData.IsInfinity && num76 < _listPinInfo.Count))
					{
						if (_isEnableRainbow)
						{
							_phase2._travelGauge01.Play("Angle_45", 0, 0f);
							_phase2._travelGauge02.gameObject.SetActive(value: true);
							_phase2._travelGauge02.Play("Angle_180", 0, 0f);
						}
						float num77 = 0f;
						_phase2._imgRainbow02.fillAmount = num77;
						float z4 = 90f - 1.8f * num77;
						_phase2._objRainbowReached02.transform.localRotation = Quaternion.Euler(0f, 0f, z4);
						_phase2._objRainbowPin01.SetActive(value: false);
						_state = MonitorState.Phase2NextRainbowWait;
						_timer = 60;
					}
					else if (!_mapData.IsInfinity)
					{
						if (num76 >= _listPinInfo.Count)
						{
							_phase2.m._isEndPinClear = true;
							_state = MonitorState.Phase2MapClearFadeIn;
							if (_phase2.m._prizeWindowType != 3)
							{
								_state = MonitorState.Phase2MapClearFadeIn;
							}
							else
							{
								_state = MonitorState.Phase2MapClearCollectionFadeIn;
							}
							ResetSkipStart();
							_timer = 60;
						}
					}
					else
					{
						_state = MonitorState.Phase2NextRainbowWait;
					}
				}
				CheckRainbowSkipStart();
				break;
			case MonitorState.Phase2NextRainbowWait:
			{
				isSkipedTimerZero();
				bool flag14 = false;
				if (_isEnableRainbow)
				{
					flag14 = _phase2.m.IsEndAnim(_phase2._travelGauge02);
				}
				if (flag14 || _timer == 0)
				{
					_phase2._travelGauge02.gameObject.SetActive(value: false);
					if (_isEnableRainbow)
					{
						_phase2._travelGauge01.Play("Fade_In", 0, 1f);
					}
					float num30 = 0f;
					_phase2._imgRainbow01.fillAmount = num30;
					float z = 90f - 1.8f * num30;
					_phase2._objRainbowReached01.transform.localRotation = Quaternion.Euler(0f, 0f, z);
					_phase2._objRainbowPin01.SetActive(value: true);
					if (!_mapData.IsInfinity)
					{
						_phase2.m.SetRainbowPin(_assetManager, _listPinInfo, _phase2._objRainbowPin01, _phase2.m._rainbowID + 1);
					}
					else
					{
						int num31 = _phase2.m._rainbowID + 1;
						if (num31 < _phase2.m._rainbowEnd && num31 < _listPinInfo.Count)
						{
							_phase2.m.SetRainbowPin(_assetManager, _listPinInfo, _phase2._objRainbowPin01, _phase2.m._rainbowID + 1);
						}
					}
					_state = MonitorState.Phase2NextRainbowJudge;
				}
				CheckRainbowSkipStart();
				break;
			}
			case MonitorState.Phase2NextRainbowJudge:
				isSkipedTimerZero();
				if (_timer == 0)
				{
					int num65 = _phase2.m._rainbowID + 1;
					bool num66 = num65 >= _phase2.m._rainbowEnd || num65 >= _listPinInfo.Count;
					bool flag31 = false;
					flag31 = num66 && _mapData.IsInfinity;
					_phase2.m._rainbowID++;
					if (_phase2.m._rainbowID < _phase2.m._rainbowEnd && !flag31)
					{
						_state = MonitorState.Phase2AddRainbow;
						uint end3 = _phase2.m._toatalBonusCommonValue.end;
						_ = _phase2.m._valueListRainbow[_phase2.m._rainbowID];
						int num67 = _phase2.m._distanceBase[_phase2.m._rainbowID];
						CommonValueUint commonValueUint4 = _phase2.m._valueListRainbowUint[_phase2.m._rainbowID];
						uint end4 = commonValueUint4.end;
						uint start8 = commonValueUint4.start;
						uint num68 = end4 - start8;
						_phase2.m._toatalBonusCommonValue.start = end3;
						if (end3 > num68)
						{
							_phase2.m._toatalBonusCommonValue.end = end3 - num68;
						}
						else
						{
							_phase2.m._toatalBonusCommonValue.end = 0u;
						}
						_phase2.m._toatalBonusCommonValue.current = _phase2.m._toatalBonusCommonValue.start;
						_phase2.m._toatalBonusCommonValue.diff = _phase2.m._valueListRainbowUint[_phase2.m._rainbowID].diff;
						_phase2.m._toatalBonusCommonValue.sub = true;
						uint start9 = commonValueUint4.start;
						uint start10 = (uint)num67 - start9;
						_phase2.m._remainUntilPinCommonValue.start = start10;
						uint end5 = commonValueUint4.end;
						if ((uint)num67 > end5)
						{
							_phase2.m._remainUntilPinCommonValue.end = (uint)num67 - end5;
						}
						else
						{
							_phase2.m._remainUntilPinCommonValue.end = 0u;
						}
						_phase2.m._remainUntilPinCommonValue.current = _phase2.m._remainUntilPinCommonValue.start;
						if (!_mapData.IsInfinity)
						{
							_phase2.m._remainUntilPinCommonValue.diff = _phase2.m._valueListRainbowUint[_phase2.m._rainbowID].diff;
							_phase2.m._remainUntilPinCommonValue.sub = true;
						}
						else
						{
							_phase2.m._remainUntilPinCommonValue.diff = _phase2.m._valueListRainbowUint[_phase2.m._rainbowID].diff;
							_phase2.m._remainUntilPinCommonValue.sub = false;
						}
						if (_mapData.IsInfinity)
						{
							_phase2.m._remainUntilPinCommonValue.start = _phase2.m._distanceStart[_phase2.m._rainbowID] + start9;
							_phase2.m._remainUntilPinCommonValue.current = _phase2.m._remainUntilPinCommonValue.start;
							_phase2.m._remainUntilPinCommonValue.end = _phase2.m._distanceStart[_phase2.m._rainbowID] + end5;
						}
					}
					else if (!_mapData.IsInfinity)
					{
						_state = MonitorState.Phase2RainbowFadeOut;
						_phase2.m._objListTotalBonusMeter[0].SetNumber(0u);
						_phase2.m._objListTotalBonusMeter[1].SetNumber(0u);
						if (_phase2.m._isJustRainbow && _phase2.m._isJustNextRainbowID >= 0)
						{
							uint num69 = (uint)_phase2.m._distanceBase[_phase2.m._isJustNextRainbowID];
							_phase2.m._objListRestPinMeter[0].SetNumber(num69);
							_phase2.m._objListRestPinMeter[1].SetNumber(num69);
							_phase2.m._objListRestPinMeter[2].SetNumber(num69);
							_phase2.m._objListRestPinMeter[3].SetNumber(num69);
							if (_mapData.IsInfinity)
							{
								uint totalMapRunMeterValue4 = num69;
								_phase2.m._totalMapRunMeterValue = totalMapRunMeterValue4;
								_phase2.m._objListTotalMapRunMeter[0].SetNumber(_phase2.m._totalMapRunMeterValue);
							}
							else
							{
								int rainbowID = _phase2.m._rainbowID;
								uint num70 = _phase2.m._distanceStart[rainbowID];
								uint num71 = num69;
								uint num72 = 0u;
								if ((uint)_phase2.m._distanceBase[rainbowID] >= num71)
								{
									num72 = (uint)_phase2.m._distanceBase[rainbowID] - num71;
								}
								uint totalMapRunMeterValue5 = num70 + num72;
								_phase2.m._totalMapRunMeterValue = totalMapRunMeterValue5;
								_phase2.m._objListTotalMapRunMeter[0].SetNumber(_phase2.m._totalMapRunMeterValue);
							}
						}
					}
					else
					{
						_state = MonitorState.Phase2AddRainbow;
						uint end6 = _phase2.m._toatalBonusCommonValue.end;
						int num73 = 0;
						if (num65 >= _listPinInfo.Count - 1)
						{
							int num74 = _phase2.m._valueListRainbow.Count - 1;
							_ = _phase2.m._valueListRainbow[num74];
							_ = _phase2.m._distanceBase[num74];
							num73 = num74;
						}
						else
						{
							_ = _phase2.m._valueListRainbow[_phase2.m._rainbowID];
							_ = _phase2.m._distanceBase[_phase2.m._rainbowID];
							num73 = _phase2.m._rainbowID;
						}
						uint num75 = end6;
						_phase2.m._toatalBonusCommonValue.start = end6;
						if (end6 > num75)
						{
							_phase2.m._toatalBonusCommonValue.end = end6 - num75;
						}
						else
						{
							_phase2.m._toatalBonusCommonValue.end = 0u;
						}
						_phase2.m._toatalBonusCommonValue.current = _phase2.m._toatalBonusCommonValue.start;
						_phase2.m._toatalBonusCommonValue.diff = _phase2.m._valueListRainbowUint[num73].diff;
						_phase2.m._toatalBonusCommonValue.sub = true;
						_phase2.m._remainUntilPinCommonValue.start = _phase2.m._remainUntilPinCommonValue.end;
						_phase2.m._remainUntilPinCommonValue.current = _phase2.m._remainUntilPinCommonValue.start;
						_phase2.m._remainUntilPinCommonValue.end = _phase2.m._remainUntilPinCommonValue.start + end6;
						_phase2.m._remainUntilPinCommonValue.diff = _phase2.m._valueListRainbowUint[num73].diff;
						_phase2.m._remainUntilPinCommonValue.sub = false;
						if (_phase2.m._toatalBonusCommonValue.current == 0)
						{
							_state = MonitorState.Phase2RainbowFadeOut;
						}
					}
				}
				CheckRainbowSkipStart();
				break;
			case MonitorState.Phase2MapClearFadeIn:
				if (_timer == 0)
				{
					int index16 = 2;
					_phase2.m.SetMapClear(_mapID, _phase2.m._objListPrize[index16]);
					_phase2.m._objListPrize[index16].SetActive(value: true);
					_phase2.m.SetActiveChildren(_phase2.m._objListPrize[index16], active: true);
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_MAP_SELECT_GOAL_PICTURE, _monitorID);
					SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000148, _monitorID);
					GameObject gameObject9 = _phase2.m._objListPrize[index16].gameObject;
					gameObject9.SetActive(value: true);
					_phase2.m._prize = gameObject9.GetComponent<Animator>();
					int layerIndex14 = _phase2.m._prize.GetLayerIndex("Base Layer");
					_phase2.m._prize.SetLayerWeight(layerIndex14, 1f);
					_phase2.m._prize = gameObject9.GetComponent<Animator>();
					_phase2.m._prize.Play("In", 0, 0f);
					layerIndex14 = _phase2.m._prize.GetLayerIndex("Rotation");
					_phase2.m._prize.SetLayerWeight(layerIndex14, 1f);
					_phase2.m._prize.Play("Star_Rotation", layerIndex14, 0f);
					layerIndex14 = _phase2.m._prize.GetLayerIndex("Star");
					_phase2.m._prize.SetLayerWeight(layerIndex14, 1f);
					_phase2.m._prize.Play("Star", layerIndex14, 0f);
					_state = MonitorState.Phase2MapClearFadeInWait;
					_timer = _DispMapClearTime;
				}
				break;
			case MonitorState.Phase2MapClearFadeInWait:
				isSkipedTimerZero();
				if (_phase2.m.IsEndAnim(_phase2.m._prize) || _timer == 0)
				{
					_state = MonitorState.Phase2MapClearFadeOut;
					_timer = 30;
					ResetSkipStart();
				}
				else
				{
					CheckMapClearSkipStart();
				}
				if (_phase2.m._prize.GetCurrentAnimatorStateInfo(1).normalizedTime >= 1f)
				{
					int layerIndex11 = _phase2.m._prize.GetLayerIndex("Rotation");
					_phase2.m._prize.SetLayerWeight(layerIndex11, 1f);
					_phase2.m._prize.Play("Star_Rotation", layerIndex11, 0f);
				}
				if (_phase2.m._prize.GetCurrentAnimatorStateInfo(2).normalizedTime >= 1f)
				{
					int layerIndex12 = _phase2.m._prize.GetLayerIndex("Star");
					_phase2.m._prize.SetLayerWeight(layerIndex12, 1f);
					_phase2.m._prize.Play("Star", layerIndex12, 0f);
				}
				break;
			case MonitorState.Phase2MapClearFadeOut:
				if (_timer == 0)
				{
					_state = MonitorState.Phase2MapClearFadeOutWait;
					_timer = 60;
				}
				break;
			case MonitorState.Phase2MapClearFadeOutWait:
				if (_phase2.m.IsEndAnim(_phase2.m._prize) || _timer == 0)
				{
					int prizeWindowType5 = _phase2.m._prizeWindowType;
					int index14 = 2;
					if (prizeWindowType5 != -1)
					{
						_phase2.m._objListPrize[index14].SetActive(value: false);
					}
					_phase2.m.SetActiveChildren(_phase2.m._objListPrize[index14], active: false);
					_phase2.m._objListPrize[index14].SetActive(value: false);
					_state = MonitorState.Phase2RainbowFadeOut;
					_timer = 30;
				}
				break;
			case MonitorState.Phase2MapClearCollectionFadeIn:
				if (_timer == 0)
				{
					int index13 = 2;
					_phase2.m.SetMapClear(_mapID, _phase2.m._objListPrize[index13]);
					_phase2.m._objListPrize[index13].SetActive(value: true);
					_phase2.m.SetActiveChildren(_phase2.m._objListPrize[index13], active: true);
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_MAP_SELECT_GOAL_PICTURE, _monitorID);
					SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000148, _monitorID);
					GameObject gameObject8 = _phase2.m._objListPrize[index13].gameObject;
					gameObject8.SetActive(value: true);
					_phase2.m._prize = gameObject8.GetComponent<Animator>();
					int layerIndex8 = _phase2.m._prize.GetLayerIndex("Base Layer");
					_phase2.m._prize.SetLayerWeight(layerIndex8, 1f);
					_phase2.m._prize = gameObject8.GetComponent<Animator>();
					_phase2.m._prize.Play("In", 0, 0f);
					layerIndex8 = _phase2.m._prize.GetLayerIndex("Rotation");
					_phase2.m._prize.SetLayerWeight(layerIndex8, 1f);
					_phase2.m._prize.Play("Star_Rotation", layerIndex8, 0f);
					layerIndex8 = _phase2.m._prize.GetLayerIndex("Star");
					_phase2.m._prize.SetLayerWeight(layerIndex8, 1f);
					_phase2.m._prize.Play("Star", layerIndex8, 0f);
					_state = MonitorState.Phase2MapClearCollectionFadeInWait;
					_timer = _DispMapClearTime;
				}
				break;
			case MonitorState.Phase2MapClearCollectionFadeInWait:
				isSkipedTimerZero();
				if (_phase2.m.IsEndAnim(_phase2.m._prize) || _timer == 0)
				{
					_phase2.m._prize.Play("Out", 0, 0f);
					_state = MonitorState.Phase2MapClearCollectionGet;
					_timer = 30;
					ResetSkipStart();
				}
				else
				{
					CheckMapClearSkipStart();
				}
				if (_phase2.m._prize.GetCurrentAnimatorStateInfo(1).normalizedTime >= 1f)
				{
					int layerIndex5 = _phase2.m._prize.GetLayerIndex("Rotation");
					_phase2.m._prize.SetLayerWeight(layerIndex5, 1f);
					_phase2.m._prize.Play("Star_Rotation", layerIndex5, 0f);
				}
				if (_phase2.m._prize.GetCurrentAnimatorStateInfo(2).normalizedTime >= 1f)
				{
					int layerIndex6 = _phase2.m._prize.GetLayerIndex("Star");
					_phase2.m._prize.SetLayerWeight(layerIndex6, 1f);
					_phase2.m._prize.Play("Star", layerIndex6, 0f);
				}
				break;
			case MonitorState.Phase2MapClearCollectionGet:
				if (_timer == 0)
				{
					int index8 = 3;
					_phase2.m.SetPrize(_assetManager, _mapData, _listPinInfo, _phase2.m._objListPrize[index8], _phase2.m._rainbowID, otomodachi: false, -1);
					_phase2.m._objListPrize[index8].SetActive(value: true);
					_phase2.m.SetActiveChildren(_phase2.m._objListPrize[index8], active: true);
					GameObject gameObject4 = _phase2.m._objListPrize[index8].gameObject;
					gameObject4.SetActive(value: true);
					_phase2.m._prize = gameObject4.GetComponent<Animator>();
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.JINGLE_MAP_GET_02, _monitorID);
					_phase2.m._prize.Play("In", 0, 0f);
					_state = MonitorState.Phase2MapClearCollectionGetWait;
					_timer = _DispGetWindowTime;
				}
				break;
			case MonitorState.Phase2MapClearCollectionGetWait:
				isSkipedTimerZero();
				if (_phase2.m.IsEndAnim(_phase2.m._prize) || _timer == 0)
				{
					_state = MonitorState.Phase2MapClearCollectionFadeOut;
					ResetSkipStart();
				}
				else
				{
					CheckGetWindowSkipStart();
				}
				break;
			case MonitorState.Phase2MapClearCollectionFadeOut:
				if (_timer == 0)
				{
					_phase2.m._prize.Play("Out", 0, 0f);
					_state = MonitorState.Phase2MapClearCollectionFadeOutWait;
					_timer = 60;
				}
				break;
			case MonitorState.Phase2MapClearCollectionFadeOutWait:
				if (_phase2.m.IsEndAnim(_phase2.m._prize) || _timer == 0)
				{
					int prizeWindowType4 = _phase2.m._prizeWindowType;
					int index7 = 3;
					if (prizeWindowType4 != -1)
					{
						_phase2.m._objListPrize[index7].gameObject.SetActive(value: false);
					}
					_phase2.m.SetActiveChildren(_phase2.m._objListPrize[index7], active: false);
					_phase2.m._objListPrize[index7].SetActive(value: false);
					_state = MonitorState.Phase2RainbowFadeOut;
					_timer = 30;
				}
				break;
			case MonitorState.Phase2MapReleaseFadeIn:
				if (_timer == 0)
				{
					if (_phase2.m._releaseIslandID.Count > 0)
					{
						int index6 = 4;
						int mapID2 = _phase2.m._releaseIslandID[0];
						_phase2.m.SetMapRelease(mapID2, _phase2.m._objListPrize[index6], _mapID);
						_phase2.m._objListPrize[index6].SetActive(value: true);
						_phase2.m.SetActiveChildren(_phase2.m._objListPrize[index6], active: true);
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.JINGLE_MAP_GET, _monitorID);
						GameObject gameObject3 = _phase2.m._objListPrize[index6].gameObject;
						gameObject3.SetActive(value: true);
						int layerIndex3 = _phase2.m._prize.GetLayerIndex("Base Layer");
						_phase2.m._prize.SetLayerWeight(layerIndex3, 0.5f);
						_phase2.m._prize = gameObject3.GetComponent<Animator>();
						_phase2.m._prize.Play("Idle", 0, 0f);
						_phase2.m._prize.Play("In", 0, 0f);
						_phase2.m._objIsland.GetComponent<Animator>().Play("In", 0, 0f);
						_state = MonitorState.Phase2MapReleaseFadeInWait;
						_timer = _DispGetWindowTime;
					}
					else
					{
						_state = MonitorState.Phase2NextRainbow;
					}
				}
				break;
			case MonitorState.Phase2MapReleaseFadeInWait:
				isSkipedTimerZero();
				if (_phase2.m.IsEndAnim(_phase2.m._prize) || _timer == 0)
				{
					_state = MonitorState.Phase2MapReleaseFadeOut;
					ResetSkipStart();
				}
				else
				{
					CheckGetWindowSkipStart();
				}
				if (_phase2.m.IsEndAnim(_phase2.m._objIsland.GetComponent<Animator>()))
				{
					_phase2.m._objIsland.GetComponent<Animator>().Play("Loop", 0, 0f);
				}
				break;
			case MonitorState.Phase2MapReleaseFadeOut:
				if (_timer == 0)
				{
					_phase2.m._objIsland.GetComponent<Animator>().Play("Loop", 0, 1f);
					_phase2.m._prize.Play("Out", 0, 0f);
					_phase2.m._objIsland.GetComponent<Animator>().Play("Out", 0, 0f);
					_state = MonitorState.Phase2MapReleaseFadeOutWait;
					_timer = 60;
				}
				break;
			case MonitorState.Phase2MapReleaseFadeOutWait:
				if (_phase2.m.IsEndAnim(_phase2.m._prize) || _timer == 0)
				{
					int prizeWindowType = _phase2.m._prizeWindowType;
					int num11 = (num11 = 4);
					_phase2.m._prize.Play("Out", 0, 1f);
					_phase2.m._objIsland.GetComponent<Animator>().Play("Out", 0, 1f);
					if (prizeWindowType != -1)
					{
						_phase2.m._objListPrize[num11].gameObject.SetActive(value: false);
					}
					_phase2.m.SetActiveChildren(_phase2.m._objListPrize[num11], active: false);
					_phase2.m._objListPrize[num11].SetActive(value: false);
					_state = MonitorState.Phase2MapReleaseJudge;
					_timer = 30;
				}
				break;
			case MonitorState.Phase2MapReleaseJudge:
				if (_phase2.m._releaseIslandID.Count > 0)
				{
					_phase2.m._releaseIslandID.RemoveAt(0);
				}
				if (_phase2.m._releaseIslandID.Count > 0)
				{
					_state = MonitorState.Phase2MapReleaseFadeIn;
				}
				else
				{
					_state = MonitorState.Phase2NextRainbow;
				}
				break;
			case MonitorState.Phase2RainbowFadeOut:
			{
				ResetSkipStart();
				if (_timer != 0)
				{
					break;
				}
				int num62 = _phase2.m._rainbowID + 1;
				if (num62 < _phase2.m._rainbowEnd && num62 < _listPinInfo.Count && _isEnableRainbow)
				{
					_phase2._travelGauge01.Play("Fade_Out", 0, 0f);
				}
				if (!_phase2.m._isChallenge)
				{
					_phase2._totalBonusMeter.Play("Out", 0, 0f);
				}
				if (!_phase2.m._isChallenge)
				{
					if (_phase2.m._DerakkumaSmoke != null)
					{
						_phase2.m._DerakkumaSmoke.SetActive(value: false);
					}
					if (_isSetLoopSE1)
					{
						SoundManager.StopSE(_loopSE1);
						_isSetLoopSE1 = false;
					}
					if (_isCardPass && _isSetLoopSE2)
					{
						SoundManager.StopSE(_loopSE2);
						_isSetLoopSE2 = false;
					}
					Animator component7 = _phase2.m._Derakkuma2.GetComponent<Animator>();
					int layerIndex13 = component7.GetLayerIndex("Side");
					component7.SetLayerWeight(layerIndex13, 0f);
					component7.Play("Default", 0, 0f);
				}
				_state = MonitorState.Phase2RainbowFadeOutWait;
				break;
			}
			case MonitorState.Phase2RainbowFadeOutWait:
			{
				bool flag30 = false;
				if (_isEnableRainbow)
				{
					flag30 = _phase2.m.IsEndAnim(_phase2._travelGauge01);
				}
				if (flag30 || _timer == 0)
				{
					_phase2._travelGauge01.gameObject.SetActive(value: false);
					_phase2._totalBonusMeter.gameObject.SetActive(value: false);
					_state = MonitorState.Phase2TotalMapRunFadeIn;
					_timer = 0;
					_phase2.m._objNearPin.SetActive(value: false);
					_phase2.m._objNearPinGhost.SetActive(value: false);
				}
				break;
			}
			case MonitorState.Phase2TotalMapRunFadeIn:
				if (_timer == 0)
				{
					if (_phase2.m._isEndPinClear && _total >= _phase2.m.GetPrizeDistance(_listPinInfo, _listPinInfo.Count - 1))
					{
						_total = _phase2.m.GetPrizeDistance(_listPinInfo, _listPinInfo.Count - 1);
					}
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_MAP_RESULT_NEXT_INFO, _monitorID);
					_state = MonitorState.Phase2TotalMapRunFadeInWait;
				}
				break;
			case MonitorState.Phase2TotalMapRunFadeInWait:
				if (_timer == 0)
				{
					_state = MonitorState.Phase2TotalMapRunFadeInWaitJudge;
				}
				break;
			case MonitorState.Phase2TotalMapRunFadeInWaitJudge:
				if (_mapData.IsInfinity && _total >= 999999999)
				{
					_isDispInfoWindow1 = true;
					_isCallVoice = false;
					_isInfoWindowVoice = false;
					_info_state = InfoWindowState.None;
					_info_timer = 0u;
					_state = MonitorState.Phase2TotalMapRunFadeInWaitJudgeWait;
				}
				else
				{
					_state = MonitorState.Phase2TotalMapRunWait;
					_timer = _DispTotalMapRunTime;
				}
				break;
			case MonitorState.Phase2TotalMapRunFadeInWaitJudgeWait:
				if (!_isDispInfoWindow1)
				{
					_state = MonitorState.Phase2TotalMapRunWait;
					_timer = _DispTotalMapRunTime;
				}
				else if (_isInfoWindowVoice && !_isCallVoice)
				{
					_isCallVoice = true;
				}
				break;
			case MonitorState.Phase2TotalMapRunWait:
				if (_timer == 0)
				{
					_state = MonitorState.Phase2FadeOut;
					_timer = 0;
				}
				break;
			case MonitorState.Phase2FadeOut:
				if (_timer == 0)
				{
					if (_mapData.IsInfinity)
					{
						_phase2._ground.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).gameObject.SetActive(value: false);
						_phase2._restPinMeter.Play("Out", 0, 0f);
					}
					_state = MonitorState.Phase2FadeOutWait;
					ResetSkipStart();
				}
				break;
			case MonitorState.Phase2FadeOutWait:
				if (_timer != 0)
				{
					break;
				}
				_state = MonitorState.Phase3FadeOut;
				if (GameManager.IsFreedomMode)
				{
					if (_isNeedAwake || GameManager.IsFreedomTimeUp || GameManager.GetMaxTrackCount() >= 6)
					{
						if (Singleton<MapMaster>.Instance.IsCallAwake != null)
						{
							Singleton<MapMaster>.Instance.IsCallAwake[_monitorID] = true;
						}
						_state = MonitorState.Phase3FadeIn;
					}
				}
				else if (GameManager.IsFinalTrack(GameManager.MusicTrackNumber))
				{
					_state = MonitorState.Phase3FadeIn;
				}
				break;
			case MonitorState.Phase3FadeIn:
				if (_timer == 0)
				{
					_phase3._party.gameObject.SetActive(value: true);
					_state = MonitorState.Phase3FadeInWait;
				}
				break;
			case MonitorState.Phase3FadeInWait:
				if (_timer == 0)
				{
					_state = MonitorState.Phase3LevelUpInfoFadeIn;
				}
				break;
			case MonitorState.Phase3LevelUpInfoFadeIn:
				if (_timer == 0)
				{
					Animator component4 = _phase2.m._Derakkuma2.GetComponent<Animator>();
					int layerIndex4 = component4.GetLayerIndex("Result");
					component4.SetLayerWeight(layerIndex4, 0f);
					layerIndex4 = component4.GetLayerIndex("Side");
					component4.SetLayerWeight(layerIndex4, 0f);
					component4.Play("Dance", 0, 0f);
					_phase3.m._levelUpInfo.gameObject.SetActive(value: true);
					if (!_phase3.m._isPlatinumPass)
					{
						_phase3.m._levelUpInfo.Play("In", 0, 0f);
					}
					else
					{
						_phase3.m._levelUpInfo.Play("In", 0, 0f);
					}
					_state = MonitorState.Phase3LevelUpInfoFadeInWait;
					_timer = 60;
				}
				break;
			case MonitorState.Phase3LevelUpInfoFadeInWait:
				if (_phase3.m.IsEndAnim(_phase3.m._levelUpInfo) || _timer == 0)
				{
					_state = MonitorState.Phase3LevelUpInfoFadeOut;
				}
				break;
			case MonitorState.Phase3LevelUpInfoFadeOut:
				if (_timer == 0)
				{
					if (!_phase3.m._isPlatinumPass)
					{
						_phase3.m._levelUpInfo.Play("Out", 0, 0f);
					}
					else
					{
						_phase3.m._levelUpInfo.Play("Out", 0, 0f);
					}
					_state = MonitorState.Phase3LevelUpInfoFadeOutWait;
					_timer = 30;
				}
				break;
			case MonitorState.Phase3LevelUpInfoFadeOutWait:
				if (_phase3.m.IsEndAnim(_phase3.m._levelUpInfo) || _timer == 0)
				{
					_phase3.m._levelUpInfo.gameObject.SetActive(value: false);
					_state = MonitorState.Phase3AddAwake;
					_phase3.m._charaID = 0;
					for (int num15 = 0; num15 < _phase3.m._charaEnd; num15++)
					{
						_phase3.m._awakeInfo[num15]._sub_timer = 0;
						_phase3.m._awakeInfo[num15]._sub_state = Phase3CommonMember.AwakeInfo.MonitorSubState.SubAddAwake;
					}
				}
				break;
			case MonitorState.Phase3AddAwake:
				_state = MonitorState.Phase3AddAwakeWait;
				_phase3.m._isLevelUpVoice = false;
				_phase3.m._isLevelUpSE = false;
				break;
			case MonitorState.Phase3AddAwakeWait:
			{
				int num14 = 0;
				_phase3.m._isLevelUpSE = false;
				for (int n = 0; n < _phase3.m._charaEnd; n++)
				{
					if (_phase3.m._awakeInfo[n]._sub_timer > 0)
					{
						_phase3.m._awakeInfo[n]._sub_timer--;
					}
					switch (_phase3.m._awakeInfo[n]._sub_state)
					{
					case Phase3CommonMember.AwakeInfo.MonitorSubState.SubAddAwake:
						if (_phase3.m._awakeInfo[n]._sub_timer == 0)
						{
							_phase3.m._awakeInfo[n]._sub_state = Phase3CommonMember.AwakeInfo.MonitorSubState.SubAddAwakeWait;
							_phase3.m._awakeInfo[n]._sub_timer = 120;
						}
						break;
					case Phase3CommonMember.AwakeInfo.MonitorSubState.SubAddAwakeWait:
					{
						bool flag8 = false;
						int index5 = n;
						CommonValue commonValue = _phase3.m._awakeInfo[index5]._valueListLevel[_phase3.m._awakeInfo[index5]._awakeID];
						bool isBlock = _phase3.m._awakeInfo[index5]._isBlock;
						Animator component2 = _phase3.m._objListChara[index5].transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.GetComponent<Animator>();
						if (!isBlock)
						{
							flag8 = commonValue.UpdateValue();
							_phase3.m._awakeInfo[index5]._isEnd = flag8;
							Image component3 = _phase3.m._objListChara[index5].transform.GetChild(0).GetChild(4).gameObject.transform.GetChild(1).gameObject.transform.GetChild(_phase3.m.GetAwakeIconIndex(0, isBigIcon: false)).gameObject.transform.GetChild(2).transform.gameObject.GetComponent<Image>();
							if (!_phase3.m._awakeInfo[index5]._isStayAwakeCenter)
							{
								component3.fillAmount = commonValue.current / 100f;
							}
							if (!_phase3.m._awakeInfo[index5]._isAwakeOverCurrent)
							{
								_phase3.m._awakeInfo[index5]._currentLevel = _phase3.m._awakeInfo[index5]._totalLevel;
								_phase3.m._objListLevel[index5].SetNumber(_phase3.m._awakeInfo[index5]._currentLevel);
								if (!_phase3.m._isPlatinumPass)
								{
									component2.Play("LevelUP", 0, 0f);
								}
								else
								{
									component2.Play("LevelUP", 0, 0f);
								}
								if (!_phase3.m._isLevelUpSE)
								{
									SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_CHARA_RESULT_LVUP, _monitorID);
									_phase3.m._isLevelUpSE = true;
								}
								if (!_phase3.m._isLevelUpVoice)
								{
									SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000149, _monitorID);
									_phase3.m._isLevelUpVoice = true;
								}
								_phase3.m._awakeInfo[index5]._isBlock = true;
								_phase3.m._awakeInfo[n]._sub_timer = 90;
							}
							else
							{
								_phase3.m._awakeInfo[index5]._isBlock = true;
								_phase3.m._awakeInfo[n]._sub_timer = 0;
							}
							if (_isSkip)
							{
								_phase3.m._awakeInfo[index5]._sub_timer = 0;
								_phase3.m._awakeInfo[index5]._isBlock = false;
								_phase3.m._awakeInfo[index5]._awakeID = _phase3.m._awakeInfo[index5]._awakeEnd;
								_phase3.m._awakeInfo[index5]._sub_state = Phase3CommonMember.AwakeInfo.MonitorSubState.SubNextAwakeJudge;
							}
						}
						else
						{
							if (!_phase3.m.IsEndAnim(component2) && _phase3.m._awakeInfo[n]._sub_timer != 0)
							{
								break;
							}
							if (_phase3.m._awakeInfo[index5]._isAwakeOverCurrent)
							{
								flag8 = true;
							}
							else
							{
								flag8 = _phase3.m._awakeInfo[index5]._isEnd;
								if (_phase3.m._awakeInfo[index5]._currentLevel >= _phase3.m._awakeInfo[index5]._totalLevel)
								{
									flag8 = true;
								}
							}
							_phase3.m._awakeInfo[index5]._isBlock = false;
							if (!flag8 && !_isSkip)
							{
								break;
							}
							if (commonValue.end >= 100f || _phase3.m._awakeInfo[index5]._isAwakeOverCurrent || _isSkip)
							{
								_phase3.m._awakeInfo[index5]._isStayAwakeCenter = true;
								_phase3.m._awakeInfo[index5]._sub_state = Phase3CommonMember.AwakeInfo.MonitorSubState.SubNextAwakeJudge;
								_phase3.m._awakeInfo[index5]._sub_timer = 60;
								if (_isSkip)
								{
									_phase3.m._awakeInfo[index5]._sub_timer = 0;
									_phase3.m._awakeInfo[index5]._isBlock = false;
									_phase3.m._awakeInfo[index5]._awakeID = _phase3.m._awakeInfo[index5]._awakeEnd;
								}
							}
							else
							{
								_phase3.m._awakeInfo[index5]._sub_state = Phase3CommonMember.AwakeInfo.MonitorSubState.SubNextAwakeJudge;
							}
							_phase3.m._awakeInfo[index5]._sub_timer = 30;
						}
						break;
					}
					case Phase3CommonMember.AwakeInfo.MonitorSubState.SubNextAwakeJudge:
					{
						int index4 = n;
						_phase3.m._awakeInfo[index4]._awakeID++;
						if (_phase3.m._awakeInfo[index4]._awakeID < _phase3.m._awakeInfo[index4]._awakeEnd)
						{
							if (_phase3.m._awakeInfo[index4]._currentLevel >= _phase3.m._awakeInfo[index4]._totalLevel)
							{
								num14++;
							}
							else
							{
								_phase3.m._awakeInfo[index4]._sub_state = Phase3CommonMember.AwakeInfo.MonitorSubState.SubAddAwake;
							}
						}
						else
						{
							num14++;
						}
						if (num14 >= _phase3.m._charaEnd)
						{
							_state = MonitorState.Phase3AwakeInfo;
							_phase3.m._charaID = 0;
						}
						break;
					}
					}
				}
				break;
			}
			case MonitorState.Phase3AwakeReached:
				if (_timer == 0)
				{
					_state = MonitorState.Phase3AwakeReachedWait;
				}
				break;
			case MonitorState.Phase3AwakeReachedWait:
				_state = MonitorState.Phase3NextAwake;
				break;
			case MonitorState.Phase3NextAwake:
				_state = MonitorState.Phase3NextAwakeWait;
				break;
			case MonitorState.Phase3NextAwakeWait:
				_state = MonitorState.Phase3NextAwakeJudge;
				break;
			case MonitorState.Phase3NextAwakeJudge:
			{
				if (_timer != 0)
				{
					break;
				}
				int charaID = _phase3.m._charaID;
				_phase3.m._awakeInfo[charaID]._awakeID++;
				if (_phase3.m._awakeInfo[charaID]._awakeID < _phase3.m._awakeInfo[charaID]._awakeEnd)
				{
					_state = MonitorState.Phase3AddAwake;
					break;
				}
				_phase3.m._charaID++;
				if (_phase3.m._charaID < _phase3.m._charaEnd)
				{
					_state = MonitorState.Phase3AddAwake;
					break;
				}
				_state = MonitorState.Phase3AwakeInfo;
				_phase3.m._charaID = 0;
				break;
			}
			case MonitorState.Phase3AwakeInfo:
			{
				if (_timer != 0)
				{
					break;
				}
				int num10 = -1;
				for (int k = _phase3.m._charaID; k < _phase3.m._charaEnd; k++)
				{
					if (_phase3.m._awakeInfo[k]._awakeCount > 0)
					{
						num10 = k;
						break;
					}
				}
				if (num10 >= 0)
				{
					_state = MonitorState.Phase3AwakeInfoFadeIn;
					_phase3.m._charaID = num10;
				}
				else
				{
					_state = MonitorState.Phase3PreFadeOutJudge;
				}
				break;
			}
			case MonitorState.Phase3AwakeInfoFadeIn:
			{
				if (_timer != 0)
				{
					break;
				}
				int num78 = 0;
				int charaID3 = _phase3.m._charaID;
				_phase3.m._charaDistance.SetNumber((uint)_phase3.m._awakeInfo[charaID3]._bonusMax);
				Image image3 = null;
				Sprite sprite3 = null;
				num78 = _phase3.m.GetAwakeIconIndex(0, isBigIcon: true);
				image3 = _phase2.m._animAwakeStar[num78].gameObject.transform.GetChild(2).gameObject.GetComponent<Image>();
				sprite3 = Resources.Load<Sprite>("Common/Sprites/CharacterColor/UI_CMN_Chara_Star_Big");
				if (sprite3 != null)
				{
					image3.sprite = UnityEngine.Object.Instantiate(sprite3);
				}
				image3 = _phase2.m._animAwakeStarEffect[num78].gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
				sprite3 = Resources.Load<Sprite>("Common/Sprites/CharacterColor/UI_CMN_Chara_Star_Small");
				if (sprite3 != null)
				{
					image3.sprite = UnityEngine.Object.Instantiate(sprite3);
				}
				for (int num79 = 1; num79 < 5; num79++)
				{
					if (_phase3.m.isExistAwakeIconIndex(num79, isBigIcon: true))
					{
						num78 = _phase3.m.GetAwakeIconIndex(num79, isBigIcon: true);
						image3 = _phase2.m._animAwakeStar[num78].gameObject.transform.GetChild(1).gameObject.GetComponent<Image>();
						sprite3 = Resources.Load<Sprite>("Common/Sprites/CharacterColor/UI_CMN_Chara_Star_Small");
						if (sprite3 != null)
						{
							image3.sprite = UnityEngine.Object.Instantiate(sprite3);
						}
						image3 = _phase2.m._animAwakeStarEffect[num78].gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
						sprite3 = Resources.Load<Sprite>("Common/Sprites/CharacterColor/UI_CMN_Chara_Star_Small");
						if (sprite3 != null)
						{
							image3.sprite = UnityEngine.Object.Instantiate(sprite3);
						}
					}
				}
				num78 = 0;
				charaID3 = _phase3.m._charaID;
				_phase3.m.SetAwakeInfo(_assetManager, _phase2.m._objListPrize[num78], _charaInfo, _phase3.m._charaID, isAwakeEnd: false);
				_phase2.m._objListPrize[num78].SetActive(value: true);
				_phase2.m.SetActiveChildren(_phase2.m._objListPrize[num78], active: true);
				_phase2.m._objListPrize[num78].transform.GetChild(5).gameObject.SetActive(value: false);
				if (_phase3.m._charaCount == 5 && !_phase3.m._isAwakeInfo)
				{
					_phase3.m._isAwakeInfo = true;
				}
				GameObject gameObject10 = _phase2.m._objListPrize[num78].gameObject;
				gameObject10.SetActive(value: true);
				_phase2.m._prize = gameObject10.GetComponent<Animator>();
				_phase2.m._prize.Play("Idle", 0, 0f);
				_phase2.m._prize.Play("In", 0, 0f);
				_phase2.m._animAwakeStarRoot[0].Play("In", 0, 0f);
				SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000150, _monitorID);
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.JINGLE_MAP_GET, _monitorID);
				_state = MonitorState.Phase3AwakeInfoFadeInWait;
				_timer = _DispAwakeTime;
				ResetSkipStart();
				_sub_count = 0;
				_isBlockingSE1 = false;
				_isBlockingSE2 = false;
				_isBlockingSE3 = false;
				break;
			}
			case MonitorState.Phase3AwakeInfoFadeInWait:
				isSkipedTimerZero();
				_sub_count++;
				if (_sub_count >= 30 && !_isBlockingSE1)
				{
					_phase2.m._animAwakeStarSpark[0].gameObject.SetActive(value: true);
					_phase2.m._animAwakeStarSpark[0].Play("SpartkStar_Eff", 0, 0f);
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_CHARA_RESULT_GAUGE_TWINKLE, _monitorID);
					_isBlockingSE1 = true;
				}
				if (_sub_count >= 60 && !_isBlockingSE2)
				{
					_phase2.m._animAwakeStarSpark[0].gameObject.SetActive(value: false);
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_CHARA_RESULT_SET_OHAJIKI, _monitorID);
					_isBlockingSE2 = true;
					Phase3CommonMember.AwakeInfo awakeInfo = _phase3.m._awakeInfo[_phase3.m._charaID];
					int awakeStart = awakeInfo._awakeStart;
					int awakeEnd = awakeInfo._awakeEnd;
					int num26 = 0;
					awakeEnd--;
					if (awakeEnd < 0)
					{
						awakeEnd = 0;
					}
					if (awakeInfo._valueListLevel[awakeEnd].end >= 100f)
					{
						for (int num27 = awakeStart + 1; num27 <= awakeEnd + 1; num27++)
						{
							if (_phase3.m.isExistAwakeIconIndex(num27, isBigIcon: true))
							{
								num26 = _phase3.m.GetAwakeIconIndex(num27, isBigIcon: true);
								_phase2.m._animAwakeStar[num26].gameObject.SetActive(value: true);
								_phase2.m._animAwakeStar[num26].Play("GetStar", 0, 0f);
								_phase2.m._animAwakeStarEffect[num26].gameObject.SetActive(value: true);
								_phase2.m._animAwakeStarEffect[num26].Play("GetStar_Eff", 0, 0f);
							}
						}
					}
					else
					{
						for (int num28 = awakeStart + 1; num28 < awakeEnd + 1; num28++)
						{
							if (_phase3.m.isExistAwakeIconIndex(num28, isBigIcon: true))
							{
								num26 = _phase3.m.GetAwakeIconIndex(num28, isBigIcon: true);
								_phase2.m._animAwakeStar[num26].gameObject.SetActive(value: true);
								_phase2.m._animAwakeStar[num26].Play("GetStar", 0, 0f);
								_phase2.m._animAwakeStarEffect[num26].gameObject.SetActive(value: true);
								_phase2.m._animAwakeStarEffect[num26].Play("GetStar_Eff", 0, 0f);
							}
						}
					}
					if (awakeInfo._isAwakeOverPervVerTotal || awakeInfo._isAwakeOverTotal)
					{
						Image image2 = null;
						Sprite sprite2 = null;
						num26 = _phase3.m.GetAwakeIconIndex(0, isBigIcon: true);
						_phase2.m._animAwakeStar[num26].gameObject.SetActive(value: true);
						if (awakeInfo._isAwakeOverTotal)
						{
							image2 = _phase2.m._animAwakeStar[num26].gameObject.transform.GetChild(2).gameObject.GetComponent<Image>();
							sprite2 = Resources.Load<Sprite>("Common/Sprites/CharacterColor/UI_CMN_Chara_star_big_MAX");
							if (sprite2 != null)
							{
								image2.sprite = UnityEngine.Object.Instantiate(sprite2);
							}
						}
						_phase2.m._animAwakeStar[num26].Play("GetStar", 0, 0f);
						_phase2.m._animAwakeStarEffect[num26].gameObject.SetActive(value: true);
						if (awakeInfo._isAwakeOverTotal)
						{
							image2 = _phase2.m._animAwakeStarEffect[num26].gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
							sprite2 = Resources.Load<Sprite>("Common/Sprites/CharacterColor/UI_CMN_Chara_star_small_MAX");
							if (sprite2 != null)
							{
								image2.sprite = UnityEngine.Object.Instantiate(sprite2);
							}
						}
						_phase2.m._animAwakeStarEffect[num26].Play("GetStar_Eff", 0, 0f);
						if (awakeInfo._isAwakeOverTotal)
						{
							for (int num29 = 1; num29 < 5; num29++)
							{
								if (_phase3.m.isExistAwakeIconIndex(num29, isBigIcon: true))
								{
									num26 = _phase3.m.GetAwakeIconIndex(num29, isBigIcon: true);
									_phase2.m._animAwakeStar[num26].gameObject.SetActive(value: true);
									image2 = _phase2.m._animAwakeStar[num26].gameObject.transform.GetChild(1).gameObject.GetComponent<Image>();
									sprite2 = Resources.Load<Sprite>("Common/Sprites/CharacterColor/UI_CMN_Chara_star_small_MAX");
									if (sprite2 != null)
									{
										image2.sprite = UnityEngine.Object.Instantiate(sprite2);
									}
									_phase2.m._animAwakeStar[num26].Play("GetStar", 0, 0f);
									_phase2.m._animAwakeStarEffect[num26].gameObject.SetActive(value: true);
									image2 = _phase2.m._animAwakeStarEffect[num26].gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
									sprite2 = Resources.Load<Sprite>("Common/Sprites/CharacterColor/UI_CMN_Chara_star_small_MAX");
									if (sprite2 != null)
									{
										image2.sprite = UnityEngine.Object.Instantiate(sprite2);
									}
									_phase2.m._animAwakeStarEffect[num26].Play("GetStar_Eff", 0, 0f);
								}
							}
						}
					}
				}
				if (_sub_count >= 120 && !_isBlockingSE3)
				{
					int index11 = 0;
					_phase3.m.SetAwakeInfo(_assetManager, _phase2.m._objListPrize[index11], _charaInfo, _phase3.m._charaID, isAwakeEnd: true);
					_isBlockingSE3 = true;
					int charaID2 = _phase3.m._charaID;
					_phase3.m.UpdateCharacterAwakeInfo(_phase3.m._objListChara[charaID2], _charaInfo, _phase3.m._charaID);
					_phase3.m._objListLevel[charaID2].SetNumber(_phase3.m._awakeInfo[charaID2]._totalLevel);
				}
				if (_timer == 0)
				{
					if (_isBlockingSE1 && _isBlockingSE2 && _isBlockingSE3)
					{
						_timer = 30;
						_sub_count = 0;
						_state = MonitorState.Phase3AwakeInfoFadeOut;
						ResetSkipStart();
					}
					else if (!_isBlockingSE1)
					{
						_sub_count = 30;
					}
					else if (!_isBlockingSE2)
					{
						_sub_count = 60;
					}
					else if (!_isBlockingSE3)
					{
						_sub_count = 120;
					}
				}
				else
				{
					CheckAwakeSkipStart();
				}
				break;
			case MonitorState.Phase3AwakeInfoFadeOut:
				if (_timer == 0)
				{
					_state = MonitorState.Phase3AwakeInfoFadeOutWait;
				}
				break;
			case MonitorState.Phase3AwakeInfoFadeOutWait:
				if (_phase2.m.IsEndAnim(_phase2.m._prize) || _timer == 0)
				{
					int index9 = 0;
					_phase2.m.SetActiveChildren(_phase2.m._objListPrize[index9], active: false);
					_phase2.m._objListPrize[index9].SetActive(value: false);
					_state = MonitorState.Phase3AwakeInfoJudge;
					_timer = 60;
				}
				break;
			case MonitorState.Phase3AwakeInfoJudge:
				if (_timer == 0)
				{
					_phase3.m._charaID++;
					if (_phase3.m._charaID < _phase3.m._charaEnd)
					{
						_state = MonitorState.Phase3AwakeInfo;
						break;
					}
					_state = MonitorState.Phase3PreFadeOutJudge;
					_phase3.m._charaID = 0;
				}
				break;
			case MonitorState.Phase3PreFadeOutJudge:
				if (_phase3.m._isAwakeInfo)
				{
					if (!Singleton<UserDataManager>.Instance.GetUserData(_monitorID).Detail.ContentBit.IsFlagOn(ContentBitID.FirstCharaAwake))
					{
						_isDispInfoWindow2 = true;
						_isCallVoice = false;
						_isInfoWindowVoice = false;
						_info_state = InfoWindowState.None;
						_info_timer = 0u;
						_state = MonitorState.Phase3PreFadeOutJudgeWait;
					}
					else
					{
						_state = MonitorState.Phase3PreFadeOut;
					}
				}
				else
				{
					_state = MonitorState.Phase3PreFadeOut;
				}
				break;
			case MonitorState.Phase3PreFadeOutJudgeWait:
				if (!_isDispInfoWindow2)
				{
					_state = MonitorState.Phase3PreFadeOut;
				}
				else if (_isInfoWindowVoice && !_isCallVoice)
				{
					_isCallVoice = true;
					SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000228, _monitorID);
				}
				break;
			case MonitorState.Phase3PreFadeOut:
				_phase3.m._charaID = 0;
				_state = MonitorState.Phase3FadeOut;
				_timer = 180;
				SkipStart();
				break;
			case MonitorState.Phase3FadeOut:
			{
				isSkipedTimerZero();
				if (_timer != 0)
				{
					break;
				}
				ResetSkipStart();
				_mapID = _userData.Detail.SelectMapID;
				int num = _userData.MapList.FindIndex((UserMapData m) => m.ID == _mapID);
				if (num < 0)
				{
					_userData.MapList.Add(_userMap);
					num = _userData.MapList.Count - 1;
				}
				if (_total >= _phase2.m._limitDistanceMax)
				{
					_total = _phase2.m._limitDistanceMax;
				}
				_userData.MapList[num].Distance = _total;
				bool flag = false;
				if (!_mapData.IsInfinity)
				{
					if (_phase2.m._isReleasePinOver)
					{
						_userData.MapList[num].IsClear = true;
						flag = true;
					}
					if (_phase2.m._isEndPinClear)
					{
						_userData.MapList[num].IsComplete = true;
						flag = true;
					}
				}
				if (flag)
				{
					num = Singleton<MapMaster>.Instance.RefUserMapList[_monitorID].FindIndex((UserMapData m) => m.ID == _mapID);
					if (num >= 0)
					{
						Singleton<MapMaster>.Instance.RefUserMapList[_monitorID][num].Distance = _total;
					}
				}
				if (_phase2.m._isEndPinClear)
				{
					_userData.Activity.MapComplete(_mapID);
				}
				for (int i = _phase2.m._rainbowStart; i < _phase2.m._rainbowEnd; i++)
				{
					if (i >= _listPinInfo.Count || !_phase2.m.IsGetPrize(_listPinInfo, i, _total))
					{
						continue;
					}
					num = _phase2.m.GetConvertedTreasureID(_mapData, _listPinInfo, i, otomodachi: true);
					MapTreasureData mapTreasureData = Singleton<DataManager>.Instance.GetMapTreasureData(num);
					_ = _phase2.m._nextOtomodachiNothing;
					if (_phase2.m._nextOtomodachiNothing)
					{
						_ = _listPinInfo.Count - 1;
					}
					if (mapTreasureData != null)
					{
						switch (mapTreasureData.TreasureType)
						{
						case MapTreasureType.Character:
							_userData.AddCollections(UserData.Collection.Chara, mapTreasureData.CharacterId.id);
							break;
						case MapTreasureType.MusicNew:
							_userData.AddUnlockMusic(UserData.MusicUnlock.Base, mapTreasureData.MusicId.id);
							break;
						case MapTreasureType.NamePlate:
							_userData.AddCollections(UserData.Collection.Plate, mapTreasureData.NamePlate.id, addNewGet: true);
							break;
						case MapTreasureType.Frame:
							_userData.AddCollections(UserData.Collection.Frame, mapTreasureData.Frame.id, addNewGet: true);
							break;
						}
					}
				}
				bool flag2 = false;
				int[] charaSlot = _userData.Detail.CharaSlot;
				int num2 = 1;
				int num3 = 1;
				float ticketLevelUpRate = _ticketLevelUpRate;
				_ = _phase3.m._isPlatinumPass;
				if (_cardPassType == Table.GoldPass)
				{
					num3 = 2;
				}
				if (GameManager.IsFreedomMode)
				{
					if (Singleton<MapMaster>.Instance.IsCallAwake != null && Singleton<MapMaster>.Instance.IsCallAwake[_monitorID])
					{
						num2 = (int)GameManager.GetMaxTrackCount();
						if (num2 >= 6)
						{
							num2 = 6;
						}
						flag2 = true;
					}
				}
				else if (GameManager.IsFinalTrack(GameManager.MusicTrackNumber))
				{
					num2 = (int)GameManager.GetMaxTrackCount();
					flag2 = true;
				}
				else
				{
					num2 = 1;
				}
				num2 *= num3;
				if (ticketLevelUpRate > 1f)
				{
					float num4 = (float)num2 * ticketLevelUpRate;
					int num5 = (int)num4;
					float num6 = num4 - (float)num5;
					num2 = num5;
					if (num6 >= 0.1f)
					{
						num2++;
					}
				}
				if (flag2)
				{
					for (int j = 0; j < charaSlot.Length; j++)
					{
						if (charaSlot[j] <= 0)
						{
							continue;
						}
						CharaData data2 = Singleton<DataManager>.Instance.GetChara(charaSlot[j]);
						UserChara userChara = _userData.CharaList.Find((UserChara a2) => a2.ID == data2.GetID());
						int num7 = (int)userChara.Level;
						if (num7 >= _phase3.m._awakeLevelMax)
						{
							num7 = _phase3.m._awakeLevelMax;
						}
						int num8 = 0;
						num8 = num7 + num2;
						if (num8 >= _phase3.m._awakeLevelMax)
						{
							num8 = _phase3.m._awakeLevelMax;
						}
						userChara.Level = (uint)num8;
						num = _userData.CharaList.FindIndex((UserChara a2) => a2.ID == data2.GetID());
						if (num >= 0)
						{
							_userData.CharaList[num].Level = (uint)num8;
							_userData.CharaList[num].CalcLevelToAwake();
							if (!_phase3.m._awakeInfo[j]._isAwakeOverCurrent && _phase3.m._awakeInfo[j]._isAwakeOverTotal)
							{
								_userData.Activity.AwakeChara(num);
							}
						}
					}
					if (Singleton<MapMaster>.Instance.IsCallAwake != null)
					{
						Singleton<MapMaster>.Instance.IsCallAwake[_monitorID] = true;
					}
				}
				_state = MonitorState.Phase3FadeOutWait;
				break;
			}
			case MonitorState.Phase3FadeOutWait:
				if (_timer == 0)
				{
					_phase2.m._Derakkuma2.SetActive(value: false);
					_phase3._party.gameObject.SetActive(value: false);
					_state = MonitorState.Finish;
				}
				break;
			case MonitorState.Finish:
				if (_timer == 0)
				{
					_state = MonitorState.End;
					int siblingIndex = _buttonController.gameObject.transform.GetSiblingIndex();
					_buttonController.gameObject.transform.SetSiblingIndex(siblingIndex - 1);
				}
				break;
			}
		}

		public bool IsEnd()
		{
			return _state == MonitorState.End;
		}

		public bool IsReleaseNewMap()
		{
			return _phase2.m._isEndPinClear;
		}

		public bool IsGetNewCharacter()
		{
			return _phase2.m._isGetNewCharacter;
		}
	}
}
