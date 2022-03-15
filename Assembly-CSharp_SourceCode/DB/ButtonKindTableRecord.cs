namespace DB
{
	public class ButtonKindTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public ButtonKindTableRecord()
		{
		}

		public ButtonKindTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
