using UnityEngine;

namespace Manager.Operation
{
	public class SegaBootTimer
	{
		public int _segaBootTrigetSec;

		private int _secSegaBoot;

		private bool _afterSegaBootTimeNow;

		private bool _afterSegaBootTimeOld;

		private bool _needSegaBoot;

		public const int ShowNoticeMin = 15;

		private int RemainingMinutes => (_secSegaBoot + 59) / 60;

		public SegaBootTimer()
		{
			_segaBootTrigetSec = 0;
			_secSegaBoot = 2147483587;
			_needSegaBoot = false;
		}

		public void Execute()
		{
			if (0 < _segaBootTrigetSec)
			{
				float realtimeSinceStartup = Time.realtimeSinceStartup;
				_needSegaBoot = realtimeSinceStartup > (float)_segaBootTrigetSec;
				_secSegaBoot = Mathf.Max(0, (int)((float)_segaBootTrigetSec - realtimeSinceStartup));
			}
		}

		public void Terminate()
		{
		}

		public bool IsSegaBootTime()
		{
			return _secSegaBoot == 0;
		}

		public int GetGotoSegaBootSec()
		{
			return _secSegaBoot;
		}

		public bool IsShowRemainingMinutes()
		{
			return RemainingMinutes <= 15;
		}

		public bool IsSegaBootNeeded()
		{
			return _needSegaBoot;
		}

		public int GetRemainingMinutes()
		{
			return Mathf.Max(0, RemainingMinutes);
		}

		public bool IsCoinAcceptable()
		{
			return !_needSegaBoot;
		}

		public void SetRebootInterval(int sec)
		{
			_segaBootTrigetSec = sec;
		}
	}
}
