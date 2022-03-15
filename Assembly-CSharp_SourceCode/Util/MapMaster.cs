using System.Collections.Generic;
using System.Linq;
using Datas.DebugData;
using MAI2.Util;
using Manager;
using Manager.MaiStudio;
using Net.VO.Mai2;
using UnityEngine;

namespace Util
{
	public class MapMaster : Singleton<MapMaster>
	{
		private class MapNode
		{
			public List<int> ReleaseConditionIds = new List<int>();

			public List<MapTreasureDataEx> TreasureDatas = new List<MapTreasureDataEx>();
		}

		public class MapTreasureDataEx
		{
			public int Distance;

			public int Flag;

			public MapTreasureData MapTreasureData;

			public MapTreasureDataEx(int dist, int flag, MapTreasureData data)
			{
				Distance = dist;
				Flag = flag;
				MapTreasureData = data;
			}
		}

		private readonly List<UserMapData>[] _debugUserMaps = new List<UserMapData>[4];

		private readonly Dictionary<int, CharacterMapColorData> _characterMapColors = new Dictionary<int, CharacterMapColorData>();

		private readonly Dictionary<int, MapNode> _mapNodes = new Dictionary<int, MapNode>();

		private int _maxPlayerCount;

		public List<UserMapData>[] RefUserMapList;

		public List<int>[] RefExistMapIDList;

		public bool[] IsCallAwake;

		public bool[] IsNeedAwake;

		public List<UserMapData>[] RefConvertUserMapList;

		public bool[] IsConvertMapDistance;

		public int[] TicketID;

		public bool IsCreateMapData { get; set; }

		public void ClearConvertMapDistance()
		{
			_maxPlayerCount = 2;
			RefConvertUserMapList = new List<UserMapData>[_maxPlayerCount];
			IsConvertMapDistance = new bool[_maxPlayerCount];
			for (int i = 0; i < 2; i++)
			{
				RefConvertUserMapList[i] = new List<UserMapData>();
				IsConvertMapDistance[i] = false;
			}
		}

		public void CreateConvertMapDistance(int monitorID)
		{
			if (IsConvertMapDistance[monitorID])
			{
				return;
			}
			IsConvertMapDistance[monitorID] = false;
			foreach (KeyValuePair<int, MapData> mapData in Singleton<DataManager>.Instance.GetMapDatas())
			{
				int key = mapData.Key;
				RefConvertUserMapList[monitorID].Add(ConvertNewPlayerMap(monitorID, key));
			}
			UserMap[] userMaps = Singleton<NetDataManager>.Instance.GetUserMaps(monitorID);
			if (userMaps != null && userMaps.Length != 0)
			{
				for (int i = 0; i < userMaps.Length; i++)
				{
					UserMapData userMapData = ConvertUserMap(userMaps[i]);
					if (userMapData != null)
					{
						int mapID = userMaps[i].mapId;
						int num = RefConvertUserMapList[monitorID].FindIndex((UserMapData userMap) => userMap.ID == mapID);
						if (num < 0)
						{
							RefConvertUserMapList[monitorID].Add(userMapData);
						}
						else
						{
							RefConvertUserMapList[monitorID][num] = userMapData;
						}
					}
				}
			}
			IsConvertMapDistance[monitorID] = true;
		}

		public void Initialize()
		{
			DataManager dmgr = Singleton<DataManager>.Instance;
			foreach (KeyValuePair<int, MapData> mapData in Singleton<DataManager>.Instance.GetMapDatas())
			{
				MapNode mapNode = new MapNode();
				MapData value = mapData.Value;
				int key = mapData.Key;
				mapNode.ReleaseConditionIds = (from i in value.ReleaseConditionIds.list
					select i.id into i
					orderby i
					select i).ToList();
				mapNode.TreasureDatas = (from i in value.TreasureExDatas
					select new MapTreasureDataEx(i.Distance, (int)i.Flag, dmgr.GetMapTreasureData(i.TreasureId.id)) into i
					orderby i.Distance
					select i).ToList();
				_mapNodes[key] = mapNode;
				if (0 < mapNode.ReleaseConditionIds.Count)
				{
					_ = mapNode.ReleaseConditionIds[0];
				}
				if (!_characterMapColors.ContainsKey(key))
				{
					_characterMapColors[key] = new CharacterMapColorData(key, value.ColorId.id);
				}
			}
		}

		public static List<MapTreasureDataEx> GetTreasures(int mapId)
		{
			if (!Singleton<MapMaster>.Instance._mapNodes.TryGetValue(mapId, out var value))
			{
				return new List<MapTreasureDataEx>();
			}
			return value.TreasureDatas;
		}

		public static CharacterMapColorData GetSlotData(int colorId)
		{
			if (!Singleton<MapMaster>.Instance._characterMapColors.TryGetValue(colorId, out var value))
			{
				return null;
			}
			return value;
		}

		public bool IsDeluxeMap(int mapId)
		{
			bool result = false;
			MapData mapData = Singleton<DataManager>.Instance.GetMapData(mapId);
			if (mapData != null && mapData.IsInfinity)
			{
				result = true;
			}
			return result;
		}

		public void CreateMapData()
		{
			if (IsCreateMapData)
			{
				for (int i = 0; i < 2; i++)
				{
					List<UserMapData> list = RefUserMapList[i];
					for (int j = 0; j < list.Count; j++)
					{
						UserMapData userMapData = list[j];
						bool isLock = IsLock(i, userMapData.ReleaseIds);
						list[j].IsLock = isLock;
					}
				}
				return;
			}
			IsCreateMapData = false;
			_maxPlayerCount = 2;
			RefUserMapList = new List<UserMapData>[_maxPlayerCount];
			RefExistMapIDList = new List<int>[_maxPlayerCount];
			IsCallAwake = new bool[_maxPlayerCount];
			IsNeedAwake = new bool[_maxPlayerCount];
			TicketID = new int[_maxPlayerCount];
			for (int k = 0; k < 2; k++)
			{
				RefUserMapList[k] = new List<UserMapData>();
				RefExistMapIDList[k] = new List<int>();
				IsCallAwake[k] = false;
				IsNeedAwake[k] = false;
				TicketID[k] = -1;
				foreach (KeyValuePair<int, MapData> mapData2 in Singleton<DataManager>.Instance.GetMapDatas())
				{
					int key = mapData2.Key;
					RefUserMapList[k].Add(ConvertNewPlayerMap(k, key));
				}
				UserMap[] userMaps = Singleton<NetDataManager>.Instance.GetUserMaps(k);
				if (userMaps != null && userMaps.Length != 0)
				{
					for (int l = 0; l < userMaps.Length; l++)
					{
						UserMapData userMapData2 = ConvertUserMap(userMaps[l]);
						if (userMapData2 != null)
						{
							int mapID = userMaps[l].mapId;
							int num = RefUserMapList[k].FindIndex((UserMapData userMap) => userMap.ID == mapID);
							if (num < 0)
							{
								RefUserMapList[k].Add(userMapData2);
								continue;
							}
							RefUserMapList[k][num] = userMapData2;
							RefExistMapIDList[k].Add(mapID);
						}
					}
				}
				if (!IsConvertMapDistance[k])
				{
					continue;
				}
				int num2 = -1;
				foreach (UserMapData map in RefConvertUserMapList[k])
				{
					num2 = RefUserMapList[k].FindIndex((UserMapData m) => m.ID == map.ID);
					if (num2 >= 0)
					{
						RefUserMapList[k][num2].Distance = map.Distance;
						if (map.IsComplete)
						{
							RefUserMapList[k][num2].IsComplete = true;
						}
					}
				}
			}
			for (int n = 0; n < 2; n++)
			{
				List<UserMapData> list2 = RefUserMapList[n];
				for (int num3 = 0; num3 < list2.Count; num3++)
				{
					UserMapData userMapData3 = list2[num3];
					bool isLock2 = IsLock(n, userMapData3.ReleaseIds);
					list2[num3].IsLock = isLock2;
					if (list2[num3].IsEvent)
					{
						int iD = list2[num3].ID;
						MapData mapData = Singleton<DataManager>.Instance.GetMapData(iD);
						if (mapData != null && !Singleton<EventManager>.Instance.IsOpenEvent(mapData.OpenEventId.id))
						{
							list2[num3].IsLock = true;
						}
					}
				}
			}
			IsCreateMapData = true;
		}

		private UserMapData ConvertNewPlayerMap(int monitorId, int mapId)
		{
			MapData mapData = Singleton<DataManager>.Instance.GetMapData(mapId);
			List<int> list = new List<int>();
			for (int i = 0; i < mapData.ReleaseConditionIds.list.Count; i++)
			{
				list.Add(mapData.ReleaseConditionIds.list[i].id);
			}
			UserMapData userMapData = Singleton<UserDataManager>.Instance.GetUserData(monitorId).MapList.ToArray().FirstOrDefault((UserMapData targetMap) => targetMap.ID == mapId);
			uint num = 0u;
			if (userMapData != null)
			{
				num = userMapData.Distance;
			}
			bool flag = IsClear(mapId, num);
			bool flag2 = IsComplete(mapId, num);
			flag = false;
			flag2 = false;
			List<MapTreasureDataEx> treasures = GetTreasures(mapId);
			MapTreasureDataEx mapTreasureDataEx = treasures.FirstOrDefault((MapTreasureDataEx t) => t.Flag == 2);
			MapTreasureDataEx mapTreasureDataEx2 = treasures.FirstOrDefault((MapTreasureDataEx t) => t.Flag == 3);
			if (mapTreasureDataEx2 == null && mapTreasureDataEx == null)
			{
				flag2 = IsComplete(mapId, num);
				flag = false;
			}
			else if (mapTreasureDataEx2 == null)
			{
				if (mapTreasureDataEx != null)
				{
					flag2 = IsComplete(mapId, num);
					flag = IsClear(mapId, num);
				}
			}
			else if (mapTreasureDataEx != null)
			{
				flag2 = IsComplete(mapId, num);
				flag = IsClear(mapId, num);
			}
			else
			{
				flag2 = IsComplete(mapId, num);
				flag = false;
			}
			return new UserMapData
			{
				ID = mapId,
				CharaColorKey = mapData.ColorId.id,
				MapColor = Color.black,
				ReleaseIds = list,
				Name = mapData.name.str,
				IsEvent = mapData.IsCollabo,
				IsClear = flag,
				IsComplete = flag2,
				Distance = num,
				IsDeluxe = mapData.IsInfinity
			};
		}

		public static UserMapData ConvertUserMap(UserMap map)
		{
			int mapId = map.mapId;
			bool isClear = map.isClear;
			bool isComplete = map.isComplete;
			MapData mapData = Singleton<DataManager>.Instance.GetMapData(mapId);
			if (mapData == null)
			{
				return null;
			}
			List<int> list = new List<int>();
			for (int i = 0; i < mapData.ReleaseConditionIds.list.Count; i++)
			{
				list.Add(mapData.ReleaseConditionIds.list[i].id);
			}
			return new UserMapData
			{
				ID = mapId,
				CharaColorKey = mapData.ColorId.id,
				MapColor = Color.black,
				ReleaseIds = list,
				Name = mapData.name.str,
				IsEvent = mapData.IsCollabo,
				IsClear = isClear,
				IsComplete = isComplete,
				Distance = map.distance,
				IsDeluxe = mapData.IsInfinity
			};
		}

		private bool IsClear(int mapId, float currentLength)
		{
			if (IsDeluxeMap(mapId))
			{
				return false;
			}
			MapTreasureDataEx mapTreasureDataEx = GetTreasures(mapId).FirstOrDefault((MapTreasureDataEx t) => t.Flag == 2);
			float num = 0f;
			if (mapTreasureDataEx != null)
			{
				num = mapTreasureDataEx.Distance;
			}
			return num <= currentLength;
		}

		private bool IsComplete(int mapId, float currentLength)
		{
			if (IsDeluxeMap(mapId))
			{
				return false;
			}
			MapTreasureDataEx mapTreasureDataEx = GetTreasures(mapId).FirstOrDefault((MapTreasureDataEx t) => t.Flag == 3);
			float num = 0f;
			if (mapTreasureDataEx != null)
			{
				num = mapTreasureDataEx.Distance;
			}
			return num <= currentLength;
		}

		public bool IsRelease(int mapId, float currentLength)
		{
			bool flag = false;
			MapTreasureDataEx mapTreasureDataEx = GetTreasures(mapId).FirstOrDefault((MapTreasureDataEx t) => t.Flag == 2 || t.Flag == 3);
			float num = 0f;
			if (mapTreasureDataEx != null)
			{
				num = mapTreasureDataEx.Distance;
			}
			MapData mapData = Singleton<DataManager>.Instance.GetMapData(mapId);
			int num2 = 0;
			if (mapData != null)
			{
				num2 = mapData.OpenEventId.id;
				if (Singleton<EventManager>.Instance.IsOpenEvent(num2))
				{
					return num <= currentLength;
				}
				return false;
			}
			return false;
		}

		private bool IsLock(int monitorId, List<int> releaseMapIds)
		{
			UserMapData[] source = Singleton<MapMaster>.Instance.RefUserMapList[monitorId].ToArray();
			foreach (int id in releaseMapIds)
			{
				UserMapData userMapData = source.FirstOrDefault((UserMapData m) => m.ID == id);
				if (userMapData != null && !IsRelease(userMapData.ID, userMapData.Distance))
				{
					return true;
				}
			}
			return false;
		}

		public void SetDebugData(int userIndex, List<DebugUserMap> mapList)
		{
			List<UserMapData> list = new List<UserMapData>(mapList.Count);
			foreach (DebugUserMap map in mapList)
			{
				list.Add(new UserMapData(map.ID)
				{
					Distance = map.Distance,
					IsLock = map.IsLock,
					IsClear = map.IsClear,
					IsComplete = map.IsComplete
				});
			}
			_debugUserMaps[userIndex] = list;
		}

		public void CreateUserDataMapList(int monitorId)
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(monitorId);
			if (userData == null || !userData.IsEntry || userData.IsGuest())
			{
				return;
			}
			List<UserMapData> list = Singleton<MapMaster>.Instance.RefUserMapList[monitorId];
			MapData mapData = null;
			MapTreasureExData mapTreasureExData = null;
			MapTreasureExData mapTreasureExData2 = null;
			foreach (var item in list.Select((UserMapData value, int index) => new { value, index }))
			{
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				int mapID3 = 0;
				int num = 0;
				int num2 = 0;
				int distance = (int)item.value.Distance;
				mapData = Singleton<DataManager>.Instance.GetMapData(item.value.ID);
				if (mapData != null && mapData.TreasureExDatas != null)
				{
					mapTreasureExData = mapData.TreasureExDatas.FirstOrDefault((MapTreasureExData t) => t.Flag == MapTreasureFlag.ReleaseFlag);
					mapTreasureExData2 = mapData.TreasureExDatas.FirstOrDefault((MapTreasureExData t) => t.Flag == MapTreasureFlag.End);
					if (mapTreasureExData != null && distance >= mapTreasureExData.Distance)
					{
						flag = true;
						list[item.index].IsClear = true;
					}
					if (mapTreasureExData2 != null && distance >= mapTreasureExData2.Distance)
					{
						flag2 = true;
						list[item.index].IsComplete = true;
					}
				}
				if (mapData != null && mapData.TreasureExDatas != null && (flag || flag2 || item.value.IsFinishedOpening))
				{
					mapID3 = item.value.ID;
					num = userData.MapList.FindIndex((UserMapData user_map) => user_map.ID == mapID3);
					if (num < 0)
					{
						userData.MapList.Add(list[item.index]);
						num = userData.MapList.Count - 1;
					}
					if (item.value.IsFinishedOpening)
					{
						userData.MapList[num].IsFinishedOpening = true;
						userData.MapList[num].IsLock = false;
					}
					if (mapTreasureExData == null)
					{
						userData.MapList[num].IsClear = false;
					}
					if (mapTreasureExData2 == null)
					{
						userData.MapList[num].IsComplete = false;
					}
					flag3 = true;
				}
				if (flag3)
				{
					continue;
				}
				mapID3 = userData.Detail.SelectMapID;
				if (item.value.ID != userData.Detail.SelectMapID)
				{
					continue;
				}
				num = userData.MapList.FindIndex((UserMapData user_map) => user_map.ID == mapID3);
				if (num < 0)
				{
					num2 = list.FindIndex((UserMapData user_map) => user_map.ID == mapID3);
					if (num2 >= 0)
					{
						userData.MapList.Add(list[num2]);
						num = userData.MapList.Count - 1;
						userData.MapList[num].IsLock = false;
					}
				}
				if (mapTreasureExData == null)
				{
					userData.MapList[num].IsClear = false;
				}
				if (mapTreasureExData2 == null)
				{
					userData.MapList[num].IsComplete = false;
				}
			}
			if (IsConvertMapDistance[monitorId])
			{
				int num3 = -1;
				foreach (UserMapData map in RefConvertUserMapList[monitorId])
				{
					num3 = RefUserMapList[monitorId].FindIndex((UserMapData m) => m.ID == map.ID);
					if (num3 >= 0)
					{
						RefUserMapList[monitorId][num3].Distance = map.Distance;
						if (map.IsComplete)
						{
							RefUserMapList[monitorId][num3].IsComplete = true;
						}
						int mapID2 = 0;
						mapID2 = RefUserMapList[monitorId][num3].ID;
						if (userData.MapList.FindIndex((UserMapData user_map) => user_map.ID == mapID2) < 0)
						{
							userData.MapList.Add(RefUserMapList[monitorId][num3]);
						}
					}
				}
				IsConvertMapDistance[monitorId] = false;
			}
			int mapID = userData.Detail.SelectMapID;
			int num4 = userData.MapList.FindIndex((UserMapData user_map) => user_map.ID == mapID);
			mapData = Singleton<DataManager>.Instance.GetMapData(mapID);
			mapTreasureExData = null;
			mapTreasureExData2 = null;
			if (mapData != null && mapData.TreasureExDatas != null)
			{
				mapTreasureExData = mapData.TreasureExDatas.FirstOrDefault((MapTreasureExData t) => t.Flag == MapTreasureFlag.ReleaseFlag);
				mapTreasureExData2 = mapData.TreasureExDatas.FirstOrDefault((MapTreasureExData t) => t.Flag == MapTreasureFlag.End);
			}
			if (num4 < 0)
			{
				int num5 = list.FindIndex((UserMapData user_map) => user_map.ID == mapID);
				if (num5 >= 0)
				{
					userData.MapList.Add(list[num5]);
					num4 = userData.MapList.Count - 1;
					userData.MapList[num4].IsLock = false;
				}
				if (mapTreasureExData == null)
				{
					userData.MapList[num4].IsClear = false;
				}
				if (mapTreasureExData2 == null)
				{
					userData.MapList[num4].IsComplete = false;
				}
			}
		}
	}
}
