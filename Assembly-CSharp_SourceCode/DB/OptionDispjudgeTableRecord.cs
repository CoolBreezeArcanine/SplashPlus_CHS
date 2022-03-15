namespace DB
{
	public class OptionDispjudgeTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public string NameEx;

		public string Detail;

		public string DetailEx;

		public string FilePath;

		public bool isDefault;

		public bool isCritical;

		public bool isFastlate;

		public OptionDispjudgeTableRecord()
		{
		}

		public OptionDispjudgeTableRecord(int EnumValue, string EnumName, string Name, string NameEx, string Detail, string DetailEx, string FilePath, int isDefault, int isCritical, int isFastlate)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.NameEx = NameEx;
			this.Detail = Detail;
			this.DetailEx = DetailEx;
			this.FilePath = FilePath;
			this.isDefault = isDefault != 0;
			this.isCritical = isCritical != 0;
			this.isFastlate = isFastlate != 0;
		}
	}
}
