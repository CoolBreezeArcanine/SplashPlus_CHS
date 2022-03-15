namespace DB
{
	public class OptionSlidesizeTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public string NameEx;

		public string Detail;

		public string DetailEx;

		public string FilePath;

		public bool isDefault;

		public float Value;

		public OptionSlidesizeTableRecord()
		{
		}

		public OptionSlidesizeTableRecord(int EnumValue, string EnumName, string Name, string NameEx, string Detail, string DetailEx, string FilePath, int isDefault, float Value)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.NameEx = NameEx;
			this.Detail = Detail;
			this.DetailEx = DetailEx;
			this.FilePath = FilePath;
			this.isDefault = isDefault != 0;
			this.Value = Value;
		}
	}
}
