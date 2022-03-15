using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class TicketEvent : SerializeBase
	{
		public StringID eventId;

		public TicketEvent()
		{
			eventId = new StringID();
		}

		public static explicit operator Manager.MaiStudio.TicketEvent(TicketEvent sz)
		{
			Manager.MaiStudio.TicketEvent ticketEvent = new Manager.MaiStudio.TicketEvent();
			ticketEvent.Init(sz);
			return ticketEvent;
		}

		public override void AddPath(string parentPath)
		{
			eventId.AddPath(parentPath);
		}
	}
}
