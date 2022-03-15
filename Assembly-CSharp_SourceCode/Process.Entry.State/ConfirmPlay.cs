using System.Linq;
using DB;
using MAI2.Util;
using Mai2.Voice_000001;
using MAI2System;
using Manager;
using Monitor.Entry;
using Monitor.Entry.Parts;
using Monitor.TakeOver;
using Net;
using Net.Packet.Helper;
using Net.Packet.Mai2;
using Net.VO.Mai2;

namespace Process.Entry.State
{
	public class ConfirmPlay : StateEntry
	{
		public enum InitType
		{
			Normal,
			Resume
		}

		private enum ProcMode
		{
			Setup,
			GetUserPreview,
			Suspend,
			Select,
			Error,
			WaitResponse
		}

		private readonly ProcEnum<ProcMode> _mode;

		private uint _aimeId;

		private string _accessCode;

		private string _offlineId;

		private string _authKey = "";

		private bool _isInherit;

		private UserPreviewResponseVO _previewVO;

		private const long ThresholdSecMultipleLogins = 900L;

		public ConfirmPlay()
		{
			_mode = new ProcEnum<ProcMode>(base.ResetResponse);
		}

		public override void Init(params object[] args)
		{
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.EnableRead(flag: false);
			switch ((InitType)args[0])
			{
			case InitType.Normal:
				_aimeId = (uint)args[1];
				_accessCode = (string)args[2];
				_offlineId = (string)args[3];
				_authKey = (string)args[4];
				_mode.Mode = ProcMode.Setup;
				break;
			case InitType.Resume:
				OpenMonitorScreens(ScreenType.ConfirmExistingAime, null, null, "", false);
				_mode.Mode = ProcMode.Select;
				break;
			}
		}

		public override void Exec(float deltaTime)
		{
			switch (_mode.Mode)
			{
			case ProcMode.Setup:
				if (Monitors.All((EntryMonitor m) => m.IsStarting))
				{
					Process.IsTimeCounting(isTimeCount: true);
					_mode.Mode = ProcMode.GetUserPreview;
				}
				break;
			case ProcMode.GetUserPreview:
				Process.IsTimeCounting(isTimeCount: false);
				PacketHelper.StartPacket(new PacketGetUserPreview(UserID.ToUserID(_aimeId), _authKey, delegate(ulong id, UserPreviewResponseVO vo)
				{
					Process.IsTimeCounting(isTimeCount: true);
					if (vo.isLogin)
					{
						OpenMonitorScreens(ScreenType.ErrorAime, 256);
						_mode.Mode = ProcMode.Error;
					}
					else if (!vo.IsNewUser())
					{
						if (IsValidVersion(vo))
						{
							if (GameManager.IsEventMode && IsMajorVersionUp(vo, isCheckMajorVersion: true))
							{
								OpenMonitorScreens(ScreenType.WindowGeneral, WindowMessageID.EntryErrorAimeEventInherit);
								_mode.Mode = ProcMode.Error;
							}
							else
							{
								SetSelectSkin(vo, isInherit: false);
								if (IsMajorVersionUp(vo, isCheckMajorVersion: true))
								{
									PlayVoice(Cue.VO_000171);
								}
								else
								{
									PlayVoice(Cue.VO_000012);
								}
								_mode.Mode = ProcMode.Select;
							}
						}
						else
						{
							OpenMonitorScreens(ScreenType.WindowGeneral, WindowMessageID.EntryErrorAimeVersion);
							_mode.Mode = ProcMode.Error;
						}
					}
					else if (!GameManager.IsEventMode)
					{
						if (!vo.IsInheritUser())
						{
							Context.SetState(StateType.ConfirmNewUser, id, _accessCode, _offlineId, null);
							PlayVoice(Cue.VO_000006);
						}
						else
						{
							SetSelectSkin(new UserPreviewResponseVO
							{
								userId = vo.userId,
								userName = vo.userName,
								iconId = 1
							}, isInherit: true);
							_mode.Mode = ProcMode.Select;
							PlayVoice(Cue.VO_000171);
						}
					}
					else
					{
						OpenMonitorScreens(ScreenType.WindowGeneral, WindowMessageID.EntryErrorAimeEventNew);
						_mode.Mode = ProcMode.Error;
					}
				}, delegate
				{
					Process.IsTimeCounting(isTimeCount: true);
					OpenMonitorScreens(ScreenType.WindowGeneral, WindowMessageID.NetworkError, true);
					_mode.Mode = ProcMode.Error;
				}));
				_mode.Mode = ProcMode.Suspend;
				break;
			case ProcMode.Select:
				switch (InputResponse())
				{
				case EntryMonitor.Response.Yes:
					SubProcesses.Add(new TryEntry(MainSatellite(), SubSatellite(), isFreedom: false, _isInherit ? UserData.UserIDType.Inherit : UserData.UserIDType.Exist, delegate(EntryMonitor m)
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
				case EntryMonitor.Response.AccessCode:
					MainSatellite()?.OpenScreen(ScreenType.DisplayAccessCodeQR);
					SubSatellite()?.OpenScreen(ScreenType.DisplayPleaseWait);
					_mode.Mode = ProcMode.WaitResponse;
					break;
				}
				break;
			case ProcMode.WaitResponse:
				if (InputResponse() != 0)
				{
					SetSelectSkin();
					_mode.Mode = ProcMode.Select;
				}
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

		private void SetSelectSkin(UserPreviewResponseVO vo, bool isInherit)
		{
			_previewVO = vo;
			_isInherit = isInherit;
			SetSelectSkin();
		}

		private void SetSelectSkin()
		{
			if (_isInherit)
			{
				OpenMonitorScreens(ScreenType.ConfirmInheritAime, _previewVO, _accessCode, _offlineId);
				return;
			}
			bool flag = Monitors.Any((EntryMonitor m) => m.IsDecide);
			OpenMonitorScreens(ScreenType.ConfirmExistingAime, _previewVO, _accessCode, _offlineId, flag);
		}

		private static bool IsValidVersion(UserPreviewResponseVO vo)
		{
			VersionNo versionNo = Singleton<SystemConfig>.Instance.config.romVersionInfo.versionNo;
			VersionNo versionNo2 = Singleton<SystemConfig>.Instance.config.dataVersionInfo.versionNo;
			VersionNo versionNo3 = default(VersionNo);
			VersionNo versionNo4 = default(VersionNo);
			versionNo3.tryParse(vo.lastRomVersion, setZeroIfFailed: true);
			versionNo4.tryParse(vo.lastDataVersion, setZeroIfFailed: true);
			uint versionCode = versionNo3.versionCode;
			uint versionCode2 = versionNo.versionCode;
			uint versionCode3 = versionNo4.versionCode;
			uint versionCode4 = versionNo2.versionCode;
			bool num = versionCode <= versionCode2;
			bool result = versionCode3 <= versionCode4;
			if (num)
			{
				return result;
			}
			return false;
		}

		private static bool IsMajorVersionUp(UserPreviewResponseVO vo, bool isCheckMajorVersion)
		{
			VersionNo versionNo = Singleton<SystemConfig>.Instance.config.romVersionInfo.versionNo;
			uint versionCode = versionNo.versionCode;
			bool result = false;
			uint num = 0u;
			VersionNo versionNo2 = default(VersionNo);
			TakeOverMajorVersion takeOverMajorVersion = new TakeOverMajorVersion();
			if (vo.lastRomVersion == null)
			{
				versionNo2.tryParse("", setZeroIfFailed: true);
			}
			else
			{
				versionNo2.tryParse(vo.lastRomVersion, setZeroIfFailed: true);
			}
			num = versionNo2.versionCode;
			if (num != 0 && num < versionCode)
			{
				TakeOverMonitor.MajorRomVersion majorRomVersion = takeOverMajorVersion.GetMajorRomVersion(num);
				TakeOverMonitor.MajorRomVersion majorRomVersion2 = takeOverMajorVersion.GetMajorRomVersion(versionCode);
				if (isCheckMajorVersion)
				{
					if (majorRomVersion < majorRomVersion2)
					{
						result = true;
					}
				}
				else if (versionNo2.versionCodeNoRelease < versionNo.versionCodeNoRelease)
				{
					result = true;
				}
			}
			return result;
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
