namespace DB
{
	public class OptionCateGameTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public string NameEx;

		public string Detail;

		public string DetailEx;

		public OptionCateGameTableRecord()
		{
		}

		public OptionCateGameTableRecord(int EnumValue, string EnumName, string Name, string NameEx, string Detail, string DetailEx)
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
