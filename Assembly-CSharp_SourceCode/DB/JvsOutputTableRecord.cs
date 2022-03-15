namespace DB
{
	public class JvsOutputTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public string OutputIDName;

		public JvsOutputTableRecord()
		{
		}

		public JvsOutputTableRecord(int EnumValue, string EnumName, string Name, string OutputIDName)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.OutputIDName = OutputIDName;
		}
	}
}
