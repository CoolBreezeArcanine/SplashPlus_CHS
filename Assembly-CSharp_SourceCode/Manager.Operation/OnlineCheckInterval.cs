using System;
using System.Diagnostics;

namespace Manager.Operation
{
	public class OnlineCheckInterval
	{
		private TimeSpan _retryInterval;

		private readonly Stopwatch _counter;

		public int RetryCount { get; private set; }

		public OnlineCheckInterval()
		{
			RetryCount = 0;
			_retryInterval = TimeSpan.Zero;
			_counter = new Stopwatch();
		}

		public void Reset()
		{
			RetryCount = 0;
			_retryInterval = TimeSpan.Zero;
			_counter.Reset();
			_counter.Stop();
		}

		public bool IsNeedCheck()
		{
			return _counter.Elapsed > _retryInterval;
		}

		public void NotifyOnline()
		{
			RetryCount = 0;
			_retryInterval = new TimeSpan(0, 30, 0);
			_counter.Reset();
			_counter.Start();
		}

		public void NotifyOffline()
		{
			if (RetryCount == 0)
			{
				_retryInterval = TimeSpan.Zero;
			}
			else if (RetryCount <= 7)
			{
				_retryInterval = new TimeSpan(0, 0, 10);
			}
			else if (RetryCount <= 66)
			{
				_retryInterval = new TimeSpan(0, 1, 0);
			}
			else
			{
				_retryInterval = new TimeSpan(0, 10, 0);
			}
			RetryCount++;
			_counter.Reset();
			_counter.Start();
		}
	}
}
