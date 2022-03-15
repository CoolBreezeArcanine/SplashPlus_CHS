using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class MapTreasureExData : SerializeBase
	{
		public int Distance;

		public MapTreasureFlag Flag;

		public int SubParam1;

		public int SubParam2;

		public StringID TreasureId;

		public MapTreasureExData()
		{
			Distance = 0;
			Flag = MapTreasureFlag.None;
			SubParam1 = 0;
			SubParam2 = 0;
			TreasureId = new StringID();
		}

		public static explicit operator Manager.MaiStudio.MapTreasureExData(MapTreasureExData sz)
		{
			Manager.MaiStudio.MapTreasureExData mapTreasureExData = new Manager.MaiStudio.MapTreasureExData();
			mapTreasureExData.Init(sz);
			return mapTreasureExData;
		}

		public override void AddPath(string parentPath)
		{
			TreasureId.AddPath(parentPath);
		}
	}
}
