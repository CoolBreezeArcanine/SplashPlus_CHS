using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class MapBonusMusicData : SerializeBase, ISerialize
	{
		public StringID name;

		public StringsCollection MusicIds;

		public string dataName;

		public MapBonusMusicData()
		{
			name = new StringID();
			MusicIds = new StringsCollection();
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.MapBonusMusicData(MapBonusMusicData sz)
		{
			Manager.MaiStudio.MapBonusMusicData mapBonusMusicData = new Manager.MaiStudio.MapBonusMusicData();
			mapBonusMusicData.Init(sz);
			return mapBonusMusicData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
			MusicIds.AddPath(parentPath);
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
