using Process.Entry.State;
using Process.MapCore;

namespace Process.UserDataNet.State
{
	public class ProcessContext<TProcess> : Context where TProcess : MapProcess
	{
		protected TProcess Process;

		public ProcessContext(TProcess process)
		{
			Process = process;
		}

		public void AddState(int key, ProcessState<TProcess> state)
		{
			AddState(key, (global::Process.Entry.State.State)state);
			state.Setup(this, Process);
		}
	}
}
