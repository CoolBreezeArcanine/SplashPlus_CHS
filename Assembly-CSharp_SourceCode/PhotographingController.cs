using System.Collections;
using Manager;
using TMPro;
using UI;
using UI.Common;
using UI.DaisyChainList;
using UnityEngine;
using UnityEngine.UI;

public class PhotographingController : MonoBehaviour
{
	private const float SHUTTER_TIME = 300f;

	private const float SHUTTER_CLOSE_TIME = 150f;

	private const float SubMonitorCutPreviewPositionX = 310f;

	private const float SubMonitorCutPreviewPositionY = 0f;

	public const string WIPE_SHADER_VALUE_NAME = "_Rate";

	[SerializeField]
	[Header("メイン画面 カット領域\u3000メイン側基準")]
	[Tooltip("カメラプレビュー表示切替オブジェクト")]
	private GameObject _previewObject;

	[SerializeField]
	[Header("メイン画面\u3000カメラプレビュー")]
	private RawImage _cameraPreview;

	[SerializeField]
	[Header("メイン画面\u3000フレーム表示")]
	private RawImage _frameImage;

	[SerializeField]
	[Tooltip("上画面カメラプレビュー")]
	[Header("上画面カメラ表示")]
	private RawImage _subMonitorAllPreview;

	[SerializeField]
	[Header("上画面フレームの疑似的な位置表示")]
	private RawImage _subMonitorPseudoFramePreview;

	[SerializeField]
	[Header("上画面カメラカットプレビュー")]
	[Tooltip("上画面プレビュー表示切替")]
	private RectTransform _subMonitorPreviewObject;

	[SerializeField]
	[Header("上画面カメラフレームプレビュー")]
	private RawImage _subMonitorFramePreview;

	[SerializeField]
	[Header("上画面フレーム表示")]
	private RawImage _subMonitorFrame;

	[SerializeField]
	[Header("上画面シャッター")]
	[Tooltip("上画面カメラシャッター")]
	private Image _shutter;

	[SerializeField]
	[Header("上画面メッセージ")]
	private RectTransform _subMonitorMessageBalloon;

	[SerializeField]
	[Header("上画面カウントダウンテキスト")]
	private TextMeshProUGUI _subMonitorCountDownText;

	[SerializeField]
	private RectTransform _subMonitorPreviewTargetObject;

	[SerializeField]
	private MultipleImage _countDownImage;

	[SerializeField]
	[Header("補正ボタン表示切替")]
	private GameObject _adjustmentObject;

	[SerializeField]
	[Header("カメラ関連ボタン管理")]
	private AdjustButtonController _adjustButtonController;

	[SerializeField]
	[Tooltip("フレーム切り替えボタン管理")]
	private FrameSelectButtonController _frameSelectButtonController;

	[SerializeField]
	[Header("フレームリスト")]
	private FrameChainList _frameChainList;

	[SerializeField]
	private TextMeshProUGUI _frameNameText;

	[SerializeField]
	[Header("アイコンセットアニメーション")]
	private Image _resultImage;

	[SerializeField]
	private Animator _cleanUpAnimator;

	[SerializeField]
	[Header("シーケンス")]
	private CanvasGroup _frameSelectGroup;

	[SerializeField]
	private CanvasGroup _preparationGroup;

	[SerializeField]
	private CanvasGroup _countDownGroup;

	[SerializeField]
	private CanvasGroup _cleanUpGroup;

	[SerializeField]
	[Header("シーケンス関連アニメーター")]
	private SelectorBackgroundController _frameSelectAnimation;

	[SerializeField]
	private Animator _preparationChassisAnimator;

	[SerializeField]
	[Header("メッセージ")]
	private GameObject _photoMessage;

	private WebCamTexture _originalTexture;

	private Texture2D _previewTexture;

	private Texture2D _mirrorPreviewTexture;

	private Color32[] _originalColors;

	private Color32[] _previewColor;

	private Color32[] _mirrorColors;

	private float _timeCounter;

	private bool _isPhotographing;

	private bool _isShutter;

	private Vector2 _adjustBasePosition;

	private int _maxAdjustWidth;

	private int _maxAdjustHeight;

	private int _verticalCounter;

	private int _horizontalCounter;

	private int _playerIndex;

	public void Initialize(int monitorIndex, IPhotoShootProcess process)
	{
		_playerIndex = monitorIndex;
		_previewTexture = new Texture2D(CameraManager.GameCameraParam.Width, CameraManager.GameCameraParam.Height, TextureFormat.ARGB32, mipChain: false);
		_mirrorPreviewTexture = new Texture2D(CameraManager.GameCameraParam.Width, CameraManager.GameCameraParam.Height, TextureFormat.ARGB32, mipChain: false);
		_mirrorColors = new Color32[CameraManager.GameCameraParam.Width * CameraManager.GameCameraParam.Height];
		_previewColor = new Color32[262144];
		_originalColors = new Color32[CameraManager.GameCameraParam.Width * CameraManager.GameCameraParam.Height];
		_originalTexture = WebCamManager.GetTexture();
		_subMonitorAllPreview.texture = _originalTexture;
		float x = ((monitorIndex == 0) ? (-220) : 220);
		_subMonitorAllPreview.rectTransform.anchoredPosition = new Vector2(x, 0f);
		_subMonitorPseudoFramePreview.texture = _originalTexture;
		_subMonitorPreviewObject.anchoredPosition = new Vector2((monitorIndex == 0) ? 310f : (-310f), 0f);
		float num = _subMonitorPreviewObject.sizeDelta.x / 512f;
		float num2 = _previewObject.GetComponent<RectTransform>().sizeDelta.x / 512f;
		float num3 = ((monitorIndex == 0) ? ((0f - 1280f * num) / 4f) : (1280f * num / 4f));
		float x2 = 1280f * num2 / 4f * (float)((monitorIndex != 0) ? 1 : (-1));
		_adjustBasePosition = new Vector2(x2, 0f);
		_cameraPreview.rectTransform.anchoredPosition = _adjustBasePosition;
		_subMonitorFramePreview.rectTransform.anchoredPosition = new Vector2(0f - num3, 0f);
		Vector2 sizeDelta = _subMonitorFramePreview.rectTransform.sizeDelta;
		Vector2 sizeDelta2 = _subMonitorPreviewObject.sizeDelta;
		Vector2 sizeDelta3 = _subMonitorAllPreview.rectTransform.sizeDelta;
		_subMonitorPseudoFramePreview.rectTransform.sizeDelta = new Vector2(sizeDelta3.x / sizeDelta.x * sizeDelta2.x, sizeDelta3.y / sizeDelta.y * sizeDelta2.y);
		Vector2 anchoredPosition = _subMonitorFramePreview.rectTransform.anchoredPosition;
		_subMonitorPseudoFramePreview.rectTransform.anchoredPosition = anchoredPosition * (sizeDelta3.x / sizeDelta.x) * -1f;
		_subMonitorPseudoFramePreview.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
		_subMonitorPseudoFramePreview.color = new Color(1f, 1f, 1f, 0.5f);
		_subMonitorMessageBalloon.gameObject.SetActive(value: false);
		_subMonitorAllPreview.gameObject.SetActive(value: false);
		_subMonitorPreviewObject.gameObject.SetActive(value: false);
		_subMonitorFrame.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
		_subMonitorFrame.gameObject.SetActive(value: false);
		_frameImage.gameObject.SetActive(value: false);
		_adjustmentObject.gameObject.SetActive(value: false);
		_countDownGroup.alpha = 0f;
		_preparationGroup.alpha = 0f;
		_cleanUpGroup.alpha = 0f;
		if (CameraManager.IsAvailableCamera)
		{
			UpdatePreview();
		}
		_adjustButtonController.Initialize(monitorIndex);
		_frameSelectButtonController.Initialize(monitorIndex);
		_frameChainList.AdvancedInitialize(process, monitorIndex);
		_frameChainList.Initialize();
		_subMonitorPreviewTargetObject.localScale = new Vector3((_playerIndex == 0) ? 1 : (-1), 1f, 1f);
		_previewObject.SetActive(value: false);
		_frameSelectGroup.alpha = 0f;
	}

	public void UpdateAdjustCapture(Vector2 v, int buttonIndex)
	{
		UpdateAdjustCapturePosition(v);
		SetAdjustButtonAnimation(buttonIndex);
		UpdateAdjustCounter(buttonIndex);
		ChangeAdjustButtonTopImage();
	}

	private void UpdateAdjustCapturePosition(Vector2 v)
	{
		_cameraPreview.rectTransform.anchoredPosition = _adjustBasePosition + v;
	}

	private void ChangeAdjustButtonTopImage()
	{
		SetTopImage(_verticalCounter != _maxAdjustHeight, 0);
		SetTopImage(_verticalCounter != -_maxAdjustHeight, 2);
		SetTopImage(_horizontalCounter != _maxAdjustWidth, 1);
		SetTopImage(_horizontalCounter != -_maxAdjustWidth, 3);
	}

	private void UpdateAdjustCounter(int buttonIndex)
	{
		switch (buttonIndex)
		{
		case 0:
			_verticalCounter++;
			break;
		case 1:
			_horizontalCounter++;
			break;
		case 2:
			_verticalCounter--;
			break;
		case 3:
			_horizontalCounter--;
			break;
		}
	}

	public void ResetAdjustPosition()
	{
		_cameraPreview.rectTransform.anchoredPosition = _adjustBasePosition;
	}

	public void SetMaterial(Material shutterMaterial)
	{
		_shutter.material = shutterMaterial;
		_shutter.material.SetFloat("_Rate", 1f);
	}

	private void UpdatePreview()
	{
		if (_originalTexture == null)
		{
			return;
		}
		_originalTexture.GetPixels32(_originalColors);
		_ = _frameImage.rectTransform.sizeDelta;
		int width = CameraManager.GameCameraParam.Width;
		int height = CameraManager.GameCameraParam.Height;
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				int num = j + i * width;
				int num2 = width - j - 1 + i * width;
				_mirrorColors[num2] = _originalColors[num];
			}
		}
		_mirrorPreviewTexture.SetPixels32(_mirrorColors);
		_mirrorPreviewTexture.Apply();
		_subMonitorAllPreview.texture = _mirrorPreviewTexture;
		_subMonitorFramePreview.texture = _mirrorPreviewTexture;
		_previewTexture.SetPixels32(_originalColors);
		_previewTexture.Apply();
		_cameraPreview.texture = _previewTexture;
	}

	public void ViewUpdate()
	{
		if (_originalTexture != null && _originalTexture.didUpdateThisFrame && !_isPhotographing)
		{
			UpdatePreview();
		}
		_frameSelectButtonController.ViewUpdate(GameManager.GetGameMSecAdd());
		_adjustButtonController.ViewUpdate(GameManager.GetGameMSecAdd());
		if (_isShutter)
		{
			_timeCounter += GameManager.GetGameMSecAdd();
			if (_timeCounter >= 300f)
			{
				_timeCounter = 0f;
				_isShutter = false;
				_shutter.material.SetFloat("_Rate", 1f);
			}
			else if (_timeCounter <= 150f)
			{
				_shutter.material.SetFloat("_Rate", 1f - Mathf.InverseLerp(0f, 150f, _timeCounter));
			}
			else
			{
				_shutter.material.SetFloat("_Rate", Mathf.InverseLerp(150f, 300f, _timeCounter));
			}
		}
		_frameChainList.ViewUpdate();
	}

	public void SetResultTexture(Vector2 size, Texture2D texture)
	{
		_resultImage.sprite = Sprite.Create(texture, new Rect(Vector2.zero, size), new Vector2(0.5f, 0.5f));
	}

	public void CleanupUpdate(float rate)
	{
	}

	public void SetMenu2FrameSelect(bool isActive)
	{
		StartCoroutine(SetMenu2FrameSelectCoroutine(isActive));
	}

	private IEnumerator SetMenu2FrameSelectCoroutine(bool isActive)
	{
		if (isActive)
		{
			_frameSelectButtonController.SetVisibleImmediate(false, 0, 1);
			_frameSelectGroup.alpha = 1f;
			_frameSelectAnimation.Play(SelectorBackgroundController.AnimationType.In);
			yield return new WaitForSeconds(0.6f);
			_frameChainList.Deploy();
			_frameChainList.Play();
			_frameSelectButtonController.SetVisible(true, 0, 1);
		}
		else
		{
			_frameSelectButtonController.SetVisible(false, 0, 1);
			_frameSelectAnimation.Play(SelectorBackgroundController.AnimationType.Out);
			_frameChainList.RemoveOut();
			yield return new WaitForSeconds(0.55f);
			_frameSelectGroup.alpha = 0f;
		}
		_subMonitorAllPreview.gameObject.SetActive(isActive);
		_subMonitorFrame.gameObject.SetActive(isActive);
		_subMonitorPreviewObject.gameObject.SetActive(isActive);
		SetVisibleFrameSelectButton(isActive);
	}

	public Coroutine SetFrameSelect2PhotoPreparation(bool isActive)
	{
		return StartCoroutine(SetFrameSelect2PhotoPreparationCoroutine(isActive));
	}

	private IEnumerator SetFrameSelect2PhotoPreparationCoroutine(bool isActive)
	{
		SelectorBackgroundController.AnimationType trigger = (isActive ? SelectorBackgroundController.AnimationType.Out : SelectorBackgroundController.AnimationType.In);
		if (isActive)
		{
			_frameSelectButtonController.SetVisible(false, 0, 1);
			_frameSelectAnimation.Play(trigger);
			_frameChainList.RemoveOut();
			yield return new WaitForSeconds(0.2f);
			yield return new WaitWhile(delegate
			{
				_frameSelectGroup.alpha -= 0.05f;
				return _frameSelectGroup.alpha > 0f;
			});
			yield return new WaitForSeconds(0.2f);
			_preparationChassisAnimator.SetTrigger((_playerIndex == 0) ? "Left_Show" : "Right_Show");
			_preparationGroup.alpha = 1f;
			yield break;
		}
		_preparationChassisAnimator.SetTrigger("Hide");
		yield return new WaitWhile(delegate
		{
			_preparationGroup.alpha -= 0.05f;
			return _preparationGroup.alpha > 0f;
		});
		_frameSelectAnimation.Play(trigger);
		yield return new WaitWhile(delegate
		{
			_frameSelectGroup.alpha += 0.05f;
			return _frameSelectGroup.alpha < 1f;
		});
		_preparationGroup.alpha = 0f;
		_frameSelectGroup.alpha = 1f;
		yield return new WaitForSeconds(0.5f);
		_frameChainList.Deploy();
		_frameChainList.Play();
		_frameSelectButtonController.SetVisible(true, 0, 1);
	}

	public Coroutine SetPreparation2Photographing()
	{
		return StartCoroutine(SetPreparation2PhotographingCoroutine());
	}

	private IEnumerator SetPreparation2PhotographingCoroutine()
	{
		_photoMessage.SetActive(value: false);
		_preparationChassisAnimator.SetTrigger((_playerIndex == 0) ? "Left_PhotoLoop" : "Right_PhotoLoop");
		yield return new WaitForSeconds(0.5f);
		_subMonitorMessageBalloon.gameObject.SetActive(value: true);
		_countDownGroup.alpha = 1f;
	}

	public void SetPhotoCountDown(int count)
	{
		if (count <= 3)
		{
			if (count - 1 < 0)
			{
				_countDownImage.color = Color.clear;
				return;
			}
			_countDownImage.color = Color.white;
			_countDownImage.ChangeSprite(count - 1);
		}
	}

	public void SetPhotoCountDown()
	{
		_countDownImage.color = Color.clear;
	}

	public void SetPhotographing2Adjustment()
	{
		_photoMessage.SetActive(value: true);
		_preparationGroup.alpha = 0f;
		_countDownGroup.alpha = 0f;
		_subMonitorPreviewObject.gameObject.SetActive(value: false);
		_subMonitorAllPreview.gameObject.SetActive(value: false);
		_subMonitorFrame.gameObject.SetActive(value: false);
		_subMonitorMessageBalloon.gameObject.SetActive(value: false);
		SetVisibleAdjustButton(isActive: true);
		_frameImage.gameObject.SetActive(value: true);
		_previewObject.SetActive(value: true);
		_adjustmentObject.gameObject.SetActive(value: true);
		ResetTopImage();
		_adjustButtonController.SetLoopButton();
	}

	public void SetAdjust2Preparation()
	{
		_isPhotographing = false;
		_preparationChassisAnimator.SetTrigger((_playerIndex == 0) ? "Left_Show" : "Right_Show");
		_frameImage.gameObject.SetActive(value: false);
		_previewObject.SetActive(value: false);
		SetVisibleAdjustButton(isActive: false);
		_adjustmentObject.gameObject.SetActive(value: false);
		_subMonitorPreviewObject.gameObject.SetActive(value: true);
		_subMonitorAllPreview.gameObject.SetActive(value: true);
		_subMonitorFrame.gameObject.SetActive(value: true);
		_preparationGroup.alpha = 1f;
	}

	public void SetAdjust2Menu()
	{
		_cleanUpGroup.alpha = 0f;
		_isPhotographing = false;
		SetVisibleAdjustButton(isActive: false);
		_adjustmentObject.gameObject.SetActive(value: false);
		_previewObject.gameObject.SetActive(value: false);
		_frameImage.gameObject.SetActive(value: false);
		_resultImage.transform.localScale = Vector3.one;
	}

	public void SetPhotographing2Cleanup()
	{
		_cleanUpGroup.alpha = 1f;
		_cleanUpAnimator.SetTrigger("Activated");
		_previewObject.gameObject.SetActive(value: false);
		SetVisibleAdjustButton(isActive: false);
		_adjustmentObject.gameObject.SetActive(value: false);
	}

	public Texture2D GetCaptureTexture()
	{
		return _previewTexture;
	}

	public void SetFrameVisible(bool isShow)
	{
		_frameImage.gameObject.SetActive(isShow);
	}

	public void SetFrameTexture(Texture2D texture)
	{
		RawImage frameImage = _frameImage;
		RawImage subMonitorFrame = _subMonitorFrame;
		Texture texture3 = (_subMonitorPseudoFramePreview.texture = texture);
		Texture texture6 = (frameImage.texture = (subMonitorFrame.texture = texture3));
		_previewColor = new Color32[texture.width * texture.height];
	}

	public void Scroll(int buttonIndex, string frameName)
	{
		switch (buttonIndex)
		{
		case 0:
			_frameChainList.Scroll(Direction.Left);
			break;
		case 1:
			_frameChainList.Scroll(Direction.Right);
			break;
		}
		ChangeFrameName(frameName);
	}

	public void SetFrameSelectButtonAnimation(int buttonIndex)
	{
		_frameSelectButtonController.SetAnimationActive(buttonIndex);
	}

	public void SetScrollCard(bool isVisible)
	{
		_frameChainList.SetScrollCard(isVisible);
	}

	public void ChangeFrameName(string frameName)
	{
		_frameNameText.text = frameName;
	}

	public void SetAdjustLength(int maxWidth, int maxHeight)
	{
		_maxAdjustWidth = maxWidth;
		_maxAdjustHeight = maxHeight;
		_horizontalCounter = 0;
		_verticalCounter = 0;
		ResetTopImage();
	}

	private void SetAdjustButtonAnimation(int buttonIndex)
	{
		_adjustButtonController.SetAnimationActive(buttonIndex);
	}

	private void ResetTopImage()
	{
		_adjustButtonController.SetTopImage(true, 0, 1, 2, 3);
	}

	private void SetTopImage(bool isNormal, int buttonIndex)
	{
		_adjustButtonController.SetTopImage(isNormal, buttonIndex);
	}

	private void SetVisibleAdjustButton(bool isActive)
	{
		_adjustButtonController.SetVisible(isActive, 0, 1, 2, 3);
	}

	public void SetVisibleFrameSelectButton(bool isActive)
	{
		_frameSelectButtonController.ChangeActiveAnimationFlag(isActive);
	}

	public void Shoot()
	{
		_isPhotographing = true;
		_isShutter = true;
	}

	public Texture2D GetCameraFrameTexture()
	{
		int num = (int)_frameImage.rectTransform.sizeDelta.x;
		int num2 = (int)_frameImage.rectTransform.sizeDelta.y;
		Texture2D texture2D = new Texture2D(num, num2, TextureFormat.ARGB32, mipChain: false);
		Color[] array = new Color[num * num2];
		array = ((Texture2D)_frameImage.texture).GetPixels();
		Color[] array2 = new Color[num * num2];
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num; j++)
			{
				int num3 = j + i * num;
				array2[num3] = array[num3] * array[num3].a + (Color)_previewColor[num3] * (1f - array[num3].a);
			}
		}
		texture2D.SetPixels(array2);
		texture2D.Apply();
		return texture2D;
	}

	public void PressedFrameButton(InputManager.ButtonSetting buttonIndex)
	{
		_frameSelectButtonController.SetAnimationActive((int)buttonIndex);
	}
}
