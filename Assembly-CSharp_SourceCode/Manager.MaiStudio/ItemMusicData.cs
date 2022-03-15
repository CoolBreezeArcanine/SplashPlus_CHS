using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class ItemMusicData : AccessorBase
	{
		public StringID netOpenName { get; private set; }

		public StringID releaseTagName { get; private set; }

		public bool disable { get; private set; }

		public StringID eventName { get; private set; }

		public StringID name { get; private set; }

		public ReleaseConditions relConds { get; private set; }

		public string dataName { get; private set; }

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

		public void Init(Manager.MaiStudio.Serialize.ItemMusicData sz)
		{
			netOpenName = (StringID)sz.netOpenName;
			releaseTagName = (StringID)sz.releaseTagName;
			disable = sz.disable;
			eventName = (StringID)sz.eventName;
			name = (StringID)sz.name;
			relConds = (ReleaseConditions)sz.relConds;
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
			return disable;
		}
	}
}
