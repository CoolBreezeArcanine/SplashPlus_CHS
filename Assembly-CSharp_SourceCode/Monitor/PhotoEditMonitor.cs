using System;
using System.Collections;
using DB;
using MAI2System;
using Manager;
using Manager.UserDatas;
using Monitor.PhotoEdit;
using Monitor.Result;
using Process;
using TMPro;
using UI;
using UI.DaisyChainList;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Monitor
{
	public class PhotoEditMonitor : MonitorBase
	{
		[SerializeField]
		[Header("ジャケット関連")]
		private RawImage _jacketImage;

		[SerializeField]
		private RawImage _nextJacketImage;

		[SerializeField]
		private Animator _nextTrackAnimator;

		[SerializeField]
		[Header("ボタン管理")]
		private PhotoEditButtonController _buttonController;

		[SerializeField]
		private TextMeshProUGUI _infomatinonText;

		[SerializeField]
		[Header("トラック数")]
		private SpriteCounter _trackNumText;

		[SerializeField]
		private SpriteCounter _trackDoubleText;

		[SerializeField]
		[Header("写真数")]
		private TextMeshProUGUI _photoNumText;

		[SerializeField]
		[Header("カスタマイズリスト管理")]
		private EditChainlist _editChainlist;

		[SerializeField]
		private Animator _photoAnimator;

		[SerializeField]
		[Header("ブラーオブジェクト")]
		private GameObject _blurObject;

		[SerializeField]
		private GameObject _fromtBlurObject;

		[SerializeField]
		[Header("カードプレビューオブジェクト")]
		private GameObject _overlayPreviewObject;

		[SerializeField]
		[Header("カードプレビューイメージ")]
		private RawImage _overlayImage;

		[SerializeField]
		[Header("カードプレビューアニメーター")]
		private Animator _overlayAnimator;

		[SerializeField]
		[Header("拡大確認オブジェクト")]
		private GameObject _zoomPreviewObject;

		[SerializeField]
		[Header("拡大確認イメージ")]
		private RawImage _zoomPreviewImage;

		[SerializeField]
		[Header("拡大確認アニメーター")]
		private Animator _zoomPreviewAnimator;

		[SerializeField]
		[Header("リザルトカード")]
		private Transform _parentOverlayObject;

		[SerializeField]
		private GameObject _originalOverlayObject;

		[SerializeField]
		[Header("アニメーション関連")]
		private Animator _customBackAnimator;

		[SerializeField]
		[Header("拡大画面メッセージオブジェクト")]
		private GameObject _previewMessageObject;

		[SerializeField]
		private InstantiateGenerator _scoreBoardGenerator;

		[SerializeField]
		private TextMeshProUGUI _touchZoomText;

		private RenderTexture _overlayRenderTexture;

		private ScoreBoardController _scoreBoardController;

		private PhotoDetailsObject _detailsObject;

		private Coroutine _mainMenuCoroutine;

		private IPhotoEditProcess _editProcess;

		public Camera OverlayCamera { get; private set; }

		private void Awake()
		{
			_touchZoomText.text = CommonMessageID.PhotoEditTouchZoom.GetName();
		}

		public void AdvancedInitialize(IPhotoEditProcess editProcess, string shopName, string dateTime, PhotoeditLayoutID layout)
		{
			_editProcess = editProcess;
			if (IsActive())
			{
				_editChainlist.Initialize();
				_editChainlist.AdvancedInitialize(_editProcess, monitorIndex);
				_detailsObject.Initialize();
				_detailsObject.SetBasicData(shopName, dateTime);
				ChangeLayout(layout);
			}
		}

		public override void Initialize(int playerIndex, bool isActive)
		{
			base.Initialize(playerIndex, isActive);
			_previewMessageObject.SetActive(value: false);
			if (!isActive)
			{
				SetDisable();
				return;
			}
			_blurObject.SetActive(value: false);
			_fromtBlurObject.SetActive(value: false);
			_buttonController.Initialize(playerIndex);
			OverlayCamera = UnityEngine.Object.Instantiate(_originalOverlayObject, _parentOverlayObject).GetComponent<Camera>();
			Transform transform = OverlayCamera.transform.Find("UI_Photograph_Canvas/UI_Photograph");
			_detailsObject = transform.GetComponent<PhotoDetailsObject>();
			_overlayRenderTexture = new RenderTexture(ConstParameter.PhotoSize.x, ConstParameter.PhotoSize.y, 24);
			OverlayCamera.targetTexture = _overlayRenderTexture;
			_overlayImage.texture = _overlayRenderTexture;
			_zoomPreviewImage.texture = _overlayRenderTexture;
			_zoomPreviewObject.SetActive(value: false);
			OverlayCamera.transform.localPosition = new Vector3((playerIndex == 0) ? (-150) : 150, 0f, -20f);
			_buttonController.SetVisibleEditButton(isVisible: false);
			_buttonController.SetVisibleZoomButton(isVisible: false);
			_buttonController.SetVisible(false, 3, 4);
			_buttonController.SetVisibleImmediate(false, 2);
			_buttonController.SetVisibleImmediate(false, 5);
			_scoreBoardController = _scoreBoardGenerator.Instantiate<ScoreBoardController>();
			Main.alpha = 0f;
		}

		public void Play()
		{
			if (isPlayerActive)
			{
				_blurObject.SetActive(value: false);
				_buttonController.SetVisibleEditButton(isVisible: false);
				_buttonController.SetVisibleZoomButton(isVisible: false);
				_buttonController.SetVisible(false, 2, 4, 5);
				_customBackAnimator.Play(Animator.StringToHash("Disabled"));
				_nextTrackAnimator.Play(Animator.StringToHash("Disabled"));
				_overlayAnimator.Play(Animator.StringToHash("Disabled"));
				StartCoroutine(PlayCoroutine());
			}
		}

		private IEnumerator PlayCoroutine()
		{
			yield return new WaitForSeconds(0.2f);
			Main.alpha = 1f;
			_customBackAnimator.Play(Animator.StringToHash("In"));
			_nextTrackAnimator.Play(Animator.StringToHash("In"));
			_overlayAnimator.Play(Animator.StringToHash("In"));
			_buttonController.SetVisibleEditButton(isVisible: true);
			_buttonController.SetVisibleZoomButton(isVisible: true);
		}

		public void AfterPlay()
		{
			ChangeButtonSymbol(InputManager.ButtonSetting.Button04, PhotoEditProcess.SelectMode.MainMenu);
			_buttonController.SetVisible(true, 3);
		}

		public void SetStamp(PhotoeditStampID stampIndex)
		{
			if (stampIndex == PhotoeditStampID.Stamp00)
			{
				_detailsObject.SetVisibleStamp(isVisible: false);
				return;
			}
			_detailsObject.SetVisibleStamp(isVisible: true);
			_detailsObject.ChangeStamp(stampIndex);
		}

		public void SetVisibleStamp(bool isVisible)
		{
		}

		public void SetMaskActive(int target, bool isSinglePlay)
		{
		}

		public void Scroll(Direction direction)
		{
			_editChainlist.Scroll(direction);
		}

		public void CardViewScroll(Direction direction)
		{
			_nextTrackAnimator.SetTrigger((direction == Direction.Right) ? "NextTrack" : "NextTrackLeft");
			_overlayAnimator.Play(Animator.StringToHash(((direction == Direction.Right) ? "Right" : "Left") + "Scroll"), 0, 0f);
		}

		public void SetValue(Direction direction, string switchName)
		{
			_editChainlist.SetValue(direction, switchName);
		}

		public void SetDeployChainList()
		{
			_editChainlist.Deploy();
		}

		public void SetTrackNumber(int leftTrack, int currentTrack, int rightTrack, int currentPhotoIndex, int maxIndex)
		{
			if (IsActive())
			{
				if (currentTrack.ToString().Length == 1)
				{
					_trackNumText.gameObject.SetActive(value: true);
					_trackDoubleText.gameObject.SetActive(value: false);
					_trackNumText.ChangeText(currentTrack.ToString());
				}
				else
				{
					_trackNumText.gameObject.SetActive(value: false);
					_trackDoubleText.gameObject.SetActive(value: true);
					_trackDoubleText.ChangeText($"{currentTrack:00}");
				}
				_buttonController.SetPhotoCount(leftTrack, rightTrack);
			}
		}

		public void SetPhotoIndex(int leftIndex, int rightIndex)
		{
		}

		public void SetUserData(GameManager.TargetID player, AssetManager manager, UserDetail data, UserOption option, bool isMyself)
		{
			_detailsObject.SetUserData(player, manager, data, option, isMyself);
		}

		public void SetUserMultiData(int index, AssetManager manager, UserDetail data, UserOption option)
		{
		}

		public void SetDetails(string musicName, int difficulty, bool isClear, Texture2D jacket, bool jacketImmediate)
		{
			_detailsObject.SetDetailsData(musicName, isClear);
			_nextJacketImage.texture = jacket;
			StartCoroutine(PhotoDataCoroutine(jacket, jacketImmediate));
		}

		private IEnumerator PhotoDataCoroutine(Texture2D jacket, bool jacketImmediate)
		{
			if (!jacketImmediate)
			{
				yield return new WaitForSeconds(_nextTrackAnimator.GetCurrentAnimatorStateInfo(0).length);
			}
			_nextJacketImage.transform.localPosition = new Vector3(0f, 128f, 0f);
			_jacketImage.transform.localPosition = Vector3.zero;
			_jacketImage.texture = jacket;
		}

		public void SetDifficulty(GameManager.TargetID player, MusicDifficultyID difficulty, bool isThis)
		{
			Color difficultyFrontColor = Utility.ConvertColor(difficulty.GetMainColor());
			Color difficultyBackColor = Utility.ConvertColor(difficulty.GetSubColor());
			_detailsObject.SetDifficulty(player, difficulty, difficultyFrontColor, difficultyBackColor, isThis);
		}

		public void SetLevel(GameManager.TargetID player, int level, MusicLevelID levelID, MusicDifficultyID difficulty, bool isThis)
		{
			_detailsObject.SetLevel(player, level, levelID, difficulty, isThis);
		}

		public void SetClearRank(GameManager.TargetID player, MusicClearrankID rank, bool isMyself)
		{
			_detailsObject.SetClearRank(player, rank, isMyself);
		}

		public void SetMultiData(string musicName, int chainCount)
		{
		}

		public void SetComboSyncData(uint maxCombo, uint maxSync)
		{
			_detailsObject.SetMaxComboData(maxCombo);
			_detailsObject.SetMaxSyncData(maxSync);
		}

		public void SetVisibleSync(bool isVisible)
		{
			_detailsObject.SetVisibleSync(isVisible);
		}

		public void SetPlayerRank(GameManager.TargetID player, uint rankOrder, bool isMyself)
		{
			_detailsObject.SetPlayerRank(player, rankOrder, isMyself);
		}

		public void SetVisiblePlayerRank(bool isVisible)
		{
			_detailsObject.SetVisiblePlayerRank(isVisible);
		}

		public void SetAchievement(GameManager.TargetID player, uint score, bool isMyself)
		{
			_detailsObject.SetAchievement(player, score, isMyself);
		}

		public void SetMyBestRecord(bool isNewRecord, int best, int diff)
		{
			_detailsObject.SetMyBestRecord(isNewRecord, best, diff);
		}

		public void SetFastRate(uint fast, uint late)
		{
			_detailsObject.SetFastLate(fast, late);
		}

		public void SetVisibleFastLate(bool isVisible)
		{
			_detailsObject.SetVisibleFastLate(isVisible);
		}

		public void ChangeLayout(PhotoeditLayoutID layout)
		{
			_detailsObject.ChangeLayout(layout);
			_detailsObject.SetSwitchPhotoCharacter(isPhoto: true);
			_detailsObject.RemoveMasks();
			if (layout == PhotoeditLayoutID.Character)
			{
				_detailsObject.SetSwitchPhotoCharacter(isPhoto: false);
			}
		}

		public void SetDxScore(uint dxScore, uint maximum, int dxStarCount)
		{
			_detailsObject.SetDxScore(dxScore, maximum, dxStarCount);
		}

		public void SetMedal(GameManager.TargetID player, PlayComboflagID comboType, PlaySyncflagID syncType, bool isMyself)
		{
			_detailsObject.SetMedal(player, comboType, syncType, isMyself);
		}

		public void SetScore(uint critical, uint perfect, uint great, uint good, uint miss)
		{
			_detailsObject.SetScore(critical, perfect, great, good, miss);
		}

		public void SetScoreData(NoteScore.EScoreType type, uint critical, uint perfect, uint great, uint good, uint miss)
		{
			_scoreBoardController.SetScore(type, NoteJudge.JudgeBox.Critical, critical);
			_scoreBoardController.SetScore(type, NoteJudge.JudgeBox.Perfect, perfect);
			_scoreBoardController.SetScore(type, NoteJudge.JudgeBox.Great, great);
			_scoreBoardController.SetScore(type, NoteJudge.JudgeBox.Good, good);
			_scoreBoardController.SetScore(type, NoteJudge.JudgeBox.Miss, miss);
		}

		public void SetScoreGauge(NoteScore.EScoreType type, float perfect, float critical, float great, float good, uint max)
		{
			_detailsObject.SetScoreGauge(type, perfect, critical, great, good, max);
		}

		public void SetVisibleCriticalPerfect(bool isVisible)
		{
			_detailsObject.SetVisibleCritical(isVisible);
			NoteScore.EScoreType[] array = (NoteScore.EScoreType[])Enum.GetValues(typeof(NoteScore.EScoreType));
			foreach (NoteScore.EScoreType type in array)
			{
				_scoreBoardController.SetVisibleCloseBoxAll(type, isVisible: false);
			}
			_scoreBoardController.SetVisibleCloseBox(NoteScore.EScoreType.Tap, NoteJudge.JudgeBox.Critical, !isVisible);
			_scoreBoardController.SetVisibleCloseBox(NoteScore.EScoreType.Hold, NoteJudge.JudgeBox.Critical, !isVisible);
			_scoreBoardController.SetVisibleCloseBox(NoteScore.EScoreType.Slide, NoteJudge.JudgeBox.Critical, !isVisible);
			_scoreBoardController.SetVisibleCloseBox(NoteScore.EScoreType.Touch, NoteJudge.JudgeBox.Critical, !isVisible);
		}

		public void SetPhotoData(Texture2D texture, int shiftPosition)
		{
			_detailsObject.SetPhotoData(texture, shiftPosition);
		}

		public void SetCharacterImage(GameManager.TargetID player, Sprite character, bool isMyself)
		{
			_detailsObject.SetCharacter(player, character, isMyself);
		}

		public void SetGameScoreType(GameManager.TargetID player, ConstParameter.ScoreKind kind, bool isMyself)
		{
			_detailsObject.SetGameScoreType(player, kind, isMyself);
		}

		public void SetPerfectChallenge(GameManager.TargetID player, bool isActive, int life, bool isClear, bool isMyself)
		{
			_detailsObject.SetPerfectChallenge(player, isActive, life, isClear, isMyself);
		}

		public override void ViewUpdate()
		{
			_editChainlist.ViewUpdate();
			_buttonController.ViewUpdate();
			_detailsObject?.ViewUpdate();
		}

		public void SetVisibleUserInfomation(bool isVisible)
		{
			_detailsObject.SetVisibleUserInfomation(isVisible);
		}

		public void SetVisibleShootingDate(bool isVisible)
		{
			_detailsObject.SetVisibleShootingDate(isVisible);
		}

		public void SetVisibleStoreName(bool isVisible)
		{
			_detailsObject.SetVisibleStoreName(isVisible);
		}

		public void SetButtonPressed(InputManager.ButtonSetting button)
		{
			_buttonController.Pressed(button);
		}

		public void PressedTouchButton(Direction direction, bool isLongTouch, bool toOut)
		{
			_editChainlist.Preseedbutton(direction, isLongTouch, toOut);
		}

		public void TouchEditButton()
		{
			_buttonController.TouchEditButton();
		}

		public void TouchZoomButton()
		{
			_buttonController.TouchZoomButton();
		}

		public void SetVisibleButton(bool isVisible, params InputManager.ButtonSetting[] buttons)
		{
			if (IsActive())
			{
				foreach (InputManager.ButtonSetting buttonSetting in buttons)
				{
					_buttonController.SetVisible(isVisible, buttonSetting);
				}
			}
		}

		public void SetVisibleCardButton(bool isVisible, Direction direction)
		{
			_editChainlist.SetVisibleButton(isVisible, direction);
		}

		private void ChangeButtonSymbol(InputManager.ButtonSetting button, PhotoEditProcess.SelectMode mode)
		{
			if (IsActive())
			{
				_buttonController.ChangeButtonSymbol(button, mode);
			}
		}

		public void SetDisable()
		{
			_overlayPreviewObject.SetActive(value: false);
			_zoomPreviewObject.SetActive(value: false);
			_buttonController.gameObject.SetActive(value: false);
			_nextTrackAnimator.gameObject.SetActive(value: false);
			_customBackAnimator.gameObject.SetActive(value: false);
		}

		public void SetMainMenu()
		{
			if (_mainMenuCoroutine != null)
			{
				StopCoroutine(_mainMenuCoroutine);
				_mainMenuCoroutine = null;
			}
			_mainMenuCoroutine = StartCoroutine(MainMenuCoroutine());
		}

		private IEnumerator MainMenuCoroutine()
		{
			_customBackAnimator.Play(Animator.StringToHash("MainMenuLoop"));
			ChangeButtonSymbol(InputManager.ButtonSetting.Button04, PhotoEditProcess.SelectMode.MainMenu);
			_buttonController.SetVisibleZoomButton(isVisible: true);
			_buttonController.SetVisibleEditButton(isVisible: true);
			yield return new WaitForSeconds(0.7f);
			_buttonController.SetVisible(true, 3);
			_mainMenuCoroutine = null;
		}

		public void SetCustomEditMode(bool isUpload)
		{
			StartCoroutine(CustomEditModeCoroutine(isUpload));
			_editChainlist.Deploy();
		}

		private IEnumerator CustomEditModeCoroutine(bool isUpload)
		{
			_blurObject.SetActive(value: false);
			_customBackAnimator.Play(Animator.StringToHash("CustomIn"));
			_buttonController.SetVisible(false, 1, 3, 6);
			_buttonController.SetVisibleZoomButton(isVisible: false);
			_buttonController.SetVisibleEditButton(isVisible: false);
			yield return new WaitForEndOfFrame();
			float length = _customBackAnimator.GetCurrentAnimatorStateInfo(0).length;
			yield return new WaitForSeconds(length);
			_buttonController.SetVisible(true, 4);
			if (isUpload)
			{
				ChangeButtonSymbol(InputManager.ButtonSetting.Button04, PhotoEditProcess.SelectMode.CustomEdit);
				_buttonController.SetVisible(true, 3);
			}
			ChangeButtonSymbol(InputManager.ButtonSetting.Button05, PhotoEditProcess.SelectMode.CustomEdit);
			_customBackAnimator.Play(Animator.StringToHash("CustomLoop"));
		}

		public void SetCustom2MainMenu()
		{
			StartCoroutine(Custom2MainMenuCoroutine());
		}

		private IEnumerator Custom2MainMenuCoroutine()
		{
			_editChainlist.RemoveAll();
			_buttonController.SetVisible(false, 2, 3, 4, 5);
			_customBackAnimator.Play(Animator.StringToHash("MainMenuIn"));
			yield return new WaitForEndOfFrame();
			float length = _customBackAnimator.GetCurrentAnimatorStateInfo(0).length;
			yield return new WaitForSeconds(length);
			SetMainMenu();
		}

		public void SetZoomMode()
		{
			StopCoroutine(Zoom2MainCoroutine());
			StartCoroutine(ZoomModeCoroutine());
		}

		private IEnumerator ZoomModeCoroutine()
		{
			_buttonController.SetVisibleZoomButton(isVisible: false);
			_buttonController.SetVisibleEditButton(isVisible: false);
			_buttonController.SetVisible(false, 1, 3, 4, 6);
			_overlayAnimator.Play(Animator.StringToHash("Out"));
			yield return new WaitForEndOfFrame();
			float length = _overlayAnimator.GetCurrentAnimatorStateInfo(0).length;
			yield return new WaitForSeconds(length);
			_previewMessageObject.SetActive(value: true);
			_blurObject.SetActive(value: true);
			_zoomPreviewObject.SetActive(value: true);
			_zoomPreviewAnimator.Play(Animator.StringToHash("ZoomIn"));
		}

		public void SetZoom2Main()
		{
			StopCoroutine(ZoomModeCoroutine());
			StartCoroutine(Zoom2MainCoroutine());
		}

		private IEnumerator Zoom2MainCoroutine()
		{
			_previewMessageObject.SetActive(value: false);
			_zoomPreviewAnimator.Play(Animator.StringToHash("ZoomOut"));
			yield return new WaitForEndOfFrame();
			float length = _overlayAnimator.GetCurrentAnimatorStateInfo(0).length;
			yield return new WaitForSeconds(length);
			_blurObject.SetActive(value: false);
			_overlayAnimator.Play(Animator.StringToHash("In"));
			SetMainMenu();
		}

		public void SetConfirmationMode()
		{
			StartCoroutine(ConfirmationModeCoroutine());
		}

		private IEnumerator ConfirmationModeCoroutine()
		{
			_blurObject.SetActive(value: true);
			_buttonController.SetVisible(false, 2, 3, 4, 5);
			yield return new WaitForSeconds(0.25f);
			ChangeButtonSymbol(InputManager.ButtonSetting.Button04, PhotoEditProcess.SelectMode.Confirmation);
			ChangeButtonSymbol(InputManager.ButtonSetting.Button05, PhotoEditProcess.SelectMode.Confirmation);
			_buttonController.SetVisible(true, 3, 4);
		}

		public void SetConfirmation2Main()
		{
			StartCoroutine(Confirmation2MainCoroutine());
		}

		private IEnumerator Confirmation2MainCoroutine()
		{
			_editChainlist.RemoveAll();
			_blurObject.SetActive(value: false);
			_buttonController.SetVisible(false, 3, 4);
			ChangeButtonSymbol(InputManager.ButtonSetting.Button04, PhotoEditProcess.SelectMode.MainMenu);
			yield return new WaitForSeconds(0.25f);
			SetMainMenu();
		}

		public void SetConfirmation2Edit()
		{
			StartCoroutine(SetConfirmation2EditCoroutine());
		}

		private IEnumerator SetConfirmation2EditCoroutine()
		{
			_blurObject.SetActive(value: false);
			_buttonController.SetVisible(false, 3, 4);
			ChangeButtonSymbol(InputManager.ButtonSetting.Button04, PhotoEditProcess.SelectMode.CustomEdit);
			ChangeButtonSymbol(InputManager.ButtonSetting.Button05, PhotoEditProcess.SelectMode.CustomEdit);
			yield return new WaitForSeconds(0.25f);
			_buttonController.SetVisible(true, 3, 4);
		}

		public void SetCompleteMode()
		{
			_blurObject.SetActive(value: true);
			ChangeButtonSymbol(InputManager.ButtonSetting.Button04, PhotoEditProcess.SelectMode.Complete);
			_buttonController.SetVisible(false, 1, 4, 6);
		}

		public void SetUnregisterd()
		{
			StartCoroutine(SetUnregisterdCoroutine());
		}

		private IEnumerator SetUnregisterdCoroutine()
		{
			_blurObject.SetActive(value: true);
			_buttonController.SetVisible(false, 1, 3, 4, 6);
			yield return new WaitForSeconds(0.5f);
			_buttonController.SetVisible(true, 3);
			ChangeButtonSymbol(InputManager.ButtonSetting.Button04, PhotoEditProcess.SelectMode.Unregistered);
		}

		public void SetFirstInformation()
		{
			_buttonController.ChangeButtonSymbol(InputManager.ButtonSetting.Button04, PhotoEditProcess.SelectMode.Complete);
			_buttonController.SetVisibleImmediate(false, 1, 2, 3, 4, 5, 6);
			_blurObject.SetActive(value: true);
		}

		public void SetFirstInformationSkip(bool isActive)
		{
			_buttonController.SetVisible(isActive, 3);
		}

		public void SetTimeUpWait()
		{
			_buttonController.SetVisible(false, 3, 4);
			_blurObject.SetActive(value: false);
			_fromtBlurObject.SetActive(value: true);
		}

		public Rect GetPhotoRect()
		{
			return new Rect(OverlayCamera.transform.localPosition, new Vector2(1056f, 594f));
		}

		public RenderTexture GetRenderTexture()
		{
			return _overlayRenderTexture;
		}

		public void SetActiveBlur(bool isActive)
		{
			_blurObject.SetActive(isActive);
		}
	}
}
