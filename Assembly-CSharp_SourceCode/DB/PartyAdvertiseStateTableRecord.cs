namespace DB
{
	public class PartyAdvertiseStateTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public bool isNormal;

		public bool isGo;

		public PartyAdvertiseStateTableRecord()
		{
		}

		public PartyAdvertiseStateTableRecord(int EnumValue, string EnumName, string Name, int isNormal, int isGo)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.isNormal = isNormal != 0;
			this.isGo = isGo != 0;
		}
	}
}
