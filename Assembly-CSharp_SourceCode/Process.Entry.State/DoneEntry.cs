using System.Linq;
using MAI2.Util;
using Manager;
using Monitor.Common;
using Monitor.Entry;
using Monitor.Entry.Parts;

namespace Process.Entry.State
{
	public class DoneEntry : StateEntry
	{
		private enum ProcMode
		{
			Setup,
			WaitInput,
			Suspend,
			NextProcess,
			Done
		}

		private readonly ProcEnum<ProcMode> _mode;

		public DoneEntry()
		{
			_mode = new ProcEnum<ProcMode>(base.ResetResponse);
		}

		public override void Init(params object[] args)
		{
			_mode.Mode = ProcMode.Setup;
		}

		public override void Exec(float deltaTime)
		{
			switch (_mode.Mode)
			{
			case ProcMode.Setup:
				if (Monitors.Any((EntryMonitor m) => m.IsDecide))
				{
					TimeManager.MarkGameStartTime();
					Singleton<EventManager>.Instance.UpdateEvent();
					Singleton<ScoreRankingManager>.Instance.UpdateData();
					Process.CreateDownloadProcess();
					Process.Container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30001));
					OpenMonitorScreens(ScreenType.DoneEntry);
					_mode.Mode = ProcMode.WaitInput;
				}
				else
				{
					Process.SetNextProcess();
					_mode.Mode = ProcMode.Suspend;
				}
				break;
			case ProcMode.WaitInput:
				if (InputResponse() == EntryMonitor.Response.None)
				{
					break;
				}
				foreach (EntryMonitor monitor in Monitors)
				{
					if (monitor.IsDecide)
					{
						monitor.Process.ProcessManager.CloseWindow(monitor.MonitorIndex);
						_mode.Mode = ProcMode.NextProcess;
					}
				}
				HideInformation();
				break;
			case ProcMode.NextProcess:
				Process.SetNextProcess();
				_mode.Mode = ProcMode.Done;
				break;
			}
			base.Exec(deltaTime);
		}

		public override string ToString()
		{
			return base.ToString() + $" {_mode.Mode}";
		}

		private void HideInformation()
		{
			for (int i = 0; i < 2; i++)
			{
				Process.Container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 40000, i, OperationInformationController.InformationType.Hide));
			}
		}
	}
}
