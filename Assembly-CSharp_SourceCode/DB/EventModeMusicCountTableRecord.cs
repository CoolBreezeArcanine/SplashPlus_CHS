namespace DB
{
	public class EventModeMusicCountTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public int Track;

		public EventModeMusicCountTableRecord()
		{
		}

		public EventModeMusicCountTableRecord(int EnumValue, string EnumName, string Name, int Track)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.Track = Track;
		}
	}
}
