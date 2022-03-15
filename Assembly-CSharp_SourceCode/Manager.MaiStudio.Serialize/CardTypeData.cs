using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class CardTypeData : SerializeBase, ISerialize
	{
		public StringID name;

		public string title;

		public int paramId;

		public bool disable;

		public string effectText;

		public CardKind kind;

		public StringID releaseId;

		public CardMapBonus mapBonus;

		public string imageFile;

		public string thumbnailName;

		public string frameImageFile;

		public string frameThumbnailName;

		public int priority;

		public int extendBitParameter;

		public string dataName;

		public CardTypeData()
		{
			name = new StringID();
			title = "";
			paramId = 0;
			disable = false;
			effectText = "";
			kind = CardKind.Pass;
			releaseId = new StringID();
			mapBonus = CardMapBonus.None;
			imageFile = "";
			thumbnailName = "";
			frameImageFile = "";
			frameThumbnailName = "";
			priority = 0;
			extendBitParameter = 0;
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.CardTypeData(CardTypeData sz)
		{
			Manager.MaiStudio.CardTypeData cardTypeData = new Manager.MaiStudio.CardTypeData();
			cardTypeData.Init(sz);
			return cardTypeData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
			releaseId.AddPath(parentPath);
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
