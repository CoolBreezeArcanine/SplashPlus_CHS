using MAI2.Util;
using UnityEngine;

namespace Manager.Operation
{
	public class ClosingTimer
	{
		public const int ShowNoticeMinutes = 60;

		public const int CloseMinutes = 15;

		private int _remainingMinutes;

		public ClosingTimer()
		{
			Initialize();
		}

		public void Initialize()
		{
			_remainingMinutes = int.MaxValue;
		}

		public void Execute()
		{
			_remainingMinutes = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.closeSetting.getRemainingMinutes();
		}

		public void Terminate()
		{
		}

		public int GetRemainingMinutes()
		{
			return Mathf.Max(0, _remainingMinutes - 15);
		}

		public bool IsShowRemainingMinutes()
		{
			return _remainingMinutes <= 60;
		}

		public bool IsClosed()
		{
			return _remainingMinutes <= 15;
		}

		public bool IsCoinAcceptable()
		{
			return !IsClosed();
		}
	}
}
