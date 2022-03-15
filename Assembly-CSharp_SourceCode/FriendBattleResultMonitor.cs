using System;
using System.Collections;
using DB;
using Mai2.Mai2Cue;
using Mai2.Voice_000001;
using Manager;
using Manager.UserDatas;
using Monitor;
using Timeline;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class FriendBattleResultMonitor : MonitorBase
{
	private enum StagingState
	{
		Init,
		BattleResult,
		GemResult,
		Boss,
		Legend,
		End
	}

	private readonly string[] _classPointMaxMColorCodes = new string[5] { "#C4FFC8", "#FBD5D5", "#E6D1BD", "#CBDFF6", "#FFE9B9" };

	[SerializeField]
	[Header("ボタン管理")]
	private FriendBattleResultButtonController _buttonController;

	[SerializeField]
	[Header("表示系")]
	private GameObject _gemObject;

	[SerializeField]
	private CanvasGroup _legendObject;

	[SerializeField]
	[Header("ユーザー情報")]
	private UserInformationController _leftPlayerController;

	[SerializeField]
	private UserInformationController _rightPlayerController;

	[SerializeField]
	[Header("達成率")]
	private AchievementCounterObject _leftAchievement;

	[SerializeField]
	private AchievementCounterObject _rightAchievement;

	[SerializeField]
	[Header("ブラー")]
	private CanvasGroup _frontBlurObject;

	[SerializeField]
	[Header("ナビキャラクター")]
	private Transform _characterParent;

	[SerializeField]
	private MultipleImage _informationImage;

	[SerializeField]
	[Header("ゲージ")]
	private Animator _gaugeAnimator;

	[SerializeField]
	private Image _upperGauge;

	[SerializeField]
	private Image _downerGauge;

	[SerializeField]
	[Header("Gem関連")]
	private Animator _gemSetAnimator;

	[SerializeField]
	private MultipleImage _classBaseImage;

	[SerializeField]
	private TextMeshProUGUI _classPointCountMax;

	[SerializeField]
	private TextMeshProUGUI _classPointCount;

	[SerializeField]
	[Header("増減表示")]
	private Animator _countIconAnimator;

	[SerializeField]
	private TextMeshProUGUI _countPlusText;

	[SerializeField]
	private TextMeshProUGUI _countMinusText;

	[SerializeField]
	[Header("ポップアップ")]
	private Animator _gaugePopupAnimator;

	[SerializeField]
	private MultipleImage _popupGemImage;

	[SerializeField]
	private MultipleImage _popupGemEffectImage;

	[SerializeField]
	private ClassMedalObject _classGemMedalObject;

	[SerializeField]
	[Header("GemPrefab")]
	private GameObject _smallGemObject;

	[SerializeField]
	private GameObject _largeGemObject;

	[SerializeField]
	[Header("台座")]
	private RectTransform _largeGemPedestal;

	[SerializeField]
	private GemPedestalController[] _gemPedestalControllers;

	[SerializeField]
	[Header("Text Material")]
	private Material[] _materials;

	[SerializeField]
	[Header("ClassUp Down")]
	private CanvasGroup _classUpGroup;

	[SerializeField]
	private Animator _classUpAnimator;

	[SerializeField]
	private ClassMedalObject _classUpNewMedalObject;

	[SerializeField]
	private ClassMedalObject _classUpOldMedalObject;

	[SerializeField]
	private Animator _classDownAnimator;

	[SerializeField]
	[Header("Animator")]
	private Animator _userInfoAnimator;

	[SerializeField]
	private Animator _winAnimator;

	[SerializeField]
	private Animator _loseAnimator;

	[SerializeField]
	private Animator _bossWinAnimator;

	[SerializeField]
	private Animator _gemBaseAnimator;

	[SerializeField]
	[Header("レジェンド")]
	private Animator _legendAnimator;

	[SerializeField]
	private Animator _legendCountAnimator;

	[SerializeField]
	[Header("Boss")]
	private Animator _bossBgAnimator;

	[SerializeField]
	private Animator _bossCutInAnimator;

	[SerializeField]
	private ExtraInfoController _bossInfoController;

	[SerializeField]
	private UnlockMusicWindow _unlockWindowController;

	[SerializeField]
	private SpriteCounter _legendSpriteCounter;

	private uint _sc;

	private Animator _winLoseInfo;

	private StagingState _state;

	private Animator _largeGemAnimator;

	private MultipleImage _largeGemBackImage;

	private NavigationCharacter _navigationCharacter;

	private bool _isVictory;

	private bool _isBossBattle;

	private bool _isBossEntry;

	private bool _isBossAppearance;

	private bool _isBossDeprivation;

	private bool _isBossSpecial;

	private bool _isBossBattleEnable;

	private bool _isBossExist;

	private int _classDirection;

	private int _classPointDirection;

	private int _smallGemIndex;

	private int _fromCP;

	private int _toCP;

	private int _frameCounter;

	private int _cp;

	private int _gem;

	private int _class;

	private int _direction;

	private int _criteria;

	private Animator _stateInfo;

	private Action _callback;

	private UdemaeID _prevClassID;

	private UdemaeID _nextClassID;

	public bool IsSkip { get; private set; }

	private uint _stagingCount
	{
		get
		{
			return _sc;
		}
		set
		{
			_sc = value;
		}
	}

	public override void Initialize(int playerIndex, bool isActive)
	{
		base.Initialize(playerIndex, isActive);
		IsSkip = false;
		if (!isActive)
		{
			Main.alpha = 0f;
			return;
		}
		_buttonController.Initialize(playerIndex);
		_bossInfoController.Initialize();
		_classUpGroup.alpha = 0f;
		_state = StagingState.Init;
	}

	public void SetDisable()
	{
		_frontBlurObject.alpha = 1f;
		_informationImage.gameObject.SetActive(value: false);
		_gemBaseAnimator.gameObject.SetActive(value: false);
	}

	public override void ViewUpdate()
	{
		switch (_state)
		{
		case StagingState.BattleResult:
			DoBattleResult();
			break;
		case StagingState.GemResult:
			DoGemResult();
			break;
		case StagingState.Boss:
			DoBoss();
			break;
		case StagingState.Legend:
			DoLegend();
			break;
		}
	}

	public void SetUserData(MessageUserInformationData playerData, uint userAchievement, int navCharID, MessageUserInformationData rivalData, uint rivalAchievement)
	{
		_leftPlayerController.SetUserData(playerData);
		_rightPlayerController.SetUserData(rivalData);
		_leftAchievement.SetAchievement(0u, userAchievement);
		_rightAchievement.SetAchievement(0u, rivalAchievement);
		_leftAchievement.OnClipTailEnd();
		_rightAchievement.OnClipTailEnd();
		NavigationCharacter component = AssetManager.Instance().GetNaviCharaPrefab(navCharID).GetComponent<NavigationCharacter>();
		_navigationCharacter = UnityEngine.Object.Instantiate(component, _characterParent);
		_navigationCharacter.SetBool("IsClear", _isVictory);
	}

	public void SetBossInformation(MessageUserInformationData info)
	{
		_bossInfoController.BossObj[0].SetUserData(info);
		_bossInfoController.BossObj[0].gameObject.SetActive(value: true);
		bool isSpecialBoss = UserUdemae.IsBossSpecial(info.ClassID);
		_bossInfoController.SetGhostActive(isActive: true, isBoss: true, isSpecialBoss);
	}

	public void SetUnlockInformation()
	{
	}

	public void SetFlagData(bool isVictory, bool isBossBattle, bool isBossEntry, bool isBossAppearance, bool isBossDeprivation, bool isBossSpecial, bool isBossBattleEnable, bool isBossExist)
	{
		_isVictory = isVictory;
		_isBossBattle = isBossBattle;
		_isBossEntry = isBossEntry;
		_isBossAppearance = isBossAppearance;
		_isBossDeprivation = isBossDeprivation;
		_isBossSpecial = isBossSpecial;
		_isBossBattleEnable = isBossBattleEnable;
		_isBossExist = isBossExist;
		if (_isBossSpecial)
		{
			_unlockWindowController.gameObject.SetActive(value: false);
		}
	}

	private void ResetGemBoard(UdemaeID classID)
	{
		int num = 0;
		if (UdemaeID.Class_SSS5 <= classID)
		{
			num = 4;
		}
		else if (UdemaeID.Class_SS5 <= classID)
		{
			num = 3;
		}
		else if (UdemaeID.Class_S5 <= classID)
		{
			num = 2;
		}
		else if (UdemaeID.Class_A5 <= classID)
		{
			num = 1;
		}
		_classBaseImage.ChangeSprite(num);
		_classPointCountMax.fontMaterial = _materials[num];
		_classPointCount.fontMaterial = _materials[num];
		_classPointCountMax.text = classID.GetBorder().ToString();
		if (ColorUtility.TryParseHtmlString(_classPointMaxMColorCodes[num], out var color))
		{
			_classPointCountMax.color = color;
		}
		_largeGemBackImage.ChangeSprite(_isBossExist ? 1 : 0);
		if (UdemaeID.Class_A4 <= classID)
		{
			_informationImage.ChangeSprite(1);
		}
		else
		{
			_informationImage.ChangeSprite(0);
		}
		if (classID == UdemaeID.Class_LEGEND)
		{
			_informationImage.gameObject.SetActive(value: false);
		}
	}

	private void SetClassUpDown(UdemaeID prevClassID, UdemaeID nextClassID)
	{
		Sprite sprite = Resources.Load<Sprite>($"Process/FriendBattleResult/Sprites/ClassMedal/UI_FBR_Class_{(int)nextClassID:00}");
		Sprite currentMedal = Resources.Load<Sprite>($"Process/FriendBattleResult/Sprites/ClassMedal/UI_FBR_Class_{(int)prevClassID:00}");
		_classUpNewMedalObject.SetCurrentMedal(sprite);
		_classUpOldMedalObject.SetCurrentMedal(currentMedal);
		_classGemMedalObject.SetCurrentMedal(currentMedal);
		_classGemMedalObject.SetNextMedal(sprite);
	}

	private void SetGemPopup(UdemaeID classID, int classValue, bool isClassDown = false)
	{
		int index = ((classID.GetBorder() - classValue <= 10) ? 1 : 0);
		if (1 < classID.GetClassBoss() && classID.GetBorder() - classValue <= 10 && _isBossExist && !isClassDown)
		{
			index = 2;
		}
		_popupGemImage.ChangeSprite(index);
		_popupGemEffectImage.ChangeSprite(index);
	}

	public void SetClassData(int prevClassValue, UdemaeID prevClassID, int nextClassValue, UdemaeID nextClassID)
	{
		_prevClassID = prevClassID;
		_nextClassID = nextClassID;
		Sprite currentMedal = Resources.Load<Sprite>($"Process/FriendBattleResult/Sprites/ClassMedal/UI_FBR_Class_{(int)prevClassID:00}");
		_classGemMedalObject.SetCurrentMedal(currentMedal);
		_largeGemAnimator = UnityEngine.Object.Instantiate(_largeGemObject, _largeGemPedestal).GetComponent<Animator>();
		_largeGemBackImage = _largeGemAnimator.transform.Find("Gem_L_Set/Blank").GetComponent<MultipleImage>();
		_largeGemBackImage.ChangeSprite(_isBossExist ? 1 : 0);
		ResetGemBoard(prevClassID);
		if (_isBossEntry)
		{
			_informationImage.ChangeSprite(2);
		}
		else if (UdemaeID.Class_A4 <= prevClassID)
		{
			_informationImage.ChangeSprite(1);
		}
		else
		{
			_informationImage.ChangeSprite(0);
		}
		if (_prevClassID == UdemaeID.Class_LEGEND)
		{
			_informationImage.gameObject.SetActive(value: false);
		}
		if (_isBossBattle || _isBossEntry)
		{
			_largeGemAnimator.Play("Loop_Boss", 0, 0f);
			_bossBgAnimator.Play("BossBG", 0, 0f);
		}
		else
		{
			_largeGemAnimator.Play("Loop_Blank", 0, 0f);
		}
		int border = prevClassID.GetBorder();
		int num = border / 10 - 1;
		if (0 < num && num <= _gemPedestalControllers.Length)
		{
			_smallGemIndex = num - 1;
			_gemPedestalControllers[_smallGemIndex].GemGenerate(_smallGemObject);
			_gemPedestalControllers[_smallGemIndex].SetGemActivate(Mathf.FloorToInt((float)prevClassValue / 10f));
		}
		SetClassPointText(prevClassValue.ToString());
		_classPointCountMax.text = border.ToString();
		_classPointDirection = 0;
		_classDirection = 0;
		_gaugePopupAnimator.Play("Loop", 0, 0f);
		if (prevClassID < nextClassID)
		{
			_classDirection = nextClassID - prevClassID;
			SetClassUpDown(prevClassID, prevClassID + 1);
			int num2 = 0;
			for (int i = (int)_prevClassID; i < (int)_nextClassID; i++)
			{
				num2 += ((UdemaeID)i).GetBorder();
			}
			_direction = 1;
			_classPointDirection = nextClassValue + num2 - prevClassValue;
			_gaugeAnimator.Play("Change_Up");
			if (!_isBossBattle)
			{
				_gem = prevClassValue / 10;
			}
		}
		else if (prevClassID > nextClassID)
		{
			_classDirection = nextClassID - prevClassID;
			int num3 = 0;
			int num4 = (int)(_prevClassID - 1);
			while ((int)_nextClassID <= num4)
			{
				num3 += ((UdemaeID)num4).GetBorder();
				num4--;
			}
			_direction = -1;
			_classPointDirection = -(prevClassValue + num3 - nextClassValue);
			_gaugeAnimator.Play("Change_Down");
		}
		if (_isBossAppearance)
		{
			_direction = 1;
		}
		if (_classDirection == 0)
		{
			if (prevClassValue < nextClassValue)
			{
				_direction = 1;
				_cp = 0;
				_gem = prevClassValue / 10;
				_classPointDirection = nextClassValue - prevClassValue;
				_gaugeAnimator.Play("Change_Up");
			}
			else if (prevClassValue > nextClassValue)
			{
				_direction = -1;
				_cp = 0;
				_gem = prevClassValue / 10;
				_classPointDirection = nextClassValue - prevClassValue;
				_gaugeAnimator.Play("Change_Down");
			}
			else
			{
				_gaugeAnimator.Play("Change_Up");
			}
		}
		SetGemPopup(prevClassID, prevClassValue);
		int num5 = ((prevClassValue != prevClassID.GetBorder()) ? (prevClassValue % 10) : ((0 < prevClassValue) ? ((prevClassValue % 10 == 0) ? 10 : (prevClassValue % 10)) : 0));
		_upperGauge.fillAmount = ((num5 < 4) ? ((float)num5 / 10f - 0.01f) : ((float)num5 / 10f));
		_downerGauge.fillAmount = ((num5 < 4) ? ((float)num5 / 10f - 0.01f) : ((float)num5 / 10f));
		_fromCP = prevClassValue;
		_toCP = nextClassValue;
		_criteria = ((prevClassValue % 10 != 0) ? (prevClassValue % 10) : 0);
		if (_prevClassID == UdemaeID.Class_LEGEND)
		{
			_gemBaseAnimator.gameObject.SetActive(value: false);
			_legendAnimator.Play("Loop", 0, 0f);
			if (10 <= prevClassValue)
			{
				_legendSpriteCounter.AddFormatFream();
			}
			if (100 <= prevClassValue)
			{
				_legendSpriteCounter.AddFormatFream();
			}
			if (1000 <= prevClassValue)
			{
				_legendSpriteCounter.AddFormatFream();
			}
			if (9999 < prevClassValue)
			{
				prevClassValue = 9999;
			}
			_legendSpriteCounter.ChangeText(prevClassValue.ToString());
		}
	}

	public void Play()
	{
		_stagingCount = 0u;
		_frameCounter = 0;
		IsSkip = false;
		_state = StagingState.BattleResult;
	}

	public bool IsEnd()
	{
		return _state == StagingState.End;
	}

	public void Skip()
	{
		StagingState state = _state;
		if (state != StagingState.BattleResult)
		{
			_ = 2;
		}
	}

	public void Cleanup()
	{
		StartCoroutine(CleanUpCoroutine());
	}

	private IEnumerator CleanUpCoroutine()
	{
		yield return new WaitWhile(delegate
		{
			float alpha = Main.alpha;
			alpha -= 0.1f;
			Main.alpha = alpha;
			return alpha > 0f;
		});
	}

	private void DoBattleResult()
	{
		switch (_stagingCount)
		{
		case 0u:
			if (30 <= _frameCounter)
			{
				_stagingCount++;
			}
			else
			{
				_frameCounter++;
			}
			break;
		case 1u:
			_userInfoAnimator.Play(Animator.StringToHash("In"), 0, 0f);
			_stagingCount++;
			break;
		case 2u:
			if (!(1f < _userInfoAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime))
			{
				break;
			}
			_userInfoAnimator.Play(Animator.StringToHash("Loop"), 0, 0f);
			if (_isVictory)
			{
				if (_isBossBattle)
				{
					_bossWinAnimator.Play("YouWin_Boss", 0, 0f);
					_winLoseInfo = _bossWinAnimator;
				}
				else
				{
					_winAnimator.Play("YouWin", 0, 0f);
					_winLoseInfo = _winAnimator;
				}
				SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000142, base.MonitorIndex);
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_RESULT_WIN, base.MonitorIndex);
			}
			else
			{
				_loseAnimator.Play("YouLose", 0, 0f);
				_winLoseInfo = _loseAnimator;
				SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000143, base.MonitorIndex);
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_RESULT_LOSE, base.MonitorIndex);
			}
			_stagingCount++;
			break;
		case 3u:
			_stagingCount++;
			break;
		case 4u:
			if (1f <= _winLoseInfo.GetCurrentAnimatorStateInfo(0).normalizedTime)
			{
				_userInfoAnimator.Play("Out", 0, 0f);
				_stagingCount++;
				_frameCounter = 0;
			}
			break;
		case 5u:
			if (30 <= _frameCounter)
			{
				if (_nextClassID == UdemaeID.Class_LEGEND)
				{
					_state = StagingState.Legend;
					_stagingCount = ((_prevClassID == UdemaeID.Class_LEGEND) ? 10u : 0u);
				}
				else if (_isBossBattle && _isVictory)
				{
					_frameCounter = 15;
					_state = StagingState.GemResult;
					_stagingCount = 7u;
				}
				else
				{
					_frameCounter = 15;
					_state = StagingState.GemResult;
					_stagingCount = 0u;
				}
			}
			else
			{
				_frameCounter++;
			}
			break;
		}
	}

	private void DoGemResult()
	{
		switch (_stagingCount)
		{
		case 0u:
			if (0 < _direction)
			{
				if (0 < _criteria && _fromCP == _prevClassID.GetBorder() && _criteria % 10 == 0)
				{
					_stagingCount = 2u;
				}
				else
				{
					_stagingCount = 1u;
				}
			}
			else if (_direction < 0)
			{
				if (_criteria == 0 || (_criteria % 10 == 0 && _fromCP < _prevClassID.GetBorder()))
				{
					_stagingCount = 2u;
				}
				else
				{
					_stagingCount = 1u;
				}
			}
			else
			{
				_stagingCount = 99u;
			}
			break;
		case 1u:
			if (5 < _frameCounter)
			{
				_frameCounter = 0;
				if (0 < _classPointDirection)
				{
					int num = _criteria % 10 + (_cp + 1);
					_cp++;
					_classPointDirection--;
					int num2 = _criteria;
					if (_criteria == 10)
					{
						num2 = 0;
					}
					SetClassPointText((_gem * 10 + num2 + _cp).ToString());
					if (_classPointDirection == 0)
					{
						_countPlusText.text = _cp.ToString();
						_countIconAnimator.Play("In_Plus", 0, 0f);
						_stagingCount = 99u;
					}
					if (num % 10 == 0)
					{
						_navigationCharacter.Play(NavigationAnime.FunStart, 0f);
						_countPlusText.text = _cp.ToString();
						_countIconAnimator.Play("In_Plus", 0, 0f);
						_gaugePopupAnimator.Play("Get", 0, 0f);
						_stagingCount = 10u;
						_frameCounter = 30;
						_callback = delegate
						{
							_stagingCount = 2u;
							_callback = null;
						};
					}
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_RESULT_METER_01, base.MonitorIndex);
					_upperGauge.fillAmount = ((num < 4) ? ((float)num / 10f - 0.01f) : ((float)num / 10f));
				}
				else
				{
					if (_classPointDirection >= 0)
					{
						break;
					}
					int num3 = ((0 < _criteria) ? ((_criteria % 10 == 0) ? 10 : (_criteria % 10)) : 0) + (_cp - 1);
					_cp--;
					_classPointDirection++;
					SetClassPointText((_criteria + _cp + _gem * 10).ToString());
					if (num3 == 0)
					{
						_navigationCharacter.Play(NavigationAnime.Sad01, 0f);
						_countMinusText.text = (_cp * -1).ToString();
						_countIconAnimator.Play("In_Minus");
						_stagingCount = 10u;
						_frameCounter = 30;
						_callback = delegate
						{
							_stagingCount = 2u;
							_callback = null;
						};
					}
					if (_classPointDirection == 0)
					{
						if (_isBossDeprivation)
						{
							_largeGemAnimator.Play("Loop_Blank", 0, 0f);
							_bossBgAnimator.Play("Idle", 0, 0f);
							_informationImage.ChangeSprite(1);
						}
						_countMinusText.text = (_cp * -1).ToString();
						_countIconAnimator.Play("In_Minus");
						_stagingCount = 99u;
					}
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_RESULT_METER_01, base.MonitorIndex);
					_downerGauge.fillAmount = ((num3 < 4) ? ((float)num3 / 10f - 0.01f) : ((float)num3 / 10f));
				}
			}
			else
			{
				_frameCounter++;
			}
			break;
		case 2u:
			if (0 < _direction)
			{
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_RESULT_GEM_GET_01, base.MonitorIndex);
				if (_criteria + _cp + 10 * _gem == (_prevClassID + _class).GetBorder())
				{
					_stagingCount = 3u;
					_criteria = 0;
					_classDirection--;
					_largeGemAnimator.Play("Gem_Get", 0, 0f);
					break;
				}
				_upperGauge.fillAmount = 0f;
				_stagingCount = 11u;
				_gemPedestalControllers[_smallGemIndex].GemGet((_criteria + _cp) / 10 - 1 + _gem, delegate
				{
					_gem++;
					_criteria = 0;
					_cp = 0;
					SetGemPopup(_prevClassID + _class, _criteria + _cp + _gem * 10);
					_gaugePopupAnimator.Play("Next", 0, 0f);
					if (0 < _classPointDirection)
					{
						_stagingCount = 1u;
					}
					else
					{
						_stagingCount = 99u;
					}
				});
			}
			else
			{
				if (_direction >= 0)
				{
					break;
				}
				if (_isBossDeprivation)
				{
					_largeGemAnimator.Play("Loop_Blank", 0, 0f);
					_bossBgAnimator.Play("Idle", 0, 0f);
					_informationImage.ChangeSprite(1);
					if (_classPointDirection < 0)
					{
						_cp = 0;
						_criteria = 10;
						_stagingCount = 1u;
					}
					else
					{
						_stagingCount = 99u;
					}
					break;
				}
				if (_criteria + _cp + _gem * -10 == 0)
				{
					_criteria = 10;
					_classDirection++;
					_gem = (_prevClassID - _class - 1).GetBorder() / 10 - 1;
					_classPointDirection = 0;
					_stagingCount = 10u;
					_frameCounter = 15;
					_callback = delegate
					{
						_class--;
						_classDownAnimator.Play("In", 0, 0f);
						_gemSetAnimator.Play("Change", 0, 0f);
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_RESULT_CLASS_DOWN, base.MonitorIndex);
						int num5 = (_prevClassID + _class).GetBorder() / 10 - 2;
						if (0 <= num5)
						{
							_gemPedestalControllers[_smallGemIndex].Dispose();
							_smallGemIndex = num5;
							_gemPedestalControllers[_smallGemIndex].GemGenerate(_smallGemObject);
							_gemPedestalControllers[_smallGemIndex].SetGemActivate(_toCP / 10);
						}
						_gaugePopupAnimator.Play("Out_Next", 0, 0f);
						SetClassUpDown(_prevClassID, _nextClassID);
						ResetGemBoard(_prevClassID + _class);
						SetClassPointText(_toCP.ToString());
						_downerGauge.fillAmount = (float)_toCP % 10f / 10f;
						_classGemMedalObject.ClassDown();
						_stagingCount = 4u;
						_callback = null;
					};
					break;
				}
				_downerGauge.fillAmount = 1f;
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_RESULT_GEM_LOSS_01, base.MonitorIndex);
				_stagingCount = 11u;
				_gemPedestalControllers[_smallGemIndex].BreakGem(_gem - 1, delegate
				{
					_gem--;
					SetGemPopup(_prevClassID, _fromCP + _cp + _classPointDirection);
					_gaugePopupAnimator.Play("Next", 0, 0f);
					_criteria = 10;
					if (_classPointDirection < 0)
					{
						_cp = 0;
						_stagingCount = 1u;
					}
					else
					{
						_stagingCount = 99u;
					}
				});
			}
			break;
		case 3u:
			if (!(1f <= _largeGemAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime))
			{
				break;
			}
			_class++;
			_gem = 0;
			if (_isBossAppearance)
			{
				_largeGemAnimator.Play("Gem_Boss", 0, 0f);
			}
			else
			{
				SetClassPointText("0");
				_upperGauge.fillAmount = 0f;
				_largeGemAnimator.Play("Gem_ClassUp", 0, 0f);
				if (0 <= _smallGemIndex)
				{
					_gemPedestalControllers[_smallGemIndex].ClassUp();
				}
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_RESULT_GEM_FLASH_01, base.MonitorIndex);
			}
			_stateInfo = _largeGemAnimator;
			_stagingCount = 12u;
			_callback = delegate
			{
				if (_isBossAppearance)
				{
					_stagingCount = 99u;
				}
				else
				{
					_stagingCount = 4u;
					_classUpAnimator.Play("In", 0, 0f);
					_classUpGroup.alpha = 1f;
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_RESULT_CLASS_UP, base.MonitorIndex);
				}
				_callback = null;
			};
			break;
		case 4u:
			if (0 < _direction)
			{
				if (1f <= _classUpAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime)
				{
					_stagingCount = 5u;
					_classUpAnimator.Play("Out", 0, 0f);
					_largeGemAnimator.Play("Loop_Blank", 0, 0f);
					_gaugePopupAnimator.Play("Out_Get", 0, 0f);
					_gemSetAnimator.Play("Change", 0, 0f);
					ResetGemBoard(_prevClassID + _class);
					_classGemMedalObject.ClassUp();
				}
			}
			else if (_direction < 0 && 1f <= _classDownAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime)
			{
				_stagingCount = 5u;
				_largeGemAnimator.Play("Loop_Blank", 0, 0f);
				_largeGemBackImage.ChangeSprite((1 < (_prevClassID + _class).GetClassBoss()) ? 1 : 0);
				SetGemPopup(_prevClassID + _class, _criteria + _cp * (_gem + 1), isClassDown: true);
			}
			break;
		case 5u:
			if (0 < _direction)
			{
				if (!(1f <= _classUpAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime))
				{
					break;
				}
				_classUpGroup.alpha = 0f;
				SetClassUpDown(_prevClassID + _class, _prevClassID + _class + 1);
				_classGemMedalObject.Loop();
				int num4 = (_prevClassID + _class).GetBorder() / 10 - 2;
				_largeGemBackImage.ChangeSprite((1 < (_prevClassID + _class).GetClassBoss()) ? 1 : 0);
				if (0 <= num4)
				{
					if (0 <= _smallGemIndex)
					{
						_gemPedestalControllers[_smallGemIndex].Dispose();
					}
					_smallGemIndex = num4;
					_gemPedestalControllers[_smallGemIndex].GemGenerate(_smallGemObject);
				}
				SetGemPopup(_prevClassID + _class, _criteria + _cp * (_gem + 1));
				_gaugePopupAnimator.Play("Next", 0, 0f);
				if (0 < _classPointDirection)
				{
					_cp = 0;
					_stagingCount = 1u;
				}
				else
				{
					_stagingCount = 99u;
				}
			}
			else if (_direction < 0)
			{
				_gaugePopupAnimator.Play("Next", 0, 0f);
				if (_classPointDirection < 0)
				{
					_cp = 0;
					_stagingCount = 1u;
				}
				else
				{
					_stagingCount = 99u;
				}
			}
			break;
		case 7u:
			_stagingCount = 10u;
			_frameCounter = 30;
			_callback = delegate
			{
				_stagingCount = 8u;
				_callback = null;
			};
			break;
		case 8u:
			_class++;
			_criteria = 0;
			_fromCP = 0;
			_bossBgAnimator.Play("Idle", 0, 0f);
			_largeGemAnimator.Play("Gem_ClassUp");
			if (0 <= _smallGemIndex)
			{
				_gemPedestalControllers[_smallGemIndex].ClassUp();
			}
			_navigationCharacter.Play(NavigationAnime.FunStart, 0f);
			_upperGauge.fillAmount = 0f;
			SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_RESULT_BOSS_OUT, base.MonitorIndex);
			SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_RESULT_GEM_FLASH_01, base.MonitorIndex);
			_stagingCount = 12u;
			_stateInfo = _largeGemAnimator;
			_callback = delegate
			{
				_classUpAnimator.Play("In", 0, 0f);
				_classUpGroup.alpha = 1f;
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_RESULT_CLASS_UP, base.MonitorIndex);
				_stagingCount = 4u;
				_callback = null;
			};
			break;
		case 10u:
			if (0 < _frameCounter)
			{
				_frameCounter--;
			}
			else
			{
				_callback?.Invoke();
			}
			break;
		case 12u:
			if (1.0 < (double)_stateInfo.GetCurrentAnimatorStateInfo(0).normalizedTime)
			{
				_stateInfo = null;
				_callback?.Invoke();
			}
			break;
		case 99u:
			if (_isBossAppearance)
			{
				_state = StagingState.Boss;
			}
			else
			{
				_state = StagingState.End;
			}
			_stagingCount = 0u;
			break;
		}
	}

	private void DoBoss()
	{
		switch (_stagingCount)
		{
		case 0u:
			IsSkip = false;
			SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button04);
			_navigationCharacter.Play(NavigationAnime.FunStartLoop, 0f);
			_bossCutInAnimator.Play("In", 0, 0f);
			_bossBgAnimator.Play("In", 0, 0f);
			_largeGemAnimator.Play("Loop_Boss", 0, 0f);
			_gaugePopupAnimator.Play("Out_Next", 0, 0f);
			SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_RESULT_CHARA_SURPRISE, base.MonitorIndex);
			SoundManager.PlayJingle(Mai2.Mai2Cue.Cue.JINGLE_BOSS_ENCOUNT, base.MonitorIndex);
			_stateInfo = _bossCutInAnimator;
			_stagingCount = 12u;
			_callback = delegate
			{
				_stagingCount = 1u;
				_callback = null;
			};
			break;
		case 1u:
			_stagingCount++;
			break;
		case 2u:
			_stagingCount = 99u;
			break;
		case 12u:
		{
			if (1.0 < (double)_stateInfo.GetCurrentAnimatorStateInfo(0).normalizedTime)
			{
				_stateInfo = null;
				_callback?.Invoke();
				break;
			}
			AnimatorClipInfo[] currentAnimatorClipInfo = _bossCutInAnimator.GetCurrentAnimatorClipInfo(0);
			if ((int)(_stateInfo.GetCurrentAnimatorStateInfo(0).normalizedTime * (currentAnimatorClipInfo[0].clip.length * currentAnimatorClipInfo[0].clip.frameRate)) == 253)
			{
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_INFO_ATTENTION, base.MonitorIndex);
			}
			break;
		}
		case 99u:
			_state = StagingState.End;
			break;
		}
	}

	private void DoLegend()
	{
		switch (_stagingCount)
		{
		case 0u:
			_stagingCount = 1u;
			break;
		case 1u:
			_bossBgAnimator.Play("Idle", 0, 0f);
			_informationImage.gameObject.SetActive(value: false);
			_navigationCharacter.Play(NavigationAnime.FunStart, 0f);
			_largeGemAnimator.Play("Gem_ClassUp", 0, 0f);
			_gemPedestalControllers[_smallGemIndex].ClassUp();
			SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_RESULT_GEM_FLASH_01, base.MonitorIndex);
			_frameCounter = 0;
			_stagingCount = 2u;
			break;
		case 2u:
			if (1f <= _largeGemAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime)
			{
				if (30 < _frameCounter)
				{
					_classUpGroup.alpha = 1f;
					_classUpAnimator.Play("In", 0, 0f);
					_gemBaseAnimator.Play("Out", 0, 0f);
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_RESULT_CLASS_UP_LEGEND, base.MonitorIndex);
					_frameCounter = 0;
					_stagingCount = 3u;
				}
				else
				{
					_frameCounter++;
				}
			}
			break;
		case 3u:
			if (1f <= _classUpAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime)
			{
				_classUpAnimator.Play("Out", 0, 0f);
				_stagingCount = 4u;
			}
			break;
		case 4u:
			if (1f <= _classUpAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime)
			{
				_legendAnimator.Play("In", 0, 0f);
				_stagingCount = 5u;
			}
			break;
		case 5u:
			if (1f <= _legendAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime)
			{
				_legendAnimator.Play("Loop", 0, 0f);
				_stagingCount = 99u;
			}
			break;
		case 10u:
			_cp = 0;
			_frameCounter = 0;
			_stagingCount = ((_classPointDirection == 0) ? 99u : 11u);
			break;
		case 11u:
			if (_frameCounter == 0)
			{
				if (0 < _classPointDirection)
				{
					_legendCountAnimator.Play("CountUp", 0, 0f);
				}
				else if (_classPointDirection < 0)
				{
					_legendCountAnimator.Play("CountDown", 0, 0f);
				}
				_frameCounter++;
			}
			else if (15 == _frameCounter)
			{
				if (0 < _classPointDirection)
				{
					int num = _fromCP + _cp + 1;
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_RESULT_CP_UP, base.MonitorIndex);
					if (10 <= num && _fromCP + _cp < 10)
					{
						_legendSpriteCounter.AddFormatFream();
					}
					else if (100 <= num && _fromCP + _cp < 100)
					{
						_legendSpriteCounter.AddFormatFream();
					}
					else if (1000 <= num && _fromCP + _cp < 1000)
					{
						_legendSpriteCounter.AddFormatFream();
					}
					_cp++;
					_classPointDirection--;
					if (9999 < num)
					{
						num = 9999;
						_classPointDirection = 0;
					}
					if (_classPointDirection == 0)
					{
						_stagingCount = 99u;
					}
					_legendSpriteCounter.ChangeText(num.ToString());
				}
				else if (_classPointDirection < 0)
				{
					int num2 = _fromCP + (_cp - 1);
					if (num2 < 10 && 10 <= _fromCP + _cp)
					{
						_legendSpriteCounter.RemoveFormatFrame();
					}
					else if (num2 < 100 && 100 <= _fromCP + _cp)
					{
						_legendSpriteCounter.RemoveFormatFrame();
					}
					else if (num2 < 1000 && 1000 <= _fromCP + _cp)
					{
						_legendSpriteCounter.RemoveFormatFrame();
					}
					_cp--;
					_classPointDirection++;
					if (_classPointDirection == 0)
					{
						_stagingCount = 99u;
					}
					_legendSpriteCounter.ChangeText(num2.ToString());
				}
				else
				{
					_stagingCount = 99u;
				}
				_frameCounter++;
			}
			else if (1f <= _legendCountAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime)
			{
				_frameCounter = 0;
			}
			else
			{
				_frameCounter++;
			}
			break;
		case 99u:
			_stagingCount = 0u;
			_state = StagingState.End;
			break;
		}
	}

	private void SetClassPointText(string text)
	{
		_classPointCount.text = text;
	}

	public void SetUnlockInfo(Sprite jacket, string musicName)
	{
		if (_unlockWindowController != null)
		{
			_unlockWindowController.Set(jacket, musicName);
		}
	}

	public void PlayUnlockInfo()
	{
		if (_unlockWindowController != null)
		{
			_unlockWindowController.gameObject.SetActive(value: true);
			_unlockWindowController.SetInfoText(CommonMessageID.TransmissionMusic.GetName());
			_unlockWindowController.Play(null);
		}
		SoundManager.PlaySE(Mai2.Mai2Cue.Cue.JINGLE_MAP_GET, monitorIndex);
	}

	public void SkipUnlockInfo()
	{
		if (_unlockWindowController != null)
		{
			_unlockWindowController.Skip();
		}
	}

	public void SetVisibleButton(bool isVisible, InputManager.ButtonSetting button)
	{
		_buttonController.SetVisibleImmediate(isVisible, (int)button);
	}

	public void ChangeButtonSymbol(InputManager.ButtonSetting button, int index)
	{
		_buttonController.ChangeButtonSymbol(button, index);
	}

	public void PressedButton(InputManager.ButtonSetting button)
	{
		_buttonController.SetAnimationActive((int)button);
	}
}
