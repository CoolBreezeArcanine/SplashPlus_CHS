using System;
using System.Collections;
using DB;
using IO;
using MAI2System;
using Manager;
using Monitor.Result;
using Timeline;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace Monitor
{
	public class ResultMonitor : MonitorBase
	{
		[Serializable]
		public class SpriteSheet
		{
			[SerializeField]
			private Sprite[] _sheet;

			public Sprite[] Sheet => _sheet;
		}

		[SerializeField]
		[Header("バックグラウンドキャンバス")]
		private GameObject _backgroundCanvasObject;

		[SerializeField]
		private GameObject _blurObject;

		[SerializeField]
		[Header("TimelineDirector")]
		private ResultTimelineManager _timelineManager;

		[SerializeField]
		private GameObject _rankParentObject;

		[SerializeField]
		[Header("ボタンコントローラー")]
		private ResultButtonController _resultButtonController;

		[SerializeField]
		[Header("基本設定")]
		[Tooltip("楽曲名")]
		private CustomTextScroll _musicNameText;

		[SerializeField]
		[Tooltip("ジャケット画像")]
		private RawImage _musicJacketImage;

		[SerializeField]
		[Header("難易度レベル")]
		private SpriteCounter _difficultySingle;

		[SerializeField]
		private SpriteCounter _difficultyDouble;

		[SerializeField]
		private Image _levelTextImage;

		[SerializeField]
		[Header("クリア表示系")]
		private GameObject _clearParentGameObject;

		[SerializeField]
		[Header("譜面型表示")]
		private MultipleImage _gameTypeImage;

		[SerializeField]
		[Header("トラックナンバー")]
		private SpriteCounter _trackNumberCounter;

		[SerializeField]
		private SpriteCounter _outlineTrackNumberCounter;

		[SerializeField]
		[Header("パーフェクトチャレンジ")]
		private GameObject _perfectChallengeDisplayObject;

		[SerializeField]
		private SpriteCounter _perfectChallengeCounter;

		[SerializeField]
		[Tooltip("達成率演出")]
		[Header("Achievement")]
		private AchievementCounterObject _counterObject;

		[SerializeField]
		[Tooltip("自己ベスト達成率")]
		private SpriteCounter _myBestAchivement;

		[SerializeField]
		[Tooltip("達成率\u3000差分")]
		private SpriteCounter _achivementDiffText;

		[SerializeField]
		[Tooltip("新記録")]
		private GameObject _newRecord;

		[SerializeField]
		[Header("でらっくすレート")]
		[Tooltip("DX矢印")]
		private MultipleImage _dxRatingArrow;

		[SerializeField]
		private MultipleImage _dxRatingArrowBackImage;

		[SerializeField]
		[Tooltip("DXレート表示")]
		private SpriteCounter _dxRatingText;

		[SerializeField]
		private Image _dxRatingBackground;

		[SerializeField]
		private TextMeshProUGUI _dxDiffText;

		[SerializeField]
		[Header("ランク生成先")]
		private RectTransform _rankObjectParent;

		[SerializeField]
		[Header("Fast/Late")]
		private GameObject _fastLateGameObject;

		[SerializeField]
		private TextMeshProUGUI _fastText;

		[SerializeField]
		private TextMeshProUGUI _lateText;

		[SerializeField]
		[Header("簡易側管理アニメ")]
		private Animator _simpleAnimator;

		[SerializeField]
		[Header("詳細側管理アニメ")]
		private Animator _scoreboardAnimator;

		[SerializeField]
		[Header("スプライトマスクオブジェクト")]
		private SpriteMask _spriteParticleMask;

		[SerializeField]
		private SpriteMask _spriteParticleMask02;

		[SerializeField]
		[Header("メダル")]
		private MedalDisplayObject _medalDisplay;

		[SerializeField]
		[Header("詳細スコア表示")]
		private ScoreBoardDisplayObject _scoreBoardDisplay;

		[SerializeField]
		[Header("上画面スコアボード")]
		private ScoreBoardController _scoreBoardController;

		[SerializeField]
		[Header("ナビキャラ生成目標")]
		private Transform _naviCharaRoot;

		[SerializeField]
		[Header("マルチプレイ表示物")]
		private GameObject _multiDisplayObject;

		[SerializeField]
		[Header("複数人プレイ\u3000各リザルト")]
		private InstantiateGenerator _multiResultGenerator;

		[SerializeField]
		[Header("でらっくすレート\u3000演出")]
		private DXRatingDirectingObject _ratingDirecting;

		[SerializeField]
		[Header("大会リザルト")]
		private KOP_ResultWindow _kopResultWindow;

		private bool _isStartShowDetails;

		private bool _isClear;

		private bool _isSinglePlay;

		private bool _isTimelineBinding;

		private bool _isNewRecord;

		private int _difficulty;

		private TrackAsset _backgroundTrack;

		private UnityEngine.Object _backgroundKeyObject;

		private UnityEngine.Object _backgroundObject;

		private Animator _backgroundAnimator;

		private UnityEngine.Object _newRecordObject;

		private int _dxRatingDirection;

		private int _musicRatingDirection;

		private int _udemaeRatingDirection;

		private int _multiIndex;

		private MultiUserDataObject[] _multiResultUserDatas;

		private UnityEngine.Object[] _multiResultDataObjects;

		private TrackAsset[] _multiResultTracks;

		private bool _isUdemaeRankUp;

		private NavigationCharacter _navigationCharacter;

		private TrackAsset _partnerTrack;

		private UnityEngine.Object _partnerBindObject;

		private TrackAsset _dxStarTrack;

		private UnityEngine.Object _dxStarBindObject;

		private DeluxcorerankrateID _dxRank;

		private double _naviFunStartEndTime;

		private uint _changeRate;

		private Sprite _changeRateSprite;

		private bool _isDXRatingUP;

		private bool _isReleaseTimelien;

		public bool IsDXRatingUP => _isDXRatingUP;

		protected ResultMonitor()
		{
		}

		public override void Initialize(int playerIndex, bool isActive)
		{
			base.Initialize(playerIndex, isActive);
			Sub.alpha = 0f;
			_blurObject.SetActive(value: false);
			if (IsActive())
			{
				MechaManager.LedIf[playerIndex].ButtonLedReset();
			}
			if (!isActive)
			{
				_backgroundCanvasObject.SetActive(value: false);
				Main.alpha = 0f;
				Main.gameObject.SetActive(value: false);
				_musicNameText.gameObject.SetActive(value: false);
				_musicJacketImage.transform.parent.gameObject.SetActive(value: false);
			}
			_resultButtonController.Initialize(playerIndex);
			string path = "Process/Result/Timelines/Result0" + (playerIndex + 1);
			_timelineManager.Initialize(playerIndex, Resources.Load<PlayableAsset>(path));
			_kopResultWindow.gameObject.SetActive(value: false);
		}

		public void SetStartData(Animator clearRankOriginal, MusicClearrankID clearRank, int difficulty, bool isDetails, bool isSingle, bool isClear, int naviCharaID)
		{
			_isSinglePlay = isSingle;
			_isClear = isClear;
			_isStartShowDetails = isDetails;
			NavigationCharacter component = AssetManager.Instance().GetNaviCharaPrefab(naviCharaID).GetComponent<NavigationCharacter>();
			_navigationCharacter = UnityEngine.Object.Instantiate(component, _naviCharaRoot);
			if (_isSinglePlay)
			{
				_multiDisplayObject.SetActive(value: false);
			}
			_timelineManager.SetData(_isNewRecord, isClear, difficulty, _navigationCharacter, clearRank);
			_timelineManager.SetAnimator(UnityEngine.Object.Instantiate(clearRankOriginal, _rankObjectParent));
		}

		public void TimelineInterpretation()
		{
			if (!isPlayerActive)
			{
				return;
			}
			Animator[] array = null;
			if (_multiResultUserDatas != null)
			{
				array = new Animator[_multiResultUserDatas.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = _multiResultUserDatas[i]?.MainAnimator;
				}
			}
			_timelineManager.SettingComplete(_medalDisplay.ComboMedal, _medalDisplay.SyncMedal, _dxRank, _isSinglePlay, _dxRatingDirection, _multiIndex, _isStartShowDetails, array);
			_timelineManager.Interpretation();
		}

		public override void ViewUpdate()
		{
			_timelineManager.ViewUpdate();
			_ratingDirecting?.ViewUpdate(GameManager.GetGameMSecAdd());
			_musicNameText.ViewUpdate();
			_kopResultWindow.ViewUpdate(GameManager.GetGameMSecAdd());
		}

		private void ReleaseTimelineBind()
		{
			_timelineManager.ReleaseTimeline();
		}

		public bool IsSkip()
		{
			return _timelineManager.IsSkippable();
		}

		public void ChangeScoreBoard(bool isDetails)
		{
			StartCoroutine(ChangeScoreBoradCoroutine(isDetails));
		}

		private IEnumerator ChangeScoreBoradCoroutine(bool isDetails)
		{
			_ = isDetails;
			yield return new WaitForSeconds(0.1f);
			if (_multiResultUserDatas != null)
			{
				for (int i = 0; i < 4; i++)
				{
					_multiResultUserDatas[i]?.SetActiveAnimation(i == _multiIndex, isDetails, _isSinglePlay);
				}
			}
			if (isDetails)
			{
				_simpleAnimator.Play(Animator.StringToHash("FadeOut"), 0, 0f);
				_scoreboardAnimator.Play(Animator.StringToHash("FadeIn"), 0, 0f);
				if (!_isUdemaeRankUp)
				{
				}
			}
			else
			{
				_simpleAnimator.Play(Animator.StringToHash("FadeIn"), 0, 0f);
				_scoreboardAnimator.Play(Animator.StringToHash("FadeOut"), 0, 0f);
			}
			_scoreBoardDisplay.Switch(isDetails);
			if (isDetails)
			{
				yield return new WaitForSeconds(0.1f);
			}
		}

		public void SetActiveMultiPlayObject(bool isActive)
		{
		}

		public void InitMultiUserData(int playerNum)
		{
			_multiResultUserDatas = new MultiUserDataObject[4];
			_multiResultDataObjects = new UnityEngine.Object[4];
			_multiResultTracks = new TrackAsset[4];
			for (int i = 0; i < playerNum; i++)
			{
				_multiResultUserDatas[i] = _multiResultGenerator.Instantiate<MultiUserDataObject>();
				_multiResultUserDatas[i].name = $"Rank{i + 1}";
			}
		}

		public void SetMultiUserData(int index, string userName, Sprite userIcon, MusicDifficultyID difficulty, uint achievement, int rank, PlayComboflagID comboType, bool myself)
		{
			if (myself)
			{
				_multiIndex = index;
			}
			_multiResultUserDatas[index].SetData(userName, userIcon, difficulty, achievement, rank, comboType);
		}

		public void PlayStaging()
		{
			if (isPlayerActive)
			{
				Sub.alpha = 1f;
				_timelineManager.Play();
			}
		}

		public bool IsStagingEnd()
		{
			bool num = _timelineManager.IsDone();
			if (num && !_isReleaseTimelien)
			{
				StartCoroutine(SkipCoroutine());
			}
			return num;
		}

		public void Skip()
		{
			if (_isDXRatingUP)
			{
				_timelineManager.SkipRatingUP();
				return;
			}
			_timelineManager.Skip();
			_counterObject.OnClipTailEnd();
			_scoreBoardDisplay.Skip();
			if (!_isReleaseTimelien)
			{
				StartCoroutine(SkipCoroutine());
			}
			if (_multiResultTracks != null)
			{
				for (int i = 0; i < 4; i++)
				{
					if (_multiResultUserDatas[i] != null && _multiResultUserDatas[i].gameObject.activeSelf && _multiResultUserDatas[i].gameObject.activeInHierarchy)
					{
						_multiResultDataObjects[i] = null;
						_multiResultUserDatas[i].MainAnimator.Play(Animator.StringToHash((i == _multiIndex) ? "Loop_User" : "Loop"));
						_multiResultUserDatas[i].Skip();
					}
				}
			}
			if (_isClear)
			{
				_navigationCharacter.SetBool("IsClear", _isClear);
				_navigationCharacter.Play(NavigationAnime.FunStartLoop, 0f);
			}
			else
			{
				_navigationCharacter.SetBool("IsClear", _isClear);
				_navigationCharacter.Play(NavigationAnime.Sad01, 0f);
			}
		}

		private IEnumerator SkipCoroutine()
		{
			yield return new WaitForEndOfFrame();
			ReleaseTimelineBind();
			_isReleaseTimelien = true;
		}

		public void SetMedalData(PlayComboflagID comboType, PlaySyncflagID syncType, MedalDisplayObject.MedalTarget medalTarget, uint toValue)
		{
			_medalDisplay.SetData(medalTarget, toValue, comboType, syncType);
		}

		public void SetFastLate(uint fast, uint late)
		{
			_fastText.text = fast.ToString("0");
			_lateText.text = late.ToString("0");
		}

		public void SetVisibleFastLate(bool isVisible)
		{
			_fastLateGameObject.SetActive(isVisible);
		}

		public void SetMusicData(string musicName, uint trakcNumber, Texture2D jacket)
		{
			_musicNameText.SetData(musicName);
			_musicJacketImage.texture = jacket;
			string text = trakcNumber.ToString("00");
			_trackNumberCounter.ChangeText(text);
			_outlineTrackNumberCounter.ChangeText(text);
		}

		public void SetLevel(int level, MusicLevelID levelID, int difficulty)
		{
			Sprite[] musicLevelSprites = CommonPrefab.GetMusicLevelSprites(difficulty);
			_difficultySingle.SetSpriteSheet(musicLevelSprites);
			_difficultyDouble.SetSpriteSheet(musicLevelSprites);
			_levelTextImage.sprite = musicLevelSprites[14];
			if (level > 9)
			{
				_difficultySingle.gameObject.SetActive(value: false);
				_difficultyDouble.gameObject.SetActive(value: true);
				_difficultyDouble.ChangeText(levelID.GetLevelNum().PadRight(3));
			}
			else
			{
				_difficultySingle.gameObject.SetActive(value: true);
				_difficultyDouble.gameObject.SetActive(value: false);
				_difficultySingle.ChangeText(levelID.GetLevelNum().PadRight(2));
			}
		}

		public void SetSync(uint sync, uint max, float leftRate, float rightRate)
		{
			_scoreBoardDisplay.SetSync(sync, max);
		}

		public void PreseedButton(InputManager.ButtonSetting button)
		{
			_resultButtonController.SetAnimationActive((int)button);
		}

		public void ChangeButtonSymbol(InputManager.ButtonSetting button, int index)
		{
			_resultButtonController.ChangeButtonSymbol(button, index);
		}

		public void SetVisibleButton(InputManager.ButtonSetting button, bool isVisisble)
		{
			_resultButtonController.SetVisible(isVisisble, (int)button);
		}

		public void SetScoreGauge(NoteScore.EScoreType type, float perfect, float critical, float great, float good, uint max)
		{
		}

		public void SetActiveCriticalScore(bool isActive)
		{
			_scoreBoardDisplay.SetActiveCritical(isActive);
			NoteScore.EScoreType[] array = (NoteScore.EScoreType[])Enum.GetValues(typeof(NoteScore.EScoreType));
			foreach (NoteScore.EScoreType type in array)
			{
				_scoreBoardController.SetVisibleCloseBoxAll(type, isVisible: false);
			}
			_scoreBoardController.SetVisibleCloseBox(NoteScore.EScoreType.Tap, NoteJudge.JudgeBox.Critical, !isActive);
			_scoreBoardController.SetVisibleCloseBox(NoteScore.EScoreType.Hold, NoteJudge.JudgeBox.Critical, !isActive);
			_scoreBoardController.SetVisibleCloseBox(NoteScore.EScoreType.Slide, NoteJudge.JudgeBox.Critical, !isActive);
			_scoreBoardController.SetVisibleCloseBox(NoteScore.EScoreType.Touch, NoteJudge.JudgeBox.Critical, !isActive);
		}

		public void SetTotalScoreDetils(uint critical, uint perfect, uint great, uint good, uint miss)
		{
			_scoreBoardDisplay.SetScoreData(critical, perfect, great, good, miss);
		}

		public void SetScoreData(NoteScore.EScoreType type, uint critical, uint perfect, uint great, uint good, uint miss)
		{
			_scoreBoardController.SetScore(type, NoteJudge.JudgeBox.Critical, critical);
			_scoreBoardController.SetScore(type, NoteJudge.JudgeBox.Perfect, perfect);
			_scoreBoardController.SetScore(type, NoteJudge.JudgeBox.Great, great);
			_scoreBoardController.SetScore(type, NoteJudge.JudgeBox.Good, good);
			_scoreBoardController.SetScore(type, NoteJudge.JudgeBox.Miss, miss);
		}

		public void SetMyBestAchievement(uint bestScore, uint diffScore, bool isNewRecord)
		{
			_myBestAchivement.ChangeText(((float)bestScore / 10000f).ToString("##0.0000").PadLeft(8) + "%");
			string text = ((isNewRecord || diffScore == 0) ? "+" : "-");
			_achivementDiffText.ChangeText(text + ((float)diffScore / 10000f).ToString("##0.0000").PadLeft(8) + "%");
			_achivementDiffText.SetColor((isNewRecord || diffScore == 0) ? CommonScriptable.GetColorSetting().RiseColor : CommonScriptable.GetColorSetting().DeclineColor);
			_newRecord.gameObject.SetActive(isNewRecord);
			_isNewRecord = isNewRecord;
		}

		public void SetAchievementRate(uint score)
		{
			_counterObject.SetAchievement(0u, score);
		}

		public void SetRankOrder(uint rank)
		{
			_scoreBoardDisplay.SetRankOrder((int)rank);
		}

		public void SetRankOrder(int playerIndex, uint player01Order, uint player02Order)
		{
			switch (playerIndex)
			{
			case 0:
				_scoreBoardDisplay.SetRankOrder((int)player01Order);
				break;
			case 1:
				_scoreBoardDisplay.SetRankOrder((int)player02Order);
				break;
			}
		}

		public void SetCombo(uint currentCombo, uint maxCombo, float rate)
		{
			_scoreBoardDisplay.SetCombo(currentCombo, maxCombo);
		}

		public void StartStagingClear()
		{
			_ = isPlayerActive;
		}

		public void SetDxRate(uint dxRate, int prevRate, int diff, int arrowState, Sprite currentPlate, Sprite prevPlate, bool isColorChange)
		{
			_ratingDirecting.Initialize();
			_changeRate = dxRate;
			_changeRateSprite = currentPlate;
			_isDXRatingUP = false;
			if (arrowState == 0)
			{
				_isDXRatingUP = true;
				_ratingDirecting.SetData(prevRate, (int)dxRate, prevPlate, currentPlate);
				_dxRatingText.ChangeText(string.Format("{0,5}", prevRate));
				_dxRatingBackground.sprite = (isColorChange ? prevPlate : currentPlate);
			}
			else
			{
				string text = string.Format("{0,5}", dxRate);
				_dxRatingText.ChangeText(text);
				_dxRatingBackground.sprite = currentPlate;
			}
			_dxRatingArrow.ChangeSprite(arrowState);
			_dxRatingArrowBackImage.ChangeSprite(arrowState);
			string arg = ((arrowState == 0) ? "+" : "");
			_dxRatingDirection = arrowState;
			_dxDiffText.text = $"{arg}{diff}";
			_timelineManager.SetDXRatingUP(arrowState == 0, isColorChange, OnChangeRateUP);
		}

		private void OnChangeRateUP()
		{
			_dxRatingText.ChangeText(string.Format("{0,5}", _changeRate));
			_dxRatingBackground.sprite = _changeRateSprite;
		}

		public void SetDxScore(uint dxScore, int fluctuation, int dxMax, DeluxcorerankrateID dxRank)
		{
			_scoreBoardDisplay.SetDxScore(dxScore, fluctuation, dxMax, dxRank);
			_dxRank = dxRank;
		}

		public void SetDerakkumaMessages(string[] messages)
		{
		}

		public void ResetStaging()
		{
			_counterObject.OnBehaviourPlay();
			_kopResultWindow.gameObject.SetActive(value: false);
			if (isPlayerActive)
			{
				_timelineManager.ResetTimeline();
			}
			_isReleaseTimelien = false;
		}

		public void SetNextWait()
		{
			_blurObject.SetActive(value: true);
			_resultButtonController.SetVisible(false, 2, 3);
		}

		public void SetVisibleBlur(bool isVisible)
		{
			_blurObject.SetActive(isVisible);
		}

		public void SetGhostResult()
		{
			_resultButtonController.SetVisible(false, InputManager.ButtonSetting.Button04);
		}

		public void SetDisable()
		{
			_rankParentObject.SetActive(value: false);
			_scoreBoardController.gameObject.SetActive(value: false);
			_fastLateGameObject.SetActive(value: false);
			_kopResultWindow.gameObject.SetActive(value: false);
		}

		public void TouchPartnerCharacter()
		{
			_navigationCharacter.Play(NavigationAnime.FunStart, 0f);
		}

		public void SetGameScoreType(ConstParameter.ScoreKind kind)
		{
			int index = ((kind != ConstParameter.ScoreKind.Deluxe) ? 1 : 0);
			_gameTypeImage.ChangeSprite(index);
		}

		public void SetPerfectChallenge(int life, bool isClear)
		{
			_perfectChallengeCounter.rectTransform.anchoredPosition = new Vector2(0f, 0f);
			string text;
			if (life < 10)
			{
				text = $" {life} ";
			}
			else if (life < 100)
			{
				text = $" {life}";
				_perfectChallengeCounter.rectTransform.anchoredPosition = new Vector2(-6.5f, 0f);
			}
			else
			{
				text = $"{life}";
			}
			_perfectChallengeCounter.ChangeText(text);
			_timelineManager.SetPerfectChallengeClear(isClear);
		}

		public void SetActivePerfectChallenge(bool isActive)
		{
			_perfectChallengeDisplayObject.SetActive(isActive);
		}

		public void InitConventionResultWindow(int maxTrack)
		{
			_kopResultWindow.Initialize(base.MonitorIndex, maxTrack);
			_kopResultWindow.gameObject.SetActive(value: false);
		}

		public void SetConventionTrack(int trackNo)
		{
			_kopResultWindow.SetTrackDisplay(trackNo, isOpen: false);
		}

		public void SetConventionData(int trackNumber, int achievement, int deluxeScore, int difficulty, MusicLevelID levelId, int level, ConstParameter.ScoreKind kind, string musicName, Texture2D jacket)
		{
			_kopResultWindow.SetResultData(trackNumber, musicName, kind, jacket, (uint)achievement, deluxeScore, level, levelId, difficulty);
		}

		public void SetTotalConventionData(uint achievement, int deluxeScore)
		{
			_kopResultWindow.SetTotalResult(achievement, deluxeScore);
		}

		public void SetTotalRankData(int rank, int diff)
		{
			_kopResultWindow.SetTotalRankData(rank, diff);
		}

		public void PlayConventionFadeIn(int currentTrack)
		{
			_kopResultWindow.gameObject.SetActive(value: true);
			_kopResultWindow.PlayFadeIn(currentTrack);
		}

		public void PlayConventionFadeOut()
		{
			_kopResultWindow.PlayFadeOut(delegate
			{
				_kopResultWindow.gameObject.SetActive(value: false);
			});
		}

		public void SetActiveConventionWindow(bool isActive)
		{
			_kopResultWindow.gameObject.SetActive(isActive);
		}
	}
}
