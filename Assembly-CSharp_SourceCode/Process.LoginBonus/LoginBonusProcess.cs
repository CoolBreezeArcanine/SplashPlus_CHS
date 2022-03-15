using DB;
using MAI2.Util;
using Manager;
using Monitor.LoginBonus;
using Process.CodeRead;
using UnityEngine;

namespace Process.LoginBonus
{
	public class LoginBonusProcess : ProcessBase
	{
		private enum LoginBonusState : byte
		{
			Wait,
			Disp,
			Released
		}

		private LoginBonusMonitor[] _monitors;

		private LoginBonusState _state;

		public bool[] _isMajorVersionUp = new bool[2];

		public LoginBonusProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public LoginBonusProcess(ProcessDataContainer dataContainer, params object[] args)
			: base(dataContainer)
		{
			if (args == null)
			{
				return;
			}
			uint num = (uint)args.Length;
			if (num == 2)
			{
				for (int i = 0; i < 2; i++)
				{
					_isMajorVersionUp[i] = (bool)args[i];
				}
			}
		}

		public override void OnAddProcess()
		{
		}

		public override void OnRelease()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				Object.Destroy(_monitors[i].gameObject);
			}
		}

		public override void OnStart()
		{
			GameObject prefs = Resources.Load<GameObject>("Process/LoginBonus/LoginBonusProcess");
			_monitors = new LoginBonusMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<LoginBonusMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<LoginBonusMonitor>()
			};
			for (int i = 0; i < _monitors.Length; i++)
			{
				_monitors[i].SetAssetManager(container.assetManager);
				_monitors[i].Initialize(i, Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry);
				_monitors[i]._isMajorVersionUp = _isMajorVersionUp[i];
			}
			for (int j = 0; j < _monitors.Length; j++)
			{
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(j);
				if (userData.IsEntry && userData.IsGuest())
				{
					container.processManager.EnqueueMessage(j, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
				}
			}
			bool flag = false;
			for (int k = 0; k < _monitors.Length; k++)
			{
				if (_monitors[k]._isEntry && !_monitors[k]._isGuest && _monitors[k]._isEnableLoginBonus && !_monitors[k]._isSetCurrentCard)
				{
					flag = true;
					break;
				}
			}
			bool flag2 = false;
			if (flag)
			{
				container.processManager.PrepareTimer(60, 0, isEntry: false, TimeUp);
				for (int l = 0; l < _monitors.Length; l++)
				{
					if (_monitors[l]._isEntry && !_monitors[l]._isGuest)
					{
						if (_monitors[l]._isEnableLoginBonus)
						{
							if (_monitors[l]._isSetCurrentCard)
							{
								container.processManager.SetVisibleTimer(l, isVisible: false);
								continue;
							}
							_monitors[l]._isEnableTimer = true;
							_monitors[l]._isTimerVisible = true;
							_monitors[l]._isBothDecideOK = false;
						}
						else if (flag2)
						{
							_monitors[l]._isEnableTimer = true;
							_monitors[l]._isTimerVisible = true;
							_monitors[l]._isBothDecideOK = true;
						}
						else
						{
							container.processManager.SetVisibleTimer(l, isVisible: false);
						}
					}
					else if (flag2)
					{
						_monitors[l]._isEnableTimer = true;
						_monitors[l]._isTimerVisible = true;
						_monitors[l]._isBothDecideOK = true;
					}
					else
					{
						container.processManager.SetVisibleTimer(l, isVisible: false);
					}
				}
			}
			container.processManager.NotificationFadeIn();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			switch (_state)
			{
			case LoginBonusState.Wait:
				_state = LoginBonusState.Disp;
				break;
			case LoginBonusState.Disp:
			{
				if (_monitors[0].IsEnd() && _monitors[1].IsEnd())
				{
					_state = LoginBonusState.Released;
					bool flag = false;
					for (int i = 0; i < _monitors.Length; i++)
					{
						if (!_monitors[i]._isSetCurrentCard)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						container.processManager.ClearTimeoutAction();
						container.processManager.IsTimeCounting(isTimeCount: false);
						container.processManager.SetVisibleTimers(isVisible: false);
					}
					ProcessBase processBase = null;
					processBase = (GameManager.IsGotoCodeRead ? new CodeReadProcess(container) : (GameManager.IsGotoPhotoShoot ? new PhotoShootProcess(container) : ((GameManager.IsCourseMode || GameManager.IsFreedomMode) ? ((ProcessBase)new GetMusicProcess(container)) : ((ProcessBase)new RegionalSelectProcess(container)))));
					if (_monitors[0].IsEnd() && _monitors[1].IsEnd())
					{
						container.processManager.AddProcess(processBase, 50);
						container.processManager.ReleaseProcess(this);
					}
					else
					{
						container.processManager.AddProcess(new FadeProcess(container, this, processBase), 50);
					}
					break;
				}
				for (int j = 0; j < _monitors.Length; j++)
				{
					if (_monitors[j].IsEnd() && !container.processManager.IsOpening(j, WindowPositionID.Middle))
					{
						UserData userData = Singleton<UserDataManager>.Instance.GetUserData(j);
						if (userData.IsEntry && !userData.IsGuest())
						{
							_monitors[j].SetLastBlur();
							container.processManager.EnqueueMessage(j, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
						}
					}
				}
				break;
			}
			}
			for (int k = 0; k < _monitors.Length; k++)
			{
				LoginBonusMonitor loginBonusMonitor = _monitors[k];
				if (loginBonusMonitor._isDispInfoWindow)
				{
					int num = 0;
					if (loginBonusMonitor._info_timer != 0)
					{
						loginBonusMonitor._info_timer--;
					}
					WindowMessageID messageId = WindowMessageID.LoginBonusFirst01;
					uint info_timer = 180u;
					uint info_timer2 = 420u;
					if (loginBonusMonitor._isDispInfoWindow)
					{
						switch (loginBonusMonitor._info_count)
						{
						case 0u:
							messageId = WindowMessageID.LoginBonusFirst01;
							break;
						case 1u:
							messageId = WindowMessageID.LoginBonusFirst02;
							break;
						case 2u:
							messageId = WindowMessageID.LoginBonusFirst03;
							break;
						}
						info_timer = 120u;
						info_timer2 = 420u;
					}
					switch (loginBonusMonitor._info_state)
					{
					case LoginBonusMonitor.InfoWindowState.None:
					{
						GameObject blurObject2 = loginBonusMonitor.GetBlurObject();
						num = blurObject2.transform.GetSiblingIndex();
						blurObject2.transform.SetSiblingIndex(num - 1);
						loginBonusMonitor._isChangeSibling = true;
						loginBonusMonitor.SetBlur();
						loginBonusMonitor.ResetOKStart();
						if (!container.processManager.IsOpening(k, WindowPositionID.Middle))
						{
							container.processManager.EnqueueMessage(k, messageId, WindowPositionID.Middle);
							loginBonusMonitor._info_state = LoginBonusMonitor.InfoWindowState.Open;
						}
						break;
					}
					case LoginBonusMonitor.InfoWindowState.Judge:
						loginBonusMonitor.ResetOKStart();
						if (!container.processManager.IsOpening(k, WindowPositionID.Middle))
						{
							container.processManager.EnqueueMessage(k, messageId, WindowPositionID.Middle);
							loginBonusMonitor._info_state = LoginBonusMonitor.InfoWindowState.Open;
						}
						break;
					case LoginBonusMonitor.InfoWindowState.Open:
						loginBonusMonitor._info_state = LoginBonusMonitor.InfoWindowState.OpenWait;
						loginBonusMonitor._info_timer = info_timer;
						if (!loginBonusMonitor._isInfoWindowVoice)
						{
							loginBonusMonitor._isInfoWindowVoice = true;
						}
						break;
					case LoginBonusMonitor.InfoWindowState.OpenWait:
						if (loginBonusMonitor._info_timer == 0)
						{
							loginBonusMonitor.OKStart();
							loginBonusMonitor._info_state = LoginBonusMonitor.InfoWindowState.Wait;
							loginBonusMonitor._info_timer = info_timer2;
						}
						break;
					case LoginBonusMonitor.InfoWindowState.Wait:
						if (loginBonusMonitor._isOK)
						{
							loginBonusMonitor._info_timer = 0u;
						}
						if (loginBonusMonitor._info_timer == 0)
						{
							loginBonusMonitor.ResetOKStart();
							loginBonusMonitor._info_state = LoginBonusMonitor.InfoWindowState.Close;
						}
						break;
					case LoginBonusMonitor.InfoWindowState.Close:
						container.processManager.CloseWindow(k);
						if (loginBonusMonitor._info_count >= 2)
						{
							if (loginBonusMonitor._isEntry && !loginBonusMonitor._isGuest && loginBonusMonitor._isDispInfoWindow)
							{
								Singleton<UserDataManager>.Instance.GetUserData(loginBonusMonitor.MonitorIndex).Detail.ContentBit.SetFlag(ContentBitID.FirstLoginBonus, flag: true);
							}
							loginBonusMonitor._info_state = LoginBonusMonitor.InfoWindowState.CloseWait;
						}
						else
						{
							loginBonusMonitor._info_count++;
							loginBonusMonitor._info_state = LoginBonusMonitor.InfoWindowState.Judge;
						}
						break;
					case LoginBonusMonitor.InfoWindowState.CloseWait:
					{
						loginBonusMonitor._info_state = LoginBonusMonitor.InfoWindowState.End;
						if (loginBonusMonitor._isDispInfoWindow)
						{
							loginBonusMonitor._isDispInfoWindow = false;
						}
						loginBonusMonitor.ResetBlur();
						GameObject blurObject = loginBonusMonitor.GetBlurObject();
						num = blurObject.transform.GetSiblingIndex();
						blurObject.transform.SetSiblingIndex(num + 1);
						loginBonusMonitor._isChangeSibling = false;
						break;
					}
					}
				}
				_monitors[k].ViewUpdate();
				if (_monitors[k]._isEnableTimer && _monitors[k]._isDecidedOK && _monitors[k]._isTimerVisible)
				{
					container.processManager.IsTimeCounting(k, isTimeCount: false);
					container.processManager.SetVisibleTimer(k, isVisible: false);
					_monitors[k]._isTimerVisible = false;
				}
			}
			for (int l = 0; l < _monitors.Length; l++)
			{
				if (!_monitors[l]._isBothDecideOK || _monitors[l]._isDecidedOK)
				{
					continue;
				}
				for (int m = 0; m < _monitors.Length; m++)
				{
					if (l != m && !_monitors[m]._isBothDecideOK && _monitors[m]._isDecidedOK)
					{
						_monitors[l]._isDecidedOK = true;
						break;
					}
				}
			}
			for (int n = 0; n < _monitors.Length; n++)
			{
				if (_monitors[n]._isEnableTimer && _monitors[n]._isDecidedOK && !_monitors[n]._isTimeUp)
				{
					container.processManager.ForceTimeUp(n);
					_monitors[n]._isTimeUp = true;
				}
			}
		}

		protected override void UpdateInput(int monitorId)
		{
			if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button04))
			{
				_monitors[monitorId].OKButtonAnim(InputManager.ButtonSetting.Button04);
			}
			else if (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3) || InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L))
			{
				_monitors[monitorId].RightButtonAnim(InputManager.ButtonSetting.Button03);
			}
			else if (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6) || InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L))
			{
				_monitors[monitorId].LeftButtonAnim(InputManager.ButtonSetting.Button06);
			}
		}

		public override void OnLateUpdate()
		{
		}

		private void TimeUp()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				_ = _monitors[i];
				_monitors[i]._isTimeUp = true;
				_monitors[i]._isDecidedOK = true;
			}
		}
	}
}
