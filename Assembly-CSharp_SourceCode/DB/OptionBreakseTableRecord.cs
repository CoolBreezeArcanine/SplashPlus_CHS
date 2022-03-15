namespace DB
{
	public class OptionBreakseTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public string NameEx;

		public string Detail;

		public string DetailEx;

		public string SeGoodEnum;

		public string SeBadEnum;

		public bool isDefault;

		public string FilePath;

		public OptionBreakseTableRecord()
		{
		}

		public OptionBreakseTableRecord(int EnumValue, string EnumName, string Name, string NameEx, string Detail, string DetailEx, string SeGoodEnum, string SeBadEnum, int isDefault, string FilePath)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.NameEx = NameEx;
			this.Detail = Detail;
			this.DetailEx = DetailEx;
			this.SeGoodEnum = SeGoodEnum;
			this.SeBadEnum = SeBadEnum;
			this.isDefault = isDefault != 0;
			this.FilePath = FilePath;
		}
	}
}
