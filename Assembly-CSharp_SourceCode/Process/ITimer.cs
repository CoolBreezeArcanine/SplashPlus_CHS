using System;

namespace Process
{
	public interface ITimer
	{
		void PrepareTimer(int second, int delayCount, bool isEntryProcess, Action both, bool isVisible);

		void SetCompleteOneSide(Action<int> single);

		void DecrementTime(int monitorId, int decrementValue);

		void IsFastSkip(int monitorId, bool isFast);

		void IsLongSkip(int monitorId, bool isLongSkip);

		bool IsTimeUp(int monitorId);

		void ForceTimeUp();

		void ForceTimeUp(int monitorId);

		void IsTimeCounting(bool isTimeCount);

		void IsTimeCounting(int monitorId, bool isTimeCount);

		void SetVisibleTimer(int monitorId, bool isVisible);

		void SetVisibleTimer(bool isVisible);

		void SetTimerSecurity(int second, int delayCount, Action both);

		bool CheckZombiAction(ProcessBase process);

		void ClearTimeoutAction();
	}
}
