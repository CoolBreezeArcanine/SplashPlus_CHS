using System;
using DB;
using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_000001;
using Manager;
using Monitor.Entry;
using Monitor.Entry.Parts;
using Net;
using Net.Packet.Helper;
using Net.Packet.Mai2;
using Net.VO.Mai2;

namespace Process.Entry
{
	public class TryEntry : EntrySubProcess
	{
		private enum ProcMode
		{
			Trying,
			Suspend,
			Error
		}

		private ProcMode _mode;

		private readonly Action<EntryMonitor> _onDone;

		private readonly Action<EntryMonitor> _onErrorResume;

		private readonly Action<EntryMonitor, bool> _onTimeSuspend;

		private readonly bool _isFreedom;

		private readonly UserData.UserIDType _userType;

		private ProcMode Mode
		{
			get
			{
				return _mode;
			}
			set
			{
				_mode = value;
				MainMonitor.ResetResponse();
				SubMonitor?.ResetResponse();
			}
		}

		public TryEntry(EntryMonitor monitor, EntryMonitor subMonitor, bool isFreedom, UserData.UserIDType userType, Action<EntryMonitor> onDone, Action<EntryMonitor> onErrorResume, Action<EntryMonitor, bool> onTimerSuspend)
		{
			MainMonitor = monitor;
			SubMonitor = subMonitor;
			_onDone = onDone;
			_onErrorResume = onErrorResume;
			_onTimeSuspend = onTimerSuspend;
			_isFreedom = isFreedom;
			_userType = userType;
			Mode = ProcMode.Trying;
		}

		public override void Execute()
		{
			if (IsFinish)
			{
				return;
			}
			switch (Mode)
			{
			case ProcMode.Trying:
				if (!CanEntry(_isFreedom, _userType))
				{
					MainMonitor.OpenScreen(ScreenType.WindowGeneral, WindowMessageID.NetworkError, true);
					SubMonitor?.OpenScreen(ScreenType.WindowGeneral, WindowMessageID.NetworkError, true);
					Mode = ProcMode.Error;
					break;
				}
				if (_userType == UserData.UserIDType.Guest)
				{
					DecideEntry(_isFreedom, _userType, MainMonitor.MonitorIndex, MainMonitor.UserId, MainMonitor.UserName, MainMonitor.AccessCode, MainMonitor.OfflineId, MainMonitor.NetMember, MainMonitor.DailyBonusDate, MainMonitor.HeadphoneVolume);
					_onDone(MainMonitor);
					IsFinish = true;
				}
				else
				{
					_onTimeSuspend(MainMonitor, arg2: true);
					PacketHelper.StartPacket(new PacketUserLogin(MainMonitor.UserId, MainMonitor.AccessCode, delegate(UserLoginResponseVO vo)
					{
						if (vo.returnCode == 1)
						{
							_onTimeSuspend(MainMonitor, arg2: false);
							DecideEntry(_isFreedom, _userType, MainMonitor.MonitorIndex, MainMonitor.UserId, MainMonitor.UserName, MainMonitor.AccessCode, MainMonitor.OfflineId, MainMonitor.NetMember, MainMonitor.DailyBonusDate, MainMonitor.HeadphoneVolume);
							Singleton<NetDataManager>.Instance.SetLoginVO(MainMonitor.MonitorIndex, vo);
							_onDone(MainMonitor);
							IsFinish = true;
						}
						else
						{
							_onTimeSuspend(MainMonitor, arg2: false);
							DispAimeError(256);
							Mode = ProcMode.Error;
						}
					}, delegate
					{
						_onTimeSuspend(MainMonitor, arg2: false);
						MainMonitor.OpenScreen(ScreenType.WindowGeneral, WindowMessageID.NetworkError, true);
						SubMonitor?.OpenScreen(ScreenType.WindowGeneral, WindowMessageID.NetworkError, true);
						Mode = ProcMode.Error;
					}));
				}
				Mode = ProcMode.Suspend;
				break;
			case ProcMode.Error:
				if (InputResponse() != 0)
				{
					_onErrorResume(MainMonitor);
					IsFinish = true;
				}
				break;
			}
		}

		private static bool CanEntry(bool isFreedom, UserData.UserIDType type)
		{
			_ = 1;
			return true;
		}

		private static void DecideEntry(bool isFreedom, UserData.UserIDType type, int index, ulong userId, string userName, string accessCode, string offlineId, int isNetMember, string dailyBonusDate, int headPhoneVol)
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(index);
			userData.Initialize();
			userData.IsEntry = true;
			if (!UserID.IsGuest(userId))
			{
				userData.Detail.UserID = userId;
				userData.Detail.UserName = userName;
				userData.Detail.AccessCode = accessCode;
				userData.OfflineId = offlineId;
				userData.IsNetMember = isNetMember;
				userData.Detail.DailyBonusDate = dailyBonusDate;
				userData.UserType = type;
				userData.Option.HeadPhoneVolume = (OptionHeadphonevolumeID)headPhoneVol;
			}
			SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000065, index);
			SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_ENTRY_FIX, index);
		}

		private void DispAimeError(int code)
		{
			MainMonitor.OpenScreen(ScreenType.ErrorAime, code);
			MainMonitor.ResetResponse();
			if (SubMonitor != null)
			{
				SubMonitor.OpenScreen(ScreenType.ErrorAime, code);
				SubMonitor.ResetResponse();
			}
		}
	}
}
