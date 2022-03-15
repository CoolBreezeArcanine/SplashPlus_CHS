using DB;
using Manager;
using Monitor.Common;
using Monitor.Entry.Util;

namespace Monitor.Entry.Parts.Screens
{
	public class WaitPartner : EntryScreen
	{
		private bool _watchOther;

		private int _pushCount;

		private float _pushEffectiveTime;

		private EntryButton _button;

		public override void Open(params object[] args)
		{
			bool isGuest = (bool)args[0];
			bool isNewAime = (bool)args[1];
			_pushCount = 0;
			_pushEffectiveTime = 0f;
			base.Open(args);
			EntryMonitor.HideInfoWindow();
			_button = CreateButton(ButtonType.TimeSkip, null, null, delegate
			{
			});
			Delay.StartDelay(0.25f, delegate
			{
				ActivateButtons();
				Delay.StartDelay(0.25f, delegate
				{
					OpenOperationInformation(OperationInformationController.InformationType.TimerSkip);
					EntryMonitor.OpenPromotion((isGuest || !isNewAime) ? PromotionType.Normal : PromotionType.NewAime);
					InitSelfSatellite();
					InitOtherSatellite();
				});
			});
		}

		private void InitSelfSatellite()
		{
			EntryMonitor entryMonitor = EntryMonitor;
			string userNameString = GetUserNameString(entryMonitor.IsGuest(), entryMonitor.IsNewUser, entryMonitor.UserName);
			EntryMonitor.ShowMachine(entryMonitor.MonitorIndex, (!entryMonitor.IsGuest()) ? SatelliteEntryType.Aime : SatelliteEntryType.Guest, userNameString);
		}

		private void InitOtherSatellite()
		{
			EntryMonitor otherMonitor = EntryMonitor.OtherMonitor;
			if (!otherMonitor.IsDecide)
			{
				EntryMonitor.ShowMachine(otherMonitor.MonitorIndex, SatelliteEntryType.None, "");
				_watchOther = true;
			}
			else
			{
				string userNameString = GetUserNameString(otherMonitor.IsGuest(), otherMonitor.IsNewUser, otherMonitor.UserName);
				EntryMonitor.ShowMachine(otherMonitor.MonitorIndex, (!otherMonitor.IsGuest()) ? SatelliteEntryType.Aime : SatelliteEntryType.Guest, userNameString);
				_watchOther = false;
			}
		}

		protected override void StateUpdate(float deltaTime)
		{
			int monitorIndex = base.Monitor.MonitorIndex;
			InputManager.ButtonSetting buttonSetting = _button.ButtonSetting;
			if (EntryMonitor.OtherMonitor.IsDecide)
			{
				if (InputManager.GetButtonDown(monitorIndex, buttonSetting))
				{
					EntryMonitor.Process.DecrementTimerSecond(30);
				}
				else if (InputManager.GetButtonLongPush(monitorIndex, buttonSetting, 200L))
				{
					EntryMonitor.Process.IsFastSkip(isFastSkip: true);
				}
			}
			else if (InputManager.GetButtonDown(monitorIndex, buttonSetting))
			{
				_pushCount++;
				_pushEffectiveTime = 0f;
				EntryMonitor.Process.DecrementTimerSecond((_pushCount < 4) ? 1 : 30);
			}
			else if (InputManager.GetButtonLongPush(monitorIndex, buttonSetting, 200L))
			{
				_pushCount = 0;
				_pushEffectiveTime = 0f;
				EntryMonitor.Process.IsFastSkip(isFastSkip: true);
			}
			else
			{
				_pushEffectiveTime += deltaTime;
				if (_pushEffectiveTime >= 2f)
				{
					_pushCount = 0;
				}
			}
			if (_watchOther && EntryMonitor.OtherMonitor.IsDecide)
			{
				InitOtherSatellite();
			}
		}

		private static string GetUserNameString(bool isGuest, bool isNewUser, string userName)
		{
			if (string.IsNullOrEmpty(userName))
			{
				return "";
			}
			string text = ((!isNewUser) ? userName : CommonMessageID.NewUserName.GetName());
			return isGuest ? ("<color=#202020FF>" + text + "</color>") : ("<color=#FF7C5EFF>" + text + "</color>");
		}
	}
}
