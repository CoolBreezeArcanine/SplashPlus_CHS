namespace DB
{
	public class NgwordExTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public NgwordExTableRecord()
		{
		}

		public NgwordExTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
