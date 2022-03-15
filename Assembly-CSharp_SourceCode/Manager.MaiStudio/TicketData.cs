using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class TicketData : AccessorBase
	{
		public StringID name { get; private set; }

		public bool isMoney { get; private set; }

		public int creditNum { get; private set; }

		public StringID ticketKind { get; private set; }

		public int areaPercent { get; private set; }

		public int charaMagnification { get; private set; }

		public int maxCount { get; private set; }

		public string detail { get; private set; }

		public string filename { get; private set; }

		public StringID ticketEvent { get; private set; }

		public int priority { get; private set; }

		public string dataName { get; private set; }

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

		public void Init(Manager.MaiStudio.Serialize.TicketData sz)
		{
			name = (StringID)sz.name;
			isMoney = sz.isMoney;
			creditNum = sz.creditNum;
			ticketKind = (StringID)sz.ticketKind;
			areaPercent = sz.areaPercent;
			charaMagnification = sz.charaMagnification;
			maxCount = sz.maxCount;
			detail = sz.detail;
			filename = sz.filename;
			ticketEvent = (StringID)sz.ticketEvent;
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
			return false;
		}
	}
}
