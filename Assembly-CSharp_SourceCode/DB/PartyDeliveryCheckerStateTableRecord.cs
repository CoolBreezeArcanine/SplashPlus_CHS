namespace DB
{
	public class PartyDeliveryCheckerStateTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public PartyDeliveryCheckerStateTableRecord()
		{
		}

		public PartyDeliveryCheckerStateTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
