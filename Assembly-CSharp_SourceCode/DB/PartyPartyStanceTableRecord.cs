namespace DB
{
	public class PartyPartyStanceTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public PartyPartyStanceTableRecord()
		{
		}

		public PartyPartyStanceTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
