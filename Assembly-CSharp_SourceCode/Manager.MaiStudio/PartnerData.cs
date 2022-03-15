using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class PartnerData : AccessorBase
	{
		public StringID netOpenName { get; private set; }

		public StringID releaseTagName { get; private set; }

		public bool disable { get; private set; }

		public StringID eventName { get; private set; }

		public StringID name { get; private set; }

		public StringID genre { get; private set; }

		public bool isDefault { get; private set; }

		public string normText { get; private set; }

		public StringID naviChara { get; private set; }

		public ItemDispKind dispCond { get; private set; }

		public TrophyRareType rareType { get; private set; }

		public ReleaseConditions relConds { get; private set; }

		public string fileName { get; private set; }

		public string thumbnailName { get; private set; }

		public bool isNew { get; private set; }

		public bool isHave { get; private set; }

		public bool isFavourite { get; private set; }

		public int priority { get; private set; }

		public string dataName { get; private set; }

		public PartnerData()
		{
			netOpenName = new StringID();
			releaseTagName = new StringID();
			disable = false;
			eventName = new StringID();
			name = new StringID();
			genre = new StringID();
			isDefault = false;
			normText = "";
			naviChara = new StringID();
			dispCond = ItemDispKind.None;
			rareType = TrophyRareType.Normal;
			relConds = new ReleaseConditions();
			fileName = "";
			thumbnailName = "";
			isNew = false;
			isHave = false;
			isFavourite = false;
			priority = 0;
			dataName = "";
		}

		public void Init(Manager.MaiStudio.Serialize.PartnerData sz)
		{
			netOpenName = (StringID)sz.netOpenName;
			releaseTagName = (StringID)sz.releaseTagName;
			disable = sz.disable;
			eventName = (StringID)sz.eventName;
			name = (StringID)sz.name;
			genre = (StringID)sz.genre;
			isDefault = sz.isDefault;
			normText = sz.normText;
			naviChara = (StringID)sz.naviChara;
			dispCond = sz.dispCond;
			rareType = sz.rareType;
			relConds = (ReleaseConditions)sz.relConds;
			fileName = sz.fileName;
			thumbnailName = sz.thumbnailName;
			isNew = sz.isNew;
			isHave = sz.isHave;
			isFavourite = sz.isFavourite;
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
