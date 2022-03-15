namespace DB
{
	public class OptionRootTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public OptionRootTableRecord()
		{
		}

		public OptionRootTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
