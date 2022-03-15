using System;
using System.Collections;
using System.Collections.Generic;
using DB;
using IO;
using MAI2.Util;
using Mai2.Voice_000001;
using Mai2.Voice_Partner_000001;
using Manager;
using Process;
using Process.CodeRead;
using TMPro;
using UI;
using UnityEngine;

public class SimpleSettingMonitor : MonitorBase
{
	[SerializeField]
	[Tooltip("全体表示切替オブジェクト")]
	[Header("基本")]
	private GameObject _mainMonitorUIContensObject;

	[SerializeField]
	[Tooltip("上画面全体表示切替オブジェクト")]
	private GameObject _subMonitorContentsObject;

	[SerializeField]
	[Tooltip("ボタン管理")]
	private MenuButtonController _menuButtonController;

	[SerializeField]
	[Tooltip("カメラ撮影関連")]
	private PhotographingController _photoController;

	[SerializeField]
	[Header("ユーザー情報")]
	private CanvasGroup _menuGroup;

	[SerializeField]
	[Tooltip("ユーザー情報プレート")]
	private GameObject _userEntryObject;

	[SerializeField]
	[Tooltip("ユーザーエントリー情報Prefab")]
	private GameObject _userEntryPrefaub;

	private UserEntryPlate _userEntryPlate;

	[SerializeField]
	[Tooltip("ユーザー情報プレート(写真変更時)")]
	private GameObject _userEntryIconSetObject;

	private UserEntryPlate _userEntryIconSetPlate;

	[SerializeField]
	[Header("メッセージ")]
	private Animator _messageAnimator;

	[SerializeField]
	[Header("ボリューム")]
	private Animator _volumeAnimator;

	[SerializeField]
	[Header("ブラーオブジェクト")]
	private GameObject _blurObject;

	[SerializeField]
	[Header("ヘッドホンボリューム")]
	private GameObject _volumeObject;

	[SerializeField]
	private SpriteCounter _volumeCounter;

	[SerializeField]
	[Header("トグルボタン")]
	private InstantiateGenerator _toggleSwitch;

	[SerializeField]
	private Animator _photoAggreesAnimator;

	[SerializeField]
	[Header("パートナー")]
	private Transform _partnerObject;

	[SerializeField]
	private Animator _partnerAnimator;

	[SerializeField]
	private TextMeshProUGUI _startThisData;

	[SerializeField]
	private TextMeshProUGUI _headphoneVol;

	[SerializeField]
	private TextMeshProUGUI _checkPreview;

	[SerializeField]
	private TextMeshProUGUI _checkCamera;

	private Animator _toggleSwitchAnimator;

	private ISimpleSettingProcess _process;

	private Coroutine _frameSelect2PhotoPreparation;

	private NavigationCharacter _partnerCharacter;

	private int _partnerID;

	public bool IsCodeReadUse { get; set; }

	private void Awake()
	{
		_startThisData.text = CommonMessageID.SimpleSettingStartThisData.GetName();
		_headphoneVol.text = CommonMessageID.SimpleSettingHeadphoneVol.GetName();
		_checkPreview.text = CommonMessageID.SimpleSettingCheckPreview.GetName();
		_checkCamera.text = CommonMessageID.SimpleSettingCheckCamera.GetName();
	}

	public void PreInitialize(ISimpleSettingProcess process)
	{
		_process = process;
	}

	public override void Initialize(int playerIndex, bool isActive)
	{
		_blurObject.SetActive(value: false);
		if (IsActive())
		{
			MechaManager.LedIf[playerIndex].ButtonLedReset();
		}
		base.Initialize(playerIndex, isActive);
		if (!isActive)
		{
			_mainMonitorUIContensObject.SetActive(value: false);
			_subMonitorContentsObject.SetActive(value: false);
			return;
		}
		_volumeObject.SetActive(value: false);
		_menuButtonController.Initialize(playerIndex);
		OptionHeadphonevolumeID headPhoneVolume = Singleton<UserDataManager>.Instance.GetUserData(playerIndex).Option.HeadPhoneVolume;
		SetVolume(headPhoneVolume);
		_menuButtonController.SetVisible(false, 1, 2, 3, 4, 5);
		_userEntryPlate = UnityEngine.Object.Instantiate(_userEntryPrefaub, _userEntryObject.transform).GetComponent<UserEntryPlate>();
		_userEntryPlate.Initialize(playerIndex);
		_userEntryIconSetPlate = UnityEngine.Object.Instantiate(_userEntryPrefaub, _userEntryIconSetObject.transform).GetComponent<UserEntryPlate>();
		_userEntryIconSetPlate.Initialize(playerIndex);
		_toggleSwitchAnimator = _toggleSwitch.Instantiate<Animator>();
		_toggleSwitchAnimator.Play(Animator.StringToHash("Hide"), 0, 0f);
		_photoAggreesAnimator.Play(Animator.StringToHash("Hide"), 0, 0f);
		_partnerID = Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).Detail.EquipPartnerID;
		SoundManager.SetPartnerVoiceCue(monitorIndex, _partnerID);
		int id = Singleton<DataManager>.Instance.GetPartner(_partnerID).naviChara.id;
		if (Singleton<UserDataManager>.Instance.GetUserData(base.MonitorIndex).Detail.FirstPlayOnDay)
		{
			NavigationCharacter component = AssetManager.Instance().GetNaviCharaPrefab(id).GetComponent<NavigationCharacter>();
			_partnerCharacter = UnityEngine.Object.Instantiate(component, _partnerObject);
		}
	}

	public void SetFirstInformation()
	{
		_menuGroup.alpha = 0f;
		_menuButtonController.SetVisible(false, InputManager.ButtonSetting.Button04);
	}

	public void Play(Action<int> callback)
	{
		StartCoroutine(PlayCoroutine(callback));
	}

	private IEnumerator PlayCoroutine(Action<int> callback)
	{
		_menuGroup.alpha = 1f;
		_userEntryPlate.AnimHide();
		if (Singleton<UserDataManager>.Instance.GetUserData(base.MonitorIndex).Detail.FirstPlayOnDay)
		{
			_partnerAnimator.Play(Animator.StringToHash("In"), 0, 0f);
			_process.SetTimerVisible(monitorIndex, isVisible: false);
			yield return new WaitForSeconds(_partnerAnimator.GetCurrentAnimatorStateInfo(0).length);
			if (_partnerCharacter != null)
			{
				_partnerCharacter.Play(NavigationAnime.Welcom, 0f);
			}
			yield return new WaitForEndOfFrame();
			SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000248, monitorIndex);
			yield return new WaitForSeconds((_partnerCharacter == null) ? 2.5f : _partnerCharacter.GetCurrentAnimatorStateInfo().length);
			_partnerAnimator.Play(Animator.StringToHash("Out"), 0, 0f);
		}
		_messageAnimator.SetTrigger("Activated");
		_userEntryPlate.AnimIn();
		SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000026, monitorIndex);
		yield return new WaitForSeconds(1f);
		_menuButtonController.ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 5);
		OptionHeadphonevolumeID headPhoneVolume = Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).Option.HeadPhoneVolume;
		List<int> list = new List<int> { 3 };
		if (headPhoneVolume != OptionHeadphonevolumeID.Vol20)
		{
			list.Add(1);
		}
		if (headPhoneVolume != 0)
		{
			list.Add(2);
		}
		_menuButtonController.SetVisible(visible: true, list.ToArray());
		_volumeObject.SetActive(value: true);
		_volumeAnimator.SetTrigger("In");
		if (WebCamManager.IsAvailableCamera())
		{
			_menuButtonController.SetVisible(WebCamManager.IsAvailableCamera(), 5);
		}
		if (IsCodeReadUse)
		{
			_toggleSwitchAnimator.gameObject.SetActive(value: true);
			_toggleSwitchAnimator.Play(Animator.StringToHash("In"), 0, 0f);
			MechaManager.LedIf[monitorIndex].SetColor(0, Color.white);
		}
		else
		{
			_toggleSwitchAnimator.gameObject.SetActive(value: false);
		}
		if (CameraManager.IsAvailableCamera)
		{
			_photoAggreesAnimator.gameObject.SetActive(value: true);
			_photoAggreesAnimator.Play(Animator.StringToHash("In"), 0, 0f);
		}
		else
		{
			_photoAggreesAnimator.gameObject.SetActive(value: false);
		}
		callback?.Invoke(monitorIndex);
	}

	public void SetFirstInformationSkip()
	{
		_menuButtonController.SetVisible(true, InputManager.ButtonSetting.Button04);
		_menuButtonController.ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 4);
	}

	public void SetFrameTexture(Texture2D currentTexture)
	{
		_photoController.SetFrameTexture(currentTexture);
	}

	public void UpdateAdjustCapture(Vector2 v, int buttonIndex)
	{
		_photoController.UpdateAdjustCapture(v, buttonIndex);
	}

	public void ResetAdjustPosition()
	{
		_photoController.ResetAdjustPosition();
	}

	public void SetAdjustLength(int maxWidth, int maxHeight)
	{
		_photoController.SetAdjustLength(maxWidth, maxHeight);
	}

	public void SetMaterial(Material shutterMaterial)
	{
		_photoController.SetMaterial(shutterMaterial);
	}

	public void SetUserData(Texture2D icon, string userName, int rating, int dispRate, int totalAwake)
	{
		_userEntryIconSetPlate.SetUserData(icon, userName, rating, dispRate, totalAwake);
		_userEntryPlate.SetUserData(icon, userName, rating, dispRate, totalAwake);
	}

	public void SetUserDxPathData(Sprite frame, Material material)
	{
		_userEntryPlate.SetDxPathData(frame, material);
	}

	public void SetUserDxPathStatus(CodeReadProcess.CardStatus status)
	{
		_userEntryPlate.SetDxPathStatus(status);
	}

	public override void ViewUpdate()
	{
		if (isPlayerActive)
		{
			_photoController.ViewUpdate();
			_menuButtonController.ViewUpdate(GameManager.GetGameMSecAdd());
		}
	}

	public void SetResultTexture(Vector2 size, Texture2D texture)
	{
		_photoController.SetResultTexture(size, texture);
	}

	public void CleanupUpdate(float rate)
	{
		_photoController.CleanupUpdate(rate);
	}

	public void SetMenu2Wait()
	{
		_menuButtonController.SetVisible(false, 0, 1, 2, 3, 5, 7);
		_blurObject.SetActive(value: true);
	}

	public void SetMenu2Contract(bool isActive, Action<int> messageAction)
	{
		StartCoroutine(SetMenu2ContractCoroutine(isActive, messageAction));
	}

	private IEnumerator SetMenu2ContractCoroutine(bool isActive, Action<int> messageAction)
	{
		if (isActive)
		{
			_volumeAnimator.SetTrigger("Out");
			_userEntryPlate.AnimOut();
			_messageAnimator.SetTrigger("Disabled");
			MechaManager.LedIf[monitorIndex].SetColor(0, Color.black);
			_toggleSwitchAnimator.Play(Animator.StringToHash("Out"), 0, 0f);
			_photoAggreesAnimator.Play(Animator.StringToHash("Out"), 0, 0f);
			_menuButtonController.SetVisible(false, 0, 1, 2, 3, 5, 7);
			yield return new WaitForSeconds(0.5f);
			_volumeObject.SetActive(value: false);
			_menuButtonController.ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 9);
			_menuButtonController.ChangeButtonSymbol(InputManager.ButtonSetting.Button05, 10);
			_menuButtonController.SetVisible(true, 3, 4);
			messageAction?.Invoke(base.MonitorIndex);
			yield break;
		}
		messageAction?.Invoke(base.MonitorIndex);
		_menuButtonController.SetVisible(false, 3, 4);
		yield return new WaitForSeconds(0.5f);
		_messageAnimator.SetTrigger("Activated");
		_userEntryPlate.AnimIn();
		yield return new WaitForSeconds(0.5f);
		_volumeObject.SetActive(value: true);
		_volumeAnimator.SetTrigger("In");
		if (IsCodeReadUse)
		{
			MechaManager.LedIf[monitorIndex].SetColor(0, Color.white);
		}
		_toggleSwitchAnimator.Play(Animator.StringToHash("In"), 0, 0f);
		_photoAggreesAnimator.Play(Animator.StringToHash("In"), 0, 0f);
		_menuButtonController.ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 5);
		OptionHeadphonevolumeID headPhoneVolume = Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).Option.HeadPhoneVolume;
		List<int> list = new List<int>();
		if (headPhoneVolume != OptionHeadphonevolumeID.Vol20)
		{
			list.Add(1);
		}
		if (headPhoneVolume != 0)
		{
			list.Add(2);
		}
		_menuButtonController.SetVisible(visible: true, list.ToArray());
	}

	public void SetMenu2FrameSelectActive(bool isActive)
	{
		StartCoroutine(FrameSelectCoroutine(isActive));
	}

	private IEnumerator FrameSelectCoroutine(bool isActive)
	{
		if (isActive)
		{
			MechaManager.LedIf[monitorIndex].SetColor(0, Color.black);
			if (!_messageAnimator.GetCurrentAnimatorStateInfo(0).IsName("Disabled"))
			{
				_messageAnimator.SetTrigger("Disabled");
			}
			if (!_volumeAnimator.GetCurrentAnimatorStateInfo(0).IsName("Out"))
			{
				_volumeAnimator.SetTrigger("Out");
			}
			_menuButtonController.SetVisible(false, 0, 1, 2, 3, 5, 7);
			_toggleSwitchAnimator.Play(Animator.StringToHash("Out"), 0, 0f);
			_photoAggreesAnimator.Play(Animator.StringToHash("Out"), 0, 0f);
			yield return new WaitForSeconds(0.5f);
			_volumeObject.SetActive(value: false);
			_menuGroup.alpha = 0f;
			_menuButtonController.SetVisible(true, 4);
			_menuButtonController.ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 5);
			_menuButtonController.ChangeButtonSymbol(InputManager.ButtonSetting.Button05, 6);
			_photoController.SetMenu2FrameSelect(isActive: true);
		}
		else
		{
			_photoController.SetMenu2FrameSelect(isActive: false);
			yield return new WaitForSeconds(0.5f);
			_volumeObject.SetActive(value: true);
			_menuButtonController.SetVisible(false, 4);
			if (IsCodeReadUse)
			{
				MechaManager.LedIf[monitorIndex].SetColor(0, Color.white);
			}
			_toggleSwitchAnimator.Play(Animator.StringToHash("In"), 0, 0f);
			_photoAggreesAnimator.Play(Animator.StringToHash("In"), 0, 0f);
			OptionHeadphonevolumeID headPhoneVolume = Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).Option.HeadPhoneVolume;
			List<int> list = new List<int>();
			if (headPhoneVolume != OptionHeadphonevolumeID.Vol20)
			{
				list.Add(1);
			}
			if (headPhoneVolume != 0)
			{
				list.Add(2);
			}
			_menuButtonController.SetVisible(visible: true, list.ToArray());
			_menuGroup.alpha = 1f;
			_userEntryPlate.AnimIn();
			_volumeAnimator.SetTrigger("In");
			_messageAnimator.SetTrigger("Activated");
			_menuButtonController.ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 5);
		}
	}

	public void SetFrameSelect2PhotoPreparation(bool isActive)
	{
		if (_frameSelect2PhotoPreparation != null)
		{
			StopCoroutine(_frameSelect2PhotoPreparation);
			_frameSelect2PhotoPreparation = null;
		}
		_frameSelect2PhotoPreparation = StartCoroutine(SetFrameSelect2PhotoPreparationCoroutine(isActive));
	}

	private IEnumerator SetFrameSelect2PhotoPreparationCoroutine(bool isActive)
	{
		_menuButtonController.SetVisible(!isActive, 3);
		if (isActive)
		{
			yield return _photoController.SetFrameSelect2PhotoPreparation(isActive: true);
			_menuButtonController.SetPhotoShootingButton(isActive: true);
		}
		else
		{
			_menuButtonController.SetPhotoShootingButton(isActive: false);
			yield return _photoController.SetFrameSelect2PhotoPreparation(isActive: false);
		}
		_frameSelect2PhotoPreparation = null;
	}

	public void SetPreparation2Photographing()
	{
		StartCoroutine(SetPreparation2PhotographingCoroutine());
	}

	private IEnumerator SetPreparation2PhotographingCoroutine()
	{
		_menuButtonController.SetVisible(false, 3, 4);
		_menuButtonController.SetPhotoShootingButton(isActive: false);
		_menuButtonController.SetActivePhotoShhtingButton();
		yield return _photoController.SetPreparation2Photographing();
		yield return new WaitForSeconds(0.2f);
	}

	public void SetPhotoCountDown(int count)
	{
		_photoController.SetPhotoCountDown(count);
	}

	public void SetPhotoCountDown()
	{
		_photoController.SetPhotoCountDown();
	}

	public void SetPhotographing2Adjustment()
	{
		_menuButtonController.SetVisible(true, 3, 4);
		_photoController.SetPhotographing2Adjustment();
	}

	public void SetAdjust2Preparation()
	{
		_photoController.SetAdjust2Preparation();
		_menuButtonController.SetPhotoShootingButton(isActive: true);
		_menuButtonController.SetVisible(false, 3);
	}

	public void SetAdjust2Menu(Texture2D icon)
	{
		_photoController.SetAdjust2Menu();
		_messageAnimator.SetTrigger("Activated");
		_userEntryPlate.SetIconData(icon);
		_userEntryPlate.AnimIn();
		_menuGroup.alpha = 1f;
		OptionHeadphonevolumeID headPhoneVolume = Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).Option.HeadPhoneVolume;
		List<int> list = new List<int> { 3, 5 };
		_toggleSwitchAnimator.Play(Animator.StringToHash("In"), 0, 0f);
		_photoAggreesAnimator.Play(Animator.StringToHash("In"), 0, 0f);
		if (headPhoneVolume != OptionHeadphonevolumeID.Vol20)
		{
			list.Add(1);
		}
		if (headPhoneVolume != 0)
		{
			list.Add(2);
		}
		_menuButtonController.SetVisible(visible: true, list.ToArray());
		_menuButtonController.SetVisible(false, 4);
		_volumeObject.SetActive(value: true);
		_volumeAnimator.SetTrigger("In");
		if (IsCodeReadUse)
		{
			MechaManager.LedIf[monitorIndex].SetColor(0, Color.white);
		}
	}

	public void SetPhotographing2Cleanup()
	{
		_menuButtonController.SetVisible(false, 3, 4);
		_photoController.SetPhotographing2Cleanup();
		_userEntryIconSetPlate.AnimLoop();
	}

	public Texture2D GetCaptureTexture()
	{
		return _photoController.GetCaptureTexture();
	}

	public void SetFrameVisible(bool isShow)
	{
		_photoController.SetFrameVisible(isShow);
	}

	public void Scroll(int buttonIndex, Texture2D texture, string frameName)
	{
		SlideScroll(buttonIndex, texture, frameName);
	}

	public void SlideScroll(int buttonIndex, Texture2D texture, string frameName)
	{
		_photoController.Scroll(buttonIndex, frameName);
		_photoController.SetFrameSelectButtonAnimation(buttonIndex);
		_photoController.SetFrameTexture(texture);
	}

	public void SetScrollCard(bool isVisible)
	{
		_photoController.SetScrollCard(isVisible);
	}

	public void ChangeFrameName(string frameName)
	{
		_photoController.ChangeFrameName(frameName);
	}

	public void Shoot()
	{
		_photoController.Shoot();
	}

	public void SetActiveBlur(bool isActive)
	{
		_blurObject.SetActive(isActive);
	}

	public void PressedButton(InputManager.ButtonSetting buttonIndex)
	{
		_menuButtonController.SetAnimationActive((int)buttonIndex);
	}

	public void PressedFrameButton(InputManager.ButtonSetting buttonIndex)
	{
		_photoController.PressedFrameButton(buttonIndex);
	}

	public void SetVolume(OptionHeadphonevolumeID volume)
	{
		_volumeCounter.ChangeText(string.Format("{0,2}", int.Parse(volume.GetName())));
	}

	public void SetVisibleButton(bool isVisible, InputManager.ButtonSetting button)
	{
		_menuButtonController.SetVisible(isVisible, button);
	}

	public void SetCodeReadState(bool on)
	{
		_toggleSwitchAnimator.SetLayerWeight(1, on ? 1f : 0f);
	}

	public void SetPhotoAggreesState(bool on)
	{
		int layerIndex = _photoAggreesAnimator.GetLayerIndex("ON");
		_photoAggreesAnimator.SetLayerWeight(layerIndex, on ? 1f : 0f);
	}

	public void SetCodeReadToggleButton(bool to)
	{
		MechaManager.LedIf[monitorIndex].SetColorButtonPressed(0, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.White));
		string text = "Switch_" + (to ? "On" : "Off");
		_toggleSwitchAnimator.Play(Animator.StringToHash(text), 0, 0f);
		_toggleSwitchAnimator.SetLayerWeight(1, to ? 1f : 0f);
	}

	public void SetVisibleToggleButton(bool isVisible)
	{
		_toggleSwitchAnimator.gameObject.SetActive(isVisible);
	}

	public void SetPhotoAgreesToggleButton(bool to)
	{
		string text = "Switch_" + (to ? "On" : "Off");
		_photoAggreesAnimator.Play(Animator.StringToHash(text), 0, 0f);
		int layerIndex = _photoAggreesAnimator.GetLayerIndex("ON");
		_photoAggreesAnimator.SetLayerWeight(layerIndex, to ? 1f : 0f);
	}
}
