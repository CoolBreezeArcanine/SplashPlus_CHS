namespace DB
{
	public class TestmodeDebugLedTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public TestmodeDebugLedTableRecord()
		{
		}

		public TestmodeDebugLedTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
