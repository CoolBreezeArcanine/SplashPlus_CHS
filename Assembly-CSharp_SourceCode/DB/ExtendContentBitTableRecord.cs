namespace DB
{
	public class ExtendContentBitTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public ExtendContentBitTableRecord()
		{
		}

		public ExtendContentBitTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
