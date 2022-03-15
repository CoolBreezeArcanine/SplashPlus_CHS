namespace DB
{
	public class PartySettingHostStateTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public bool isNormal;

		public bool isError;

		public PartySettingHostStateTableRecord()
		{
		}

		public PartySettingHostStateTableRecord(int EnumValue, string EnumName, string Name, int isNormal, int isError)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.isNormal = isNormal != 0;
			this.isError = isError != 0;
		}
	}
}
