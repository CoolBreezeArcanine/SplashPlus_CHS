namespace DB
{
	public class RatingTableTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public int Achive;

		public int Offset;

		public RatingTableTableRecord()
		{
		}

		public RatingTableTableRecord(int EnumValue, string EnumName, string Name, int Achive, int Offset)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.Achive = Achive;
			this.Offset = Offset;
		}
	}
}
