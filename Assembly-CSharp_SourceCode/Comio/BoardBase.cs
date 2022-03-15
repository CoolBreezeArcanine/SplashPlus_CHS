namespace Comio
{
	public abstract class BoardBase
	{
		public abstract BoardCtrlBase GetCtrlBase();

		public bool IsInitBoard()
		{
			return GetCtrlBase().IsInitBoard();
		}

		public bool CheckFirmVersion(byte boardVersion, byte fileVersion)
		{
			return GetCtrlBase().CheckFirmVersion(boardVersion, fileVersion);
		}

		public bool ReqHalt()
		{
			return GetCtrlBase().ReqHalt();
		}

		public bool IsHalted()
		{
			return GetCtrlBase().IsHalted();
		}

		public virtual void Reset()
		{
			GetCtrlBase().Reset();
		}

		public byte GetBoardNodeId()
		{
			return GetCtrlBase().GetBoardNodeId();
		}

		public bool IsBoardSpecInfoRecv()
		{
			return GetCtrlBase().IsBoardSpecInfoRecv();
		}

		public BoardSpecInfo GetBoardSpecInfo()
		{
			return GetCtrlBase().GetBoardSpecInfo();
		}
	}
}
