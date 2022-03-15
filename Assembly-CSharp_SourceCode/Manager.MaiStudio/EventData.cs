using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class EventData : AccessorBase
	{
		public StringID name { get; private set; }

		public EventInfoType infoType { get; private set; }

		public bool alwaysOpen { get; private set; }

		public string dataName { get; private set; }

		public EventData()
		{
			name = new StringID();
			infoType = EventInfoType.Normal;
			alwaysOpen = false;
			dataName = "";
		}

		public void Init(Manager.MaiStudio.Serialize.EventData sz)
		{
			name = (StringID)sz.name;
			infoType = sz.infoType;
			alwaysOpen = sz.alwaysOpen;
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
			return false;
		}
	}
}
