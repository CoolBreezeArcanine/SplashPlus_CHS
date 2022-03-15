namespace DB
{
	public class MachineGroupTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public MachineGroupTableRecord()
		{
		}

		public MachineGroupTableRecord(int EnumValue, string EnumName, string Name)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
		}
	}
}
