namespace DB
{
	public class PlayComboflagTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public PlayComboflagTableRecord()
		{
		}

		public PlayComboflagTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
