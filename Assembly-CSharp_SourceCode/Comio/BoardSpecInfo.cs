namespace Comio
{
	public class BoardSpecInfo
	{
		public BoardNo BoardNo = new BoardNo();

		public FirmInfo FirmInfo = new FirmInfo();

		public void Clear()
		{
			BoardNo.Clear();
			FirmInfo.Clear();
		}

		public void Dump()
		{
			ComioLog.Log(string.Concat(string.Concat("" + "BoardSpecInfo.dump\n", " boardNo      : ", BoardNo.Text, "\n"), " firmRev      : ", FirmInfo.Revision.ToString(), "\n"));
		}
	}
}
