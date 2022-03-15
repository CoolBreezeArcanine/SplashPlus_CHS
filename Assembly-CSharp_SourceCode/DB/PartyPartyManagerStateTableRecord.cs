namespace DB
{
	public class PartyPartyManagerStateTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public bool isNormal;

		public bool isHost;

		public bool isClient;

		public PartyPartyManagerStateTableRecord()
		{
		}

		public PartyPartyManagerStateTableRecord(int EnumValue, string EnumName, string Name, int isNormal, int isHost, int isClient)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.isNormal = isNormal != 0;
			this.isHost = isHost != 0;
			this.isClient = isClient != 0;
		}
	}
}
