using System.Collections;
using DB;
using Mai2.Mai2Cue;
using Manager;
using Manager.UserDatas;
using Monitor.GhostResult.Controller;
using Process.TimelineSound;
using Timeline;
using UI;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace Monitor.GhostResult
{
	public class GhostResultMonitor : MonitorBase
	{
		private const int IntervalUnit = 360;

		private const double Skip01Time = 5.58;

		[SerializeField]
		private PlayableDirector _director;

		[SerializeField]
		private CanvasGroup _frontBlurObject;

		[SerializeField]
		private GhostResultButtonController _buttonController;

		[SerializeField]
		[Header("ユーザー情報")]
		private UserInformationController _leftPlayerInfomation;

		[SerializeField]
		private UserInformationController _rightPlayerInfomation;

		[SerializeField]
		[Header("レート増減分")]
		private SpriteCounter _rateDiffCounter;

		[SerializeField]
		[Header("カーソル称号")]
		private Image _rankImage;

		[SerializeField]
		[Header("カーソル称号")]
		private Image _prevRankImage;

		[SerializeField]
		[Header("表示管理用")]
		private GameObject _rankImageObject;

		[SerializeField]
		private GameObject _titleObject;

		[SerializeField]
		[Header("タイムラインコントロール系")]
		private TranslateControleObject _translateControle;

		[SerializeField]
		private SizeControleObject _sizeControle;

		[SerializeField]
		private TranslateControleObject _effectTranslateControle;

		[SerializeField]
		private EventTriggerControlObject _triggerControl;

		[SerializeField]
		[Header("Achevement")]
		private AchievementCounterObject _userAchievement;

		[SerializeField]
		private AchievementCounterObject _otomoAchievement;

		[SerializeField]
		[Header("RankText")]
		private Transform _rankParent;

		[SerializeField]
		[Header("BG")]
		private Image _nowEffect;

		[SerializeField]
		[Header("Timeline Attach")]
		private Animator _rankHudAnimator;

		[SerializeField]
		private TranslateControleObject _bossBannerControle;

		[SerializeField]
		private TranslateControleObject _bossParent;

		[SerializeField]
		private GameObject _runRankObject;

		[SerializeField]
		private InstantiateGenerator _bossInfoGenerator;

		[SerializeField]
		private InstantiateGenerator _musicGetInfoGenerator;

		private Image[] _rankTextImages;

		private UdemaeID _prevClassID;

		private UdemaeID _nextClassID;

		private bool _isVictory;

		private bool _isBossBattle;

		private bool _isBossEntry;

		private bool _isBossAppearance;

		private bool _isBossDeprivation;

		private bool _isBossSpecial;

		private bool _isPlaying;

		private bool _isRankUp;

		private bool _isRankDown;

		private int _skipCount;

		private int _gaugeCompareTo;

		private ExtraInfoController _bossInfoController;

		private UnlockMusicWindow _unlockWindowController;

		public bool IsSkip { get; private set; }

		private void Awake()
		{
			Main.alpha = 0f;
			_frontBlurObject.alpha = 0f;
		}

		public override void Initialize(int playerIndex, bool isActive)
		{
			base.Initialize(playerIndex, isActive);
			if (isActive)
			{
				_buttonController.Initialize(playerIndex);
				_buttonController.SetVisibleImmediate(false, 3);
				string path = "Process/GhostResult/Timelines/GhostResult_0" + (playerIndex + 1);
				_director.playableAsset = null;
				_director.playableAsset = Resources.Load<PlayableAsset>(path);
				_rankTextImages = new Image[_rankParent.childCount];
				for (int i = 0; i < _rankParent.childCount; i++)
				{
					Transform child = _rankParent.GetChild(i);
					_rankTextImages[i] = child.GetComponent<Image>();
				}
			}
		}

		public override void ViewUpdate()
		{
			if (_skipCount == 0 && _director.time >= 5.58)
			{
				_skipCount++;
			}
			if (_director.time >= _director.duration && _isPlaying)
			{
				_isPlaying = false;
				ReleaseTimeline();
			}
		}

		public void SetData(bool isVictory, int prevClassValue, UdemaeID prevClassID, int nextClassValue, UdemaeID nextClassID, int diff, bool isBossBattle, bool isBossEntry, bool isBossAppearance, bool isBossDeprivation, bool isBossSpecial)
		{
			_prevClassID = prevClassID;
			_nextClassID = nextClassID;
			_isRankUp = prevClassID < nextClassID;
			_isRankDown = prevClassID > nextClassID;
			_isBossBattle = isBossBattle;
			_isBossEntry = isBossEntry;
			_isBossDeprivation = isBossDeprivation;
			_isBossSpecial = isBossSpecial;
			if (_isBossSpecial)
			{
				_unlockWindowController = _musicGetInfoGenerator.Instantiate<UnlockMusicWindow>();
				_unlockWindowController.gameObject.SetActive(value: false);
			}
			_isBossAppearance = isBossAppearance;
			if (_isBossAppearance || _isBossBattle)
			{
				_bossInfoController = _bossInfoGenerator.Instantiate<ExtraInfoController>();
				_bossInfoController.Initialize();
			}
			string text = ((diff > 0) ? "+" : " ") + diff.ToString().PadRight(4);
			_rateDiffCounter.ChangeText(text);
			_prevRankImage.sprite = Resources.Load<Sprite>($"Common/Sprites/Dani/UI_CMN_MatchLevel_{prevClassID.GetEnumValue() + 1:00}");
			_rankImage.sprite = Resources.Load<Sprite>($"Common/Sprites/Dani/UI_CMN_MatchLevel_{nextClassID.GetEnumValue() + 1:00}");
			_isVictory = isVictory;
			int border = (prevClassID + 1).GetBorder();
			float t = Mathf.InverseLerp(prevClassID.GetBorder(), border, prevClassValue);
			int border2 = (nextClassID + 1).GetBorder();
			float t2 = Mathf.InverseLerp(nextClassID.GetBorder(), border2, nextClassValue);
			int num = (int)(prevClassID - 1);
			int num2 = 360 - num * 360;
			float b = num2 - 360;
			if (num2 > 360)
			{
				num2 = 360;
			}
			float y = Mathf.Lerp(num2, b, t);
			int num3 = 360 - (int)(nextClassID - 1) * 360;
			float y2 = Mathf.Lerp(num3, num3 - 360, t2);
			if (nextClassID == UdemaeID.Class_B5)
			{
				y2 = 360f;
			}
			Vector3 from = new Vector3(0f, y, 0f);
			Vector3 to = new Vector3(0f, y2, 0f);
			_translateControle.SetPosition(from, to);
			_translateControle.OnBehaviourPlay();
			_bossBannerControle.SetPosition(Vector3.zero, Vector3.zero);
			_bossBannerControle.OnBehaviourPlay();
			_bossParent.SetPosition(from, to);
			_bossParent.OnBehaviourPlay();
			int num4 = ((prevClassID == UdemaeID.Class_B5) ? 146 : 380);
			int num5 = ((prevClassID == UdemaeID.Class_B5) ? 146 : 0);
			int num6 = num4 + num * 360;
			if (num6 < 146)
			{
				num6 = 146;
			}
			float num7 = Mathf.Lerp(num6, num6 + (360 - num5), t);
			int num8 = (int)((nextClassID - 1 >= UdemaeID.Class_B5) ? (nextClassID - 1) : UdemaeID.Class_B5);
			int num9 = 380 + num8 * 360;
			if (nextClassID == UdemaeID.Class_B5)
			{
				num9 = 146;
			}
			if (nextClassID == UdemaeID.Class_B4)
			{
				num9 = 400;
			}
			float num10 = Mathf.Lerp(num9, num9 + 360, t2);
			if (nextClassID == UdemaeID.Class_B5)
			{
				num10 = Mathf.Lerp(num9, 380f, t2);
			}
			if (nextClassID == UdemaeID.Class_LEGEND)
			{
				num10 += 150f;
			}
			_sizeControle.SetSize(new Vector2(95f, num7), new Vector2(95f, num10));
			_sizeControle.OnBehaviourPlay();
			_triggerControl.TriggerAction = OnTrigger;
			int num11 = -964 + _prevClassID.GetEnumValue() * 360;
			int num12 = -964 + _nextClassID.GetEnumValue() * 360;
			_gaugeCompareTo = num10.CompareTo(num7);
			_effectTranslateControle.SetPosition(new Vector3(0f, num11, 0f), new Vector3(0f, num12, 0f));
			_effectTranslateControle.OnBehaviourPlay();
			double num13 = 0.0;
			double num14 = 0.0;
			foreach (TrackAsset outputTrack in ((TimelineAsset)_director.playableAsset).GetOutputTracks())
			{
				string text2 = outputTrack.name;
				if (!(text2 == "BG"))
				{
					continue;
				}
				foreach (TimelineClip clip in outputTrack.GetClips())
				{
					if (clip.displayName == "TimeControlAsset")
					{
						num13 = clip.start;
						num14 = clip.end;
						break;
					}
				}
			}
			float t3 = Mathf.InverseLerp(prevClassValue, nextClassValue, nextClassID.GetBorder());
			double start = Mathf.Lerp((float)num13, (float)num14, t3);
			foreach (TrackAsset outputTrack2 in ((TimelineAsset)_director.playableAsset).GetOutputTracks())
			{
				string text2 = outputTrack2.name;
				if (!(text2 == "Effect"))
				{
					continue;
				}
				foreach (TimelineClip clip2 in outputTrack2.GetClips())
				{
					if (clip2.displayName == "TimeControlAsset")
					{
						clip2.start = start;
					}
				}
			}
		}

		public void SetBossInfo(MessageUserInformationData info)
		{
			_bossInfoController.BossObj[0].SetUserData(info);
			_bossInfoController.BossObj[0].gameObject.SetActive(value: true);
			bool isSpecialBoss = UserUdemae.IsBossSpecial(info.ClassID);
			_bossInfoController.SetGhostActive(isActive: true, isBoss: true, isSpecialBoss);
		}

		public void SetUnlockInfo(Sprite jacket, string musicName)
		{
			_unlockWindowController?.Set(jacket, musicName);
		}

		public void PlayUnlockInfo()
		{
			_unlockWindowController?.gameObject.SetActive(value: true);
			_unlockWindowController?.SetInfoText(CommonMessageID.TransmissionMusic.GetName());
			_unlockWindowController?.Play(null);
			SoundManager.PlaySE(Cue.JINGLE_MAP_GET, monitorIndex);
		}

		public void SkipUnlockInfo()
		{
			_unlockWindowController?.Skip();
		}

		public void SetWait()
		{
			Main.alpha = 1f;
			_frontBlurObject.alpha = 1f;
			_sizeControle.gameObject.SetActive(value: false);
			_effectTranslateControle.gameObject.SetActive(value: false);
			_rankImageObject.SetActive(value: false);
			_titleObject.SetActive(value: false);
			_buttonController.SetVisibleImmediate(false, 3);
		}

		private void OnTrigger()
		{
			if (_isBossAppearance)
			{
				_runRankObject.SetActive(value: false);
			}
		}

		public void SetUserData(MessageUserInformationData playerData, MessageUserInformationData rivalData, uint userAchievement, uint otomoAchievement, UdemaeID userClassID, UdemaeID otomoClassID)
		{
			_leftPlayerInfomation.SetUserData(playerData);
			_rightPlayerInfomation.SetUserData(rivalData);
			_userAchievement.SetAchievement(0u, userAchievement);
			_userAchievement.OnClipTailEnd();
			_otomoAchievement.SetAchievement(0u, otomoAchievement);
			_otomoAchievement.OnClipTailEnd();
		}

		public void InitializeTimeline()
		{
			foreach (TrackAsset outputTrack in ((TimelineAsset)_director.playableAsset).GetOutputTracks())
			{
				switch (outputTrack.name)
				{
				case "VoiceTrack01":
					foreach (TimelineClip clip in outputTrack.GetClips())
					{
						if (clip.displayName == "WinOrLossVoiceClip")
						{
							VoicePlayableAsset obj3 = (VoicePlayableAsset)clip.asset;
							obj3.CueCode = "VO_000" + (_isVictory ? "142" : "143");
							obj3.MonitorIndex = base.MonitorIndex;
						}
					}
					break;
				case "VoiceTrack02":
					if (_isRankUp || _isRankDown)
					{
						outputTrack.muted = false;
						foreach (TimelineClip clip2 in outputTrack.GetClips())
						{
							if (clip2.displayName == "RankUpDownVoiceClip")
							{
								VoicePlayableAsset obj2 = (VoicePlayableAsset)clip2.asset;
								obj2.CueCode = "VO_000" + (_isRankUp ? "209" : (_isRankDown ? "210" : ""));
								obj2.MonitorIndex = base.MonitorIndex;
							}
						}
					}
					else
					{
						outputTrack.muted = true;
					}
					break;
				case "SETrack01":
					foreach (TimelineClip clip3 in outputTrack.GetClips())
					{
						if (clip3.displayName == "WinOrLossSEClip")
						{
							SePlayableAsset obj4 = (SePlayableAsset)clip3.asset;
							obj4.CueCode = (_isVictory ? "SE_RESULT_WIN" : "SE_RESULT_LOSE");
							obj4.MonitorIndex = base.MonitorIndex;
						}
						else if (clip3.displayName == "RankSEClip")
						{
							SePlayableAsset obj5 = (SePlayableAsset)clip3.asset;
							obj5.CueCode = (_isRankUp ? "SE_RESULT_RANKUP" : (_isRankDown ? "SE_RESULT_RANKDOWN" : ""));
							obj5.MonitorIndex = base.MonitorIndex;
						}
					}
					break;
				case "SETrack02":
					foreach (TimelineClip clip4 in outputTrack.GetClips())
					{
						if (clip4.displayName == "ResultGuageSEClip")
						{
							SeLoopPlayableAsset obj = (SeLoopPlayableAsset)clip4.asset;
							obj.CueCode = ((_gaugeCompareTo > 0) ? "SE_RESULT_GAUGE_UP" : ((_gaugeCompareTo < 0) ? "SE_RESULT_GAUGE_DOWN" : ""));
							obj.MonitorIndex = base.MonitorIndex;
						}
					}
					break;
				case "BossSETrack":
					outputTrack.muted = !_isBossBattle || (!_isVictory && !_isBossAppearance);
					break;
				case "WinTrack":
					outputTrack.muted = !_isVictory || _isBossBattle;
					break;
				case "LoseTrack":
					outputTrack.muted = _isVictory;
					break;
				case "BossWinTrack":
					outputTrack.muted = !_isVictory || !_isBossBattle;
					break;
				case "UserAnimationTrack":
					foreach (TrackAsset childTrack in outputTrack.GetChildTracks())
					{
						string text = childTrack.name;
						if (!(text == "1P_Win"))
						{
							if (text == "2P_Win")
							{
								if (monitorIndex == 0)
								{
									childTrack.muted = _isVictory;
								}
								else
								{
									childTrack.muted = !_isVictory;
								}
							}
						}
						else if (monitorIndex == 0)
						{
							childTrack.muted = !_isVictory;
						}
						else
						{
							childTrack.muted = _isVictory;
						}
					}
					break;
				case "WindowAnimationTrack":
					foreach (TrackAsset childTrack2 in outputTrack.GetChildTracks())
					{
						string text = childTrack2.name;
						if (!(text == "Win"))
						{
							if (text == "Defeat")
							{
								childTrack2.muted = _isVictory;
							}
						}
						else
						{
							childTrack2.muted = !_isVictory;
						}
					}
					break;
				case "RankTrack":
					foreach (TrackAsset childTrack3 in outputTrack.GetChildTracks())
					{
						string text = childTrack3.name;
						if (!(text == "RankUp"))
						{
							if (text == "RankDown")
							{
								childTrack3.muted = !_isRankDown;
							}
						}
						else
						{
							childTrack3.muted = !_isRankUp;
						}
					}
					break;
				case "RankHUDTrack":
					foreach (TrackAsset childTrack4 in outputTrack.GetChildTracks())
					{
						string text = childTrack4.name;
						if (!(text == "HUD_RankUp"))
						{
							if (text == "HUD_RankDown")
							{
								childTrack4.muted = !_isRankDown;
							}
						}
						else
						{
							childTrack4.muted = !_isRankUp;
						}
					}
					break;
				case "BossCutInTrack":
					foreach (TrackAsset childTrack5 in outputTrack.GetChildTracks())
					{
						switch (childTrack5.name)
						{
						case "BossBannerLoop":
							childTrack5.muted = !_isBossEntry;
							break;
						case "BossDefeat":
							childTrack5.muted = !_isVictory || !_isBossBattle;
							break;
						case "BossPlayerDefeat":
							childTrack5.muted = !_isBossDeprivation;
							break;
						case "BossIn":
							childTrack5.muted = !_isBossAppearance;
							break;
						}
					}
					break;
				case "BossEncountJingle":
					outputTrack.muted = !_isBossAppearance;
					break;
				}
			}
		}

		public void Play()
		{
			Main.alpha = 1f;
			BindTimeline();
			IsSkip = true;
			_director.Play();
			_isPlaying = true;
		}

		public void SkipPossible()
		{
			_buttonController.SetVisible(true, 3);
		}

		public void Skip()
		{
			double time = _director.time;
			if (_isBossAppearance)
			{
				if (time <= 2.63)
				{
					_director.time = 2.63;
					_director.Evaluate();
				}
				else if (time <= 8.1)
				{
					_director.time = 8.1;
					_director.Evaluate();
				}
				else if (time <= 12.64)
				{
					_director.time = 12.64;
					_director.Evaluate();
					SkipEnd();
				}
			}
			else if (_isRankUp || _isRankDown)
			{
				if (time <= 2.63)
				{
					_director.time = 2.63;
					_director.Evaluate();
				}
				else if (time <= 7.5)
				{
					_director.time = 7.5;
					_director.Evaluate();
					SkipEnd();
				}
			}
			else if (time <= 5.58)
			{
				_director.time = 5.58;
				_director.Evaluate();
			}
			else if (time <= _director.duration)
			{
				_director.time = _director.duration;
				_director.Evaluate();
				OnTrigger();
				_sizeControle.OnGraphStop();
				_translateControle.OnGraphStop();
				_bossParent.OnGraphStop();
				_bossBannerControle.OnGraphStop();
				_effectTranslateControle.OnGraphStop();
				SkipEnd();
			}
		}

		private void SkipEnd()
		{
			IsSkip = false;
		}

		public void ResetTimeline()
		{
			_skipCount = 0;
			_director.time = 0.0;
			_director.Evaluate();
		}

		public bool IsEnd()
		{
			return _director.time >= _director.duration;
		}

		public void PreseedButton(InputManager.ButtonSetting button)
		{
			_buttonController.SetAnimationActive((int)button);
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

		private void BindTimeline()
		{
			foreach (PlayableBinding output in _director.playableAsset.outputs)
			{
				string streamName = output.streamName;
				if (streamName == "RankHUDTrack")
				{
					_director.SetGenericBinding(output.sourceObject, _rankHudAnimator);
				}
			}
		}

		private void ReleaseTimeline()
		{
			foreach (PlayableBinding output in _director.playableAsset.outputs)
			{
				string streamName = output.streamName;
				if (streamName == "RankHUDTrack")
				{
					_director.SetGenericBinding(output.sourceObject, null);
				}
			}
		}

		public void SetVisibleFrontBlur(bool isVisible)
		{
			_frontBlurObject.alpha = (isVisible ? 1f : 0f);
		}

		public void CleanUp()
		{
			StartCoroutine(CleanUpCoroutine());
		}

		private IEnumerator CleanUpCoroutine()
		{
			yield return new WaitWhile(delegate
			{
				Main.alpha -= 0.1f;
				return Main.alpha > 0f;
			});
		}
	}
}
