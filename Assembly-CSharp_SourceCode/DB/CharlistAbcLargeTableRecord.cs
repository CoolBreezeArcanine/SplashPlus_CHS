namespace DB
{
	public class CharlistAbcLargeTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public CharlistAbcLargeTableRecord()
		{
		}

		public CharlistAbcLargeTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
