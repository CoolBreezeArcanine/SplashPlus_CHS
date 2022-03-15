namespace DB
{
	public class TestmodeEmoneyTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public string NameEx;

		public TestmodeEmoneyTableRecord()
		{
		}

		public TestmodeEmoneyTableRecord(int EnumValue, string EnumName, string Name, string NameEx)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.NameEx = NameEx;
		}
	}
}
