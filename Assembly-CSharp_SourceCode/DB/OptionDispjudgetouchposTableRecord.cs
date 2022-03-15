namespace DB
{
	public class OptionDispjudgetouchposTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public string NameEx;

		public string Detail;

		public string DetailEx;

		public string FilePath;

		public bool isDefault;

		public OptionDispjudgetouchposTableRecord()
		{
		}

		public OptionDispjudgetouchposTableRecord(int EnumValue, string EnumName, string Name, string NameEx, string Detail, string DetailEx, string FilePath, int isDefault)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.NameEx = NameEx;
			this.Detail = Detail;
			this.DetailEx = DetailEx;
			this.FilePath = FilePath;
			this.isDefault = isDefault != 0;
		}
	}
}
