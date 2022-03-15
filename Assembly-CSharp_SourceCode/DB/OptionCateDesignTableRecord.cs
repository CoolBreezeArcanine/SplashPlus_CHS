namespace DB
{
	public class OptionCateDesignTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public string NameEx;

		public string Detail;

		public string DetailEx;

		public OptionCateDesignTableRecord()
		{
		}

		public OptionCateDesignTableRecord(int EnumValue, string EnumName, string Name, string NameEx, string Detail, string DetailEx)
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
