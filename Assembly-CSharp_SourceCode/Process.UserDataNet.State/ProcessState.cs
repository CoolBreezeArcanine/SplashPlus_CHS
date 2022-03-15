using Process.Entry.State;
using Process.MapCore;

namespace Process.UserDataNet.State
{
	public abstract class ProcessState<TProcess> : global::Process.Entry.State.State where TProcess : MapProcess
	{
		protected TProcess Process;

		protected new ProcessContext<TProcess> Context;

		public void Setup(ProcessContext<TProcess> context, TProcess process)
		{
			Process = process;
			Context = context;
		}

		public override void Exec(float deltaTime)
		{
		}

		public override void Term()
		{
		}
	}
}
