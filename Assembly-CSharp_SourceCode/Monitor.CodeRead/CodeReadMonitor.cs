using System;
using System.Collections;
using Manager;
using Monitor.CodeRead.ChainList;
using Monitor.CodeRead.Controller;
using Process.CodeRead;
using UI.Common;
using UI.DaisyChainList;
using UnityEngine;

namespace Monitor.CodeRead
{
	public class CodeReadMonitor : MonitorBase
	{
		[SerializeField]
		[Header("ボタンコントロール")]
		private CodeReadButtonController _buttonController;

		[SerializeField]
		[Header("ブラー")]
		private GameObject _blurObject;

		[SerializeField]
		[Header("メッセージウィンドウ")]
		private ReadMessageWindow _readMessageWindow;

		[SerializeField]
		[Header("セレクター背景")]
		private SelectorBackgroundController _selectorBackground;

		[SerializeField]
		[Header("履歴カードリスト")]
		private ReadCardChainList _readCardChainList;

		[SerializeField]
		[Header("獲得演出 楽曲")]
		private MusicWindow _musicWindow;

		[SerializeField]
		[Header("獲得演出 キャラクター")]
		private CharaWindow _charaWindow;

		[SerializeField]
		[Header("生成先オブジェクト")]
		private RectTransform _readImageLarge;

		[SerializeField]
		private RectTransform _singleDeco;

		[SerializeField]
		private RectTransform _centerBaseCircle;

		[SerializeField]
		private RectTransform _cardInfo;

		[SerializeField]
		private RectTransform _centerBase;

		[SerializeField]
		private RectTransform _readImage;

		[SerializeField]
		private RectTransform _partnerStatus;

		[SerializeField]
		private RectTransform _title;

		[SerializeField]
		private RectTransform _centerBaseEff;

		[SerializeField]
		private RectTransform _decoLanDolly;

		[SerializeField]
		private RectTransform _fusion;

		[SerializeField]
		[Header("オリジナル")]
		private GameObject _originalReadImageLarge;

		[SerializeField]
		private GameObject _originalSingleDeco;

		[SerializeField]
		private GameObject _originalCenterBaseCircle;

		[SerializeField]
		private GameObject _originalCardInfo;

		[SerializeField]
		private GameObject _originalCenterBase;

		[SerializeField]
		private GameObject _originalReadImage;

		[SerializeField]
		private GameObject _originalPartnerStatus;

		[SerializeField]
		private GameObject _originalTitle;

		[SerializeField]
		private GameObject _originalCenterBaseEff;

		[SerializeField]
		private GameObject _originalDecoLanDolly;

		[SerializeField]
		private GameObject _originalFusion;

		private ReadImageLargeController _readImageLargeController;

		private CenterBaseController _centerBaseController;

		private PartnerStatusController _partnerStatusController;

		private ReadImageController _readImageController;

		private DecoLanDollyController _decoLanDollyController;

		private CenterBaseCircleController _centerBaseCircleController;

		private CodeInfoController _codeInfoController;

		private CenterBaseEffectController _centerBaseEffectController;

		private FusionController _fusionController;

		private Animator _titleAnimator;

		private Animator _singleDecoAnimator;

		private bool _isSinglePlay;

		public override void Initialize(int playerIndex, bool active)
		{
			base.Initialize(playerIndex, active);
			_readMessageWindow.gameObject.SetActive(value: false);
			_selectorBackground.Play(SelectorBackgroundController.AnimationType.Disabled);
			if (active)
			{
				_readImageLargeController = UnityEngine.Object.Instantiate(_originalReadImageLarge, _readImageLarge).GetComponent<ReadImageLargeController>();
				_centerBaseController = UnityEngine.Object.Instantiate(_originalCenterBase, _centerBase).GetComponent<CenterBaseController>();
				_partnerStatusController = UnityEngine.Object.Instantiate(_originalPartnerStatus, _partnerStatus).GetComponent<PartnerStatusController>();
				_readImageController = UnityEngine.Object.Instantiate(_originalReadImage, _readImage).GetComponent<ReadImageController>();
				_decoLanDollyController = UnityEngine.Object.Instantiate(_originalDecoLanDolly, _decoLanDolly).GetComponent<DecoLanDollyController>();
				_centerBaseCircleController = UnityEngine.Object.Instantiate(_originalCenterBaseCircle, _centerBaseCircle).GetComponent<CenterBaseCircleController>();
				_codeInfoController = UnityEngine.Object.Instantiate(_originalCardInfo, _cardInfo).GetComponent<CodeInfoController>();
				_centerBaseEffectController = UnityEngine.Object.Instantiate(_originalCenterBaseEff, _centerBaseEff).GetComponent<CenterBaseEffectController>();
				_fusionController = UnityEngine.Object.Instantiate(_originalFusion, _fusion).GetComponent<FusionController>();
				_titleAnimator = UnityEngine.Object.Instantiate(_originalTitle, _title).GetComponent<Animator>();
				_singleDecoAnimator = UnityEngine.Object.Instantiate(_originalSingleDeco, _singleDeco).GetComponent<Animator>();
				_readImageLargeController.Initialize(playerIndex);
				_centerBaseController.Initialize(playerIndex);
				Direction direction = ((playerIndex != 0) ? Direction.Left : Direction.Right);
				_partnerStatusController.Initialize(playerIndex, direction);
				_readImageController.Initialize(playerIndex, direction);
				_decoLanDollyController.Initialize(playerIndex);
				_centerBaseCircleController.Initialize(playerIndex);
				_centerBaseCircleController.SetData(FusionType.None);
				_centerBaseEffectController.Initialize(playerIndex);
				_fusionController.Initialize(playerIndex);
				_codeInfoController.Initialize(playerIndex);
				_readCardChainList.Initialize();
				_readMessageWindow.Initialize();
				_buttonController.Initialize(playerIndex);
				_buttonController.SetVisible(false, 2, 3, 4, 5);
				_title.gameObject.SetActive(value: false);
				_readImageLarge.gameObject.SetActive(value: false);
				_singleDeco.gameObject.SetActive(value: false);
				_centerBaseCircle.gameObject.SetActive(value: false);
				_cardInfo.gameObject.SetActive(value: false);
				_centerBase.gameObject.SetActive(value: false);
				_readImage.gameObject.SetActive(value: false);
				_partnerStatus.gameObject.SetActive(value: false);
				_centerBaseEff.gameObject.SetActive(value: false);
				_decoLanDolly.gameObject.SetActive(value: false);
				_fusion.gameObject.SetActive(value: false);
				Main.alpha = 0f;
			}
		}

		public override void ViewUpdate()
		{
			_buttonController.ViewUpdate(GameManager.GetGameMSecAdd());
			_readCardChainList.ViewUpdate();
		}

		public void SetData(ICodeReadProcess process, bool isGuest, bool isSinglePlay)
		{
			_isSinglePlay = isSinglePlay;
			_centerBaseController.SetData(monitorIndex, _isSinglePlay);
			_readCardChainList.SetData(process, base.MonitorIndex);
		}

		public void SetCardReadPlayer(CardReadPlayer player)
		{
			_readImageLargeController.SetData(player);
		}

		public void SetFirstInformation()
		{
			if (Main.alpha < 1f)
			{
				Main.alpha = 1f;
			}
			_buttonController.ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 2);
			_buttonController.SetVisible(true, 3);
		}

		public void FirstCardReadPlayIn(bool isInit)
		{
			if (Main.alpha < 1f)
			{
				Main.alpha = 1f;
			}
			_title.gameObject.SetActive(value: true);
			_readImageLarge.gameObject.SetActive(value: true);
			StartCoroutine(FirstCardReadCoroutine(isInit));
		}

		private IEnumerator FirstCardReadCoroutine(bool isInit)
		{
			_titleAnimator.Play(Animator.StringToHash("In"));
			yield return _readImageLargeController.PlayIntroduction(isInit);
			_readImageLargeController.Play(AnimationType.Loop);
			_buttonController.SetVisible(true, 3);
			_buttonController.ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 0);
		}

		public void FirstCardReadPlayOut()
		{
			_titleAnimator.Play(Animator.StringToHash("Out"));
			_readImageLargeController.Play(AnimationType.Out);
		}

		public void ReadMessageWindowPlayIn(bool isSuccess, string effectText, string subMessage, string boostText, CodeReadProcess.CardStatus status, CodeReadProcess.ReadCard card)
		{
			_readMessageWindow.SetData(effectText, subMessage, boostText, status);
			_readMessageWindow.SetCardData(card);
			_readMessageWindow.PlayIntroduction();
			StartCoroutine(isSuccess ? ReadMessageWindowPlayInCoroutine() : ReadFailedMessageCoroutine());
		}

		private IEnumerator ReadMessageWindowPlayInCoroutine()
		{
			yield return new WaitForSeconds(0.5f);
			_buttonController.ChangeButtonSymbol(InputManager.ButtonSetting.Button05, 1);
			_buttonController.ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 1);
			_buttonController.SetVisible(true, 3, 4);
		}

		public void ReadFailedMessagePlay()
		{
			StartCoroutine(ReadFailedMessageCoroutine());
		}

		private IEnumerator ReadFailedMessageCoroutine()
		{
			_buttonController.SetVisible(false, 3);
			yield return new WaitForSeconds(0.5f);
			_buttonController.SetVisible(true, 4);
			_buttonController.ChangeButtonSymbol(InputManager.ButtonSetting.Button05, 1);
		}

		public void ReadMessageWindowPlayOut()
		{
			_readMessageWindow.PlayOutroduction();
			_buttonController.SetVisible(false, 3, 4);
			_readImageController.gameObject.SetActive(value: false);
		}

		public void CardSelectPlayIn(bool isDecision)
		{
			if (Main.alpha < 1f)
			{
				Main.alpha = 1f;
			}
			if (_isSinglePlay)
			{
				_singleDeco.gameObject.SetActive(value: true);
				_singleDecoAnimator.Play(Animator.StringToHash("In"));
			}
			else
			{
				_partnerStatus.gameObject.SetActive(value: true);
				_partnerStatusController.Play(AnimationType.In);
				_readImage.gameObject.SetActive(value: true);
				_readImageController.Play(AnimationType.In);
				_decoLanDolly.gameObject.SetActive(value: true);
				_decoLanDollyController.Play(AnimationType.In);
				_centerBaseEffectController.Play(AnimationType.In);
			}
			_centerBaseCircle.gameObject.SetActive(value: true);
			_centerBaseCircleController.Play(AnimationType.In);
			_cardInfo.gameObject.SetActive(value: true);
			_codeInfoController.Play(AnimationType.In);
			_centerBase.gameObject.SetActive(value: true);
			_centerBaseController.Play(AnimationType.In);
			_title.gameObject.SetActive(value: true);
			_titleAnimator.Play(Animator.StringToHash("In"));
			_selectorBackground.gameObject.SetActive(value: true);
			_selectorBackground.Play(SelectorBackgroundController.AnimationType.In);
			_readCardChainList.Deploy();
			_readCardChainList.Play();
			StartCoroutine(CardSelectPlayInCoroutine(isDecision));
		}

		private IEnumerator CardSelectPlayInCoroutine(bool isDecision)
		{
			_buttonController.SetVisible(false, 3, 4);
			yield return new WaitForSeconds(0.5f);
			_buttonController.SetVisible(true, 3);
			_buttonController.ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 0);
		}

		public void CardSelectPlayOut()
		{
			_buttonController.SetVisible(false, 2, 3, 4, 5);
			_selectorBackground.Play(SelectorBackgroundController.AnimationType.Out);
			_readCardChainList.RemoveOut();
			if (_titleAnimator.gameObject.activeSelf && _titleAnimator.gameObject.activeInHierarchy)
			{
				_titleAnimator.Play(Animator.StringToHash("Out"));
			}
			_centerBaseCircleController.Play(AnimationType.Out);
			_centerBaseController.Play(AnimationType.Out);
			_codeInfoController.Play(AnimationType.Out);
			if (_isSinglePlay)
			{
				if (_singleDecoAnimator.gameObject.activeSelf && _singleDecoAnimator.gameObject.activeInHierarchy)
				{
					_singleDecoAnimator.Play(Animator.StringToHash("Out"));
				}
			}
			else
			{
				_partnerStatusController.Play(AnimationType.Out);
				_readImageController.Play(AnimationType.Out);
				_decoLanDollyController.Play(AnimationType.Out);
				_centerBaseEffectController.Play(AnimationType.Out);
			}
			StartCoroutine(CardSelectOutCoroutine());
		}

		private IEnumerator CardSelectOutCoroutine()
		{
			yield return new WaitForSeconds(0.5f);
			_selectorBackground.Play(SelectorBackgroundController.AnimationType.Disabled);
		}

		public void PressedButton(InputManager.ButtonSetting button)
		{
			_buttonController.SetAnimationActive((int)button);
		}

		public void SetVisibleButton(bool isVisible, InputManager.ButtonSetting button)
		{
			_buttonController.SetVisible(isVisible, button);
		}

		public void ChangeButtonSymbol(InputManager.ButtonSetting button, int index)
		{
			_buttonController.ChangeButtonSymbol(button, index);
		}

		public void SetCardInfoData(string title, string text, string boost)
		{
			_codeInfoController.SetData(base.MonitorIndex, title, text, boost);
			_codeInfoController.PlayChoiceIn();
		}

		public void Scroll(Direction direction)
		{
			_readCardChainList.Scroll(direction);
		}

		public void PlayGetMusic(Sprite jacket, string musicName, Action<int> callback)
		{
			if (!_musicWindow.gameObject.activeSelf)
			{
				_musicWindow.gameObject.SetActive(value: true);
			}
			_musicWindow.Set(jacket, musicName);
			_musicWindow.Play(delegate
			{
				callback(base.MonitorIndex);
			});
		}

		public void PlayGetCharacter(Sprite character, string characterName, int id, Action<int> callback)
		{
			if (!_charaWindow.gameObject.activeSelf)
			{
				_charaWindow.gameObject.SetActive(value: true);
			}
			_charaWindow.Set(character, characterName, id);
			_charaWindow.Play(delegate
			{
				callback(base.MonitorIndex);
			});
		}

		public void PlayCenterBaseFusion(FusionType type)
		{
			if (!_centerBaseEff.gameObject.activeSelf)
			{
				_centerBaseEff.gameObject.SetActive(value: true);
			}
			_centerBaseEffectController.PlayEffect(type);
			_centerBaseCircleController.PlayCircle(type);
		}

		public void PlayFusion(CodeReadProcess.ReadCard card, FusionType type)
		{
			_buttonController.SetVisibleImmediate(false, 0, 1, 2, 3, 4, 5, 6);
			_fusion.gameObject.SetActive(value: true);
			_fusionController.SetData(card);
			_fusionController.PlayFusion(type);
		}

		public void SetDecision(bool isDecision)
		{
			_readCardChainList.SetDecision(isDecision);
		}

		public void SetLanDollyDecision(int targetIndex)
		{
			_decoLanDollyController.SetDecision(targetIndex);
		}

		public void SetPartnerDecision(FusionType type)
		{
			_partnerStatusController.PlayTypeIn(type);
		}

		public void SetPartnerStatus(CodeReadProcess.ReadCard card, bool isDownEffect, CodeReadProcess.CardStatus status)
		{
			_partnerStatusController.SetData(card, isDownEffect, status);
		}

		public void ReadImageSetInsert(CodeReadProcess.CodeReaderState state)
		{
			_centerBaseController.PlayCenterBase(state);
			_readImageController.PlayReadImage(state);
			_readImageLargeController.PlayAbnormality();
		}

		public void SetTimeUp(bool isFirstRead)
		{
			if (IsActive())
			{
				if (isFirstRead)
				{
					FirstCardReadPlayOut();
				}
				else
				{
					CardSelectPlayOut();
				}
				ReadMessageWindowPlayOut();
			}
		}

		public void SetVisibleBlur(bool isVisible)
		{
			Main.alpha = 1f;
			_blurObject.gameObject.SetActive(isVisible);
		}
	}
}
