using AMDaemon;
using Balance;
using DB;
using IO;
using MAI2.Util;
using Manager;
using Monitor.TicketSelect;
using UnityEngine;

namespace Process.TicketSelect
{
	public class TicketSelectProcess : ProcessBase
	{
		private enum ProcessState : byte
		{
			Wait,
			Disp,
			Released
		}

		private TicketSelectMonitor[] _monitors;

		private ProcessState _state;

		public bool _isTimeUp;

		public bool _isCallSettingBlur;

		public bool _isCallCreditMain;

		public bool _isBlockingDecideCredit;

		public int _blockingCount;

		public int _creditPrevius;

		public int _creditCurrent;

		public int _creditCost;

		public bool _isBlockingDecideBalance;

		public int _blockingCount2;

		public bool _isBlockingDecideTicket;

		public int _blockingCount3;

		public TicketSelectProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
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
			GameObject prefs = Resources.Load<GameObject>("Process/TicketSelect/TicketSelectProcess");
			_monitors = new TicketSelectMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<TicketSelectMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<TicketSelectMonitor>()
			};
			for (int i = 0; i < _monitors.Length; i++)
			{
				_monitors[i].Initialize(i, active: true);
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			for (int j = 0; j < _monitors.Length; j++)
			{
				if (_monitors[j]._isEntry)
				{
					if (_monitors[j]._isGuest)
					{
						num2++;
					}
					switch (Singleton<UserDataManager>.Instance.GetUserData(j).UserType)
					{
					case UserData.UserIDType.New:
						num3++;
						break;
					}
					if (!_monitors[j]._isGuest)
					{
						num++;
					}
				}
			}
			for (int k = 0; k < _monitors.Length; k++)
			{
				_monitors[k]._isEntryNum = num;
			}
			_ = _monitors.Length;
			for (int l = 0; l < _monitors.Length; l++)
			{
				_monitors[l].SetMiniSelector();
			}
			bool flag = false;
			for (int m = 0; m < _monitors.Length; m++)
			{
				if (_monitors[m]._isAwake)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				container.processManager.PrepareTimer(99, 0, isEntry: false, TimeUp);
				int num4 = 0;
				for (int n = 0; n < _monitors.Length; n++)
				{
					if (_monitors[n]._isAwake)
					{
						_monitors[n]._isEnableTimer = true;
						_monitors[n]._isTimerVisible = true;
						_monitors[n]._isBothDecidedEntry = false;
						num4++;
					}
					else
					{
						container.processManager.SetVisibleTimer(n, isVisible: false);
					}
				}
				if (num4 == 2)
				{
					for (int num5 = 0; num5 < _monitors.Length; num5++)
					{
						_monitors[num5]._isBothDecidedEntry = true;
					}
				}
			}
			_creditPrevius = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.NowCredit;
			_creditCurrent = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.NowCredit;
			_creditCost = 0;
			container.processManager.NotificationFadeIn();
		}

		public override void OnUpdate()
		{
			_creditPrevius = _creditCurrent;
			_creditCost = 0;
			base.OnUpdate();
			switch (_state)
			{
			case ProcessState.Wait:
				_state = ProcessState.Disp;
				break;
			case ProcessState.Disp:
			{
				if (_monitors[0].IsEnd() && _monitors[1].IsEnd())
				{
					_state = ProcessState.Released;
					GameManager.SetMaxTrack();
					container.processManager.AddProcess(new FadeProcess(container, this, new GetMusicProcess(container)), 50);
					if (_isCallCreditMain)
					{
						container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30001));
						_isCallCreditMain = false;
					}
					for (int i = 0; i < _monitors.Length; i++)
					{
						if (_monitors[i]._isAwake && _monitors[i]._isDecidedEntry && _monitors[i]._isEntryNum >= 2)
						{
							if (_monitors[i]._decidedSelectTicketID <= 0 && container.processManager.IsOpening(i, WindowPositionID.Middle) && _monitors[i]._isDecidedEntryNum >= _monitors[i]._isEntryNum && !_monitors[i]._isServerRejected)
							{
								container.processManager.CloseWindow(i);
							}
						}
						else if (!_monitors[i]._isAwake && _monitors[i]._isEntry && _monitors[i]._isGuest && container.processManager.IsOpening(i, WindowPositionID.Middle))
						{
							container.processManager.CloseWindow(i);
						}
					}
					break;
				}
				if (!_isCallCreditMain)
				{
					for (int j = 0; j < _monitors.Length; j++)
					{
						if (_monitors[j]._isAwake && _monitors[j]._state >= TicketSelectMonitor.MonitorState.SelectCardFadeInJudge)
						{
							container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30000));
							_isCallCreditMain = true;
							break;
						}
					}
				}
				for (int k = 0; k < _monitors.Length; k++)
				{
					if (_monitors[k]._isAwake && _monitors[k]._isDecidedEntry)
					{
						if (_monitors[k]._isEntryNum >= 2)
						{
							if (_monitors[k]._decidedSelectTicketID > 0)
							{
								continue;
							}
							if (!container.processManager.IsOpening(k, WindowPositionID.Middle) || _monitors[k]._isCallRejectedInfo)
							{
								if (_monitors[k]._isDecidedEntryNum >= _monitors[k]._isEntryNum)
								{
									if (_monitors[k]._isCallRejectedInfo)
									{
										_monitors[k]._isCallRejectedInfo = false;
										container.processManager.EnqueueMessage(k, WindowMessageID.TicketConnectFailed, WindowPositionID.Middle);
									}
								}
								else if (_monitors[k]._isCallRejectedInfo)
								{
									_monitors[k]._isCallRejectedInfo = false;
									container.processManager.EnqueueMessage(k, WindowMessageID.TicketConnectFailed, WindowPositionID.Middle);
								}
								else
								{
									container.processManager.EnqueueMessage(k, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
								}
							}
							else if (container.processManager.IsOpening(k, WindowPositionID.Middle) && _monitors[k]._isDecidedEntryNum >= _monitors[k]._isEntryNum && !_monitors[k]._isServerRejected)
							{
								container.processManager.CloseWindow(k);
							}
						}
						else if (_monitors[k]._isEntryNum >= 1 && _monitors[k]._isCallRejectedInfo)
						{
							_monitors[k]._isCallRejectedInfo = false;
							container.processManager.EnqueueMessage(k, WindowMessageID.TicketConnectFailed, WindowPositionID.Middle);
						}
					}
					else if (!_monitors[k]._isAwake && _monitors[k]._isEntry && _monitors[k]._isGuest)
					{
						if (!container.processManager.IsOpening(k, WindowPositionID.Middle))
						{
							container.processManager.EnqueueMessage(k, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
						}
						if (!_monitors[k]._isCallLastBlur)
						{
							_monitors[k]._isCallLastBlur = true;
						}
					}
				}
				break;
			}
			}
			if (_isCallCreditMain)
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				int num6 = 0;
				bool flag = false;
				int num7 = 0;
				for (int l = 0; l < _monitors.Length; l++)
				{
					if (!_monitors[l]._isAwake)
					{
						continue;
					}
					num = _monitors[l].GetSelectorCreditNum(_monitors[l]._decidedSelectTicketID);
					num2 = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.NowCredit;
					num5 = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.CoinToCredit;
					num6 = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.Remain;
					if (num5 > 1)
					{
						flag = true;
						num7 = num5 * num;
						num7 -= num5 * num2;
						if (num7 < 0)
						{
							num7 = 0;
						}
						num7 -= num6;
						if (num7 < 0)
						{
							num7 = 0;
						}
						num3 = num7 / num5;
						num7 %= num5;
					}
					else if (!flag)
					{
						num3 = num - num2;
						if (num3 < 0)
						{
							num3 = 0;
						}
					}
					if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.IsFreePlay())
					{
						num3 = 0;
					}
					if (flag)
					{
						if (num3 != 0 || num7 != 0)
						{
							num4 = num3;
							_monitors[l]._needCreditNum = num3;
							_monitors[l]._needCoinNum = num4;
							_monitors[l]._needCreditDenominator = num5;
							_monitors[l]._needCreditNumerator = num7;
							_monitors[l]._isNeedCreditFraction = true;
						}
						else
						{
							num4 = 0;
							_monitors[l]._needCreditNum = 0;
							_monitors[l]._needCoinNum = 0;
							_monitors[l]._needCreditDenominator = num5;
							_monitors[l]._needCreditNumerator = 0;
							_monitors[l]._isNeedCreditFraction = true;
						}
					}
					else if (!flag)
					{
						if (num3 > 0)
						{
							num4 = num3;
							_monitors[l]._needCreditNum = num3;
							_monitors[l]._needCoinNum = num4;
							_monitors[l]._needCreditDenominator = 1;
							_monitors[l]._needCreditNumerator = 0;
							_monitors[l]._isNeedCreditFraction = false;
						}
						else
						{
							num4 = 0;
							_monitors[l]._needCreditNum = 0;
							_monitors[l]._needCoinNum = 0;
							_monitors[l]._needCreditDenominator = 1;
							_monitors[l]._needCreditNumerator = 0;
							_monitors[l]._isNeedCreditFraction = false;
						}
					}
					_monitors[l].SetNeedCreditNum();
				}
				if (_isBlockingDecideCredit)
				{
					_blockingCount++;
					if (_blockingCount > 5)
					{
						_isBlockingDecideCredit = false;
						for (int m = 0; m < _monitors.Length; m++)
						{
							if (_monitors[m]._isAwake)
							{
								_monitors[m]._isBlockingDecideCredit = false;
							}
						}
					}
				}
				if (_isBlockingDecideBalance)
				{
					_blockingCount2++;
					if (_blockingCount2 > 5)
					{
						_isBlockingDecideBalance = false;
						for (int n = 0; n < _monitors.Length; n++)
						{
							if (_monitors[n]._isAwake)
							{
								_monitors[n]._isBlockingDecideBalance = false;
							}
						}
					}
				}
				if (_isBlockingDecideTicket)
				{
					_blockingCount3++;
					if (_blockingCount3 > 5)
					{
						_isBlockingDecideTicket = false;
						for (int num8 = 0; num8 < _monitors.Length; num8++)
						{
							if (_monitors[num8]._isAwake)
							{
								_monitors[num8]._isBlockingDecideTicket = false;
							}
						}
					}
				}
				int num9 = 0;
				for (int num10 = 0; num10 < _monitors.Length; num10++)
				{
					int num11 = (num10 + 1) % _monitors.Length;
					if (_monitors[num10]._isAwake && _monitors[num10]._isResetAnotherBalanceBlocking)
					{
						if (_monitors[num11]._isAwake && _monitors[num11]._isOwnBalanceBlocking)
						{
							_monitors[num11]._isOwnBalanceBlocking = false;
						}
						_monitors[num10]._isResetAnotherBalanceBlocking = false;
						if (num9 == 0)
						{
							Singleton<OperationManager>.Instance.SetCoinBlockerMode(CoinBlocker.Mode.Game);
							num9++;
						}
					}
				}
				if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsEnableEmoneyExecute)
				{
					for (int num12 = 0; num12 < _monitors.Length; num12++)
					{
						if (!_monitors[num12]._isAwake)
						{
							continue;
						}
						if ((AMDaemon.EMoney.IsAuthCompleted && (!AMDaemon.EMoney.IsServiceAlive || !AMDaemon.EMoney.Operation.CanOperateDeal)) || SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsEmoneyUnconfirm)
						{
							if (!_monitors[num12]._isBalanceNetworkDisconnection)
							{
								_monitors[num12]._isBalanceNetworkDisconnection = true;
							}
						}
						else if (((AMDaemon.EMoney.IsAuthCompleted && AMDaemon.EMoney.IsServiceAlive && AMDaemon.EMoney.Operation.CanOperateDeal) || !SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsEmoneyUnconfirm) && _monitors[num12]._isBalanceNetworkDisconnection)
						{
							_monitors[num12]._isBalanceNetworkDisconnection = false;
						}
					}
				}
			}
			for (int num13 = 0; num13 < _monitors.Length; num13++)
			{
				if (_monitors[num13]._isDispInfoWindow)
				{
					int num14 = 0;
					int num15 = 6;
					if (_monitors[num13]._info_timer != 0)
					{
						_monitors[num13]._info_timer--;
					}
					WindowMessageID windowMessageID = WindowMessageID.TicketSelectFirst;
					uint info_timer = 180u;
					uint info_timer2 = 420u;
					if (_monitors[num13]._isDispInfoWindow)
					{
						if (_monitors[num13]._windowMessageList.Count > 0 && _monitors[num13]._info_count < _monitors[num13]._windowMessageList.Count)
						{
							windowMessageID = _monitors[num13]._windowMessageList[(int)_monitors[num13]._info_count];
						}
						info_timer = 120u;
						info_timer2 = 420u;
					}
					switch (_monitors[num13]._info_state)
					{
					case TicketSelectMonitor.InfoWindowState.None:
					{
						GameObject blurObject2 = _monitors[num13].GetBlurObject();
						num14 = blurObject2.transform.GetSiblingIndex();
						blurObject2.transform.SetSiblingIndex(num14 - num15);
						_monitors[num13]._isChangeSibling = true;
						_monitors[num13].SetBlur();
						_monitors[num13].ResetOKStart();
						if (!container.processManager.IsOpening(num13, WindowPositionID.Middle))
						{
							container.processManager.EnqueueMessage(num13, windowMessageID, WindowPositionID.Middle);
							_monitors[num13]._info_state = TicketSelectMonitor.InfoWindowState.Open;
						}
						break;
					}
					case TicketSelectMonitor.InfoWindowState.Judge:
						_monitors[num13].ResetOKStart();
						if (!container.processManager.IsOpening(num13, WindowPositionID.Middle))
						{
							container.processManager.EnqueueMessage(num13, windowMessageID, WindowPositionID.Middle);
							_monitors[num13]._info_state = TicketSelectMonitor.InfoWindowState.Open;
						}
						break;
					case TicketSelectMonitor.InfoWindowState.Open:
						_monitors[num13]._info_state = TicketSelectMonitor.InfoWindowState.OpenWait;
						_monitors[num13]._info_timer = info_timer;
						if (!_monitors[num13]._isInfoWindowVoice)
						{
							_monitors[num13]._isInfoWindowVoice = true;
						}
						break;
					case TicketSelectMonitor.InfoWindowState.OpenWait:
						if (_monitors[num13]._info_timer == 0)
						{
							_monitors[num13].OKStart();
							_monitors[num13]._info_state = TicketSelectMonitor.InfoWindowState.Wait;
							_monitors[num13]._info_timer = info_timer2;
						}
						break;
					case TicketSelectMonitor.InfoWindowState.Wait:
						if (_monitors[num13]._isOK)
						{
							_monitors[num13]._info_timer = 0u;
						}
						if (_monitors[num13]._info_timer == 0)
						{
							_monitors[num13].ResetOKStart();
							_monitors[num13]._info_state = TicketSelectMonitor.InfoWindowState.Close;
						}
						break;
					case TicketSelectMonitor.InfoWindowState.Close:
						container.processManager.CloseWindow(num13);
						if (_monitors[num13]._windowMessageList.Count <= 0 || _monitors[num13]._info_count >= _monitors[num13]._windowMessageList.Count - 1)
						{
							if (_monitors[num13]._isEntry && !_monitors[num13]._isGuest && _monitors[num13]._isDispInfoWindow)
							{
								if (_monitors[num13]._windowMessageList.Count > 0 && _monitors[num13]._info_count < _monitors[num13]._windowMessageList.Count)
								{
									windowMessageID = _monitors[num13]._windowMessageList[(int)_monitors[num13]._info_count];
								}
								if (windowMessageID == WindowMessageID.TicketSelectFirst)
								{
									Singleton<UserDataManager>.Instance.GetUserData(num13).Detail.ContentBit.SetFlag(ContentBitID.FirstTicketSelect, flag: true);
								}
							}
							_monitors[num13]._info_state = TicketSelectMonitor.InfoWindowState.CloseWait;
						}
						else
						{
							_monitors[num13]._info_count++;
							_monitors[num13]._info_state = TicketSelectMonitor.InfoWindowState.Judge;
						}
						break;
					case TicketSelectMonitor.InfoWindowState.CloseWait:
					{
						_monitors[num13]._info_state = TicketSelectMonitor.InfoWindowState.End;
						if (_monitors[num13]._isDispInfoWindow)
						{
							_monitors[num13]._isDispInfoWindow = false;
						}
						_monitors[num13].ResetBlur();
						GameObject blurObject = _monitors[num13].GetBlurObject();
						num14 = blurObject.transform.GetSiblingIndex();
						blurObject.transform.SetSiblingIndex(num14 + num15);
						_monitors[num13]._isChangeSibling = false;
						break;
					}
					}
				}
				_monitors[num13].ViewUpdate();
				if (_monitors[num13]._isEnableTimer && _monitors[num13]._isDecidedEntry && _monitors[num13]._isTimerVisible)
				{
					container.processManager.IsTimeCounting(num13, isTimeCount: false);
					container.processManager.SetVisibleTimer(num13, isVisible: false);
					_monitors[num13]._isTimerVisible = false;
				}
			}
			int num16 = 0;
			int num17 = 6;
			for (int num18 = 0; num18 < _monitors.Length; num18++)
			{
				if (_monitors[num18]._isDispInfoWindow)
				{
					continue;
				}
				if (_monitors[num18]._isCallBlur)
				{
					if (!_monitors[num18]._isChangeSibling)
					{
						GameObject blurObject3 = _monitors[num18].GetBlurObject();
						num16 = blurObject3.transform.GetSiblingIndex();
						blurObject3.transform.SetSiblingIndex(num16 - num17);
						_monitors[num18]._isChangeSibling = true;
						_monitors[num18].SetBlur();
					}
				}
				else if (!_monitors[num18]._isCallBlur && _monitors[num18]._isChangeSibling)
				{
					GameObject blurObject4 = _monitors[num18].GetBlurObject();
					num16 = blurObject4.transform.GetSiblingIndex();
					blurObject4.transform.SetSiblingIndex(num16 + num17);
					_monitors[num18]._isChangeSibling = false;
					_monitors[num18].ResetBlur();
				}
				if (_monitors[num18]._isCallLastBlur)
				{
					if (!_monitors[num18]._isChangeLastBlur)
					{
						_monitors[num18]._isChangeLastBlur = true;
						_monitors[num18].SetLastBlur();
					}
				}
				else if (!_monitors[num18]._isCallLastBlur && _monitors[num18]._isChangeLastBlur)
				{
					_monitors[num18]._isChangeLastBlur = false;
					_monitors[num18].ResetLastBlur();
				}
			}
			for (int num19 = 0; num19 < _monitors.Length; num19++)
			{
				if (!_monitors[num19]._isBothDecidedEntry || _monitors[num19]._isDecidedEntry)
				{
					continue;
				}
				for (int num20 = 0; num20 < _monitors.Length; num20++)
				{
					if (num19 != num20 && !_monitors[num20]._isBothDecidedEntry && _monitors[num20]._isDecidedEntry)
					{
						_monitors[num19]._isDecidedEntry = true;
						break;
					}
				}
			}
			for (int num21 = 0; num21 < _monitors.Length; num21++)
			{
				if (_monitors[num21]._isEnableTimer && _monitors[num21]._isDecidedEntry && !_monitors[num21]._isTimeUp)
				{
					container.processManager.ForceTimeUp(num21);
					_monitors[num21]._isTimeUp = true;
				}
			}
			int num22 = 0;
			for (int num23 = 0; num23 < _monitors.Length; num23++)
			{
				if (_monitors[num23]._isAwake && _monitors[num23]._isDecidedEntry)
				{
					num22++;
				}
			}
			for (int num24 = 0; num24 < _monitors.Length; num24++)
			{
				_monitors[num24]._isDecidedEntryNum = num22;
			}
			_creditCurrent = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.NowCredit;
			if (!SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.IsAddCoinNow())
			{
				return;
			}
			int num25 = _creditPrevius - _creditCost;
			if (_creditCurrent - num25 <= 0)
			{
				return;
			}
			container.processManager.PrepareTimer(99, 0, isEntry: false, TimeUp);
			for (int num26 = 0; num26 < _monitors.Length; num26++)
			{
				if (_monitors[num26]._isAwake)
				{
					if (_monitors[num26]._isDecidedEntry || _monitors[num26]._isTimeUp)
					{
						container.processManager.ForceTimeUp(num26);
						container.processManager.SetVisibleTimer(num26, isVisible: false);
						_monitors[num26]._isEnableTimer = false;
					}
				}
				else
				{
					container.processManager.SetVisibleTimer(num26, isVisible: false);
				}
			}
		}

		protected override void UpdateInput(int monitorId)
		{
			if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button04))
			{
				if (_isCallCreditMain)
				{
					if (!_isBlockingDecideTicket && _monitors[monitorId]._isAwake)
					{
						if (_monitors[monitorId]._state >= TicketSelectMonitor.MonitorState.DecideCreditFadeIn && _monitors[monitorId]._state <= TicketSelectMonitor.MonitorState.DecideCreditWait)
						{
							bool flag = true;
							int num = (monitorId + 1) % _monitors.Length;
							if (_monitors[num]._isAwake && _monitors[num]._isServerRequest)
							{
								flag = false;
								if (_monitors[num]._isServerAccepted)
								{
									flag = true;
								}
							}
							if (_monitors[monitorId]._isSelectButton || _monitors[monitorId]._button_mode == TicketSelectMonitor.MonitorButtonMode.MonitorButtonModeEMoney)
							{
								flag = true;
							}
							if (flag)
							{
								_monitors[monitorId].OKButtonAnim(InputManager.ButtonSetting.Button04);
								_isBlockingDecideTicket = true;
								_blockingCount3 = 0;
								for (int i = 0; i < _monitors.Length; i++)
								{
									if (_monitors[i]._isAwake)
									{
										_monitors[i]._isBlockingDecideTicket = true;
									}
								}
							}
						}
						else
						{
							_monitors[monitorId].OKButtonAnim(InputManager.ButtonSetting.Button04);
						}
					}
				}
				else
				{
					_monitors[monitorId].OKButtonAnim(InputManager.ButtonSetting.Button04);
				}
			}
			else if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button05))
			{
				_monitors[monitorId].BackButtonAnim(InputManager.ButtonSetting.Button05);
			}
			else if (!_isBlockingDecideBalance && (_monitors[monitorId]._state == TicketSelectMonitor.MonitorState.InsertCoinWait || _monitors[monitorId]._state == TicketSelectMonitor.MonitorState.DecideCreditWait) && (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Select) || InputManager.GetButtonLongPush(monitorId, InputManager.ButtonSetting.Select, 1000L)))
			{
				if (_monitors[monitorId]._isAwake)
				{
					bool flag2 = false;
					int num2 = (monitorId + 1) % _monitors.Length;
					if (_monitors[num2]._isAwake && (_monitors[num2]._isSelectButton || _monitors[num2]._button_mode == TicketSelectMonitor.MonitorButtonMode.MonitorButtonModeEMoney))
					{
						flag2 = true;
					}
					if (!flag2 && _monitors[monitorId]._button_mode == TicketSelectMonitor.MonitorButtonMode.MonitorButtonModeCommon)
					{
						_monitors[monitorId].SelectButtonAnim(InputManager.ButtonSetting.Select);
						if (_monitors[monitorId]._isSelectButton)
						{
							_isBlockingDecideBalance = true;
							_blockingCount2 = 0;
							Singleton<OperationManager>.Instance.SetCoinBlockerMode(CoinBlocker.Mode.Balance);
							if (_monitors[num2]._isAwake)
							{
								_monitors[num2]._isOwnBalanceBlocking = true;
							}
							for (int j = 0; j < _monitors.Length; j++)
							{
								if (_monitors[j]._isAwake)
								{
									_monitors[j]._isBlockingDecideBalance = true;
								}
							}
						}
					}
				}
			}
			else if (_monitors[monitorId]._balanceCommon._state == BalanceCommon.BalanceCommonState.EMoney01CreditWait && (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button02, InputManager.TouchPanelArea.A2) || InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button02, InputManager.TouchPanelArea.A2, 1000L)))
			{
				if (_monitors[monitorId]._isAwake)
				{
					TicketSelectMonitor.MonitorButtonMode button_mode = _monitors[monitorId]._button_mode;
					if (button_mode == TicketSelectMonitor.MonitorButtonMode.MonitorButtonModeEMoney)
					{
						_monitors[monitorId].BalanceButtonAnim(InputManager.ButtonSetting.Button02);
					}
				}
			}
			else if (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3) || InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L))
			{
				if (_monitors[monitorId]._isAwake)
				{
					switch (_monitors[monitorId]._button_mode)
					{
					case TicketSelectMonitor.MonitorButtonMode.MonitorButtonModeCommon:
						_monitors[monitorId].RightButtonAnim(InputManager.ButtonSetting.Button03);
						break;
					case TicketSelectMonitor.MonitorButtonMode.MonitorButtonModeEMoney:
						if (!_monitors[monitorId]._balanceCommon._isRightLimits)
						{
							_monitors[monitorId].RightButtonAnim(InputManager.ButtonSetting.Button03);
						}
						break;
					}
				}
			}
			else if ((InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6) || InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L)) && _monitors[monitorId]._isAwake)
			{
				switch (_monitors[monitorId]._button_mode)
				{
				case TicketSelectMonitor.MonitorButtonMode.MonitorButtonModeCommon:
					_monitors[monitorId].LeftButtonAnim(InputManager.ButtonSetting.Button06);
					break;
				case TicketSelectMonitor.MonitorButtonMode.MonitorButtonModeEMoney:
					if (!_monitors[monitorId]._balanceCommon._isLeftLimits)
					{
						_monitors[monitorId].LeftButtonAnim(InputManager.ButtonSetting.Button06);
					}
					break;
				}
			}
			if (!_isCallCreditMain || _isBlockingDecideCredit || !_monitors[monitorId]._isAwake || _monitors[monitorId]._isPaidCreditAsTicketCost || _monitors[monitorId]._isAnotherBlocking || !_monitors[monitorId]._isServerRequest || _monitors[monitorId]._state < TicketSelectMonitor.MonitorState.DecideCreditFadeIn)
			{
				return;
			}
			if (_monitors[monitorId]._isServerAccepted)
			{
				int num3 = -1;
				int num4 = 0;
				num3 = _monitors[monitorId]._decidedSelectTicketID;
				num4 = _monitors[monitorId].GetSelectorCreditNum(num3);
				if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.IsFreePlay())
				{
					num4 = 0;
				}
				if (!_monitors[monitorId]._isCallRejectedInfo)
				{
					switch (num4)
					{
					case 1:
					{
						bool num6 = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.PayItemCost(monitorId, bookkeep: false);
						_creditCost++;
						if (num6)
						{
							SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.bookkeep.addCreditTicket((uint)num4);
						}
						_monitors[monitorId]._isBoughtTicket = true;
						break;
					}
					case 2:
					{
						bool num5 = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.PayGameCostFreedom(monitorId, bookkeep: false);
						_creditCost += 2;
						if (num5)
						{
							SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.bookkeep.addCreditTicket((uint)num4);
						}
						_monitors[monitorId]._isBoughtTicket = true;
						break;
					}
					}
				}
				_monitors[monitorId]._isPaidCreditAsTicketCost = true;
				_isBlockingDecideCredit = true;
				_blockingCount = 0;
				if (_monitors[monitorId]._isAwake && _monitors[monitorId]._isBoughtTicket && !_monitors[monitorId]._isSetAccounting)
				{
					UserData userData = Singleton<UserDataManager>.Instance.GetUserData(monitorId);
					if (!userData.IsGuest() && userData.UserType != UserData.UserIDType.New && !SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.IsFreePlay() && !GameManager.IsEventMode && !GameManager.IsFreedomMode && !GameManager.IsCourseMode)
					{
						SingletonStateMachine<AmManager, AmManager.EState>.Instance.Accounting.EndPlay(monitorId, Accounting.KindCoe.Credit, Accounting.StatusCode.PayToPlay_End, 0);
						SingletonStateMachine<AmManager, AmManager.EState>.Instance.Accounting.BeginPlay(monitorId, Accounting.KindCoe.Ticket, Accounting.StatusCode.PayToPlay_Start);
						SingletonStateMachine<AmManager, AmManager.EState>.Instance.Accounting.PutGeneralId(monitorId, Accounting.KindCoe.Common, userData.Detail.UserID.ToString());
						SingletonStateMachine<AmManager, AmManager.EState>.Instance.Accounting.EndPlay(monitorId, Accounting.KindCoe.Ticket, Accounting.StatusCode.PayToPlay_End, num4);
						SingletonStateMachine<AmManager, AmManager.EState>.Instance.Accounting.BeginPlay(monitorId, Accounting.KindCoe.Credit, Accounting.StatusCode.PayToPlay_Start);
						SingletonStateMachine<AmManager, AmManager.EState>.Instance.Accounting.PutGeneralId(monitorId, Accounting.KindCoe.Common, userData.Detail.UserID.ToString());
					}
					_monitors[monitorId]._isSetAccounting = true;
				}
				if (!_monitors[monitorId]._isCallBlur)
				{
					_monitors[monitorId]._isCallBlur = true;
				}
				if (_monitors[monitorId]._isCallLastBlur)
				{
					_monitors[monitorId]._isCallLastBlur = false;
				}
				int num7 = (monitorId + 1) % _monitors.Length;
				if (_monitors[num7]._isAwake && !_monitors[num7]._isDecidedEntry && _monitors[num7]._state >= TicketSelectMonitor.MonitorState.CreditJudge && !_monitors[num7]._isSelectButton && _monitors[num7]._button_mode != TicketSelectMonitor.MonitorButtonMode.MonitorButtonModeEMoney)
				{
					_monitors[num7]._state = TicketSelectMonitor.MonitorState.CreditJudgeReset;
				}
				if (_monitors[num7]._isAwake && !_monitors[num7]._isDecidedEntry && !_monitors[num7]._isServerRequest && _monitors[num7]._isAnotherBlocking)
				{
					_monitors[num7]._isAnotherBlocking = false;
					if (_monitors[num7]._isCallBlur)
					{
						_monitors[num7]._isCallBlur = false;
					}
					if (_monitors[num7]._isCallLastBlur)
					{
						_monitors[num7]._isCallLastBlur = false;
					}
					if (container.processManager.IsOpening(num7, WindowPositionID.Middle))
					{
						container.processManager.CloseWindow(num7);
					}
				}
				for (int k = 0; k < _monitors.Length; k++)
				{
					if (_monitors[k]._isAwake)
					{
						_monitors[k]._isBlockingDecideCredit = true;
					}
				}
				if (container.processManager.IsOpening(monitorId, WindowPositionID.Middle))
				{
					container.processManager.CloseWindow(monitorId);
				}
				return;
			}
			if (!_monitors[monitorId]._isServerAccepted)
			{
				_monitors[monitorId]._isServerWaitCount = true;
			}
			if (_monitors[monitorId]._isServerWaitCount && _monitors[monitorId]._nServerWaitTimer > 0)
			{
				_monitors[monitorId]._nServerWaitTimer--;
			}
			if (_monitors[monitorId]._nServerWaitTimer == 0)
			{
				_monitors[monitorId]._isServerWaitCount = false;
				if (_monitors[monitorId]._isCallBlur)
				{
					_monitors[monitorId]._isCallBlur = false;
				}
				if (!_monitors[monitorId]._isCallLastBlur)
				{
					_monitors[monitorId]._isCallLastBlur = true;
				}
				if (!container.processManager.IsOpening(monitorId, WindowPositionID.Middle))
				{
					container.processManager.EnqueueMessage(monitorId, WindowMessageID.TicketConnectServer, WindowPositionID.Middle);
				}
			}
			int num8 = (monitorId + 1) % _monitors.Length;
			if (!_monitors[num8]._isAwake || _monitors[num8]._isDecidedEntry || _monitors[num8]._isServerRequest || (_monitors[num8]._isAnotherBlocking && (!_monitors[num8]._isAnotherBlocking || !_monitors[num8]._isRecallAnoterBlur) && !_monitors[num8]._isServerWaitCount))
			{
				return;
			}
			_monitors[num8]._isServerWaitCount = true;
			_monitors[num8]._isAnotherBlocking = true;
			if (_monitors[num8]._isServerWaitCount && _monitors[num8]._nServerWaitTimer > 0)
			{
				_monitors[num8]._nServerWaitTimer--;
			}
			if (_monitors[num8]._nServerWaitTimer != 0)
			{
				return;
			}
			_monitors[num8]._isServerWaitCount = false;
			_monitors[num8]._isRecallAnoterBlur = false;
			if (_monitors[num8]._state >= TicketSelectMonitor.MonitorState.DecideCreditFadeIn)
			{
				if (_monitors[num8]._isCallBlur)
				{
					_monitors[num8]._isCallBlur = false;
				}
				if (!_monitors[num8]._isCallLastBlur)
				{
					_monitors[num8]._isCallLastBlur = true;
				}
				if (!container.processManager.IsOpening(num8, WindowPositionID.Middle))
				{
					container.processManager.EnqueueMessage(num8, WindowMessageID.TicketConnectServer, WindowPositionID.Middle);
				}
			}
		}

		public override void OnLateUpdate()
		{
		}

		private void TimeUp()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				_monitors[i]._isTimeUp = true;
				_monitors[i]._isDecidedOK = true;
				_monitors[i]._isDecidedEntry = true;
			}
			_isTimeUp = true;
		}
	}
}
