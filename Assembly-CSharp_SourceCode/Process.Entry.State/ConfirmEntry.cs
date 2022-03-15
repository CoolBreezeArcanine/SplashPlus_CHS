using System.Linq;
using DB;
using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_000001;
using Manager;
using Monitor.Entry;
using Monitor.Entry.Parts;
using Net;

namespace Process.Entry.State
{
	public class ConfirmEntry : StateEntry
	{
		private enum ProcMode
		{
			Setup,
			Request,
			Reading,
			Suspend,
			ResultNewAime,
			ResultNewFelica,
			NewFelicaPolling,
			NewFelicaWaitNotice,
			NewFelicaWaitConfirmSite,
			NewFelicaWaitDispQR,
			NewFelicaDone,
			Error
		}

		private readonly ProcEnum<ProcMode> _mode;

		private TryAime _tryAime;

		private EntryMonitor _monitor;

		private bool _isNewAime;

		private bool _isOfflineGrayIcon;

		private bool _isEnableAccessCode;

		public ConfirmEntry()
		{
			_isOfflineGrayIcon = false;
			_mode = new ProcEnum<ProcMode>(base.ResetResponse);
		}

		public override void Init(params object[] args)
		{
			bool flag = (bool)args[0];
			_isOfflineGrayIcon = !Singleton<OperationManager>.Instance.IsAliveAimeServer || !Singleton<OperationManager>.Instance.IsAuthGood || !Singleton<OperationManager>.Instance.IsAliveServer;
			_isEnableAccessCode = Singleton<OperationManager>.Instance.IsAliveAimeReader;
			if (!Singleton<OperationManager>.Instance.IsAliveAimeServer)
			{
				_isEnableAccessCode = false;
			}
			if (_isOfflineGrayIcon)
			{
				flag = false;
				SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.EnableRead(flag: false);
			}
			if (flag)
			{
				SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.EnableRead(flag: true);
			}
			_mode.Mode = ProcMode.Setup;
		}

		public override void Exec(float deltaTime)
		{
			switch (_mode.Mode)
			{
			case ProcMode.Setup:
				if (Monitors.All((EntryMonitor m) => m.IsStarting))
				{
					bool flag = !Singleton<OperationManager>.Instance.IsAliveAimeReader || !SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.AdvCheck();
					_isEnableAccessCode = Singleton<OperationManager>.Instance.IsAliveAimeReader;
					if (!Singleton<OperationManager>.Instance.IsAliveAimeServer)
					{
						_isEnableAccessCode = false;
					}
					OpenMonitorScreens(ScreenType.ConfirmEntry, flag, _isOfflineGrayIcon, _isEnableAccessCode);
					Process.IsTimeCounting(isTimeCount: true);
					_mode.Mode = ProcMode.Request;
				}
				break;
			case ProcMode.Request:
				_tryAime = new TryAime(Monitors[0], Monitors[1], delegate
				{
					PlaySE(Mai2.Mai2Cue.Cue.SE_ENTRY_AIME_OK);
					uint value = SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.GetAimeId().Value;
					string accessCode = SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.GetAccessCode();
					string offlineIdString = SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.GetOfflineIdString();
					if (_isNewAime)
					{
						Context.SetState(StateType.ConfirmNewUser, UserID.ToUserID(value), accessCode, offlineIdString, _monitor);
						PlayVoice(Mai2.Voice_000001.Cue.VO_000006);
						_monitor = null;
						_isNewAime = false;
					}
					else
					{
						string segaIdAuthKey = SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.GetSegaIdAuthKey();
						Context.SetState(StateType.ConfirmPlay, ConfirmPlay.InitType.Normal, value, accessCode, offlineIdString, segaIdAuthKey);
					}
				}, delegate
				{
					PlaySE(Mai2.Mai2Cue.Cue.SE_ENTRY_AIME_OK);
					if (!GameManager.IsEventMode)
					{
						if (_monitor != null)
						{
							_monitor.OpenScreen(ScreenType.ConfirmNewAime);
							PartnerSatellite(_monitor).OpenScreen(ScreenType.DisplayPleaseWait);
						}
						else
						{
							OpenMonitorScreens(ScreenType.ConfirmNewAime);
						}
						_mode.Mode = ProcMode.ResultNewAime;
					}
					else
					{
						OpenMonitorScreens(ScreenType.WindowGeneral, WindowMessageID.EntryErrorAimeEventNew);
						_mode.Mode = ProcMode.Error;
					}
				}, delegate
				{
					PlaySE(Mai2.Mai2Cue.Cue.SE_ENTRY_AIME_OK);
					if (!GameManager.IsEventMode)
					{
						OpenMonitorScreens(ScreenType.ConfirmFelica);
						_mode.Mode = ProcMode.ResultNewFelica;
					}
					else
					{
						OpenMonitorScreens(ScreenType.WindowGeneral, WindowMessageID.EntryErrorAimeEventNew);
						_mode.Mode = ProcMode.Error;
					}
				}, delegate
				{
					Init(true);
				}, delegate(bool f)
				{
					Process.IsTimeCounting(!f);
				}, delegate(EntryMonitor _, uint aimeId)
				{
					Context.SetState(StateType.ConfirmPlay, ConfirmPlay.InitType.Normal, aimeId, "AGING TEST", "");
				});
				SubProcesses.Add(_tryAime);
				_mode.Mode = ProcMode.Reading;
				break;
			case ProcMode.Reading:
				if (!_tryAime.IsError)
				{
					switch (InputResponse())
					{
					case EntryMonitor.Response.Yes:
						_tryAime.Discard();
						Context.SetState(StateType.ConfirmGuest, MainSatellite());
						break;
					case EntryMonitor.Response.AccessCode:
						_tryAime.Discard();
						Context.SetState(StateType.ConfirmAccessCode, MainSatellite());
						break;
					}
				}
				break;
			case ProcMode.ResultNewAime:
			{
				EntryMonitor.Response response = InputResponse();
				if (response != 0)
				{
					SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.Confirm(response == EntryMonitor.Response.Yes);
					switch (response)
					{
					case EntryMonitor.Response.Yes:
						_monitor = MainSatellite();
						_isNewAime = true;
						_tryAime.Resume();
						_mode.Mode = ProcMode.Suspend;
						break;
					case EntryMonitor.Response.No:
						_tryAime.Resume();
						OpenMonitorScreens(ScreenType.ConfirmEntry, true);
						_mode.Mode = ProcMode.Reading;
						break;
					}
				}
				break;
			}
			case ProcMode.ResultNewFelica:
			{
				EntryMonitor.Response response = InputResponse();
				if (response != 0)
				{
					SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.Confirm(response == EntryMonitor.Response.Yes);
					switch (response)
					{
					case EntryMonitor.Response.Yes:
						_monitor = MainSatellite();
						SubSatellite()?.OpenScreen(ScreenType.DisplayPleaseWait);
						_mode.Mode = ProcMode.NewFelicaPolling;
						break;
					case EntryMonitor.Response.No:
						_tryAime.Resume();
						OpenMonitorScreens(ScreenType.ConfirmEntry, true);
						_mode.Mode = ProcMode.Reading;
						break;
					}
				}
				break;
			}
			case ProcMode.NewFelicaPolling:
				if (!SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.IsPollingState())
				{
					_monitor.OpenScreen(ScreenType.NoticeFelicaRegistration);
					_mode.Mode = ProcMode.NewFelicaWaitNotice;
				}
				break;
			case ProcMode.NewFelicaWaitNotice:
				if (InputResponse() == EntryMonitor.Response.Yes)
				{
					MainSatellite()?.OpenScreen(ScreenType.ConfirmFelicaSite);
					_mode.Mode = ProcMode.NewFelicaWaitConfirmSite;
				}
				break;
			case ProcMode.NewFelicaWaitConfirmSite:
				switch (InputResponse())
				{
				case EntryMonitor.Response.Yes:
					MainSatellite()?.OpenScreen(ScreenType.DisplayFelicaQR, SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.GetAccessCode());
					_mode.Mode = ProcMode.NewFelicaWaitDispQR;
					break;
				case EntryMonitor.Response.No:
					_mode.Mode = ProcMode.NewFelicaDone;
					break;
				}
				break;
			case ProcMode.NewFelicaWaitDispQR:
				if (InputResponse() == EntryMonitor.Response.Yes)
				{
					_mode.Mode = ProcMode.NewFelicaDone;
				}
				break;
			case ProcMode.NewFelicaDone:
				_tryAime.Resume();
				_mode.Mode = ProcMode.Suspend;
				break;
			case ProcMode.Error:
				if (InputResponse() != 0)
				{
					Context.SetState(StateType.ConfirmEntry, true);
				}
				break;
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
