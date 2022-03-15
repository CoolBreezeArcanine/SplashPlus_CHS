namespace DB
{
	public class SystemInitializeTextTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public SystemInitializeTextTableRecord()
		{
		}

		public SystemInitializeTextTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
