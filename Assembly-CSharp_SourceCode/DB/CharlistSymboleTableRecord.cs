namespace DB
{
	public class CharlistSymboleTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public CharlistSymboleTableRecord()
		{
		}

		public CharlistSymboleTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
