namespace Comio.BD15070_4
{
	public class BoardStatus
	{
		public byte TimeoutStat;

		public byte TimeoutSec;

		public byte PwmIo;

		public byte FetTimeout;

		public void Clear()
		{
			TimeoutStat = 0;
			TimeoutSec = 0;
			PwmIo = 0;
			FetTimeout = 0;
		}
	}
}
