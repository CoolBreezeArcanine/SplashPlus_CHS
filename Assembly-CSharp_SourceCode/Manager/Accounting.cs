using AMDaemon.Allnet;

namespace Manager
{
	public class Accounting
	{
		public enum KindCoe
		{
			Common = 0,
			Credit = 100,
			Freedom = 101,
			Ticket = 102
		}

		public enum StatusCode
		{
			PayToPlay_Start = 0,
			PayToPlay_End = 1,
			FreeToPlay_Start = 4,
			FreeToPlay_End = 5,
			FreedomPlay_Start = 0,
			FreedomPlay_End = 1,
			BuyTicket_Start = 0,
			BuyTicket_End = 1
		}

		private AccountingUnit[] _units;

		private AccountingHandle[] _storeHandles;

		private int _maxPlayerCount;

		public void initialize()
		{
			_maxPlayerCount = AMDaemon.Allnet.Accounting.PlayerCount;
			_units = new AccountingUnit[_maxPlayerCount];
			_storeHandles = new AccountingHandle[_maxPlayerCount];
			for (int i = 0; i < _maxPlayerCount; i++)
			{
				_units[i] = AMDaemon.Allnet.Accounting.Players[i];
				_storeHandles[i] = AccountingHandle.Zero;
			}
		}

		public void execute()
		{
		}

		public void terminate()
		{
		}

		public void BeginPlay(int unit, KindCoe kind, StatusCode status)
		{
			if (AMDaemon.Allnet.Accounting.IsAvailable && _units[unit] != null)
			{
				_storeHandles[unit] = _units[unit].BeginPlay((int)kind, (int)status);
			}
		}

		public bool EndPlay(int unit, KindCoe kind, StatusCode status, int count)
		{
			bool result = false;
			if (AMDaemon.Allnet.Accounting.IsAvailable && _units[unit] != null && _storeHandles[unit].IsValid)
			{
				result = _units[unit].EndPlay(_storeHandles[unit], (int)kind, (int)status, count);
				_storeHandles[unit] = AccountingHandle.Zero;
			}
			return result;
		}

		public bool AccountItem(int unit, KindCoe kind, StatusCode status, int count)
		{
			bool result = false;
			if (AMDaemon.Allnet.Accounting.IsAvailable && _units[unit] != null && _storeHandles[unit].IsValid)
			{
				result = _units[unit].AccountItem((int)kind, (int)status, count);
			}
			return result;
		}

		public bool PutGeneralId(int unit, KindCoe kind, string data)
		{
			bool result = false;
			if (AMDaemon.Allnet.Accounting.IsAvailable && _units[unit] != null && _storeHandles[unit].IsValid)
			{
				result = _units[unit].PutGeneralId((int)kind, data);
			}
			return result;
		}

		public bool isReporting()
		{
			if (AMDaemon.Allnet.Accounting.IsAvailable)
			{
				return AMDaemon.Allnet.Accounting.IsReporting;
			}
			return false;
		}
	}
}
