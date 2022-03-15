namespace DB
{
	public class ErrorTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public bool isWarning;

		public bool isFirmUpdate;

		public int Code;

		public ErrorTableRecord()
		{
		}

		public ErrorTableRecord(int EnumValue, string EnumName, string Name, int isWarning, int isFirmUpdate, int Code)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.isWarning = isWarning != 0;
			this.isFirmUpdate = isFirmUpdate != 0;
			this.Code = Code;
		}
	}
}
