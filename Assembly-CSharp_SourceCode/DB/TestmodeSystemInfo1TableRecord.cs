namespace DB
{
	public class TestmodeSystemInfo1TableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public string NameEx;

		public TestmodeSystemInfo1TableRecord()
		{
		}

		public TestmodeSystemInfo1TableRecord(int EnumValue, string EnumName, string Name, string NameEx)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.NameEx = NameEx;
		}
	}
}
