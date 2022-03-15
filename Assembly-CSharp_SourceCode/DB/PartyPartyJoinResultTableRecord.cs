namespace DB
{
	public class PartyPartyJoinResultTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public PartyPartyJoinResultTableRecord()
		{
		}

		public PartyPartyJoinResultTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
