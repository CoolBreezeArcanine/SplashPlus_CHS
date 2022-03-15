using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DB;
using IO;
using MAI2.Util;
using Manager;
using Manager.UserDatas;
using Monitor.CharacterSelect;
using Monitor.CharacterSelect.Controllers;
using Process;
using TMPro;
using UI;
using UI.Common;
using UI.DaisyChainList;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor
{
	public class CharacterSelectMonitor : MonitorBase
	{
		private const string Disabled = "Disabled";

		private const string In = "In";

		private const string Out = "Out";

		private const string Loop = "Loop";

		[SerializeField]
		[Header("背景")]
		private Animator _backgroundAnimator;

		[SerializeField]
		private Image _townBackImage;

		[SerializeField]
		private Image _townImage;

		[SerializeField]
		[Header("キャラクタースロット")]
		private GameObject _characterSlotOriginal;

		[SerializeField]
		private Transform[] _characterSlotPositions;

		[SerializeField]
		private CharacterSelectButtonController _buttonSelectController;

		[SerializeField]
		[Header("リスト")]
		private CharacterChainList _characterChainList;

		[SerializeField]
		[Header("セレクター")]
		private Animator _selectorAnimator;

		[SerializeField]
		[Header("比較表示用")]
		private CharacterStateObject _exchangeOriginState;

		[SerializeField]
		private Animator _selectorStateAnimator;

		[SerializeField]
		[Header("セレクター：新規加入")]
		private NewcomerComparisonObject _newcomerComparisonObject;

		[SerializeField]
		[Header("スロットビュー")]
		private Animator _slotViewAnimator;

		[SerializeField]
		private Animator[] _slotAnimators;

		[SerializeField]
		private Animator _autoButtonAnimator;

		[SerializeField]
		[Header("距離表示計")]
		private OdoSpriteTexts _slotviewDistance;

		[SerializeField]
		private OdoSpriteTexts _newcomerDistance;

		[SerializeField]
		[Header("メッセージバルーン")]
		private Animator _messageBalloonAnimator;

		[SerializeField]
		private GameObject _newcomerMessageObject;

		[SerializeField]
		[Header("キャラクター情報")]
		private TextMeshProUGUI _characterNameText;

		[SerializeField]
		[Header("おまかせボタン")]
		private Animator _autoSetButton;

		[SerializeField]
		[Header("ブラーオブジェクト")]
		private CanvasGroup _blurObject;

		[SerializeField]
		[Header("タブコントローラー")]
		private CharacterTabController _tabController;

		[SerializeField]
		[Header("セレクター背景")]
		private Transform _selectorBackParent;

		[SerializeField]
		private GameObject _selectorBackGroundOrigin;

		[SerializeField]
		[Header("新規加入中央表示")]
		private CanvasGroup _newcomerCenterGroup;

		[SerializeField]
		private Animator[] _holdLockAnimators;

		[SerializeField]
		private InstantiateGenerator _newcomerViewGenerator;

		[SerializeField]
		private TextMeshProUGUI _normalText;

		[SerializeField]
		private TextMeshProUGUI _newCommerText;

		private SelectorBackgroundController _backgroundController;

		private StateAnimController _animController;

		private ReadOnlyCollection<SelectSlotObject> _slotList;

		private ICharacterSelectProcess _characterSelect;

		private Coroutine _currentCoroutine;

		private CharacterSelectCenterItemObject _newcomerItem;

		private int _currentSlotIndex;

		private float _normalizeTimer;

		private bool[] _isSlotActives;

		private bool _isNewcomerMode;

		private bool _isBlankSlot;

		private bool _isButtonControllerInitialize;

		private string _newcomerName;

		private UserChara _newcomerChara;

		private float Blur
		{
			get
			{
				return _blurObject.alpha;
			}
			set
			{
				_blurObject.alpha = value;
			}
		}

		public void SetData(ICharacterSelectProcess characterSelect)
		{
			_characterSelect = characterSelect;
		}

		private void Awake()
		{
			_normalText.text = CommonMessageID.CharaSetNormal.GetName();
			_newCommerText.text = CommonMessageID.CharaSetNewCommer.GetName();
		}

		public override void Initialize(int playerIndex, bool isActive)
		{
			base.Initialize(playerIndex, isActive);
			Main.alpha = 0f;
			if (isActive)
			{
				List<SelectSlotObject> list = new List<SelectSlotObject>(_characterSlotPositions.Length);
				Transform[] characterSlotPositions = _characterSlotPositions;
				foreach (Transform parent in characterSlotPositions)
				{
					SelectSlotObject component = UnityEngine.Object.Instantiate(_characterSlotOriginal, parent).GetComponent<SelectSlotObject>();
					component.Initialize();
					list.Add(component);
				}
				_slotList = new ReadOnlyCollection<SelectSlotObject>(list);
				_isSlotActives = new bool[list.Count + 1];
				for (int j = 0; j < _isSlotActives.Length; j++)
				{
					_isSlotActives[j] = false;
				}
				_characterChainList.Initialize();
				_characterChainList.SetData(playerIndex, _characterSelect);
				_tabController.Initialize(playerIndex);
				_animController?.SetExitParts(SetVisibleFavoriteParts, Animator.StringToHash("Base Layer.Pressed"));
				_backgroundController = UnityEngine.Object.Instantiate(_selectorBackGroundOrigin, _selectorBackParent).GetComponent<SelectorBackgroundController>();
				if (!_isButtonControllerInitialize)
				{
					_buttonSelectController.Initialize(monitorIndex);
					_isButtonControllerInitialize = true;
				}
				Blur = 0f;
				_exchangeOriginState.Initialize();
			}
		}

		public void SetBackground(Sprite townBack, Sprite town)
		{
			if (isPlayerActive)
			{
				_townBackImage.sprite = townBack;
				_townBackImage.transform.GetChild(0).GetComponent<Image>().sprite = townBack;
				_townImage.sprite = town;
				_townImage.transform.GetChild(0).GetComponent<Image>().sprite = town;
			}
		}

		public void SetNewcomerMode(bool newcomerMode)
		{
			_isNewcomerMode = newcomerMode;
			_newcomerMessageObject.SetActive(_isNewcomerMode);
			_slotviewDistance.gameObject.SetActive(!_isNewcomerMode);
			_newcomerDistance.gameObject.SetActive(_isNewcomerMode);
			if (newcomerMode)
			{
				_newcomerItem = _newcomerViewGenerator.Instantiate<CharacterSelectCenterItemObject>();
				_newcomerItem.Initialize();
				_newcomerComparisonObject.gameObject.SetActive(value: true);
				_newcomerComparisonObject.Initialize();
			}
			else
			{
				_newcomerComparisonObject.gameObject.SetActive(value: false);
			}
		}

		public void SetBlankSlots(bool isBlank)
		{
			_isBlankSlot = isBlank;
		}

		public void SetNewcomer(CharacterData chara)
		{
			if (_isNewcomerMode)
			{
				CharacterMapColorData mapColorData = _characterSelect.GetMapColorData(chara.Data.genre.id);
				bool flag = _characterSelect.IsMatchColor(monitorIndex, chara.UserChara.ID);
				Texture2D texture = chara.Texture;
				Sprite face = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
				_newcomerChara = chara.UserChara;
				_newcomerName = chara.Data.name.str;
				int movementParam = (int)chara.UserChara.GetMovementParam(flag);
				WeakPoint weakPoint = ((!flag) ? WeakPoint.Weak : WeakPoint.Forte);
				_newcomerItem.SetInformation(isParty: false, chara.Data.name.str, movementParam, weakPoint, CharacterSelectProces.ArrowDirection.Stay);
				_newcomerItem.SetData((int)chara.UserChara.Level, chara.UserChara.NextAwakePercent, (int)chara.UserChara.Awakening, mapColorData.SmallBase, face, mapColorData.SmallFrame, mapColorData.Level, mapColorData.SmallAwakeStar, mapColorData.AwakeStarBase);
				_newcomerComparisonObject.SetNewcomerData(chara.Data.name.str, movementParam, weakPoint, CharacterSelectProces.ArrowDirection.Stay);
				_newcomerComparisonObject.SetNewcomerParts(1, 0f, 0, mapColorData.SmallBase, face, mapColorData.Level);
			}
		}

		public void SetFirstInformation()
		{
			_blurObject.alpha = 1f;
			_buttonSelectController.SetVisibleImmediate(false, 2, 3, 4, 5);
		}

		public void SetFirstInformationNext()
		{
			_buttonSelectController.SetVisible(false, 3);
		}

		public void SetFirstInformationButton()
		{
			Main.alpha = 1f;
			_buttonSelectController.SetVisible(true, 3);
			_buttonSelectController.ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 4);
		}

		public void SetFirstInformationResume()
		{
			_blurObject.alpha = 0f;
			if (_isNewcomerMode)
			{
				_buttonSelectController.SetVisibleImmediate(false, 2, 3, 4, 5);
				if (!_isBlankSlot)
				{
					StartCoroutine(InfoResume());
				}
			}
			else
			{
				_buttonSelectController.SetVisibleImmediate(false, 2, 4, 5);
				StartCoroutine(InfoResume());
			}
			SetSlotViewLed();
		}

		private IEnumerator InfoResume()
		{
			yield return new WaitForSeconds(0.5f);
			int index = (_isNewcomerMode ? 4 : 2);
			_buttonSelectController.ChangeButtonSymbol(InputManager.ButtonSetting.Button04, index);
			_buttonSelectController.SetVisible(true, 3);
		}

		public void OnReset()
		{
			if (!isPlayerActive)
			{
				return;
			}
			Main.alpha = 1f;
			_backgroundController.Play(SelectorBackgroundController.AnimationType.Disabled);
			_selectorAnimator.SetTrigger("Disabled");
			_selectorStateAnimator.Play(Animator.StringToHash("Disabled"));
			_slotViewAnimator.SetTrigger("In");
			_buttonSelectController.SetVisibleImmediate(false, 2, 3, 4, 5);
			if (IsActive())
			{
				MechaManager.LedIf[monitorIndex].ButtonLedReset();
			}
			foreach (SelectSlotObject slot in _slotList)
			{
				slot.SetActiveNewcomerMode(_isNewcomerMode);
			}
			Animator[] slotAnimators = _slotAnimators;
			for (int i = 0; i < slotAnimators.Length; i++)
			{
				slotAnimators[i].Play(Animator.StringToHash("FadeIn"), 0, 0f);
			}
			_currentCoroutine = StartCoroutine(ResetCoroutine());
			StartCoroutine(BackgroundCorutine());
		}

		private IEnumerator ResetCoroutine()
		{
			yield return new WaitForSeconds(0.5f);
			for (int i = 0; i < _slotAnimators.Length; i++)
			{
				if (_isSlotActives[i])
				{
					_slotAnimators[i].Play(Animator.StringToHash("Loop"), 0, 0f);
				}
			}
			_autoButtonAnimator.SetTrigger("Loop");
			_characterSelect.StartCountUp(monitorIndex);
			_autoSetButton.SetTrigger("Loop");
			_messageBalloonAnimator.Play(Animator.StringToHash("In"));
		}

		private IEnumerator BackgroundCorutine()
		{
			yield return new WaitForSeconds(0.1f);
			_backgroundAnimator.Play(Animator.StringToHash("In"), 0, 0f);
			float length = _backgroundAnimator.GetCurrentAnimatorStateInfo(0).length;
			yield return new WaitForSeconds(length);
			_backgroundAnimator.Play(Animator.StringToHash("Loop"), 0, 0f);
		}

		public void NextNewcomer()
		{
			_slotViewAnimator.SetTrigger("In");
			if (_isBlankSlot)
			{
				_buttonSelectController.SetVisible(false, 2, 3, 4, 5);
			}
			else
			{
				_buttonSelectController.SetVisible(false, 2, 4, 5);
				_buttonSelectController.ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 4);
			}
			Animator[] slotAnimators = _slotAnimators;
			for (int i = 0; i < slotAnimators.Length; i++)
			{
				slotAnimators[i].Play(Animator.StringToHash("FadeIn"), 0, 0f);
			}
			_newcomerCenterGroup.alpha = 1f;
			_newcomerDistance.gameObject.SetActive(value: true);
			_exchangeOriginState.gameObject.SetActive(value: true);
			SetSlotViewLed();
			_currentCoroutine = StartCoroutine(ToSlotViewCoroutine());
		}

		public void ToSlotView(Action callback = null)
		{
			Blur = 0f;
			_selectorAnimator.SetTrigger("Out");
			if (_selectorStateAnimator.gameObject.activeSelf)
			{
				_selectorStateAnimator.Play("Out");
			}
			_slotViewAnimator.SetTrigger("In");
			_tabController.PlayOutAnimation();
			_backgroundController.SetVisibleIndicator(isActive: false);
			if (_isNewcomerMode)
			{
				if (_isBlankSlot)
				{
					_buttonSelectController.SetVisible(false, 2, 3, 4, 5);
				}
				else
				{
					_buttonSelectController.SetVisible(false, 2, 4, 5);
					_buttonSelectController.ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 4);
				}
			}
			else
			{
				_buttonSelectController.SetVisible(false, 2, 4, 5);
				_buttonSelectController.ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 2);
			}
			SetSlotViewLed();
			_characterChainList.RemoveAll();
			_backgroundController.Play(SelectorBackgroundController.AnimationType.Out);
			for (int i = 0; i < _slotAnimators.Length; i++)
			{
				if (i == _currentSlotIndex)
				{
					_slotAnimators[i].Play(Animator.StringToHash("CenterOut"), 0, 0f);
					SetImmediateChangeLockIcon(i, isVisible: false);
				}
				else
				{
					_slotAnimators[i].Play(Animator.StringToHash("FadeIn"), 0, 0f);
				}
			}
			if (_isNewcomerMode)
			{
				_newcomerCenterGroup.alpha = 1f;
				_newcomerDistance.gameObject.SetActive(value: true);
				_exchangeOriginState.gameObject.SetActive(value: true);
			}
			_currentCoroutine = StartCoroutine(ToSlotViewCoroutine(callback));
		}

		private IEnumerator ToSlotViewCoroutine(Action callback = null)
		{
			yield return new WaitForSeconds(0.5f);
			for (int i = 0; i < _slotAnimators.Length; i++)
			{
				if (_isSlotActives[i])
				{
					_slotAnimators[i].Play(Animator.StringToHash("Loop"), 0, 0f);
				}
			}
			_autoButtonAnimator.SetTrigger("Loop");
			_characterSelect.StartCountUp(monitorIndex);
			_messageBalloonAnimator.Play(Animator.StringToHash("In"));
			yield return new WaitForSeconds(0.3f);
			callback?.Invoke();
		}

		public void ToSlotViewAutoSet()
		{
			Blur = 0f;
			SetSlotViewLed();
			if (_isNewcomerMode)
			{
				_buttonSelectController.SetVisible(false, 2, 3);
			}
			_slotViewAnimator.SetTrigger("In");
			for (int i = 0; i < _slotAnimators.Length; i++)
			{
				if (i < 5 && _characterSelect.IsLockedSlot(monitorIndex, i))
				{
					_holdLockAnimators[i].Play(Animator.StringToHash("HoldAction"), 0, 0f);
				}
				else
				{
					_slotAnimators[i].Play(Animator.StringToHash("FadeIn"), 0, 0f);
				}
				_ = _isSlotActives[i];
			}
			_characterSelect.StartCountUp(monitorIndex);
			if (_isNewcomerMode)
			{
				_newcomerCenterGroup.alpha = 0f;
				_newcomerDistance.gameObject.SetActive(value: false);
				_exchangeOriginState.gameObject.SetActive(value: true);
			}
		}

		public void OnReturn()
		{
			if (_currentCoroutine != null)
			{
				StopCoroutine(_currentCoroutine);
				_currentCoroutine = null;
			}
			Blur = 0f;
			_characterChainList.RemoveAll();
			_backgroundController?.Play(SelectorBackgroundController.AnimationType.Disabled);
			_selectorAnimator.SetTrigger("Disabled");
			if (_selectorStateAnimator.gameObject.activeSelf && _selectorStateAnimator.gameObject.activeInHierarchy)
			{
				_selectorStateAnimator.Play(Animator.StringToHash("Disabled"));
			}
			_messageBalloonAnimator.Play(Animator.StringToHash("Out"));
			_slotViewAnimator.SetTrigger("In");
			if (IsActive())
			{
				MechaManager.LedIf[monitorIndex].ButtonLedReset();
			}
		}

		public void ToCharacterSelect(int slotIndex = -1)
		{
			_backgroundController.SetVisibleIndicator(isActive: true);
			_backgroundController.Play(SelectorBackgroundController.AnimationType.In);
			byte ledPos = 0;
			switch (slotIndex)
			{
			case 0:
				ledPos = 5;
				break;
			case 1:
				ledPos = 6;
				break;
			case 2:
				ledPos = 7;
				break;
			case 3:
				ledPos = 0;
				break;
			case 4:
				ledPos = 1;
				break;
			}
			MechaManager.LedIf[monitorIndex].SetColorButtonPressed(ledPos, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.Red));
			_messageBalloonAnimator.Play(Animator.StringToHash("Out"));
			_selectorAnimator.SetTrigger("In");
			_slotViewAnimator.SetTrigger("Out");
			_currentSlotIndex = slotIndex;
			for (int i = 0; i < _slotAnimators.Length; i++)
			{
				if (i == _currentSlotIndex)
				{
					_slotAnimators[i].Play(Animator.StringToHash("CenterIn"), 0, 0f);
				}
				else
				{
					_slotAnimators[i].Play(Animator.StringToHash("FadeOut"), 0, 0f);
				}
			}
			CharacterData userCharaData = _characterSelect.GetUserCharaData(monitorIndex, 0);
			_backgroundController.SetBackgroundColor(8);
			_backgroundController.PrepareIndicator(_characterSelect.GetCurrentCharacterIndex(monitorIndex), _characterSelect.GetCurrentCharacterListMax(monitorIndex));
			_backgroundController.SetScrollMessage(CommonMessageID.Scroll_Character_Select.GetName());
			CharacterData slotData = _characterSelect.GetSlotData(monitorIndex, slotIndex);
			if (_isNewcomerMode)
			{
				WeakPoint weak = ((!_characterSelect.IsMatchColor(monitorIndex, _newcomerChara.ID)) ? WeakPoint.Weak : WeakPoint.Forte);
				if (slotIndex == 0)
				{
					weak = WeakPoint.Leader;
				}
				int movementParam = (int)_newcomerChara.GetMovementParam(matchColor: true, slotIndex == 0);
				_newcomerComparisonObject.SetNewcomerData(_newcomerName, movementParam, weak, CharacterSelectProces.ArrowDirection.Stay);
			}
			if (slotData != null)
			{
				_exchangeOriginState.gameObject.SetActive(value: true);
				_selectorStateAnimator.Play(Animator.StringToHash("In"), 0, 0f);
				CharacterMapColorData mapColorData = _characterSelect.GetMapColorData(slotData.Data.genre.id);
				Texture2D texture = userCharaData.Texture;
				Sprite face = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
				_exchangeOriginState.SetParts(mapColorData.Base, face, mapColorData.Frame, mapColorData.Level, slotData.ShadowColor);
				bool flag = _characterSelect.IsMatchColor(monitorIndex, userCharaData.UserChara.ID);
				WeakPoint weakPoint = ((!flag) ? WeakPoint.Weak : WeakPoint.Forte);
				if (slotIndex == 0)
				{
					weakPoint = WeakPoint.Leader;
				}
				int movementParam2 = (int)userCharaData.UserChara.GetMovementParam(flag, slotIndex == 0);
				_exchangeOriginState.SetData(userCharaData.Data.name.str, movementParam2, weakPoint, CharacterSelectProces.ArrowDirection.Stay);
				if (_isNewcomerMode)
				{
					CharacterData newcomerData = _characterSelect.GetNewcomerData(monitorIndex);
					flag = _characterSelect.IsMatchColor(monitorIndex, newcomerData.UserChara.ID);
					CharacterSelectProces.ArrowDirection arrowDirection = _characterSelect.GetArrowDirection(monitorIndex, (int)newcomerData.UserChara.GetMovementParam(flag, slotIndex == 0));
					_newcomerComparisonObject.SetSelectData(userCharaData.Data.name.str, movementParam2, weakPoint);
					_newcomerComparisonObject.SetSelectParts(1, 0f, 0, mapColorData.SmallBase, face, mapColorData.Level, newcomerData.ShadowColor);
					_newcomerComparisonObject.SetNewcomerComparisonArrow(arrowDirection);
				}
			}
			else
			{
				_exchangeOriginState.SetBlank();
				_exchangeOriginState.gameObject.SetActive(value: false);
				if (_isNewcomerMode)
				{
					CharacterData newcomerData2 = _characterSelect.GetNewcomerData(monitorIndex);
					bool matchColor = _characterSelect.IsMatchColor(monitorIndex, newcomerData2.UserChara.ID);
					CharacterSelectProces.ArrowDirection arrowDirection2 = _characterSelect.GetArrowDirection(monitorIndex, (int)newcomerData2.UserChara.GetMovementParam(matchColor));
					_newcomerComparisonObject.SetSelectBlank();
					_newcomerComparisonObject.SetNewcomerComparisonArrow(arrowDirection2);
				}
			}
			if (_isNewcomerMode)
			{
				_newcomerCenterGroup.alpha = 0f;
				_newcomerDistance.gameObject.SetActive(value: false);
				_exchangeOriginState.gameObject.SetActive(value: false);
			}
			else
			{
				_tabController.SlotView2CharacterSelect(_characterSelect.GetTabDataList(monitorIndex), _characterSelect.GetCurrentCategoryIndex(monitorIndex));
			}
			_currentCoroutine = StartCoroutine(ToCharacterSelectCoroutine(slotIndex == 0));
		}

		private IEnumerator ToCharacterSelectCoroutine(bool isLeaderChange)
		{
			if (IsActive())
			{
				MechaManager.LedIf[monitorIndex].ButtonLedReset();
			}
			yield return new WaitForSeconds(0.5f);
			_characterChainList.IsLeaderChange = isLeaderChange;
			SetSelectLed();
			if (_isNewcomerMode)
			{
				_buttonSelectController.SetVisible(true, 3, 4);
				_characterChainList.NewcomerDeploy();
			}
			else
			{
				_buttonSelectController.SetVisible(true, 2, 3, 4, 5);
				_characterChainList.Deploy();
			}
			_buttonSelectController.ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 2);
		}

		public void NewcomerStaging(Action onAction, bool isCharacterChange)
		{
			StartCoroutine(NewcomerStagingCoroutine(onAction, isCharacterChange));
		}

		private IEnumerator NewcomerStagingCoroutine(Action onAction, bool isCharacterChange)
		{
			Blur = 0f;
			_newcomerCenterGroup.alpha = 0f;
			foreach (SelectSlotObject slot in _slotList)
			{
				slot.SetVisibleTouchMessageObject(isVisible: false);
			}
			_backgroundController.SetVisibleIndicator(isActive: false);
			_newcomerDistance.gameObject.SetActive(value: false);
			_buttonSelectController.SetVisible(false, 2, 3, 4, 5);
			_characterChainList.RemoveAll();
			if (isCharacterChange)
			{
				_selectorAnimator.SetTrigger("Out");
				if (_selectorStateAnimator.gameObject.activeSelf)
				{
					_selectorStateAnimator.Play("Out");
				}
				_slotViewAnimator.SetTrigger("In");
				_tabController.PlayOutAnimation();
				_backgroundController.Play(SelectorBackgroundController.AnimationType.Out);
				for (int i = 0; i < _slotAnimators.Length; i++)
				{
					if (i == _currentSlotIndex)
					{
						_slotAnimators[i].SetTrigger("CenterOut");
					}
					else
					{
						_slotAnimators[i].SetTrigger("In");
					}
				}
				yield return new WaitForSeconds(0.5f);
			}
			_slotviewDistance.gameObject.SetActive(value: true);
			for (int j = 0; j < _slotAnimators.Length; j++)
			{
				if (_isSlotActives[j])
				{
					_slotAnimators[j].Play(Animator.StringToHash("Loop"), 0, 0f);
				}
			}
			_autoButtonAnimator.SetTrigger("Loop");
			_characterSelect.StartCountUp(monitorIndex);
			if (onAction == null)
			{
				Blur = 1f;
			}
			yield return new WaitForSeconds(2f);
			onAction?.Invoke();
		}

		public void ToFavoriteList()
		{
			_characterChainList.FavoriteDeploy();
		}

		public void ToAutoSetMessage()
		{
			Blur = 1f;
			_autoSetButton.SetTrigger("Pressed");
			if (IsActive())
			{
				MechaManager.LedIf[monitorIndex].ButtonLedReset();
			}
			_buttonSelectController.ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 2);
			_buttonSelectController.ChangeButtonSymbol(InputManager.ButtonSetting.Button05, 6);
		}

		public void OutAutoSetMessage()
		{
			Blur = 0f;
			SetSlotViewLed();
			_buttonSelectController.ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 2);
			if (_isNewcomerMode && _isBlankSlot)
			{
				_buttonSelectController.SetVisible(false, InputManager.ButtonSetting.Button04);
			}
			_buttonSelectController.SetVisible(false, InputManager.ButtonSetting.Button05);
			MechaManager.LedIf[monitorIndex].SetColorButton(4, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.Black));
		}

		public void SetActiveCharacterSlot(int index, bool isSlotActive)
		{
			_isSlotActives[index] = isSlotActive;
		}

		public void SetCharacterSlotData(int index, CharacterData userChara)
		{
			CharacterMapColorData mapColorData = _characterSelect.GetMapColorData(userChara.Data.genre.id);
			if (_isNewcomerMode)
			{
				if (_isBlankSlot)
				{
					_slotList[index].SetVisibleTouchMessageObject(mapColorData == null);
				}
				else
				{
					_slotList[index].SetVisibleTouchMessageObject(isVisible: true);
				}
			}
			if (mapColorData != null)
			{
				Texture2D texture = userChara.Texture;
				Sprite face = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
				_slotList[index].InstanceCharaParts.SetParts(mapColorData.SmallBase, face, mapColorData.SmallFrame, mapColorData.Level, mapColorData.SmallAwakeStar, mapColorData.AwakeStarBase, userChara.UserChara.NextAwakePercent, (int)userChara.UserChara.Level, (int)userChara.UserChara.Awakening, mapColorData.ShadowColor, index == 0);
				_slotList[index].InstanceCharaParts.SetLevel((int)userChara.UserChara.Level);
				_slotList[index].PlayJoinParty();
				bool flag = _characterSelect.IsMatchColor(monitorIndex, userChara.UserChara.ID);
				int movementParam = (int)userChara.UserChara.GetMovementParam(flag, index == 0);
				WeakPoint targetPoint = ((!flag) ? WeakPoint.Weak : WeakPoint.Forte);
				if (index == 0)
				{
					targetPoint = WeakPoint.Leader;
				}
				_slotList[index].SetForteWeak(movementParam, targetPoint);
			}
		}

		public void ResetCharacterSlotData(int index)
		{
			_slotList[index].SetBlank();
		}

		public void SetLockIcon(int index, bool isVisible)
		{
			_holdLockAnimators[index].Play(isVisible ? Animator.StringToHash("In") : Animator.StringToHash("Out"), 0, 0f);
			_slotList[index].SetActiveRing(!isVisible);
		}

		public void SetImmediateChangeLockIcon(int index, bool isVisible)
		{
			_holdLockAnimators[index].Play(isVisible ? Animator.StringToHash("Enabled") : Animator.StringToHash("Disabled"), 0, 0f);
		}

		public void SetVisibleFavoriteSlot(bool isVisible)
		{
			_characterChainList.SetFavorite(isVisible);
		}

		private void SetVisibleFavoriteParts()
		{
		}

		public void SetCharacterMovementDistance(int distance)
		{
			_slotviewDistance.SetDistance(distance);
			_newcomerDistance.SetDistance(distance);
		}

		public void SetMovementDistance(int distance, CharacterSelectProces.ArrowDirection direction)
		{
		}

		public void SetCharacterSlotView(bool isSlotActive)
		{
		}

		public void ResetCharacotrSelectController(int distance, CharacterSelectProces.ArrowDirection direction)
		{
		}

		public override void ViewUpdate()
		{
			if (!isPlayerActive)
			{
				return;
			}
			_buttonSelectController.ViewUpdate(GameManager.GetGameMSecAdd());
			_characterChainList.ViewUpdate();
			_tabController.UpdateButtonAnimation();
			_normalizeTimer += (float)GameManager.GetGameMSecAdd() / 8000f;
			_backgroundController.UpdateIndicator(_characterSelect.GetCurrentCharacterIndex(monitorIndex));
			_backgroundController.UpdateScroll();
			foreach (SelectSlotObject slot in _slotList)
			{
				slot.ViewUpdate(_normalizeTimer);
			}
			if (_normalizeTimer > 1f)
			{
				_normalizeTimer = 0f;
			}
		}

		public void ScrollCharacter(Direction direction, int currentIndex, int tabIndex, int maxIndex)
		{
			if (_characterChainList.Scroll(direction))
			{
				_tabController.Change(tabIndex);
			}
			_buttonSelectController.SetAnimationActive((direction == Direction.Left) ? 2 : 5);
			_backgroundController.PrepareIndicator(currentIndex, maxIndex);
		}

		public void ScrollCategory(Direction direction, int tabIndex)
		{
			_buttonSelectController.SetAnimationActive((direction == Direction.Left) ? 7 : 0);
			_characterChainList.Deploy();
			_tabController.Change(tabIndex, direction);
			_tabController.PressedTabButton(direction != Direction.Right);
			_backgroundController.PrepareIndicator(_characterSelect.GetCurrentCharacterIndex(monitorIndex), _characterSelect.GetCurrentCharacterListMax(monitorIndex));
		}

		public void SetScrollCharacterCard(bool isVisible)
		{
			if (isPlayerActive)
			{
				_characterChainList.SetScrollCard(isVisible);
			}
		}

		public void SetSlotTouchMessageVisible(int index, bool isVisible)
		{
			if (_isNewcomerMode)
			{
				_slotList[index].SetVisibleTouchMessageObject(isVisible);
			}
		}

		public void PressedButton(InputManager.ButtonSetting button)
		{
			_buttonSelectController.SetAnimationActive((int)button);
		}

		public void PressedOutButton(InputManager.ButtonSetting button)
		{
		}

		private void SetSlotViewLed()
		{
			if (_characterSelect.IsNewcomerModeSlotActive(monitorIndex, 3))
			{
				MechaManager.LedIf[monitorIndex].SetColorButton(0, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.White));
			}
			if (_characterSelect.IsNewcomerModeSlotActive(monitorIndex, 4))
			{
				MechaManager.LedIf[monitorIndex].SetColorButton(1, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.White));
			}
			MechaManager.LedIf[monitorIndex].SetColorButton(2, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.Purple));
			if (!_isNewcomerMode)
			{
				MechaManager.LedIf[monitorIndex].SetColorButton(3, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.Red));
			}
			else
			{
				MechaManager.LedIf[monitorIndex].SetColorButton(3, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.Black));
			}
			MechaManager.LedIf[monitorIndex].SetColorButton(4, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.Black));
			if (_characterSelect.IsNewcomerModeSlotActive(monitorIndex, 0))
			{
				MechaManager.LedIf[monitorIndex].SetColorButton(5, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.White));
			}
			if (_characterSelect.IsNewcomerModeSlotActive(monitorIndex, 1))
			{
				MechaManager.LedIf[monitorIndex].SetColorButton(6, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.White));
			}
			if (_characterSelect.IsNewcomerModeSlotActive(monitorIndex, 2))
			{
				MechaManager.LedIf[monitorIndex].SetColorButton(7, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.White));
			}
		}

		private void SetSelectLed()
		{
			MechaManager.LedIf[monitorIndex].SetColorButton(0, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.Black));
			MechaManager.LedIf[monitorIndex].SetColorButton(1, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.Black));
			MechaManager.LedIf[monitorIndex].SetColorButton(3, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.Red));
			MechaManager.LedIf[monitorIndex].SetColorButton(4, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.Blue));
			MechaManager.LedIf[monitorIndex].SetColorButton(6, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.Black));
			MechaManager.LedIf[monitorIndex].SetColorButton(7, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.Black));
		}

		public static bool IsFavorite(int userIndex, UserChara chara)
		{
			if (chara != null)
			{
				return Singleton<UserDataManager>.Instance.GetUserData(userIndex).IsFavorite(UserData.Collection.Chara, chara.ID);
			}
			return false;
		}

		public bool IsFavorite(UserChara chara)
		{
			return IsFavorite(monitorIndex, chara);
		}
	}
}
