namespace DB
{
	public class CharlistTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public CharlistTableRecord()
		{
		}

		public CharlistTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
