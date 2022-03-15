using System.Linq;
using MAI2.Util;
using Manager;
using Monitor.Entry;
using Monitor.Entry.Parts;
using Net.VO.Mai2;

namespace Process.Entry.State
{
	public class ConfirmContinue : StateEntry
	{
		private enum ProcMode
		{
			Setup,
			Entry,
			Suspend,
			WaitResponse
		}

		private ProcMode[] _mode;

		private bool[] _timerSuspend;

		public override void Init(params object[] args)
		{
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.EnableRead(flag: false);
			_mode = new ProcMode[2];
			_timerSuspend = new bool[2] { true, true };
		}

		public override void Exec(float deltaTime)
		{
			foreach (EntryMonitor monitor in Monitors)
			{
				int monitorIndex = monitor.MonitorIndex;
				switch (_mode[monitorIndex])
				{
				case ProcMode.Setup:
					if (monitor.IsStarting)
					{
						_timerSuspend[monitorIndex] = false;
						SetEntrySkin(monitor);
					}
					break;
				case ProcMode.Entry:
					switch (monitor.InputResponse)
					{
					case EntryMonitor.Response.Yes:
						SubProcesses.Add(new TryEntry(monitor, null, isFreedom: false, UserData.UserIDType.Exist, delegate(EntryMonitor m)
						{
							m.DecideEntry();
							m.OpenScreen(ScreenType.WaitPartner);
							m.ResponseEnd();
							m.IsLoginProcessing = false;
						}, delegate(EntryMonitor m)
						{
							SetEntrySkin(m);
							m.IsLoginProcessing = false;
						}, delegate(EntryMonitor m, bool f)
						{
							_timerSuspend[m.MonitorIndex] = f;
						}));
						_mode[monitorIndex] = ProcMode.Suspend;
						break;
					case EntryMonitor.Response.No:
						if (PartnerSatellite(monitor).InputResponse != EntryMonitor.Response.End)
						{
							monitor.OpenScreen(ScreenType.DisplayPleaseWait);
						}
						monitor.ResponseEnd();
						Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).Initialize();
						Singleton<UserDataManager>.Instance.SetDefault(monitorIndex);
						break;
					case EntryMonitor.Response.AccessCode:
						monitor.OpenScreen(ScreenType.DisplayAccessCodeQR);
						monitor.ResetResponse();
						_mode[monitorIndex] = ProcMode.WaitResponse;
						break;
					}
					break;
				case ProcMode.WaitResponse:
					if (monitor.InputResponse != 0)
					{
						SetEntrySkin(monitor);
					}
					break;
				}
			}
			Process.IsTimeCounting(!_timerSuspend.Any((bool flag) => flag));
			if (Monitors.All((EntryMonitor m) => m.InputResponse == EntryMonitor.Response.End))
			{
				Context.SetState(StateType.ConfirmEntry, true);
			}
			base.Exec(deltaTime);
		}

		private void SetEntrySkin(EntryMonitor monitor)
		{
			int monitorIndex = monitor.MonitorIndex;
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(monitorIndex);
			monitor.OpenScreen(ScreenType.ConfirmExistingAimeContinue, new UserPreviewResponseVO
			{
				userId = userData.Detail.UserID,
				userName = userData.Detail.UserName,
				playerRating = (int)userData.Detail.Rating,
				dispRate = (int)userData.Option.DispRate,
				iconId = userData.Detail.EquipIconID,
				partnerId = userData.Detail.EquipPartnerID,
				frameId = userData.Detail.EquipFrameID,
				lastDataVersion = userData.Detail.LastDataVersion,
				lastRomVersion = userData.Detail.LastRomVersion,
				totalAwake = userData.Detail.TotalAwake,
				isNetMember = userData.Detail.IsNetMember,
				dailyBonusDate = userData.Detail.DailyBonusDate,
				headPhoneVolume = (int)userData.Option.HeadPhoneVolume
			}, userData.Detail.AccessCode, userData.OfflineId, true);
			_mode[monitorIndex] = ProcMode.Entry;
			monitor.ResetResponse();
		}

		public override string ToString()
		{
			return base.ToString() + $" {_mode[0]}:{_mode[1]}";
		}
	}
}
