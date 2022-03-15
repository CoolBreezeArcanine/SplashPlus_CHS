namespace DB
{
	public class WindowPositionTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public WindowPositionTableRecord()
		{
		}

		public WindowPositionTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
