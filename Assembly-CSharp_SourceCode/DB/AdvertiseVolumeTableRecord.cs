namespace DB
{
	public class AdvertiseVolumeTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public int Volume;

		public AdvertiseVolumeTableRecord()
		{
		}

		public AdvertiseVolumeTableRecord(int EnumValue, string EnumName, string Name, int Volume)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.Volume = Volume;
		}
	}
}
