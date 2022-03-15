namespace DB
{
	public class NgwordJpTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public NgwordJpTableRecord()
		{
		}

		public NgwordJpTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
