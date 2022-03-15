namespace DB
{
	public class ButtonTypeTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public ButtonTypeTableRecord()
		{
		}

		public ButtonTypeTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
