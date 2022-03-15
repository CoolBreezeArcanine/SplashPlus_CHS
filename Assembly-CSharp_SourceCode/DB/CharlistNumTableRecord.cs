namespace DB
{
	public class CharlistNumTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public CharlistNumTableRecord()
		{
		}

		public CharlistNumTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
