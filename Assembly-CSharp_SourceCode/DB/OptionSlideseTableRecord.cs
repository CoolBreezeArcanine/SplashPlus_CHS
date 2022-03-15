namespace DB
{
	public class OptionSlideseTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public string NameEx;

		public string Detail;

		public string DetailEx;

		public string SeEnum;

		public bool isDefault;

		public string FilePath;

		public OptionSlideseTableRecord()
		{
		}

		public OptionSlideseTableRecord(int EnumValue, string EnumName, string Name, string NameEx, string Detail, string DetailEx, string SeEnum, int isDefault, string FilePath)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.NameEx = NameEx;
			this.Detail = Detail;
			this.DetailEx = DetailEx;
			this.SeEnum = SeEnum;
			this.isDefault = isDefault != 0;
			this.FilePath = FilePath;
		}
	}
}
