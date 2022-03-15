using MAI2.Util;
using Manager;
using Monitor.Entry;
using Monitor.Entry.Parts;

namespace Process.Entry.State
{
	public class ConfirmAccessCode : StateEntry
	{
		private enum ProcMode
		{
			Setup,
			WaitResponse,
			Idle
		}

		private readonly ProcEnum<ProcMode> _mode;

		private TryAime _tryAime;

		private EntryMonitor _monitor;

		public ConfirmAccessCode()
		{
			_mode = new ProcEnum<ProcMode>(base.ResetResponse);
		}

		public override void Init(params object[] args)
		{
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.EnableRead(flag: true);
			_monitor = (EntryMonitor)args[0];
			_monitor.OpenScreen(ScreenType.ConfirmAccessCode);
			PartnerSatellite(_monitor).OpenScreen(ScreenType.DisplayPleaseWait);
			_mode.Mode = ProcMode.Setup;
		}

		public override void Exec(float deltaTime)
		{
			switch (_mode.Mode)
			{
			case ProcMode.Setup:
				_tryAime = new TryAime(_monitor, null, delegate(EntryMonitor m)
				{
					m.OpenScreen(ScreenType.DisplayAccessCodeQR, SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.GetAccessCode());
					_mode.Mode = ProcMode.WaitResponse;
				}, delegate(EntryMonitor m)
				{
					m.OpenScreen(ScreenType.ErrorAccessCode);
					_mode.Mode = ProcMode.WaitResponse;
				}, delegate(EntryMonitor m)
				{
					m.OpenScreen(ScreenType.ErrorAccessCode);
					_mode.Mode = ProcMode.WaitResponse;
				}, delegate(EntryMonitor m, bool isFatal)
				{
					if (!isFatal)
					{
						_mode.Mode = ProcMode.WaitResponse;
						m.ResponseYes();
					}
					else
					{
						Context.SetState(StateType.ConfirmEntry, true);
					}
				}, delegate(bool f)
				{
					Process.IsTimeCounting(!f);
				});
				SubProcesses.Add(_tryAime);
				_mode.Mode = ProcMode.Idle;
				break;
			case ProcMode.Idle:
				if (!_tryAime.IsError && InputResponse() != 0)
				{
					_tryAime.Discard();
					Context.SetState(StateType.ConfirmEntry, true);
				}
				break;
			case ProcMode.WaitResponse:
				if (InputResponse() != 0)
				{
					SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.EnableRead(flag: true);
					_monitor.OpenScreen(ScreenType.ConfirmAccessCode);
					_mode.Mode = ProcMode.Setup;
				}
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
