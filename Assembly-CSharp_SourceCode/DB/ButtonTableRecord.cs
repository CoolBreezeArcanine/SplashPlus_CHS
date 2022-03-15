namespace DB
{
	public class ButtonTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public ButtonKindID Kind;

		public ButtonTypeID Type;

		public int Name;

		public ButtonTableRecord()
		{
		}

		public ButtonTableRecord(int EnumValue, string EnumName, int Kind, int Type, int Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Kind = (ButtonKindID)Kind;
			this.Type = (ButtonTypeID)Type;
			this.Name = Name;
		}
	}
}
