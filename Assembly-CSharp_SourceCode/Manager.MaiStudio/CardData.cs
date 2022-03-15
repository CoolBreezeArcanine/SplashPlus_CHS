using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class CardData : AccessorBase
	{
		public StringID netOpenName { get; private set; }

		public StringID releaseTagName { get; private set; }

		public bool disable { get; private set; }

		public StringID name { get; private set; }

		public StringID type { get; private set; }

		public StringID chara { get; private set; }

		public StringID enableVersion { get; private set; }

		public int priority { get; private set; }

		public string dataName { get; private set; }

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

		public void Init(Manager.MaiStudio.Serialize.CardData sz)
		{
			netOpenName = (StringID)sz.netOpenName;
			releaseTagName = (StringID)sz.releaseTagName;
			disable = sz.disable;
			name = (StringID)sz.name;
			type = (StringID)sz.type;
			chara = (StringID)sz.chara;
			enableVersion = (StringID)sz.enableVersion;
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
