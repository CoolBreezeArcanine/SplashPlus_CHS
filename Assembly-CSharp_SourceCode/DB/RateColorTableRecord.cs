namespace DB
{
	public class RateColorTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public int Rate;

		public RateColorTableRecord()
		{
		}

		public RateColorTableRecord(int EnumValue, string EnumName, string Name, int Rate)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.Rate = Rate;
		}
	}
}
