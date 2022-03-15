using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class MapBonusMusicData : AccessorBase
	{
		public StringID name { get; private set; }

		public StringsCollection MusicIds { get; private set; }

		public string dataName { get; private set; }

		public MapBonusMusicData()
		{
			name = new StringID();
			MusicIds = new StringsCollection();
			dataName = "";
		}

		public void Init(Manager.MaiStudio.Serialize.MapBonusMusicData sz)
		{
			name = (StringID)sz.name;
			MusicIds = (StringsCollection)sz.MusicIds;
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
