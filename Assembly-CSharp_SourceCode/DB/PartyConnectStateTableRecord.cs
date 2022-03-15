namespace DB
{
	public class PartyConnectStateTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public PartyConnectStateTableRecord()
		{
		}

		public PartyConnectStateTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
