namespace DB
{
	public class OptionCateSpeedTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public string NameEx;

		public string Detail;

		public string DetailEx;

		public OptionCateSpeedTableRecord()
		{
		}

		public OptionCateSpeedTableRecord(int EnumValue, string EnumName, string Name, string NameEx, string Detail, string DetailEx)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.NameEx = NameEx;
			this.Detail = Detail;
			this.DetailEx = DetailEx;
		}
	}
}
