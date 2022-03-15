using System.Collections.Generic;
using System.Linq;
using AMDaemon.Allnet;
using MAI2.Util;
using Manager.MaiStudio;
using Manager.MaiStudio.Serialize;
using Manager.UserDatas;
using Util;

namespace Manager.Achieve
{
	public class AchieveTotalData
	{
		public UserData UserData;

		public Dictionary<int, UserScore[]> Scores;

		public Dictionary<int, UserScore> AnyScores;

		public int PlayCount;

		public uint Tenpo;

		public long PlayTrackCount;

		public Dictionary<int, UserChara> CharaList;

		public Dictionary<int, UserMapData> MapList;

		public long MapDistance;

		public int ContinueLoginCount;

		public int TotalLoginCount;

		public Dictionary<int, int[]> MusicGroups;

		public Manager.MaiStudio.Serialize.ReleaseItemConditions SConditions;

		public Manager.MaiStudio.ReleaseItemConditions Conditions;

		public Dictionary<int, long> MapCharaCounts;

		public Dictionary<int, long> MapCharaAwakenings;

		public Dictionary<int, UserRegion> PlayRegions;

		public int PlayRegionCount;

		public static AchieveTotalData Create(int index)
		{
			UserData udata = Singleton<UserDataManager>.Instance.GetUserData(index);
			Dictionary<int, UserRegion> dictionary = udata.PlayRegionList.ToDictionary((UserRegion i) => i.RegionId);
			int regionCode = Auth.RegionCode;
			if (!dictionary.ContainsKey(regionCode))
			{
				dictionary[regionCode] = new UserRegion
				{
					RegionId = regionCode,
					PlayCount = 1
				};
			}
			List<UserMapData> list = new List<UserMapData>();
			list.Clear();
			if (Singleton<MapMaster>.Instance.RefUserMapList != null)
			{
				foreach (UserMapData item in Singleton<MapMaster>.Instance.RefUserMapList[index])
				{
					list.Add(new UserMapData
					{
						ID = item.ID,
						Distance = item.Distance,
						IsClear = item.IsClear,
						IsComplete = item.IsComplete,
						IsLock = item.IsLock
					});
				}
			}
			foreach (UserMapData v in udata.MapList)
			{
				UserMapData userMapData = list.FirstOrDefault((UserMapData x) => x.ID == v.ID);
				if (userMapData != null)
				{
					userMapData.Distance = v.Distance;
					userMapData.IsClear = v.IsClear;
					userMapData.IsComplete = v.IsComplete;
					userMapData.IsLock = v.IsLock;
				}
				else
				{
					list.Add(new UserMapData
					{
						ID = v.ID,
						Distance = v.Distance,
						IsClear = v.IsClear,
						IsComplete = v.IsComplete,
						IsLock = v.IsLock
					});
				}
			}
			AchieveTotalData obj = new AchieveTotalData
			{
				UserData = udata,
				Scores = ToScoreDictionary(udata.ScoreList),
				PlayCount = udata.Detail.PlayCount + 1,
				Tenpo = Auth.LocationId,
				PlayTrackCount = udata.ScoreList.Select((List<UserScore> i) => i.Sum((UserScore j) => j.playcount)).Sum(),
				CharaList = udata.CharaList.ToDictionary((UserChara i) => i.ID, (UserChara i) => i),
				MapList = list.ToDictionary((UserMapData i) => i.ID, (UserMapData i) => i),
				MapDistance = list.Sum((UserMapData i) => i.Distance),
				ContinueLoginCount = (Singleton<NetDataManager>.Instance.GetLoginVO(index)?.consecutiveLoginCount ?? 0),
				TotalLoginCount = (Singleton<NetDataManager>.Instance.GetLoginVO(index)?.loginCount ?? 0),
				PlayRegions = dictionary,
				PlayRegionCount = dictionary.Count,
				SConditions = new Manager.MaiStudio.Serialize.ReleaseItemConditions(),
				Conditions = new Manager.MaiStudio.ReleaseItemConditions()
			};
			obj.AnyScores = obj.Scores.ToDictionary((KeyValuePair<int, UserScore[]> i) => i.Key, delegate(KeyValuePair<int, UserScore[]> i)
			{
				UserScore[] source2 = i.Value.Where((UserScore j) => j != null).ToArray();
				return new UserScore(i.Key)
				{
					playcount = (uint)source2.Sum((UserScore j) => j.playcount),
					combo = source2.Max((UserScore j) => j.combo),
					sync = source2.Max((UserScore j) => j.sync)
				};
			});
			obj.MusicGroups = Singleton<DataManager>.Instance.GetMusicGroups().ToDictionary((KeyValuePair<int, Manager.MaiStudio.MusicGroupData> i) => i.Key, (KeyValuePair<int, Manager.MaiStudio.MusicGroupData> i) => i.Value.MusicIds.list.Select((Manager.MaiStudio.StringID j) => j.id).ToArray());
			Dictionary<int, UserChara[]> source = Singleton<DataManager>.Instance.GetMapDatas().ToDictionary((KeyValuePair<int, Manager.MaiStudio.MapData> i) => i.Key, (KeyValuePair<int, Manager.MaiStudio.MapData> i) => (from j in i.Value.TreasureExDatas
				select Singleton<DataManager>.Instance.GetMapTreasureData(j.TreasureId.id) into j
				where j != null && j.TreasureType == MapTreasureType.Character
				select udata.CharaList.FirstOrDefault((UserChara k) => k.ID == j.CharacterId.id) into j
				where j != null
				select j).ToArray());
			obj.MapCharaCounts = source.ToDictionary((KeyValuePair<int, UserChara[]> i) => i.Key, (KeyValuePair<int, UserChara[]> i) => i.Value.Sum((UserChara j) => j.Count));
			obj.MapCharaAwakenings = source.ToDictionary((KeyValuePair<int, UserChara[]> i) => i.Key, (KeyValuePair<int, UserChara[]> i) => i.Value.Sum((UserChara j) => j.Awakening));
			return obj;
		}

		private static Dictionary<int, UserScore[]> ToScoreDictionary(IEnumerable<List<UserScore>> src)
		{
			return (from i in src.SelectMany((List<UserScore> i, int index) => i.Select((UserScore j) => new
				{
					id = j.id,
					difficulty = index,
					score = j
				}))
				group i by i.id).ToDictionary(grp => grp.Key, grp =>
			{
				UserScore[] array = new UserScore[6];
				foreach (var item in grp)
				{
					array[item.difficulty] = item.score;
				}
				return array;
			});
		}
	}
}
