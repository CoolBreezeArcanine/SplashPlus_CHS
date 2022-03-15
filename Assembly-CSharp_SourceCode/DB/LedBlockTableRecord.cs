namespace DB
{
	public class LedBlockTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public int LedbdID;

		public int Playerindex;

		public bool isJvs;

		public bool isFet;

		public LedBlockTableRecord()
		{
		}

		public LedBlockTableRecord(int EnumValue, string EnumName, string Name, int LedbdID, int Playerindex, int isJvs, int isFet)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.LedbdID = LedbdID;
			this.Playerindex = Playerindex;
			this.isJvs = isJvs != 0;
			this.isFet = isFet != 0;
		}
	}
}
