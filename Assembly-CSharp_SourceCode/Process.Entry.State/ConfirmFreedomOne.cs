using Mai2.Voice_000001;
using Manager;
using Monitor.Entry;
using Monitor.Entry.Parts;

namespace Process.Entry.State
{
	public class ConfirmFreedomOne : StateEntry
	{
		private enum ProcMode
		{
			Select,
			Suspend
		}

		private readonly ProcEnum<ProcMode> _mode;

		private EntryMonitor _monitor;

		public ConfirmFreedomOne()
		{
			_mode = new ProcEnum<ProcMode>(delegate
			{
				_monitor?.ResetResponse();
			});
		}

		public override void Init(params object[] args)
		{
			_monitor = (EntryMonitor)args[0];
			_monitor.OpenScreen(ScreenType.ConfirmFreedom);
			PlayVoice(Cue.VO_000172, force: true);
			_mode.Mode = ProcMode.Select;
		}

		public override void Exec(float deltaTime)
		{
			if (_mode.Mode == ProcMode.Select)
			{
				switch (_monitor.InputResponse)
				{
				case EntryMonitor.Response.Yes:
					SubProcesses.Add(new TryEntry(_monitor, null, isFreedom: true, UserData.UserIDType.Exist, delegate(EntryMonitor m)
					{
						m.DecideEntryFreedom(m.MonitorIndex);
						m.OpenScreen(ScreenType.WaitPartner);
					}, delegate
					{
						Context.SetState(StateType.ConfirmContinueOne, -1, ConfirmContinueOne.InitType.Resume);
					}, delegate(EntryMonitor _, bool f)
					{
						Process.IsTimeCounting(!f);
					}));
					_mode.Mode = ProcMode.Suspend;
					break;
				case EntryMonitor.Response.No:
					Context.SetState(StateType.ConfirmContinueOne, -1, ConfirmContinueOne.InitType.Resume);
					break;
				}
			}
			base.Exec(deltaTime);
		}

		public override string ToString()
		{
			return base.ToString() + $" {_mode.Mode}";
		}

		public override int GetProcMode()
		{
			return (int)_mode.Mode;
		}
	}
}
