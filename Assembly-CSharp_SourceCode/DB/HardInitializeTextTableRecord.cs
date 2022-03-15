namespace DB
{
	public class HardInitializeTextTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public HardInitializeTextTableRecord()
		{
		}

		public HardInitializeTextTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
