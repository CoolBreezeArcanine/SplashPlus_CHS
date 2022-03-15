namespace DB
{
	public class OptionHeadphonevolumeTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public float Value;

		public string Name;

		public string NameEx;

		public string Detail;

		public string DetailEx;

		public bool isDefault;

		public OptionHeadphonevolumeTableRecord()
		{
		}

		public OptionHeadphonevolumeTableRecord(int EnumValue, string EnumName, float Value, string Name, string NameEx, string Detail, string DetailEx, int isDefault)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Value = Value;
			this.Name = Name;
			this.NameEx = NameEx;
			this.Detail = Detail;
			this.DetailEx = DetailEx;
			this.isDefault = isDefault != 0;
		}
	}
}
