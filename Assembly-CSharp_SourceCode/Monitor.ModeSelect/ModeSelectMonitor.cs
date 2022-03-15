using System.Collections.Generic;
using AMDaemon;
using Balance;
using DB;
using IO;
using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_000001;
using Manager;
using Manager.MaiStudio.CardTypeName;
using Manager.UserDatas;
using Mecha;
using Monitor.Entry.Parts.Screens;
using Process.CodeRead;
using Process.ModeSelect;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.ModeSelect
{
	public class ModeSelectMonitor : MonitorBase
	{
		public enum MonitorState
		{
			None,
			Init,
			SelectCardFadeInJudge,
			SelectCardFadeInJudgeWait,
			SelectCardFadeInWait,
			SelectCardFadeInSync,
			SelectCardFadeInSyncWait,
			SelectCardWaitInit,
			SelectCardWait,
			SelectCardWaitSyncBoth,
			SelectCardMoveRight,
			SelectCardMoveLeft,
			SettingOpen_SyncAnimWait,
			SettingOpen_SyncBothReached,
			SettingWait,
			SettingChange,
			SettingChangeWait,
			SettingClose_SyncAnimWait,
			SettingClose_SyncBothReached,
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
			DecideCreditFadeOut,
			DecideCreditFadeOutWait,
			CreditJudgeReset,
			DecidedEntryFadeIn,
			DecidedEntryFadeInWait,
			DecidedEntryWait,
			DecidedEntryFadeOut,
			DecidedEntryFadeOutWait,
			EntryResetInit,
			EntryReset,
			EntryTimeOutJudge,
			EntryTimeOutReturnTitleFadeIn,
			EntryTimeOutReturnTitleFadeInWait,
			EntryTimeOutReturnTitleWait,
			EntryTimeOutReturnTitleFadeOut,
			EntryTimeOutReturnTitleFadeOutWait,
			AddTrackInfoJudge,
			AddTrackInfoJudgeWait,
			AddTrackInfoFadeIn,
			AddTrackInfoFadeInWait,
			AddTrackInfoWait,
			Finish,
			End
		}

		public enum MonitorButtonMode
		{
			MonitorButtonModeCommon,
			MonitorButtonModeEMoney
		}

		public enum ModeSelectType
		{
			ModeSelectTypeNormal,
			ModeSelectTypeDani,
			ModeSelectTypeFreedom,
			ModeSelectTypeMAX
		}

		public enum VolumeButton
		{
			VolumeButtonMinus,
			VolumeButtonPlus,
			VolumeButtonMax
		}

		public enum AddTrackType
		{
			AddTrackTypeNone,
			AddTrackType1PEvent,
			AddTrackType1PNormal,
			AddTrackTypeDailyBonus,
			AddTrackTypeWeekDayBonus,
			AddTrackType2PEvent,
			AddTrackType2PNormal,
			AddTrackType2POwnIsNew,
			AddTrackTypeMAX
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
		public ModeSelectButtonController _buttonController;

		[SerializeField]
		public GameObject _masterNull;

		[SerializeField]
		public GameObject _masterNullSelector;

		[SerializeField]
		public GameObject _cardPassIcon;

		[SerializeField]
		public GameObject _settingWindow;

		[SerializeField]
		public GameObject _settingVolumePlus;

		[SerializeField]
		public GameObject _settingVolumeMinus;

		[SerializeField]
		public GameObject _headphoneVolume;

		[SerializeField]
		public GameObject _headphoneVolumeDigit;

		[SerializeField]
		public GameObject _settingMiniWindow;

		[SerializeField]
		public GameObject _creditWindow;

		[SerializeField]
		public GameObject _emoneyWindow;

		[SerializeField]
		public GameObject _entryInfoWindow;

		[SerializeField]
		public GameObject _messageWindow;

		[SerializeField]
		public GameObject _lastBlur;

		public MonitorState _state = MonitorState.End;

		public ModeSelectProcess.ProccessActiveMode _active_mode;

		public MonitorButtonMode _button_mode;

		public int _timer;

		private int _sub_count;

		public bool _isMajorVersionUp;

		public bool _isMinorVersionUp;

		private int _monitorID;

		public bool _isEntry;

		public bool _isGuest;

		public bool _isAwake;

		public bool _isNewAime;

		public bool _isEnableOK;

		public bool _isCheckOK;

		public bool _isOK;

		public bool _isCheckLR;

		public bool _isRight;

		public bool _isLeft;

		public bool _isCheckTimeSkip;

		public bool _isTimeSkip;

		public bool _isEnableTimeSkip;

		public bool _isInvisibleTimeSkip;

		public bool _isCheckBack;

		public bool _isBack;

		private bool _isEnableBack;

		public bool _isCheckSetting;

		public bool _isSetting;

		public int _isEntryNum = 1;

		public bool _isCreditResult;

		private List<GameObject> _nullListMiniCard = new List<GameObject>();

		private List<GameObject> _nullListBigCard = new List<GameObject>();

		private List<int> _objModeSelectTypeListCard = new List<int>();

		private List<CommonValue> _valueListMiniCardPosX = new List<CommonValue>();

		private List<CommonValue> _valueListMiniCardScaleXY = new List<CommonValue>();

		private List<CommonValue> _valueListBigCardPosX = new List<CommonValue>();

		private List<CommonValue> _valueListBigCardScaleXY = new List<CommonValue>();

		private int _dispCardMax = 5;

		private float[] _cardMiniPosX = new float[5] { -500f, -220f, 0f, 220f, 500f };

		private float[] _cardMiniScaleXY = new float[5] { 0.75f, 0.75f, 1f, 0.75f, 0.75f };

		private float[] _cardBigPosX = new float[5] { -1080f, -540f, 0f, 540f, 1080f };

		private float[] _cardBigScaleXY = new float[5] { 1f, 1f, 1f, 1f, 1f };

		private float _baseBigPosY = 80f;

		private int[] _modeSelectBaseType = new int[3];

		private bool[] _modeSelectTypeEnable = new bool[3] { true, true, true };

		private List<GameObject> _nullSetting = new List<GameObject>();

		private List<Animator> _animSetting = new List<Animator>();

		private List<bool> _isSettingToggleState = new List<bool>();

		private List<bool> _isSettingEnable = new List<bool>();

		private int _toggleMax = 4;

		private int[] _callSettingToggleID = new int[4] { -1, -1, -1, -1 };

		private int[] _toggleWaitCount = new int[4];

		private int _volumeValue = 50;

		private int _callSettingVolumeID = -1;

		private int _volumeWaitCount;

		private float _cardMoveTime = 10f;

		private int _cardPosEnd;

		public bool _isEnableTimer;

		public bool _isTimerVisible;

		public bool _isDecidedOK;

		public bool _isBothDecidedEntry;

		public bool _isTimeUp;

		public int _decidedSelectModeType = -1;

		public bool _isChangeSettingSibling;

		public bool _isCallBlur;

		public bool _isChangeSibling;

		public bool _isCallLastBlur;

		public int _needCreditNum = -1;

		public int _needCoinNum = -1;

		public int _needCreditDenominator = -1;

		public int _needCreditNumerator = -1;

		public bool _isNeedCreditFraction;

		public bool _isBlockingDecideCredit;

		public bool _isDecidedEntry;

		public int _isDecidedEntryNum;

		public bool _isCanceledEntry;

		public int _isCanceledEntryNum;

		public bool _isEntrySE;

		public bool _isSetBookkeepStartTime;

		public bool _isRemainCredit;

		public bool _isGrayOKButton;

		public bool _isRequestChangeOKButtonColor;

		public bool _isSetGrayOKButton;

		public bool _isGrayBackButton;

		public bool _isRequestChangeBackButtonColor;

		public bool _isSetGrayBackButton;

		public bool _isGrayBalanceButton;

		public bool _isRequestChangeBalanceButtonColor;

		public bool _isSetGrayBalanceButton;

		private static readonly int Sync = Animator.StringToHash("Sync");

		private float syncTime;

		public bool _isHoldVolume;

		public bool _isDailyBonus;

		public bool _isWeekDayBonus;

		public int _addTrackType;

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

		public bool IsEnableModeSelectType()
		{
			int index = _dispCardMax / 2;
			return _modeSelectTypeEnable[_objModeSelectTypeListCard[index]];
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

		public void HeadphoneVolumeViewUpdate(float add)
		{
			Animator component = _settingVolumeMinus.GetComponent<Animator>();
			Animator component2 = _settingVolumePlus.GetComponent<Animator>();
			syncTime += add / 1000f;
			component.SetFloat(Sync, syncTime);
			component2.SetFloat(Sync, syncTime);
			if (1f < syncTime)
			{
				syncTime = 0f;
			}
		}

		public void OKButtonAnim(InputManager.ButtonSetting button)
		{
			int animationActive = 0;
			if (!_isBlockingDecideCredit && _isCheckOK && !_isOK && _isAwake && !_isDecidedEntry && !_isCanceledEntry && IsEnableModeSelectType() && !_isRight && !_isLeft && !_isGrayOKButton)
			{
				_buttonController.SetAnimationActive(animationActive);
				_isOK = true;
			}
		}

		public void TimeSkipButtonAnim(InputManager.ButtonSetting button)
		{
			int animationActive = 1;
			if (_isCheckTimeSkip && !_isTimeSkip && _isAwake && !_isDecidedEntry && !_isCanceledEntry && !_isInvisibleTimeSkip)
			{
				_buttonController.SetAnimationActive(animationActive);
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
			if (_isCheckLR && !_isRight && !_isLeft && _isAwake && !_isDecidedEntry && !_isCanceledEntry)
			{
				_buttonController.SetAnimationActive(animationActive);
				_isRight = true;
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
			if (_isCheckLR && !_isRight && !_isLeft && _isAwake && !_isDecidedEntry && !_isCanceledEntry)
			{
				_buttonController.SetAnimationActive(animationActive);
				_isLeft = true;
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

		public void SettingButtonAnim(InputManager.ButtonSetting button)
		{
			int animationActive = 8;
			if (_isCheckSetting && _isAwake && !_isDecidedEntry && !_isCanceledEntry)
			{
				if (!_isSetting)
				{
					_buttonController.SetAnimationActive(animationActive);
					_isSetting = true;
				}
				else
				{
					_buttonController.SetAnimationActive(animationActive);
					_isSetting = false;
				}
			}
		}

		public void ToggleButtonAnim(InputManager.TouchPanelArea button)
		{
			int num = 0;
			switch (button)
			{
			case InputManager.TouchPanelArea.A1:
			case InputManager.TouchPanelArea.D2:
				num = 0;
				break;
			case InputManager.TouchPanelArea.E2:
				num = 1;
				break;
			case InputManager.TouchPanelArea.B2:
				num = 2;
				break;
			case InputManager.TouchPanelArea.B3:
				num = 3;
				break;
			}
			if (_callSettingToggleID[num] == -1 && _isAwake && !_isDecidedEntry && !_isCanceledEntry && _isSettingEnable[num])
			{
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(_monitorID);
				if (_isSettingToggleState[num])
				{
					_animSetting[num].Play("On_Off", 0, 0f);
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, _monitorID);
					_isSettingToggleState[num] = false;
				}
				else
				{
					_animSetting[num].Play("Off_On", 0, 0f);
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, _monitorID);
					_isSettingToggleState[num] = true;
				}
				switch (num)
				{
				case 0:
					userData.Extend.ExtendContendBit.SetFlag(ExtendContentBitID.GotoCodeRead, _isSettingToggleState[num]);
					break;
				case 1:
					userData.Extend.ExtendContendBit.SetFlag(ExtendContentBitID.GotoIconPhotoShoot, _isSettingToggleState[num]);
					break;
				case 2:
					userData.Extend.ExtendContendBit.SetFlag(ExtendContentBitID.GotoCharaSelect, _isSettingToggleState[num]);
					break;
				case 3:
					userData.Extend.ExtendContendBit.SetFlag(ExtendContentBitID.PhotoAgree, _isSettingToggleState[num]);
					break;
				}
				_callSettingToggleID[num] = num;
				_toggleWaitCount[num] = 0;
			}
		}

		public void VolumeButtonAnim(InputManager.TouchPanelArea button, bool isHold = false)
		{
			Animator animator = null;
			int num = 0;
			switch (button)
			{
			case InputManager.TouchPanelArea.E6:
				num = 0;
				break;
			case InputManager.TouchPanelArea.E4:
				num = 1;
				break;
			}
			if (_callSettingVolumeID != -1 || !_isAwake || _isDecidedEntry || _isCanceledEntry)
			{
				return;
			}
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(_monitorID);
			bool flag = true;
			switch (num)
			{
			case 0:
				animator = _settingVolumeMinus.GetComponent<Animator>();
				if (_volumeValue <= 5)
				{
					flag = false;
				}
				_volumeValue -= 5;
				if (_volumeValue < 0)
				{
					_volumeValue = 0;
				}
				break;
			case 1:
				animator = _settingVolumePlus.GetComponent<Animator>();
				if (_volumeValue >= 100)
				{
					flag = false;
				}
				_volumeValue += 5;
				if (_volumeValue > 100)
				{
					_volumeValue = 100;
				}
				break;
			}
			int num2 = 0;
			num2 = _volumeValue - 5;
			num2 /= 5;
			if (num2 <= 0)
			{
				num2 = 0;
			}
			if (num2 >= 19)
			{
				num2 = 19;
			}
			userData.Option.HeadPhoneVolume = (OptionHeadphonevolumeID)num2;
			SoundManager.SetHeadPhoneVolume(_monitorID, userData.Option.HeadPhoneVolume.GetValue());
			if (flag)
			{
				if (isHold)
				{
					animator.Play("Hold", 0, 0f);
				}
				else
				{
					animator.Play("Press", 0, 0f);
				}
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, _monitorID);
				_isHoldVolume = isHold;
			}
			_callSettingVolumeID = num;
			_volumeWaitCount = 0;
		}

		public void SelectButtonAnim(InputManager.ButtonSetting button)
		{
			if (_isAwake && !_isDecidedEntry && !_isCanceledEntry && !_isBlockingDecideBalance && !_isBalanceNetworkDisconnection && GotoBalance() && AMDaemon.EMoney.IsAuthCompleted && AMDaemon.EMoney.Operation.CanOperateDeal && !_isSelectButton)
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

		public void OKStart(bool isChangeSE = false)
		{
			if (!_isEnableOK)
			{
				_isEnableOK = true;
				_buttonController.SetVisible(true, default(int));
				_buttonController.ChangeBottonSE(ButtonControllerBase.FlatButtonType.Ok, isChangeSE);
				_isCheckOK = true;
			}
		}

		public void ResetOKStart()
		{
			if (_isEnableOK)
			{
				_buttonController.SetVisible(false, default(int));
				_buttonController.ChangeBottonSE(ButtonControllerBase.FlatButtonType.Ok);
			}
			_isEnableOK = false;
			_isCheckOK = false;
			_isOK = false;
		}

		public void TimeSkipStart(bool isInvisible = false)
		{
			if (!_isEnableTimeSkip)
			{
				_isEnableTimeSkip = true;
				if (!isInvisible)
				{
					_buttonController.SetVisible(true, 1);
				}
				_isInvisibleTimeSkip = isInvisible;
				_isCheckTimeSkip = true;
			}
		}

		public void ResetTimeSkipStart(bool isInvisible = false)
		{
			if (_isEnableTimeSkip)
			{
				if (!isInvisible)
				{
					_buttonController.SetVisible(false, 1);
				}
				_isInvisibleTimeSkip = isInvisible;
			}
			_isEnableTimeSkip = false;
			_isCheckTimeSkip = false;
			_isTimeSkip = false;
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

		public int GetMiniSelectorTexID(int id, bool enable = false)
		{
			int result = 0;
			switch (id)
			{
			case 0:
				result = 0;
				break;
			case 1:
				result = (enable ? 1 : 2);
				break;
			case 2:
				result = ((!enable) ? ((_isEntryNum != 2) ? 4 : 5) : 3);
				break;
			}
			return result;
		}

		public int GetSelectorCreditNum(int id, bool isNewAime = false)
		{
			int result = 1;
			if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.IsFreePlay())
			{
				result = 0;
			}
			else if (!isNewAime)
			{
				switch (id)
				{
				case 0:
					result = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.GameCostEnoughPlay();
					break;
				case 1:
					result = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.GameCostEnoughPlay();
					break;
				case 2:
					result = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.GameCostEnoughFreedom();
					break;
				}
			}
			else
			{
				result = 0;
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

		public int GetBigSelectorTexID(int id)
		{
			int result = 0;
			switch (id)
			{
			case 0:
				result = 1;
				break;
			case 1:
				result = 2;
				break;
			case 2:
				result = 0;
				break;
			}
			return result;
		}

		public void SetModeSelectTypeDisable(int id)
		{
			int num = -1;
			switch (id)
			{
			case 0:
				num = 0;
				break;
			case 1:
				num = 1;
				break;
			case 2:
				num = 2;
				break;
			}
			if (num >= 0)
			{
				_modeSelectTypeEnable[num] = false;
			}
		}

		public bool IsToggleAnim()
		{
			bool result = false;
			for (int i = 0; i < _toggleMax; i++)
			{
				if (_callSettingToggleID[i] != -1)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public bool IsToggleAnim(int id)
		{
			bool result = false;
			int num = id;
			if (num >= _toggleMax)
			{
				num = _toggleMax - 1;
			}
			if (num < 0)
			{
				num = 0;
			}
			if (_callSettingToggleID[num] != -1)
			{
				result = true;
			}
			return result;
		}

		public bool IsVolumeAnim(int id)
		{
			bool result = false;
			int num = id;
			if (num >= 2)
			{
				num = 1;
			}
			if (num < 0)
			{
				num = 0;
			}
			if (_callSettingVolumeID != -1)
			{
				result = true;
			}
			switch (num)
			{
			case 0:
				if (_volumeValue <= 5)
				{
					result = true;
				}
				break;
			case 1:
				if (_volumeValue >= 100)
				{
					result = true;
				}
				break;
			}
			return result;
		}

		public GameObject GetSettingBlurObject()
		{
			int index = 8;
			if (_isChangeSettingSibling)
			{
				index = 3;
			}
			return _masterNull.transform.GetChild(index).gameObject;
		}

		public void SetSettingBlur()
		{
			GameObject settingBlurObject = GetSettingBlurObject();
			settingBlurObject.SetActive(value: true);
			CanvasGroup component = settingBlurObject.GetComponent<CanvasGroup>();
			if (component != null)
			{
				component.alpha = 1f;
			}
		}

		public void ResetSettingBlur()
		{
			GameObject settingBlurObject = GetSettingBlurObject();
			settingBlurObject.SetActive(value: false);
			CanvasGroup component = settingBlurObject.GetComponent<CanvasGroup>();
			if (component != null)
			{
				component.alpha = 0f;
			}
		}

		public void SetLastSettingBlur()
		{
			GameObject settingBlurObject = GetSettingBlurObject();
			int siblingIndex = settingBlurObject.transform.GetSiblingIndex();
			settingBlurObject.transform.SetSiblingIndex(siblingIndex + 2);
			settingBlurObject.SetActive(value: true);
			CanvasGroup component = settingBlurObject.GetComponent<CanvasGroup>();
			if (component != null)
			{
				component.alpha = 1f;
			}
		}

		public GameObject GetBlurObject()
		{
			int index = 8;
			if (_isChangeSibling)
			{
				index = 4;
			}
			return _masterNull.transform.GetChild(index).gameObject;
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

		public void SetMiniSelector()
		{
			MultipleImage multipleImage = null;
			int num = 0;
			int num2 = 1;
			string text = "";
			bool flag = false;
			for (int i = 0; i < _dispCardMax; i++)
			{
				num2 = GetSelectorCreditNum(_objModeSelectTypeListCard[i], _isNewAime);
				flag = _modeSelectTypeEnable[_objModeSelectTypeListCard[i]];
				text = ((!flag) ? ("<color=#a9a9a9>" + num2.ToString("0") + "</color>") : ("<color=#ffffff>" + num2.ToString("0") + "</color>"));
				multipleImage = _nullListMiniCard[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<MultipleImage>();
				if (multipleImage != null)
				{
					num = GetMiniSelectorTexID(_objModeSelectTypeListCard[i], flag);
					multipleImage.ChangeSprite(num);
				}
				_nullListMiniCard[i].transform.GetChild(0).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = text;
			}
		}

		public void SetBigSelector()
		{
			MultipleImage multipleImage = null;
			int num = 0;
			int num2 = 1;
			string text = "";
			for (int i = 0; i < _dispCardMax; i++)
			{
				num2 = GetSelectorCreditNum(_objModeSelectTypeListCard[i], _isNewAime);
				_ = _modeSelectTypeEnable[_objModeSelectTypeListCard[i]];
				text = num2.ToString("0");
				multipleImage = _nullListBigCard[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<MultipleImage>();
				if (multipleImage != null)
				{
					num = GetBigSelectorTexID(_objModeSelectTypeListCard[i]);
					multipleImage.ChangeSprite(num);
				}
				_nullListBigCard[i].transform.GetChild(0).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = text;
			}
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
			base.Initialize(_monitorID, active);
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(_monitorID);
			if (userData.IsEntry)
			{
				MechaManager.LedIf[monIndex].ButtonLedReset();
			}
			else
			{
				Color item = new Color(0f, 0f, 0f, 1f);
				int num = MechaManager.LedIf.Length;
				int num2 = 0;
				int num3 = 0;
				int num4 = num;
				switch (_monitorID)
				{
				case 0:
					num3 = 0;
					num4 = num / 2;
					break;
				case 1:
					num3 = num / 2;
					num4 = num;
					break;
				}
				num4--;
				if (num4 < 0)
				{
					num4 = 0;
				}
				List<IO.Jvs.LedPwmFadeParam> list = new List<IO.Jvs.LedPwmFadeParam>();
				List<Color> list2 = new List<Color> { item, item };
				for (int i = 0; i < list2.Count; i++)
				{
					list.Add(new IO.Jvs.LedPwmFadeParam
					{
						StartFadeColor = list2[i],
						EndFadeColor = list2[(i + 1) % list2.Count],
						FadeTime = 4000L,
						NextIndex = (i + 1) % list2.Count
					});
				}
				Bd15070_4IF[] ledIf = MechaManager.LedIf;
				foreach (Bd15070_4IF bd15070_4IF in ledIf)
				{
					if (num2 >= num3 && num2 <= num4)
					{
						bd15070_4IF.SetColorMultiAutoFade(list);
					}
					num2++;
				}
			}
			_isEntry = userData.IsEntry;
			_isGuest = userData.IsGuest();
			_isEntryNum = 1;
			_isDecidedEntryNum = 0;
			_isCanceledEntryNum = 0;
			_active_mode = ModeSelectProcess.ProccessActiveMode.Individual;
			_timer = 0;
			_sub_count = 0;
			_baseBigPosY = 84f;
			_isDispInfoWindow = false;
			_isChangeSibling = false;
			_info_state = InfoWindowState.None;
			_info_timer = 0u;
			_info_count = 0u;
			for (int k = 0; k < 3; k++)
			{
				_modeSelectBaseType[k] = k;
			}
			GameObject gameObject = null;
			GameObject gameObject2 = null;
			GameObject gameObject3 = null;
			Table table = Table.None;
			int monitorID = _monitorID;
			int num5 = 0;
			if (_isEntry)
			{
				List<int> selectedCardList = userData.Extend.SelectedCardList;
				if (selectedCardList.Count > 0 && selectedCardList[0] != 0)
				{
					num5 = selectedCardList[0];
				}
				CodeReadProcess.CardStatus status;
				UserCard userCard = CodeReadProcess.GetUserCard(monitorID, num5, out status);
				bool flag = false;
				if (userCard != null)
				{
					flag = CodeReadProcess.IsInPeriod(userCard.startDate, userCard.endDataDate) && status == CodeReadProcess.CardStatus.Normal;
				}
				Singleton<UserDataManager>.Instance.GetUserData(monitorID).Detail.CardType = ((!flag) ? 1 : num5);
				if (userCard == null || status == CodeReadProcess.CardStatus.Unowned)
				{
					Singleton<UserDataManager>.Instance.GetUserData(monitorID).Detail.CardType = 0;
				}
			}
			table = (Table)userData.Detail.CardType;
			bool flag2 = false;
			int num6 = -1;
			switch (table)
			{
			case Table.FreedomPass:
				flag2 = true;
				num6 = 0;
				break;
			case Table.GoldPass:
				flag2 = true;
				num6 = 1;
				break;
			case Table.OutOfEffect:
				flag2 = true;
				num6 = 2;
				break;
			}
			if (flag2)
			{
				_cardPassIcon.SetActive(value: true);
				SetActiveChildren(_cardPassIcon, active: false);
				if (num6 != -1)
				{
					_cardPassIcon.transform.GetChild(num6).gameObject.SetActive(value: true);
				}
			}
			else
			{
				_cardPassIcon.SetActive(value: false);
			}
			_ = _masterNullSelector.gameObject.transform.parent.gameObject;
			gameObject = _masterNull.gameObject.transform.parent.gameObject;
			gameObject3 = _masterNullSelector.transform.GetChild(0).GetChild(1).gameObject;
			for (int l = 0; l < _dispCardMax; l++)
			{
				_nullListMiniCard.Add(gameObject3.transform.GetChild(l).gameObject);
			}
			int num7 = _dispCardMax / 2;
			for (int m = 0; m < _dispCardMax; m++)
			{
				int num8 = m - num7;
				num8 %= _modeSelectBaseType.Length;
				if (num8 < 0)
				{
					num8 = _modeSelectBaseType.Length - num8 * -1;
					num8 %= _modeSelectBaseType.Length;
				}
				_objModeSelectTypeListCard.Add(_modeSelectBaseType[num8]);
			}
			SetMiniSelector();
			gameObject2 = _masterNullSelector.transform.GetChild(0).GetChild(2).gameObject;
			for (int n = 0; n < _dispCardMax; n++)
			{
				_nullListBigCard.Add(gameObject2.transform.GetChild(n).gameObject);
			}
			for (int num9 = 0; num9 < _dispCardMax; num9++)
			{
				_nullListBigCard[num9].transform.GetChild(0).GetChild(0).GetChild(0)
					.gameObject.SetActive(value: false);
			}
			int index = _dispCardMax / 2;
			_nullListBigCard[index].transform.GetChild(0).GetChild(0).GetChild(0)
				.gameObject.SetActive(value: true);
			SetBigSelector();
			_settingWindow.transform.parent.gameObject.SetActive(value: true);
			_settingWindow.SetActive(value: true);
			for (int num10 = 0; num10 < _toggleMax; num10++)
			{
				_callSettingToggleID[num10] = -1;
				_toggleWaitCount[num10] = 0;
			}
			_callSettingVolumeID = -1;
			_volumeWaitCount = 0;
			_settingWindow.GetComponent<Animator>().Play("In", 0, 1f);
			for (int num11 = 0; num11 < _toggleMax; num11++)
			{
				_nullSetting.Add(_settingWindow.transform.GetChild(0).GetChild(num11 + 2).gameObject);
				_isSettingToggleState.Add(item: true);
				_isSettingEnable.Add(item: true);
			}
			for (int num12 = 0; num12 < _toggleMax; num12++)
			{
				_animSetting.Add(_nullSetting[num12].transform.GetChild(0).GetChild(4).gameObject.GetComponent<Animator>());
			}
			bool value = userData.Extend.ExtendContendBit.IsFlagOn(ExtendContentBitID.GotoCodeRead);
			bool value2 = userData.Extend.ExtendContendBit.IsFlagOn(ExtendContentBitID.GotoIconPhotoShoot);
			bool value3 = userData.Extend.ExtendContendBit.IsFlagOn(ExtendContentBitID.PhotoAgree);
			bool value4 = userData.Extend.ExtendContendBit.IsFlagOn(ExtendContentBitID.GotoCharaSelect);
			int num13 = 0;
			num13 = (int)userData.Option.HeadPhoneVolume;
			if (num13 <= 0)
			{
				num13 = 0;
			}
			if (num13 >= 19)
			{
				num13 = 19;
			}
			userData.Option.HeadPhoneVolume = (OptionHeadphonevolumeID)num13;
			_volumeValue = num13 * 5 + 5;
			SoundManager.SetHeadPhoneVolume(_monitorID, userData.Option.HeadPhoneVolume.GetValue());
			_isSettingToggleState[0] = value;
			_isSettingEnable[0] = true;
			_isSettingToggleState[1] = value2;
			_isSettingEnable[1] = true;
			_isSettingToggleState[2] = value4;
			_isSettingEnable[2] = true;
			_isSettingToggleState[3] = value3;
			_isSettingEnable[3] = true;
			bool flag3 = false;
			for (int num14 = 0; num14 < _toggleMax; num14++)
			{
				flag3 = _isSettingEnable[num14];
				_nullSetting[num14].transform.GetChild(0).GetChild(5).gameObject.SetActive(!flag3);
				if (_isSettingToggleState[num14])
				{
					_animSetting[num14].Play("On_Loop", 0, 0f);
				}
				else
				{
					_animSetting[num14].Play("Off_Loop", 0, 0f);
				}
				_nullSetting[num14].transform.GetChild(0).GetChild(4).GetChild(2)
					.gameObject.SetActive(value: false);
			}
			for (int num15 = 0; num15 < _toggleMax; num15++)
			{
				flag3 = _isSettingEnable[num15];
				if (_isSettingToggleState[num15])
				{
					_settingMiniWindow.transform.GetChild(num15).GetChild(0).GetChild(0)
						.gameObject.SetActive(value: true);
					_settingMiniWindow.transform.GetChild(num15).GetChild(0).GetChild(1)
						.gameObject.SetActive(value: false);
				}
				else
				{
					_settingMiniWindow.transform.GetChild(num15).GetChild(0).GetChild(0)
						.gameObject.SetActive(value: false);
					_settingMiniWindow.transform.GetChild(num15).GetChild(0).GetChild(1)
						.gameObject.SetActive(value: true);
				}
			}
			float fillAmount = (float)_volumeValue / 100f;
			_headphoneVolume.GetComponent<Image>().fillAmount = fillAmount;
			string text = (_volumeValue / 5).ToString("0");
			_headphoneVolumeDigit.GetComponent<TextMeshProUGUI>().text = text;
			_settingMiniWindow.transform.GetChild(4).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = text;
			_creditWindow.SetActive(value: false);
			_emoneyWindow.SetActive(value: false);
			_entryInfoWindow.SetActive(value: false);
			_messageWindow.SetActive(value: false);
			_balanceCommon.Initialize(_creditWindow, _emoneyWindow, _buttonController);
			_balanceCommon.SetMode(BalanceCommon.ParentMode.ParentModeModeSelect, this, null);
			if (_isEntry)
			{
				_isAwake = true;
				switch (Singleton<UserDataManager>.Instance.GetUserData(_monitorID).UserType)
				{
				case UserData.UserIDType.New:
					_isNewAime = true;
					break;
				}
			}
			if (!_isAwake)
			{
				for (int num16 = 0; num16 < gameObject.transform.childCount; num16++)
				{
					gameObject.transform.GetChild(num16).gameObject.SetActive(value: false);
				}
				_settingWindow.SetActive(value: false);
				_state = MonitorState.End;
				return;
			}
			if (!Singleton<UserDataManager>.Instance.GetUserData(_monitorID).Detail.ContentBit.IsFlagOn(ContentBitID.FirstLoginBonus))
			{
				_isInvisibleButtonDispInfoWindow = true;
			}
			for (int num17 = 0; num17 < _dispCardMax; num17++)
			{
				CommonValue item2 = new CommonValue();
				_valueListMiniCardPosX.Add(item2);
			}
			for (int num18 = 0; num18 < _dispCardMax; num18++)
			{
				CommonValue item3 = new CommonValue();
				_valueListMiniCardScaleXY.Add(item3);
			}
			for (int num19 = 0; num19 < _dispCardMax; num19++)
			{
				CommonValue item4 = new CommonValue();
				_valueListBigCardPosX.Add(item4);
			}
			for (int num20 = 0; num20 < _dispCardMax; num20++)
			{
				CommonValue item5 = new CommonValue();
				_valueListBigCardScaleXY.Add(item5);
			}
			_buttonController.Initialize(_monitorID);
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
			GameObject obj3 = _buttonController.transform.GetChild(10).GetChild(0).GetChild(0)
				.gameObject;
			obj3.SetActive(value: false);
			image = obj3.transform.GetChild(0).gameObject.GetComponent<Image>();
			if (image != null)
			{
				image.preserveAspect = true;
				image.SetNativeSize();
			}
			obj3.SetActive(value: true);
			_buttonController.SetVisible(false, 3);
			_buttonController.SetVisible(false, 4);
			_buttonController.SetVisible(false, default(int));
			_buttonController.SetVisible(false, 1);
			_buttonController.SetVisible(false, 2);
			_buttonController.SetVisible(false, 8);
			_buttonController.SetVisibleImmediate(false, 5);
			_buttonController.SetVisibleImmediate(false, 6);
			_buttonController.SetVisible(false, 7);
			_messageWindow.transform.GetChild(0).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "";
			_state = MonitorState.None;
		}

		public void SetGotoParam()
		{
			if (_isAwake && _isDecidedEntry)
			{
				GameManager.IsGotoCodeRead |= _isSettingToggleState[0];
				GameManager.IsGotoPhotoShoot |= _isSettingToggleState[1];
				GameManager.IsGotoCharacterSelect |= _isSettingToggleState[2];
				GameManager.IsPhotoAgree |= _isSettingToggleState[3];
			}
		}

		private void SetCurrentHeadPhoneVolume()
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(_monitorID);
			int num = 0;
			num = (int)userData.Option.HeadPhoneVolume;
			if (num <= 0)
			{
				num = 0;
			}
			if (num >= 19)
			{
				num = 19;
			}
			userData.Option.HeadPhoneVolume = (OptionHeadphonevolumeID)num;
			_volumeValue = num * 5 + 5;
			float fillAmount = (float)_volumeValue / 100f;
			_headphoneVolume.GetComponent<Image>().fillAmount = fillAmount;
			string text = (_volumeValue / 5).ToString("0");
			_headphoneVolumeDigit.GetComponent<TextMeshProUGUI>().text = text;
			_settingMiniWindow.transform.GetChild(4).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = text;
		}

		public void SetNextState(MonitorState _next, bool _forced = false)
		{
			switch (_active_mode)
			{
			case ModeSelectProcess.ProccessActiveMode.Common:
				if (_forced)
				{
					_state = _next;
				}
				break;
			default:
				_state = _next;
				break;
			}
		}

		public void ChangeOKButtonColor()
		{
			if (_state <= MonitorState.CreditJudgeInit)
			{
				Image image = null;
				if (_isGrayOKButton && IsEnableModeSelectType())
				{
					image = _buttonController.transform.GetChild(4).GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
					if (image != null)
					{
						image.color = new Color(1f, 1f, 1f, 1f);
					}
					_isGrayOKButton = false;
					_buttonController.ChangeOKButtonColor(_isGrayOKButton);
				}
				else if (!_isGrayOKButton && !IsEnableModeSelectType())
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
			else
			{
				if (!_isRequestChangeOKButtonColor)
				{
					return;
				}
				Image image2 = null;
				if (_isSetGrayOKButton)
				{
					if (!_isGrayOKButton && !_isGrayOKButton)
					{
						image2 = _buttonController.transform.GetChild(4).GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
						if (image2 != null)
						{
							image2.color = new Color(0.5019608f, 0.5019608f, 0.5019608f, 1f);
						}
						_isGrayOKButton = true;
						_buttonController.ChangeOKButtonColor(_isGrayOKButton);
					}
				}
				else if (!_isSetGrayOKButton)
				{
					if (_isGrayOKButton)
					{
						image2 = _buttonController.transform.GetChild(4).GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
						if (image2 != null)
						{
							image2.color = new Color(1f, 1f, 1f, 1f);
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
			HeadphoneVolumeViewUpdate(GameManager.GetGameMSecAdd());
			if (_active_mode != 0)
			{
				_ = 1;
				if (_timer > 0)
				{
					_timer--;
				}
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
				SetNextState(MonitorState.Init);
				break;
			case MonitorState.Init:
				_settingWindow.SetActive(value: false);
				_isCheckOK = false;
				_isCheckLR = false;
				_isCheckSetting = false;
				_masterNull.GetComponent<Animator>().Play("In", 0, 0f);
				_nullListMiniCard[0].SetActive(value: false);
				_nullListMiniCard[4].SetActive(value: false);
				SetNextState(MonitorState.SelectCardFadeInJudge);
				break;
			case MonitorState.SelectCardFadeInJudge:
			{
				_buttonController.SetVisibleImmediate(false, 5);
				_buttonController.SetVisibleImmediate(false, 6);
				bool flag = Singleton<CourseManager>.Instance.FirstSinRankAdd(_monitorID);
				if (!Singleton<UserDataManager>.Instance.GetUserData(_monitorID).Detail.ContentBit.IsFlagOn(ContentBitID.FirstModeSelect))
				{
					_windowMessageList.Add(WindowMessageID.ModeSelectFirst);
					if (flag)
					{
						_windowMessageList.Add(WindowMessageID.ModeSelectSinRankAdd);
					}
					_isDispInfoWindow = true;
					_info_state = InfoWindowState.None;
					_info_timer = 0u;
					_state = MonitorState.SelectCardFadeInJudgeWait;
				}
				else if (flag)
				{
					_windowMessageList.Add(WindowMessageID.ModeSelectSinRankAdd);
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
			}
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
					_state = MonitorState.SelectCardFadeInSync;
				}
				break;
			case MonitorState.SelectCardFadeInSync:
				if (_timer == 0)
				{
					_state = MonitorState.SelectCardFadeInSyncWait;
				}
				break;
			case MonitorState.SelectCardWaitInit:
				if (_timer == 0)
				{
					SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000263, _monitorID);
					_buttonController.SetVisibleImmediate(false, 5);
					_buttonController.SetVisibleImmediate(false, 6);
					_buttonController.SetVisible(true, 3);
					_buttonController.SetVisibleFlip(true, true, 4);
					_buttonController.SetVisible(false, 1);
					_buttonController.SetVisible(true, 8);
					_isCheckLR = true;
					_isCheckSetting = true;
					_isCheckTimeSkip = true;
					_isInvisibleTimeSkip = false;
					_buttonController.ChangeButtonType(ButtonControllerBase.FlatButtonType.Ok);
					OKStart();
					TimeSkipStart(isInvisible: true);
					SetNextState(MonitorState.SelectCardWait);
					_sub_count = 0;
				}
				break;
			case MonitorState.SelectCardWait:
				if (_isOK || _isTimeUp)
				{
					SetNextState(MonitorState.CreditJudgeInit);
					if (_isOK)
					{
						int index5 = _dispCardMax / 2;
						bool num16 = _modeSelectTypeEnable[_objModeSelectTypeListCard[index5]];
						_isDecidedOK = true;
						if (num16)
						{
							_decidedSelectModeType = _objModeSelectTypeListCard[index5];
						}
					}
					else if (_isTimeUp)
					{
						_isDecidedOK = false;
						_isDecidedEntry = false;
						_isCanceledEntry = true;
						_decidedSelectModeType = -1;
						SetNextState(MonitorState.EntryResetInit);
						if (!_isCallBlur)
						{
							_isCallBlur = true;
						}
					}
					_buttonController.SetVisible(false, 3);
					_buttonController.SetVisible(false, 4);
					_buttonController.SetVisible(false, 8);
					_isCheckLR = false;
					_isRight = false;
					_isLeft = false;
					_isCheckOK = false;
					_isTimeSkip = false;
					_isCheckSetting = false;
					_timer = 60;
					_nullListMiniCard[0].SetActive(value: false);
					_nullListMiniCard[4].SetActive(value: false);
					ResetOKStart();
					ResetTimeSkipStart(isInvisible: true);
					break;
				}
				if (_isSetting)
				{
					_settingWindow.transform.parent.gameObject.SetActive(value: true);
					_settingWindow.SetActive(value: true);
					_settingWindow.GetComponent<Animator>().Play("In", 0, 0f);
					_buttonController.ChangeButtonType(ButtonControllerBase.FlatButtonType.SettingBack);
					_isCheckLR = false;
					_isCheckSetting = false;
					_buttonController.SetVisible(false, 3);
					_buttonController.SetVisible(false, 4);
					_nullListMiniCard[0].SetActive(value: false);
					_nullListMiniCard[4].SetActive(value: false);
					ResetOKStart();
					SetCurrentHeadPhoneVolume();
					SetNextState(MonitorState.SettingOpen_SyncAnimWait);
					_isCheckSetting = false;
					break;
				}
				if (_isRight)
				{
					for (int num17 = 0; num17 < _dispCardMax; num17++)
					{
						CommonValue commonValue5 = _valueListMiniCardPosX[num17];
						CommonValue commonValue6 = _valueListMiniCardScaleXY[num17];
						commonValue5.start = _cardMiniPosX[num17];
						commonValue5.current = _cardMiniPosX[num17];
						commonValue6.start = _cardMiniScaleXY[num17];
						commonValue6.current = _cardMiniScaleXY[num17];
						if (num17 == 0)
						{
							commonValue5.end = _cardMiniPosX[num17];
							commonValue6.end = _cardMiniScaleXY[num17];
						}
						else
						{
							commonValue5.end = _cardMiniPosX[num17 - 1];
							commonValue6.end = _cardMiniScaleXY[num17 - 1];
						}
						commonValue5.diff = (commonValue5.end - commonValue5.start) / _cardMoveTime;
						commonValue6.diff = (commonValue6.end - commonValue6.start) / _cardMoveTime;
						_cardPosEnd = 0;
					}
					for (int num18 = 0; num18 < _dispCardMax; num18++)
					{
						CommonValue commonValue7 = _valueListBigCardPosX[num18];
						CommonValue commonValue8 = _valueListBigCardScaleXY[num18];
						commonValue7.start = _cardBigPosX[num18];
						commonValue7.current = _cardBigPosX[num18];
						commonValue8.start = _cardBigScaleXY[num18];
						commonValue8.current = _cardBigScaleXY[num18];
						if (num18 == 0)
						{
							commonValue7.end = _cardBigPosX[num18];
							commonValue8.end = _cardBigScaleXY[num18];
						}
						else
						{
							commonValue7.end = _cardBigPosX[num18 - 1];
							commonValue8.end = _cardBigScaleXY[num18 - 1];
						}
						commonValue7.diff = (commonValue7.end - commonValue7.start) / _cardMoveTime;
						commonValue8.diff = (commonValue8.end - commonValue8.start) / _cardMoveTime;
						_cardPosEnd = 0;
					}
					SetNextState(MonitorState.SelectCardMoveRight);
					_nullListMiniCard[4].SetActive(value: true);
				}
				else if (_isLeft)
				{
					for (int num19 = 0; num19 < _dispCardMax; num19++)
					{
						CommonValue commonValue9 = _valueListMiniCardPosX[num19];
						CommonValue commonValue10 = _valueListMiniCardScaleXY[num19];
						commonValue9.start = _cardMiniPosX[num19];
						commonValue9.current = _cardMiniPosX[num19];
						commonValue10.start = _cardMiniScaleXY[num19];
						commonValue10.current = _cardMiniScaleXY[num19];
						if (num19 == _dispCardMax - 1)
						{
							commonValue9.end = _cardMiniPosX[num19];
							commonValue10.end = _cardMiniScaleXY[num19];
						}
						else
						{
							commonValue9.end = _cardMiniPosX[num19 + 1];
							commonValue10.end = _cardMiniScaleXY[num19 + 1];
						}
						commonValue9.diff = (commonValue9.end - commonValue9.start) / _cardMoveTime;
						commonValue10.diff = (commonValue10.end - commonValue10.start) / _cardMoveTime;
						_cardPosEnd = 0;
					}
					for (int num20 = 0; num20 < _dispCardMax; num20++)
					{
						CommonValue commonValue11 = _valueListBigCardPosX[num20];
						CommonValue commonValue12 = _valueListBigCardScaleXY[num20];
						commonValue11.start = _cardBigPosX[num20];
						commonValue11.current = _cardBigPosX[num20];
						commonValue12.start = _cardBigScaleXY[num20];
						commonValue12.current = _cardBigScaleXY[num20];
						if (num20 == _dispCardMax - 1)
						{
							commonValue11.end = _cardBigPosX[num20];
							commonValue12.end = _cardBigScaleXY[num20];
						}
						else
						{
							commonValue11.end = _cardBigPosX[num20 + 1];
							commonValue12.end = _cardBigScaleXY[num20 + 1];
						}
						commonValue11.diff = (commonValue11.end - commonValue11.start) / _cardMoveTime;
						commonValue12.diff = (commonValue12.end - commonValue12.start) / _cardMoveTime;
						_cardPosEnd = 0;
					}
					SetNextState(MonitorState.SelectCardMoveLeft);
					_nullListMiniCard[0].SetActive(value: true);
				}
				if (!_isRight && !_isLeft)
				{
				}
				break;
			case MonitorState.SelectCardMoveRight:
			case MonitorState.SelectCardMoveLeft:
			{
				if (_sub_count < 60)
				{
					_sub_count++;
				}
				_cardPosEnd = 0;
				for (int i = 0; i < _dispCardMax; i++)
				{
					CommonValue commonValue = _valueListMiniCardPosX[i];
					CommonValue commonValue2 = _valueListMiniCardScaleXY[i];
					if (commonValue.UpdateValue())
					{
						_cardPosEnd++;
					}
					commonValue2.UpdateValue();
					_nullListMiniCard[i].transform.localPosition = new Vector3(commonValue.current, 0f, 0f);
					_nullListMiniCard[i].transform.localScale = new Vector3(commonValue2.current, commonValue2.current, 1f);
				}
				for (int j = 0; j < _dispCardMax; j++)
				{
					CommonValue commonValue3 = _valueListBigCardPosX[j];
					CommonValue commonValue4 = _valueListBigCardScaleXY[j];
					commonValue3.UpdateValue();
					commonValue4.UpdateValue();
					_nullListBigCard[j].transform.localPosition = new Vector3(commonValue3.current, _baseBigPosY, 0f);
					_nullListBigCard[j].transform.localScale = new Vector3(commonValue4.current, commonValue4.current, 1f);
				}
				if (_cardPosEnd == _dispCardMax)
				{
					for (int k = 0; k < _dispCardMax; k++)
					{
						_nullListMiniCard[k].transform.localPosition = new Vector3(_cardMiniPosX[k], 0f, 0f);
						_nullListMiniCard[k].transform.localScale = new Vector3(_cardMiniScaleXY[k], _cardMiniScaleXY[k], 1f);
					}
					for (int l = 0; l < _dispCardMax; l++)
					{
						_nullListBigCard[l].transform.localPosition = new Vector3(_cardBigPosX[l], _baseBigPosY, 0f);
						_nullListBigCard[l].transform.localScale = new Vector3(_cardBigScaleXY[l], _cardBigScaleXY[l], 1f);
					}
					for (int m = 0; m < _dispCardMax; m++)
					{
						_nullListBigCard[m].transform.GetChild(0).GetChild(0).GetChild(0)
							.gameObject.SetActive(value: false);
					}
					int index = _dispCardMax / 2;
					_nullListBigCard[index].transform.GetChild(0).GetChild(0).GetChild(0)
						.gameObject.SetActive(value: true);
					_nullListBigCard[index].transform.GetChild(0).GetChild(0).GetChild(0)
						.GetComponent<Image>()
						.color = new Color(1f, 1f, 1f, 1f);
					if (_isRight)
					{
						int num = _dispCardMax - 1;
						int index2 = _dispCardMax - 1;
						int num2 = _objModeSelectTypeListCard[index2];
						num2++;
						num2 %= _modeSelectBaseType.Length;
						for (int n = 0; n < num; n++)
						{
							_objModeSelectTypeListCard[n] = _objModeSelectTypeListCard[n + 1];
						}
						_objModeSelectTypeListCard[index2] = num2;
						SetMiniSelector();
						SetBigSelector();
					}
					else if (_isLeft)
					{
						int dispCardMax = _dispCardMax;
						int index3 = 0;
						int num3 = _objModeSelectTypeListCard[index3];
						num3 += _modeSelectBaseType.Length;
						num3--;
						num3 %= _modeSelectBaseType.Length;
						int num4 = _dispCardMax - 1;
						for (int num5 = 1; num5 < dispCardMax; num5++)
						{
							_objModeSelectTypeListCard[num4 - (num5 - 1)] = _objModeSelectTypeListCard[num4 - num5];
						}
						_objModeSelectTypeListCard[index3] = num3;
						SetMiniSelector();
						SetBigSelector();
					}
					SetNextState(MonitorState.SelectCardWaitSyncBoth, _forced: true);
					_isRight = false;
					_isLeft = false;
					_nullListMiniCard[0].SetActive(value: false);
					_nullListMiniCard[4].SetActive(value: false);
				}
				else
				{
					int num6 = _dispCardMax / 2;
					int num7 = num6;
					float num8 = 255f / _cardMoveTime;
					num8 = 255f - num8 * (float)_sub_count * 2.5f;
					if (num8 < 0f)
					{
						num8 = 0f;
					}
					num7 = num6;
					_nullListBigCard[num7].transform.GetChild(0).GetChild(0).GetChild(0)
						.GetComponent<Image>()
						.color = new Color(1f, 1f, 1f, num8 / 255f);
					float num9 = 0.3f;
					float num10 = 255f / (_cardMoveTime * num9);
					int num11 = (int)(_cardMoveTime * (1f - num9));
					num10 = ((_sub_count < num11) ? 0f : (0f + num10 * (float)(_sub_count - num11)));
					if (_isRight)
					{
						num7 = num6 + 1;
						_nullListBigCard[num7].transform.GetChild(0).GetChild(0).GetChild(0)
							.gameObject.SetActive(value: true);
						_nullListBigCard[num7].transform.GetChild(0).GetChild(0).GetChild(0)
							.GetComponent<Image>()
							.color = new Color(1f, 1f, 1f, num10 / 255f);
					}
					if (_isLeft)
					{
						num7 = num6 - 1;
						_nullListBigCard[num7].transform.GetChild(0).GetChild(0).GetChild(0)
							.gameObject.SetActive(value: true);
						_nullListBigCard[num7].transform.GetChild(0).GetChild(0).GetChild(0)
							.GetComponent<Image>()
							.color = new Color(1f, 1f, 1f, num10 / 255f);
					}
				}
				break;
			}
			case MonitorState.SelectCardWaitSyncBoth:
				SetNextState(MonitorState.SelectCardWait);
				break;
			case MonitorState.SettingOpen_SyncAnimWait:
			{
				Animator component6 = _settingWindow.GetComponent<Animator>();
				if (IsEndAnim(component6))
				{
					SetCurrentHeadPhoneVolume();
					Animator component7 = _settingVolumeMinus.GetComponent<Animator>();
					Animator component8 = _settingVolumePlus.GetComponent<Animator>();
					component7.Play("Loop", 0, 0f);
					component8.Play("Loop", 0, 0f);
					SetNextState(MonitorState.SettingOpen_SyncBothReached, _forced: true);
				}
				_isCheckSetting = false;
				break;
			}
			case MonitorState.SettingOpen_SyncBothReached:
			{
				for (int num13 = 0; num13 < _toggleMax; num13++)
				{
					_toggleWaitCount[num13] = 0;
				}
				_volumeWaitCount = 0;
				SetNextState(MonitorState.SettingWait);
				_isCheckSetting = true;
				break;
			}
			case MonitorState.SettingWait:
			{
				if (_isTimeUp || (!_isSetting && !IsToggleAnim()))
				{
					_isCheckOK = false;
					_isCheckLR = false;
					_settingWindow.GetComponent<Animator>().Play("Out", 0, 0f);
					if (_isTimeUp)
					{
						_buttonController.SetVisible(false, 8);
					}
					else
					{
						_buttonController.ChangeButtonType(ButtonControllerBase.FlatButtonType.Setting);
					}
					SetNextState(MonitorState.SettingClose_SyncAnimWait);
					_isCheckSetting = false;
				}
				else
				{
					for (int num14 = 0; num14 < _toggleMax; num14++)
					{
						if (_callSettingToggleID[num14] == -1)
						{
							continue;
						}
						int index4 = _callSettingToggleID[num14];
						Animator anim = _animSetting[index4];
						if (IsEndAnim(anim) && _toggleWaitCount[num14] >= 2)
						{
							if (_isSettingToggleState[num14])
							{
								_settingMiniWindow.transform.GetChild(num14).GetChild(0).GetChild(0)
									.gameObject.SetActive(value: true);
								_settingMiniWindow.transform.GetChild(num14).GetChild(0).GetChild(1)
									.gameObject.SetActive(value: false);
							}
							else
							{
								_settingMiniWindow.transform.GetChild(num14).GetChild(0).GetChild(0)
									.gameObject.SetActive(value: false);
								_settingMiniWindow.transform.GetChild(num14).GetChild(0).GetChild(1)
									.gameObject.SetActive(value: true);
							}
							_toggleWaitCount[num14] = 0;
							_callSettingToggleID[num14] = -1;
						}
					}
					if (_callSettingVolumeID != -1)
					{
						Animator anim2 = null;
						switch (_callSettingVolumeID)
						{
						case 0:
							anim2 = _settingVolumeMinus.GetComponent<Animator>();
							break;
						case 1:
							anim2 = _settingVolumePlus.GetComponent<Animator>();
							break;
						}
						if (_isHoldVolume || (IsEndAnim(anim2) && _volumeWaitCount >= 2))
						{
							float fillAmount = (float)_volumeValue / 100f;
							_headphoneVolume.GetComponent<Image>().fillAmount = fillAmount;
							string text2 = (_volumeValue / 5).ToString("0");
							_headphoneVolumeDigit.GetComponent<TextMeshProUGUI>().text = text2;
							_settingMiniWindow.transform.GetChild(4).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = text2;
							_volumeWaitCount = 0;
							_callSettingVolumeID = -1;
							Animator component4 = _settingVolumeMinus.GetComponent<Animator>();
							Animator component5 = _settingVolumePlus.GetComponent<Animator>();
							if (_isHoldVolume)
							{
								if (_volumeValue <= 5 || _volumeValue >= 100)
								{
									component4.Play("Loop", 0, 0f);
									component5.Play("Loop", 0, 0f);
									_isHoldVolume = false;
								}
							}
							else
							{
								component4.Play("Loop", 0, 0f);
								component5.Play("Loop", 0, 0f);
							}
						}
					}
				}
				for (int num15 = 0; num15 < _toggleMax; num15++)
				{
					if (_toggleWaitCount[num15] < 60)
					{
						_toggleWaitCount[num15]++;
					}
				}
				if (_volumeWaitCount < 60)
				{
					_volumeWaitCount++;
				}
				break;
			}
			case MonitorState.SettingClose_SyncAnimWait:
			{
				Animator component3 = _settingWindow.GetComponent<Animator>();
				if (IsEndAnim(component3))
				{
					SetNextState(MonitorState.SettingClose_SyncBothReached, _forced: true);
				}
				_isCheckSetting = false;
				break;
			}
			case MonitorState.SettingClose_SyncBothReached:
				_isCheckLR = true;
				_isCheckSetting = true;
				if (!_isTimeUp)
				{
					_buttonController.SetVisible(true, 3);
					_buttonController.SetVisibleFlip(true, true, 4);
					_buttonController.ChangeButtonType(ButtonControllerBase.FlatButtonType.Ok);
					OKStart();
					_buttonController.ChangeOKButtonColor(_isGrayOKButton);
				}
				SetNextState(MonitorState.SelectCardWait);
				_isCheckSetting = true;
				break;
			case MonitorState.CreditJudgeInit:
				if (_timer == 0)
				{
					_buttonController.SetVisible(false, default(int));
					_buttonController.SetVisible(false, 1);
					_buttonController.SetVisible(false, 8);
					_isOK = false;
					SetNextState(MonitorState.CreditJudge);
				}
				break;
			case MonitorState.CreditJudge:
				_isOK = false;
				_isCallBlur = true;
				_timer = 60;
				_active_mode = ModeSelectProcess.ProccessActiveMode.Individual;
				if (_needCreditNum > 0 || _needCreditNum == -1)
				{
					_state = MonitorState.InsertCoinFadeIn;
					break;
				}
				_state = MonitorState.DecideCreditFadeIn;
				if (_isTimeUp)
				{
					_isDecidedOK = false;
					_isDecidedEntry = false;
					_isCanceledEntry = true;
					ResetTimeSkipStart();
					_state = MonitorState.EntryResetInit;
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
				GameObject gameObject5 = null;
				_creditWindow.SetActive(value: true);
				_emoneyWindow.SetActive(value: false);
				gameObject5 = _creditWindow.transform.GetChild(0).GetChild(1).gameObject;
				gameObject5.SetActive(value: true);
				gameObject5 = _creditWindow.transform.GetChild(0).GetChild(2).gameObject;
				gameObject5.SetActive(value: false);
				_creditWindow.transform.GetChild(0).GetComponent<Animator>().Play("InsertCoin_In", 0, 0f);
				gameObject5 = _creditWindow.transform.GetChild(0).GetChild(1).gameObject;
				gameObject5.transform.GetChild(0).gameObject.GetComponent<Animator>().Play("Loop", 0, 0f);
				SetNeedCreditNum();
				gameObject5.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Animator>().Play("In", 0, 0f);
				gameObject5.transform.GetChild(3).gameObject.GetComponent<Animator>().Play("Out", 0, 1f);
				gameObject5.transform.GetChild(4).gameObject.GetComponent<Animator>().Play("Out", 0, 1f);
				if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsEnableEmoneyExecute)
				{
					if (GotoBalance() && AMDaemon.EMoney.IsAuthCompleted)
					{
						gameObject5.transform.GetChild(5).gameObject.SetActive(value: true);
						gameObject5.transform.GetChild(5).GetChild(1).gameObject.GetComponent<MultipleImage>().ChangeSprite(_monitorID);
						_isBalanceAuthCompleted = true;
					}
					else
					{
						gameObject5.transform.GetChild(5).gameObject.SetActive(value: false);
						gameObject5.transform.GetChild(5).GetChild(1).gameObject.GetComponent<MultipleImage>().ChangeSprite(_monitorID);
						_isBalanceAuthCompleted = false;
					}
				}
				else
				{
					gameObject5.transform.GetChild(5).gameObject.SetActive(value: false);
					gameObject5.transform.GetChild(5).GetChild(1).gameObject.GetComponent<MultipleImage>().ChangeSprite(_monitorID);
					_isBalanceAuthCompleted = false;
				}
				_state = MonitorState.InsertCoinFadeInWait;
				_timer = 2;
				_sub_count = 0;
				break;
			}
			case MonitorState.InsertCoinFadeInWait:
			{
				Animator animator2 = null;
				animator2 = _creditWindow.transform.GetChild(0).GetComponent<Animator>();
				if (IsEndAnim(animator2))
				{
					animator2.Play("InsertCoin_Loop", 0, 0f);
					GameObject obj2 = _creditWindow.transform.GetChild(0).GetChild(1).gameObject;
					animator2 = obj2.transform.GetChild(0).gameObject.GetComponent<Animator>();
					if (IsEndAnim(animator2))
					{
						animator2.Play("Loop", 0, 0f);
					}
					animator2 = obj2.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Animator>();
					animator2.Play("Coin", 0, 0f);
					if (!_isTimeUp)
					{
						TimeSkipStart();
					}
					_state = MonitorState.InsertCoinWait;
				}
				else if (_timer == 0 && _sub_count == 0)
				{
					GameObject obj3 = _creditWindow.transform.GetChild(0).GetChild(1).gameObject;
					obj3.transform.GetChild(3).gameObject.GetComponent<Animator>().Play("Out", 0, 1f);
					obj3.transform.GetChild(4).gameObject.GetComponent<Animator>().Play("Out", 0, 1f);
					_sub_count++;
				}
				break;
			}
			case MonitorState.InsertCoinWait:
			{
				Animator animator8 = null;
				GameObject gameObject6 = null;
				animator8 = _creditWindow.transform.GetChild(0).GetComponent<Animator>();
				if (IsEndAnim(animator8))
				{
					animator8.Play("InsertCoin_Loop", 0, 0f);
				}
				gameObject6 = _creditWindow.transform.GetChild(0).GetChild(1).gameObject;
				animator8 = gameObject6.transform.GetChild(0).gameObject.GetComponent<Animator>();
				if (IsEndAnim(animator8))
				{
					animator8.Play("Loop", 0, 0f);
				}
				animator8 = gameObject6.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Animator>();
				if (IsEndAnim(animator8))
				{
					animator8.Play("Coin", 0, 0f);
				}
				if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsEnableEmoneyExecute)
				{
					if (AMDaemon.EMoney.IsAuthCompleted)
					{
						if (!_isBalanceAuthCompleted)
						{
							gameObject6.transform.GetChild(5).gameObject.SetActive(value: true);
							_isBalanceAuthCompleted = true;
						}
						if (_isBalanceNetworkDisconnection)
						{
							if (!_balanceCommon._isOwnBalanceError01)
							{
								animator8 = gameObject6.transform.GetChild(3).gameObject.GetComponent<Animator>();
								animator8.Play("In", 0, 0f);
								gameObject6.transform.GetChild(5).gameObject.SetActive(value: false);
								_balanceCommon._isOwnBalanceError01 = true;
							}
						}
						else if (!_isBalanceNetworkDisconnection)
						{
							_balanceCommon.ResetError01();
						}
						if (_isOwnBalanceBlocking)
						{
							if (!_balanceCommon._isOwnBalanceError01 && !_balanceCommon._isOwnBalanceError02)
							{
								animator8 = gameObject6.transform.GetChild(4).gameObject.GetComponent<Animator>();
								animator8.Play("In", 0, 0f);
								gameObject6.transform.GetChild(5).gameObject.SetActive(value: false);
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
						gameObject6.transform.GetChild(5).gameObject.SetActive(value: false);
						gameObject6.transform.GetChild(3).gameObject.GetComponent<Animator>().Play("Out", 0, 1f);
						gameObject6.transform.GetChild(4).gameObject.GetComponent<Animator>().Play("Out", 0, 1f);
						_isBalanceAuthCompleted = false;
					}
				}
				if (_isTimeUp)
				{
					_isDecidedOK = false;
					_isDecidedEntry = false;
					_isCanceledEntry = true;
					ResetTimeSkipStart();
					SetNextState(MonitorState.EntryResetInit);
					if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsEnableEmoneyExecute)
					{
						_balanceCommon.ResetError();
					}
				}
				else if ((!_isNeedCreditFraction && _needCreditNum == 0) || (_isNeedCreditFraction && _needCreditNum == 0 && _needCreditNumerator == 0))
				{
					_state = MonitorState.InsertCoinFadeOut;
				}
				else if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsEnableEmoneyExecute && _isSelectButton)
				{
					_isSelectButton = false;
					ResetTimeSkipStart();
					animator8 = _creditWindow.transform.GetChild(0).GetComponent<Animator>();
					animator8.Play("InsertCoin_Out", 0, 0f);
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
				Animator component2 = _creditWindow.transform.GetChild(0).GetComponent<Animator>();
				if (_timer == 0 || IsEndAnim(component2))
				{
					if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsEnableEmoneyExecute)
					{
						_balanceCommon.ResetError();
					}
					_state = MonitorState.DecideCreditFadeIn;
				}
				break;
			}
			case MonitorState.DecideCreditFadeIn:
			{
				GameObject gameObject = null;
				GameObject gameObject2 = null;
				_creditWindow.SetActive(value: true);
				_emoneyWindow.SetActive(value: false);
				gameObject = _creditWindow.transform.GetChild(0).GetChild(1).gameObject;
				gameObject.SetActive(value: false);
				gameObject = _creditWindow.transform.GetChild(0).GetChild(2).gameObject;
				gameObject.SetActive(value: true);
				_creditWindow.transform.GetChild(0).GetComponent<Animator>().Play("Decide_In", 0, 0f);
				gameObject = _creditWindow.transform.GetChild(0).GetChild(2).gameObject;
				gameObject.transform.GetChild(0).gameObject.GetComponent<Animator>().Play("Loop", 0, 0f);
				switch (_monitorID)
				{
				case 0:
					gameObject2 = gameObject.transform.GetChild(3).gameObject;
					gameObject.transform.GetChild(4).gameObject.SetActive(value: false);
					break;
				case 1:
					gameObject2 = gameObject.transform.GetChild(4).gameObject;
					gameObject.transform.GetChild(3).gameObject.SetActive(value: false);
					break;
				}
				gameObject2.GetComponent<Animator>().Play("Loop", 0, 0f);
				gameObject.transform.GetChild(5).gameObject.SetActive(value: false);
				_state = MonitorState.DecideCreditFadeInWait;
				break;
			}
			case MonitorState.DecideCreditFadeInWait:
			{
				Animator animator9 = null;
				GameObject gameObject7 = null;
				GameObject gameObject8 = null;
				animator9 = _creditWindow.transform.GetChild(0).GetComponent<Animator>();
				if (IsEndAnim(animator9))
				{
					animator9.Play("Decide_Loop", 0, 0f);
					gameObject7 = _creditWindow.transform.GetChild(0).GetChild(2).gameObject;
					animator9 = gameObject7.transform.GetChild(0).gameObject.GetComponent<Animator>();
					if (IsEndAnim(animator9))
					{
						animator9.Play("Loop", 0, 0f);
					}
					switch (_monitorID)
					{
					case 0:
						gameObject8 = gameObject7.transform.GetChild(3).gameObject;
						gameObject7.transform.GetChild(4).gameObject.SetActive(value: false);
						break;
					case 1:
						gameObject8 = gameObject7.transform.GetChild(4).gameObject;
						gameObject7.transform.GetChild(3).gameObject.SetActive(value: false);
						break;
					}
					animator9 = gameObject8.GetComponent<Animator>();
					if (IsEndAnim(animator9))
					{
						animator9.Play("Loop", 0, 0f);
					}
					_buttonController.ChangeButtonType(ButtonControllerBase.FlatButtonType.Decide);
					_isRequestChangeOKButtonColor = true;
					_isSetGrayOKButton = false;
					OKStart(isChangeSE: true);
					if (!_isTimeUp)
					{
						TimeSkipStart();
					}
					_state = MonitorState.DecideCreditWait;
				}
				break;
			}
			case MonitorState.DecideCreditWait:
			{
				Animator animator5 = null;
				GameObject gameObject3 = null;
				GameObject gameObject4 = null;
				animator5 = _creditWindow.transform.GetChild(0).GetComponent<Animator>();
				if (IsEndAnim(animator5))
				{
					animator5.Play("Decide_Loop", 0, 0f);
				}
				gameObject3 = _creditWindow.transform.GetChild(0).GetChild(2).gameObject;
				animator5 = gameObject3.transform.GetChild(0).gameObject.GetComponent<Animator>();
				if (IsEndAnim(animator5))
				{
					animator5.Play("Loop", 0, 0f);
				}
				switch (_monitorID)
				{
				case 0:
					gameObject4 = gameObject3.transform.GetChild(3).gameObject;
					gameObject3.transform.GetChild(4).gameObject.SetActive(value: false);
					break;
				case 1:
					gameObject4 = gameObject3.transform.GetChild(4).gameObject;
					gameObject3.transform.GetChild(3).gameObject.SetActive(value: false);
					break;
				default:
					gameObject4 = gameObject3.transform.GetChild(3).gameObject;
					gameObject3.transform.GetChild(4).gameObject.SetActive(value: false);
					break;
				}
				animator5 = gameObject4.GetComponent<Animator>();
				if (IsEndAnim(animator5))
				{
					animator5.Play("Loop", 0, 0f);
				}
				if (_isOK)
				{
					_isDecidedEntry = true;
					ResetOKStart();
					ResetTimeSkipStart();
					UserData userData2 = Singleton<UserDataManager>.Instance.GetUserData(_monitorID);
					userData2.Extend.ExtendContendBit.SetFlag(ExtendContentBitID.GotoCodeRead, _isSettingToggleState[0]);
					userData2.Extend.ExtendContendBit.SetFlag(ExtendContentBitID.GotoIconPhotoShoot, _isSettingToggleState[1]);
					userData2.Extend.ExtendContendBit.SetFlag(ExtendContentBitID.GotoCharaSelect, _isSettingToggleState[2]);
					userData2.Extend.ExtendContendBit.SetFlag(ExtendContentBitID.PhotoAgree, _isSettingToggleState[3]);
					int num12 = 0;
					num12 = _volumeValue - 5;
					num12 /= 5;
					if (num12 <= 0)
					{
						num12 = 0;
					}
					if (num12 >= 19)
					{
						num12 = 19;
					}
					userData2.Option.HeadPhoneVolume = (OptionHeadphonevolumeID)num12;
					_volumeValue = num12 * 5 + 5;
					SoundManager.SetHeadPhoneVolume(_monitorID, userData2.Option.HeadPhoneVolume.GetValue());
					_state = MonitorState.DecideCreditFadeOut;
				}
				else if (_isTimeUp)
				{
					_isDecidedOK = false;
					_isDecidedEntry = false;
					_isCanceledEntry = true;
					ResetTimeSkipStart();
					SetNextState(MonitorState.EntryResetInit);
				}
				else if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsEnableEmoneyExecute && _isSelectButton)
				{
					_isSelectButton = false;
					ResetOKStart();
					ResetTimeSkipStart();
					animator5 = _creditWindow.transform.GetChild(0).GetComponent<Animator>();
					animator5.Play("Decide_Out", 0, 0f);
					_button_mode = MonitorButtonMode.MonitorButtonModeEMoney;
					_state = MonitorState.EMoneyWait;
					_balanceCommon._state = BalanceCommon.BalanceCommonState.EMoney01Init;
				}
				break;
			}
			case MonitorState.DecideCreditFadeOut:
				_creditWindow.transform.GetChild(0).GetComponent<Animator>().Play("Decide_Out", 0, 0f);
				_state = MonitorState.DecideCreditFadeOutWait;
				_timer = 30;
				break;
			case MonitorState.DecideCreditFadeOutWait:
			{
				Animator component = _creditWindow.transform.GetChild(0).GetComponent<Animator>();
				if (_timer == 0 || IsEndAnim(component))
				{
					_state = MonitorState.DecidedEntryFadeIn;
				}
				break;
			}
			case MonitorState.CreditJudgeReset:
				ResetOKStart();
				ResetTimeSkipStart();
				_isResetAnotherBalanceBlocking = true;
				_state = MonitorState.CreditJudge;
				break;
			case MonitorState.DecidedEntryFadeIn:
			{
				_sub_count = 0;
				_isEntrySE = false;
				Animator animator = null;
				_creditWindow.SetActive(value: false);
				_emoneyWindow.SetActive(value: false);
				_entryInfoWindow.SetActive(value: true);
				GameObject obj = _entryInfoWindow.transform.GetChild(0).gameObject;
				obj.SetActive(value: true);
				animator = obj.GetComponent<Animator>();
				if (_isEntryNum == 1)
				{
					animator.Play("Entry", 0, 0f);
				}
				else
				{
					animator.Play("Waiting_In", 0, 0f);
				}
				animator = _entryInfoWindow.transform.GetChild(0).GetChild(0).GetChild(1)
					.gameObject.GetComponent<Animator>();
				animator.Play("Loop", 0, 0f);
				_state = MonitorState.DecidedEntryFadeInWait;
				break;
			}
			case MonitorState.DecidedEntryFadeInWait:
			{
				_sub_count++;
				if (_sub_count > 15 && !_isEntrySE)
				{
					SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000011, _monitorID);
					_isEntrySE = true;
				}
				Animator animator7 = null;
				GameObject obj5 = _entryInfoWindow.transform.GetChild(0).gameObject;
				obj5.SetActive(value: true);
				animator7 = obj5.GetComponent<Animator>();
				if (_isEntryNum == 1)
				{
					if (IsEndAnim(animator7))
					{
						_state = MonitorState.DecidedEntryFadeOut;
					}
				}
				else if (IsEndAnim(animator7))
				{
					animator7.Play("Waiting_Loop", 0, 0f);
					_state = MonitorState.DecidedEntryWait;
				}
				animator7 = _entryInfoWindow.transform.GetChild(0).GetChild(0).GetChild(1)
					.gameObject.GetComponent<Animator>();
				if (IsEndAnim(animator7))
				{
					animator7.Play("Loop", 0, 0f);
				}
				break;
			}
			case MonitorState.DecidedEntryWait:
			{
				Animator animator6 = null;
				GameObject obj4 = _entryInfoWindow.transform.GetChild(0).gameObject;
				obj4.SetActive(value: true);
				animator6 = obj4.GetComponent<Animator>();
				if (_isEntryNum == 1)
				{
					if (IsEndAnim(animator6))
					{
						_state = MonitorState.DecidedEntryFadeOut;
					}
				}
				else if (IsEndAnim(animator6))
				{
					animator6.Play("Waiting_Loop", 0, 0f);
				}
				animator6 = _entryInfoWindow.transform.GetChild(0).GetChild(0).GetChild(1)
					.gameObject.GetComponent<Animator>();
				if (IsEndAnim(animator6))
				{
					animator6.Play("Loop", 0, 0f);
				}
				if (_isDecidedEntryNum + _isCanceledEntryNum >= _isEntryNum)
				{
					_state = MonitorState.DecidedEntryFadeOut;
				}
				break;
			}
			case MonitorState.DecidedEntryFadeOut:
				_state = MonitorState.DecidedEntryFadeOutWait;
				_timer = 30;
				break;
			case MonitorState.DecidedEntryFadeOutWait:
				if (_timer == 0)
				{
					_state = MonitorState.EntryTimeOutJudge;
				}
				break;
			case MonitorState.EntryResetInit:
				if (_timer == 0)
				{
					_creditWindow.SetActive(value: false);
					_emoneyWindow.SetActive(value: false);
					_entryInfoWindow.SetActive(value: false);
					_messageWindow.SetActive(value: false);
					_buttonController.SetVisible(false, default(int));
					_buttonController.SetVisible(false, 1);
					_buttonController.SetVisible(false, 8);
					SetNextState(MonitorState.EntryReset);
				}
				break;
			case MonitorState.EntryReset:
				_state = MonitorState.EntryTimeOutJudge;
				break;
			case MonitorState.EntryTimeOutJudge:
				switch (_isEntryNum)
				{
				case 1:
					if (_isCanceledEntryNum >= _isEntryNum)
					{
						_state = MonitorState.EntryTimeOutReturnTitleFadeIn;
					}
					else if (_isDecidedEntryNum >= _isEntryNum)
					{
						_state = MonitorState.AddTrackInfoJudge;
					}
					break;
				case 2:
					if (_isCanceledEntryNum >= _isEntryNum)
					{
						_state = MonitorState.EntryTimeOutReturnTitleFadeIn;
					}
					else if (_isDecidedEntryNum >= _isEntryNum)
					{
						_state = MonitorState.AddTrackInfoJudge;
					}
					else if (_isDecidedEntryNum + _isCanceledEntryNum >= _isEntryNum)
					{
						_state = MonitorState.AddTrackInfoJudge;
					}
					break;
				}
				break;
			case MonitorState.EntryTimeOutReturnTitleFadeIn:
			{
				string text = "";
				_messageWindow.SetActive(value: true);
				_messageWindow.transform.GetChild(0).GetComponent<Animator>().Play("In", 0, 0f);
				text = ((!_isRemainCredit) ? "タイトルに戻ります" : "タイトルに戻ります\r\nコインが残っています！");
				_messageWindow.transform.GetChild(0).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = text;
				_state = MonitorState.EntryTimeOutReturnTitleFadeInWait;
				break;
			}
			case MonitorState.EntryTimeOutReturnTitleFadeInWait:
			{
				Animator animator4 = null;
				animator4 = _messageWindow.transform.GetChild(0).GetComponent<Animator>();
				if (IsEndAnim(animator4))
				{
					_state = MonitorState.EntryTimeOutReturnTitleWait;
				}
				break;
			}
			case MonitorState.EntryTimeOutReturnTitleWait:
				_state = MonitorState.EntryTimeOutReturnTitleFadeOut;
				_timer = 120;
				break;
			case MonitorState.EntryTimeOutReturnTitleFadeOut:
				if (_timer == 0)
				{
					_messageWindow.SetActive(value: true);
					_messageWindow.transform.GetChild(0).GetComponent<Animator>().Play("Out", 0, 0f);
					_state = MonitorState.EntryTimeOutReturnTitleFadeOutWait;
				}
				break;
			case MonitorState.EntryTimeOutReturnTitleFadeOutWait:
			{
				Animator animator3 = null;
				animator3 = _messageWindow.transform.GetChild(0).GetComponent<Animator>();
				if (IsEndAnim(animator3))
				{
					_messageWindow.SetActive(value: false);
					_state = MonitorState.Finish;
				}
				break;
			}
			case MonitorState.AddTrackInfoJudge:
			{
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(_monitorID);
				_isDailyBonus = DoneEntry.IsDailyBonus(userData);
				_isWeekDayBonus = DoneEntry.IsWeekdayBonus(userData);
				if (_isDecidedEntry)
				{
					_state = MonitorState.AddTrackInfoJudgeWait;
				}
				else
				{
					_state = MonitorState.Finish;
				}
				break;
			}
			case MonitorState.AddTrackInfoFadeIn:
				_state = MonitorState.AddTrackInfoFadeInWait;
				_timer = 30;
				break;
			case MonitorState.AddTrackInfoFadeInWait:
				if (_timer == 0)
				{
					switch (_addTrackType)
					{
					case 3:
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_ENTRY_MUSIC_UP, _monitorID);
						SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000008, _monitorID);
						break;
					case 4:
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_ENTRY_MUSIC_UP, _monitorID);
						SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000008, _monitorID);
						break;
					case 6:
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_ENTRY_MUSIC_UP, _monitorID);
						SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000008, _monitorID);
						break;
					case 7:
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_ENTRY_MUSIC_UP, _monitorID);
						SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000008, _monitorID);
						break;
					}
					_state = MonitorState.AddTrackInfoWait;
					_timer = 120;
				}
				break;
			case MonitorState.AddTrackInfoWait:
				if (_timer == 0)
				{
					_state = MonitorState.Finish;
				}
				break;
			case MonitorState.Finish:
				if (_timer == 0)
				{
					SetNextState(MonitorState.End);
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
