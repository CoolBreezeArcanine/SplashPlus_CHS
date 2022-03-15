using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class TicketData : SerializeBase, ISerialize
	{
		public StringID name;

		public bool isMoney;

		public int creditNum;

		public StringID ticketKind;

		public int areaPercent;

		public int charaMagnification;

		public int maxCount;

		public string detail;

		public string filename;

		public StringID ticketEvent;

		public int priority;

		public string dataName;

		public TicketData()
		{
			name = new StringID();
			isMoney = false;
			creditNum = 0;
			ticketKind = new StringID();
			areaPercent = 0;
			charaMagnification = 0;
			maxCount = 0;
			detail = "";
			filename = "";
			ticketEvent = new StringID();
			priority = 0;
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.TicketData(TicketData sz)
		{
			Manager.MaiStudio.TicketData ticketData = new Manager.MaiStudio.TicketData();
			ticketData.Init(sz);
			return ticketData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
			ticketKind.AddPath(parentPath);
			ticketEvent.AddPath(parentPath);
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
			return false;
		}
	}
}
