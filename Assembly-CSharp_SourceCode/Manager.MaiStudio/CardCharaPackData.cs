using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class CardCharaPackData : AccessorBase
	{
		public StringID name { get; private set; }

		public StringsCollection CardCharaIds { get; private set; }

		public string imageFile { get; private set; }

		public string thumbnailName { get; private set; }

		public string dataName { get; private set; }

		public CardCharaPackData()
		{
			name = new StringID();
			CardCharaIds = new StringsCollection();
			imageFile = "";
			thumbnailName = "";
			dataName = "";
		}

		public void Init(Manager.MaiStudio.Serialize.CardCharaPackData sz)
		{
			name = (StringID)sz.name;
			CardCharaIds = (StringsCollection)sz.CardCharaIds;
			imageFile = sz.imageFile;
			thumbnailName = sz.thumbnailName;
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
