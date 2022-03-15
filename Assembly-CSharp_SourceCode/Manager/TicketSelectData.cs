namespace Manager
{
	public class TicketSelectData
	{
		public int ticketID;

		public string ticketName;

		public string ticketFileName;

		public string ticketFileName_S;

		public int creditNum;

		public int ticketNum;

		public int maxTiceketNum;

		public long expirationUnixTime;

		public int areaPercent;

		public int charaMagnification;

		public TicketKind ticketKind;

		public TicketSelectData()
		{
			ticketID = -1;
			ticketName = "";
			ticketFileName = "";
			ticketFileName_S = "";
			creditNum = -1;
			ticketNum = -1;
			maxTiceketNum = -1;
			expirationUnixTime = 0L;
			areaPercent = -1;
			charaMagnification = -1;
			ticketKind = TicketKind.None;
		}
	}
}
