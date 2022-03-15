using System.Linq;
using MAI2.Util;
using Manager;
using Monitor.Entry;
using Monitor.Entry.Parts;
using Net.VO.Mai2;

namespace Process.Entry.State
{
	public class ConfirmContinueOne : StateEntry
	{
		public enum InitType
		{
			Normal,
			Resume
		}

		private enum ProcMode
		{
			Setup,
			Entry,
			Suspend,
			WaitResponse
		}

		private readonly ProcEnum<ProcMode> _mode;

		private EntryMonitor _monitor;

		private EntryMonitor _partner;

		private bool _timerSuspend;

		public ConfirmContinueOne()
		{
			_mode = new ProcEnum<ProcMode>(delegate
			{
				_monitor?.ResetResponse();
			});
		}

		public override void Init(params object[] args)
		{
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.EnableRead(flag: false);
			int num = (int)args[0];
			switch ((InitType)args[1])
			{
			case InitType.Normal:
				_monitor = Monitors[num];
				_partner = Monitors[(num + 1) & 1];
				break;
			}
			_mode.Mode = ProcMode.Setup;
			_timerSuspend = true;
		}

		public override void Exec(float deltaTime)
		{
			switch (_mode.Mode)
			{
			case ProcMode.Setup:
				if (_monitor.IsStarting)
				{
					_timerSuspend = false;
					SetEntrySkin(_monitor);
					_partner.OpenScreen(ScreenType.DisplayPleaseWait);
					_partner.ResponseEnd();
				}
				break;
			case ProcMode.Entry:
				switch (_monitor.InputResponse)
				{
				case EntryMonitor.Response.Yes:
					SubProcesses.Add(new TryEntry(_monitor, null, isFreedom: false, UserData.UserIDType.Exist, delegate(EntryMonitor m)
					{
						m.DecideEntry();
						m.OpenScreen(ScreenType.WaitPartner);
						m.ResponseEnd();
					}, SetEntrySkin, delegate(EntryMonitor m, bool f)
					{
						_timerSuspend = f;
					}));
					_mode.Mode = ProcMode.Suspend;
					break;
				case EntryMonitor.Response.No:
				{
					_monitor.ResponseEnd();
					int monitorIndex = _monitor.MonitorIndex;
					Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).Initialize();
					Singleton<UserDataManager>.Instance.SetDefault(monitorIndex);
					break;
				}
				case EntryMonitor.Response.Freedom:
					Context.SetState(StateType.ConfirmFreedomOne, _monitor);
					break;
				case EntryMonitor.Response.AccessCode:
					_monitor.OpenScreen(ScreenType.DisplayAccessCodeQR);
					_monitor.ResetResponse();
					_mode.Mode = ProcMode.WaitResponse;
					break;
				}
				break;
			case ProcMode.WaitResponse:
				if (_monitor.InputResponse != 0)
				{
					SetEntrySkin(_monitor);
				}
				break;
			}
			Process.IsTimeCounting(!_timerSuspend);
			if (Monitors.All((EntryMonitor m) => m.InputResponse == EntryMonitor.Response.End))
			{
				Context.SetState(StateType.ConfirmEntry, true);
			}
			base.Exec(deltaTime);
		}

		private void SetEntrySkin(EntryMonitor monitor)
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(monitor.MonitorIndex);
			monitor.OpenScreen(ScreenType.ConfirmExistingAime, new UserPreviewResponseVO
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
			}, userData.Detail.AccessCode, userData.OfflineId, false);
			_mode.Mode = ProcMode.Entry;
			monitor.ResetResponse();
		}

		public override string ToString()
		{
			return base.ToString() + $" {_mode.Mode}";
		}
	}
}
