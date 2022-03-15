using System.Collections.Generic;
using System.Linq;
using MAI2.Util;
using Manager;
using Manager.MaiStudio;
using Manager.MaiStudio.CardTypeName;
using Manager.UserDatas;
using Monitor.MapResult.Common;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.MapResult
{
	public class Phase3CommonMember : CommonBase
	{
		public class AwakeInfo
		{
			public enum MonitorSubState
			{
				SubNone,
				SubAddAwake,
				SubAddAwakeWait,
				SubAwakeReached,
				SubAwakeReachedWait,
				SubNextAwake,
				SubNextAwakeWait,
				SubNextAwakeJudge,
				SubFinish,
				SubEnd
			}

			public List<CommonValue> _valueListLevel;

			public int _awakeID;

			public int _awakeStart;

			public int _awakeEnd;

			public bool _isBlock;

			public bool _isEnd;

			public int _awakeCount;

			public int _currentLevel;

			public int _totalLevel;

			public int _bonusMax;

			public bool _isAwakeOverCurrent;

			public bool _isAwakeOverTotal;

			public bool _isAwakeOverPervVerCurrent;

			public bool _isAwakeOverPervVerTotal;

			public bool _isStayAwakeCenter;

			public MonitorSubState _sub_state;

			public int _sub_timer;

			public AwakeInfo()
			{
				_valueListLevel = new List<CommonValue>();
				_awakeID = 0;
				_awakeStart = 0;
				_awakeEnd = 0;
				_isBlock = false;
				_isEnd = false;
				_awakeCount = 0;
				_currentLevel = 0;
				_totalLevel = 0;
				_bonusMax = 0;
				_isAwakeOverCurrent = false;
				_isAwakeOverTotal = false;
				_isAwakeOverPervVerCurrent = false;
				_isAwakeOverPervVerTotal = false;
				_isStayAwakeCenter = false;
				_sub_state = MonitorSubState.SubNone;
				_sub_timer = 0;
			}
		}

		private GameObject _prefabChara;

		private List<GameObject> _nullListChara = new List<GameObject>();

		public List<GameObject> _objListChara = new List<GameObject>();

		public List<CharaLevelNumber> _objListLevel = new List<CharaLevelNumber>();

		private GameObject _prefabLevelUpInfo;

		private GameObject _objLevelUpInfo;

		public Animator _levelUpInfo;

		private int[] awake_icon_index = new int[5] { 4, 1, 3, 0, 2 };

		private int[] awake_big_icon_index = new int[5] { 4, 1, 2, 0, 3 };

		private int[] awake_big_icon_index2 = new int[5] { 4, 1, 3, 0, 2 };

		public List<AwakeInfo> _awakeInfo = new List<AwakeInfo>();

		public int _charaID;

		public int _charaEnd;

		public OdometerNumber _charaDistance;

		public int _awakeLevelMax = 9999;

		public int _awakeLevelNormalMax = 999;

		public bool _isLevelUpVoice;

		public bool _isLevelUpSE;

		public bool _isPlatinumPass;

		public bool _isAwakeInfo;

		public uint _charaCount;

		public void SetAwakeInfo(AssetManager manager, GameObject awake, List<UserChara> info, int chara_id, bool isAwakeEnd)
		{
			UserChara userChara = info[chara_id];
			if (!isAwakeEnd)
			{
				for (int i = 0; i < awake.transform.childCount; i++)
				{
					awake.transform.GetChild(i).gameObject.SetActive(value: false);
				}
				awake.gameObject.SetActive(value: true);
			}
			GameObject gameObject = awake.gameObject;
			Image image = null;
			Texture2D texture2D = null;
			GameObject gameObject2 = null;
			Sprite sprite = null;
			int num = 0;
			int num2 = 0;
			string text = null;
			int num3 = 0;
			text = Singleton<DataManager>.Instance.GetChara(userChara.ID).genre.id.ToString("000000");
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
			gameObject2 = gameObject.transform.GetChild(3).gameObject;
			MultiImage component = gameObject2.transform.GetChild(1).gameObject.GetComponent<MultiImage>();
			num3 = userChara.ID;
			texture2D = manager.GetCharacterTexture2D(num3);
			sprite = (component.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero));
			MultiImage component2 = gameObject2.transform.GetChild(0).gameObject.GetComponent<MultiImage>();
			component2.Image2 = sprite;
			num = Singleton<DataManager>.Instance.GetChara(userChara.ID).color.id;
			Color24 colorDark = Singleton<DataManager>.Instance.GetMapColorData(num).ColorDark;
			Color color2 = (component2.color = new Color((float)(int)colorDark.R / 255f, (float)(int)colorDark.G / 255f, (float)(int)colorDark.B / 255f));
			if (!isAwakeEnd)
			{
				gameObject2.transform.GetChild(0).gameObject.SetActive(value: false);
				gameObject2.transform.GetChild(0).gameObject.SetActive(value: true);
			}
			GameObject gameObject3 = null;
			int num4 = 0;
			Image image2 = null;
			gameObject2 = gameObject.transform.GetChild(6).gameObject;
			gameObject = gameObject2.transform.GetChild(0).gameObject;
			gameObject3 = gameObject;
			num4 = GetAwakeIconIndex(0, isBigIcon: true) + 1;
			gameObject2 = gameObject.transform.GetChild(num4).gameObject;
			gameObject2.SetActive(value: true);
			if (!isAwakeEnd)
			{
				num2 = Singleton<DataManager>.Instance.GetChara(userChara.ID).genre.id;
				Image image3 = null;
				for (int j = 1; j < 6; j++)
				{
					gameObject = gameObject3.transform.GetChild(j).gameObject;
					gameObject2 = gameObject.transform.GetChild(0).gameObject;
					image3 = gameObject2.GetComponent<Image>();
					sprite = ((j != 5) ? AssetManager.Instance().GetMapBgSprite(num2, "UI_Chara_Star_S") : AssetManager.Instance().GetMapBgSprite(num2, "UI_Chara_Star"));
					if (sprite != null)
					{
						image3.sprite = Object.Instantiate(sprite);
					}
				}
			}
			gameObject = gameObject3;
			AwakeInfo awakeInfo = _awakeInfo[chara_id];
			if (!isAwakeEnd)
			{
				for (int k = 1; k < gameObject.transform.childCount - 2; k++)
				{
					num4 = GetAwakeIconIndex(k, isBigIcon: true) + 1;
					gameObject2 = gameObject.transform.GetChild(num4).gameObject;
					gameObject2.SetActive(value: false);
				}
				for (int l = 1; l <= awakeInfo._awakeStart; l++)
				{
					num4 = GetAwakeIconIndex(l, isBigIcon: true) + 1;
					gameObject2 = gameObject.transform.GetChild(num4).gameObject;
					gameObject2.SetActive(value: true);
				}
				if (awakeInfo._valueListLevel[awakeInfo._awakeStart].end >= 100f)
				{
					num4 = GetAwakeIconIndex(0, isBigIcon: true) + 1;
					gameObject2 = gameObject.transform.GetChild(num4).gameObject;
					gameObject2.SetActive(value: true);
					gameObject3 = gameObject2.transform.GetChild(1).gameObject;
					gameObject3.SetActive(value: true);
					image2 = gameObject3.transform.GetChild(1).gameObject.GetComponent<Image>();
					image2.fillAmount = 1f;
					gameObject3 = gameObject2.transform.GetChild(2).gameObject;
					gameObject3.SetActive(value: false);
				}
				return;
			}
			int awakeEnd = awakeInfo._awakeEnd;
			awakeEnd--;
			if (awakeEnd < 0)
			{
				awakeEnd = 0;
			}
			if (awakeInfo._valueListLevel[awakeEnd].end >= 100f)
			{
				for (int m = 0; m <= awakeEnd + 1; m++)
				{
					num4 = GetAwakeIconIndex(m, isBigIcon: true) + 1;
					gameObject2 = gameObject.transform.GetChild(num4).gameObject;
					gameObject2.SetActive(value: true);
				}
			}
			else
			{
				for (int n = 0; n < awakeEnd + 1; n++)
				{
					num4 = GetAwakeIconIndex(n, isBigIcon: true) + 1;
					gameObject2 = gameObject.transform.GetChild(num4).gameObject;
					gameObject2.SetActive(value: true);
				}
			}
			num4 = GetAwakeIconIndex(0, isBigIcon: true) + 1;
			gameObject2 = gameObject.transform.GetChild(num4).gameObject;
			gameObject2.SetActive(value: true);
			gameObject3 = gameObject2.transform.GetChild(1).gameObject;
			gameObject3.SetActive(value: true);
			image2 = gameObject3.transform.GetChild(1).gameObject.GetComponent<Image>();
			if (awakeInfo._valueListLevel[awakeEnd].end >= 100f)
			{
				image2.fillAmount = 0f;
			}
			else
			{
				image2.fillAmount = awakeInfo._valueListLevel[awakeEnd].end / 100f;
			}
			gameObject3 = gameObject2.transform.GetChild(2).gameObject;
			bool flag = false;
			if (!isAwakeEnd)
			{
				if (awakeInfo._isAwakeOverPervVerCurrent || awakeInfo._isAwakeOverCurrent)
				{
					flag = true;
				}
			}
			else if (awakeInfo._isAwakeOverPervVerTotal || awakeInfo._isAwakeOverTotal)
			{
				flag = true;
			}
			if (flag)
			{
				gameObject3.SetActive(value: true);
			}
			else
			{
				gameObject3.SetActive(value: false);
			}
		}

		public void SetCharacterAwakeInfoOverTotal(GameObject img_star)
		{
			GameObject gameObject = null;
			int num = 0;
			Image image = null;
			Sprite sprite = null;
			num = GetAwakeIconIndex(0, isBigIcon: false);
			img_star.SetActive(value: true);
			image = img_star.GetComponent<Image>();
			sprite = Resources.Load<Sprite>("Common/Sprites/CharacterColor/UI_CMN_Chara_star_big_MAX");
			if (sprite != null)
			{
				image.sprite = Object.Instantiate(sprite);
			}
			gameObject = img_star.transform.parent.parent.gameObject;
			for (int i = 1; i < 5; i++)
			{
				num = GetAwakeIconIndex(i, isBigIcon: false);
				image = gameObject.transform.GetChild(num).gameObject.transform.GetChild(1).gameObject.GetComponent<Image>();
				sprite = Resources.Load<Sprite>("Common/Sprites/CharacterColor/UI_CMN_Chara_star_small_MAX");
				if (sprite != null)
				{
					image.sprite = Object.Instantiate(sprite);
				}
			}
		}

		public void UpdateCharacterAwakeInfo(GameObject awake, List<UserChara> info, int chara_id)
		{
			GameObject gameObject = null;
			_ = info[chara_id];
			gameObject = awake.transform.GetChild(0).gameObject;
			gameObject = gameObject.transform.GetChild(4).gameObject.transform.GetChild(1).gameObject;
			GameObject gameObject2 = null;
			int num = 0;
			AwakeInfo awakeInfo = _awakeInfo[chara_id];
			int awakeEnd = awakeInfo._awakeEnd;
			awakeEnd--;
			if (awakeEnd < 0)
			{
				awakeEnd = 0;
			}
			if (awakeInfo._valueListLevel[awakeEnd].end >= 100f)
			{
				for (int i = 0; i <= awakeEnd + 1; i++)
				{
					num = GetAwakeIconIndex(i, isBigIcon: false);
					gameObject.transform.GetChild(num).gameObject.SetActive(value: true);
				}
			}
			else
			{
				for (int j = 0; j < awakeEnd + 1; j++)
				{
					num = GetAwakeIconIndex(j, isBigIcon: false);
					gameObject.transform.GetChild(num).gameObject.SetActive(value: true);
				}
			}
			num = GetAwakeIconIndex(0, isBigIcon: false);
			GameObject gameObject3 = gameObject.transform.GetChild(num).gameObject;
			gameObject3.SetActive(value: true);
			gameObject2 = gameObject3.transform.GetChild(2).gameObject;
			gameObject2.SetActive(value: true);
			Image component = gameObject2.GetComponent<Image>();
			if (awakeInfo._valueListLevel[awakeEnd].end >= 100f)
			{
				component.fillAmount = 0f;
			}
			else
			{
				component.fillAmount = awakeInfo._valueListLevel[awakeEnd].end / 100f;
			}
			gameObject2 = gameObject3.transform.GetChild(3).gameObject;
			if (awakeInfo._isAwakeOverPervVerTotal || awakeInfo._isAwakeOverTotal)
			{
				gameObject2.SetActive(value: true);
				if (awakeInfo._isAwakeOverTotal)
				{
					SetCharacterAwakeInfoOverTotal(gameObject2);
				}
			}
			else
			{
				gameObject2.SetActive(value: false);
			}
		}

		public bool isExistAwakeIconIndex(int i, bool isBigIcon)
		{
			bool result = true;
			int num = 0;
			num = ((!isBigIcon) ? (awake_icon_index.Count() - 1) : (awake_big_icon_index.Count() - 1));
			if (i > num)
			{
				result = false;
			}
			if (num < 0)
			{
				result = false;
			}
			return result;
		}

		public int GetAwakeIconIndex(int i, bool isBigIcon)
		{
			int num = 0;
			num = ((!isBigIcon) ? (awake_icon_index.Count() - 1) : (awake_big_icon_index.Count() - 1));
			int num2 = i;
			if (i >= num)
			{
				num2 = num;
			}
			if (num <= 0)
			{
				num2 = 0;
			}
			if (isBigIcon)
			{
				return awake_big_icon_index[num2];
			}
			return awake_icon_index[num2];
		}

		public int GetAwakeIconIndex2(int i, bool isBigIcon)
		{
			int num = 0;
			num = ((!isBigIcon) ? (awake_icon_index.Count() - 1) : (awake_big_icon_index2.Count() - 1));
			int num2 = i;
			if (i >= num)
			{
				num2 = num;
			}
			if (num <= 0)
			{
				num2 = 0;
			}
			if (isBigIcon)
			{
				return awake_big_icon_index2[num2];
			}
			return awake_icon_index[num2];
		}

		public void Initialize(AssetManager manager, GameObject obj, List<UserChara> info, Table card_type, float ticket_rate)
		{
			GameObject gameObject = null;
			GameObject gameObject2 = null;
			GameObject gameObject3 = null;
			_prefabChara = Resources.Load<GameObject>("Process/CharacterSelect/Prefabs/UI_Character");
			for (int i = 0; i < obj.transform.childCount; i++)
			{
				gameObject = obj.transform.GetChild(i).gameObject;
				gameObject2 = gameObject.transform.GetChild(0).gameObject;
				gameObject3 = gameObject2.transform.GetChild(1).gameObject;
				GameObject gameObject4 = new GameObject("EmptyChara");
				gameObject4.transform.SetParent(gameObject3.transform);
				gameObject4.transform.localPosition = Vector3.zero;
				gameObject4.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
				gameObject4.transform.localScale = Vector3.one;
				_nullListChara.Add(gameObject4);
				_objListChara.Add(Object.Instantiate(_prefabChara));
				_objListChara[i].transform.SetParent(_nullListChara[i].transform);
				_objListChara[i].transform.localPosition = _prefabChara.transform.localPosition;
				_objListChara[i].transform.localRotation = _prefabChara.transform.localRotation;
				_objListChara[i].transform.localScale = _prefabChara.transform.localScale;
				_ = _objListChara[i].transform.childCount;
				_objListChara[i].SetActive(value: false);
			}
			for (int j = 0; j < info.Count; j++)
			{
				Texture2D texture2D = null;
				Sprite sprite = null;
				Image image = null;
				int iD = info[j].ID;
				int num = 0;
				int id = Singleton<DataManager>.Instance.GetChara(info[j].ID).genre.id;
				gameObject = _objListChara[j].transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
				image = gameObject.GetComponent<Image>();
				sprite = AssetManager.Instance().GetMapBgSprite(id, "UI_Chara_Base_S");
				if (sprite != null)
				{
					image.sprite = Object.Instantiate(sprite);
				}
				gameObject = _objListChara[j].transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;
				gameObject2 = gameObject.transform.GetChild(0).gameObject;
				MultiImage component = gameObject2.GetComponent<MultiImage>();
				texture2D = manager.GetCharacterTexture2D(iD);
				sprite = (component.Image2 = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero));
				num = Singleton<DataManager>.Instance.GetChara(info[j].ID).color.id;
				Color24 colorDark = Singleton<DataManager>.Instance.GetMapColorData(num).ColorDark;
				Color color2 = (component.color = new Color((float)(int)colorDark.R / 255f, (float)(int)colorDark.G / 255f, (float)(int)colorDark.B / 255f));
				gameObject2 = gameObject.transform.GetChild(1).gameObject;
				MultiImage component2 = gameObject2.GetComponent<MultiImage>();
				texture2D = manager.GetCharacterTexture2D(iD);
				sprite = (component2.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero));
				gameObject2.gameObject.SetActive(value: false);
				gameObject2.gameObject.SetActive(value: true);
				gameObject2 = gameObject.transform.GetChild(2).gameObject;
				gameObject2.GetComponent<MultiImage>().sprite = sprite;
				gameObject2.gameObject.SetActive(value: false);
				gameObject2 = gameObject.transform.GetChild(3).gameObject;
				gameObject2.gameObject.SetActive(value: false);
				_objListChara[j].SetActive(value: true);
				gameObject = _objListChara[j].transform.GetChild(0).gameObject.transform.GetChild(2).gameObject;
				image = gameObject.GetComponent<Image>();
				sprite = AssetManager.Instance().GetMapBgSprite(id, "UI_Chara_Frame_S");
				if (sprite != null)
				{
					image.sprite = Object.Instantiate(sprite);
				}
				gameObject = _objListChara[j].transform.GetChild(0).GetChild(3).gameObject;
				if (j == 0)
				{
					gameObject.SetActive(value: true);
				}
				else
				{
					gameObject.SetActive(value: false);
				}
				gameObject = _objListChara[j].transform.GetChild(0).GetChild(4).gameObject;
				gameObject2 = gameObject.transform.GetChild(0).gameObject;
				image = gameObject2.GetComponent<Image>();
				sprite = AssetManager.Instance().GetMapBgSprite(id, "UI_Chara_Level_S");
				if (sprite != null)
				{
					image.sprite = Object.Instantiate(sprite);
				}
				gameObject3 = gameObject2.transform.GetChild(0).gameObject;
				CharaLevelNumber item = new CharaLevelNumber(gameObject3);
				_objListLevel.Add(item);
				gameObject3 = gameObject.transform.GetChild(1).gameObject;
				for (int k = 0; k < 5; k++)
				{
					gameObject = gameObject3.transform.GetChild(k).gameObject;
					gameObject2 = gameObject.transform.GetChild(0).gameObject;
					image = gameObject2.GetComponent<Image>();
					sprite = AssetManager.Instance().GetMapBgSprite(id, "UI_Chara_Star_S");
					if (sprite != null)
					{
						image.sprite = Object.Instantiate(sprite);
					}
				}
			}
			int num2 = 1;
			int num3 = 1;
			_isPlatinumPass = false;
			if (card_type == Table.PlatinumPass)
			{
				_isPlatinumPass = true;
			}
			if (card_type == Table.GoldPass)
			{
				num3 = 2;
			}
			if (GameManager.IsFreedomMode)
			{
				num2 = (int)GameManager.GetMaxTrackCount();
				if (num2 >= 6)
				{
					num2 = 6;
				}
			}
			else if (GameManager.IsFinalTrack(GameManager.MusicTrackNumber))
			{
				num2 = (int)GameManager.GetMaxTrackCount();
			}
			num2 *= num3;
			if (ticket_rate > 1f)
			{
				float num4 = (float)num2 * ticket_rate;
				int num5 = (int)num4;
				float num6 = num4 - (float)num5;
				num2 = num5;
				if (num6 >= 0.1f)
				{
					num2++;
				}
			}
			_charaCount = (uint)info.Count;
			for (int l = 0; l < info.Count; l++)
			{
				_objListLevel[l].SetNumber((int)info[l].Level);
			}
			for (int m = 0; m < info.Count; m++)
			{
				AwakeInfo item2 = new AwakeInfo();
				_awakeInfo.Add(item2);
				AwakeInfo awakeInfo = _awakeInfo[m];
				UserChara userChara = info[m];
				UserChara userChara2 = new UserChara(info[m].ID, info[m].Count, info[m].Level);
				Singleton<DataManager>.Instance.GetChara(userChara.ID);
				List<CharaAwakeData> list = (from x in Singleton<DataManager>.Instance.GetCharaAwakes()
					select x.Value).ToList();
				int num7 = (int)userChara.Level;
				if (num7 >= _awakeLevelMax)
				{
					awakeInfo._isAwakeOverCurrent = true;
					num7 = _awakeLevelMax;
				}
				if (num7 >= _awakeLevelNormalMax)
				{
					awakeInfo._isAwakeOverPervVerCurrent = true;
				}
				awakeInfo._currentLevel = num7;
				int num8 = 0;
				num8 = num7 + num2;
				if (num8 >= _awakeLevelMax)
				{
					awakeInfo._isAwakeOverTotal = true;
					num8 = _awakeLevelMax;
				}
				if (num8 >= _awakeLevelNormalMax)
				{
					awakeInfo._isAwakeOverPervVerTotal = true;
				}
				awakeInfo._totalLevel = num8;
				userChara2.AddLevel((uint)(awakeInfo._totalLevel - awakeInfo._currentLevel));
				awakeInfo._bonusMax = (int)userChara2.GetMovementParam(matchColor: true);
				awakeInfo._bonusMax *= 1000;
				int num9 = 0;
				foreach (var item5 in list.Select((CharaAwakeData value, int index) => new { value, index }))
				{
					int index2 = item5.index;
					if (num7 >= item5.value.awakeLevel)
					{
						num9 = item5.index + 1;
					}
					CommonValue item3 = new CommonValue();
					awakeInfo._valueListLevel.Add(item3);
					awakeInfo._valueListLevel[index2].start = 0f;
					awakeInfo._valueListLevel[index2].end = 100f;
					int num10 = 0;
					int num11 = 0;
					if (index2 > 0)
					{
						num10 = list[index2 - 1].awakeLevel;
					}
					num11 = list[index2].awakeLevel;
					if ((float)(num11 - num10) != 0f)
					{
						awakeInfo._valueListLevel[index2].diff = 100f / (float)(num11 - num10);
					}
				}
				int count = awakeInfo._valueListLevel.Count;
				CommonValue item4 = new CommonValue();
				awakeInfo._valueListLevel.Add(item4);
				awakeInfo._valueListLevel[count].start = 100f;
				awakeInfo._valueListLevel[count].end = 100f;
				awakeInfo._valueListLevel[count].diff = 0f;
				awakeInfo._awakeEnd = 0;
				foreach (var item6 in list.Select((CharaAwakeData value, int index) => new { value, index }))
				{
					int index3 = item6.index;
					if (num8 > item6.value.awakeLevel)
					{
						awakeInfo._awakeEnd = index3 + 1;
					}
				}
				if (awakeInfo._awakeEnd >= list.Count)
				{
					awakeInfo._isAwakeOverTotal = true;
					awakeInfo._awakeEnd = list.Count - 1;
					if (awakeInfo._awakeEnd < 0)
					{
						awakeInfo._awakeEnd = 0;
					}
				}
				int awakeEnd = awakeInfo._awakeEnd;
				int num12 = 0;
				int num13 = 0;
				if (list.Count > 0)
				{
					if (awakeEnd > 0)
					{
						num12 = list[awakeEnd - 1].awakeLevel;
					}
					num13 = (awakeInfo._isAwakeOverTotal ? list[list.Count - 1].awakeLevel : list[awakeEnd].awakeLevel);
				}
				if ((float)(num13 - num12) != 0f)
				{
					awakeInfo._valueListLevel[awakeEnd].end = (float)(num8 - num12) / (float)(num13 - num12) * 100f;
				}
				else
				{
					awakeInfo._valueListLevel[awakeEnd].end = 100f;
				}
				if (awakeInfo._valueListLevel[awakeEnd].end >= 100f)
				{
					awakeInfo._valueListLevel[awakeEnd].end = 100f;
				}
				awakeInfo._awakeEnd++;
				awakeInfo._awakeStart = num9;
				int num14 = num9;
				int num15 = 0;
				int num16 = 0;
				if (list.Count > 0)
				{
					if (num14 > 0)
					{
						num15 = list[num14 - 1].awakeLevel;
					}
					if (num14 >= list.Count)
					{
						awakeInfo._isAwakeOverCurrent = true;
					}
					num16 = (awakeInfo._isAwakeOverCurrent ? list[list.Count - 1].awakeLevel : list[num14].awakeLevel);
				}
				if ((float)(num16 - num15) != 0f)
				{
					awakeInfo._valueListLevel[num14].start = (float)(num7 - num15) / (float)(num16 - num15) * 100f;
				}
				else
				{
					awakeInfo._valueListLevel[num14].start = 100f;
				}
				if (awakeInfo._valueListLevel[num14].start >= 100f)
				{
					awakeInfo._valueListLevel[num14].start = 100f;
				}
				for (int n = 0; n < awakeInfo._valueListLevel.Count; n++)
				{
					awakeInfo._valueListLevel[n].current = awakeInfo._valueListLevel[n].start;
				}
				awakeInfo._awakeID = awakeInfo._awakeStart;
			}
			for (int num17 = 0; num17 < info.Count; num17++)
			{
				AwakeInfo awakeInfo2 = _awakeInfo[num17];
				_ = info[num17];
				gameObject = _objListChara[num17].transform.GetChild(0).GetChild(4).gameObject;
				gameObject2 = gameObject.transform.GetChild(1).gameObject;
				for (int num18 = 0; num18 < gameObject2.transform.childCount; num18++)
				{
					gameObject2.transform.GetChild(num18).gameObject.SetActive(value: false);
				}
				float start = awakeInfo2._valueListLevel[awakeInfo2._awakeStart].start;
				int awakeIconIndex = GetAwakeIconIndex(0, isBigIcon: false);
				gameObject2.transform.GetChild(awakeIconIndex).gameObject.transform.GetChild(2).transform.gameObject.GetComponent<Image>().fillAmount = start / 100f;
				if (awakeInfo2._isAwakeOverCurrent)
				{
					gameObject2.transform.GetChild(awakeIconIndex).gameObject.transform.GetChild(3).transform.gameObject.SetActive(value: true);
				}
				else
				{
					gameObject2.transform.GetChild(awakeIconIndex).gameObject.transform.GetChild(3).transform.gameObject.SetActive(value: false);
				}
				gameObject2.transform.GetChild(awakeIconIndex).gameObject.SetActive(value: true);
				for (int num19 = 1; num19 < awakeInfo2._awakeStart + 1; num19++)
				{
					awakeIconIndex = GetAwakeIconIndex(num19, isBigIcon: false);
					gameObject2.transform.GetChild(awakeIconIndex).gameObject.SetActive(value: true);
				}
				awakeIconIndex = GetAwakeIconIndex(0, isBigIcon: false);
				gameObject3 = gameObject2.transform.GetChild(awakeIconIndex).GetChild(3).gameObject;
				if (awakeInfo2._isAwakeOverPervVerCurrent || awakeInfo2._isAwakeOverCurrent)
				{
					gameObject3.SetActive(value: true);
					if (awakeInfo2._isAwakeOverCurrent)
					{
						SetCharacterAwakeInfoOverTotal(gameObject3);
					}
				}
				else
				{
					gameObject3.SetActive(value: false);
				}
			}
			_charaID = 0;
			_charaEnd = info.Count;
			for (int num20 = _charaID; num20 < _charaEnd; num20++)
			{
				int awakeStart = _awakeInfo[num20]._awakeStart;
				int awakeEnd2 = _awakeInfo[num20]._awakeEnd;
				for (int num21 = awakeStart; num21 < awakeEnd2; num21++)
				{
					if (_awakeInfo[num20]._valueListLevel[num21].end >= 100f)
					{
						_awakeInfo[num20]._awakeCount++;
					}
				}
			}
			_isAwakeInfo = false;
		}

		public void InitializeLevelUpInfo(GameObject obj)
		{
			_prefabLevelUpInfo = Resources.Load<GameObject>("Process/MapResult/Prefabs/UI_LevelUPWindow");
			GameObject gameObject = new GameObject("Empty");
			gameObject.transform.SetParent(obj.transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			gameObject.transform.localScale = Vector3.one;
			_objLevelUpInfo = Object.Instantiate(_prefabLevelUpInfo);
			_objLevelUpInfo.transform.SetParent(gameObject.transform);
			_objLevelUpInfo.transform.localPosition = _prefabLevelUpInfo.transform.localPosition;
			_objLevelUpInfo.transform.localRotation = _prefabLevelUpInfo.transform.localRotation;
			_objLevelUpInfo.transform.localScale = _prefabLevelUpInfo.transform.localScale;
			_levelUpInfo = _objLevelUpInfo.GetComponent<Animator>();
			_levelUpInfo.gameObject.SetActive(value: false);
		}

		public void InitializeAmakeCharaDistance(GameObject obj)
		{
			GameObject gameObject = obj.transform.GetChild(5).GetChild(1).gameObject;
			_charaDistance = new OdometerNumber(gameObject.transform.GetChild(0).gameObject, another: true, 300);
			gameObject.transform.parent.gameObject.SetActive(value: false);
		}
	}
}
