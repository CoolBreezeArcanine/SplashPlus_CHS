namespace DB
{
	public class ContentBitTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public bool isGuestIgnore;

		public ContentBitTableRecord()
		{
		}

		public ContentBitTableRecord(int EnumValue, string EnumName, string Name, int isGuestIgnore)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.isGuestIgnore = isGuestIgnore != 0;
		}
	}
}
