namespace DB
{
	public class PartyPartyHostStateTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public bool isNormal;

		public bool isWorking;

		public bool isRecruit;

		public bool isPlay;

		public PartyPartyHostStateTableRecord()
		{
		}

		public PartyPartyHostStateTableRecord(int EnumValue, string EnumName, string Name, int isNormal, int isWorking, int isRecruit, int isPlay)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.isNormal = isNormal != 0;
			this.isWorking = isWorking != 0;
			this.isRecruit = isRecruit != 0;
			this.isPlay = isPlay != 0;
		}
	}
}
