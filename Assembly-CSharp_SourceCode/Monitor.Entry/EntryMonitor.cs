using System;
using System.Collections.Generic;
using DB;
using MAI2.Util;
using Manager;
using Monitor.Entry.Parts;
using Monitor.Entry.Parts.Screens;
using Monitor.Entry.Util;
using Monitor.MapCore;
using Monitor.MapCore.Component;
using Net;
using Net.VO.Mai2;
using Process;

namespace Monitor.Entry
{
	public class EntryMonitor : MapMonitor
	{
		public enum TimeValue
		{
			DecrementTime = 30,
			DecrementTime2 = 1,
			ThresholdLongpush = 200
		}

		public enum Response
		{
			None = 0,
			Yes = 1,
			No = 2,
			Freedom = 10,
			AccessCode = 11,
			End = 255
		}

		private Machine _machine;

		public InfoWindow _infoWindow;

		private PromotionManager _promotionManager;

		private ScreenManager _screen;

		private ButtonManager _buttonManager;

		private Wipe _wipe;

		private DelayComponent _delay;

		private Dictionary<ScreenType, Action<object[]>> _commands;

		public bool IsLoginProcessing;

		public EntryProcess Process { get; private set; }

		public ProcessManager ProcessManager => Process.ProcessManager;

		public EntryMonitor OtherMonitor { get; private set; }

		public ulong UserId { get; private set; }

		public string UserName { get; private set; }

		public int UserRating { get; private set; }

		public int UserDispRate { get; private set; }

		public int UserIcon { get; private set; }

		public int UserPartner { get; private set; }

		public int UserFrame { get; private set; }

		public string AccessCode { get; private set; }

		public string OfflineId { get; private set; }

		public int NetMember { get; private set; }

		public string DailyBonusDate { get; private set; }

		public string TableDataVersion { get; private set; }

		public string RomDataVersion { get; private set; }

		public int UserTotalAwake { get; private set; }

		public int HeadphoneVolume { get; private set; }

		public bool IsStarting { get; private set; }

		public bool IsDecide { get; private set; }

		public bool IsFreedom { get; private set; }

		public bool IsNewUser { get; private set; }

		public Response InputResponse { get; private set; }

		public bool IsGuest()
		{
			return UserID.IsGuest(UserId);
		}

		public void Initialize(EntryProcess process, int index, bool active)
		{
			_machine = GetComponentInChildren<Machine>();
			_infoWindow = GetComponentInChildren<InfoWindow>();
			_promotionManager = GetComponentInChildren<PromotionManager>();
			_screen = GetComponentInChildren<ScreenManager>();
			_buttonManager = GetComponentInChildren<ButtonManager>();
			_wipe = GetComponentInChildren<Wipe>();
			_delay = base.gameObject.AddComponent<DelayComponent>();
			base.Initialize(index, active);
			_promotionManager.Initialize();
			_buttonManager.Initialize();
			_screen.Initialize();
			_infoWindow.Initialize();
			_wipe.Initialize();
			InitUserData();
			InitCommands();
			Process = process;
			_delay.StartDelay(0.6f, delegate
			{
				IsStarting = true;
			});
		}

		public void PostInitialize(EntryMonitor other)
		{
			OtherMonitor = other;
		}

		public void InitUserData()
		{
			UserId = UserID.GuestID();
			UserName = CommonMessageID.GuestUserName.GetName();
			UserRating = 0;
			UserDispRate = 0;
			UserIcon = 1;
			UserPartner = 0;
			AccessCode = "";
			OfflineId = "";
			NetMember = 0;
			DailyBonusDate = "";
			IsNewUser = false;
			TableDataVersion = "";
			RomDataVersion = "";
			UserTotalAwake = 0;
			HeadphoneVolume = 0;
		}

		public void SetUserData(UserPreviewResponseVO vo, string accessCode, string offlineId)
		{
			UserId = vo.userId;
			UserName = vo.userName;
			UserRating = vo.playerRating;
			UserDispRate = vo.dispRate;
			UserIcon = vo.iconId;
			UserPartner = vo.partnerId;
			UserFrame = vo.frameId;
			AccessCode = accessCode;
			OfflineId = offlineId;
			NetMember = vo.isNetMember;
			DailyBonusDate = vo.dailyBonusDate;
			IsNewUser = false;
			TableDataVersion = vo.lastDataVersion;
			RomDataVersion = vo.lastRomVersion;
			UserTotalAwake = vo.totalAwake;
			HeadphoneVolume = vo.headPhoneVolume;
		}

		public void SetNewUserData(ulong userId, string accessCode, string offlineId)
		{
			UserId = userId;
			UserName = CommonMessageID.DefaultUserName.GetName();
			UserRating = 0;
			UserDispRate = 0;
			UserIcon = 1;
			UserPartner = 0;
			AccessCode = accessCode;
			OfflineId = offlineId;
			NetMember = 0;
			DailyBonusDate = "";
			IsNewUser = true;
			TableDataVersion = "";
			RomDataVersion = "";
			UserTotalAwake = 0;
			HeadphoneVolume = 0;
		}

		public void ResetResponse()
		{
			InputResponse = Response.None;
		}

		public void ResponseYes()
		{
			InputResponse = Response.Yes;
		}

		public void ResponseNo()
		{
			InputResponse = Response.No;
		}

		public void ResponseFreedom()
		{
			InputResponse = Response.Freedom;
		}

		public void ResponseAccessCode()
		{
			InputResponse = Response.AccessCode;
		}

		public void ResponseEnd()
		{
			InputResponse = Response.End;
		}

		public void OpenScreen(ScreenType type, params object[] args)
		{
			_commands[type](args);
		}

		public void LockScreenButtons()
		{
			_screen.LockScreenButtons();
		}

		private void InitCommands()
		{
			_commands = new Dictionary<ScreenType, Action<object[]>>
			{
				[ScreenType.ConfirmEntry] = ConfirmEntry,
				[ScreenType.ConfirmGuest] = ConfirmGuest,
				[ScreenType.DisplayPleaseWait] = DisplayPleaseWait,
				[ScreenType.ConfirmNewAime] = ConfirmNewAime,
				[ScreenType.ConfirmNewUser] = ConfirmNewUser,
				[ScreenType.ConfirmExistingAime] = ConfirmExistingAime,
				[ScreenType.ConfirmInheritAime] = ConfirmInheritAime,
				[ScreenType.ConfirmFreedom] = ConfirmFreedom,
				[ScreenType.ErrorAime] = ErrorAime,
				[ScreenType.WaitPartner] = WaitPartner,
				[ScreenType.DoneEntry] = DoneEntry,
				[ScreenType.ConfirmFelica] = ConfirmFelica,
				[ScreenType.NoticeFelicaRegistration] = NoticeFelicaRegistration,
				[ScreenType.ConfirmFelicaSite] = ConfirmFelicaSite,
				[ScreenType.DisplayFelicaQR] = DisplayFelicaQR,
				[ScreenType.ConfirmAccessCode] = ConfirmAccessCode,
				[ScreenType.DisplayAccessCodeQR] = DisplayAccessCodeQR,
				[ScreenType.ErrorAccessCode] = ErrorAccessCode,
				[ScreenType.WindowGeneral] = WindowGeneral,
				[ScreenType.ConfirmExistingAimeContinue] = ConfirmExistingAimeContinue
			};
		}

		private void ConfirmEntry(object[] args)
		{
			if (!IsDecide)
			{
				InitUserData();
				_screen.OpenScreen(ScreenType.ConfirmEntry, args);
			}
		}

		private void ConfirmGuest(object[] args)
		{
			if (!IsDecide)
			{
				_screen.OpenScreen(ScreenType.ConfirmGuest);
			}
		}

		private void DisplayPleaseWait(object[] args)
		{
			if (!IsDecide)
			{
				_screen.OpenScreen(ScreenType.DisplayPleaseWait);
			}
		}

		private void ConfirmNewAime(object[] args)
		{
			if (!IsDecide)
			{
				_screen.OpenScreen(ScreenType.ConfirmNewAime);
			}
		}

		private void ConfirmNewUser(object[] args)
		{
			if (!IsDecide)
			{
				_screen.OpenScreen(ScreenType.ConfirmNewUser);
			}
		}

		private void ConfirmExistingAime(object[] args)
		{
			if (!IsDecide)
			{
				ConfirmAimeEntry(ScreenType.ConfirmExistingAime, args);
			}
		}

		private void ConfirmExistingAimeContinue(object[] args)
		{
			if (!IsDecide)
			{
				ConfirmAimeEntry(ScreenType.ConfirmExistingAimeContinue, args);
			}
		}

		private void ConfirmInheritAime(object[] args)
		{
			if (!IsDecide)
			{
				ConfirmAimeEntry(ScreenType.ConfirmInheritAime, new object[4]
				{
					args[0],
					args[1],
					args[2],
					true
				});
			}
		}

		private void ConfirmAimeEntry(ScreenType type, IReadOnlyList<object> args)
		{
			if (args[0] != null)
			{
				SetUserData((UserPreviewResponseVO)args[0], (string)args[1], (string)args[2]);
			}
			_screen.OpenScreen(type, args[3]);
		}

		private void ConfirmFreedom(object[] args)
		{
			if (!IsDecide)
			{
				_screen.OpenScreen(ScreenType.ConfirmFreedom);
			}
		}

		private void ErrorAime(object[] args)
		{
			if (!IsDecide)
			{
				_screen.OpenScreen(ScreenType.ErrorAime, args);
			}
		}

		private void WaitPartner(object[] args)
		{
			_screen.OpenScreen(ScreenType.WaitPartner, IsGuest(), IsNewUser);
		}

		private void DoneEntry(object[] args)
		{
			bool isEntry = Singleton<UserDataManager>.Instance.GetUserData(base.MonitorIndex).IsEntry;
			bool isEntry2 = Singleton<UserDataManager>.Instance.GetUserData(OtherMonitor.MonitorIndex).IsEntry;
			if (isEntry)
			{
				DoneEntry.Type type = (IsFreedom ? Monitor.Entry.Parts.Screens.DoneEntry.Type.Freedom : (isEntry2 ? Monitor.Entry.Parts.Screens.DoneEntry.Type.TwoPlayer : Monitor.Entry.Parts.Screens.DoneEntry.Type.OnePlayer));
				_screen.OpenScreen(ScreenType.DoneEntry, type, base.MonitorIndex);
			}
			else
			{
				DisplayPleaseWait(args);
			}
		}

		private void ConfirmFelica(object[] args)
		{
			if (!IsDecide)
			{
				_screen.OpenScreen(ScreenType.ConfirmFelica);
			}
		}

		private void NoticeFelicaRegistration(object[] args)
		{
			if (!IsDecide)
			{
				_screen.OpenScreen(ScreenType.NoticeFelicaRegistration);
			}
		}

		private void ConfirmFelicaSite(object[] args)
		{
			if (!IsDecide)
			{
				_screen.OpenScreen(ScreenType.ConfirmFelicaSite);
			}
		}

		private void DisplayFelicaQR(object[] args)
		{
			if (!IsDecide)
			{
				_screen.OpenScreen(ScreenType.DisplayFelicaQR, args);
			}
		}

		private void ConfirmAccessCode(object[] args)
		{
			if (!IsDecide)
			{
				_screen.OpenScreen(ScreenType.ConfirmAccessCode);
			}
		}

		private void DisplayAccessCodeQR(object[] args)
		{
			if (!IsDecide)
			{
				_screen.OpenScreen(ScreenType.DisplayAccessCodeQR, (args.Length != 0) ? args : new object[1] { AccessCode });
			}
		}

		private void ErrorAccessCode(object[] args)
		{
			if (!IsDecide)
			{
				_screen.OpenScreen(ScreenType.ErrorAccessCode);
			}
		}

		private void WindowGeneral(object[] args)
		{
			if (!IsDecide)
			{
				_screen.OpenScreen(ScreenType.WindowGeneral, args);
			}
		}

		public void DecideEntry()
		{
			IsDecide = true;
			if (OtherMonitor.IsDecide)
			{
				State = StateSyncMachineLoop;
			}
		}

		public void DecideEntryFreedom(int index)
		{
			IsDecide = true;
			if (base.MonitorIndex != index)
			{
				UserName = "";
				return;
			}
			IsFreedom = true;
			OtherMonitor.DecideEntryFreedom(index);
		}

		private void StateSyncMachineLoop(float deltaTime)
		{
			if (IsMachineLoopState() && OtherMonitor.IsMachineLoopState())
			{
				SyncMachineLoopAnimation();
				OtherMonitor.SyncMachineLoopAnimation();
				SetStateTerminate();
			}
		}

		public void OpenPromotion(PromotionType type)
		{
			_promotionManager.Open(type);
		}

		public void ClosePromotion()
		{
			_promotionManager.Close();
		}

		public EntryButton CreateCommonButton(ButtonType type, bool isGrayButton = false)
		{
			return _buttonManager.CreateButton(type, isGrayButton);
		}

		public void ShowInfoWindow(Action onDone)
		{
			_infoWindow.Show(onDone);
		}

		public void HideInfoWindow()
		{
			_infoWindow.Hide();
		}

		public bool IsShowInfoWindow()
		{
			return _infoWindow.IsShow;
		}

		public void CallShowImmidiateHide()
		{
			_infoWindow.ShowImmediateHide(InfoWindow.GetCurrentKind());
		}

		public void SetIsShowFlag(bool flag)
		{
			_infoWindow.SetIsShowFlag(flag);
		}

		public void ShowMachine(int index, SatelliteEntryType type, string text)
		{
			_machine.Show(index, type, text);
		}

		public void HideMachine()
		{
			_machine.Hide();
		}

		public bool IsShowMachine()
		{
			return _machine.IsShow;
		}

		public void ShowWipe(Action onDone)
		{
			_wipe.Show(onDone);
		}

		public bool IsMachineLoopState()
		{
			return _machine.IsSatellitesLoopState();
		}

		public void SyncMachineLoopAnimation()
		{
			_machine.SyncSatellitesLoopAnimation();
		}
	}
}
