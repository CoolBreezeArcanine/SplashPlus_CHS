namespace DB
{
	public class CharlistAbcSmallTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public CharlistAbcSmallTableRecord()
		{
		}

		public CharlistAbcSmallTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
