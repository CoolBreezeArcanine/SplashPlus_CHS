namespace DB
{
	public class PartySettingClientStateTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public bool isNormal;

		public bool isError;

		public bool isBusy;

		public PartySettingClientStateTableRecord()
		{
		}

		public PartySettingClientStateTableRecord(int EnumValue, string EnumName, string Name, int isNormal, int isError, int isBusy)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.isNormal = isNormal != 0;
			this.isError = isError != 0;
			this.isBusy = isBusy != 0;
		}
	}
}
