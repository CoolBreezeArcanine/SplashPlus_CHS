using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class MapTreasureData : SerializeBase, ISerialize
	{
		public StringID name;

		public MapTreasureType TreasureType;

		public StringID CharacterId;

		public StringID MusicId;

		public int Numeric;

		public StringID Otomodachi;

		public StringID NamePlate;

		public StringID Frame;

		public StringID Challenge;

		public string dataName;

		public MapTreasureData()
		{
			name = new StringID();
			TreasureType = (MapTreasureType)0;
			CharacterId = new StringID();
			MusicId = new StringID();
			Numeric = 0;
			Otomodachi = new StringID();
			NamePlate = new StringID();
			Frame = new StringID();
			Challenge = new StringID();
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.MapTreasureData(MapTreasureData sz)
		{
			Manager.MaiStudio.MapTreasureData mapTreasureData = new Manager.MaiStudio.MapTreasureData();
			mapTreasureData.Init(sz);
			return mapTreasureData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
			CharacterId.AddPath(parentPath);
			MusicId.AddPath(parentPath);
			Otomodachi.AddPath(parentPath);
			NamePlate.AddPath(parentPath);
			Frame.AddPath(parentPath);
			Challenge.AddPath(parentPath);
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
