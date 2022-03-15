namespace DB
{
	public class WindowSizeTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public WindowSizeTableRecord()
		{
		}

		public WindowSizeTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
