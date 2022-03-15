namespace Mecha
{
	public class BoardInfo
	{
		public string BoardNo;

		public byte FirmRev;

		public BoardInfo()
		{
			Clear();
		}

		public void Clear()
		{
			BoardNo = "";
			FirmRev = 0;
		}
	}
}
