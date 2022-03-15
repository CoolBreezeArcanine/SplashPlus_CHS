using System.Collections.Generic;
using System.Linq;
using MAI2.Util;
using Manager;
using Monitor.Common;
using Monitor.Entry.Util;

namespace Monitor.Entry.Parts.Screens
{
	public class ConfirmEntry : EntryScreen
	{
		private IEnumerable<InputManager.ButtonSetting> _unuseBs;

		private bool _isDelay;

		private bool _isOfflineGrayIcon;

		private bool _isEnableAccessCode;

		private bool _isCallButton;

		private int _count;

		public override void Open(params object[] args)
		{
			bool flag = (bool)args[0];
			base.Open(args);
			OpenOperationInformation(OperationInformationController.InformationType.Hide);
			EntryMonitor.OpenPromotion(PromotionType.None);
			EntryMonitor.CallShowImmidiateHide();
			EntryMonitor.SetIsShowFlag(flag: true);
			_isCallButton = false;
			_count = 0;
			_isEnableAccessCode = Singleton<OperationManager>.Instance.IsAliveAimeReader;
			if (!Singleton<OperationManager>.Instance.IsAliveAimeServer)
			{
				_isEnableAccessCode = false;
			}
			if (args != null)
			{
				uint num = (uint)args.Length;
				if (num == 3)
				{
					_isOfflineGrayIcon = (bool)args[1];
					_isEnableAccessCode = (bool)args[2];
				}
			}
			if (flag)
			{
				CreateButton(ButtonType.GuestPlay, delegate
				{
					EntryMonitor.ResponseYes();
				});
				CreateButton(ButtonType.AccessCode, delegate
				{
					EntryMonitor.ResponseAccessCode();
				}, () => _isEnableAccessCode);
			}
			else
			{
				CreateButton(ButtonType.GuestPlay, delegate
				{
					EntryMonitor.ResponseYes();
				});
				CreateButton(ButtonType.AccessCode, delegate
				{
					EntryMonitor.ResponseAccessCode();
				}, () => _isEnableAccessCode);
			}
			IEnumerable<InputManager.ButtonSetting> first = from i in Enumerable.Range(0, 8)
				select (InputManager.ButtonSetting)i;
			_unuseBs = first.Except(Buttons.Select((EntryButton i) => i.ButtonSetting)).ToArray();
		}

		protected override void StateUpdate(float deltaTime)
		{
			_count++;
			if (_count >= 600)
			{
				_count = 600;
			}
			if (!_isDelay && !Buttons.Any((EntryButton i) => i.IsLock))
			{
				if (_unuseBs.Any((InputManager.ButtonSetting bs) => InputManager.GetButtonDown(base.Monitor.MonitorIndex, bs)))
				{
					EntryMonitor.Process.DecrementTimerSecond(30);
				}
				else if (_unuseBs.Any((InputManager.ButtonSetting bs) => InputManager.GetButtonLongPush(base.Monitor.MonitorIndex, bs, 200L)))
				{
					EntryMonitor.Process.IsFastSkip(isFastSkip: true);
				}
				if (_count >= 2 && !_isCallButton)
				{
					ActivateButtons();
					_isCallButton = true;
				}
			}
		}
	}
}
