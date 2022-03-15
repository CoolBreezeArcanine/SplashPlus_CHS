using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class CardCharaData : SerializeBase, ISerialize
	{
		public bool disable;

		public StringID name;

		public StringID mapId;

		public int uniqueId;

		public string imageFile;

		public string thumbnailName;

		public int priority;

		public string dataName;

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

		public static explicit operator Manager.MaiStudio.CardCharaData(CardCharaData sz)
		{
			Manager.MaiStudio.CardCharaData cardCharaData = new Manager.MaiStudio.CardCharaData();
			cardCharaData.Init(sz);
			return cardCharaData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
			mapId.AddPath(parentPath);
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
