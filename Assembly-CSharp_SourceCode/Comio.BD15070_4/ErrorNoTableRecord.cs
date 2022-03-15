namespace Comio.BD15070_4
{
	public class ErrorNoTableRecord
	{
		public int EnumValue;

		public string String;

		public bool IsNeedFirmUpdate;

		public ErrorNoTableRecord()
		{
		}

		public ErrorNoTableRecord(int enumValue, string String, bool isNeedFirmUpdate)
		{
			EnumValue = enumValue;
			this.String = String;
			IsNeedFirmUpdate = isNeedFirmUpdate;
		}
	}
}
