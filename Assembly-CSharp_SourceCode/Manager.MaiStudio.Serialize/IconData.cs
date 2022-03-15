using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class IconData : SerializeBase, ISerialize
	{
		public StringID netOpenName;

		public StringID releaseTagName;

		public bool disable;

		public StringID eventName;

		public StringID name;

		public StringID genre;

		public bool isDefault;

		public ItemDispKind dispCond;

		public string normText;

		public ReleaseConditions relConds;

		public string fileName;

		public string thumbnailName;

		public bool isNew;

		public bool isHave;

		public bool isFavourite;

		public int priority;

		public string dataName;

		public IconData()
		{
			netOpenName = new StringID();
			releaseTagName = new StringID();
			disable = false;
			eventName = new StringID();
			name = new StringID();
			genre = new StringID();
			isDefault = false;
			dispCond = ItemDispKind.None;
			normText = "";
			relConds = new ReleaseConditions();
			fileName = "";
			thumbnailName = "";
			isNew = false;
			isHave = false;
			isFavourite = false;
			priority = 0;
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.IconData(IconData sz)
		{
			Manager.MaiStudio.IconData iconData = new Manager.MaiStudio.IconData();
			iconData.Init(sz);
			return iconData;
		}

		public override void AddPath(string parentPath)
		{
			netOpenName.AddPath(parentPath);
			releaseTagName.AddPath(parentPath);
			eventName.AddPath(parentPath);
			name.AddPath(parentPath);
			genre.AddPath(parentPath);
			relConds.AddPath(parentPath);
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
