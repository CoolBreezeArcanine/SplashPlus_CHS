namespace DB
{
	public class WindowKindTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public WindowKindTableRecord()
		{
		}

		public WindowKindTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
