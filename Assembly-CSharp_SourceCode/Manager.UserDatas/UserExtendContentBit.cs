using DB;

namespace Manager.UserDatas
{
	public class UserExtendContentBit
	{
		private ulong _contentBit;

		public void Clear()
		{
			_contentBit = 0uL;
		}

		public bool IsFlagOn(ExtendContentBitID bit)
		{
			if ((_contentBit & (ulong)(1L << (int)bit)) == 0L)
			{
				return false;
			}
			return true;
		}

		public void SetFlag(ExtendContentBitID bit, bool flag)
		{
			if (flag)
			{
				_contentBit |= (ulong)(1L << (int)bit);
			}
			else
			{
				_contentBit &= (ulong)(~(1L << (int)bit));
			}
		}
	}
}
