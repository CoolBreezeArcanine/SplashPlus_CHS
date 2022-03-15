using System;
using System.Collections;
using IO;
using Manager;
using UnityEngine;

public class PhotoShootMonitor : MonitorBase
{
	[SerializeField]
	private PhotographingController _photoController;

	[SerializeField]
	[Header("ボタン管理")]
	private MenuButtonController _menuButtonController;

	[SerializeField]
	[Header("ユーザー情報プレート")]
	private GameObject _userEntryObject;

	[SerializeField]
	[Tooltip("ユーザーエントリー情報Prefab")]
	private GameObject _userEntryPrefab;

	private UserEntryPlate _userEntryIconSetPlate;

	private Coroutine _frameSelect2PhotoPreparation;

	public override void Initialize(int playerIndex, bool isActive)
	{
		base.Initialize(playerIndex, isActive);
		if (IsActive())
		{
			MechaManager.LedIf[playerIndex].ButtonLedReset();
		}
		_menuButtonController.Initialize(playerIndex);
		_menuButtonController.SetVisible(false, 1, 2, 3, 4, 5);
		_userEntryIconSetPlate = UnityEngine.Object.Instantiate(_userEntryPrefab, _userEntryObject.transform).GetComponent<UserEntryPlate>();
		_userEntryIconSetPlate.Initialize(playerIndex);
	}

	public void SetData(IPhotoShootProcess process)
	{
		_photoController.Initialize(monitorIndex, process);
	}

	public override void ViewUpdate()
	{
		if (isPlayerActive)
		{
			_photoController.ViewUpdate();
			_menuButtonController.ViewUpdate(GameManager.GetGameMSecAdd());
		}
	}

	public void PressedButton(InputManager.ButtonSetting buttonIndex)
	{
		_menuButtonController.SetAnimationActive((int)buttonIndex);
	}

	public void SetUserData(Texture2D icon, string userName, int rating, int dispRate, int totalAwake)
	{
		_userEntryIconSetPlate.SetUserData(icon, userName, rating, dispRate, totalAwake);
	}

	public void ChangeFrameName(string frameName)
	{
		_photoController.ChangeFrameName(frameName);
	}

	public void SetFirstInformation()
	{
		_menuButtonController.SetVisible(false, InputManager.ButtonSetting.Button04);
	}

	public void SetFirstInformationSkip()
	{
		_menuButtonController.SetVisible(true, InputManager.ButtonSetting.Button04);
		_menuButtonController.ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 4);
	}

	public void SetMenu2Wait()
	{
		_menuButtonController.SetVisible(false, 0, 1, 2, 3, 4, 5, 6, 7);
	}

	public void SetMaterial(Material shutterMaterial)
	{
		_photoController.SetMaterial(shutterMaterial);
	}

	public void SetContract(bool isActive, Action<int> messageAction, Action<int> onFinish)
	{
		StartCoroutine(SetContractCoroutine(isActive, messageAction, onFinish));
	}

	private IEnumerator SetContractCoroutine(bool isActive, Action<int> messageAction, Action<int> onFinish)
	{
		if (isActive)
		{
			_userEntryIconSetPlate.AnimHide();
			_menuButtonController.SetVisible(false, 0, 1, 2, 3, 4, 5, 6, 7);
			messageAction?.Invoke(base.MonitorIndex);
			yield return new WaitForSeconds(1f);
			_menuButtonController.ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 9);
			_menuButtonController.ChangeButtonSymbol(InputManager.ButtonSetting.Button05, 10);
			_menuButtonController.SetVisible(true, 3, 4);
			yield return new WaitForSeconds(0.3f);
			onFinish?.Invoke(base.MonitorIndex);
		}
		else
		{
			messageAction?.Invoke(base.MonitorIndex);
			_menuButtonController.SetVisible(false, 3, 4);
			onFinish?.Invoke(base.MonitorIndex);
		}
	}

	public void SetFrameSelectActive(bool isActive)
	{
		StartCoroutine(FrameSelectCoroutine(isActive));
	}

	private IEnumerator FrameSelectCoroutine(bool isActive)
	{
		if (isActive)
		{
			MechaManager.LedIf[monitorIndex].SetColor(0, Color.black);
			_menuButtonController.SetVisible(false, 0, 1, 2, 3, 4, 5, 6, 7);
			_photoController.SetMenu2FrameSelect(isActive: true);
			yield return new WaitForSeconds(1f);
			_menuButtonController.SetVisible(true, 4);
			_menuButtonController.ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 5);
			_menuButtonController.ChangeButtonSymbol(InputManager.ButtonSetting.Button05, 3);
		}
		else
		{
			_photoController.SetMenu2FrameSelect(isActive: false);
			yield return new WaitForSeconds(0.5f);
			_menuButtonController.SetVisible(false, 4);
			if (WebCamManager.IsAvailableCamera())
			{
				_menuButtonController.SetVisible(true, 5);
			}
			_menuButtonController.ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 5);
		}
	}

	public void SetFrameSelect2Cancel(bool toCancel, Action<int> callback)
	{
		StartCoroutine(FrameSelect2CancelCoroutine(toCancel, callback));
	}

	private IEnumerator FrameSelect2CancelCoroutine(bool toCancel, Action<int> callback)
	{
		if (toCancel)
		{
			_photoController.SetMenu2FrameSelect(isActive: false);
			yield return new WaitForSeconds(0.6f);
			callback?.Invoke(monitorIndex);
			_menuButtonController.SetVisible(false, 0, 1, 2, 3, 4, 5, 6, 7);
			yield return new WaitForSeconds(1f);
			_menuButtonController.ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 0);
			_menuButtonController.ChangeButtonSymbol(InputManager.ButtonSetting.Button05, 1);
			_menuButtonController.SetVisible(true, 4, 5);
		}
		else
		{
			MechaManager.LedIf[monitorIndex].SetColor(0, Color.black);
			_menuButtonController.SetVisible(false, 0, 1, 2, 3, 4, 5, 6, 7);
			yield return new WaitForSeconds(1f);
			_menuButtonController.SetVisible(true, 3, 4);
			_menuButtonController.ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 5);
			_menuButtonController.ChangeButtonSymbol(InputManager.ButtonSetting.Button05, 3);
			_photoController.SetMenu2FrameSelect(isActive: true);
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
		if (isActive)
		{
			_menuButtonController.SetVisible(false, 0, 1, 2, 3, 4, 5, 6, 7);
			yield return _photoController.SetFrameSelect2PhotoPreparation(isActive: true);
			yield return new WaitForSeconds(0.5f);
			_menuButtonController.SetVisible(true, 4);
			_menuButtonController.SetPhotoShootingButton(isActive: true);
		}
		else
		{
			_menuButtonController.SetVisible(false, 0, 1, 2, 3, 4, 5, 6, 7);
			_menuButtonController.SetPhotoShootingButton(isActive: false);
			yield return _photoController.SetFrameSelect2PhotoPreparation(isActive: false);
			yield return new WaitForSeconds(0.5f);
			_menuButtonController.SetVisible(true, 3, 4);
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

	public void SetPhotographing2Adjustment()
	{
		_menuButtonController.SetVisible(true, 3, 4);
		_photoController.SetPhotographing2Adjustment();
	}

	public void SetPhotoCountDown(int count)
	{
		_photoController.SetPhotoCountDown(count);
	}

	public void SetPhotoCountDown()
	{
		_photoController.SetPhotoCountDown();
	}

	public void Shoot()
	{
		_photoController.Shoot();
	}

	public void SetResultTexture(Vector2 size, Texture2D texture)
	{
		_photoController.SetResultTexture(size, texture);
	}

	public Texture2D GetCaptureTexture()
	{
		return _photoController.GetCaptureTexture();
	}

	public void SetAdjustLength(int maxWidth, int maxHeight)
	{
		_photoController.SetAdjustLength(maxWidth, maxHeight);
	}

	public void UpdateAdjustCapture(Vector2 v, int buttonIndex)
	{
		_photoController.UpdateAdjustCapture(v, buttonIndex);
	}

	public void ResetAdjustPosition()
	{
		_photoController.ResetAdjustPosition();
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
		_userEntryIconSetPlate.SetIconData(icon);
		_userEntryIconSetPlate.AnimIn();
	}

	public void SetPhotographing2Cleanup()
	{
		_menuButtonController.SetVisible(false, 0, 1, 2, 3, 4, 5, 6, 7);
		_photoController.SetPhotographing2Cleanup();
		_userEntryIconSetPlate.AnimLoop();
	}

	public void CleanupUpdate(float rate)
	{
		_photoController.CleanupUpdate(rate);
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

	public void SetFrameVisible(bool isShow)
	{
		_photoController.SetFrameVisible(isShow);
	}

	public void SetFrameTexture(Texture2D currentTexture)
	{
		_photoController.SetFrameTexture(currentTexture);
	}
}
