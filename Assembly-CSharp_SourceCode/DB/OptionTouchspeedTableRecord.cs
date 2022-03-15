namespace DB
{
	public class OptionTouchspeedTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public float Value;

		public string Name;

		public string NameEx;

		public string Detail;

		public string DetailEx;

		public string FilePath;

		public bool isDefault;

		public OptionTouchspeedTableRecord()
		{
		}

		public OptionTouchspeedTableRecord(int EnumValue, string EnumName, float Value, string Name, string NameEx, string Detail, string DetailEx, string FilePath, int isDefault)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Value = Value;
			this.Name = Name;
			this.NameEx = NameEx;
			this.Detail = Detail;
			this.DetailEx = DetailEx;
			this.FilePath = FilePath;
			this.isDefault = isDefault != 0;
		}
	}
}
