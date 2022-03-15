using MAI2.Util;
using Manager;
using Monitor.Entry;
using Monitor.Entry.Parts;

namespace Process.Entry.State
{
	public class ConfirmGuest : StateEntry
	{
		private enum ProcMode
		{
			Confirm,
			Suspend
		}

		private readonly ProcEnum<ProcMode> _mode;

		public ConfirmGuest()
		{
			_mode = new ProcEnum<ProcMode>(base.ResetResponse);
		}

		public override void Init(params object[] args)
		{
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.EnableRead(flag: false);
			EntryMonitor entryMonitor = (EntryMonitor)args[0];
			entryMonitor.OpenScreen(ScreenType.ConfirmGuest);
			PartnerSatellite(entryMonitor).OpenScreen(ScreenType.DisplayPleaseWait);
			_mode.Mode = ProcMode.Confirm;
		}

		public override void Exec(float deltaTime)
		{
			if (_mode.Mode == ProcMode.Confirm)
			{
				switch (InputResponse())
				{
				case EntryMonitor.Response.Yes:
					SubProcesses.Add(new TryEntry(MainSatellite(), null, isFreedom: false, UserData.UserIDType.Guest, delegate(EntryMonitor m)
					{
						m.DecideEntry();
						m.OpenScreen(ScreenType.WaitPartner);
						Context.SetState(StateType.ConfirmEntry, true);
					}, delegate
					{
						Context.SetState(StateType.ConfirmEntry, true);
					}, delegate(EntryMonitor _, bool f)
					{
						Process.IsTimeCounting(!f);
					}));
					_mode.Mode = ProcMode.Suspend;
					break;
				case EntryMonitor.Response.No:
					Context.SetState(StateType.ConfirmEntry, true);
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
