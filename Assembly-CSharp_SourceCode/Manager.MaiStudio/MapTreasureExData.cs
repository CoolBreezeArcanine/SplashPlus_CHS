using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class MapTreasureExData : AccessorBase
	{
		public int Distance { get; private set; }

		public MapTreasureFlag Flag { get; private set; }

		public int SubParam1 { get; private set; }

		public int SubParam2 { get; private set; }

		public StringID TreasureId { get; private set; }

		public MapTreasureExData()
		{
			Distance = 0;
			Flag = MapTreasureFlag.None;
			SubParam1 = 0;
			SubParam2 = 0;
			TreasureId = new StringID();
		}

		public void Init(Manager.MaiStudio.Serialize.MapTreasureExData sz)
		{
			Distance = sz.Distance;
			Flag = sz.Flag;
			SubParam1 = sz.SubParam1;
			SubParam2 = sz.SubParam2;
			TreasureId = (StringID)sz.TreasureId;
		}
	}
}
