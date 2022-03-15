using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class MapTreasureData : AccessorBase
	{
		public StringID name { get; private set; }

		public MapTreasureType TreasureType { get; private set; }

		public StringID CharacterId { get; private set; }

		public StringID MusicId { get; private set; }

		public int Numeric { get; private set; }

		public StringID Otomodachi { get; private set; }

		public StringID NamePlate { get; private set; }

		public StringID Frame { get; private set; }

		public StringID Challenge { get; private set; }

		public string dataName { get; private set; }

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

		public void Init(Manager.MaiStudio.Serialize.MapTreasureData sz)
		{
			name = (StringID)sz.name;
			TreasureType = sz.TreasureType;
			CharacterId = (StringID)sz.CharacterId;
			MusicId = (StringID)sz.MusicId;
			Numeric = sz.Numeric;
			Otomodachi = (StringID)sz.Otomodachi;
			NamePlate = (StringID)sz.NamePlate;
			Frame = (StringID)sz.Frame;
			Challenge = (StringID)sz.Challenge;
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
