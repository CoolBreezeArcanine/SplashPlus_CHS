namespace DB
{
	public class VsghostnpcTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public VsghostnpcTableRecord()
		{
		}

		public VsghostnpcTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
