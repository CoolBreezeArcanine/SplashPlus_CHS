namespace DB
{
	public class PartyPartyClientStateTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public bool isConnect;

		public bool isNormal;

		public bool isRequest;

		public bool isWaitPlay;

		public PartyPartyClientStateTableRecord()
		{
		}

		public PartyPartyClientStateTableRecord(int EnumValue, string EnumName, string Name, int isConnect, int isNormal, int isRequest, int isWaitPlay)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.isConnect = isConnect != 0;
			this.isNormal = isNormal != 0;
			this.isRequest = isRequest != 0;
			this.isWaitPlay = isWaitPlay != 0;
		}
	}
}
