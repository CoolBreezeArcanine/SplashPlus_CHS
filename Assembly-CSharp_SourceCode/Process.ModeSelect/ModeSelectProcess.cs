using System.Collections.Generic;
using AMDaemon;
using Balance;
using DB;
using IO;
using MAI2.Util;
using Manager;
using Manager.MaiStudio;
using Manager.MaiStudio.CardTypeName;
using Manager.UserDatas;
using Monitor.ModeSelect;
using Net;
using Process.CodeRead;
using Process.CourseSelect;
using Process.LoginBonus;
using UnityEngine;

namespace Process.ModeSelect
{
	public class ModeSelectProcess : ProcessBase
	{
		private enum ProcessState : byte
		{
			Wait,
			Disp,
			Released
		}

		public enum ProccessActiveMode
		{
			Common,
			Individual
		}

		private ModeSelectMonitor[] _monitors;

		private ProcessState _state;

		public ProccessActiveMode _active_mode;

		public ModeSelectMonitor.MonitorState _monitor_state;

		public int _monitor_timer;

		public bool _isEnableOK;

		public bool _isCheckOK;

		public bool _isOK;

		public bool _isCheckLR;

		public bool _isRight;

		public bool _isLeft;

		public bool _isCheckTimeSkip;

		public bool _isTimeSkip;

		public bool _isEnableTimeSkip;

		public bool _isCheckSetting;

		public bool _isSetting;

		public bool _isDecidedOK;

		public bool _isTimeUp;

		public int _decidedSelectModeType = -1;

		public bool _isCallSettingBlur;

		public bool _isCallCreditMain;

		public bool _isBlockingDecideCredit;

		public int _blockingCount;

		public int _creditPrevius;

		public int _creditCurrent;

		public int _creditCost;

		public int _isEntryNum;

		public int _isDecidedEntryNum;

		public int _isCanceledEntryNum;

		public bool _isSetAddTrackInfo;

		public bool _isFinishedDispInfoWindow;

		public bool _isJoinActiveModeCommon;

		public bool _isBlockingDecideBalance;

		public int _blockingCount2;

		public bool _isDispCanceledMessage;

		public int _canceldMessagePlayerID = -1;

		public bool[] _isMajorVersionUp = new bool[2];

		public ModeSelectProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public ModeSelectProcess(ProcessDataContainer dataContainer, params object[] args)
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
			GameObject prefs = Resources.Load<GameObject>("Process/ModeSelect/ModeSelectProcess");
			_monitors = new ModeSelectMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<ModeSelectMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<ModeSelectMonitor>()
			};
			for (int i = 0; i < _monitors.Length; i++)
			{
				_monitors[i].Initialize(i, active: true);
				_monitors[i]._isMajorVersionUp = _isMajorVersionUp[i];
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
					num++;
				}
			}
			for (int k = 0; k < _monitors.Length; k++)
			{
				_monitors[k]._isEntryNum = num;
			}
			_isEntryNum = num;
			if (num == _monitors.Length)
			{
				for (int l = 0; l < _monitors.Length; l++)
				{
					_monitors[l].SetModeSelectTypeDisable(2);
				}
				if (num3 > 0)
				{
					for (int m = 0; m < _monitors.Length; m++)
					{
						_monitors[m].SetModeSelectTypeDisable(1);
					}
				}
			}
			else if (num == _monitors.Length - 1)
			{
				if (num3 > 0 || num2 > 0)
				{
					for (int n = 0; n < _monitors.Length; n++)
					{
						_monitors[n].SetModeSelectTypeDisable(2);
					}
				}
				if (num3 > 0)
				{
					for (int num4 = 0; num4 < _monitors.Length; num4++)
					{
						_monitors[num4].SetModeSelectTypeDisable(1);
					}
				}
			}
			if (GameManager.IsEventMode)
			{
				for (int num5 = 0; num5 < _monitors.Length; num5++)
				{
					_monitors[num5].SetModeSelectTypeDisable(2);
					_monitors[num5].SetModeSelectTypeDisable(1);
				}
			}
			for (int num6 = 0; num6 < _monitors.Length; num6++)
			{
				_monitors[num6].SetMiniSelector();
				_monitors[num6].SetBigSelector();
			}
			bool flag = false;
			for (int num7 = 0; num7 < _monitors.Length; num7++)
			{
				if (_monitors[num7]._isAwake)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				container.processManager.PrepareTimer(99, 0, isEntry: false, TimeUp);
				int num8 = 0;
				for (int num9 = 0; num9 < _monitors.Length; num9++)
				{
					if (_monitors[num9]._isAwake)
					{
						_monitors[num9]._isEnableTimer = true;
						_monitors[num9]._isTimerVisible = true;
						_monitors[num9]._isBothDecidedEntry = false;
						num8++;
					}
					else
					{
						container.processManager.SetVisibleTimer(num9, isVisible: false);
					}
				}
				if (num8 == 2)
				{
					for (int num10 = 0; num10 < _monitors.Length; num10++)
					{
						_monitors[num10]._isBothDecidedEntry = true;
					}
				}
			}
			_creditPrevius = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.NowCredit;
			_creditCurrent = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.NowCredit;
			_creditCost = 0;
			container.processManager.NotificationFadeIn();
		}

		public bool SyncedState(ModeSelectMonitor.MonitorState current_monitor_state, ModeSelectMonitor.MonitorState next_monitor_state)
		{
			bool result = false;
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < _monitors.Length; i++)
			{
				if (_monitors[i]._isEntry)
				{
					num++;
				}
				if (_monitors[i]._state == current_monitor_state)
				{
					num2++;
				}
			}
			if (num2 == num)
			{
				_monitor_state = next_monitor_state;
				result = true;
			}
			return result;
		}

		public void SyncedParam(bool update)
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				if (_monitors[i]._isEntry)
				{
					_monitors[i]._isEnableOK = _isEnableOK;
					_monitors[i]._isCheckOK = _isCheckOK;
					_monitors[i]._isOK = _isOK;
					_monitors[i]._isCheckLR = _isCheckLR;
					_monitors[i]._isRight = _isRight;
					_monitors[i]._isLeft = _isLeft;
					_monitors[i]._isEnableTimeSkip = _isEnableTimeSkip;
					_monitors[i]._isCheckTimeSkip = _isCheckTimeSkip;
					_monitors[i]._isTimeSkip = _isTimeSkip;
					_monitors[i]._isCheckSetting = _isCheckSetting;
					_monitors[i]._isSetting = _isSetting;
					if (update)
					{
						_monitors[i]._state = _monitor_state;
					}
				}
			}
		}

		public override void OnUpdate()
		{
			_creditPrevius = _creditCurrent;
			_creditCost = 0;
			base.OnUpdate();
			int num = 0;
			int num2 = 0;
			switch (_state)
			{
			case ProcessState.Wait:
			{
				_active_mode = ProccessActiveMode.Individual;
				_monitor_state = ModeSelectMonitor.MonitorState.None;
				_monitor_timer = 0;
				_state = ProcessState.Disp;
				for (int num13 = 0; num13 < _monitors.Length; num13++)
				{
					if (_monitors[num13]._isEntry)
					{
						UserData userData2 = Singleton<UserDataManager>.Instance.GetUserData(num13);
						UserDetail detail = userData2.Detail;
						UserOption option = userData2.Option;
						container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20010, num13, true));
						MessageUserInformationData messageUserInformationData = new MessageUserInformationData(num13, container.assetManager, detail, option.DispRate, isSubMonitor: true);
						container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20011, num13, messageUserInformationData));
						container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30008, num13, option.SubmonitorAppeal));
					}
				}
				break;
			}
			case ProcessState.Disp:
			{
				if (_monitors[0].IsEnd() && _monitors[1].IsEnd())
				{
					_state = ProcessState.Released;
					bool flag = false;
					for (int i = 0; i < 2; i++)
					{
						UserData userData = Singleton<UserDataManager>.Instance.GetUserData(i);
						if (userData.IsEntry && !userData.IsGuest() && userData.Detail.PlayCount >= 0)
						{
							flag = true;
						}
					}
					if (GameManager.IsEventMode)
					{
						flag = false;
					}
					switch (_decidedSelectModeType)
					{
					case 1:
						GameManager.IsCourseMode = true;
						break;
					case 2:
						GameManager.IsFreedomMode = true;
						break;
					}
					GameManager.SetMaxTrack();
					bool flag2 = false;
					int num3 = 0;
					num2 = 0;
					for (int j = 0; j < _monitors.Length; j++)
					{
						if (_monitors[j]._isAwake)
						{
							num3++;
							if (_monitors[j]._isCanceledEntry)
							{
								num2++;
							}
						}
					}
					if (num2 >= num3)
					{
						flag2 = true;
					}
					for (int k = 0; k < _monitors.Length; k++)
					{
						if (_monitors[k]._isEntry && _monitors[k]._isCanceledEntry)
						{
							NetPacketUtil.ForcedUserLogout(k);
							Singleton<UserDataManager>.Instance.GetUserData(k).IsEntry = false;
							Singleton<UserDataManager>.Instance.GetUserData(k).Initialize();
							Singleton<UserDataManager>.Instance.SetDefault(k);
							_monitors[k]._isEntry = false;
							container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20000));
						}
					}
					if (flag2)
					{
						container.processManager.AddProcess(new FadeProcess(container, this, new AdvertiseProcess(container)), 50);
						container.processManager.SendMessage(new Message(ProcessType.PleaseWaitProcess, 20000));
					}
					else
					{
						if (!GameManager.IsFreedomMode)
						{
							int num4 = 0;
							int num5 = 0;
							for (int l = 0; l < _monitors.Length; l++)
							{
								if (_monitors[l]._isAwake && _monitors[l]._isDecidedEntry)
								{
									int decidedSelectModeType = _monitors[l]._decidedSelectModeType;
									int num6 = _monitors[l].GetSelectorCreditNum(decidedSelectModeType, _monitors[l]._isNewAime);
									if (!_monitors[l]._isCreditResult)
									{
										num6 = 0;
									}
									num4++;
									num5 += num6;
								}
							}
							if (num4 > 0 && num5 > 0)
							{
								switch (num4)
								{
								case 1:
									SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.bookkeep.addCredit1P((uint)num5);
									break;
								case 2:
									SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.bookkeep.addCredit2P((uint)num5);
									break;
								default:
									SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.bookkeep.addCredit1P((uint)num5);
									break;
								}
							}
						}
						int monitorIndex = -1;
						for (int m = 0; m < _monitors.Length; m++)
						{
							if (_monitors[m]._isCanceledEntry)
							{
								monitorIndex = m;
							}
						}
						for (int n = 0; n < _monitors.Length; n++)
						{
							_monitors[n].SetGotoParam();
						}
						if (!GameManager.IsGotoCodeRead)
						{
							int[] array = new int[_monitors.Length];
							for (int num7 = 0; num7 < _monitors.Length; num7++)
							{
								array[num7] = 0;
							}
							for (int num8 = 0; num8 < _monitors.Length; num8++)
							{
								if (_monitors[num8]._isAwake && _monitors[num8]._isDecidedEntry)
								{
									List<int> selectedCardList = Singleton<UserDataManager>.Instance.GetUserData(num8).Extend.SelectedCardList;
									if (selectedCardList.Count > 0 && selectedCardList[0] != 0)
									{
										array[num8] = selectedCardList[0];
									}
								}
							}
							for (int num9 = 0; num9 < _monitors.Length; num9++)
							{
								if (_monitors[num9]._isAwake && _monitors[num9]._isDecidedEntry)
								{
									CodeReadProcess.CardStatus status;
									UserCard userCard = CodeReadProcess.GetUserCard(num9, array[num9], out status);
									bool flag3 = false;
									if (userCard != null)
									{
										flag3 = CodeReadProcess.IsInPeriod(userCard.startDate, userCard.endDataDate) && status == CodeReadProcess.CardStatus.Normal;
									}
									Singleton<UserDataManager>.Instance.GetUserData(num9).Detail.CardType = ((!flag3) ? 1 : array[num9]);
									if (userCard == null || status == CodeReadProcess.CardStatus.Unowned)
									{
										Singleton<UserDataManager>.Instance.GetUserData(num9).Detail.CardType = 0;
									}
									if (Singleton<UserDataManager>.Instance.GetUserData(num9).Detail.CardType > 1)
									{
										string dxCardTypeText = CodeReadProcess.GetDxCardTypeText((Table)Singleton<UserDataManager>.Instance.GetUserData(num9).Detail.CardType);
										Sprite sprite = Resources.Load<Sprite>("Process/Common/Sprites/UpperMonitor/UI_CMN_DXPass_" + dxCardTypeText);
										container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30005, num9, sprite));
									}
									CardTypeData cardType = Singleton<DataManager>.Instance.GetCardType(Singleton<UserDataManager>.Instance.GetUserData(num9).Detail.CardType);
									if (cardType != null)
									{
										GameManager.SetCardEffect(cardType.extendBitParameter, num9);
									}
								}
							}
						}
						if (GameManager.IsEventMode)
						{
							if (GameManager.IsCourseMode)
							{
								container.processManager.AddProcess(new FadeProcess(container, this, new CourseSelectProcess(container), monitorIndex), 50);
							}
							else
							{
								container.processManager.AddProcess(new FadeProcess(container, this, new MusicSelectProcess(container), monitorIndex), 50);
							}
						}
						else if (flag)
						{
							container.processManager.AddProcess(new FadeProcess(container, this, new LoginBonusProcess(container, _monitors[0]._isMajorVersionUp, _monitors[1]._isMajorVersionUp), monitorIndex), 50);
						}
						else if (GameManager.IsGotoCodeRead)
						{
							container.processManager.AddProcess(new FadeProcess(container, this, new CodeReadProcess(container), monitorIndex), 50);
						}
						else if (GameManager.IsGotoPhotoShoot)
						{
							container.processManager.AddProcess(new FadeProcess(container, this, new PhotoShootProcess(container), monitorIndex), 50);
						}
						else if (!GameManager.IsCourseMode && !GameManager.IsFreedomMode)
						{
							container.processManager.AddProcess(new FadeProcess(container, this, new RegionalSelectProcess(container), monitorIndex), 50);
						}
						else
						{
							container.processManager.AddProcess(new FadeProcess(container, this, new GetMusicProcess(container), monitorIndex), 50);
						}
					}
					if (_isCallCreditMain)
					{
						container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30001));
						_isCallCreditMain = false;
					}
					for (int num10 = 0; num10 < _monitors.Length; num10++)
					{
						if (_monitors[num10]._isAwake && _monitors[num10]._isCanceledEntry && _monitors[num10]._isEntryNum >= 2 && container.processManager.IsOpening(num10, WindowPositionID.Middle) && (_monitors[num10]._isCanceledEntryNum >= _monitors[num10]._isEntryNum || _monitors[num10]._isDecidedEntryNum >= _monitors[num10]._isEntryNum || _monitors[num10]._isDecidedEntryNum + _monitors[num10]._isCanceledEntryNum >= _monitors[num10]._isEntryNum))
						{
							container.processManager.CloseWindow(num10);
						}
					}
					break;
				}
				if (!_isCallCreditMain)
				{
					for (int num11 = 0; num11 < _monitors.Length; num11++)
					{
						if (_monitors[num11]._isAwake && _monitors[num11]._state >= ModeSelectMonitor.MonitorState.SelectCardFadeInJudge)
						{
							container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30000));
							_isCallCreditMain = true;
							break;
						}
					}
				}
				for (int num12 = 0; num12 < _monitors.Length; num12++)
				{
					if (!_monitors[num12]._isAwake || !_monitors[num12]._isCanceledEntry || _monitors[num12]._isEntryNum < 2)
					{
						continue;
					}
					if (!container.processManager.IsOpening(num12, WindowPositionID.Middle))
					{
						if (_monitors[num12]._isCanceledEntryNum < _monitors[num12]._isEntryNum && _monitors[num12]._isDecidedEntryNum < _monitors[num12]._isEntryNum && _monitors[num12]._isDecidedEntryNum + _monitors[num12]._isCanceledEntryNum < _monitors[num12]._isEntryNum)
						{
							container.processManager.EnqueueMessage(num12, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
						}
					}
					else if (container.processManager.IsOpening(num12, WindowPositionID.Middle) && (_monitors[num12]._isCanceledEntryNum >= _monitors[num12]._isEntryNum || _monitors[num12]._isDecidedEntryNum >= _monitors[num12]._isEntryNum || _monitors[num12]._isDecidedEntryNum + _monitors[num12]._isCanceledEntryNum >= _monitors[num12]._isEntryNum) && (!_isDispCanceledMessage || _canceldMessagePlayerID != num12))
					{
						container.processManager.CloseWindow(num12);
					}
				}
				break;
			}
			}
			for (int num14 = 0; num14 < _monitors.Length; num14++)
			{
				if (_monitors[num14]._isAwake && _monitors[num14]._isEnableTimer && !_monitors[num14]._isTimeUp && container.processManager.IsTimeUp(num14))
				{
					container.processManager.ForceTimeUp(num14);
					container.processManager.SetVisibleTimer(num14, isVisible: false);
					_monitors[num14]._isEnableTimer = false;
					_monitors[num14]._isTimeUp = true;
				}
			}
			if (_isCallCreditMain)
			{
				int num15 = 0;
				int num16 = 0;
				int num17 = 0;
				int num18 = 0;
				int num19 = 0;
				int num20 = 0;
				bool flag4 = false;
				int num21 = 0;
				for (int num22 = 0; num22 < _monitors.Length; num22++)
				{
					if (!_monitors[num22]._isAwake)
					{
						continue;
					}
					num15 = _monitors[num22].GetSelectorCreditNum(_monitors[num22]._decidedSelectModeType, _monitors[num22]._isNewAime);
					num16 = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.NowCredit;
					num19 = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.CoinToCredit;
					num20 = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.Remain;
					if (num19 > 1)
					{
						flag4 = true;
						num21 = num19 * num15;
						num21 -= num19 * num16;
						if (num21 < 0)
						{
							num21 = 0;
						}
						num21 -= num20;
						if (num21 < 0)
						{
							num21 = 0;
						}
						num17 = num21 / num19;
						num21 %= num19;
					}
					else if (!flag4)
					{
						num17 = num15 - num16;
						if (num17 < 0)
						{
							num17 = 0;
						}
					}
					if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.IsFreePlay())
					{
						num17 = 0;
					}
					if (flag4)
					{
						if (num17 != 0 || num21 != 0)
						{
							num18 = num17;
							_monitors[num22]._needCreditNum = num17;
							_monitors[num22]._needCoinNum = num18;
							_monitors[num22]._needCreditDenominator = num19;
							_monitors[num22]._needCreditNumerator = num21;
							_monitors[num22]._isNeedCreditFraction = true;
						}
						else
						{
							num18 = 0;
							_monitors[num22]._needCreditNum = 0;
							_monitors[num22]._needCoinNum = 0;
							_monitors[num22]._needCreditDenominator = num19;
							_monitors[num22]._needCreditNumerator = 0;
							_monitors[num22]._isNeedCreditFraction = true;
						}
					}
					else if (!flag4)
					{
						if (num17 > 0)
						{
							num18 = num17;
							_monitors[num22]._needCreditNum = num17;
							_monitors[num22]._needCoinNum = num18;
							_monitors[num22]._needCreditDenominator = 1;
							_monitors[num22]._needCreditNumerator = 0;
							_monitors[num22]._isNeedCreditFraction = false;
						}
						else
						{
							num18 = 0;
							_monitors[num22]._needCreditNum = 0;
							_monitors[num22]._needCoinNum = 0;
							_monitors[num22]._needCreditDenominator = 1;
							_monitors[num22]._needCreditNumerator = 0;
							_monitors[num22]._isNeedCreditFraction = false;
						}
					}
					_monitors[num22].SetNeedCreditNum();
				}
				if (_isBlockingDecideCredit)
				{
					_blockingCount++;
					if (_blockingCount > 5)
					{
						_isBlockingDecideCredit = false;
						for (int num23 = 0; num23 < _monitors.Length; num23++)
						{
							if (_monitors[num23]._isAwake)
							{
								_monitors[num23]._isBlockingDecideCredit = false;
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
						for (int num24 = 0; num24 < _monitors.Length; num24++)
						{
							if (_monitors[num24]._isAwake)
							{
								_monitors[num24]._isBlockingDecideBalance = false;
							}
						}
					}
				}
				int num25 = 0;
				for (int num26 = 0; num26 < _monitors.Length; num26++)
				{
					int num27 = (num26 + 1) % _monitors.Length;
					if (_monitors[num26]._isAwake && _monitors[num26]._isResetAnotherBalanceBlocking)
					{
						if (_monitors[num27]._isAwake && _monitors[num27]._isOwnBalanceBlocking)
						{
							_monitors[num27]._isOwnBalanceBlocking = false;
						}
						_monitors[num26]._isResetAnotherBalanceBlocking = false;
						if (num25 == 0)
						{
							Singleton<OperationManager>.Instance.SetCoinBlockerMode(CoinBlocker.Mode.Game);
							num25++;
						}
					}
				}
				if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsEnableEmoneyExecute)
				{
					for (int num28 = 0; num28 < _monitors.Length; num28++)
					{
						if (!_monitors[num28]._isAwake)
						{
							continue;
						}
						if ((AMDaemon.EMoney.IsAuthCompleted && (!AMDaemon.EMoney.IsServiceAlive || !AMDaemon.EMoney.Operation.CanOperateDeal)) || SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsEmoneyUnconfirm)
						{
							if (!_monitors[num28]._isBalanceNetworkDisconnection)
							{
								_monitors[num28]._isBalanceNetworkDisconnection = true;
							}
						}
						else if (((AMDaemon.EMoney.IsAuthCompleted && AMDaemon.EMoney.IsServiceAlive && AMDaemon.EMoney.Operation.CanOperateDeal) || !SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsEmoneyUnconfirm) && _monitors[num28]._isBalanceNetworkDisconnection)
						{
							_monitors[num28]._isBalanceNetworkDisconnection = false;
						}
					}
				}
			}
			switch (_active_mode)
			{
			case ProccessActiveMode.Common:
			{
				CommonViewUpdate();
				SyncedParam(update: false);
				ModeSelectMonitor.MonitorState monitorState = ModeSelectMonitor.MonitorState.None;
				ModeSelectMonitor.MonitorState monitorState2 = ModeSelectMonitor.MonitorState.None;
				switch (_monitor_state)
				{
				case ModeSelectMonitor.MonitorState.SelectCardMoveRight:
				case ModeSelectMonitor.MonitorState.SelectCardMoveLeft:
					monitorState = ModeSelectMonitor.MonitorState.SelectCardWaitSyncBoth;
					monitorState2 = monitorState;
					SyncedState(monitorState, monitorState2);
					break;
				case ModeSelectMonitor.MonitorState.SelectCardWaitSyncBoth:
					monitorState = ModeSelectMonitor.MonitorState.SelectCardWaitSyncBoth;
					monitorState2 = ModeSelectMonitor.MonitorState.SelectCardWait;
					if (SyncedState(monitorState, monitorState2))
					{
						_isRight = false;
						_isLeft = false;
						SyncedParam(update: true);
					}
					break;
				case ModeSelectMonitor.MonitorState.SettingOpen_SyncAnimWait:
					monitorState = ModeSelectMonitor.MonitorState.SettingOpen_SyncBothReached;
					monitorState2 = monitorState;
					SyncedState(monitorState, monitorState2);
					_isCheckSetting = false;
					break;
				case ModeSelectMonitor.MonitorState.SettingOpen_SyncBothReached:
					monitorState = ModeSelectMonitor.MonitorState.SettingOpen_SyncBothReached;
					monitorState2 = ModeSelectMonitor.MonitorState.SettingWait;
					if (SyncedState(monitorState, monitorState2))
					{
						_isRight = false;
						_isLeft = false;
						_isCheckOK = false;
						_isCheckLR = false;
						_isCheckSetting = true;
						SyncedParam(update: true);
						_isCheckSetting = true;
					}
					break;
				case ModeSelectMonitor.MonitorState.SettingClose_SyncAnimWait:
					monitorState = ModeSelectMonitor.MonitorState.SettingClose_SyncBothReached;
					monitorState2 = monitorState;
					SyncedState(monitorState, monitorState2);
					_isCheckSetting = false;
					break;
				case ModeSelectMonitor.MonitorState.SettingClose_SyncBothReached:
					monitorState = ModeSelectMonitor.MonitorState.SettingClose_SyncBothReached;
					monitorState2 = ModeSelectMonitor.MonitorState.SelectCardWait;
					if (SyncedState(monitorState, monitorState2))
					{
						_isRight = false;
						_isLeft = false;
						_isCheckOK = true;
						_isCheckLR = true;
						_isCheckSetting = true;
						OKStart();
						SyncedParam(update: true);
						_isCheckSetting = true;
					}
					break;
				}
				for (int num46 = 0; num46 < _monitors.Length; num46++)
				{
					if (_monitors[num46]._isEntry)
					{
						ModeSelectMonitor.MonitorState state = _monitors[num46]._state;
						if (state != ModeSelectMonitor.MonitorState.SelectCardWaitSyncBoth && state != ModeSelectMonitor.MonitorState.SettingOpen_SyncBothReached && state != ModeSelectMonitor.MonitorState.SettingClose_SyncBothReached)
						{
							_monitors[num46]._state = _monitor_state;
						}
					}
					if (_monitors[num46]._isEnableTimer && (_monitors[num46]._isDecidedEntry || _monitors[num46]._isCanceledEntry) && _monitors[num46]._isTimerVisible)
					{
						container.processManager.IsTimeCounting(num46, isTimeCount: false);
						container.processManager.SetVisibleTimer(num46, isVisible: false);
						_monitors[num46]._isTimerVisible = false;
					}
				}
				break;
			}
			default:
			{
				int num29 = 0;
				if (!_isFinishedDispInfoWindow)
				{
					for (int num30 = 0; num30 < _monitors.Length; num30++)
					{
						if (_monitors[num30]._isAwake && _monitors[num30]._state >= ModeSelectMonitor.MonitorState.SelectCardFadeInSyncWait)
						{
							num29++;
						}
					}
					if (num29 >= _isEntryNum)
					{
						for (int num31 = 0; num31 < _monitors.Length; num31++)
						{
							if (_monitors[num31]._isAwake)
							{
								container.processManager.ForcedCloseWindow(num31);
								_monitors[num31]._isCallBlur = false;
							}
						}
						_isFinishedDispInfoWindow = true;
					}
					else
					{
						for (int num32 = 0; num32 < _monitors.Length; num32++)
						{
							if (_monitors[num32]._isAwake && _monitors[num32]._state >= ModeSelectMonitor.MonitorState.SelectCardFadeInSyncWait)
							{
								if (!container.processManager.IsOpening(num32, WindowPositionID.Middle))
								{
									container.processManager.EnqueueMessage(num32, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
								}
								_monitors[num32]._isCallBlur = true;
							}
						}
					}
				}
				bool flag5 = false;
				bool flag6 = false;
				int num33 = 0;
				int num34 = 0;
				int num35 = 0;
				if (!_isSetAddTrackInfo)
				{
					for (int num36 = 0; num36 < _monitors.Length; num36++)
					{
						if (_monitors[num36]._isAwake && (_monitors[num36]._isDecidedEntry || _monitors[num36]._isCanceledEntry) && _monitors[num36]._state >= ModeSelectMonitor.MonitorState.EntryTimeOutJudge)
						{
							num33++;
						}
					}
					if (num33 >= _isEntryNum)
					{
						switch (_isEntryNum)
						{
						case 1:
							if (_isCanceledEntryNum >= _isEntryNum)
							{
								flag5 = false;
							}
							else if (_isDecidedEntryNum >= _isEntryNum)
							{
								flag5 = true;
							}
							break;
						case 2:
							if (_isCanceledEntryNum >= _isEntryNum)
							{
								flag5 = false;
							}
							else if (_isDecidedEntryNum >= _isEntryNum)
							{
								flag5 = true;
							}
							else if (_isDecidedEntryNum + _isCanceledEntryNum >= _isEntryNum)
							{
								flag5 = true;
							}
							break;
						}
					}
					if (flag5)
					{
						for (int num37 = 0; num37 < _monitors.Length; num37++)
						{
							if (_monitors[num37]._isAwake && (_monitors[num37]._isDecidedEntry || _monitors[num37]._isCanceledEntry) && _monitors[num37]._state >= ModeSelectMonitor.MonitorState.AddTrackInfoJudgeWait)
							{
								num34++;
								if (_monitors[num37]._isDecidedEntry)
								{
									num35++;
								}
							}
						}
					}
					if (num34 >= _isEntryNum)
					{
						flag6 = true;
					}
				}
				for (int num38 = 0; num38 < _monitors.Length; num38++)
				{
					if (_monitors[num38]._isDispInfoWindow)
					{
						if (_monitors[num38]._info_timer != 0)
						{
							_monitors[num38]._info_timer--;
						}
						WindowMessageID windowMessageID = WindowMessageID.ModeSelectFirst;
						uint info_timer = 180u;
						uint info_timer2 = 420u;
						if (_monitors[num38]._isDispInfoWindow)
						{
							if (_monitors[num38]._windowMessageList.Count > 0 && _monitors[num38]._info_count < _monitors[num38]._windowMessageList.Count)
							{
								windowMessageID = _monitors[num38]._windowMessageList[(int)_monitors[num38]._info_count];
							}
							info_timer = 120u;
							info_timer2 = 420u;
						}
						switch (_monitors[num38]._info_state)
						{
						case ModeSelectMonitor.InfoWindowState.None:
							_monitors[num38]._isCallBlur = true;
							_monitors[num38].ResetOKStart();
							if (!container.processManager.IsOpening(num38, WindowPositionID.Middle))
							{
								container.processManager.EnqueueMessage(num38, windowMessageID, WindowPositionID.Middle);
								_monitors[num38]._info_state = ModeSelectMonitor.InfoWindowState.Open;
							}
							break;
						case ModeSelectMonitor.InfoWindowState.Judge:
							_monitors[num38].ResetOKStart();
							if (!container.processManager.IsOpening(num38, WindowPositionID.Middle))
							{
								container.processManager.EnqueueMessage(num38, windowMessageID, WindowPositionID.Middle);
								_monitors[num38]._info_state = ModeSelectMonitor.InfoWindowState.Open;
							}
							break;
						case ModeSelectMonitor.InfoWindowState.Open:
							_monitors[num38]._info_state = ModeSelectMonitor.InfoWindowState.OpenWait;
							_monitors[num38]._info_timer = info_timer;
							if (!_monitors[num38]._isInfoWindowVoice)
							{
								_monitors[num38]._isInfoWindowVoice = true;
							}
							break;
						case ModeSelectMonitor.InfoWindowState.OpenWait:
							if (_monitors[num38]._info_timer == 0)
							{
								_monitors[num38].OKStart();
								_monitors[num38]._info_state = ModeSelectMonitor.InfoWindowState.Wait;
								_monitors[num38]._info_timer = info_timer2;
							}
							break;
						case ModeSelectMonitor.InfoWindowState.Wait:
							if (_monitors[num38]._isOK)
							{
								_monitors[num38]._info_timer = 0u;
							}
							if (_monitors[num38]._info_timer == 0)
							{
								_monitors[num38].ResetOKStart();
								_monitors[num38]._info_state = ModeSelectMonitor.InfoWindowState.Close;
							}
							break;
						case ModeSelectMonitor.InfoWindowState.Close:
							container.processManager.CloseWindow(num38);
							if (_monitors[num38]._windowMessageList.Count <= 0 || _monitors[num38]._info_count >= _monitors[num38]._windowMessageList.Count - 1)
							{
								if (_monitors[num38]._isEntry && !_monitors[num38]._isGuest && _monitors[num38]._isDispInfoWindow)
								{
									if (_monitors[num38]._windowMessageList.Count > 0 && _monitors[num38]._info_count < _monitors[num38]._windowMessageList.Count)
									{
										windowMessageID = _monitors[num38]._windowMessageList[(int)_monitors[num38]._info_count];
									}
									if (windowMessageID != WindowMessageID.ModeSelectSinRankAdd && windowMessageID == WindowMessageID.ModeSelectFirst)
									{
										Singleton<UserDataManager>.Instance.GetUserData(num38).Detail.ContentBit.SetFlag(ContentBitID.FirstModeSelect, flag: true);
									}
								}
								_monitors[num38]._info_state = ModeSelectMonitor.InfoWindowState.CloseWait;
							}
							else
							{
								_monitors[num38]._info_count++;
								_monitors[num38]._info_state = ModeSelectMonitor.InfoWindowState.Judge;
							}
							break;
						case ModeSelectMonitor.InfoWindowState.CloseWait:
							_monitors[num38]._info_state = ModeSelectMonitor.InfoWindowState.End;
							if (_monitors[num38]._isDispInfoWindow)
							{
								_monitors[num38]._isDispInfoWindow = false;
							}
							break;
						}
					}
					_monitors[num38].ViewUpdate();
					if (_monitors[num38]._isEnableTimer && (_monitors[num38]._isDecidedEntry || _monitors[num38]._isCanceledEntry) && _monitors[num38]._isTimerVisible)
					{
						container.processManager.IsTimeCounting(num38, isTimeCount: false);
						container.processManager.SetVisibleTimer(num38, isVisible: false);
						_monitors[num38]._isTimerVisible = false;
					}
				}
				if (_isFinishedDispInfoWindow && !_isJoinActiveModeCommon)
				{
					for (int num39 = 0; num39 < _monitors.Length; num39++)
					{
						if (_monitors[num39]._isAwake)
						{
							_monitors[num39]._state = ModeSelectMonitor.MonitorState.SelectCardWaitInit;
							_monitors[num39]._active_mode = ProccessActiveMode.Common;
						}
					}
					_active_mode = ProccessActiveMode.Common;
					_monitor_state = ModeSelectMonitor.MonitorState.SelectCardWaitInit;
					_isJoinActiveModeCommon = true;
				}
				if (_isSetAddTrackInfo || !flag6)
				{
					break;
				}
				int num40 = -1;
				_isSetAddTrackInfo = true;
				switch (num35)
				{
				case 1:
				{
					for (int num42 = 0; num42 < _monitors.Length; num42++)
					{
						if (_monitors[num42]._isAwake && _monitors[num42]._isDecidedEntry)
						{
							num40 = num42;
							break;
						}
					}
					if (num40 >= 0)
					{
						int num43 = num40;
						switch (_decidedSelectModeType)
						{
						case 0:
							_monitors[num43]._state = ModeSelectMonitor.MonitorState.AddTrackInfoFadeIn;
							_monitors[num43]._addTrackType = 2;
							GameManager.IsMaxTrack = false;
							if (GameManager.IsEventMode)
							{
								_monitors[num43]._state = ModeSelectMonitor.MonitorState.AddTrackInfoFadeIn;
								_monitors[num43]._addTrackType = 1;
								GameManager.IsMaxTrack = false;
							}
							else if (_monitors[num43]._isWeekDayBonus)
							{
								_monitors[num43]._state = ModeSelectMonitor.MonitorState.AddTrackInfoFadeIn;
								_monitors[num43]._addTrackType = 4;
								GameManager.IsMaxTrack = true;
							}
							else if (_monitors[num43]._isDailyBonus)
							{
								_monitors[num43]._state = ModeSelectMonitor.MonitorState.AddTrackInfoFadeIn;
								_monitors[num43]._addTrackType = 3;
								GameManager.IsMaxTrack = true;
							}
							break;
						default:
							_monitors[num43]._state = ModeSelectMonitor.MonitorState.AddTrackInfoFadeIn;
							_monitors[num43]._addTrackType = 0;
							break;
						}
						switch (_monitors[num43]._addTrackType)
						{
						case 3:
							container.processManager.EnqueueMessage(num43, WindowMessageID.EntryDoneEntryDailyBonus, WindowPositionID.Middle);
							break;
						case 4:
							container.processManager.EnqueueMessage(num43, WindowMessageID.EntryDoneEntryWeekdayBonus, WindowPositionID.Middle);
							break;
						}
					}
					int num44 = -1;
					if (num40 >= 0)
					{
						num44 = num40 + 1;
						num44 %= 2;
					}
					int isEntryNum = _isEntryNum;
					if (isEntryNum == 2 && num44 >= 0)
					{
						int monitorId = num44;
						container.processManager.EnqueueMessage(monitorId, WindowMessageID.ModeSelectWhichOneTimeOut, WindowPositionID.Middle);
						_isDispCanceledMessage = true;
						_canceldMessagePlayerID = num44;
					}
					break;
				}
				case 2:
				{
					for (int num41 = 0; num41 < _monitors.Length; num41++)
					{
						if (!_monitors[num41]._isAwake || !_monitors[num41]._isDecidedEntry)
						{
							continue;
						}
						switch (_decidedSelectModeType)
						{
						case 0:
						{
							_monitors[num41]._state = ModeSelectMonitor.MonitorState.AddTrackInfoFadeIn;
							_monitors[num41]._addTrackType = 6;
							GameManager.IsMaxTrack = true;
							if (GameManager.IsEventMode)
							{
								_monitors[num41]._state = ModeSelectMonitor.MonitorState.AddTrackInfoFadeIn;
								_monitors[num41]._addTrackType = 5;
								GameManager.IsMaxTrack = false;
								break;
							}
							UserData.UserIDType userType = Singleton<UserDataManager>.Instance.GetUserData(num41).UserType;
							if (userType == UserData.UserIDType.New)
							{
								_monitors[num41]._state = ModeSelectMonitor.MonitorState.AddTrackInfoFadeIn;
								_monitors[num41]._addTrackType = 7;
								GameManager.IsMaxTrack = true;
							}
							break;
						}
						default:
							_monitors[num41]._state = ModeSelectMonitor.MonitorState.AddTrackInfoFadeIn;
							_monitors[num41]._addTrackType = 0;
							break;
						}
						switch (_monitors[num41]._addTrackType)
						{
						case 6:
							container.processManager.EnqueueMessage(num41, WindowMessageID.EntryDoneEntryTwoPlayer, WindowPositionID.Middle);
							break;
						case 7:
							container.processManager.EnqueueMessage(num41, WindowMessageID.EntryDoneEntryTwoPlayerNew, WindowPositionID.Middle);
							break;
						}
					}
					break;
				}
				}
				for (int num45 = 0; num45 < _monitors.Length; num45++)
				{
					bool flag7 = false;
					if (_monitors[num45]._isAwake && _monitors[num45]._isDecidedEntry)
					{
						switch (_monitors[num45]._addTrackType)
						{
						case 3:
							flag7 = true;
							break;
						case 4:
							flag7 = true;
							break;
						case 6:
							flag7 = true;
							break;
						case 7:
							flag7 = true;
							break;
						}
					}
					if (flag7)
					{
						_monitors[num45]._isCallBlur = false;
						_monitors[num45].SetLastBlur();
						_monitors[num45]._isCallLastBlur = true;
					}
				}
				break;
			}
			}
			int num47 = 0;
			int num48 = 5;
			if (_isSetting)
			{
				if (!_isCallSettingBlur)
				{
					for (int num49 = 0; num49 < _monitors.Length; num49++)
					{
						GameObject settingBlurObject = _monitors[num49].GetSettingBlurObject();
						num47 = settingBlurObject.transform.GetSiblingIndex();
						settingBlurObject.transform.SetSiblingIndex(num47 - num48);
						_monitors[num49]._isChangeSettingSibling = true;
						_monitors[num49].SetSettingBlur();
					}
					_isCallSettingBlur = true;
				}
			}
			else if (_isCallSettingBlur)
			{
				for (int num50 = 0; num50 < _monitors.Length; num50++)
				{
					GameObject settingBlurObject2 = _monitors[num50].GetSettingBlurObject();
					num47 = settingBlurObject2.transform.GetSiblingIndex();
					settingBlurObject2.transform.SetSiblingIndex(num47 + num48);
					_monitors[num50]._isChangeSettingSibling = false;
					_monitors[num50].ResetSettingBlur();
				}
				_isCallSettingBlur = false;
			}
			int num51 = 0;
			int num52 = 4;
			for (int num53 = 0; num53 < _monitors.Length; num53++)
			{
				if (_monitors[num53]._isCallBlur)
				{
					if (!_monitors[num53]._isChangeSibling)
					{
						GameObject blurObject = _monitors[num53].GetBlurObject();
						num51 = blurObject.transform.GetSiblingIndex();
						blurObject.transform.SetSiblingIndex(num51 - num52);
						_monitors[num53]._isChangeSibling = true;
						_monitors[num53].SetBlur();
					}
				}
				else if (_monitors[num53]._isChangeSibling)
				{
					GameObject blurObject2 = _monitors[num53].GetBlurObject();
					num51 = blurObject2.transform.GetSiblingIndex();
					blurObject2.transform.SetSiblingIndex(num51 + num52);
					_monitors[num53]._isChangeSibling = false;
					_monitors[num53].ResetBlur();
				}
			}
			for (int num54 = 0; num54 < _monitors.Length; num54++)
			{
				if (!_monitors[num54]._isBothDecidedEntry || _monitors[num54]._isDecidedEntry)
				{
					continue;
				}
				for (int num55 = 0; num55 < _monitors.Length; num55++)
				{
					if (num54 != num55 && !_monitors[num55]._isBothDecidedEntry && _monitors[num55]._isDecidedEntry)
					{
						_monitors[num54]._isDecidedEntry = true;
						break;
					}
				}
			}
			for (int num56 = 0; num56 < _monitors.Length; num56++)
			{
				if (_monitors[num56]._isEnableTimer && _monitors[num56]._isDecidedEntry && !_monitors[num56]._isTimeUp)
				{
					container.processManager.ForceTimeUp(num56);
					_monitors[num56]._isTimeUp = true;
				}
			}
			num = 0;
			num2 = 0;
			for (int num57 = 0; num57 < _monitors.Length; num57++)
			{
				if (_monitors[num57]._isAwake)
				{
					if (_monitors[num57]._isDecidedEntry)
					{
						num++;
					}
					if (_monitors[num57]._isCanceledEntry)
					{
						num2++;
					}
					if (_monitors[num57]._isDecidedEntry && _decidedSelectModeType == -1)
					{
						_decidedSelectModeType = _monitors[num57]._decidedSelectModeType;
					}
				}
			}
			for (int num58 = 0; num58 < _monitors.Length; num58++)
			{
				_monitors[num58]._isDecidedEntryNum = num;
				_monitors[num58]._isCanceledEntryNum = num2;
			}
			_isDecidedEntryNum = num;
			_isCanceledEntryNum = num2;
			_creditCurrent = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.NowCredit;
			if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.IsAddCoinNow())
			{
				int num59 = _creditPrevius - _creditCost;
				if (_creditCurrent - num59 > 0)
				{
					container.processManager.PrepareTimer(99, 0, isEntry: false, TimeUp);
					for (int num60 = 0; num60 < _monitors.Length; num60++)
					{
						if (_monitors[num60]._isAwake)
						{
							if (_monitors[num60]._isDecidedEntry || _monitors[num60]._isCanceledEntry || _monitors[num60]._isTimeUp)
							{
								container.processManager.ForceTimeUp(num60);
								container.processManager.SetVisibleTimer(num60, isVisible: false);
								_monitors[num60]._isEnableTimer = false;
							}
						}
						else
						{
							container.processManager.SetVisibleTimer(num60, isVisible: false);
						}
					}
				}
			}
			for (int num61 = 0; num61 < _monitors.Length; num61++)
			{
				if (_monitors[num61]._isAwake)
				{
					if (_creditCurrent > 0)
					{
						_monitors[num61]._isRemainCredit = true;
					}
					else
					{
						_monitors[num61]._isRemainCredit = false;
					}
				}
			}
		}

		public void OKButtonAnim(InputManager.ButtonSetting button)
		{
			if (!_isCheckOK || _isOK)
			{
				return;
			}
			bool isOK = false;
			for (int i = 0; i < _monitors.Length; i++)
			{
				if (_monitors[i]._isAwake && !_monitors[i]._isDecidedEntry && !_monitors[i]._isCanceledEntry && _monitors[i].IsEnableModeSelectType() && !_monitors[i]._isRight && !_monitors[i]._isLeft)
				{
					_monitors[i].OKButtonAnim(button);
					isOK = true;
				}
			}
			_isOK = isOK;
		}

		public void TimeSkipButtonAnim(InputManager.ButtonSetting button)
		{
			if (!_isCheckTimeSkip || _isTimeSkip)
			{
				return;
			}
			for (int i = 0; i < _monitors.Length; i++)
			{
				if (_monitors[i]._isEntry)
				{
					_monitors[i].TimeSkipButtonAnim(button);
				}
			}
		}

		public void RightButtonAnim(InputManager.ButtonSetting button)
		{
			if (!_isCheckLR || _isRight || _isLeft)
			{
				return;
			}
			for (int i = 0; i < _monitors.Length; i++)
			{
				if (_monitors[i]._isEntry)
				{
					_monitors[i].RightButtonAnim(button);
				}
			}
			_isRight = true;
		}

		public void LeftButtonAnim(InputManager.ButtonSetting button)
		{
			if (!_isCheckLR || _isRight || _isLeft)
			{
				return;
			}
			for (int i = 0; i < _monitors.Length; i++)
			{
				if (_monitors[i]._isEntry)
				{
					_monitors[i].LeftButtonAnim(button);
				}
			}
			_isLeft = true;
		}

		public void SettingButtonAnim(InputManager.ButtonSetting button)
		{
			if (!_isCheckSetting)
			{
				return;
			}
			if (!_isSetting)
			{
				for (int i = 0; i < _monitors.Length; i++)
				{
					if (_monitors[i]._isEntry)
					{
						_monitors[i].SettingButtonAnim(button);
					}
				}
				_isSetting = true;
				return;
			}
			for (int j = 0; j < _monitors.Length; j++)
			{
				if (_monitors[j]._isEntry)
				{
					_monitors[j].SettingButtonAnim(button);
				}
			}
			_isSetting = false;
		}

		public void isOKTimerZero()
		{
			if (_isOK)
			{
				_monitor_timer = 0;
			}
		}

		public void OKStart()
		{
			if (!_isEnableOK)
			{
				_isEnableOK = true;
				_isCheckOK = true;
			}
		}

		public void ResetOKStart()
		{
			_ = _isEnableOK;
			_isEnableOK = false;
			_isCheckOK = false;
			_isOK = false;
		}

		public void TimeSkipStart()
		{
			if (!_isEnableTimeSkip)
			{
				_isEnableTimeSkip = true;
				_isCheckTimeSkip = true;
			}
		}

		public void ResetTimeSkipStart()
		{
			_ = _isEnableTimeSkip;
			_isEnableTimeSkip = false;
			_isCheckTimeSkip = false;
			_isTimeSkip = false;
		}

		public void CommonViewUpdate()
		{
			if (_monitor_timer > 0)
			{
				_monitor_timer--;
			}
			for (int i = 0; i < _monitors.Length; i++)
			{
				if (_monitors[i]._isEntry)
				{
					_monitors[i]._timer = _monitor_timer;
				}
			}
			for (int j = 0; j < _monitors.Length; j++)
			{
				if (_monitors[j]._isEntry)
				{
					_monitors[j].ViewUpdate();
				}
			}
			switch (_monitor_state)
			{
			case ModeSelectMonitor.MonitorState.Init:
				_isCheckOK = false;
				_isCheckLR = false;
				_isCheckTimeSkip = false;
				_isCheckSetting = false;
				break;
			case ModeSelectMonitor.MonitorState.SelectCardWaitInit:
				if (_monitor_timer == 0)
				{
					_isCheckLR = true;
					_isCheckTimeSkip = true;
					_isCheckSetting = true;
					OKStart();
					TimeSkipStart();
					_monitor_state = ModeSelectMonitor.MonitorState.SelectCardWait;
				}
				break;
			case ModeSelectMonitor.MonitorState.SelectCardWait:
				if (_isOK || _isTimeUp)
				{
					_monitor_state = ModeSelectMonitor.MonitorState.CreditJudgeInit;
					if (_isOK)
					{
						_isDecidedOK = true;
					}
					else if (_isTimeUp)
					{
						_isDecidedOK = false;
						_decidedSelectModeType = -1;
						_monitor_state = ModeSelectMonitor.MonitorState.EntryResetInit;
					}
					_isCheckLR = false;
					_isRight = false;
					_isLeft = false;
					_isCheckOK = false;
					_isTimeSkip = false;
					_isCheckSetting = false;
					_monitor_timer = 60;
					ResetOKStart();
					ResetTimeSkipStart();
				}
				else if (_isSetting)
				{
					_isCheckLR = false;
					_isRight = false;
					_isLeft = false;
					_isCheckOK = false;
					ResetOKStart();
					_monitor_state = ModeSelectMonitor.MonitorState.SettingOpen_SyncAnimWait;
					_isCheckSetting = false;
				}
				else
				{
					if (_isRight)
					{
						_monitor_state = ModeSelectMonitor.MonitorState.SelectCardMoveRight;
					}
					else if (_isLeft)
					{
						_monitor_state = ModeSelectMonitor.MonitorState.SelectCardMoveLeft;
					}
					if (!_isRight)
					{
						_ = _isLeft;
					}
				}
				break;
			case ModeSelectMonitor.MonitorState.SelectCardMoveRight:
			case ModeSelectMonitor.MonitorState.SelectCardMoveLeft:
			{
				ProccessActiveMode active_mode = _active_mode;
				if (active_mode != 0 && active_mode == ProccessActiveMode.Individual)
				{
					_monitor_state = ModeSelectMonitor.MonitorState.SelectCardWait;
				}
				break;
			}
			case ModeSelectMonitor.MonitorState.SettingOpen_SyncAnimWait:
			{
				ProccessActiveMode active_mode = _active_mode;
				if (active_mode != 0 && active_mode == ProccessActiveMode.Individual)
				{
					_monitor_state = ModeSelectMonitor.MonitorState.SettingWait;
				}
				break;
			}
			case ModeSelectMonitor.MonitorState.SettingWait:
				if (_isTimeUp || (!_isSetting && !_monitors[0].IsToggleAnim() && !_monitors[1].IsToggleAnim()))
				{
					_monitor_state = ModeSelectMonitor.MonitorState.SettingClose_SyncAnimWait;
					_isCheckSetting = false;
				}
				break;
			case ModeSelectMonitor.MonitorState.SettingClose_SyncAnimWait:
			{
				ProccessActiveMode active_mode = _active_mode;
				if (active_mode != 0 && active_mode == ProccessActiveMode.Individual)
				{
					_monitor_state = ModeSelectMonitor.MonitorState.SelectCardWait;
				}
				_isCheckOK = false;
				_isCheckLR = false;
				_isCheckSetting = false;
				break;
			}
			case ModeSelectMonitor.MonitorState.CreditJudgeInit:
				if (_monitor_timer == 0)
				{
					_isOK = false;
					_monitor_state = ModeSelectMonitor.MonitorState.CreditJudge;
					for (int l = 0; l < _monitors.Length; l++)
					{
						_monitors[l]._active_mode = ProccessActiveMode.Individual;
					}
					_active_mode = ProccessActiveMode.Individual;
				}
				break;
			case ModeSelectMonitor.MonitorState.EntryResetInit:
			{
				_monitor_state = ModeSelectMonitor.MonitorState.EntryReset;
				for (int k = 0; k < _monitors.Length; k++)
				{
					_monitors[k]._active_mode = ProccessActiveMode.Individual;
				}
				_active_mode = ProccessActiveMode.Individual;
				break;
			}
			}
		}

		protected override void UpdateInput(int monitorId)
		{
			if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button04))
			{
				switch (_active_mode)
				{
				case ProccessActiveMode.Common:
					if (_monitors[monitorId]._isAwake && !_monitors[monitorId]._isDecidedEntry && !_monitors[monitorId]._isCanceledEntry)
					{
						OKButtonAnim(InputManager.ButtonSetting.Button04);
					}
					break;
				default:
					if (_isCallCreditMain)
					{
						if (_isBlockingDecideCredit)
						{
							break;
						}
						_monitors[monitorId].OKButtonAnim(InputManager.ButtonSetting.Button04);
						if (!_monitors[monitorId]._isOK || _monitors[monitorId]._state < ModeSelectMonitor.MonitorState.DecideCreditFadeIn)
						{
							break;
						}
						int num = -1;
						int num2 = 0;
						num = _monitors[monitorId]._decidedSelectModeType;
						num2 = _monitors[monitorId].GetSelectorCreditNum(num, _monitors[monitorId]._isNewAime);
						switch (num2)
						{
						case 1:
							_monitors[monitorId]._isCreditResult = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.PayItemCost(monitorId);
							_creditCost++;
							break;
						case 2:
							SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.PayGameCostFreedom(monitorId);
							_creditCost += 2;
							break;
						}
						_isBlockingDecideCredit = true;
						_blockingCount = 0;
						int num3 = (monitorId + 1) % _monitors.Length;
						if (_monitors[num3]._isAwake && !_monitors[num3]._isDecidedEntry && !_monitors[num3]._isCanceledEntry && _monitors[num3]._state >= ModeSelectMonitor.MonitorState.CreditJudge && !_monitors[num3]._isSelectButton && _monitors[num3]._button_mode != ModeSelectMonitor.MonitorButtonMode.MonitorButtonModeEMoney)
						{
							_monitors[num3]._state = ModeSelectMonitor.MonitorState.CreditJudgeReset;
						}
						for (int i = 0; i < _monitors.Length; i++)
						{
							if (_monitors[i]._isAwake)
							{
								_monitors[i]._isBlockingDecideCredit = true;
							}
						}
						if (!_monitors[monitorId]._isAwake || _monitors[monitorId]._isSetBookkeepStartTime)
						{
							break;
						}
						UserData userData = Singleton<UserDataManager>.Instance.GetUserData(monitorId);
						if (!_monitors[monitorId]._isNewAime)
						{
							if (_monitors[monitorId]._decidedSelectModeType != 2)
							{
								SingletonStateMachine<AmManager, AmManager.EState>.Instance.Accounting.BeginPlay(monitorId, Accounting.KindCoe.Credit, Accounting.StatusCode.PayToPlay_Start);
								if (!userData.IsGuest())
								{
									SingletonStateMachine<AmManager, AmManager.EState>.Instance.Accounting.PutGeneralId(monitorId, Accounting.KindCoe.Common, userData.Detail.UserID.ToString());
								}
							}
							else
							{
								SingletonStateMachine<AmManager, AmManager.EState>.Instance.Accounting.BeginPlay(monitorId, Accounting.KindCoe.Freedom, Accounting.StatusCode.PayToPlay_Start);
								SingletonStateMachine<AmManager, AmManager.EState>.Instance.Accounting.PutGeneralId(monitorId, Accounting.KindCoe.Common, userData.Detail.UserID.ToString());
							}
							if (!userData.IsGuest())
							{
								if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.IsFreePlay())
								{
									SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.SendAimeLog(userData.AimeId, AimeLogStatus.Enter);
								}
								else
								{
									SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.SendAimeLog(userData.AimeId, AimeLogStatus.Enter, num2);
								}
							}
						}
						else
						{
							SingletonStateMachine<AmManager, AmManager.EState>.Instance.Accounting.BeginPlay(monitorId, Accounting.KindCoe.Credit, Accounting.StatusCode.FreeToPlay_Start);
							SingletonStateMachine<AmManager, AmManager.EState>.Instance.Accounting.PutGeneralId(monitorId, Accounting.KindCoe.Common, userData.Detail.UserID.ToString());
							SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.SendAimeLog(userData.AimeId, AimeLogStatus.Enter);
						}
						Manager.Backup backup = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup;
						BackupBookkeep.EntryState entry = new BackupBookkeep.EntryState
						{
							entry = true
						};
						if (UserID.IsGuest(userData.Detail.UserID))
						{
							entry.type = BackupBookkeep.LoginType.Guest;
						}
						else
						{
							entry.type = BackupBookkeep.LoginType.Aime;
						}
						backup.bookkeep.startPlayTime(entry, monitorId);
						_monitors[monitorId]._isSetBookkeepStartTime = true;
					}
					else
					{
						_monitors[monitorId].OKButtonAnim(InputManager.ButtonSetting.Button04);
					}
					break;
				}
			}
			else if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button05))
			{
				switch (_active_mode)
				{
				case ProccessActiveMode.Common:
				{
					if (!_monitors[monitorId]._isAwake || _monitors[monitorId]._isDecidedEntry || _monitors[monitorId]._isCanceledEntry)
					{
						break;
					}
					TimeSkipButtonAnim(InputManager.ButtonSetting.Button05);
					for (int j = 0; j < _monitors.Length; j++)
					{
						if (_monitors[j]._isEntry)
						{
							container.processManager.DecrementTime(j, 10);
						}
					}
					break;
				}
				default:
					if (!_monitors[monitorId]._isAwake)
					{
						break;
					}
					switch (_monitors[monitorId]._button_mode)
					{
					case ModeSelectMonitor.MonitorButtonMode.MonitorButtonModeCommon:
						if (_monitors[monitorId]._state >= ModeSelectMonitor.MonitorState.SelectCardWaitInit)
						{
							_monitors[monitorId].TimeSkipButtonAnim(InputManager.ButtonSetting.Button05);
							container.processManager.DecrementTime(monitorId, 10);
						}
						break;
					case ModeSelectMonitor.MonitorButtonMode.MonitorButtonModeEMoney:
						_monitors[monitorId].BackButtonAnim(InputManager.ButtonSetting.Button05);
						break;
					}
					break;
				}
			}
			else if (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button08, InputManager.TouchPanelArea.A8) || InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button08, InputManager.TouchPanelArea.A8, 1000L))
			{
				switch (_active_mode)
				{
				case ProccessActiveMode.Common:
					if (_monitors[monitorId]._isAwake && !_monitors[monitorId]._isDecidedEntry && !_monitors[monitorId]._isCanceledEntry && !_monitors[0].IsToggleAnim() && !_monitors[1].IsToggleAnim())
					{
						SettingButtonAnim(InputManager.ButtonSetting.Button08);
					}
					break;
				default:
					_monitors[monitorId].SettingButtonAnim(InputManager.ButtonSetting.Button08);
					break;
				}
			}
			else if (!_isBlockingDecideBalance && (_monitors[monitorId]._state == ModeSelectMonitor.MonitorState.InsertCoinWait || _monitors[monitorId]._state == ModeSelectMonitor.MonitorState.DecideCreditWait) && (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Select) || InputManager.GetButtonLongPush(monitorId, InputManager.ButtonSetting.Select, 1000L)))
			{
				ProccessActiveMode active_mode = _active_mode;
				if (active_mode == ProccessActiveMode.Individual && _monitors[monitorId]._isAwake)
				{
					bool flag = false;
					int num4 = (monitorId + 1) % _monitors.Length;
					if (_monitors[num4]._isAwake && (_monitors[num4]._isSelectButton || _monitors[num4]._button_mode == ModeSelectMonitor.MonitorButtonMode.MonitorButtonModeEMoney))
					{
						flag = true;
					}
					if (!flag && _monitors[monitorId]._button_mode == ModeSelectMonitor.MonitorButtonMode.MonitorButtonModeCommon)
					{
						_monitors[monitorId].SelectButtonAnim(InputManager.ButtonSetting.Select);
						if (_monitors[monitorId]._isSelectButton)
						{
							_isBlockingDecideBalance = true;
							_blockingCount2 = 0;
							Singleton<OperationManager>.Instance.SetCoinBlockerMode(CoinBlocker.Mode.Balance);
							if (_monitors[num4]._isAwake)
							{
								_monitors[num4]._isOwnBalanceBlocking = true;
							}
							for (int k = 0; k < _monitors.Length; k++)
							{
								if (_monitors[k]._isAwake)
								{
									_monitors[k]._isBlockingDecideBalance = true;
								}
							}
						}
					}
				}
			}
			else if (_monitors[monitorId]._balanceCommon._state == BalanceCommon.BalanceCommonState.EMoney01CreditWait && (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button02, InputManager.TouchPanelArea.A2) || InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button02, InputManager.TouchPanelArea.A2, 1000L)))
			{
				ProccessActiveMode active_mode = _active_mode;
				if (active_mode == ProccessActiveMode.Individual && _monitors[monitorId]._isAwake)
				{
					ModeSelectMonitor.MonitorButtonMode button_mode = _monitors[monitorId]._button_mode;
					if (button_mode == ModeSelectMonitor.MonitorButtonMode.MonitorButtonModeEMoney)
					{
						_monitors[monitorId].BalanceButtonAnim(InputManager.ButtonSetting.Button02);
					}
				}
			}
			else if (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3) || InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L))
			{
				switch (_active_mode)
				{
				case ProccessActiveMode.Common:
					if (_monitors[monitorId]._isAwake && !_monitors[monitorId]._isDecidedEntry && !_monitors[monitorId]._isCanceledEntry)
					{
						RightButtonAnim(InputManager.ButtonSetting.Button03);
					}
					break;
				default:
					if (!_monitors[monitorId]._isAwake)
					{
						break;
					}
					switch (_monitors[monitorId]._button_mode)
					{
					case ModeSelectMonitor.MonitorButtonMode.MonitorButtonModeCommon:
						_monitors[monitorId].RightButtonAnim(InputManager.ButtonSetting.Button03);
						break;
					case ModeSelectMonitor.MonitorButtonMode.MonitorButtonModeEMoney:
						if (!_monitors[monitorId]._balanceCommon._isRightLimits)
						{
							_monitors[monitorId].RightButtonAnim(InputManager.ButtonSetting.Button03);
						}
						break;
					}
					break;
				}
			}
			else if (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6) || InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L))
			{
				switch (_active_mode)
				{
				case ProccessActiveMode.Common:
					if (_monitors[monitorId]._isAwake && !_monitors[monitorId]._isDecidedEntry && !_monitors[monitorId]._isCanceledEntry)
					{
						LeftButtonAnim(InputManager.ButtonSetting.Button06);
					}
					break;
				default:
					if (!_monitors[monitorId]._isAwake)
					{
						break;
					}
					switch (_monitors[monitorId]._button_mode)
					{
					case ModeSelectMonitor.MonitorButtonMode.MonitorButtonModeCommon:
						_monitors[monitorId].LeftButtonAnim(InputManager.ButtonSetting.Button06);
						break;
					case ModeSelectMonitor.MonitorButtonMode.MonitorButtonModeEMoney:
						if (!_monitors[monitorId]._balanceCommon._isLeftLimits)
						{
							_monitors[monitorId].LeftButtonAnim(InputManager.ButtonSetting.Button06);
						}
						break;
					}
					break;
				}
			}
			if (!_isSetting)
			{
				return;
			}
			int num5 = -1;
			if (InputManager.GetTouchPanelAreaDown(monitorId, InputManager.TouchPanelArea.A1) || InputManager.GetTouchPanelAreaLongPush(monitorId, InputManager.TouchPanelArea.A1, 1000L))
			{
				num5 = 0;
				if (_monitors[monitorId]._isAwake && !_monitors[monitorId]._isDecidedEntry && !_monitors[monitorId]._isCanceledEntry && !_monitors[monitorId].IsToggleAnim(num5))
				{
					_monitors[monitorId].ToggleButtonAnim(InputManager.TouchPanelArea.A1);
				}
				return;
			}
			if (InputManager.GetTouchPanelAreaDown(monitorId, InputManager.TouchPanelArea.D2) || InputManager.GetTouchPanelAreaLongPush(monitorId, InputManager.TouchPanelArea.D2, 1000L))
			{
				num5 = 0;
				if (_monitors[monitorId]._isAwake && !_monitors[monitorId]._isDecidedEntry && !_monitors[monitorId]._isCanceledEntry && !_monitors[monitorId].IsToggleAnim(num5))
				{
					_monitors[monitorId].ToggleButtonAnim(InputManager.TouchPanelArea.D2);
				}
				return;
			}
			if (InputManager.GetTouchPanelAreaDown(monitorId, InputManager.TouchPanelArea.E2) || InputManager.GetTouchPanelAreaLongPush(monitorId, InputManager.TouchPanelArea.E2, 1000L))
			{
				num5 = 1;
				if (_monitors[monitorId]._isAwake && !_monitors[monitorId]._isDecidedEntry && !_monitors[monitorId]._isCanceledEntry && !_monitors[monitorId].IsToggleAnim(num5))
				{
					_monitors[monitorId].ToggleButtonAnim(InputManager.TouchPanelArea.E2);
				}
				return;
			}
			if (InputManager.GetTouchPanelAreaDown(monitorId, InputManager.TouchPanelArea.B2) || InputManager.GetTouchPanelAreaLongPush(monitorId, InputManager.TouchPanelArea.B2, 1000L))
			{
				num5 = 2;
				if (_monitors[monitorId]._isAwake && !_monitors[monitorId]._isDecidedEntry && !_monitors[monitorId]._isCanceledEntry && !_monitors[monitorId].IsToggleAnim(num5))
				{
					_monitors[monitorId].ToggleButtonAnim(InputManager.TouchPanelArea.B2);
				}
				return;
			}
			if (InputManager.GetTouchPanelAreaDown(monitorId, InputManager.TouchPanelArea.B3) || InputManager.GetTouchPanelAreaLongPush(monitorId, InputManager.TouchPanelArea.B3, 1000L))
			{
				num5 = 3;
				if (_monitors[monitorId]._isAwake && !_monitors[monitorId]._isDecidedEntry && !_monitors[monitorId]._isCanceledEntry && !_monitors[monitorId].IsToggleAnim(num5))
				{
					_monitors[monitorId].ToggleButtonAnim(InputManager.TouchPanelArea.B3);
				}
				return;
			}
			bool touchPanelAreaLongPush = InputManager.GetTouchPanelAreaLongPush(monitorId, InputManager.TouchPanelArea.E6, 1000L);
			if (InputManager.GetTouchPanelAreaDown(monitorId, InputManager.TouchPanelArea.E6) || touchPanelAreaLongPush)
			{
				num5 = 0;
				if (_monitors[monitorId]._isAwake && !_monitors[monitorId]._isDecidedEntry && !_monitors[monitorId]._isCanceledEntry && !_monitors[monitorId].IsVolumeAnim(num5))
				{
					_monitors[monitorId].VolumeButtonAnim(InputManager.TouchPanelArea.E6, touchPanelAreaLongPush);
					if (_monitors[monitorId]._isHoldVolume)
					{
						SetInputLockInfo(monitorId, 100f);
					}
				}
				return;
			}
			bool touchPanelAreaLongPush2 = InputManager.GetTouchPanelAreaLongPush(monitorId, InputManager.TouchPanelArea.E4, 1000L);
			if (InputManager.GetTouchPanelAreaDown(monitorId, InputManager.TouchPanelArea.E4) || touchPanelAreaLongPush2)
			{
				num5 = 1;
				if (_monitors[monitorId]._isAwake && !_monitors[monitorId]._isDecidedEntry && !_monitors[monitorId]._isCanceledEntry && !_monitors[monitorId].IsVolumeAnim(num5))
				{
					_monitors[monitorId].VolumeButtonAnim(InputManager.TouchPanelArea.E4, touchPanelAreaLongPush2);
					if (_monitors[monitorId]._isHoldVolume)
					{
						SetInputLockInfo(monitorId, 100f);
					}
				}
			}
			else if (_monitors[monitorId]._isHoldVolume)
			{
				_monitors[monitorId]._settingVolumeMinus.GetComponent<Animator>().Play("Loop", 0, 0f);
				_monitors[monitorId]._settingVolumePlus.GetComponent<Animator>().Play("Loop", 0, 0f);
				_monitors[monitorId]._isHoldVolume = false;
			}
		}

		public override void OnLateUpdate()
		{
		}

		private void TimeUp()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				if (_monitors[i]._isEnableTimer)
				{
					container.processManager.ForceTimeUp(i);
					container.processManager.SetVisibleTimer(i, isVisible: false);
					_monitors[i]._isEnableTimer = false;
				}
				_monitors[i]._isTimeUp = true;
			}
			_isTimeUp = true;
		}
	}
}
