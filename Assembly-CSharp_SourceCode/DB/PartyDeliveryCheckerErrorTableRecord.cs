namespace DB
{
	public class PartyDeliveryCheckerErrorTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public PartyDeliveryCheckerErrorTableRecord()
		{
		}

		public PartyDeliveryCheckerErrorTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
