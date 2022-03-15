namespace DB
{
	public class ButtonPosTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public ButtonPosTableRecord()
		{
		}

		public ButtonPosTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
