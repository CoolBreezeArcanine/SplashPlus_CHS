using System;
using System.Collections.Generic;
using DB;
using Fx;
using IO;
using Manager;
using Process;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class RegionalSelectMonitor : MonitorBase
{
	[SerializeField]
	private Animator _regionalSelectAnimator;

	[SerializeField]
	[Header("地方一覧リスト管理")]
	private RegionalList _selectList;

	[SerializeField]
	[Header("選択地方情報表示管理")]
	private RegionalInfoDisplay _infoDisplay;

	[SerializeField]
	[Header("ボタン管理")]
	private RegionalSelectButtonController _buttonController;

	[SerializeField]
	[Header("ブラー")]
	private CanvasGroup _blurGroup;

	[SerializeField]
	[Header("Event")]
	private Animator _eventAnimator;

	[SerializeField]
	private Image _eventBunner;

	[SerializeField]
	[Header("新地方発見管理")]
	private RegionalDiscover _discover;

	[SerializeField]
	[Header("パーティクル")]
	private InstantiateGenerator _emitterGenerator;

	[SerializeField]
	[Header("キャラクター")]
	[Header("報酬獲得ウィンドウ")]
	private InstantiateGenerator _characterGetGenerator;

	[SerializeField]
	[Header("楽曲")]
	private InstantiateGenerator _musicGetGenerator;

	[SerializeField]
	[Header("ネームプレート")]
	private InstantiateGenerator _namePlateGetGenerator;

	[SerializeField]
	[Header("Text")]
	private TextMeshProUGUI _textCountMusic;

	[SerializeField]
	private TextMeshProUGUI _textCountCollection;

	[SerializeField]
	private TextMeshProUGUI _textTotalDistance;

	[SerializeField]
	private TextMeshProUGUI _textEvent;

	[SerializeField]
	[Header("設定")]
	[Tooltip("Null_SettingWindow/UI_CMN_SettingWindow")]
	private SettingWindowController _settingWindow;

	[SerializeField]
	[Tooltip("Null_UI_CMN_SettingMiniWindow")]
	private SettingIndicator _settingIndicator;

	private IRegionalSelectProcess _process;

	private EventWindowBase _rewordGetWindow;

	private Action<int> _onFinishReward;

	private FX_Controler _fxControler;

	private bool _isScroll;

	private bool _isEvent;

	public RegionalDiscover Discover => _discover;

	public void Initialize(IRegionalSelectProcess process, int monIndex, int firstRegionIndex, bool isActive, bool isEvent, AssetManager manager)
	{
		_process = process;
		_blurGroup.alpha = 0f;
		_isEvent = isEvent;
		if (isActive)
		{
			List<UserMapData> userMapDatas = _process.GetUserMapDatas(monIndex);
			_selectList.Initialize(monIndex, userMapDatas, manager);
			_infoDisplay.Initialize(userMapDatas, manager);
			_isScroll = true;
			_buttonController.Initialize(monIndex, isEvent);
			_fxControler = _emitterGenerator.Instantiate<FX_Controler>();
			_regionalSelectAnimator.Play(Animator.StringToHash("InitIn"), 0, 0f);
			_settingWindow.Initialize();
		}
		else
		{
			Disabled();
		}
		_textCountMusic.text = CommonMessageID.RegionalSelectCountMusic.GetName();
		_textCountCollection.text = CommonMessageID.RegionalSelectCountCollection.GetName();
		_textTotalDistance.text = CommonMessageID.RegionalSelectTotalDistancce.GetName();
		_textEvent.text = CommonMessageID.RegionalSelectEvent.GetName();
		base.Initialize(monIndex, isActive);
	}

	public override void ViewUpdate()
	{
		float deltaTime = (float)GameManager.GetGameMSecAdd() / 1000f;
		_selectList.ViewUpdate(deltaTime);
		_infoDisplay.ViewUpdate();
		_discover.ViewUpdate(deltaTime);
		_buttonController.ViewUpdate(GameManager.GetGameMSecAdd());
		_settingWindow.ViewUpdate(GameManager.GetGameMSecAdd());
		if (_isScroll)
		{
			_infoDisplay.SetData(_process.GetUserMapDatas(monitorIndex)[_process.SelectCursolIndex(monitorIndex)], monitorIndex);
			_isScroll = false;
		}
	}

	public void Play()
	{
		_regionalSelectAnimator.Play(Animator.StringToHash("In"), 0, 0f);
		_selectList.ScrollExecute(_process.SelectCursolIndex(base.MonitorIndex));
	}

	public void ListScroll(int index)
	{
		if (IsActive())
		{
			_selectList.ScrollExecute(index);
			_isScroll = true;
		}
	}

	public void SetActiveToggleButton(bool isActive, bool toggleState)
	{
		_buttonController.SetActiveToggleButton(isActive, toggleState);
		Color32 color = CommonButtonObject.LedColors32(isActive ? CommonButtonObject.LedColors.White : CommonButtonObject.LedColors.Black);
		MechaManager.LedIf[monitorIndex].SetColorButton(0, color);
	}

	public void PushToggleButton(bool value)
	{
		_buttonController.SwtchToggle(value);
	}

	public void SetVisibleButton(bool isActive, params int[] buttonIndex)
	{
		_buttonController.SetVisible(isActive, buttonIndex);
	}

	public void PushButton(int buttonIndex)
	{
		_buttonController.SetAnimationActive(buttonIndex);
	}

	public void ChangeDecisionButton()
	{
		_buttonController.ChangeDecisionButton();
	}

	public void InitRegionButton(bool value)
	{
		_buttonController.InitRegionButtonState(value);
	}

	public void ChangeRegionButtonState(bool isEvent)
	{
		_buttonController.ChangeRegionButtonState(isEvent);
	}

	public void SetEventBanner(Sprite image)
	{
		_eventBunner.sprite = image;
		_eventAnimator.Play(Animator.StringToHash("Loop"), 0, 0f);
	}

	public void SetActiveEventBanner(bool isActive)
	{
		if (_eventAnimator.gameObject.activeSelf != isActive)
		{
			_eventAnimator.gameObject.SetActive(isActive);
			if (isActive)
			{
				_eventAnimator.Play(Animator.StringToHash("Loop"), 0, 0f);
			}
		}
	}

	public void PushEventBanner()
	{
		_eventAnimator.Play(Animator.StringToHash("In"), 0, 0f);
	}

	public void Decide()
	{
		_fxControler.Play();
		_blurGroup.alpha = 1f;
	}

	public void RewardCharaGetWindow(Sprite chara, string charaName, int id, Action<int> finish)
	{
		_onFinishReward = finish;
		_rewordGetWindow = _characterGetGenerator.Instantiate<EventWindowBase>();
		((CharaWindow)_rewordGetWindow).Set(chara, charaName, id);
		((CharaWindow)_rewordGetWindow).Play(CallbackRewardFinish);
	}

	public void RewardMusicGetWindow(Sprite jacket, string musicName, Action<int> finish)
	{
		_onFinishReward = finish;
		_rewordGetWindow = _musicGetGenerator.Instantiate<EventWindowBase>();
		((MusicWindow)_rewordGetWindow).Set(jacket, musicName);
		_rewordGetWindow.Play(CallbackRewardFinish);
	}

	public void RewardNamePlateGetWindow(Sprite plate, string title, Action<int> finish)
	{
		_onFinishReward = finish;
		_rewordGetWindow = _namePlateGetGenerator.Instantiate<EventWindowBase>();
		((CollectionWindow)_rewordGetWindow).SetNamePlate(plate, title);
		_rewordGetWindow.Play(CallbackRewardFinish);
	}

	public void SetPerfectChallenge(int life, MusicDifficultyID difficulty, Sprite jacket)
	{
		_infoDisplay.SetPerfectChallenge(life, difficulty, jacket);
	}

	private void CallbackRewardFinish()
	{
		_onFinishReward(monitorIndex);
	}

	public void Disabled()
	{
		Main.alpha = 0f;
	}

	public void OpenSettingWindow()
	{
		_settingWindow.Open();
		_blurGroup.alpha = 1f;
	}

	public void CloseSettingWindow()
	{
		_settingWindow.Close();
		_blurGroup.alpha = 0f;
	}

	public void SetSettingState(int index, bool state, bool isActive, bool nextTime)
	{
		_settingWindow.SetSettingState(index, state, nextTime);
		_settingWindow.SetActive(index, isActive);
		_settingIndicator.SetSettingState(index, state);
	}

	public void SetVolume(OptionHeadphonevolumeID volume)
	{
		_settingWindow.SetVolume(volume);
		_settingIndicator.SetVolume(volume.GetName());
	}

	public void PressedToggle(int index, bool toState)
	{
		_settingWindow.PressedToggle(index, toState);
		_settingIndicator.SetSettingState(index, toState);
	}

	public void PressedPlusButton()
	{
		_settingWindow.PressedPlusButton();
	}

	public void PressedMinusButton()
	{
		_settingWindow.PressedMinusButton();
	}

	public void HoldPlusButton()
	{
		_settingWindow.HoldPlusButton();
	}

	public void HoldMinusButton()
	{
		_settingWindow.HoldMinusButton();
	}

	public void ChangeSettingButton(bool state)
	{
		_buttonController.ChangeSettingButton(state);
	}

	public void SetActiveSettingIndicator(bool isActive)
	{
		_settingIndicator.gameObject.SetActive(isActive);
	}
}
