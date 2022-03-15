using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class FrameData : SerializeBase, ISerialize
	{
		public StringID netOpenName;

		public StringID releaseTagName;

		public bool disable;

		public StringID eventName;

		public StringID name;

		public StringID genre;

		public bool isDefault;

		public bool isEffect;

		public ItemDispKind dispCond;

		public string normText;

		public ReleaseConditions relConds;

		public string fileName;

		public string maskName;

		public string thumbnailName;

		public bool isNew;

		public bool isHave;

		public bool isFavourite;

		public int priority;

		public string dataName;

		public FrameData()
		{
			netOpenName = new StringID();
			releaseTagName = new StringID();
			disable = false;
			eventName = new StringID();
			name = new StringID();
			genre = new StringID();
			isDefault = false;
			isEffect = false;
			dispCond = ItemDispKind.None;
			normText = "";
			relConds = new ReleaseConditions();
			fileName = "";
			maskName = "";
			thumbnailName = "";
			isNew = false;
			isHave = false;
			isFavourite = false;
			priority = 0;
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.FrameData(FrameData sz)
		{
			Manager.MaiStudio.FrameData frameData = new Manager.MaiStudio.FrameData();
			frameData.Init(sz);
			return frameData;
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
