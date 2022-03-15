using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class CardCharaData : AccessorBase
	{
		public bool disable { get; private set; }

		public StringID name { get; private set; }

		public StringID mapId { get; private set; }

		public int uniqueId { get; private set; }

		public string imageFile { get; private set; }

		public string thumbnailName { get; private set; }

		public int priority { get; private set; }

		public string dataName { get; private set; }

		public CardCharaData()
		{
			disable = false;
			name = new StringID();
			mapId = new StringID();
			uniqueId = 0;
			imageFile = "";
			thumbnailName = "";
			priority = 0;
			dataName = "";
		}

		public void Init(Manager.MaiStudio.Serialize.CardCharaData sz)
		{
			disable = sz.disable;
			name = (StringID)sz.name;
			mapId = (StringID)sz.mapId;
			uniqueId = sz.uniqueId;
			imageFile = sz.imageFile;
			thumbnailName = sz.thumbnailName;
			priority = sz.priority;
			dataName = sz.dataName;
		}

		public int GetID()
		{
			return name.id;
		}

		public void SetPriority(int pri)
		{
			priority = pri;
		}

		public bool IsDisable()
		{
			return disable;
		}
	}
}
