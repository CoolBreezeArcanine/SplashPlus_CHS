using Monitor.Entry;

namespace Process.Entry
{
	public abstract class EntrySubProcess
	{
		public bool IsFinish;

		protected EntryMonitor MainMonitor;

		protected EntryMonitor SubMonitor;

		protected EntryMonitor.Response InputResponse()
		{
			EntryMonitor.Response inputResponse = MainMonitor.InputResponse;
			if (inputResponse == EntryMonitor.Response.None && SubMonitor != null)
			{
				inputResponse = SubMonitor.InputResponse;
			}
			return inputResponse;
		}

		protected void ResetResponse()
		{
			MainMonitor.ResetResponse();
			SubMonitor?.ResetResponse();
		}

		public abstract void Execute();
	}
}
