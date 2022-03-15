using System;
using System.Collections.Generic;
using AMDaemon;
using Balance;
using DB;
using IO;
using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_000001;
using Manager;
using Manager.MaiStudio;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Monitor.TicketSelect
{
	public class TicketSelectMonitor : MonitorBase
	{
		public enum MonitorState
		{
			None,
			Init,
			SelectCardFadeInJudge,
			SelectCardFadeInJudgeWait,
			SelectCardFadeInWait,
			RecoveryTicketInit,
			RecoveryTicketFadeIn,
			RecoveryTicketFadeInWait,
			RecoveryTicketWait,
			RecoveryTicketFadeOut,
			RecoveryTicketFadeOutWait,
			SelectCardReset,
			SelectCardWaitInit,
			SelectCardWait,
			SelectCardMoveRight,
			SelectCardMoveLeft,
			CreditJudgeInit,
			CreditJudge,
			EMoneyWait,
			EMoneyExit,
			InsertCoinFadeIn,
			InsertCoinFadeInWait,
			InsertCoinWait,
			InsertCoinFadeOut,
			InsertCoinFadeOutWait,
			DecideCreditFadeIn,
			DecideCreditFadeInWait,
			DecideCreditWait,
			DecideCreditAnotherBlocking,
			DecideCreditAnotherBlockingWait,
			DecideCreditRequestAllow,
			DecideCreditRequestAllowWait,
			DecideCreditFadeOut,
			DecideCreditFadeOutWait,
			CreditJudgeReset,
			DecidedEntryFadeIn,
			DecidedEntryFadeInWait,
			DecidedEntryWait01,
			DecidedEntryWait02,
			DecidedEntryFadeOut,
			DecidedEntryFadeOutWait,
			Finish,
			End
		}

		public enum MonitorButtonMode
		{
			MonitorButtonModeCommon,
			MonitorButtonModeEMoney
		}

		public enum TicketSelectType
		{
			TicketSelectTypeInvalid = -1,
			TicketSelectTypeNone,
			TicketSelectTypeMAX
		}

		public enum InfoWindowState
		{
			None,
			Judge,
			Open,
			OpenWait,
			Wait,
			Close,
			CloseWait,
			End
		}

		[SerializeField]
		public TicketSelectButtonController _buttonController;

		[SerializeField]
		public GameObject _masterNullSelector;

		[SerializeField]
		public GameObject _creditWindow;

		[SerializeField]
		public GameObject _emoneyWindow;

		[SerializeField]
		public GameObject _ticketNoUseInfoWindow;

		[SerializeField]
		public GameObject _ticketInfoWindow;

		[SerializeField]
		public GameObject _ticketExceptionWindow;

		[SerializeField]
		public GameObject _messageWindow;

		[SerializeField]
		public GameObject _lastBlur;

		public MonitorState _state = MonitorState.End;

		public MonitorButtonMode _button_mode;

		private int _timer;

		private int _sub_count;

		private int _monitorID;

		public bool _isEntry;

		public bool _isGuest;

		public bool _isAwake;

		private bool _isEnableOK;

		public bool _isCheckOK;

		public bool _isOK;

		public bool _isServerRequest;

		public bool _isServerAccepted;

		public bool _isServerRejected;

		public bool _isCallRejectedInfo;

		public bool _isAnotherBlocking;

		public bool _isRecallAnoterBlur;

		public bool _isServerWaitCount;

		public int _nServerWaitTimer = 120;

		public bool _isRecoveryTicket;

		public int _recoveryTicketID = -1;

		public bool _isPaidCreditAsTicketCost;

		public bool _isCheckLR;

		public bool _isRight;

		public bool _isLeft;

		public bool _isCheckBack;

		public bool _isBack;

		private bool _isEnableBack;

		public int _isEntryNum = 1;

		private List<GameObject> _nullListMiniCard = new List<GameObject>();

		private List<GameObject> _nullListBigCard = new List<GameObject>();

		private List<int> _objTicketSelectListRefID = new List<int>();

		private List<CommonValue> _valueListMiniCardPosX = new List<CommonValue>();

		private List<CommonValue> _valueListMiniCardScaleXY = new List<CommonValue>();

		private List<CommonValue> _valueListBigCardPosX = new List<CommonValue>();

		private List<CommonValue> _valueListBigCardScaleXY = new List<CommonValue>();

		private List<int> _ticketSelectTicketID = new List<int>();

		private int _ticketDefaultRefID = -1;

		private int _dispCardMax = 9;

		private float[] _cardMiniPosX = new float[9] { -606f, -465f, -323f, -182f, 0f, 182f, 323f, 465f, 606f };

		private float[] _cardMiniScaleXY = new float[9] { 0.75f, 0.75f, 0.75f, 0.75f, 1f, 0.75f, 0.75f, 0.75f, 0.75f };

		private float[] _cardBigPosX = new float[5] { -1080f, -540f, 0f, 540f, 1080f };

		private float[] _cardBigScaleXY = new float[5] { 1f, 1f, 1f, 1f, 1f };

		private float _baseBigPosY = 65f;

		private List<bool> _ticketSelectEnable = new List<bool>();

		private float _cardMoveTime = 10f;

		private int _cardPosEnd;

		private bool _isRightButtonVisibleRequest;

		private bool _isLeftButtonVisibleRequest;

		private bool _isRightButtonVisible;

		private bool _isLeftButtonVisible;

		public bool _isEnableTimer;

		public bool _isTimerVisible;

		public bool _isDecidedOK;

		public bool _isBothDecidedEntry;

		public bool _isTimeUp;

		public int _decidedSelectTicketID = -1;

		public bool _isCallBlur;

		public bool _isChangeSibling;

		public bool _isCallLastBlur;

		public bool _isChangeLastBlur;

		public int _needCreditNum = -1;

		public int _needCoinNum = -1;

		public int _needCreditDenominator = -1;

		public int _needCreditNumerator = -1;

		public bool _isNeedCreditFraction;

		public bool _isBlockingDecideCredit;

		public bool _isDecidedEntry;

		public int _isDecidedEntryNum;

		public bool _isTicketSE;

		private List<TicketSelectData> _ticketDataList;

		public bool _isCallTicketPrepare;

		public bool _isGrayOKButton;

		public bool _isRequestChangeOKButtonColor;

		public bool _isSetGrayOKButton;

		public bool _isGrayBackButton;

		public bool _isRequestChangeBackButtonColor;

		public bool _isSetGrayBackButton;

		public bool _isGrayBalanceButton;

		public bool _isRequestChangeBalanceButtonColor;

		public bool _isSetGrayBalanceButton;

		public bool _isDispInfoWindow;

		public bool _isInvisibleButtonDispInfoWindow;

		public bool _isCallVoice;

		public bool _isInfoWindowVoice;

		public List<WindowMessageID> _windowMessageList = new List<WindowMessageID>();

		public InfoWindowState _info_state;

		public uint _info_timer;

		public uint _info_count;

		public bool _isSelectButton;

		public bool _isEMoneyReaderSuccess;

		public bool _isEMoneyReaderFailed;

		public bool _isCheckBalance;

		public bool _isBalance;

		private bool _isEnableBalance;

		public bool _isBlockingDecideBalance;

		public bool _isOwnBalanceBlocking;

		public bool _isResetAnotherBalanceBlocking;

		public bool _isBalanceNetworkDisconnection;

		public BalanceCommon _balanceCommon = new BalanceCommon();

		public bool _isBalanceAuthCompleted;

		public bool _isBlockingDecideTicket;

		public bool _isBoughtTicket;

		public bool _isSetAccounting;

		public bool IsEnableTicketSelectType()
		{
			int index = _dispCardMax / 2;
			bool result = false;
			int num = _objTicketSelectListRefID[index];
			if (num >= 0)
			{
				result = _ticketSelectEnable[num];
			}
			return result;
		}

		public bool IsEndAnim(Animator anim)
		{
			if (anim == null || anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
			{
				return true;
			}
			return false;
		}

		public void SetActiveChildren(GameObject obj, bool active)
		{
			int childCount = obj.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				obj.transform.GetChild(i).gameObject.SetActive(active);
			}
		}

		public void OKButtonAnim(InputManager.ButtonSetting button)
		{
			int animationActive = 0;
			if (!_isBlockingDecideCredit && !_isBlockingDecideTicket && _isCheckOK && !_isOK && IsEnableTicketSelectType() && !_isRight && !_isLeft && !_isGrayOKButton)
			{
				_buttonController.SetAnimationActive(animationActive);
				_isOK = true;
			}
		}

		public void BackButtonAnim(InputManager.ButtonSetting button)
		{
			int animationActive = 2;
			if (_isCheckBack && !_isBack && !_isGrayBackButton)
			{
				_buttonController.SetAnimationActive(animationActive);
				_isBack = true;
			}
		}

		public void RightButtonAnim(InputManager.ButtonSetting button)
		{
			int animationActive = 3;
			switch (_button_mode)
			{
			case MonitorButtonMode.MonitorButtonModeCommon:
				animationActive = 3;
				break;
			case MonitorButtonMode.MonitorButtonModeEMoney:
				animationActive = 5;
				break;
			}
			if (!_isCheckLR || _isRight || _isLeft || !_isAwake || _isDecidedEntry)
			{
				return;
			}
			switch (_button_mode)
			{
			case MonitorButtonMode.MonitorButtonModeCommon:
				if (_isRightButtonVisible)
				{
					_buttonController.SetAnimationActive(animationActive);
					_isRight = true;
				}
				break;
			case MonitorButtonMode.MonitorButtonModeEMoney:
				_buttonController.SetAnimationActive(animationActive);
				_isRight = true;
				break;
			}
		}

		public void LeftButtonAnim(InputManager.ButtonSetting button)
		{
			int animationActive = 4;
			switch (_button_mode)
			{
			case MonitorButtonMode.MonitorButtonModeCommon:
				animationActive = 4;
				break;
			case MonitorButtonMode.MonitorButtonModeEMoney:
				animationActive = 6;
				break;
			}
			if (!_isCheckLR || _isRight || _isLeft || !_isAwake || _isDecidedEntry)
			{
				return;
			}
			switch (_button_mode)
			{
			case MonitorButtonMode.MonitorButtonModeCommon:
				if (_isLeftButtonVisible)
				{
					_buttonController.SetAnimationActive(animationActive);
					_isLeft = true;
				}
				break;
			case MonitorButtonMode.MonitorButtonModeEMoney:
				_buttonController.SetAnimationActive(animationActive);
				_isLeft = true;
				break;
			}
		}

		public void BalanceButtonAnim(InputManager.ButtonSetting button)
		{
			int animationActive = 7;
			if (_isCheckBalance && !_isBalance && !_isGrayBalanceButton)
			{
				_buttonController.SetAnimationActive(animationActive);
				_isBalance = true;
			}
		}

		public void SelectButtonAnim(InputManager.ButtonSetting button)
		{
			if (_isAwake && !_isDecidedEntry && !_isBlockingDecideBalance && !_isBalanceNetworkDisconnection && GotoBalance() && AMDaemon.EMoney.IsAuthCompleted && AMDaemon.EMoney.Operation.CanOperateDeal && !_isSelectButton)
			{
				_isSelectButton = true;
			}
		}

		public void isOKTimerZero()
		{
			if (_isOK)
			{
				_timer = 0;
			}
		}

		public void OKStart()
		{
			if (!_isEnableOK)
			{
				_isEnableOK = true;
				_buttonController.SetVisible(true, default(int));
				_isCheckOK = true;
			}
		}

		public void ResetOKStart()
		{
			if (_isEnableOK)
			{
				_buttonController.SetVisible(false, default(int));
			}
			_isEnableOK = false;
			_isCheckOK = false;
			_isOK = false;
		}

		public void isBackedTimerZero()
		{
			if (_isBack)
			{
				_timer = 0;
			}
		}

		public void BackStart()
		{
			if (!_isEnableBack)
			{
				_isEnableBack = true;
				_buttonController.SetVisible(true, 2);
				_isCheckBack = true;
			}
		}

		public void ResetBackStart()
		{
			if (_isEnableBack)
			{
				_buttonController.SetVisible(false, 2);
			}
			_isEnableBack = false;
			_isCheckBack = false;
			_isBack = false;
		}

		public void BalanceStart()
		{
			if (!_isEnableBalance)
			{
				_isEnableBalance = true;
				_buttonController.SetVisible(true, 7);
				_isCheckBalance = true;
			}
		}

		public void ResetBalanceStart()
		{
			if (_isEnableBalance)
			{
				_buttonController.SetVisible(false, 7);
			}
			_isEnableBalance = false;
			_isCheckBalance = false;
			_isBalance = false;
		}

		public GameObject GetBlurObject()
		{
			int index = 7;
			if (_isChangeSibling)
			{
				index = 1;
			}
			return _masterNullSelector.transform.parent.transform.GetChild(index).gameObject;
		}

		public void SetBlur()
		{
			GameObject blurObject = GetBlurObject();
			blurObject.SetActive(value: true);
			CanvasGroup component = blurObject.GetComponent<CanvasGroup>();
			if (component != null)
			{
				component.alpha = 1f;
			}
		}

		public void ResetBlur()
		{
			GameObject blurObject = GetBlurObject();
			blurObject.SetActive(value: false);
			CanvasGroup component = blurObject.GetComponent<CanvasGroup>();
			if (component != null)
			{
				component.alpha = 0f;
			}
		}

		public void SetLastBlur()
		{
			GameObject lastBlur = _lastBlur;
			lastBlur.SetActive(value: true);
			CanvasGroup component = lastBlur.GetComponent<CanvasGroup>();
			if (component != null)
			{
				component.alpha = 1f;
			}
		}

		public void ResetLastBlur()
		{
			GameObject lastBlur = _lastBlur;
			lastBlur.SetActive(value: false);
			CanvasGroup component = lastBlur.GetComponent<CanvasGroup>();
			if (component != null)
			{
				component.alpha = 0f;
			}
		}

		public bool IsSkipInsertCreditInfo()
		{
			bool result = false;
			bool flag = false;
			TicketData ticketData = null;
			int decidedSelectTicketID = _decidedSelectTicketID;
			if ((uint)(decidedSelectTicketID - -1) <= 1u)
			{
				result = true;
				flag = true;
			}
			if (!flag)
			{
				ticketData = Singleton<DataManager>.Instance.GetTicket(_decidedSelectTicketID);
				if (ticketData != null)
				{
					switch (ConvertTicketKind(ticketData))
					{
					case TicketKind.Invalid:
					case TicketKind.None:
					case TicketKind.Event:
					case TicketKind.Free:
						result = true;
						break;
					case TicketKind.Paid:
						decidedSelectTicketID = ticketData.areaPercent;
						if (decidedSelectTicketID != 200)
						{
							_ = 300;
						}
						result = false;
						break;
					}
				}
			}
			return result;
		}

		public TicketKind ConvertTicketKind(TicketData data)
		{
			TicketKind result = TicketKind.Invalid;
			if (data != null)
			{
				result = (TicketKind)data.ticketKind.id;
				if (data.ticketKind.id == 0)
				{
					switch (data.areaPercent)
					{
					case 200:
						result = TicketKind.Paid;
						break;
					case 300:
						result = TicketKind.Paid;
						break;
					case 500:
						result = TicketKind.Event;
						break;
					case 150:
						result = TicketKind.Free;
						break;
					}
				}
			}
			return result;
		}

		public TicketKind ConvertTicketKind(TicketSelectData data)
		{
			TicketKind result = TicketKind.Invalid;
			if (data != null)
			{
				result = data.ticketKind;
				if (data.ticketKind == TicketKind.None)
				{
					switch (data.areaPercent)
					{
					case 200:
						result = TicketKind.Paid;
						break;
					case 300:
						result = TicketKind.Paid;
						break;
					case 500:
						result = TicketKind.Event;
						break;
					case 150:
						result = TicketKind.Free;
						break;
					}
				}
			}
			return result;
		}

		public int GetMiniSelectorChildID(TicketSelectData data)
		{
			int result = -1;
			if (data != null)
			{
				switch (ConvertTicketKind(data))
				{
				case TicketKind.None:
					result = 0;
					break;
				case TicketKind.Free:
					result = 1;
					break;
				case TicketKind.Event:
					result = 2;
					break;
				case TicketKind.Paid:
					result = data.areaPercent switch
					{
						200 => 3, 
						300 => 4, 
						_ => 3, 
					};
					break;
				}
			}
			return result;
		}

		public void SetMiniSelector()
		{
			int num = 1;
			int num2 = 0;
			int num3 = 0;
			string text = "";
			GameObject gameObject = null;
			int num4 = -1;
			int num5 = -1;
			int num6 = -1;
			TicketSelectData ticketSelectData = null;
			for (int i = 0; i < _dispCardMax; i++)
			{
				ticketSelectData = null;
				num4 = _objTicketSelectListRefID[i];
				if (num4 == -1)
				{
					continue;
				}
				if (_ticketDataList != null && num4 >= 0 && num4 <= _ticketDataList.Count - 1)
				{
					ticketSelectData = _ticketDataList[num4];
				}
				if (ticketSelectData == null || ConvertTicketKind(ticketSelectData) == TicketKind.Invalid)
				{
					continue;
				}
				num5 = ticketSelectData.ticketID;
				num = GetSelectorCreditNum(num5);
				_ = _ticketSelectEnable[num4];
				if (num == 0)
				{
					switch (ConvertTicketKind(ticketSelectData))
					{
					case TicketKind.Free:
						num2 = ticketSelectData.ticketNum;
						if (num2 > ticketSelectData.maxTiceketNum)
						{
							num2 = ticketSelectData.maxTiceketNum;
						}
						text = num2.ToString("0");
						gameObject = _nullListMiniCard[i].transform.GetChild(0).gameObject;
						SetActiveChildren(gameObject, active: false);
						num6 = GetMiniSelectorChildID(ticketSelectData);
						if (num6 != -1)
						{
							gameObject.transform.GetChild(num6).gameObject.SetActive(value: true);
							gameObject.transform.GetChild(num6).GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = text;
						}
						break;
					case TicketKind.Event:
					{
						DateTime dateTime = TimeManager.GetDateTime(ticketSelectData.expirationUnixTime);
						DateTime dateTime2 = TimeManager.GetDateTime(TimeManager.PlayBaseTime);
						TimeSpan timeSpan = dateTime - dateTime2;
						bool flag = false;
						bool flag2 = false;
						if (timeSpan.Days < 0)
						{
							num3 = 0;
						}
						else
						{
							if (timeSpan.Minutes > 0)
							{
								flag = true;
							}
							if (timeSpan.Hours > 0)
							{
								flag2 = true;
							}
							num3 = timeSpan.Days;
							if (!flag2 && flag)
							{
								flag2 = true;
							}
							if (flag2)
							{
								num3++;
							}
						}
						if (num3 >= 99)
						{
							num3 = 99;
						}
						text = num3.ToString("0");
						gameObject = _nullListMiniCard[i].transform.GetChild(0).gameObject;
						SetActiveChildren(gameObject, active: false);
						num6 = GetMiniSelectorChildID(ticketSelectData);
						if (num6 != -1)
						{
							gameObject.transform.GetChild(num6).gameObject.SetActive(value: true);
							gameObject.transform.GetChild(num6).GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = text;
						}
						break;
					}
					}
				}
				else if (num > 0)
				{
					text = num.ToString("0");
					gameObject = _nullListMiniCard[i].transform.GetChild(0).gameObject;
					num6 = GetMiniSelectorChildID(ticketSelectData);
					if (num6 != -1)
					{
						gameObject.transform.GetChild(num6).gameObject.SetActive(value: true);
						gameObject.transform.GetChild(num6).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = text;
					}
					if (num4 >= 0)
					{
						bool flag3 = false;
						flag3 = ((!_ticketSelectEnable[num4]) ? true : false);
						num6 = 5;
						gameObject.transform.GetChild(num6).gameObject.SetActive(flag3);
					}
				}
			}
		}

		public void SetBigSelector(int anim_type)
		{
			int num = 1;
			string text = "";
			string text2 = "";
			string text3 = "";
			string text4 = "";
			int num2 = -1;
			int num3 = -1;
			int num4 = -1;
			int index = _dispCardMax / 2;
			TicketSelectData ticketSelectData = null;
			num2 = _objTicketSelectListRefID[index];
			if (_ticketDataList != null && num2 >= 0 && num2 <= _ticketDataList.Count - 1)
			{
				ticketSelectData = _ticketDataList[num2];
			}
			SetActiveChildren(_nullListBigCard[0], active: false);
			num4 = GetMiniSelectorChildID(ticketSelectData);
			if (num4 != -1)
			{
				_nullListBigCard[0].transform.GetChild(num4).gameObject.SetActive(value: true);
				switch (anim_type)
				{
				case 1:
					_nullListBigCard[0].GetComponent<Animator>().Play("In", 0, 0f);
					break;
				case 2:
					_nullListBigCard[0].GetComponent<Animator>().Play("Out", 0, 0f);
					break;
				}
			}
			if (ticketSelectData == null || ConvertTicketKind(ticketSelectData) == TicketKind.Invalid)
			{
				return;
			}
			num3 = ticketSelectData.ticketID;
			num = GetSelectorCreditNum(num3);
			_ = _ticketSelectEnable[num2];
			if (num == 0)
			{
				int num5 = (int)ConvertTicketKind(ticketSelectData);
				if (num5 != 2)
				{
					_ = 3;
					return;
				}
				DateTime dateTime = TimeManager.GetDateTime(ticketSelectData.expirationUnixTime);
				TimeSpan timeSpan = new TimeSpan(0, 7, 0, 0);
				DateTime dateTime2 = DateTime.Parse((dateTime - timeSpan).ToShortDateString() + " 0:00:00");
				text2 = dateTime2.Year.ToString("0000");
				text3 = dateTime2.Month.ToString("00");
				text4 = dateTime2.Day.ToString("00");
				GameObject obj = _nullListBigCard[0].transform.GetChild(num4).gameObject;
				text = text2 + "年";
				obj.transform.GetChild(5).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = text;
				text = text3 + "月" + text4 + "日";
				obj.transform.GetChild(5).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = text;
			}
			else if (num > 0)
			{
				text = num.ToString("0");
				_nullListBigCard[0].transform.GetChild(num4).gameObject.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = text;
			}
		}

		public int GetBigSelectorTexID(int ticket_id)
		{
			int result = 1;
			bool flag = false;
			TicketData ticketData = null;
			int decidedSelectTicketID = _decidedSelectTicketID;
			if ((uint)(decidedSelectTicketID - -1) <= 1u)
			{
				result = 1;
				flag = true;
			}
			if (!flag)
			{
				ticketData = Singleton<DataManager>.Instance.GetTicket(_decidedSelectTicketID);
				if (ticketData != null)
				{
					switch (ConvertTicketKind(ticketData))
					{
					case TicketKind.Invalid:
					case TicketKind.None:
						result = 1;
						break;
					case TicketKind.Free:
						result = 1;
						break;
					case TicketKind.Event:
						result = 0;
						break;
					case TicketKind.Paid:
						result = 2;
						switch (ticketData.areaPercent)
						{
						case 200:
							result = 2;
							break;
						case 300:
							result = 3;
							break;
						}
						break;
					}
				}
			}
			return result;
		}

		public int GetSelectorCreditNum(int ticket_id)
		{
			int result = 1;
			TicketData ticketData = null;
			ticketData = Singleton<DataManager>.Instance.GetTicket(ticket_id);
			if (ticketData != null)
			{
				result = ticketData.creditNum;
			}
			return result;
		}

		public void SetNeedCreditNum()
		{
			GameObject gameObject = null;
			string text = "";
			gameObject = _creditWindow.transform.GetChild(0).GetChild(1).gameObject;
			text = ((!_isNeedCreditFraction) ? (_needCreditNum.ToString("0") + " クレジット必要です") : ((_needCreditNumerator == 0) ? (_needCreditNum.ToString("0") + " クレジット必要です") : ((_needCreditNum != 0 || _needCreditNumerator <= 0) ? (_needCreditNum.ToString("0") + " " + _needCreditNumerator.ToString("0") + "/" + _needCreditDenominator.ToString("0") + " クレジット必要です") : (_needCreditNumerator.ToString("0") + "/" + _needCreditDenominator.ToString("0") + " クレジット必要です"))));
			gameObject.transform.GetChild(1).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = text;
		}

		public bool GotoBalance()
		{
			int num = 14;
			bool flag = false;
			if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.IsFreePlay() || !Singleton<OperationManager>.Instance.IsCoinAcceptable())
			{
				flag = false;
			}
			else if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.NowCredit > num)
			{
				flag = false;
			}
			else
			{
				flag = true;
				if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.NowCredit == num)
				{
					flag = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.CoinToCredit <= 1 || ((SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.Remain == 0) ? true : false);
				}
			}
			return flag;
		}

		public override void Initialize(int monIndex, bool active)
		{
			_monitorID = monIndex;
			base.Initialize(monIndex, active);
			if (IsActive())
			{
				MechaManager.LedIf[monIndex].ButtonLedReset();
			}
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(_monitorID);
			_isEntry = userData.IsEntry;
			_isGuest = userData.IsGuest();
			_timer = 0;
			_sub_count = 0;
			_baseBigPosY = 65f;
			_isDispInfoWindow = false;
			_isChangeSibling = false;
			_info_state = InfoWindowState.None;
			_info_timer = 0u;
			_info_count = 0u;
			GameObject gameObject = null;
			GameObject gameObject2 = null;
			gameObject = _masterNullSelector.gameObject.transform.parent.gameObject.gameObject.transform.parent.gameObject;
			gameObject2 = _masterNullSelector.transform.GetChild(1).gameObject;
			for (int i = 0; i < _dispCardMax; i++)
			{
				_nullListMiniCard.Add(gameObject2.transform.GetChild(i).gameObject);
			}
			_nullListBigCard.Add(_masterNullSelector.transform.GetChild(2).GetChild(0).GetChild(0)
				.gameObject);
				_ticketDataList = Singleton<TicketManager>.Instance.GetTicketData(_monitorID);
				int num = 0;
				for (int j = 0; j < _ticketDataList.Count; j++)
				{
					num = _ticketDataList[j].ticketID;
					if (num != -1)
					{
						_ticketSelectTicketID.Add(num);
						_ticketSelectEnable.Add(item: true);
					}
				}
				for (int k = 0; k < _ticketSelectTicketID.Count; k++)
				{
					if (_ticketSelectTicketID[k] == 0)
					{
						_ticketDefaultRefID = k;
						break;
					}
				}
				int num2 = _dispCardMax / 2;
				int num3 = _ticketSelectTicketID.Count - 1 - _ticketDefaultRefID;
				int num4 = 0;
				for (int l = 0; l < _dispCardMax; l++)
				{
					int num5 = l - num2;
					num4 = _ticketDefaultRefID + num5;
					bool flag = true;
					if (num4 < 0)
					{
						flag = false;
					}
					else if ((num3 == 0 && num5 > 0) || num5 > num3)
					{
						flag = false;
					}
					if (!flag)
					{
						_objTicketSelectListRefID.Add(-1);
					}
					else
					{
						_objTicketSelectListRefID.Add(num4);
					}
				}
				GameObject gameObject3 = null;
				num4 = 0;
				int num6 = -1;
				TicketSelectData ticketSelectData = null;
				for (int m = 0; m < _dispCardMax; m++)
				{
					ticketSelectData = null;
					num4 = _objTicketSelectListRefID[m];
					if (num4 != -1)
					{
						if (_ticketDataList != null && num4 >= 0 && num4 <= _ticketDataList.Count - 1)
						{
							ticketSelectData = _ticketDataList[num4];
						}
						gameObject3 = _nullListMiniCard[m].transform.GetChild(0).gameObject;
						SetActiveChildren(gameObject3, active: false);
						num6 = GetMiniSelectorChildID(ticketSelectData);
						if (num6 != -1)
						{
							gameObject3.transform.GetChild(num6).gameObject.SetActive(value: true);
						}
					}
				}
				for (int n = 0; n < _objTicketSelectListRefID.Count; n++)
				{
					if (_objTicketSelectListRefID[n] == -1)
					{
						_nullListMiniCard[n].SetActive(value: false);
					}
				}
				if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.IsFreePlay() && _ticketDataList != null && _ticketSelectEnable != null)
				{
					for (int num7 = 0; num7 < _ticketDataList.Count; num7++)
					{
						ticketSelectData = null;
						ticketSelectData = _ticketDataList[num7];
						if (ticketSelectData != null)
						{
							int num8 = (int)ConvertTicketKind(ticketSelectData);
							if (num8 == 1)
							{
								_ticketSelectEnable[num7] = false;
							}
						}
					}
				}
				SetMiniSelector();
				int bigSelector = 0;
				SetBigSelector(bigSelector);
				_creditWindow.SetActive(value: false);
				_emoneyWindow.SetActive(value: false);
				_ticketNoUseInfoWindow.SetActive(value: false);
				_ticketInfoWindow.SetActive(value: false);
				_ticketExceptionWindow.SetActive(value: false);
				_messageWindow.SetActive(value: false);
				_balanceCommon.Initialize(_creditWindow, _emoneyWindow, _buttonController);
				_balanceCommon.SetMode(BalanceCommon.ParentMode.ParentModeTicketSelect, null, this);
				bool flag2 = false;
				if (_isEntry && !_isGuest)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(_monitorID).UserType == UserData.UserIDType.New || GameManager.IsEventMode || GameManager.IsFreedomMode || GameManager.IsCourseMode)
					{
						_isAwake = true;
						_isDecidedOK = true;
						_isDecidedEntry = true;
						_decidedSelectTicketID = 0;
						flag2 = true;
					}
					else
					{
						_isAwake = true;
					}
				}
				if (!_isAwake || flag2)
				{
					for (int num9 = 0; num9 < gameObject.transform.childCount; num9++)
					{
						gameObject.transform.GetChild(num9).gameObject.SetActive(value: false);
					}
					_state = MonitorState.End;
					return;
				}
				_isRecoveryTicket = false;
				if (_isAwake && !flag2)
				{
					_recoveryTicketID = Singleton<TicketManager>.Instance.GetUserChargeId(_monitorID);
					if (_recoveryTicketID > 0)
					{
						_isRecoveryTicket = true;
					}
				}
				for (int num10 = 0; num10 < _dispCardMax; num10++)
				{
					CommonValue item = new CommonValue();
					_valueListMiniCardPosX.Add(item);
				}
				for (int num11 = 0; num11 < _dispCardMax; num11++)
				{
					CommonValue item2 = new CommonValue();
					_valueListMiniCardScaleXY.Add(item2);
				}
				for (int num12 = 0; num12 < _dispCardMax; num12++)
				{
					CommonValue item3 = new CommonValue();
					_valueListBigCardPosX.Add(item3);
				}
				for (int num13 = 0; num13 < _dispCardMax; num13++)
				{
					CommonValue item4 = new CommonValue();
					_valueListBigCardScaleXY.Add(item4);
				}
				_buttonController.Initialize(_monitorID);
				_isRightButtonVisibleRequest = false;
				_isLeftButtonVisibleRequest = false;
				_isRightButtonVisible = false;
				_isLeftButtonVisible = false;
				Image image = null;
				GameObject obj = _buttonController.transform.GetChild(2).GetChild(0).GetChild(0)
					.gameObject;
				obj.SetActive(value: false);
				image = obj.transform.GetChild(0).gameObject.GetComponent<Image>();
				if (image != null)
				{
					image.preserveAspect = true;
					image.SetNativeSize();
				}
				obj.SetActive(value: true);
				GameObject obj2 = _buttonController.transform.GetChild(7).GetChild(0).GetChild(0)
					.gameObject;
				obj2.SetActive(value: false);
				image = obj2.transform.GetChild(0).gameObject.GetComponent<Image>();
				if (image != null)
				{
					image.preserveAspect = true;
					image.SetNativeSize();
				}
				_buttonController.transform.GetChild(7).GetChild(0).GetChild(1)
					.gameObject.transform.localRotation = Quaternion.Euler(0f, -180f, 0f);
				obj2.SetActive(value: true);
				_buttonController.SetVisible(false, 3);
				_buttonController.SetVisible(false, 4);
				_buttonController.SetVisible(false, default(int));
				_buttonController.SetVisible(false, 1);
				_buttonController.SetVisible(false, 2);
				_buttonController.SetVisibleImmediate(false, 5);
				_buttonController.SetVisibleImmediate(false, 6);
				_buttonController.SetVisible(false, 7);
				_state = MonitorState.None;
			}

			public void ChangeOKButtonColor()
			{
				if (!_isRequestChangeOKButtonColor)
				{
					return;
				}
				Image image = null;
				if (_isSetGrayOKButton)
				{
					if (!_isGrayOKButton && !_isGrayOKButton)
					{
						image = _buttonController.transform.GetChild(4).GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
						if (image != null)
						{
							image.color = new Color(0.5019608f, 0.5019608f, 0.5019608f, 1f);
						}
						_isGrayOKButton = true;
						_buttonController.ChangeOKButtonColor(_isGrayOKButton);
					}
				}
				else if (!_isSetGrayOKButton)
				{
					if (_isGrayOKButton)
					{
						image = _buttonController.transform.GetChild(4).GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
						if (image != null)
						{
							image.color = new Color(1f, 1f, 1f, 1f);
						}
						_isGrayOKButton = false;
						_buttonController.ChangeOKButtonColor(_isGrayOKButton);
					}
					else
					{
						_ = _isGrayOKButton;
					}
				}
				_isRequestChangeOKButtonColor = false;
			}

			public void ChangeBackButtonColor()
			{
				if (!_isRequestChangeBackButtonColor)
				{
					return;
				}
				Image image = null;
				if (_isSetGrayBackButton)
				{
					if (!_isGrayBackButton && !_isGrayBackButton)
					{
						image = _buttonController.transform.GetChild(6).GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
						if (image != null)
						{
							image.color = new Color(0.5019608f, 0.5019608f, 0.5019608f, 1f);
						}
						_isGrayBackButton = true;
						_buttonController.ChangeBackButtonColor(_isGrayBackButton);
					}
				}
				else if (!_isSetGrayBackButton)
				{
					if (_isGrayBackButton)
					{
						image = _buttonController.transform.GetChild(6).GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
						if (image != null)
						{
							image.color = new Color(1f, 1f, 1f, 1f);
						}
						_isGrayBackButton = false;
						_buttonController.ChangeBackButtonColor(_isGrayBackButton);
					}
					else
					{
						_ = _isGrayBackButton;
					}
				}
				_isRequestChangeBackButtonColor = false;
			}

			public void ChangeBalanceButtonColor()
			{
				if (!_isRequestChangeBalanceButtonColor)
				{
					return;
				}
				Image image = null;
				if (_isSetGrayBalanceButton)
				{
					if (!_isGrayBalanceButton && !_isGrayBalanceButton)
					{
						image = _buttonController.transform.GetChild(1).GetChild(0).GetChild(0)
							.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
						if (image != null)
						{
							image.color = new Color(0.5019608f, 0.5019608f, 0.5019608f, 1f);
						}
						_isGrayBalanceButton = true;
						_buttonController.ChangeBalanceButtonColor(_isGrayBalanceButton);
					}
				}
				else if (!_isSetGrayBalanceButton)
				{
					if (_isGrayBalanceButton)
					{
						image = _buttonController.transform.GetChild(1).GetChild(0).GetChild(0)
							.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
						if (image != null)
						{
							image.color = new Color(1f, 1f, 1f, 1f);
						}
						_isGrayBalanceButton = false;
						_buttonController.ChangeBalanceButtonColor(_isGrayBalanceButton);
					}
					else
					{
						_ = _isGrayBalanceButton;
					}
				}
				_isRequestChangeBalanceButtonColor = false;
			}

			public override void ViewUpdate()
			{
				if (_timer > 0)
				{
					_timer--;
				}
				if (_balanceCommon._timer > 0)
				{
					_balanceCommon._timer--;
				}
				ChangeOKButtonColor();
				ChangeBackButtonColor();
				ChangeBalanceButtonColor();
				switch (_state)
				{
				case MonitorState.None:
					_state = MonitorState.Init;
					break;
				case MonitorState.Init:
					_isCheckOK = false;
					_isCheckLR = false;
					_buttonController.SetVisible(false, default(int));
					_buttonController.SetVisible(false, 1);
					_buttonController.SetVisible(false, 2);
					_buttonController.SetVisible(false, 3);
					_buttonController.SetVisible(false, 4);
					_state = MonitorState.SelectCardFadeInJudge;
					break;
				case MonitorState.SelectCardFadeInJudge:
					_buttonController.SetVisibleImmediate(false, 5);
					_buttonController.SetVisibleImmediate(false, 6);
					if (!Singleton<UserDataManager>.Instance.GetUserData(_monitorID).Detail.ContentBit.IsFlagOn(ContentBitID.FirstTicketSelect))
					{
						_windowMessageList.Add(WindowMessageID.TicketSelectFirst);
						_isDispInfoWindow = true;
						_info_state = InfoWindowState.None;
						_info_timer = 0u;
						_state = MonitorState.SelectCardFadeInJudgeWait;
					}
					else
					{
						_state = MonitorState.SelectCardFadeInWait;
					}
					break;
				case MonitorState.SelectCardFadeInJudgeWait:
					if (!_isDispInfoWindow)
					{
						_state = MonitorState.SelectCardFadeInWait;
					}
					else if (_isInfoWindowVoice && !_isCallVoice)
					{
						_isCallVoice = true;
						SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000228, _monitorID);
					}
					break;
				case MonitorState.SelectCardFadeInWait:
					if (_timer == 0)
					{
						_timer = 90;
						_state = MonitorState.SelectCardWaitInit;
						if (_isRecoveryTicket)
						{
							_state = MonitorState.RecoveryTicketInit;
							_timer = 120;
						}
					}
					break;
				case MonitorState.RecoveryTicketInit:
					if (_timer == 0)
					{
						_isCallBlur = true;
						_state = MonitorState.RecoveryTicketFadeIn;
					}
					break;
				case MonitorState.RecoveryTicketFadeIn:
					_ticketExceptionWindow.SetActive(value: true);
					_ticketExceptionWindow.transform.GetChild(0).GetComponent<Animator>().Play("In", 0, 0f);
					_ticketExceptionWindow.transform.GetChild(0).GetChild(3).GetChild(0)
						.GetComponent<Animator>()
						.Play("ExclamationMark", 0, 0f);
					_state = MonitorState.RecoveryTicketFadeInWait;
					_timer = 180;
					break;
				case MonitorState.RecoveryTicketFadeInWait:
				{
					Animator animator9 = null;
					if (_timer == 0)
					{
						OKStart();
						_timer = 300;
						_state = MonitorState.RecoveryTicketWait;
						break;
					}
					animator9 = _ticketExceptionWindow.transform.GetChild(0).GetComponent<Animator>();
					if (IsEndAnim(animator9))
					{
						animator9.Play("Loop", 0, 0f);
					}
					break;
				}
				case MonitorState.RecoveryTicketWait:
					if (_timer == 0 || _isOK)
					{
						ResetOKStart();
						_ticketExceptionWindow.transform.GetChild(0).GetComponent<Animator>().Play("Out", 0, 0f);
						_timer = 60;
						_state = MonitorState.RecoveryTicketFadeOut;
					}
					break;
				case MonitorState.RecoveryTicketFadeOut:
					if (_timer == 0)
					{
						Animator animator3 = null;
						animator3 = _ticketExceptionWindow.transform.GetChild(0).GetComponent<Animator>();
						if (IsEndAnim(animator3))
						{
							_ticketExceptionWindow.SetActive(value: false);
							_state = MonitorState.RecoveryTicketFadeOutWait;
						}
					}
					break;
				case MonitorState.RecoveryTicketFadeOutWait:
					_isDecidedOK = true;
					_isDecidedEntry = true;
					_decidedSelectTicketID = _recoveryTicketID;
					_state = MonitorState.DecidedEntryFadeIn;
					break;
				case MonitorState.SelectCardReset:
				{
					int index4 = _dispCardMax / 2;
					bool flag2 = true;
					if (_objTicketSelectListRefID[index4] == _ticketSelectTicketID.Count - 1)
					{
						flag2 = false;
						_isRightButtonVisible = false;
						_isLeftButtonVisible = true;
						_buttonController.SetVisibleFlip(true, true, 4);
					}
					else if (_objTicketSelectListRefID[index4] == 0)
					{
						flag2 = false;
						_isRightButtonVisible = true;
						_isLeftButtonVisible = false;
						_buttonController.SetVisible(true, 3);
					}
					if (flag2)
					{
						_isRightButtonVisible = true;
						_isLeftButtonVisible = true;
						_buttonController.SetVisible(true, 3);
						_buttonController.SetVisibleFlip(true, true, 4);
					}
					_isRightButtonVisibleRequest = false;
					_isLeftButtonVisibleRequest = false;
					_isCheckLR = true;
					_buttonController.ChangeButtonType(ButtonControllerBase.FlatButtonType.Ok);
					OKStart();
					_state = MonitorState.SelectCardWait;
					break;
				}
				case MonitorState.SelectCardWaitInit:
					if (_timer == 0)
					{
						if (_ticketSelectTicketID.Count - 1 - _ticketDefaultRefID == 0)
						{
							_buttonController.SetVisible(false, 3);
						}
						else
						{
							_buttonController.SetVisible(true, 3);
							_isRightButtonVisible = true;
						}
						if (_ticketDefaultRefID == 0)
						{
							_buttonController.SetVisible(false, 4);
						}
						else
						{
							_buttonController.SetVisibleFlip(true, true, 4);
							_isLeftButtonVisible = true;
						}
						_isRightButtonVisibleRequest = false;
						_isLeftButtonVisibleRequest = false;
						_buttonController.SetVisible(false, 2);
						_isCheckLR = true;
						_buttonController.ChangeButtonType(ButtonControllerBase.FlatButtonType.Ok);
						OKStart();
						_state = MonitorState.SelectCardWait;
					}
					break;
				case MonitorState.SelectCardWait:
					if (_isOK || _isTimeUp)
					{
						_state = MonitorState.CreditJudgeInit;
						if (_isOK)
						{
							int index = _dispCardMax / 2;
							int num = _objTicketSelectListRefID[index];
							bool flag = false;
							if (num >= 0)
							{
								flag = _ticketSelectEnable[num];
							}
							_isDecidedOK = true;
							if (flag)
							{
								_decidedSelectTicketID = 0;
								TicketSelectData ticketSelectData = null;
								if (num != -1 && _ticketDataList != null && num >= 0 && num <= _ticketDataList.Count - 1)
								{
									ticketSelectData = _ticketDataList[num];
									_decidedSelectTicketID = ticketSelectData.ticketID;
								}
								_isCallBlur = true;
							}
							_state = MonitorState.CreditJudgeInit;
							if (IsSkipInsertCreditInfo())
							{
								_isDecidedOK = true;
								_isDecidedEntry = true;
								_state = MonitorState.DecideCreditFadeOut;
							}
						}
						else if (_isTimeUp)
						{
							_isDecidedOK = true;
							_isDecidedEntry = true;
							_decidedSelectTicketID = 0;
							_isCallBlur = true;
							_state = MonitorState.DecideCreditFadeOut;
						}
						_buttonController.SetVisible(false, 3);
						_buttonController.SetVisible(false, 4);
						_isCheckLR = false;
						_isRight = false;
						_isLeft = false;
						_isCheckOK = false;
						_timer = 60;
						ResetOKStart();
						break;
					}
					if (_isRight)
					{
						for (int i = 0; i < _dispCardMax; i++)
						{
							CommonValue commonValue = _valueListMiniCardPosX[i];
							CommonValue commonValue2 = _valueListMiniCardScaleXY[i];
							commonValue.start = _cardMiniPosX[i];
							commonValue.current = _cardMiniPosX[i];
							commonValue2.start = _cardMiniScaleXY[i];
							commonValue2.current = _cardMiniScaleXY[i];
							if (i == 0)
							{
								commonValue.end = _cardMiniPosX[i];
								commonValue2.end = _cardMiniScaleXY[i];
							}
							else
							{
								commonValue.end = _cardMiniPosX[i - 1];
								commonValue2.end = _cardMiniScaleXY[i - 1];
							}
							commonValue.diff = (commonValue.end - commonValue.start) / _cardMoveTime;
							commonValue2.diff = (commonValue2.end - commonValue2.start) / _cardMoveTime;
							_cardPosEnd = 0;
						}
						int index2 = _dispCardMax / 2;
						if (!_isRightButtonVisibleRequest && _objTicketSelectListRefID[index2] == _ticketSelectTicketID.Count - 1 - 1)
						{
							_isRightButtonVisibleRequest = true;
							_sub_count = 0;
						}
						if (!_isLeftButtonVisibleRequest && !_isLeftButtonVisible)
						{
							_isLeftButtonVisibleRequest = true;
							_sub_count = 0;
						}
						_state = MonitorState.SelectCardMoveRight;
					}
					else if (_isLeft)
					{
						for (int j = 0; j < _dispCardMax; j++)
						{
							CommonValue commonValue3 = _valueListMiniCardPosX[j];
							CommonValue commonValue4 = _valueListMiniCardScaleXY[j];
							commonValue3.start = _cardMiniPosX[j];
							commonValue3.current = _cardMiniPosX[j];
							commonValue4.start = _cardMiniScaleXY[j];
							commonValue4.current = _cardMiniScaleXY[j];
							if (j == _dispCardMax - 1)
							{
								commonValue3.end = _cardMiniPosX[j];
								commonValue4.end = _cardMiniScaleXY[j];
							}
							else
							{
								commonValue3.end = _cardMiniPosX[j + 1];
								commonValue4.end = _cardMiniScaleXY[j + 1];
							}
							commonValue3.diff = (commonValue3.end - commonValue3.start) / _cardMoveTime;
							commonValue4.diff = (commonValue4.end - commonValue4.start) / _cardMoveTime;
							_cardPosEnd = 0;
						}
						int index3 = _dispCardMax / 2;
						if (!_isLeftButtonVisibleRequest && _objTicketSelectListRefID[index3] == 1)
						{
							_isLeftButtonVisibleRequest = true;
							_sub_count = 0;
						}
						if (!_isRightButtonVisibleRequest && !_isRightButtonVisible)
						{
							_isRightButtonVisibleRequest = true;
							_sub_count = 0;
						}
						_state = MonitorState.SelectCardMoveLeft;
					}
					if (_isRight || _isLeft)
					{
						int bigSelector = 2;
						SetBigSelector(bigSelector);
					}
					break;
				case MonitorState.SelectCardMoveRight:
				case MonitorState.SelectCardMoveLeft:
				{
					_sub_count++;
					if ((float)_sub_count >= _cardMoveTime - 5f)
					{
						if (_isRightButtonVisibleRequest)
						{
							if (_isRightButtonVisible)
							{
								_isRightButtonVisibleRequest = false;
								_isRightButtonVisible = false;
								_buttonController.SetVisible(false, 3);
							}
							else
							{
								_isRightButtonVisibleRequest = false;
								_isRightButtonVisible = true;
								_buttonController.SetVisible(true, 3);
							}
						}
						else if (_isLeftButtonVisibleRequest)
						{
							if (_isLeftButtonVisible)
							{
								_isLeftButtonVisibleRequest = false;
								_isLeftButtonVisible = false;
								_buttonController.SetVisible(false, 4);
							}
							else
							{
								_isLeftButtonVisibleRequest = false;
								_isLeftButtonVisible = true;
								_buttonController.SetVisibleFlip(true, true, 4);
							}
						}
					}
					_cardPosEnd = 0;
					for (int k = 0; k < _dispCardMax; k++)
					{
						CommonValue commonValue5 = _valueListMiniCardPosX[k];
						CommonValue commonValue6 = _valueListMiniCardScaleXY[k];
						if (commonValue5.UpdateValue())
						{
							_cardPosEnd++;
						}
						commonValue6.UpdateValue();
						_nullListMiniCard[k].transform.localPosition = new Vector3(commonValue5.current, 0f, 0f);
						_nullListMiniCard[k].transform.localScale = new Vector3(commonValue6.current, commonValue6.current, 1f);
					}
					if (_cardPosEnd != _dispCardMax)
					{
						break;
					}
					for (int l = 0; l < _dispCardMax; l++)
					{
						_nullListMiniCard[l].transform.localPosition = new Vector3(_cardMiniPosX[l], 0f, 0f);
						_nullListMiniCard[l].transform.localScale = new Vector3(_cardMiniScaleXY[l], _cardMiniScaleXY[l], 1f);
					}
					if (_isRight)
					{
						int num4 = _dispCardMax - 1;
						int num5 = _objTicketSelectListRefID[num4];
						int num6 = -1;
						for (int m = 0; m < num4; m++)
						{
							num6 = _objTicketSelectListRefID[m + 1];
							if (num6 == -1)
							{
								_objTicketSelectListRefID[m] = -1;
								_nullListMiniCard[m].SetActive(value: false);
							}
							else
							{
								_objTicketSelectListRefID[m] = _objTicketSelectListRefID[m + 1];
								_nullListMiniCard[m].SetActive(value: true);
							}
						}
						if (num5 == -1)
						{
							_objTicketSelectListRefID[num4] = -1;
						}
						else
						{
							num5++;
							if (num5 > _ticketSelectTicketID.Count - 1)
							{
								_objTicketSelectListRefID[num4] = -1;
								_nullListMiniCard[num4].SetActive(value: false);
							}
							else
							{
								_objTicketSelectListRefID[num4] = num5;
								_nullListMiniCard[num4].SetActive(value: true);
							}
						}
						GameObject gameObject9 = null;
						int num7 = -1;
						TicketSelectData ticketSelectData2 = null;
						for (int n = 0; n < _dispCardMax; n++)
						{
							ticketSelectData2 = null;
							num6 = _objTicketSelectListRefID[n];
							if (num6 != -1)
							{
								if (_ticketDataList != null && num6 >= 0 && num6 <= _ticketDataList.Count - 1)
								{
									ticketSelectData2 = _ticketDataList[num6];
								}
								gameObject9 = _nullListMiniCard[n].transform.GetChild(0).gameObject;
								SetActiveChildren(gameObject9, active: false);
								num7 = GetMiniSelectorChildID(ticketSelectData2);
								if (num7 != -1)
								{
									gameObject9.transform.GetChild(num7).gameObject.SetActive(value: true);
								}
							}
						}
						SetMiniSelector();
					}
					else if (_isLeft)
					{
						int num8 = 1;
						int dispCardMax = _dispCardMax;
						int num9 = _objTicketSelectListRefID[num8 - 1];
						int num10 = -1;
						int num11 = _dispCardMax - 1;
						for (int num12 = num8; num12 < dispCardMax; num12++)
						{
							num10 = _objTicketSelectListRefID[num11 - num12];
							if (num10 == -1)
							{
								_objTicketSelectListRefID[num11 - (num12 - 1)] = -1;
								_nullListMiniCard[num11 - (num12 - 1)].SetActive(value: false);
							}
							else
							{
								_objTicketSelectListRefID[num11 - (num12 - 1)] = _objTicketSelectListRefID[num11 - num12];
								_nullListMiniCard[num11 - (num12 - 1)].SetActive(value: true);
							}
						}
						num9--;
						if (num9 < 0)
						{
							_objTicketSelectListRefID[num8 - 1] = -1;
							_nullListMiniCard[num8 - 1].SetActive(value: false);
						}
						else
						{
							_objTicketSelectListRefID[num8 - 1] = num9;
							_nullListMiniCard[num8 - 1].SetActive(value: true);
						}
						GameObject gameObject10 = null;
						int num13 = -1;
						TicketSelectData ticketSelectData3 = null;
						for (int num14 = 0; num14 < _dispCardMax; num14++)
						{
							ticketSelectData3 = null;
							num10 = _objTicketSelectListRefID[num14];
							if (num10 != -1)
							{
								if (_ticketDataList != null && num10 >= 0 && num10 <= _ticketDataList.Count - 1)
								{
									ticketSelectData3 = _ticketDataList[num10];
								}
								gameObject10 = _nullListMiniCard[num14].transform.GetChild(0).gameObject;
								SetActiveChildren(gameObject10, active: false);
								num13 = GetMiniSelectorChildID(ticketSelectData3);
								if (num13 != -1)
								{
									gameObject10.transform.GetChild(num13).gameObject.SetActive(value: true);
								}
							}
						}
						SetMiniSelector();
					}
					int bigSelector2 = 1;
					SetBigSelector(bigSelector2);
					_sub_count = 0;
					_state = MonitorState.SelectCardWait;
					_isRight = false;
					_isLeft = false;
					break;
				}
				case MonitorState.CreditJudgeInit:
					if (_timer == 0)
					{
						_buttonController.SetVisible(false, default(int));
						_buttonController.SetVisible(false, 2);
						_isOK = false;
						_isBack = false;
						_state = MonitorState.CreditJudge;
					}
					break;
				case MonitorState.CreditJudge:
					_isOK = false;
					_isBack = false;
					_isCallBlur = true;
					_timer = 60;
					if (_needCreditNum > 0 || _needCreditNum == -1)
					{
						_state = MonitorState.InsertCoinFadeIn;
						break;
					}
					_state = MonitorState.DecideCreditFadeIn;
					if (IsSkipInsertCreditInfo())
					{
						_isDecidedOK = true;
						_isDecidedEntry = true;
						_state = MonitorState.DecideCreditFadeOut;
						if (_decidedSelectTicketID == 0)
						{
							_state = MonitorState.DecidedEntryWait02;
						}
					}
					break;
				case MonitorState.EMoneyWait:
					if (_balanceCommon.IsEnd())
					{
						_state = MonitorState.EMoneyExit;
					}
					else
					{
						_balanceCommon.ViewUpdate();
					}
					break;
				case MonitorState.EMoneyExit:
					_button_mode = MonitorButtonMode.MonitorButtonModeCommon;
					_state = MonitorState.CreditJudgeReset;
					break;
				case MonitorState.InsertCoinFadeIn:
				{
					GameObject gameObject8 = null;
					_creditWindow.SetActive(value: true);
					_emoneyWindow.SetActive(value: false);
					gameObject8 = _creditWindow.transform.GetChild(0).GetChild(1).gameObject;
					gameObject8.SetActive(value: true);
					gameObject8 = _creditWindow.transform.GetChild(0).GetChild(2).gameObject;
					gameObject8.SetActive(value: false);
					_creditWindow.transform.GetChild(0).GetComponent<Animator>().Play("InsertCoin_In", 0, 0f);
					gameObject8 = _creditWindow.transform.GetChild(0).GetChild(1).gameObject;
					gameObject8.transform.GetChild(0).gameObject.GetComponent<Animator>().Play("Loop", 0, 0f);
					SetNeedCreditNum();
					gameObject8.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Animator>().Play("In", 0, 0f);
					gameObject8.transform.GetChild(3).gameObject.GetComponent<Animator>().Play("Out", 0, 1f);
					gameObject8.transform.GetChild(4).gameObject.GetComponent<Animator>().Play("Out", 0, 1f);
					if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsEnableEmoneyExecute)
					{
						if (GotoBalance() && AMDaemon.EMoney.IsAuthCompleted)
						{
							gameObject8.transform.GetChild(5).gameObject.SetActive(value: true);
							gameObject8.transform.GetChild(5).GetChild(1).gameObject.GetComponent<MultipleImage>().ChangeSprite(_monitorID);
							_isBalanceAuthCompleted = true;
						}
						else
						{
							gameObject8.transform.GetChild(5).gameObject.SetActive(value: false);
							gameObject8.transform.GetChild(5).GetChild(1).gameObject.GetComponent<MultipleImage>().ChangeSprite(_monitorID);
							_isBalanceAuthCompleted = false;
						}
					}
					else
					{
						gameObject8.transform.GetChild(5).gameObject.SetActive(value: false);
						gameObject8.transform.GetChild(5).GetChild(1).gameObject.GetComponent<MultipleImage>().ChangeSprite(_monitorID);
						_isBalanceAuthCompleted = false;
					}
					_state = MonitorState.InsertCoinFadeInWait;
					_timer = 2;
					_sub_count = 0;
					break;
				}
				case MonitorState.InsertCoinFadeInWait:
				{
					Animator animator7 = null;
					animator7 = _creditWindow.transform.GetChild(0).GetComponent<Animator>();
					if (IsEndAnim(animator7))
					{
						animator7.Play("InsertCoin_Loop", 0, 0f);
						GameObject obj3 = _creditWindow.transform.GetChild(0).GetChild(1).gameObject;
						animator7 = obj3.transform.GetChild(0).gameObject.GetComponent<Animator>();
						if (IsEndAnim(animator7))
						{
							animator7.Play("Loop", 0, 0f);
						}
						animator7 = obj3.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Animator>();
						animator7.Play("Coin", 0, 0f);
						BackStart();
						_state = MonitorState.InsertCoinWait;
					}
					else if (_timer == 0 && _sub_count == 0)
					{
						GameObject obj4 = _creditWindow.transform.GetChild(0).GetChild(1).gameObject;
						obj4.transform.GetChild(3).gameObject.GetComponent<Animator>().Play("Out", 0, 1f);
						obj4.transform.GetChild(4).gameObject.GetComponent<Animator>().Play("Out", 0, 1f);
						_sub_count++;
					}
					break;
				}
				case MonitorState.InsertCoinWait:
				{
					Animator animator = null;
					GameObject gameObject = null;
					animator = _creditWindow.transform.GetChild(0).GetComponent<Animator>();
					if (IsEndAnim(animator))
					{
						animator.Play("InsertCoin_Loop", 0, 0f);
					}
					gameObject = _creditWindow.transform.GetChild(0).GetChild(1).gameObject;
					animator = gameObject.transform.GetChild(0).gameObject.GetComponent<Animator>();
					if (IsEndAnim(animator))
					{
						animator.Play("Loop", 0, 0f);
					}
					animator = gameObject.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Animator>();
					if (IsEndAnim(animator))
					{
						animator.Play("Coin", 0, 0f);
					}
					if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsEnableEmoneyExecute)
					{
						if (AMDaemon.EMoney.IsAuthCompleted)
						{
							if (!_isBalanceAuthCompleted)
							{
								gameObject.transform.GetChild(5).gameObject.SetActive(value: true);
								_isBalanceAuthCompleted = true;
							}
							if (_isBalanceNetworkDisconnection)
							{
								if (!_balanceCommon._isOwnBalanceError01)
								{
									animator = gameObject.transform.GetChild(3).gameObject.GetComponent<Animator>();
									animator.Play("In", 0, 0f);
									gameObject.transform.GetChild(5).gameObject.SetActive(value: false);
									_balanceCommon._isOwnBalanceError01 = true;
								}
							}
							else if (!_isBalanceNetworkDisconnection)
							{
								_balanceCommon.ResetError01();
							}
							if (_isOwnBalanceBlocking)
							{
								if (!_balanceCommon._isOwnBalanceError02)
								{
									animator = gameObject.transform.GetChild(4).gameObject.GetComponent<Animator>();
									animator.Play("In", 0, 0f);
									gameObject.transform.GetChild(5).gameObject.SetActive(value: false);
									_balanceCommon._isOwnBalanceError02 = true;
								}
							}
							else if (!_isOwnBalanceBlocking)
							{
								_balanceCommon.ResetError02();
							}
						}
						else if (!AMDaemon.EMoney.IsAuthCompleted && _isBalanceAuthCompleted)
						{
							_balanceCommon.ResetError();
							gameObject.transform.GetChild(5).gameObject.SetActive(value: false);
							gameObject.transform.GetChild(3).gameObject.GetComponent<Animator>().Play("Out", 0, 1f);
							gameObject.transform.GetChild(4).gameObject.GetComponent<Animator>().Play("Out", 0, 1f);
							_isBalanceAuthCompleted = false;
						}
					}
					if (_isTimeUp)
					{
						_isDecidedOK = true;
						_isDecidedEntry = true;
						_decidedSelectTicketID = 0;
						_isCallBlur = true;
						ResetBackStart();
						_isBack = true;
						_state = MonitorState.DecideCreditFadeOut;
					}
					else if ((!_isNeedCreditFraction && _needCreditNum == 0) || (_isNeedCreditFraction && _needCreditNum == 0 && _needCreditNumerator == 0))
					{
						_state = MonitorState.InsertCoinFadeOut;
					}
					else if (_isBack)
					{
						_isTimeUp = false;
						_isDecidedOK = false;
						_decidedSelectTicketID = -1;
						_isCallBlur = false;
						ResetBackStart();
						_isBack = true;
						_state = MonitorState.DecideCreditFadeOut;
					}
					else if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsEnableEmoneyExecute && _isSelectButton)
					{
						_isSelectButton = false;
						ResetBackStart();
						animator = _creditWindow.transform.GetChild(0).GetComponent<Animator>();
						animator.Play("InsertCoin_Out", 0, 0f);
						_button_mode = MonitorButtonMode.MonitorButtonModeEMoney;
						_state = MonitorState.EMoneyWait;
						_balanceCommon._state = BalanceCommon.BalanceCommonState.EMoney01Init;
					}
					break;
				}
				case MonitorState.InsertCoinFadeOut:
					_creditWindow.transform.GetChild(0).GetComponent<Animator>().Play("InsertCoin_Out", 0, 0f);
					_state = MonitorState.InsertCoinFadeOutWait;
					_timer = 30;
					break;
				case MonitorState.InsertCoinFadeOutWait:
				{
					Animator component3 = _creditWindow.transform.GetChild(0).GetComponent<Animator>();
					if (_timer != 0 && !IsEndAnim(component3))
					{
						break;
					}
					_state = MonitorState.DecideCreditFadeIn;
					if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsEnableEmoneyExecute)
					{
						_balanceCommon.ResetError();
					}
					if (_isBack)
					{
						_isBack = false;
						_state = MonitorState.SelectCardReset;
						if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsEnableEmoneyExecute)
						{
							_balanceCommon.ResetError();
						}
						_creditWindow.SetActive(value: false);
					}
					else if (IsSkipInsertCreditInfo())
					{
						_isDecidedOK = true;
						_isDecidedEntry = true;
						_state = MonitorState.DecideCreditFadeOut;
						if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsEnableEmoneyExecute)
						{
							_balanceCommon.ResetError();
						}
					}
					break;
				}
				case MonitorState.DecideCreditFadeIn:
				{
					if (_isAnotherBlocking && !_isRecallAnoterBlur)
					{
						_isRecallAnoterBlur = true;
					}
					GameObject gameObject6 = null;
					GameObject gameObject7 = null;
					_creditWindow.SetActive(value: true);
					_emoneyWindow.SetActive(value: false);
					gameObject6 = _creditWindow.transform.GetChild(0).GetChild(1).gameObject;
					gameObject6.SetActive(value: false);
					gameObject6 = _creditWindow.transform.GetChild(0).GetChild(2).gameObject;
					gameObject6.SetActive(value: true);
					_creditWindow.transform.GetChild(0).GetComponent<Animator>().Play("Decide_In", 0, 0f);
					gameObject6 = _creditWindow.transform.GetChild(0).GetChild(2).gameObject;
					gameObject6.transform.GetChild(0).gameObject.GetComponent<Animator>().Play("Loop", 0, 0f);
					switch (_monitorID)
					{
					case 0:
						gameObject7 = gameObject6.transform.GetChild(3).gameObject;
						gameObject6.transform.GetChild(4).gameObject.SetActive(value: false);
						break;
					case 1:
						gameObject7 = gameObject6.transform.GetChild(4).gameObject;
						gameObject6.transform.GetChild(3).gameObject.SetActive(value: false);
						break;
					default:
						gameObject7 = gameObject6.transform.GetChild(3).gameObject;
						gameObject6.transform.GetChild(4).gameObject.SetActive(value: false);
						break;
					}
					gameObject7.GetComponent<Animator>().Play("Loop", 0, 0f);
					gameObject6.transform.GetChild(5).gameObject.SetActive(value: false);
					_state = MonitorState.DecideCreditFadeInWait;
					break;
				}
				case MonitorState.DecideCreditFadeInWait:
				{
					Animator animator4 = null;
					GameObject gameObject2 = null;
					GameObject gameObject3 = null;
					animator4 = _creditWindow.transform.GetChild(0).GetComponent<Animator>();
					if (IsEndAnim(animator4))
					{
						animator4.Play("Decide_Loop", 0, 0f);
						gameObject2 = _creditWindow.transform.GetChild(0).GetChild(2).gameObject;
						animator4 = gameObject2.transform.GetChild(0).gameObject.GetComponent<Animator>();
						if (IsEndAnim(animator4))
						{
							animator4.Play("Loop", 0, 0f);
						}
						switch (_monitorID)
						{
						case 0:
							gameObject3 = gameObject2.transform.GetChild(3).gameObject;
							gameObject2.transform.GetChild(4).gameObject.SetActive(value: false);
							break;
						case 1:
							gameObject3 = gameObject2.transform.GetChild(4).gameObject;
							gameObject2.transform.GetChild(3).gameObject.SetActive(value: false);
							break;
						}
						animator4 = gameObject3.GetComponent<Animator>();
						if (IsEndAnim(animator4))
						{
							animator4.Play("Loop", 0, 0f);
						}
						_buttonController.ChangeButtonType(ButtonControllerBase.FlatButtonType.Decide);
						OKStart();
						BackStart();
						_state = MonitorState.DecideCreditWait;
					}
					break;
				}
				case MonitorState.DecideCreditWait:
				{
					Animator animator6 = null;
					GameObject gameObject4 = null;
					GameObject gameObject5 = null;
					animator6 = _creditWindow.transform.GetChild(0).GetComponent<Animator>();
					if (IsEndAnim(animator6))
					{
						animator6.Play("Decide_Loop", 0, 0f);
					}
					gameObject4 = _creditWindow.transform.GetChild(0).GetChild(2).gameObject;
					animator6 = gameObject4.transform.GetChild(0).gameObject.GetComponent<Animator>();
					if (IsEndAnim(animator6))
					{
						animator6.Play("Loop", 0, 0f);
					}
					switch (_monitorID)
					{
					case 0:
						gameObject5 = gameObject4.transform.GetChild(3).gameObject;
						gameObject4.transform.GetChild(4).gameObject.SetActive(value: false);
						break;
					case 1:
						gameObject5 = gameObject4.transform.GetChild(4).gameObject;
						gameObject4.transform.GetChild(3).gameObject.SetActive(value: false);
						break;
					}
					animator6 = gameObject5.GetComponent<Animator>();
					if (IsEndAnim(animator6))
					{
						animator6.Play("Loop", 0, 0f);
					}
					if (_isAnotherBlocking)
					{
						ResetOKStart();
						ResetBackStart();
						_state = MonitorState.DecideCreditAnotherBlocking;
					}
					else if (_isOK)
					{
						ResetOKStart();
						ResetBackStart();
						_state = MonitorState.DecideCreditRequestAllow;
						_isServerRequest = true;
						_isServerAccepted = false;
					}
					else if (_isTimeUp)
					{
						_isDecidedOK = true;
						_isDecidedEntry = true;
						_decidedSelectTicketID = 0;
						_isCallBlur = true;
						ResetOKStart();
						ResetBackStart();
						_state = MonitorState.DecideCreditFadeOut;
					}
					else if (_isBack)
					{
						_isTimeUp = false;
						_isDecidedOK = false;
						_decidedSelectTicketID = -1;
						_isCallBlur = false;
						ResetBackStart();
						_isBack = true;
						_state = MonitorState.DecideCreditFadeOut;
					}
					else if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsEnableEmoneyExecute && _isSelectButton)
					{
						_isSelectButton = false;
						ResetOKStart();
						ResetBackStart();
						animator6 = _creditWindow.transform.GetChild(0).GetComponent<Animator>();
						animator6.Play("Decide_Out", 0, 0f);
						_button_mode = MonitorButtonMode.MonitorButtonModeEMoney;
						_state = MonitorState.EMoneyWait;
						_balanceCommon._state = BalanceCommon.BalanceCommonState.EMoney01Init;
					}
					break;
				}
				case MonitorState.DecideCreditAnotherBlocking:
					_state = MonitorState.DecideCreditAnotherBlockingWait;
					break;
				case MonitorState.DecideCreditAnotherBlockingWait:
				{
					Animator component = _creditWindow.transform.GetChild(0).GetComponent<Animator>();
					if (IsEndAnim(component) && _isTimeUp)
					{
						_isDecidedOK = true;
						_isDecidedEntry = true;
						_decidedSelectTicketID = 0;
					}
					break;
				}
				case MonitorState.DecideCreditRequestAllow:
				{
					if (_decidedSelectTicketID <= 0)
					{
						_isDecidedEntry = true;
						_state = MonitorState.DecideCreditFadeOut;
						break;
					}
					bool num2 = Singleton<TicketManager>.Instance.UseTicketPrepare(_monitorID, _decidedSelectTicketID);
					_isCallTicketPrepare = true;
					if (num2)
					{
						_state = MonitorState.DecideCreditRequestAllowWait;
						_sub_count = 0;
					}
					else
					{
						_state = MonitorState.DecideCreditFadeOut;
					}
					break;
				}
				case MonitorState.DecideCreditRequestAllowWait:
				{
					_sub_count++;
					int num15 = 0;
					bool isError = false;
					bool flag3 = false;
					num15 = GetSelectorCreditNum(_decidedSelectTicketID);
					flag3 = Singleton<TicketManager>.Instance.IsFinishServerTicket(_monitorID, ref num15, ref isError);
					Animator component4 = _creditWindow.transform.GetChild(0).GetComponent<Animator>();
					if ((IsEndAnim(component4) || _sub_count > 5) && flag3)
					{
						if (isError)
						{
							_isDecidedOK = true;
							_isDecidedEntry = true;
							_decidedSelectTicketID = 0;
							_isServerAccepted = true;
							_state = MonitorState.DecideCreditFadeOut;
							_isServerRejected = true;
							_isCallRejectedInfo = true;
						}
						else
						{
							_isDecidedEntry = true;
							_isServerAccepted = true;
							_state = MonitorState.DecideCreditFadeOut;
						}
					}
					break;
				}
				case MonitorState.DecideCreditFadeOut:
					if (!_isCallTicketPrepare && IsSkipInsertCreditInfo() && _isDecidedOK && _isDecidedEntry && _decidedSelectTicketID > 0)
					{
						Singleton<TicketManager>.Instance.UseTicketPrepare(_monitorID, _decidedSelectTicketID);
						_isCallTicketPrepare = true;
					}
					_creditWindow.transform.GetChild(0).GetComponent<Animator>().Play("Decide_Out", 0, 1f);
					_state = MonitorState.DecideCreditFadeOutWait;
					_timer = 30;
					break;
				case MonitorState.DecideCreditFadeOutWait:
				{
					if (_isTimeUp)
					{
						_creditWindow.SetActive(value: false);
						_state = MonitorState.DecidedEntryFadeIn;
						if (IsSkipInsertCreditInfo() && _decidedSelectTicketID == 0)
						{
							_state = MonitorState.DecidedEntryWait02;
							if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsEnableEmoneyExecute)
							{
								_balanceCommon.ResetError();
							}
						}
						break;
					}
					Animator component2 = _creditWindow.transform.GetChild(0).GetComponent<Animator>();
					if (_timer != 0 && !IsEndAnim(component2))
					{
						break;
					}
					_state = MonitorState.DecidedEntryFadeIn;
					if (_isBack)
					{
						_isBack = false;
						_creditWindow.SetActive(value: false);
						_state = MonitorState.SelectCardReset;
						if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsEnableEmoneyExecute)
						{
							_balanceCommon.ResetError();
						}
					}
					else if (IsSkipInsertCreditInfo() && _decidedSelectTicketID == 0)
					{
						_state = MonitorState.DecidedEntryWait02;
						if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsEnableEmoneyExecute)
						{
							_balanceCommon.ResetError();
						}
					}
					break;
				}
				case MonitorState.CreditJudgeReset:
					ResetOKStart();
					_isResetAnotherBalanceBlocking = true;
					_state = MonitorState.CreditJudge;
					break;
				case MonitorState.DecidedEntryFadeIn:
				{
					_sub_count = 0;
					_isTicketSE = false;
					Animator animator8 = null;
					MultipleImage multipleImage = null;
					int num3 = 0;
					_creditWindow.SetActive(value: false);
					_emoneyWindow.SetActive(value: false);
					_ticketInfoWindow.SetActive(value: true);
					GameObject obj5 = _ticketInfoWindow.transform.GetChild(0).gameObject;
					obj5.SetActive(value: true);
					animator8 = obj5.GetComponent<Animator>();
					if (_isEntryNum == 1)
					{
						if (_decidedSelectTicketID == 0)
						{
							animator8.Play("NotUsedTicket_In", 0, 0f);
						}
						else
						{
							animator8.Play("UsedTicket", 0, 0f);
						}
					}
					else if (_decidedSelectTicketID == 0)
					{
						animator8.Play("NotUsedTicket_In", 0, 0f);
					}
					else
					{
						animator8.Play("Waiting_In", 0, 0f);
					}
					animator8 = _ticketInfoWindow.transform.GetChild(0).GetChild(0).GetChild(1)
						.gameObject.GetComponent<Animator>();
					animator8.Play("Loop", 0, 0f);
					multipleImage = _ticketInfoWindow.transform.GetChild(0).GetChild(0).GetChild(2)
						.gameObject.transform.GetChild(1).gameObject.GetComponent<MultipleImage>();
					if (multipleImage != null)
					{
						num3 = 0;
						if (_decidedSelectTicketID == 0 || _decidedSelectTicketID < 0)
						{
							num3 = 2;
						}
						if (_isRecoveryTicket)
						{
							num3 = 1;
						}
						multipleImage.ChangeSprite(num3);
					}
					GameObject obj6 = _ticketInfoWindow.transform.GetChild(0).GetChild(2).gameObject;
					multipleImage = obj6.transform.GetChild(0).GetChild(0).gameObject.GetComponent<MultipleImage>();
					if (multipleImage != null)
					{
						num3 = GetBigSelectorTexID(_decidedSelectTicketID);
						multipleImage.ChangeSprite(num3);
					}
					multipleImage = obj6.transform.GetChild(1).GetChild(0).gameObject.GetComponent<MultipleImage>();
					if (multipleImage != null)
					{
						num3 = GetBigSelectorTexID(_decidedSelectTicketID);
						multipleImage.ChangeSprite(num3);
					}
					_state = MonitorState.DecidedEntryFadeInWait;
					break;
				}
				case MonitorState.DecidedEntryFadeInWait:
				{
					_sub_count++;
					if (_sub_count > 15 && !_isTicketSE && _decidedSelectTicketID >= 0 && _decidedSelectTicketID != 0)
					{
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_TICKET_01, _monitorID);
						_isTicketSE = true;
					}
					Animator animator5 = null;
					GameObject obj2 = _ticketInfoWindow.transform.GetChild(0).gameObject;
					obj2.SetActive(value: true);
					animator5 = obj2.GetComponent<Animator>();
					if (_isEntryNum == 1)
					{
						if (IsEndAnim(animator5))
						{
							_state = MonitorState.DecidedEntryFadeOut;
						}
					}
					else if (IsEndAnim(animator5))
					{
						if (_decidedSelectTicketID == 0)
						{
							animator5.Play("NotUsedTicket_Loop", 0, 0f);
						}
						else
						{
							animator5.Play("Waiting_Loop", 0, 0f);
						}
						_state = MonitorState.DecidedEntryWait01;
					}
					animator5 = _ticketInfoWindow.transform.GetChild(0).GetChild(0).GetChild(1)
						.gameObject.GetComponent<Animator>();
					if (IsEndAnim(animator5))
					{
						animator5.Play("Loop", 0, 0f);
					}
					break;
				}
				case MonitorState.DecidedEntryWait01:
				{
					Animator animator2 = null;
					GameObject obj = _ticketInfoWindow.transform.GetChild(0).gameObject;
					obj.SetActive(value: true);
					animator2 = obj.GetComponent<Animator>();
					if (_isEntryNum == 1)
					{
						if (IsEndAnim(animator2))
						{
							_state = MonitorState.DecidedEntryFadeOut;
						}
					}
					else if (IsEndAnim(animator2))
					{
						if (_decidedSelectTicketID == 0)
						{
							animator2.Play("NotUsedTicket_Loop", 0, 0f);
						}
						else
						{
							animator2.Play("Waiting_Loop", 0, 0f);
						}
					}
					animator2 = _ticketInfoWindow.transform.GetChild(0).GetChild(0).GetChild(1)
						.gameObject.GetComponent<Animator>();
					if (IsEndAnim(animator2))
					{
						animator2.Play("Loop", 0, 0f);
					}
					if (_isDecidedEntryNum >= _isEntryNum)
					{
						_state = MonitorState.DecidedEntryFadeOut;
					}
					break;
				}
				case MonitorState.DecidedEntryWait02:
					if (_isDecidedEntryNum >= _isEntryNum)
					{
						_state = MonitorState.DecidedEntryFadeOut;
					}
					break;
				case MonitorState.DecidedEntryFadeOut:
					Singleton<MapMaster>.Instance.TicketID[_monitorID] = _decidedSelectTicketID;
					_state = MonitorState.DecidedEntryFadeOutWait;
					_timer = 30;
					if (_isServerRejected)
					{
						_timer = 300;
					}
					break;
				case MonitorState.DecidedEntryFadeOutWait:
					if (_timer == 0)
					{
						_state = MonitorState.Finish;
					}
					break;
				case MonitorState.Finish:
					if (_timer == 0)
					{
						_state = MonitorState.End;
					}
					break;
				}
				ChangeOKButtonColor();
				ChangeBackButtonColor();
				ChangeBalanceButtonColor();
			}

			public bool IsEnd()
			{
				return _state == MonitorState.End;
			}
		}
	}
