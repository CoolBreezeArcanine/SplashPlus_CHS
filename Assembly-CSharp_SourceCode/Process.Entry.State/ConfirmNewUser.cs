using MAI2.Util;
using Manager;
using Monitor.Entry;
using Monitor.Entry.Parts;

namespace Process.Entry.State
{
	public class ConfirmNewUser : StateEntry
	{
		private enum ProcMode
		{
			Confirm,
			Entry,
			Suspend
		}

		private readonly ProcEnum<ProcMode> _mode;

		private ulong _userId;

		private string _accessCode;

		private string _offlineId;

		private EntryMonitor _monitor;

		public ConfirmNewUser()
		{
			_mode = new ProcEnum<ProcMode>(base.ResetResponse);
		}

		public override void Init(params object[] args)
		{
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.EnableRead(flag: false);
			_userId = (ulong)args[0];
			_accessCode = (string)args[1];
			_offlineId = (string)args[2];
			_monitor = (EntryMonitor)args[3];
			if (_monitor == null)
			{
				OpenMonitorScreens(ScreenType.ConfirmNewUser);
			}
			else
			{
				_monitor.OpenScreen(ScreenType.ConfirmNewUser);
				PartnerSatellite(_monitor).OpenScreen(ScreenType.DisplayPleaseWait);
			}
			_mode.Mode = ProcMode.Confirm;
		}

		public override void Exec(float deltaTime)
		{
			switch (_mode.Mode)
			{
			case ProcMode.Confirm:
				switch (InputResponse())
				{
				case EntryMonitor.Response.Yes:
					_monitor = MainSatellite();
					_mode.Mode = ProcMode.Entry;
					break;
				case EntryMonitor.Response.No:
					Context.SetState(StateType.ConfirmEntry, true);
					break;
				}
				break;
			case ProcMode.Entry:
				_monitor.SetNewUserData(_userId, _accessCode, _offlineId);
				SubProcesses.Add(new TryEntry(_monitor, null, isFreedom: false, UserData.UserIDType.New, delegate(EntryMonitor m)
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
			}
			base.Exec(deltaTime);
		}

		public override string ToString()
		{
			return base.ToString() + $" {_mode.Mode}";
		}
	}
}
