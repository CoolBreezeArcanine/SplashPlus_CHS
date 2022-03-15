namespace DB
{
	public class OptionGametapTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public string NameEx;

		public string Detail;

		public string DetailEx;

		public bool isDefault;

		public string FilePath;

		public OptionGametapTableRecord()
		{
		}

		public OptionGametapTableRecord(int EnumValue, string EnumName, string Name, string NameEx, string Detail, string DetailEx, int isDefault, string FilePath)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.NameEx = NameEx;
			this.Detail = Detail;
			this.DetailEx = DetailEx;
			this.isDefault = isDefault != 0;
			this.FilePath = FilePath;
		}
	}
}
