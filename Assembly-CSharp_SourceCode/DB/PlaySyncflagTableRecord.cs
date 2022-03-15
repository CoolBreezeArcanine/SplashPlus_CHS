namespace DB
{
	public class PlaySyncflagTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public int Point;

		public PlaySyncflagTableRecord()
		{
		}

		public PlaySyncflagTableRecord(int EnumValue, string EnumName, string Name, int Point)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.Point = Point;
		}
	}
}
