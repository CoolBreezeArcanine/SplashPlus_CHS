using System;
using System.Collections.Generic;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class MapData : SerializeBase, ISerialize
	{
		public StringID name;

		public bool IsCollabo;

		public bool IsInfinity;

		public StringID IslandId;

		public StringsCollection ReleaseConditionIds;

		public StringID ColorId;

		public StringID BonusMusicId;

		public int BonusMusicMagnification;

		public StringID OpenEventId;

		public StringID netOpenName;

		public List<MapTreasureExData> TreasureExDatas;

		public string dataName;

		public MapData()
		{
			name = new StringID();
			IsCollabo = false;
			IsInfinity = false;
			IslandId = new StringID();
			ReleaseConditionIds = new StringsCollection();
			ColorId = new StringID();
			BonusMusicId = new StringID();
			BonusMusicMagnification = 0;
			OpenEventId = new StringID();
			netOpenName = new StringID();
			TreasureExDatas = new List<MapTreasureExData>();
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.MapData(MapData sz)
		{
			Manager.MaiStudio.MapData mapData = new Manager.MaiStudio.MapData();
			mapData.Init(sz);
			return mapData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
			IslandId.AddPath(parentPath);
			ReleaseConditionIds.AddPath(parentPath);
			ColorId.AddPath(parentPath);
			BonusMusicId.AddPath(parentPath);
			OpenEventId.AddPath(parentPath);
			netOpenName.AddPath(parentPath);
			foreach (MapTreasureExData treasureExData in TreasureExDatas)
			{
				treasureExData.AddPath(parentPath);
			}
		}

		public int GetID()
		{
			return name.id;
		}

		public void SetPriority(int pri)
		{
		}

		public bool IsDisable()
		{
			return false;
		}
	}
}
