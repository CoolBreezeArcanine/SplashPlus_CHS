using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class TicketEvent : AccessorBase
	{
		public StringID eventId { get; private set; }

		public TicketEvent()
		{
			eventId = new StringID();
		}

		public void Init(Manager.MaiStudio.Serialize.TicketEvent sz)
		{
			eventId = (StringID)sz.eventId;
		}
	}
}
