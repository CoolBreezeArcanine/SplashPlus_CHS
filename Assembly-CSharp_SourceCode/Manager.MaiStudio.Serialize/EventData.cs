using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class EventData : SerializeBase, ISerialize
	{
		public StringID name;

		public EventInfoType infoType;

		public bool alwaysOpen;

		public string dataName;

		public EventData()
		{
			name = new StringID();
			infoType = EventInfoType.Normal;
			alwaysOpen = false;
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.EventData(EventData sz)
		{
			Manager.MaiStudio.EventData eventData = new Manager.MaiStudio.EventData();
			eventData.Init(sz);
			return eventData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
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
