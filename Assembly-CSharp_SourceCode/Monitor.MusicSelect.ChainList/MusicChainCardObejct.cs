using DB;
using ExpansionImage;
using Manager;
using Monitor.MusicSelect.OtherParts;
using TMPro;
using UI;
using UI.DaisyChainList;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.MusicSelect.ChainList
{
	public class MusicChainCardObejct : ChainObject
	{
		[SerializeField]
		[Header("Extention")]
		private Image _background;

		[SerializeField]
		[Header("モード変更アニメーション")]
		private Animator _modeChangeAnim;

		[SerializeField]
		[Header("楽曲")]
		private CustomTextScroll _titleTextScroll;

		[SerializeField]
		[Header("アーティスト名")]
		private CustomTextScroll _artistNameTextScroll;

		[SerializeField]
		[Header("ジャケット画像")]
		private RawImage _jacketImage;

		[SerializeField]
		[Header("ノーツ作者名")]
		private CustomTextScroll _noteDesignerNameText;

		[SerializeField]
		[Header("解禁表示")]
		private MultipleImage _lockInfomationImage;

		[SerializeField]
		[Header("ランク")]
		private MultipleImage _rankImage;

		[SerializeField]
		[Header("達成率")]
		private GameObject _achievementObject;

		[SerializeField]
		private TextMeshProUGUI _achievementIntegerText;

		[SerializeField]
		private TextMeshProUGUI _achievementDecimalText;

		[SerializeField]
		[Header("レベル表示")]
		private SpriteCounter _digitLevel;

		[SerializeField]
		private SpriteCounter _doubleDigitLevel;

		[SerializeField]
		private Image _levelTextImage;

		[SerializeField]
		private MultipleImage _difficultyBase;

		[SerializeField]
		[Header("称号")]
		private MultipleImage _medalImage;

		[SerializeField]
		private MultipleImage _multiMedalImage;

		[SerializeField]
		[Header("ラベル")]
		private GameObject _newObject;

		[SerializeField]
		private GameObject _bonusImage;

		[SerializeField]
		private GameObject _unlockImage;

		[SerializeField]
		private GameObject _ratingImage;

		[SerializeField]
		private GameObject _ghostRankImage;

		[SerializeField]
		private MultipleImage _ghostRankNum;

		[SerializeField]
		[Header("DXスコア")]
		private Animator _deluxcoreAnim;

		[SerializeField]
		private TextMeshProUGUI _dxScore;

		[SerializeField]
		private TextMeshProUGUI _dxScoreMax;

		[SerializeField]
		private Image _dxScoreTtileMask;

		[SerializeField]
		[Header("BPM")]
		private TextMeshProUGUI _bpm;

		[SerializeField]
		[Header("ゴースト対戦")]
		private GameObject _ghostObject;

		[SerializeField]
		private MultipleImage _ghostImage;

		[SerializeField]
		private MultipleImage _playerImage;

		[SerializeField]
		private MultipleImage _playerImage2;

		[SerializeField]
		private Animator _bossAnimation;

		[SerializeField]
		[Header("リマスター存在アイコン")]
		private MultipleImage _remasterExistIcon;

		[SerializeField]
		[Header("ジャンルベース")]
		private GameObject _genreBase;

		[SerializeField]
		[Header("ジャンルのスプライト")]
		private Image _genreSprite;

		[SerializeField]
		[Header("ジャンルの背景カラー変更用")]
		private Image _genreBaseColor;

		[SerializeField]
		[Header("装飾")]
		private MultipleImage _clearCenterDecoration;

		[SerializeField]
		private MultipleImage _clearUnderDecoration;

		[SerializeField]
		private MaskingImage _starMask;

		[SerializeField]
		[Header("newアイコン")]
		private GameObject _originalNewIconObj;

		[SerializeField]
		private Transform _newIconPosition;

		[SerializeField]
		[Header("")]
		private GameObject _starSSSMask;

		[SerializeField]
		[Header("タグベース画像")]
		private Image _tagBaseFront;

		[SerializeField]
		private Image _tagAddFront;

		[SerializeField]
		private Image _tagBaseBack;

		[SerializeField]
		private Image _tagAddBack;

		[SerializeField]
		private Color[] _starColors;

		[SerializeField]
		private GameObject _challengeObj;

		[SerializeField]
		private SpriteCounter _challengeLife;

		[SerializeField]
		private Animator _challengeEffect;

		[SerializeField]
		[Header("権利表記")]
		private RawImage _copyRightImage;

		[SerializeField]
		[Header("オリジナルのランキングインフォ")]
		private ScoreAttackRankingInfo _originalScoreAttackRankingInfo;

		[SerializeField]
		[Header("ランキングインフォのポジション")]
		private Transform _scoreAttackRankingInfoPos;

		[SerializeField]
		[Header("ミニカード")]
		private MusicChainCardObejct _miniCard;

		[SerializeField]
		private CanvasGroup _miniCardGroup;

		[SerializeField]
		private CanvasGroup _mainCardGroup;

		[SerializeField]
		private Animator _animator;

		private NewIconObject _newIconObj;

		private ScoreAttackRankingInfo _scoreAttackRankingInfo;

		protected override void Awake()
		{
			base.Awake();
			if (_originalScoreAttackRankingInfo != null && _scoreAttackRankingInfoPos != null)
			{
				_scoreAttackRankingInfo = Object.Instantiate(_originalScoreAttackRankingInfo, _scoreAttackRankingInfoPos);
				_scoreAttackRankingInfo.Initialize();
			}
		}

		public Texture2D GetJacket()
		{
			return _jacketImage.texture as Texture2D;
		}

		public void SetMusicData(string musicName, string artistName, string noteDesignerName, int bpm, Texture2D jacket, int difficulty)
		{
			_titleTextScroll.SetData(musicName);
			_titleTextScroll.ResetPosition();
			if (_artistNameTextScroll != null)
			{
				_artistNameTextScroll.SetData(artistName);
				_artistNameTextScroll.ResetPosition();
			}
			if (_noteDesignerNameText != null)
			{
				if (difficulty <= 1 && noteDesignerName == "")
				{
					_noteDesignerNameText.SetData("-");
				}
				else
				{
					_noteDesignerNameText.SetData(noteDesignerName);
				}
				_noteDesignerNameText.ResetPosition();
			}
			_jacketImage.texture = jacket;
			if (_bpm != null)
			{
				_bpm.text = "BPM " + bpm.ToString("000");
			}
			_bonusImage.SetActive(value: false);
			_unlockImage.SetActive(value: false);
			if (_miniCard != null)
			{
				_miniCard.SetMusicData(musicName, artistName, noteDesignerName, bpm, jacket, difficulty);
			}
		}

		public void SetNewLable(bool isVisible)
		{
			if (_newIconObj == null)
			{
				_newIconObj = Object.Instantiate(_originalNewIconObj, _newIconPosition).GetComponent<NewIconObject>();
			}
			_newIconObj.SetView(isVisible);
			if (_miniCard != null)
			{
				_miniCard.SetNewLable(isVisible);
			}
		}

		public void SetBonus(bool isVisible)
		{
			_bonusImage.SetActive(isVisible);
			if (_miniCard != null)
			{
				_miniCard.SetBonus(isVisible);
			}
		}

		public void SetLabel(bool isUnlock, bool isRating)
		{
			_unlockImage.SetActive(isUnlock);
			_ratingImage.SetActive(isRating);
			if (_miniCard != null)
			{
				_miniCard.SetLabel(isUnlock, isRating);
			}
		}

		public void SetPlayerPlate(bool isVisible, int playerIndex, int playerIndex2 = -1)
		{
			if (playerIndex < 0)
			{
				_playerImage.gameObject.SetActive(value: false);
				_playerImage.ChangeSprite(0);
			}
			else
			{
				_playerImage.gameObject.SetActive(isVisible);
				_playerImage.ChangeSprite(playerIndex);
			}
			if (playerIndex2 < 0)
			{
				_playerImage2.gameObject.SetActive(value: false);
				_playerImage2.ChangeSprite(0);
			}
			else
			{
				_playerImage2.gameObject.SetActive(isVisible);
				_playerImage2.ChangeSprite(playerIndex2);
			}
			if (_miniCard != null)
			{
				_miniCard.SetPlayerPlate(isVisible, playerIndex, playerIndex2);
			}
		}

		public void SetDifficulty(Sprite background, Sprite tagDeluxe, Sprite tagStandard, Sprite miniBackground = null)
		{
			_background.sprite = background;
			_tagBaseFront.sprite = tagDeluxe;
			_tagBaseBack.sprite = tagStandard;
			if (_miniCard != null)
			{
				_miniCard.SetDifficulty(miniBackground, tagDeluxe, tagStandard);
			}
		}

		public void SetTagAddRank(Sprite tagAddDeluxeSprite, Sprite tagAddStandardSprite)
		{
			if (tagAddDeluxeSprite != null)
			{
				_tagAddFront.sprite = tagAddDeluxeSprite;
				_tagAddFront.gameObject.SetActive(value: true);
			}
			else
			{
				_tagAddFront.gameObject.SetActive(value: false);
			}
			_tagAddBack.gameObject.SetActive(value: false);
			if (_miniCard != null)
			{
				_miniCard.SetTagAddRank(tagAddDeluxeSprite, tagAddStandardSprite);
			}
		}

		public void SetScoreAchivement(MusicClearrankID rank, int achivement, uint dxScore, uint dxScoreMax, PlayComboflagID scoreAchivment, PlaySyncflagID sync)
		{
			if (achivement >= 0)
			{
				_achievementObject.SetActive(value: true);
				int num = achivement / 10000;
				_achievementIntegerText.text = num.ToString();
				_achievementDecimalText.text = "." + achivement.ToString("0000000").Substring(3, 4);
			}
			else
			{
				_achievementObject.SetActive(value: false);
			}
			if (_deluxcoreAnim != null && _dxScore != null && _dxScoreMax != null)
			{
				if (dxScore != 0)
				{
					_deluxcoreAnim.gameObject.SetActive(value: true);
					_dxScore.gameObject.SetActive(value: true);
					_dxScoreMax.gameObject.SetActive(value: true);
					if (dxScoreMax != 0)
					{
						switch (GameManager.GetDeluxcoreRank((int)(dxScore * 100 / dxScoreMax)))
						{
						case DeluxcorerankrateID.Rank_00:
							_deluxcoreAnim.Play("Star_None");
							break;
						case DeluxcorerankrateID.Rank_01:
							_deluxcoreAnim.Play("Star_01");
							break;
						case DeluxcorerankrateID.Rank_02:
							_deluxcoreAnim.Play("Star_02");
							break;
						case DeluxcorerankrateID.Rank_03:
							_deluxcoreAnim.Play("Star_03");
							break;
						case DeluxcorerankrateID.Rank_04:
							_deluxcoreAnim.Play("Star_04");
							break;
						case DeluxcorerankrateID.Rank_05:
							_deluxcoreAnim.Play("Star_05");
							break;
						default:
							_deluxcoreAnim.Play("Star_None");
							break;
						}
					}
					else
					{
						_deluxcoreAnim.Play("Star_None");
					}
					_dxScore.text = dxScore.ToString();
					_dxScoreMax.text = dxScoreMax.ToString();
				}
				else
				{
					_deluxcoreAnim.gameObject.SetActive(value: false);
					_dxScoreMax.gameObject.SetActive(value: false);
					_dxScoreMax.gameObject.SetActive(value: false);
				}
			}
			if (rank == MusicClearrankID.Invalid)
			{
				_rankImage.gameObject.SetActive(value: false);
			}
			else
			{
				_rankImage.gameObject.SetActive(value: true);
				_rankImage.ChangeSprite((int)rank);
			}
			SetDecoration(rank);
			_medalImage.gameObject.SetActive(value: false);
			if (scoreAchivment > PlayComboflagID.None)
			{
				_medalImage.gameObject.SetActive(value: true);
				_medalImage.ChangeSprite((int)(scoreAchivment - 1));
			}
			if (sync > PlaySyncflagID.None)
			{
				_multiMedalImage.gameObject.SetActive(value: true);
				_multiMedalImage.ChangeSprite((int)(sync - 1));
			}
			if (_miniCard != null)
			{
				_miniCard.SetScoreAchivement(rank, achivement, dxScore, dxScoreMax, scoreAchivment, sync);
			}
		}

		public void SetLevel(int level, MusicLevelID levelID, int difficulty)
		{
			if (level > 9)
			{
				_digitLevel.gameObject.SetActive(value: false);
				_doubleDigitLevel.gameObject.SetActive(value: true);
				_doubleDigitLevel.ChangeText(levelID.GetLevelNum().PadRight(3));
			}
			else
			{
				_digitLevel.gameObject.SetActive(value: true);
				_doubleDigitLevel.gameObject.SetActive(value: false);
				_digitLevel.ChangeText(levelID.GetLevelNum().PadRight(2));
			}
			Sprite[] musicLevelSprites = CommonPrefab.GetMusicLevelSprites(difficulty);
			_digitLevel.SetSpriteSheet(musicLevelSprites);
			_doubleDigitLevel.SetSpriteSheet(musicLevelSprites);
			_levelTextImage.sprite = musicLevelSprites[14];
			_difficultyBase.ChangeSprite(difficulty);
			if (_miniCard != null)
			{
				_miniCard.SetLevel(level, levelID, difficulty);
			}
		}

		public void SetCopyright(Texture2D copyrightTexture)
		{
			if (copyrightTexture == null)
			{
				_copyRightImage.transform.parent.gameObject.SetActive(value: false);
				return;
			}
			if (!_copyRightImage.transform.parent.gameObject.activeSelf)
			{
				_copyRightImage.transform.parent.gameObject.SetActive(value: true);
			}
			_copyRightImage.texture = copyrightTexture;
		}

		public void SetScoreAttackRankingInfo(bool isEnable, bool isRanking = false, int score = 0, int ranking = -1)
		{
			if (isEnable)
			{
				_scoreAttackRankingInfo.gameObject.SetActive(value: true);
				_scoreAttackRankingInfo.SetRankingInfo(isRanking, score, ranking);
			}
			else
			{
				_scoreAttackRankingInfo.gameObject.SetActive(value: false);
			}
		}

		public void ChangeSize(bool isMainActive)
		{
			if (_miniCard != null)
			{
				_miniCardGroup.alpha = ((!isMainActive) ? 1 : 0);
				_mainCardGroup.alpha = (isMainActive ? 1 : 0);
			}
		}

		public void SetDisableScore(int type)
		{
			_lockInfomationImage.gameObject.SetActive(value: true);
			_lockInfomationImage.ChangeSprite(type);
			_digitLevel.gameObject.SetActive(value: false);
			_doubleDigitLevel.gameObject.SetActive(value: true);
			_doubleDigitLevel.ChangeText("-- ");
			if (_miniCard != null)
			{
				_miniCard.SetDisableScore(type);
			}
		}

		public void SetLockScore(int type)
		{
			_lockInfomationImage.gameObject.SetActive(value: true);
			_lockInfomationImage.ChangeSprite(type);
			if (_miniCard != null)
			{
				_miniCard.SetLockScore(type);
			}
		}

		public void SetDecoration(MusicClearrankID type)
		{
			if (type < MusicClearrankID.Rank_S || MusicClearrankID.End <= type)
			{
				_clearCenterDecoration.gameObject.SetActive(value: false);
				_clearUnderDecoration.gameObject.SetActive(value: false);
				if (_animator != null && base.gameObject.activeInHierarchy)
				{
					_animator.Play("Idle");
				}
				return;
			}
			_clearCenterDecoration.gameObject.SetActive(value: true);
			_clearUnderDecoration.gameObject.SetActive(value: true);
			switch (type)
			{
			case MusicClearrankID.Rank_S:
			case MusicClearrankID.Rank_SP:
				_clearCenterDecoration.ChangeSprite(0);
				_clearUnderDecoration.ChangeSprite(0);
				_starMask.color = _starColors[0];
				if (_animator != null)
				{
					_animator.SetTrigger("S");
				}
				_starMask.gameObject.SetActive(value: true);
				break;
			case MusicClearrankID.Rank_SS:
			case MusicClearrankID.Rank_SSP:
				_clearCenterDecoration.ChangeSprite(1);
				_clearUnderDecoration.ChangeSprite(1);
				_starMask.color = _starColors[1];
				if (_animator != null)
				{
					_animator?.SetTrigger("SS");
				}
				_starMask.gameObject.SetActive(value: true);
				break;
			case MusicClearrankID.Rank_SSS:
			case MusicClearrankID.Rank_SSSP:
				_clearCenterDecoration.ChangeSprite(2);
				_clearUnderDecoration.ChangeSprite(2);
				if (_animator != null)
				{
					_animator?.SetTrigger("SSS");
				}
				_starSSSMask.SetActive(value: true);
				break;
			default:
				if (_animator != null)
				{
					_animator?.Play("Idle");
				}
				break;
			}
			if (_miniCard != null)
			{
				_miniCard.SetDecoration(type);
			}
		}

		public void SetScoreKind(bool isDeluxe, bool deluxeExist, bool standardExist, bool isAnimation)
		{
			if (deluxeExist && standardExist)
			{
				if (isDeluxe)
				{
					if (isAnimation)
					{
						if (_modeChangeAnim != null && base.gameObject.activeInHierarchy)
						{
							_modeChangeAnim.Play("Deluxe_Change");
						}
					}
					else if (_modeChangeAnim != null && base.gameObject.activeInHierarchy)
					{
						_modeChangeAnim.Play("Deluxe_Loop");
					}
				}
				else if (isAnimation)
				{
					if (_modeChangeAnim != null && base.gameObject.activeInHierarchy)
					{
						_modeChangeAnim.Play("Standard_Change");
					}
				}
				else if (_modeChangeAnim != null && base.gameObject.activeInHierarchy)
				{
					_modeChangeAnim.Play("Standard_Loop");
				}
			}
			else if (deluxeExist)
			{
				if (_modeChangeAnim != null && base.gameObject.activeInHierarchy)
				{
					_modeChangeAnim.Play("Deluxe_Only");
				}
			}
			else if (standardExist && _modeChangeAnim != null && base.gameObject.activeInHierarchy)
			{
				_modeChangeAnim.Play("Standard_Only");
			}
			if (_miniCard != null)
			{
				_miniCard.SetScoreKind(isDeluxe, deluxeExist, standardExist, isAnimation);
			}
		}

		public void SetRemasterExistIcon(bool isExist, bool isUnlock)
		{
			if (isUnlock)
			{
				_remasterExistIcon.ChangeSprite(1);
			}
			else
			{
				_remasterExistIcon.ChangeSprite(0);
			}
			_remasterExistIcon.gameObject.SetActive(isExist);
			if (_miniCard != null)
			{
				_miniCard.SetRemasterExistIcon(isExist, isUnlock);
			}
		}

		public void SetRanking(Sprite rankingSprite, Color bgColor)
		{
			if (rankingSprite != null)
			{
				_genreBase.SetActive(value: true);
				_genreSprite.sprite = rankingSprite;
				_genreBaseColor.color = bgColor;
			}
			else
			{
				_genreBase.SetActive(value: false);
			}
			if (_miniCard != null)
			{
				_miniCard.SetRanking(rankingSprite, bgColor);
				_genreBaseColor.color = bgColor;
			}
		}

		public void SetChallengeView(bool active)
		{
			_challengeObj.SetActive(active);
			_challengeEffect.gameObject.SetActive(active);
			if (_miniCard != null)
			{
				_miniCard.SetChallengeView(active);
			}
		}

		public void SetChallengeLife(int challengeLife)
		{
			_challengeLife.gameObject.SetActive(value: true);
			_challengeEffect.gameObject.SetActive(value: true);
			_challengeEffect.Play("Loop");
			string text = challengeLife.ToString("D000");
			if (challengeLife > 999)
			{
				text = "999";
			}
			_challengeLife.ChangeText(text);
			if (challengeLife < 100)
			{
				_challengeLife.FrameList[2].Scale = 0f;
				_challengeLife.FrameList[0].RelativePosition.x = 25f;
				_challengeLife.FrameList[1].RelativePosition.x = 8f;
				if (challengeLife < 10)
				{
					_challengeLife.FrameList[1].Scale = 0f;
					_challengeLife.FrameList[0].RelativePosition.x = 34f;
				}
				else
				{
					_challengeLife.FrameList[1].Scale = 1f;
				}
			}
			else
			{
				_challengeLife.FrameList[1].Scale = 1f;
				_challengeLife.FrameList[2].Scale = 1f;
				_challengeLife.FrameList[0].RelativePosition.x = 17f;
				_challengeLife.FrameList[1].RelativePosition.x = 0f;
				_challengeLife.FrameList[2].RelativePosition.x = -17f;
			}
			_challengeLife.ChangeText(challengeLife.ToString());
			if (_miniCard != null)
			{
				_miniCard.SetChallengeLife(challengeLife);
			}
		}

		public void SetGhost(bool isVisisble, int type)
		{
			_ghostObject.SetActive(isVisisble);
			_ghostImage.ChangeSprite(type);
			_ghostImage.SetNativeSize();
			if (_bossAnimation != null)
			{
				if (type == 2 && isVisisble)
				{
					_bossAnimation.gameObject.SetActive(value: true);
					_bossAnimation?.Play("Loop");
				}
				else
				{
					_bossAnimation.gameObject.SetActive(value: false);
				}
			}
			if (_miniCard != null)
			{
				_miniCard.SetGhost(isVisisble, type);
			}
		}

		public void SetGhost(Sprite sprite)
		{
			if (!_ghostObject.activeSelf)
			{
				_ghostObject.SetActive(value: true);
			}
			_ghostImage.sprite = sprite;
			_ghostImage.SetNativeSize();
			if (_miniCard != null)
			{
				_miniCard.SetGhost(sprite);
			}
		}

		public void SetGhostRank(bool isVisisble, MusicClearrankID rank)
		{
			if (rank == MusicClearrankID.Invalid || !isVisisble)
			{
				_ghostRankImage.SetActive(value: false);
			}
			else
			{
				_ghostRankImage.SetActive(value: true);
				switch (rank)
				{
				case MusicClearrankID.Rank_D:
				case MusicClearrankID.Rank_C:
				case MusicClearrankID.Rank_B:
					_ghostRankNum.ChangeSprite(0);
					break;
				case MusicClearrankID.Rank_A:
					_ghostRankNum.ChangeSprite(1);
					break;
				case MusicClearrankID.Rank_AA:
					_ghostRankNum.ChangeSprite(2);
					break;
				case MusicClearrankID.Rank_AAA:
					_ghostRankNum.ChangeSprite(3);
					break;
				case MusicClearrankID.Rank_S:
					_ghostRankNum.ChangeSprite(4);
					break;
				case MusicClearrankID.Rank_SP:
					_ghostRankNum.ChangeSprite(5);
					break;
				case MusicClearrankID.Rank_SS:
					_ghostRankNum.ChangeSprite(6);
					break;
				case MusicClearrankID.Rank_SSP:
					_ghostRankNum.ChangeSprite(7);
					break;
				case MusicClearrankID.Rank_SSS:
					_ghostRankNum.ChangeSprite(8);
					break;
				case MusicClearrankID.Rank_SSSP:
					_ghostRankNum.ChangeSprite(9);
					break;
				}
			}
			if (_miniCard != null)
			{
				_miniCard.SetGhostRank(isVisisble, rank);
			}
		}

		public override void ResetChain()
		{
			_ghostObject.SetActive(value: false);
			_lockInfomationImage.gameObject.SetActive(value: false);
			_rankImage.gameObject.SetActive(value: true);
			_dxScoreTtileMask?.gameObject.SetActive(value: true);
			_medalImage.gameObject.SetActive(value: false);
			_multiMedalImage.gameObject.SetActive(value: false);
			_clearCenterDecoration.gameObject.SetActive(value: false);
			_starMask.gameObject.SetActive(value: false);
			_starSSSMask.SetActive(value: false);
			_playerImage.gameObject.SetActive(value: false);
			_playerImage2.gameObject.SetActive(value: false);
			SetNewLable(isVisible: false);
			SetBonus(isVisible: false);
			SetLabel(isUnlock: false, isRating: false);
			_bossAnimation.gameObject.SetActive(value: false);
			_challengeEffect.gameObject.SetActive(value: false);
			if (_miniCard != null)
			{
				_miniCard.ResetChain();
			}
		}

		public override void ViewUpdate(float syncTimer)
		{
			_titleTextScroll.ViewUpdate();
			if (_artistNameTextScroll != null)
			{
				_artistNameTextScroll.ViewUpdate();
			}
			if (_noteDesignerNameText != null)
			{
				_noteDesignerNameText.ViewUpdate();
			}
			base.ViewUpdate(syncTimer);
		}

		public override void OnCenterIn()
		{
			ChangeSize(isMainActive: true);
			_titleTextScroll.ResetPosition();
			if (_artistNameTextScroll != null)
			{
				_artistNameTextScroll.ResetPosition();
			}
			if (_noteDesignerNameText != null)
			{
				_noteDesignerNameText.ResetPosition();
			}
		}

		public override void OnCenter()
		{
		}

		public override void OnCenterOut()
		{
			ChangeSize(isMainActive: false);
			_titleTextScroll.ResetPosition();
			if (_artistNameTextScroll != null)
			{
				_artistNameTextScroll.ResetPosition();
			}
			if (_noteDesignerNameText != null)
			{
				_noteDesignerNameText.ResetPosition();
			}
		}

		public override void OnCenterOutEnd()
		{
		}
	}
}
