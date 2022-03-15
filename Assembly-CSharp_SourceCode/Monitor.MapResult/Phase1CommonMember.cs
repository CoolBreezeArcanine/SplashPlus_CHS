using System.Collections.Generic;
using System.Linq;
using MAI2.Util;
using Manager;
using Manager.MaiStudio;
using Monitor.MapResult.Common;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.MapResult
{
	public class Phase1CommonMember : CommonBase
	{
		private GameObject _prefabPins;

		private List<GameObject> _nullListPin = new List<GameObject>();

		private List<GameObject> _objListPin = new List<GameObject>();

		public float _angle;

		public GameObject _Derakkuma1;

		public uint _nextOtomodachiDistance;

		public int _nextOtomodachiInsertID;

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

		public void Initialize(AssetManager manager, List<MapTreasureExData> list, int mapID, GameObject masterNullPin, uint current)
		{
			_angle = 0f;
			int count = list.Count;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			List<MapTreasureType> list2 = new List<MapTreasureType>();
			int num4 = 0;
			foreach (MapTreasureExData item in list)
			{
				num2 = item.TreasureId.id;
				if (_nextOtomodachiInsertID != -1 && num4 == _nextOtomodachiInsertID)
				{
					list2.Add(MapTreasureType.Otomodachi);
				}
				else if (Singleton<DataManager>.Instance.GetMapTreasureData(num2) == null)
				{
					list2.Add(MapTreasureType.Otomodachi);
				}
				else
				{
					list2.Add(Singleton<DataManager>.Instance.GetMapTreasureData(num2).TreasureType);
				}
				num4++;
			}
			num = list2.Where((MapTreasureType t) => t == MapTreasureType.Otomodachi).Count();
			num++;
			int num5 = 0;
			foreach (var item2 in list.Select((MapTreasureExData value, int index) => new { value, index }))
			{
				int index2 = item2.index;
				uint distance = (uint)item2.value.Distance;
				bool flag = true;
				distance = GetArrangedDistance(distance, index2);
				flag = distance != 0;
				if (list2[index2] != MapTreasureType.Otomodachi && flag)
				{
					if (current == 0 && distance == 0)
					{
						num5 = 0;
					}
					else if (current >= distance)
					{
						num5++;
					}
				}
			}
			num3 = num5;
			_prefabPins = Resources.Load<GameObject>("Process/MapResult/Prefabs/Pin_Parts/UI_Pin_Landmark_All");
			count = list.Count + 1;
			count -= num;
			_angle = 360f / (float)(count + 2);
			float num6 = _angle * (float)num3 + _angle * 0.1f;
			int num7 = 0;
			int index3 = 0;
			num5 = 0;
			SetEmptyWithPrefab(masterNullPin.transform, _nullListPin, _objListPin, _prefabPins);
			_nullListPin[num5].transform.localRotation = Quaternion.Euler(0f, 0f, (float)(num5 * -1) * _angle + num6);
			num7 = 0;
			_objListPin[num5].transform.GetChild(num7).gameObject.SetActive(value: true);
			if (num7 == 0)
			{
				GameObject gameObject = _objListPin[num5].transform.GetChild(num7).gameObject;
				Image image = null;
				image = gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
				Sprite mapBgSprite = AssetManager.Instance().GetMapBgSprite(mapID, "UI_Landmark_Start");
				if (mapBgSprite != null)
				{
					image.sprite = Object.Instantiate(mapBgSprite);
				}
			}
			num5++;
			foreach (var item3 in list.Select((MapTreasureExData value, int index) => new { value, index }))
			{
				bool flag2 = false;
				int index4 = item3.index;
				uint distance2 = (uint)item3.value.Distance;
				bool flag3 = true;
				distance2 = GetArrangedDistance(distance2, index4);
				flag3 = distance2 != 0;
				if (list2[index4] == MapTreasureType.Otomodachi || !flag3)
				{
					continue;
				}
				SetEmptyWithPrefab(masterNullPin.transform, _nullListPin, _objListPin, _prefabPins);
				_nullListPin[num5].transform.localRotation = Quaternion.Euler(0f, 0f, (float)(num5 * -1) * _angle + num6);
				MapTreasureFlag flag4 = list[index4].Flag;
				if (flag4 == MapTreasureFlag.End)
				{
					num7 = 4;
				}
				else
				{
					num7 = 2;
					flag2 = true;
				}
				_objListPin[num5].transform.GetChild(num7).gameObject.SetActive(value: true);
				if (num7 == 4)
				{
					GameObject gameObject2 = _objListPin[num5].transform.GetChild(num7).gameObject;
					Image image2 = null;
					image2 = gameObject2.transform.GetChild(1).gameObject.GetComponent<Image>();
					Sprite mapBgSprite2 = AssetManager.Instance().GetMapBgSprite(mapID, "UI_Landmark_Goal");
					if (mapBgSprite2 != null)
					{
						image2.sprite = Object.Instantiate(mapBgSprite2);
					}
					flag2 = true;
				}
				if (flag2)
				{
					switch (list2[index4])
					{
					case MapTreasureType.Character:
						index3 = 1;
						break;
					case MapTreasureType.MusicNew:
						index3 = 0;
						break;
					case MapTreasureType.NamePlate:
					case MapTreasureType.Frame:
						index3 = 2;
						break;
					}
					GameObject gameObject3 = null;
					gameObject3 = ((num7 != 4) ? _objListPin[num5].transform.GetChild(num7).gameObject : _objListPin[num5].transform.GetChild(num7).gameObject.transform.GetChild(0).gameObject);
					for (int i = 0; i < gameObject3.transform.childCount; i++)
					{
						gameObject3.transform.GetChild(i).gameObject.SetActive(value: false);
					}
					gameObject3.transform.GetChild(index3).gameObject.SetActive(value: true);
					num2 = list[index4].TreasureId.id;
					MapTreasureData mapTreasureData = Singleton<DataManager>.Instance.GetMapTreasureData(num2);
					Texture2D texture2D = null;
					int num8 = 0;
					switch (list2[index4])
					{
					case MapTreasureType.Character:
					{
						Image component2 = gameObject3.transform.GetChild(index3).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
						texture2D = manager.GetCharacterTexture2D(mapTreasureData.CharacterId.id);
						component2.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
						num8 = ((num3 <= num5 - 1) ? 1 : 0);
						gameObject3.gameObject.GetComponent<MultipleImage>().ChangeSprite(num8);
						break;
					}
					case MapTreasureType.MusicNew:
					{
						Image component = gameObject3.transform.GetChild(index3).gameObject.GetComponent<Image>();
						texture2D = manager.GetJacketTexture2D(mapTreasureData.MusicId.id);
						component.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
						num8 = ((num3 > num5 - 1) ? 2 : 3);
						gameObject3.gameObject.GetComponent<MultipleImage>().ChangeSprite(num8);
						break;
					}
					case MapTreasureType.NamePlate:
					case MapTreasureType.Frame:
						num8 = ((num3 > num5 - 1) ? 4 : 5);
						gameObject3.gameObject.GetComponent<MultipleImage>().ChangeSprite(num8);
						break;
					}
				}
				if (num3 == num5 - 1)
				{
					num7 = 3;
					GameObject gameObject4 = _objListPin[num5].transform.GetChild(num7).gameObject;
					gameObject4.SetActive(value: true);
					GameObject gameObject5 = gameObject4.transform.GetChild(0).gameObject;
					if (item3.value.Flag == MapTreasureFlag.ReleaseFlag || item3.value.Flag == MapTreasureFlag.End)
					{
						gameObject5.SetActive(value: true);
					}
					else
					{
						gameObject5.SetActive(value: false);
					}
				}
				num5++;
			}
		}
	}
}
