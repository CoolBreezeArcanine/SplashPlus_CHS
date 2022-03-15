using System.Collections.Generic;
using AMDaemon;
using MAI2.Util;
using Manager;
using Monitor.ModeSelect;
using Monitor.TicketSelect;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Balance
{
	public class BalanceCommon
	{
		public enum BalanceCommonState
		{
			None,
			EMoney01Init,
			EMoney01CreditBackedFadeIn,
			EMoney01CreditFadeIn,
			EMoney01CreditFadeInWait,
			EMoney01CreditWait,
			EMoney01CreditMoveRight,
			EMoney01CreditMoveLeft,
			EMoney01CreditBackFadeOut,
			EMoney01CreditBackFadeOutWait,
			EMoney01CreditNextFadeOut,
			EMoney01CreditNextFadeOutWait,
			EMoney01CreditBalanceFadeOut,
			EMoney01CreditBalanceFadeOutWait,
			EMoney01BrandFadeIn,
			EMoney01BrandFadeInWait,
			EMoney01BrandWait,
			EMoney01BrandMoveRight,
			EMoney01BrandMoveLeft,
			EMoney01BrandBackFadeOut,
			EMoney01BrandBackFadeOutWait,
			EMoney01BrandNextFadeOut,
			EMoney01BrandNextFadeOutWait,
			EMoney01ReaderFadeIn,
			EMoney01ReaderFadeInWait,
			EMoney01ReaderPreWait,
			EMoney01ReaderWait,
			EMoney01ReaderBackFadeOut,
			EMoney01ReaderBackFadeOutWait,
			EMoney01ReaderBackPreWait,
			EMoney01ReaderBackWait,
			EMoney01ReaderResultFadeIn,
			EMoney01ReaderResultFadeInWait,
			EMoney01ReaderResultPreWait,
			EMoney01ReaderResultWait,
			EMoney01ReaderNextFadeOut,
			EMoney01ReaderNextFadeOutWait,
			EMoney02Init,
			EMoney02BrandFadeIn,
			EMoney02BrandFadeInWait,
			EMoney02BrandWait,
			EMoney02BrandMoveRight,
			EMoney02BrandMoveLeft,
			EMoney02BrandCancelFadeOut,
			EMoney02BrandCancelFadeOutWait,
			EMoney02BrandNextFadeOut,
			EMoney02BrandNextFadeOutWait,
			EMoney02ReaderFadeIn,
			EMoney02ReaderFadeInWait,
			EMoney02ReaderPreWait,
			EMoney02ReaderWait,
			EMoney02ReaderCancelFadeOut,
			EMoney02ReaderCancelFadeOutWait,
			EMoney02ReaderCancelPreWait,
			EMoney02ReaderCancelWait,
			EMoney02ReaderResultFadeIn,
			EMoney02ReaderResultFadeInWait,
			EMoney02ReaderResultPreWait,
			EMoney02ReaderResultWait,
			EMoney02ReaderNextFadeOut,
			EMoney02ReaderNextFadeOutWait,
			Finish,
			End
		}

		public enum ParentMode
		{
			ParentModeModeSelect,
			ParentModeTicketSelect
		}

		public enum GotoBalanceCredit
		{
			GotoBalanceEnableCredit = 14,
			GotoBalanceCoinBlockCredit = 15,
			GotoBalanceMaxCredit = 24
		}

		public BalanceCommonState _state = BalanceCommonState.End;

		public ParentMode _parent_mode;

		public ModeSelectMonitor _monitor_mode_select;

		public TicketSelectMonitor _monitor_ticket_select;

		public GameObject _creditWindow;

		public GameObject _emoneyWindow;

		public ButtonControllerBase _buttonController;

		public int _timer;

		private int _sub_count;

		public int _nEMoneyCreditIconNum = 5;

		public int _nEMoneyCreditIconCursorID;

		public int _nEMoneyCreditRate = 100;

		public int _nEMoneyCreditCoins = 1;

		public bool _isRightLimits;

		public bool _isLeftLimits;

		public int _nEMoneyBrandIconNum = 6;

		public int _nEMoneyBrandIconCursorID;

		private EMoneyBrandId _nEMoneyCurrentBrandTypeID;

		private List<EMoneyBrandId> _valueListEMoneyBrandTypeID = new List<EMoneyBrandId>();

		private List<bool> _valueListEMoneyBrandEnable = new List<bool>();

		public bool _isEMoneyReaderSuccess;

		public bool _isEMoneyReaderFailed;

		public bool _isEMoneyUnconfirm;

		public bool _isHeldOver;

		public bool _isNotCallCancel;

		public bool _isOwnBalanceError01;

		public bool _isOwnBalanceError02;

		public int _maxCreditValue = 500;

		public int _maxBrandTexID = 5;

		public bool _requestInvalid;

		public int _requestMinTime = 5;

		public int _requestWaitTime = 30;

		public int _requestOverTime = 900;

		public bool _isGrayOKButton;

		public BalanceCommon()
		{
			_nEMoneyCreditIconNum = 5;
			_nEMoneyCreditIconCursorID = 0;
			_nEMoneyCreditRate = 100;
			_nEMoneyCreditCoins = 1;
			_isRightLimits = false;
			_isLeftLimits = false;
			_nEMoneyBrandIconNum = 6;
			_nEMoneyBrandIconCursorID = 0;
			_isEMoneyReaderSuccess = false;
			_isEMoneyReaderFailed = false;
			_isOwnBalanceError01 = false;
			_isOwnBalanceError02 = false;
		}

		public void Initialize(GameObject credit, GameObject emoney, ButtonControllerBase button)
		{
			_creditWindow = credit;
			_emoneyWindow = emoney;
			_buttonController = button;
		}

		public void SetMode(ParentMode mode, ModeSelectMonitor mode_select, TicketSelectMonitor ticket_select)
		{
			_parent_mode = mode;
			_monitor_mode_select = mode_select;
			_monitor_ticket_select = ticket_select;
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

		public void CreditMoveRight()
		{
			if (_isLeftLimits)
			{
				_buttonController.SetVisibleImmediate(true, 6);
				_isLeftLimits = false;
			}
			_nEMoneyCreditIconCursorID++;
			_nEMoneyCreditIconCursorID %= _nEMoneyCreditIconNum;
			if (_nEMoneyCreditIconCursorID >= _nEMoneyCreditIconNum - 1)
			{
				_buttonController.SetVisibleImmediate(false, 5);
				_isRightLimits = true;
			}
		}

		public void CreditMoveLeft()
		{
			if (_isRightLimits)
			{
				_buttonController.SetVisibleImmediate(true, 5);
				_isRightLimits = false;
			}
			_nEMoneyCreditIconCursorID += _nEMoneyCreditIconNum;
			_nEMoneyCreditIconCursorID--;
			_nEMoneyCreditIconCursorID %= _nEMoneyCreditIconNum;
			if (_nEMoneyCreditIconCursorID <= 0)
			{
				_buttonController.SetVisibleImmediate(false, 6);
				_isLeftLimits = true;
			}
		}

		public void BrandMoveRight()
		{
			if (_isLeftLimits)
			{
				_buttonController.SetVisibleImmediate(true, 6);
				_isLeftLimits = false;
			}
			_nEMoneyBrandIconCursorID++;
			_nEMoneyBrandIconCursorID %= _nEMoneyBrandIconNum;
			if (_nEMoneyBrandIconCursorID >= _nEMoneyBrandIconNum - 1)
			{
				_buttonController.SetVisibleImmediate(false, 5);
				_isRightLimits = true;
			}
			_nEMoneyCurrentBrandTypeID = _valueListEMoneyBrandTypeID[_nEMoneyBrandIconCursorID];
		}

		public void BrandMoveLeft()
		{
			if (_isRightLimits)
			{
				_buttonController.SetVisibleImmediate(true, 5);
				_isRightLimits = false;
			}
			_nEMoneyBrandIconCursorID += _nEMoneyBrandIconNum;
			_nEMoneyBrandIconCursorID--;
			_nEMoneyBrandIconCursorID %= _nEMoneyBrandIconNum;
			if (_nEMoneyBrandIconCursorID <= 0)
			{
				_buttonController.SetVisibleImmediate(false, 6);
				_isLeftLimits = true;
			}
			_nEMoneyCurrentBrandTypeID = _valueListEMoneyBrandTypeID[_nEMoneyBrandIconCursorID];
		}

		public bool IsEnd()
		{
			return _state == BalanceCommonState.End;
		}

		public bool IsBalanceNetworkDisconnection()
		{
			bool result = false;
			switch (_parent_mode)
			{
			case ParentMode.ParentModeModeSelect:
				if (_monitor_mode_select != null)
				{
					result = _monitor_mode_select._isBalanceNetworkDisconnection;
				}
				break;
			case ParentMode.ParentModeTicketSelect:
				if (_monitor_ticket_select != null)
				{
					result = _monitor_ticket_select._isBalanceNetworkDisconnection;
				}
				break;
			}
			return result;
		}

		public void OKStart()
		{
			switch (_parent_mode)
			{
			case ParentMode.ParentModeModeSelect:
				if (_monitor_mode_select != null)
				{
					_monitor_mode_select.OKStart();
				}
				break;
			case ParentMode.ParentModeTicketSelect:
				if (_monitor_ticket_select != null)
				{
					_monitor_ticket_select.OKStart();
				}
				break;
			}
		}

		public void ResetOKStart()
		{
			switch (_parent_mode)
			{
			case ParentMode.ParentModeModeSelect:
				if (_monitor_mode_select != null)
				{
					_monitor_mode_select.ResetOKStart();
				}
				break;
			case ParentMode.ParentModeTicketSelect:
				if (_monitor_ticket_select != null)
				{
					_monitor_ticket_select.ResetOKStart();
				}
				break;
			}
		}

		public void BackStart()
		{
			switch (_parent_mode)
			{
			case ParentMode.ParentModeModeSelect:
				if (_monitor_mode_select != null)
				{
					_monitor_mode_select.BackStart();
				}
				break;
			case ParentMode.ParentModeTicketSelect:
				if (_monitor_ticket_select != null)
				{
					_monitor_ticket_select.BackStart();
				}
				break;
			}
		}

		public void ResetBackStart()
		{
			switch (_parent_mode)
			{
			case ParentMode.ParentModeModeSelect:
				if (_monitor_mode_select != null)
				{
					_monitor_mode_select.ResetBackStart();
				}
				break;
			case ParentMode.ParentModeTicketSelect:
				if (_monitor_ticket_select != null)
				{
					_monitor_ticket_select.ResetBackStart();
				}
				break;
			}
		}

		public void BalanceStart()
		{
			switch (_parent_mode)
			{
			case ParentMode.ParentModeModeSelect:
				if (_monitor_mode_select != null)
				{
					_monitor_mode_select.BalanceStart();
				}
				break;
			case ParentMode.ParentModeTicketSelect:
				if (_monitor_ticket_select != null)
				{
					_monitor_ticket_select.BalanceStart();
				}
				break;
			}
		}

		public void ResetBalanceStart()
		{
			switch (_parent_mode)
			{
			case ParentMode.ParentModeModeSelect:
				if (_monitor_mode_select != null)
				{
					_monitor_mode_select.ResetBalanceStart();
				}
				break;
			case ParentMode.ParentModeTicketSelect:
				if (_monitor_ticket_select != null)
				{
					_monitor_ticket_select.ResetBalanceStart();
				}
				break;
			}
		}

		public bool IsOK()
		{
			bool result = false;
			switch (_parent_mode)
			{
			case ParentMode.ParentModeModeSelect:
				if (_monitor_mode_select != null)
				{
					result = _monitor_mode_select._isOK;
				}
				break;
			case ParentMode.ParentModeTicketSelect:
				if (_monitor_ticket_select != null)
				{
					result = _monitor_ticket_select._isOK;
				}
				break;
			}
			return result;
		}

		public bool IsBack()
		{
			bool result = false;
			switch (_parent_mode)
			{
			case ParentMode.ParentModeModeSelect:
				if (_monitor_mode_select != null)
				{
					result = _monitor_mode_select._isBack;
				}
				break;
			case ParentMode.ParentModeTicketSelect:
				if (_monitor_ticket_select != null)
				{
					result = _monitor_ticket_select._isBack;
				}
				break;
			}
			return result;
		}

		public bool IsTimeUp()
		{
			bool result = false;
			switch (_parent_mode)
			{
			case ParentMode.ParentModeModeSelect:
				if (_monitor_mode_select != null)
				{
					result = _monitor_mode_select._isTimeUp;
				}
				break;
			case ParentMode.ParentModeTicketSelect:
				if (_monitor_ticket_select != null)
				{
					result = _monitor_ticket_select._isTimeUp;
				}
				break;
			}
			return result;
		}

		public bool IsBalance()
		{
			bool result = false;
			switch (_parent_mode)
			{
			case ParentMode.ParentModeModeSelect:
				if (_monitor_mode_select != null)
				{
					result = _monitor_mode_select._isBalance;
				}
				break;
			case ParentMode.ParentModeTicketSelect:
				if (_monitor_ticket_select != null)
				{
					result = _monitor_ticket_select._isBalance;
				}
				break;
			}
			return result;
		}

		public bool IsRight()
		{
			bool result = false;
			switch (_parent_mode)
			{
			case ParentMode.ParentModeModeSelect:
				if (_monitor_mode_select != null)
				{
					result = _monitor_mode_select._isRight;
				}
				break;
			case ParentMode.ParentModeTicketSelect:
				if (_monitor_ticket_select != null)
				{
					result = _monitor_ticket_select._isRight;
				}
				break;
			}
			return result;
		}

		public bool IsLeft()
		{
			bool result = false;
			switch (_parent_mode)
			{
			case ParentMode.ParentModeModeSelect:
				if (_monitor_mode_select != null)
				{
					result = _monitor_mode_select._isLeft;
				}
				break;
			case ParentMode.ParentModeTicketSelect:
				if (_monitor_ticket_select != null)
				{
					result = _monitor_ticket_select._isLeft;
				}
				break;
			}
			return result;
		}

		private void SetParentRightFlag(bool flag)
		{
			switch (_parent_mode)
			{
			case ParentMode.ParentModeModeSelect:
				if (_monitor_mode_select != null)
				{
					_monitor_mode_select._isRight = flag;
				}
				break;
			case ParentMode.ParentModeTicketSelect:
				if (_monitor_ticket_select != null)
				{
					_monitor_ticket_select._isRight = flag;
				}
				break;
			}
		}

		private void SetParentLeftFlag(bool flag)
		{
			switch (_parent_mode)
			{
			case ParentMode.ParentModeModeSelect:
				if (_monitor_mode_select != null)
				{
					_monitor_mode_select._isLeft = flag;
				}
				break;
			case ParentMode.ParentModeTicketSelect:
				if (_monitor_ticket_select != null)
				{
					_monitor_ticket_select._isLeft = flag;
				}
				break;
			}
		}

		private void SetParentCheckLRFlag(bool flag)
		{
			switch (_parent_mode)
			{
			case ParentMode.ParentModeModeSelect:
				if (_monitor_mode_select != null)
				{
					_monitor_mode_select._isCheckLR = flag;
				}
				break;
			case ParentMode.ParentModeTicketSelect:
				if (_monitor_ticket_select != null)
				{
					_monitor_ticket_select._isCheckLR = flag;
				}
				break;
			}
		}

		public void RequestParentChangeOKButton(bool isGray, bool flag = true)
		{
			switch (_parent_mode)
			{
			case ParentMode.ParentModeModeSelect:
				if (_monitor_mode_select != null)
				{
					_monitor_mode_select._isSetGrayOKButton = isGray;
					_monitor_mode_select._isRequestChangeOKButtonColor = flag;
					_isGrayOKButton = isGray;
				}
				break;
			case ParentMode.ParentModeTicketSelect:
				if (_monitor_ticket_select != null)
				{
					_monitor_ticket_select._isSetGrayOKButton = isGray;
					_monitor_ticket_select._isRequestChangeOKButtonColor = flag;
					_isGrayOKButton = isGray;
				}
				break;
			}
		}

		public void RequestParentChangeBackButton(bool isGray, bool flag = true)
		{
			switch (_parent_mode)
			{
			case ParentMode.ParentModeModeSelect:
				if (_monitor_mode_select != null)
				{
					_monitor_mode_select._isSetGrayBackButton = isGray;
					_monitor_mode_select._isRequestChangeBackButtonColor = flag;
				}
				break;
			case ParentMode.ParentModeTicketSelect:
				if (_monitor_ticket_select != null)
				{
					_monitor_ticket_select._isSetGrayBackButton = isGray;
					_monitor_ticket_select._isRequestChangeBackButtonColor = flag;
				}
				break;
			}
		}

		public void RequestParentChangeBalanceButton(bool isGray, bool flag = true)
		{
			switch (_parent_mode)
			{
			case ParentMode.ParentModeModeSelect:
				if (_monitor_mode_select != null)
				{
					_monitor_mode_select._isSetGrayBalanceButton = isGray;
					_monitor_mode_select._isRequestChangeBalanceButtonColor = flag;
				}
				break;
			case ParentMode.ParentModeTicketSelect:
				if (_monitor_ticket_select != null)
				{
					_monitor_ticket_select._isSetGrayBalanceButton = isGray;
					_monitor_ticket_select._isRequestChangeBalanceButtonColor = flag;
				}
				break;
			}
		}

		public void ChangeButtonType(ButtonControllerBase.FlatButtonType type)
		{
			switch (_parent_mode)
			{
			case ParentMode.ParentModeModeSelect:
				if (_monitor_mode_select != null)
				{
					_monitor_mode_select._buttonController.ChangeButtonType(type);
				}
				break;
			case ParentMode.ParentModeTicketSelect:
				if (_monitor_ticket_select != null)
				{
					_monitor_ticket_select._buttonController.ChangeButtonType(type);
				}
				break;
			}
		}

		public void InitDefaultIconNum()
		{
			_nEMoneyCreditIconNum = 5;
			_nEMoneyCreditIconCursorID = 0;
		}

		public void InitDefaultBrandNum()
		{
			_nEMoneyBrandIconNum = 6;
			_nEMoneyBrandIconCursorID = 0;
		}

		public void SetPriceIconNum()
		{
			_nEMoneyCreditCoins = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.CoinToCredit;
			_nEMoneyCreditRate = (int)AMDaemon.Credit.Config.CoinAmount;
			int num = _maxCreditValue / (_nEMoneyCreditCoins * _nEMoneyCreditRate);
			int num2 = 24 - SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.NowCredit;
			int num3 = 0;
			int coinToCredit = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.CoinToCredit;
			num3 = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.Remain;
			if (coinToCredit > 1 && num3 > 0)
			{
				num2--;
			}
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (num > num2)
			{
				num = num2;
			}
			_nEMoneyCreditIconNum = 0;
			if (num > 0)
			{
				_nEMoneyCreditIconNum = num;
				if (_nEMoneyCreditIconNum > 5)
				{
					_nEMoneyCreditIconNum = 5;
				}
			}
			else
			{
				_nEMoneyCreditIconNum = -1;
			}
		}

		public void SetBrandIconNum(bool isBalance = false)
		{
			bool flag = false;
			_valueListEMoneyBrandTypeID.Clear();
			_valueListEMoneyBrandEnable.Clear();
			_nEMoneyBrandIconNum = AMDaemon.EMoney.AvailableBrandCount;
			if (_nEMoneyBrandIconNum <= 0)
			{
				flag = true;
			}
			else
			{
				if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsCheckDisableEmoneyBrandID)
				{
					bool flag2 = false;
					int num = 0;
					if (!isBalance)
					{
						for (int i = 0; i < _nEMoneyBrandIconNum; i++)
						{
							if (AMDaemon.EMoney.AvailableBrands[i].Id != EMoneyBrandId.Sapica)
							{
								bool flag3 = false;
								_valueListEMoneyBrandTypeID.Add(AMDaemon.EMoney.AvailableBrands[i].Id);
								flag3 = AMDaemon.EMoney.IsBrandAvailable(AMDaemon.EMoney.AvailableBrands[i].Id);
								_valueListEMoneyBrandEnable.Add(flag3);
							}
							else
							{
								flag2 = true;
								num++;
							}
						}
						if (flag2)
						{
							_nEMoneyBrandIconNum -= num;
						}
					}
					else if (isBalance)
					{
						for (int j = 0; j < _nEMoneyBrandIconNum; j++)
						{
							if (AMDaemon.EMoney.AvailableBrands[j].Id != EMoneyBrandId.Sapica && SingletonStateMachine<AmManager, AmManager.EState>.Instance.Emoney.IsAvalilableBalance(AMDaemon.EMoney.AvailableBrands[j].Id))
							{
								bool flag4 = false;
								_valueListEMoneyBrandTypeID.Add(AMDaemon.EMoney.AvailableBrands[j].Id);
								flag4 = AMDaemon.EMoney.IsBrandAvailable(AMDaemon.EMoney.AvailableBrands[j].Id);
								_valueListEMoneyBrandEnable.Add(flag4);
							}
							else
							{
								flag2 = true;
								num++;
							}
						}
						if (flag2)
						{
							_nEMoneyBrandIconNum -= num;
						}
					}
				}
				int num2 = _valueListEMoneyBrandTypeID.FindIndex((EMoneyBrandId m) => m == _nEMoneyCurrentBrandTypeID);
				if (_nEMoneyBrandIconNum <= 0)
				{
					flag = true;
				}
				else if (num2 < 0)
				{
					_nEMoneyBrandIconCursorID = 0;
					_nEMoneyCurrentBrandTypeID = _valueListEMoneyBrandTypeID[0];
				}
				else
				{
					_nEMoneyBrandIconCursorID = num2;
					_nEMoneyCurrentBrandTypeID = _valueListEMoneyBrandTypeID[num2];
				}
			}
			if (flag)
			{
				_nEMoneyBrandIconNum = -1;
				_nEMoneyBrandIconCursorID = 0;
				_nEMoneyCurrentBrandTypeID = EMoneyBrandId.Nanaco;
			}
		}

		public bool IsEnableBaranceBrand(bool isBalance = true)
		{
			bool flag = false;
			int num = 0;
			num = AMDaemon.EMoney.AvailableBrandCount;
			if (num <= 0)
			{
				flag = false;
			}
			else
			{
				flag = false;
				if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsCheckDisableEmoneyBrandID)
				{
					if (isBalance)
					{
						for (int i = 0; i < num; i++)
						{
							if (AMDaemon.EMoney.AvailableBrands[i].Id != EMoneyBrandId.Sapica && SingletonStateMachine<AmManager, AmManager.EState>.Instance.Emoney.IsAvalilableBalance(AMDaemon.EMoney.AvailableBrands[i].Id))
							{
								flag = true;
								break;
							}
						}
					}
					else if (!isBalance)
					{
						for (int j = 0; j < num; j++)
						{
							if (AMDaemon.EMoney.AvailableBrands[j].Id != EMoneyBrandId.Sapica && SingletonStateMachine<AmManager, AmManager.EState>.Instance.Emoney.IsAvalilable(AMDaemon.EMoney.AvailableBrands[j].Id))
							{
								flag = true;
								break;
							}
						}
					}
				}
			}
			return flag;
		}

		public int GetBrandTextureID(EMoneyBrandId brand_id)
		{
			int result = 0;
			switch (brand_id)
			{
			case EMoneyBrandId.Nanaco:
				result = 2;
				break;
			case EMoneyBrandId.Edy:
				result = 3;
				break;
			case EMoneyBrandId.Transport:
				result = 1;
				break;
			case EMoneyBrandId.Waon:
				result = 0;
				break;
			case EMoneyBrandId.Paseli:
				result = 4;
				break;
			case EMoneyBrandId.iD:
				result = 5;
				break;
			case EMoneyBrandId.Sapica:
				result = 1;
				break;
			}
			return result;
		}

		public void ChangeCreditIconColor(GameObject obj, bool isGray = false)
		{
			Image image = null;
			if (!(obj != null))
			{
				return;
			}
			for (int i = 0; i < obj.transform.childCount; i++)
			{
				image = obj.transform.GetChild(i).gameObject.GetComponent<Image>();
				if (image != null)
				{
					if (isGray)
					{
						image.color = new Color(0.5019608f, 0.5019608f, 0.5019608f, 1f);
					}
					else
					{
						image.color = new Color(1f, 1f, 1f, 1f);
					}
				}
			}
			for (int j = 0; j < obj.transform.GetChild(0).childCount; j++)
			{
				image = obj.transform.GetChild(0).GetChild(j).gameObject.GetComponent<Image>();
				if (image != null)
				{
					if (isGray)
					{
						image.color = new Color(0.5019608f, 0.5019608f, 0.5019608f, 1f);
					}
					else
					{
						image.color = new Color(1f, 1f, 1f, 1f);
					}
				}
			}
		}

		public void ChangeBrandIconColor(GameObject obj, bool isGray = false)
		{
			Image image = null;
			if (!(obj != null))
			{
				return;
			}
			for (int i = 0; i < obj.transform.childCount; i++)
			{
				image = obj.transform.GetChild(i).gameObject.GetComponent<Image>();
				if (image != null)
				{
					if (isGray)
					{
						image.color = new Color(0.5019608f, 0.5019608f, 0.5019608f, 1f);
					}
					else
					{
						image.color = new Color(1f, 1f, 1f, 1f);
					}
				}
			}
		}

		public void SetCreditIcon(bool active_list = true)
		{
			GameObject gameObject = _emoneyWindow.transform.GetChild(0).GetChild(0).GetChild(3)
				.gameObject;
			GameObject gameObject2 = null;
			string text = "";
			int num = 0;
			int num2 = 0;
			if (!active_list)
			{
				SetActiveChildren(gameObject, active: false);
			}
			if (_nEMoneyCreditIconNum - 1 >= 0)
			{
				gameObject2 = gameObject.transform.GetChild(_nEMoneyCreditIconNum - 1).gameObject;
				gameObject2.SetActive(active_list);
			}
			_isRightLimits = false;
			_isLeftLimits = false;
			if (_nEMoneyCreditIconCursorID >= _nEMoneyCreditIconNum - 1)
			{
				_isRightLimits = true;
			}
			if (_nEMoneyCreditIconCursorID <= 0)
			{
				_isLeftLimits = true;
			}
			if (active_list)
			{
				if (!_isRightLimits)
				{
					_buttonController.SetVisibleImmediate(true, 5);
				}
				if (!_isLeftLimits)
				{
					_buttonController.SetVisibleImmediate(true, 6);
				}
			}
			GameObject gameObject3 = null;
			Animator animator = null;
			if (!active_list)
			{
				return;
			}
			bool flag = IsEnableRequest() && !_isGrayOKButton;
			for (int i = 0; i < _nEMoneyCreditIconNum; i++)
			{
				if (gameObject2 != null)
				{
					gameObject3 = gameObject2.transform.GetChild(i).GetChild(0).gameObject;
					animator = gameObject3.GetComponent<Animator>();
					num2 = _nEMoneyCreditRate * _nEMoneyCreditCoins * (i + 1);
					if (_nEMoneyCreditIconCursorID == i)
					{
						animator.Play("Select", 0, 0f);
						if (flag)
						{
							ChangeCreditIconColor(animator.gameObject);
						}
						else
						{
							ChangeCreditIconColor(animator.gameObject, isGray: true);
						}
						gameObject3.transform.GetChild(3).GetChild(0).gameObject.SetActive(value: true);
						gameObject3.transform.GetChild(3).GetChild(1).gameObject.SetActive(value: true);
						if (_isRightLimits || _isLeftLimits)
						{
							if (_isRightLimits)
							{
								gameObject3.transform.GetChild(3).GetChild(0).gameObject.SetActive(value: false);
							}
							if (_isLeftLimits)
							{
								gameObject3.transform.GetChild(3).GetChild(1).gameObject.SetActive(value: false);
							}
						}
					}
					else if (flag)
					{
						animator.Play("Active", 0, 0f);
						ChangeCreditIconColor(animator.gameObject);
					}
					else
					{
						animator.Play("Active", 0, 0f);
						ChangeCreditIconColor(animator.gameObject, isGray: true);
					}
				}
				num = i;
				if (num >= _nEMoneyCreditIconNum - 1)
				{
					num = _nEMoneyCreditIconNum - 1;
				}
				gameObject2.transform.GetChild(i).GetChild(0).GetChild(1)
					.GetComponent<MultipleImage>()
					.ChangeSprite(num);
				text = ((!flag) ? ("<color=#808080>" + num2.ToString("0") + "</color>") : ("<color=#ffffff>" + num2.ToString("0") + "</color>"));
				gameObject2.transform.GetChild(i).GetChild(0).GetChild(2)
					.GetComponent<TextMeshProUGUI>()
					.text = text;
			}
		}

		public void SetBrandIcon01(bool active_list = true)
		{
			GameObject gameObject = _emoneyWindow.transform.GetChild(0).GetChild(0).GetChild(2)
				.gameObject;
			GameObject gameObject2 = null;
			GameObject gameObject3 = null;
			Animator animator = null;
			int num = 0;
			if (!active_list)
			{
				SetActiveChildren(gameObject, active: false);
			}
			if (_nEMoneyBrandIconNum - 1 >= 0)
			{
				gameObject2 = gameObject.transform.GetChild(_nEMoneyBrandIconNum - 1).gameObject;
				gameObject2.SetActive(active_list);
			}
			_isRightLimits = false;
			_isLeftLimits = false;
			if (_nEMoneyBrandIconCursorID >= _nEMoneyBrandIconNum - 1)
			{
				_isRightLimits = true;
			}
			if (_nEMoneyBrandIconCursorID <= 0)
			{
				_isLeftLimits = true;
			}
			if (!active_list)
			{
				return;
			}
			if (!_isRightLimits)
			{
				_buttonController.SetVisibleImmediate(true, 5);
			}
			if (!_isLeftLimits)
			{
				_buttonController.SetVisibleImmediate(true, 6);
			}
			bool flag = IsEnableRequest() && !_isGrayOKButton;
			for (int i = 0; i < _nEMoneyBrandIconNum; i++)
			{
				if (!(gameObject2 != null))
				{
					continue;
				}
				gameObject3 = gameObject2.transform.GetChild(i).GetChild(0).gameObject;
				animator = gameObject3.GetComponent<Animator>();
				if (_nEMoneyBrandIconCursorID == i)
				{
					animator.Play("Selected", 0, 0f);
					gameObject3.transform.GetChild(4).GetChild(1).gameObject.SetActive(value: true);
					gameObject3.transform.GetChild(4).GetChild(0).gameObject.SetActive(value: true);
					if (_isRightLimits || _isLeftLimits)
					{
						if (_isRightLimits)
						{
							gameObject3.transform.GetChild(4).GetChild(1).gameObject.SetActive(value: false);
						}
						if (_isLeftLimits)
						{
							gameObject3.transform.GetChild(4).GetChild(0).gameObject.SetActive(value: false);
						}
					}
				}
				else
				{
					animator.Play("NotSelected_01", 0, 0f);
				}
				num = GetBrandTextureID(_valueListEMoneyBrandTypeID[i]);
				if (num > _maxBrandTexID)
				{
					num = GetBrandTextureID(_valueListEMoneyBrandTypeID[0]);
				}
				gameObject3.transform.GetChild(3).GetComponent<MultipleImage>().ChangeSprite(num);
				if (_valueListEMoneyBrandEnable[i] && flag)
				{
					ChangeBrandIconColor(animator.gameObject);
				}
				else
				{
					ChangeBrandIconColor(animator.gameObject, isGray: true);
				}
			}
		}

		public void SetBrandIcon02(bool active_list = true)
		{
			GameObject gameObject = _emoneyWindow.transform.GetChild(0).GetChild(1).GetChild(2)
				.gameObject;
			GameObject gameObject2 = null;
			GameObject gameObject3 = null;
			Animator animator = null;
			int num = 0;
			if (!active_list)
			{
				SetActiveChildren(gameObject, active: false);
			}
			if (_nEMoneyBrandIconNum - 1 >= 0)
			{
				gameObject2 = gameObject.transform.GetChild(_nEMoneyBrandIconNum - 1).gameObject;
				gameObject2.SetActive(active_list);
			}
			_isRightLimits = false;
			_isLeftLimits = false;
			if (_nEMoneyBrandIconCursorID >= _nEMoneyBrandIconNum - 1)
			{
				_isRightLimits = true;
			}
			if (_nEMoneyBrandIconCursorID <= 0)
			{
				_isLeftLimits = true;
			}
			if (!active_list)
			{
				return;
			}
			if (!_isRightLimits)
			{
				_buttonController.SetVisibleImmediate(true, 5);
			}
			if (!_isLeftLimits)
			{
				_buttonController.SetVisibleImmediate(true, 6);
			}
			bool flag = IsEnableRequest() && !_isGrayOKButton;
			for (int i = 0; i < _nEMoneyBrandIconNum; i++)
			{
				if (!(gameObject2 != null))
				{
					continue;
				}
				gameObject3 = gameObject2.transform.GetChild(i).GetChild(0).gameObject;
				animator = gameObject3.GetComponent<Animator>();
				if (_nEMoneyBrandIconCursorID == i)
				{
					animator.Play("Selected", 0, 0f);
					gameObject3.transform.GetChild(4).GetChild(1).gameObject.SetActive(value: true);
					gameObject3.transform.GetChild(4).GetChild(0).gameObject.SetActive(value: true);
					if (_isRightLimits || _isLeftLimits)
					{
						if (_isRightLimits)
						{
							gameObject3.transform.GetChild(4).GetChild(1).gameObject.SetActive(value: false);
						}
						if (_isLeftLimits)
						{
							gameObject3.transform.GetChild(4).GetChild(0).gameObject.SetActive(value: false);
						}
					}
				}
				else
				{
					animator.Play("NotSelected_02", 0, 0f);
				}
				num = GetBrandTextureID(_valueListEMoneyBrandTypeID[i]);
				if (num > _maxBrandTexID)
				{
					num = GetBrandTextureID(_valueListEMoneyBrandTypeID[0]);
				}
				gameObject3.transform.GetChild(3).GetComponent<MultipleImage>().ChangeSprite(num);
				if (_valueListEMoneyBrandEnable[i] && flag)
				{
					ChangeBrandIconColor(animator.gameObject);
				}
				else
				{
					ChangeBrandIconColor(animator.gameObject, isGray: true);
				}
			}
		}

		public void BrandBottomInfo(bool disp_brand_icon, bool disp_credit_info = false)
		{
			GameObject gameObject = _emoneyWindow.transform.GetChild(0).GetChild(0).gameObject;
			GameObject gameObject2 = gameObject.transform.GetChild(4).gameObject;
			GameObject gameObject3 = gameObject.transform.GetChild(5).gameObject;
			GameObject gameObject4 = gameObject.transform.GetChild(6).gameObject;
			int num = 0;
			if (disp_brand_icon)
			{
				gameObject2.SetActive(disp_credit_info);
				gameObject3.SetActive(value: false);
				SetActiveChildren(gameObject3, active: false);
				gameObject4.SetActive(value: true);
				SetActiveChildren(gameObject4, active: true);
			}
			else
			{
				gameObject2.SetActive(disp_credit_info);
				gameObject3.SetActive(value: true);
				SetActiveChildren(gameObject3, active: true);
				gameObject4.SetActive(value: false);
				SetActiveChildren(gameObject4, active: false);
			}
			int num2 = _nEMoneyCreditRate * _nEMoneyCreditCoins * (_nEMoneyCreditIconCursorID + 1);
			string text = "";
			if (disp_brand_icon)
			{
				int num3 = 0;
				text = "お支払い金額\u3000￥";
				gameObject4.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = text;
				text = num2.ToString("000");
				gameObject4.transform.GetChild(0).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = text;
				gameObject4.transform.GetChild(1).gameObject.SetActive(value: true);
				gameObject4.transform.GetChild(1).GetChild(0).GetComponent<Animator>()
					.Play("NotSelected_01", 0, 0f);
				num3 = _nEMoneyBrandIconCursorID;
				if (num3 > _nEMoneyBrandIconNum - 1)
				{
					num3 = 0;
				}
				num = GetBrandTextureID(_valueListEMoneyBrandTypeID[num3]);
				if (num > _maxBrandTexID)
				{
					num = GetBrandTextureID(_valueListEMoneyBrandTypeID[0]);
				}
				gameObject4.transform.GetChild(1).GetChild(0).GetChild(3)
					.GetComponent<MultipleImage>()
					.ChangeSprite(num);
				if (disp_brand_icon && disp_credit_info)
				{
					GameObject gameObject5 = null;
					gameObject5 = gameObject4.transform.GetChild(1).GetChild(0).gameObject;
					if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Emoney.IsAvalilable(_valueListEMoneyBrandTypeID[num3]) && IsEnableRequest())
					{
						ChangeBrandIconColor(gameObject5);
					}
					else
					{
						ChangeBrandIconColor(gameObject5, isGray: true);
					}
				}
			}
			else
			{
				text = "お支払い金額\u3000￥";
				gameObject3.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = text;
				text = num2.ToString("000");
				gameObject3.transform.GetChild(0).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = text;
			}
		}

		public void ResetError()
		{
			ResetError01();
			ResetError02();
		}

		public void ResetError01()
		{
			if (_isOwnBalanceError01)
			{
				GameObject gameObject = _creditWindow.transform.GetChild(0).GetChild(1).gameObject;
				gameObject.transform.GetChild(4).gameObject.GetComponent<Animator>().Play("Out", 0, 0f);
				gameObject.transform.GetChild(5).gameObject.SetActive(value: true);
				_isOwnBalanceError01 = false;
			}
		}

		public void ResetError02()
		{
			if (_isOwnBalanceError02)
			{
				GameObject gameObject = _creditWindow.transform.GetChild(0).GetChild(1).gameObject;
				gameObject.transform.GetChild(4).gameObject.GetComponent<Animator>().Play("Out", 0, 0f);
				gameObject.transform.GetChild(5).gameObject.SetActive(value: true);
				_isOwnBalanceError02 = false;
			}
		}

		public void SetBalanceText(int mode, int type)
		{
			string text = "";
			switch (mode)
			{
			case 0:
				switch (type)
				{
				case 0:
					text = "お支払い金額を選択してください";
					break;
				case 1:
					text = "お支払い方法を選択してください";
					break;
				case 2:
					text = "タッチ式カードリーダーにタッチしてください";
					break;
				case 3:
					text = "タッチ式カードリーダ上部の画面をご確認ください";
					break;
				}
				_emoneyWindow.transform.GetChild(0).GetChild(0).GetChild(1)
					.gameObject.GetComponent<TextMeshProUGUI>().text = text;
				break;
			case 1:
				switch (type)
				{
				case 0:
					text = "お支払い方法を選択してください";
					break;
				case 1:
					text = "タッチ式カードリーダーにタッチしてください\r\n残高はタッチ式カードリーダ上部の画面をご確認ください";
					break;
				case 2:
					text = "残高はタッチ式カードリーダ上部の画面をご確認ください";
					break;
				}
				_emoneyWindow.transform.GetChild(0).GetChild(1).GetChild(1)
					.gameObject.GetComponent<TextMeshProUGUI>().text = text;
				break;
			}
		}

		public bool IsEnableRequest()
		{
			bool flag = false;
			if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.IsFreePlay() || !Singleton<OperationManager>.Instance.IsCoinAcceptable() || IsBalanceNetworkDisconnection() || !AMDaemon.EMoney.IsAuthCompleted || !AMDaemon.EMoney.IsServiceAlive || !AMDaemon.EMoney.Operation.CanOperateDeal)
			{
				return false;
			}
			return true;
		}

		public void ViewUpdate()
		{
		}
	}
}
