namespace DB
{
	public class TestmodeDebugEventsetTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public TestmodeDebugEventsetTableRecord()
		{
		}

		public TestmodeDebugEventsetTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
