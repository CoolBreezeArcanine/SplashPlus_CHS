using System.Collections.Generic;
using Monitor.Entry;

namespace Process.Entry.State
{
	public class ContextEntry : Context
	{
		protected EntryProcess Process;

		protected List<EntryMonitor> Monitors;

		public ContextEntry(EntryProcess process, EntryMonitor monitor0, EntryMonitor monitor1)
		{
			Process = process;
			Monitors = new List<EntryMonitor> { monitor0, monitor1 };
		}

		public void AddState(StateType key, StateEntry state)
		{
			AddState((int)key, state);
			state.Setup(this, Process, Monitors[0], Monitors[1]);
		}

		public void SetState(StateType key, params object[] args)
		{
			SetState((int)key, args);
		}

		public void GetCurrentState(out StateType type, out int mode)
		{
			GetCurrentState(out int key, out State state);
			StateEntry stateEntry = state as StateEntry;
			type = (StateType)key;
			mode = stateEntry?.GetProcMode() ?? (-1);
		}
	}
}
