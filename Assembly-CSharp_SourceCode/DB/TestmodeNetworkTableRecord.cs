namespace DB
{
	public class TestmodeNetworkTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public string NameEx;

		public TestmodeNetworkTableRecord()
		{
		}

		public TestmodeNetworkTableRecord(int EnumValue, string EnumName, string Name, string NameEx)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.NameEx = NameEx;
		}
	}
}
