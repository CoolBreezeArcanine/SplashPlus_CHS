namespace DB
{
	public class OptionBodybrightnessTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public string Detail;

		public string FilePath;

		public bool isDefault;

		public float Value;

		public OptionBodybrightnessTableRecord()
		{
		}

		public OptionBodybrightnessTableRecord(int EnumValue, string EnumName, string Name, string Detail, string FilePath, int isDefault, float Value)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.Detail = Detail;
			this.FilePath = FilePath;
			this.isDefault = isDefault != 0;
			this.Value = Value;
		}
	}
}
