namespace DB
{
	public class PartyHeartBeatStateTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public PartyHeartBeatStateTableRecord()
		{
		}

		public PartyHeartBeatStateTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
