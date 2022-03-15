namespace Manager
{
	public class TicketConnectData
	{
		public bool isConnectFinish;

		public bool isError;

		public int connectTicketId;

		public int credit;

		public TicketConnectData()
		{
			isConnectFinish = false;
			isError = false;
			connectTicketId = -1;
			credit = -1;
		}
	}
}
