using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class TitleData : SerializeBase, ISerialize
	{
		public StringID netOpenName;

		public StringID releaseTagName;

		public bool disable;

		public StringID eventName;

		public StringID name;

		public StringID genre;

		public bool isDefault;

		public string normText;

		public ItemDispKind dispCond;

		public TrophyRareType rareType;

		public ReleaseConditions relConds;

		public bool isNew;

		public bool isHave;

		public bool isFavourite;

		public int priority;

		public string dataName;

		public TitleData()
		{
			netOpenName = new StringID();
			releaseTagName = new StringID();
			disable = false;
			eventName = new StringID();
			name = new StringID();
			genre = new StringID();
			isDefault = false;
			normText = "";
			dispCond = ItemDispKind.None;
			rareType = TrophyRareType.Normal;
			relConds = new ReleaseConditions();
			isNew = false;
			isHave = false;
			isFavourite = false;
			priority = 0;
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.TitleData(TitleData sz)
		{
			Manager.MaiStudio.TitleData titleData = new Manager.MaiStudio.TitleData();
			titleData.Init(sz);
			return titleData;
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
