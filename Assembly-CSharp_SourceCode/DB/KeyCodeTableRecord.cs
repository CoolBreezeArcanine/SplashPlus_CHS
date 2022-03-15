namespace DB
{
	public class KeyCodeTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public int Value;

		public KeyCodeTableRecord()
		{
		}

		public KeyCodeTableRecord(int EnumValue, string EnumName, string Name, int Value)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.Value = Value;
		}
	}
}
