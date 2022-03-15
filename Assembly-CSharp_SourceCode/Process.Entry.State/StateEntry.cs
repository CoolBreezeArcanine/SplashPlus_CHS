using System.Collections.Generic;
using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_000001;
using Manager;
using Monitor.Entry;
using Monitor.Entry.Parts;

namespace Process.Entry.State
{
	public abstract class StateEntry : State
	{
		protected EntryProcess Process;

		protected List<EntryMonitor> Monitors;

		protected List<EntrySubProcess> SubProcesses;

		protected new ContextEntry Context;

		public void Setup(ContextEntry context, EntryProcess process, EntryMonitor monitor0, EntryMonitor monitor1)
		{
			Process = process;
			Monitors = new List<EntryMonitor> { monitor0, monitor1 };
			SubProcesses = new List<EntrySubProcess>();
			Context = context;
		}

		public override void Exec(float deltaTime)
		{
			SubProcesses.ForEach(delegate(EntrySubProcess p)
			{
				p.Execute();
			});
			SubProcesses.RemoveAll((EntrySubProcess p) => p.IsFinish);
		}

		public override void Term()
		{
		}

		protected void OpenMonitorScreens(ScreenType type, params object[] args)
		{
			foreach (EntryMonitor monitor in Monitors)
			{
				monitor.OpenScreen(type, args);
			}
		}

		protected EntryMonitor.Response InputResponse()
		{
			foreach (EntryMonitor monitor in Monitors)
			{
				if (monitor.InputResponse != 0)
				{
					return monitor.InputResponse;
				}
			}
			return EntryMonitor.Response.None;
		}

		protected void ResetResponse()
		{
			Monitors.ForEach(delegate(EntryMonitor m)
			{
				m.ResetResponse();
			});
		}

		protected EntryMonitor MainSatellite()
		{
			foreach (EntryMonitor monitor in Monitors)
			{
				if (monitor.InputResponse != 0)
				{
					return monitor;
				}
			}
			return null;
		}

		protected EntryMonitor SubSatellite()
		{
			EntryMonitor entryMonitor = MainSatellite();
			if (!(entryMonitor != null))
			{
				return null;
			}
			return PartnerSatellite(entryMonitor);
		}

		protected EntryMonitor PartnerSatellite(EntryMonitor monitor)
		{
			return Monitors[(monitor.MonitorIndex + 1) & 1];
		}

		public virtual int GetProcMode()
		{
			return -1;
		}

		protected void PlaySE(Mai2.Mai2Cue.Cue cue, bool force = false)
		{
			foreach (EntryMonitor monitor in Monitors)
			{
				if (force || !monitor.IsDecide)
				{
					Singleton<SeManager>.Instance.PlaySE(cue, monitor.MonitorIndex);
				}
			}
		}

		protected void PlayVoice(Mai2.Voice_000001.Cue cue, bool force = false)
		{
			foreach (EntryMonitor monitor in Monitors)
			{
				if (force || !monitor.IsDecide)
				{
					SoundManager.PlayVoice(cue, monitor.MonitorIndex);
				}
			}
		}
	}
}
