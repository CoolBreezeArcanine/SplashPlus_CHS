namespace DB
{
	public class PartySettingErrorTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public PartySettingErrorTableRecord()
		{
		}

		public PartySettingErrorTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
