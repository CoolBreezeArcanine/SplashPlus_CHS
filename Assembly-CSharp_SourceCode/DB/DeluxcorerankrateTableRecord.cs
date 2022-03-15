namespace DB
{
	public class DeluxcorerankrateTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public int Achieve;

		public DeluxcorerankrateTableRecord()
		{
		}

		public DeluxcorerankrateTableRecord(int EnumValue, string EnumName, string Name, int Achieve)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.Achieve = Achieve;
		}
	}
}
