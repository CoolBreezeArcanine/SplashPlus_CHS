using System.Collections.Generic;
using System.Collections.ObjectModel;
using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class MapData : AccessorBase
	{
		public StringID name { get; private set; }

		public bool IsCollabo { get; private set; }

		public bool IsInfinity { get; private set; }

		public StringID IslandId { get; private set; }

		public StringsCollection ReleaseConditionIds { get; private set; }

		public StringID ColorId { get; private set; }

		public StringID BonusMusicId { get; private set; }

		public int BonusMusicMagnification { get; private set; }

		public StringID OpenEventId { get; private set; }

		public StringID netOpenName { get; private set; }

		public ReadOnlyCollection<MapTreasureExData> TreasureExDatas { get; private set; }

		public string dataName { get; private set; }

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
			TreasureExDatas = new ReadOnlyCollection<MapTreasureExData>(new List<MapTreasureExData>());
			dataName = "";
		}

		public void Init(Manager.MaiStudio.Serialize.MapData sz)
		{
			name = (StringID)sz.name;
			IsCollabo = sz.IsCollabo;
			IsInfinity = sz.IsInfinity;
			IslandId = (StringID)sz.IslandId;
			ReleaseConditionIds = (StringsCollection)sz.ReleaseConditionIds;
			ColorId = (StringID)sz.ColorId;
			BonusMusicId = (StringID)sz.BonusMusicId;
			BonusMusicMagnification = sz.BonusMusicMagnification;
			OpenEventId = (StringID)sz.OpenEventId;
			netOpenName = (StringID)sz.netOpenName;
			List<MapTreasureExData> list = new List<MapTreasureExData>();
			foreach (Manager.MaiStudio.Serialize.MapTreasureExData treasureExData in sz.TreasureExDatas)
			{
				list.Add((MapTreasureExData)treasureExData);
			}
			TreasureExDatas = new ReadOnlyCollection<MapTreasureExData>(list);
			dataName = sz.dataName;
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
