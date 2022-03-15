using DB;

namespace Process
{
	public interface IGeneric
	{
		void Enqueue(int monitorId, WindowMessageID messageId);

		void Enqueue(int monitorId, WindowMessageID messageId, WindowPositionID positionId);

		void EnqueueWarning(WarningWindowInfo info);

		void AllInterrupt();

		void AllInterrupt(int monitorId);

		void WarningInterrupt(int monitorId);

		bool IsOpening(int monitorId, WindowPositionID positionId);

		bool IsOpeningWarningWindow(int monitorId);

		void Close(int monitorId);

		void Close(int monitorId, WindowPositionID positionId);

		void CloseWarningWindow(int monitorId);

		void ForcedClose(int monitorId);
	}
}
