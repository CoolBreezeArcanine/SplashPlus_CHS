namespace DB
{
	public class MaintenanceInfoTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public MaintenanceInfoTableRecord()
		{
		}

		public MaintenanceInfoTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
