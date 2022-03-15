using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class CardData : SerializeBase, ISerialize
	{
		public StringID netOpenName;

		public StringID releaseTagName;

		public bool disable;

		public StringID name;

		public StringID type;

		public StringID chara;

		public StringID enableVersion;

		public int priority;

		public string dataName;

		public CardData()
		{
			netOpenName = new StringID();
			releaseTagName = new StringID();
			disable = false;
			name = new StringID();
			type = new StringID();
			chara = new StringID();
			enableVersion = new StringID();
			priority = 0;
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.CardData(CardData sz)
		{
			Manager.MaiStudio.CardData cardData = new Manager.MaiStudio.CardData();
			cardData.Init(sz);
			return cardData;
		}

		public override void AddPath(string parentPath)
		{
			netOpenName.AddPath(parentPath);
			releaseTagName.AddPath(parentPath);
			name.AddPath(parentPath);
			type.AddPath(parentPath);
			chara.AddPath(parentPath);
			enableVersion.AddPath(parentPath);
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
