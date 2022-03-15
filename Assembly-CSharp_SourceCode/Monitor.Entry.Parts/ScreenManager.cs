using System;
using System.Collections.Generic;
using Monitor.Entry.Parts.Screens;
using Monitor.Entry.Util;
using Monitor.MapCore;
using UnityEngine;

namespace Monitor.Entry.Parts
{
	public class ScreenManager : MapBehaviour
	{
		[SerializeField]
		private BgBlur _bgBlur;

		private Dictionary<ScreenType, EntryScreen> _screens;

		private ScreenType _currentScreenType;

		public void Initialize()
		{
			foreach (Transform item in base.transform)
			{
				item.gameObject.SetActive(value: true);
			}
			_screens = new Dictionary<ScreenType, EntryScreen>
			{
				[ScreenType.None] = null,
				[ScreenType.ConfirmEntry] = GetComponentInChildren<ConfirmEntry>(),
				[ScreenType.ConfirmGuest] = GetComponentInChildren<ConfirmGuest>(),
				[ScreenType.DisplayPleaseWait] = GetComponentInChildren<DisplayPleaseWait>(),
				[ScreenType.ConfirmNewAime] = GetComponentInChildren<ConfirmNewAime>(),
				[ScreenType.ConfirmNewUser] = GetComponentInChildren<ConfirmNewUser>(),
				[ScreenType.ConfirmExistingAime] = GetComponentInChildren<ConfirmExistingAime>(),
				[ScreenType.ConfirmInheritAime] = GetComponentInChildren<ConfirmInheritAime>(),
				[ScreenType.ConfirmFreedom] = GetComponentInChildren<ConfirmFreedom>(),
				[ScreenType.ErrorAime] = GetComponentInChildren<ErrorAime>(),
				[ScreenType.WaitPartner] = GetComponentInChildren<WaitPartner>(),
				[ScreenType.DoneEntry] = GetComponentInChildren<DoneEntry>(),
				[ScreenType.ConfirmFelica] = GetComponentInChildren<ConfirmFelica>(),
				[ScreenType.NoticeFelicaRegistration] = GetComponentInChildren<NoticeFelicaRegistration>(),
				[ScreenType.ConfirmFelicaSite] = GetComponentInChildren<ConfirmFelicaSite>(),
				[ScreenType.DisplayFelicaQR] = GetComponentInChildren<DisplayFelicaQR>(),
				[ScreenType.ConfirmAccessCode] = GetComponentInChildren<ConfirmAccessCode>(),
				[ScreenType.DisplayAccessCodeQR] = GetComponentInChildren<DisplayAccessCodeQR>(),
				[ScreenType.ErrorAccessCode] = GetComponentInChildren<ErrorAccessCode>(),
				[ScreenType.WindowGeneral] = GetComponentInChildren<WindowGeneral>(),
				[ScreenType.ConfirmExistingAimeContinue] = GetComponentInChildren<ConfirmExistingAimeContinue>()
			};
			foreach (Transform item2 in base.transform)
			{
				item2.gameObject.SetActive(value: false);
			}
		}

		public void OpenScreen(ScreenType type, params object[] args)
		{
			if (_currentScreenType != type)
			{
				RenewalBgBlur(type);
				CloseScreen();
				_screens[type].Open(args);
				_currentScreenType = type;
			}
		}

		public void CloseScreen()
		{
			if (_currentScreenType != 0)
			{
				_screens[_currentScreenType].Close();
				_currentScreenType = ScreenType.None;
			}
		}

		public void LockScreenButtons()
		{
			if (_currentScreenType != ScreenType.WaitPartner)
			{
				_screens[_currentScreenType].LockButtons();
			}
		}

		private void RenewalBgBlur(ScreenType screenNew)
		{
			int num = Convert.ToInt32(_screens[_currentScreenType]?.UseBlurBg ?? false);
			int num2 = Convert.ToInt32(_screens[screenNew]?.UseBlurBg ?? false);
			switch ((num << 1) | num2)
			{
			default:
				_bgBlur.ReferenceCanvasGroup = null;
				break;
			case 1:
				_bgBlur.ReferenceCanvasGroup = _screens[screenNew].GetComponent<CanvasGroup>();
				break;
			case 2:
				_bgBlur.ReferenceCanvasGroup = _screens[_currentScreenType].GetComponent<CanvasGroup>();
				break;
			}
		}
	}
}
