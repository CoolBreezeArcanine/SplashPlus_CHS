namespace DB
{
	public class OptionCategoryTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public string NameEx;

		public uint MainColor;

		public string Filename;

		public OptionCategoryTableRecord()
		{
		}

		public OptionCategoryTableRecord(int EnumValue, string EnumName, string Name, string NameEx, uint MainColor, string Filename)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.NameEx = NameEx;
			this.MainColor = MainColor;
			this.Filename = Filename;
		}
	}
}
