using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class CardCharaPackData : SerializeBase, ISerialize
	{
		public StringID name;

		public StringsCollection CardCharaIds;

		public string imageFile;

		public string thumbnailName;

		public string dataName;

		public CardCharaPackData()
		{
			name = new StringID();
			CardCharaIds = new StringsCollection();
			imageFile = "";
			thumbnailName = "";
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.CardCharaPackData(CardCharaPackData sz)
		{
			Manager.MaiStudio.CardCharaPackData cardCharaPackData = new Manager.MaiStudio.CardCharaPackData();
			cardCharaPackData.Init(sz);
			return cardCharaPackData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
			CardCharaIds.AddPath(parentPath);
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
