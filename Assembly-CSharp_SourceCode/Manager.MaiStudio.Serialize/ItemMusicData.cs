using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class ItemMusicData : SerializeBase, ISerialize
	{
		public StringID netOpenName;

		public StringID releaseTagName;

		public bool disable;

		public StringID eventName;

		public StringID name;

		public ReleaseConditions relConds;

		public string dataName;

		public ItemMusicData()
		{
			netOpenName = new StringID();
			releaseTagName = new StringID();
			disable = false;
			eventName = new StringID();
			name = new StringID();
			relConds = new ReleaseConditions();
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.ItemMusicData(ItemMusicData sz)
		{
			Manager.MaiStudio.ItemMusicData itemMusicData = new Manager.MaiStudio.ItemMusicData();
			itemMusicData.Init(sz);
			return itemMusicData;
		}

		public override void AddPath(string parentPath)
		{
			netOpenName.AddPath(parentPath);
			releaseTagName.AddPath(parentPath);
			eventName.AddPath(parentPath);
			name.AddPath(parentPath);
			relConds.AddPath(parentPath);
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
			return disable;
		}
	}
}
