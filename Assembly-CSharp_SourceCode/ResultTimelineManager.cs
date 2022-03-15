using System;
using System.Collections.Generic;
using DB;
using Manager;
using Process.TimelineSound;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class ResultTimelineManager : MonoBehaviour
{
	private class BindKeyObject
	{
		public TrackAsset Asset;

		public UnityEngine.Object Key;

		public UnityEngine.Object Baggage;

		public bool IsRelease(double time)
		{
			if (time >= Asset?.end)
			{
				return Key != null;
			}
			return false;
		}

		public void Clear()
		{
			Key = null;
		}

		public void Release()
		{
			Key = null;
			Asset = null;
		}
	}

	[SerializeField]
	[Header("TimelineDirector")]
	private PlayableDirector _director;

	[SerializeField]
	[Header("クリアランクアニメーションクリップ")]
	private AnimationClip[] _clearRankFadeInClips;

	[SerializeField]
	[Header("クリアランク別バックグラウンドアニメーションクリップ")]
	private AnimationClip[] _backgroundFadeInAnimationClip;

	[SerializeField]
	private AnimationClip[] _backgroundLoopAnimationClip;

	[SerializeField]
	[Header("難易度別アニメーションクリップ")]
	private AnimationClip[] _musicDataDifficultyAnimationClips;

	[SerializeField]
	[Header("コンボメダル")]
	private AnimationClip[] _comboMedalAnimationClips;

	[SerializeField]
	private AnimationClip[] _comboMedalLoopAnimationClips;

	[SerializeField]
	[Header("シンクロメダル")]
	private AnimationClip[] _syncMedalAnimationClips;

	[SerializeField]
	private AnimationClip[] _syncMedalLoopAnimationClips;

	[SerializeField]
	[Header("サインボード\u3000看板")]
	private AnimationClip[] _singboardCombo;

	[SerializeField]
	private AnimationClip[] _singboardSync;

	[SerializeField]
	[Header("複数人プレイ結果アニメーションクリップ")]
	private AnimationClip[] _multiResultAnimationClip;

	[SerializeField]
	private AnimationClip[] _dxRatingAnimationClips;

	[SerializeField]
	[Header("でらっくすすたー")]
	private Animator _dxStarAnimator;

	private NavigationCharacter _partnerCharacter;

	private int _playerIndex;

	private int _multiPlayIndex;

	private int _difficulty;

	private int _dxRateIndex;

	private bool _isClear;

	private bool _isNewRecord;

	private bool _isPlaying;

	private bool _isSkippable;

	private bool _isSinglePlay;

	private bool _isStartShowDetails;

	private double _naviFunStartEndTime;

	private BindKeyObject _backgroundKey;

	private BindKeyObject _partner01Key;

	private BindKeyObject _partner02Key;

	private BindKeyObject _clearRankKey;

	private BindKeyObject _scoreBoardKey;

	private BindKeyObject _simpleBoardKey;

	private BindKeyObject _comboMedalKey;

	private BindKeyObject _syncMedalKey;

	private BindKeyObject[] _multiResultKeys;

	private MusicClearrankID _clearRank;

	private DeluxcorerankrateID _dxRank;

	private PlayComboflagID _comboMedal;

	private PlaySyncflagID _syncMedal;

	private Animator _rankAnimator;

	private Animator[] _multiPlayUserAnimators;

	private bool _isDXRatingUP;

	private bool _isDXColorChange;

	private bool _isChallengeClear;

	private readonly List<UnityEngine.Object> _bindKeyList = new List<UnityEngine.Object>();

	private Action _onChange;

	private bool _isExecuteChangeAction;

	private double _dxRatingUPChangeTime;

	private bool _isDXRatingUPSkipable;

	public void Initialize(int playerIndex, PlayableAsset playableAsset)
	{
		_playerIndex = playerIndex;
		_director.playableAsset = playableAsset;
		_partner01Key = new BindKeyObject();
		_partner02Key = new BindKeyObject();
		_scoreBoardKey = new BindKeyObject();
		_simpleBoardKey = new BindKeyObject();
		_clearRankKey = new BindKeyObject();
		_backgroundKey = new BindKeyObject();
		_comboMedalKey = new BindKeyObject();
		_syncMedalKey = new BindKeyObject();
		_multiResultKeys = new BindKeyObject[4];
		for (int i = 0; i < 4; i++)
		{
			_multiResultKeys[i] = new BindKeyObject();
		}
	}

	public void SetAnimator(Animator rankAnimator)
	{
		_rankAnimator = rankAnimator;
	}

	public void SetData(bool isNewRecord, bool isClear, int difficulty, NavigationCharacter navigation, MusicClearrankID clearRank)
	{
		_isNewRecord = isNewRecord;
		_isClear = isClear;
		_partnerCharacter = navigation;
		_clearRank = clearRank;
		_difficulty = difficulty;
	}

	public void SetPerfectChallengeClear(bool isClear)
	{
		_isChallengeClear = isClear;
	}

	public void SettingComplete(PlayComboflagID comboMedal, PlaySyncflagID syncMedal, DeluxcorerankrateID dxRank, bool isSinglePlay, int dxRatingDirection, int multiIndex, bool isDetails, Animator[] multiUserAnimators)
	{
		_comboMedal = comboMedal;
		_syncMedal = syncMedal;
		_dxRank = dxRank;
		_isSinglePlay = isSinglePlay;
		_multiPlayIndex = multiIndex;
		_multiPlayUserAnimators = multiUserAnimators;
		_isStartShowDetails = isDetails;
		_dxRateIndex = dxRatingDirection;
	}

	public void SetDXRatingUP(bool isUP, bool isColorChange, Action onChange)
	{
		_isDXRatingUP = isUP;
		_isDXColorChange = isColorChange;
		_onChange = onChange;
	}

	public void ViewUpdate()
	{
		if (!_isPlaying)
		{
			return;
		}
		if (!_isSkippable)
		{
			BindLeavingUpdate();
			if (_director.time >= _dxRatingUPChangeTime && !_isExecuteChangeAction && _onChange != null)
			{
				_onChange();
				_isExecuteChangeAction = true;
			}
		}
		if (_director.time >= _director.duration)
		{
			_isPlaying = false;
		}
	}

	public void Play()
	{
		_isPlaying = true;
		_director.RebuildGraph();
		_director.Play();
	}

	public bool IsDone()
	{
		return _director.time / _director.duration >= 1.0;
	}

	public void Skip()
	{
		_isSkippable = true;
		_director.time = _director.duration;
		BindLeavingUpdate();
		_director.Evaluate();
	}

	public void SkipRatingUP()
	{
		if (!_isDXRatingUPSkipable)
		{
			_director.time = 5.0;
			_director.Evaluate();
			_isDXRatingUPSkipable = true;
		}
	}

	public bool IsSkippable()
	{
		if (_isDXRatingUP)
		{
			if (_director.time >= 3.0)
			{
				return _director.time <= 5.0;
			}
			return false;
		}
		return _director.time >= 3.0;
	}

	public void ResetTimeline()
	{
		BindTimeline();
		_director.time = 0.0;
		_director.Stop();
		_director.Evaluate();
		_isDXRatingUPSkipable = false;
		_isSkippable = false;
	}

	public void Interpretation()
	{
		BindTimeline();
		foreach (TrackAsset outputTrack in ((TimelineAsset)_director.playableAsset).GetOutputTracks())
		{
			InitSoundTrack(outputTrack);
			switch (outputTrack.name)
			{
			case "BackgroundTrack":
			{
				_backgroundKey.Asset = outputTrack;
				int num8;
				switch (_clearRank)
				{
				case MusicClearrankID.Rank_SSS:
				case MusicClearrankID.Rank_SSSP:
					num8 = 4;
					break;
				case MusicClearrankID.Rank_SS:
				case MusicClearrankID.Rank_SSP:
					num8 = 3;
					break;
				case MusicClearrankID.Rank_S:
				case MusicClearrankID.Rank_SP:
					num8 = 2;
					break;
				case MusicClearrankID.Rank_A:
				case MusicClearrankID.Rank_AA:
				case MusicClearrankID.Rank_AAA:
					num8 = 1;
					break;
				default:
					num8 = 0;
					break;
				}
				foreach (TimelineClip clip in outputTrack.GetClips())
				{
					if (clip.displayName == "FadeInClip")
					{
						((AnimationPlayableAsset)clip.asset).clip = _backgroundFadeInAnimationClip[num8];
					}
					else if (clip.displayName == "LoopClip")
					{
						((AnimationPlayableAsset)clip.asset).clip = _backgroundLoopAnimationClip[num8];
					}
				}
				break;
			}
			case "MusicDataTrack":
				foreach (TrackAsset childTrack in outputTrack.GetChildTracks())
				{
					if (!(childTrack.name == "DifficultyLayerTrack"))
					{
						continue;
					}
					foreach (TimelineClip clip2 in childTrack.GetClips())
					{
						if (clip2.displayName == "DifficultyClip")
						{
							((AnimationPlayableAsset)clip2.asset).clip = _musicDataDifficultyAnimationClips[_difficulty];
							break;
						}
					}
					break;
				}
				break;
			case "NavigationCharacterTrack01":
			case "NavigationCharacterTrack02":
				foreach (TimelineClip clip3 in outputTrack.GetClips())
				{
					if (clip3.displayName == "DefaultClip")
					{
						AnimationPlayableAsset animationPlayableAsset7 = (AnimationPlayableAsset)clip3.asset;
						clip3.duration = _partnerCharacter.Default.length;
						animationPlayableAsset7.clip = _partnerCharacter.Default;
					}
				}
				foreach (TrackAsset childTrack2 in outputTrack.GetChildTracks())
				{
					string text2 = childTrack2.name;
					if (!(text2 == "FunTrack"))
					{
						if (!(text2 == "SadTrack"))
						{
							continue;
						}
						childTrack2.muted = _isClear;
						foreach (TimelineClip clip4 in childTrack2.GetClips())
						{
							if (clip4.displayName == "SadClip")
							{
								AnimationPlayableAsset animationPlayableAsset8 = (AnimationPlayableAsset)clip4.asset;
								clip4.duration = _partnerCharacter.Sad.length;
								animationPlayableAsset8.clip = _partnerCharacter.Sad;
								break;
							}
						}
						if (!_isClear)
						{
							if (outputTrack.name == "NavigationCharacterTrack01")
							{
								_partner01Key.Asset = childTrack2;
							}
							else
							{
								_partner02Key.Asset = childTrack2;
							}
						}
						continue;
					}
					childTrack2.muted = !_isClear;
					foreach (TimelineClip clip5 in childTrack2.GetClips())
					{
						text2 = clip5.displayName;
						if (!(text2 == "FunStartClip"))
						{
							if (text2 == "LoopClip")
							{
								AnimationPlayableAsset obj5 = (AnimationPlayableAsset)clip5.asset;
								clip5.start = _naviFunStartEndTime;
								clip5.duration = _partnerCharacter.FunLoop.length;
								obj5.clip = _partnerCharacter.FunLoop;
							}
						}
						else
						{
							AnimationPlayableAsset obj6 = (AnimationPlayableAsset)clip5.asset;
							clip5.duration = _partnerCharacter.FunStart.length;
							obj6.clip = _partnerCharacter.FunStart;
							_naviFunStartEndTime = clip5.end;
						}
					}
					if (_isClear)
					{
						if (outputTrack.name == "NavigationCharacterTrack01")
						{
							_partner01Key.Asset = childTrack2;
						}
						else
						{
							_partner02Key.Asset = childTrack2;
						}
					}
				}
				break;
			case "ClearRankTrack":
				_clearRankKey.Asset = outputTrack;
				foreach (TimelineClip clip6 in outputTrack.GetClips())
				{
					if (clip6.displayName == "FadeIn")
					{
						int num7;
						switch (_clearRank)
						{
						case MusicClearrankID.Rank_SSS:
						case MusicClearrankID.Rank_SSSP:
							num7 = 10;
							break;
						case MusicClearrankID.Rank_SS:
						case MusicClearrankID.Rank_SSP:
							num7 = 9;
							break;
						case MusicClearrankID.Rank_SP:
							num7 = 8;
							break;
						case MusicClearrankID.Rank_S:
							num7 = 7;
							break;
						case MusicClearrankID.Rank_AAA:
							num7 = 6;
							break;
						case MusicClearrankID.Rank_AA:
							num7 = 5;
							break;
						case MusicClearrankID.Rank_A:
							num7 = 4;
							break;
						case MusicClearrankID.Rank_BBB:
							num7 = 3;
							break;
						case MusicClearrankID.Rank_BB:
							num7 = 2;
							break;
						case MusicClearrankID.Rank_B:
							num7 = 1;
							break;
						default:
							num7 = 0;
							break;
						}
						AnimationPlayableAsset animationPlayableAsset6 = (AnimationPlayableAsset)clip6.asset;
						clip6.duration = _clearRankFadeInClips[num7].length;
						animationPlayableAsset6.clip = _clearRankFadeInClips[num7];
					}
				}
				break;
			case "ComboMedalTrack":
				foreach (TrackAsset childTrack3 in outputTrack.GetChildTracks())
				{
					if (!(childTrack3.name == "GetMedalTrack"))
					{
						continue;
					}
					if (_comboMedal == PlayComboflagID.None)
					{
						_comboMedalKey.Asset = outputTrack;
						childTrack3.muted = true;
						break;
					}
					_comboMedalKey.Asset = childTrack3;
					childTrack3.muted = false;
					foreach (TimelineClip clip7 in childTrack3.GetClips())
					{
						if (clip7.displayName == "InClip")
						{
							AnimationPlayableAsset animationPlayableAsset4 = (AnimationPlayableAsset)clip7.asset;
							clip7.duration = _comboMedalAnimationClips[(int)_comboMedal].length;
							animationPlayableAsset4.clip = _comboMedalAnimationClips[(int)_comboMedal];
						}
						else if (clip7.displayName == "LoopClip")
						{
							int num6 = ((_comboMedal >= PlayComboflagID.AllPerfect) ? 1 : 0);
							AnimationPlayableAsset animationPlayableAsset5 = (AnimationPlayableAsset)clip7.asset;
							clip7.duration = _comboMedalLoopAnimationClips[num6].length;
							animationPlayableAsset5.clip = _comboMedalLoopAnimationClips[num6];
						}
					}
					break;
				}
				break;
			case "SyncMedalTrack":
				foreach (TrackAsset childTrack4 in outputTrack.GetChildTracks())
				{
					if (!(childTrack4.name == "GetMedalTrack"))
					{
						continue;
					}
					if (_syncMedal == PlaySyncflagID.None)
					{
						_syncMedalKey.Asset = outputTrack;
						childTrack4.muted = true;
						break;
					}
					_syncMedalKey.Asset = childTrack4;
					childTrack4.muted = false;
					foreach (TimelineClip clip8 in childTrack4.GetClips())
					{
						if (clip8.displayName == "InClip")
						{
							AnimationPlayableAsset animationPlayableAsset2 = (AnimationPlayableAsset)clip8.asset;
							clip8.duration = _syncMedalAnimationClips[(int)_syncMedal].length;
							animationPlayableAsset2.clip = _syncMedalAnimationClips[(int)_syncMedal];
						}
						else if (clip8.displayName == "LoopClip")
						{
							int num5 = ((_syncMedal >= PlaySyncflagID.SyncLow) ? 1 : 0);
							AnimationPlayableAsset animationPlayableAsset3 = (AnimationPlayableAsset)clip8.asset;
							clip8.duration = _syncMedalLoopAnimationClips[num5].length;
							animationPlayableAsset3.clip = _syncMedalLoopAnimationClips[num5];
						}
					}
					break;
				}
				break;
			case "ScoreDetailsTrack":
				outputTrack.muted = !_isStartShowDetails;
				foreach (TrackAsset childTrack5 in outputTrack.GetChildTracks())
				{
					if (childTrack5.name == "DisabledTrack")
					{
						childTrack5.muted = _isStartShowDetails;
						break;
					}
				}
				break;
			case "DXScore_Star":
			{
				string text = $"Star_{(int)_dxRank:00}";
				foreach (TrackAsset childTrack6 in outputTrack.GetChildTracks())
				{
					childTrack6.muted = childTrack6.name != text;
				}
				break;
			}
			case "ScoreSimpleTrack":
				outputTrack.muted = _isStartShowDetails;
				foreach (TrackAsset childTrack7 in outputTrack.GetChildTracks())
				{
					if (childTrack7.name == "DisabledTrack")
					{
						childTrack7.muted = !_isStartShowDetails;
						break;
					}
				}
				break;
			case "DXRatingTrack":
				foreach (TrackAsset childTrack8 in outputTrack.GetChildTracks())
				{
					if (childTrack8.name == "DirectionTrack")
					{
						childTrack8.muted = _isDXColorChange;
						foreach (TimelineClip clip9 in childTrack8.GetClips())
						{
							if (clip9.displayName == "DirectionClip")
							{
								AnimationPlayableAsset animationPlayableAsset = (AnimationPlayableAsset)clip9.asset;
								clip9.duration = _dxRatingAnimationClips[_dxRateIndex].length;
								animationPlayableAsset.clip = _dxRatingAnimationClips[_dxRateIndex];
							}
						}
					}
					else if (childTrack8.name == "DirectionChangeTrack")
					{
						childTrack8.muted = !_isDXRatingUP || !_isDXColorChange;
					}
				}
				break;
			case "DXRatingUPDirectingTrack":
				outputTrack.muted = !_isDXRatingUP;
				foreach (TrackAsset childTrack9 in outputTrack.GetChildTracks())
				{
					if (childTrack9.name == "RatingUPTrack")
					{
						childTrack9.muted = !_isDXRatingUP || _isDXColorChange;
						if (!childTrack9.muted)
						{
							_dxRatingUPChangeTime = childTrack9.end;
						}
					}
					else if (childTrack9.name == "ColorChangeTrack")
					{
						childTrack9.muted = !_isDXRatingUP || !_isDXColorChange;
						if (!childTrack9.muted)
						{
							_dxRatingUPChangeTime = childTrack9.end;
						}
					}
				}
				break;
			case "Rank1stTrack":
				_multiResultKeys[0].Asset = outputTrack;
				foreach (TimelineClip clip10 in outputTrack.GetClips())
				{
					if (clip10.displayName == "InClip")
					{
						AnimationPlayableAsset obj4 = (AnimationPlayableAsset)clip10.asset;
						int num4 = ((_multiPlayIndex == 0) ? 1 : 0);
						clip10.duration = _multiResultAnimationClip[num4].length;
						obj4.clip = _multiResultAnimationClip[num4];
					}
				}
				break;
			case "Rank2ndTrack":
				_multiResultKeys[1].Asset = outputTrack;
				foreach (TimelineClip clip11 in outputTrack.GetClips())
				{
					if (clip11.displayName == "InClip")
					{
						AnimationPlayableAsset obj3 = (AnimationPlayableAsset)clip11.asset;
						int num3 = ((_multiPlayIndex == 1) ? 1 : 0);
						clip11.duration = _multiResultAnimationClip[num3].length;
						obj3.clip = _multiResultAnimationClip[num3];
					}
				}
				break;
			case "Rank3rdTrack":
				_multiResultKeys[2].Asset = outputTrack;
				foreach (TimelineClip clip12 in outputTrack.GetClips())
				{
					if (clip12.displayName == "InClip")
					{
						AnimationPlayableAsset obj2 = (AnimationPlayableAsset)clip12.asset;
						int num2 = ((_multiPlayIndex == 2) ? 1 : 0);
						clip12.duration = _multiResultAnimationClip[num2].length;
						obj2.clip = _multiResultAnimationClip[num2];
					}
				}
				break;
			case "Rank4thTrack":
				_multiResultKeys[3].Asset = outputTrack;
				foreach (TimelineClip clip13 in outputTrack.GetClips())
				{
					if (clip13.displayName == "InClip")
					{
						AnimationPlayableAsset obj = (AnimationPlayableAsset)clip13.asset;
						int num = ((_multiPlayIndex == 3) ? 1 : 0);
						clip13.duration = _multiResultAnimationClip[num].length;
						obj.clip = _multiResultAnimationClip[num];
					}
				}
				break;
			case "PerfectChallengeTrack":
				foreach (TrackAsset childTrack10 in outputTrack.GetChildTracks())
				{
					if (childTrack10.name == "Clear")
					{
						childTrack10.muted = !_isChallengeClear;
					}
				}
				break;
			}
		}
	}

	private void InitSoundTrack(TrackAsset track)
	{
		switch (track.name)
		{
		case "RankVoiceTrack":
		{
			foreach (TimelineClip clip in track.GetClips())
			{
				if (clip.displayName == "RankVoiceClip")
				{
					track.muted = false;
					string cueCode;
					switch (_clearRank)
					{
					case MusicClearrankID.Rank_A:
						cueCode = "VO_000131";
						break;
					case MusicClearrankID.Rank_AA:
						cueCode = "VO_000130";
						break;
					case MusicClearrankID.Rank_AAA:
						cueCode = "VO_000129";
						break;
					case MusicClearrankID.Rank_S:
						cueCode = "VO_000128";
						break;
					case MusicClearrankID.Rank_SP:
						cueCode = "VO_000127";
						break;
					case MusicClearrankID.Rank_SS:
						cueCode = "VO_000126";
						break;
					case MusicClearrankID.Rank_SSP:
						cueCode = "VO_000125";
						break;
					case MusicClearrankID.Rank_SSS:
						cueCode = "VO_000124";
						break;
					case MusicClearrankID.Rank_SSSP:
						cueCode = "VO_000123";
						break;
					default:
						cueCode = "VO_000222";
						track.muted = true;
						break;
					}
					VoicePlayableAsset obj2 = (VoicePlayableAsset)clip.asset;
					obj2.MonitorIndex = _playerIndex;
					obj2.CueCode = cueCode;
				}
				else if (clip.displayName == "RankExtraVoiceClip")
				{
					string cueCode2 = "";
					switch (_clearRank)
					{
					case MusicClearrankID.Rank_D:
					case MusicClearrankID.Rank_C:
					case MusicClearrankID.Rank_B:
					case MusicClearrankID.Rank_BB:
					case MusicClearrankID.Rank_BBB:
						cueCode2 = "VO_000222";
						break;
					case MusicClearrankID.Rank_A:
						cueCode2 = "VO_000220";
						break;
					case MusicClearrankID.Rank_AA:
						cueCode2 = "VO_000220";
						break;
					case MusicClearrankID.Rank_AAA:
						cueCode2 = "VO_000220";
						break;
					case MusicClearrankID.Rank_S:
						cueCode2 = "VO_000217";
						break;
					case MusicClearrankID.Rank_SP:
						cueCode2 = "VO_000217";
						break;
					case MusicClearrankID.Rank_SS:
						cueCode2 = "VO_000214";
						break;
					case MusicClearrankID.Rank_SSP:
						cueCode2 = "VO_000214";
						break;
					case MusicClearrankID.Rank_SSS:
						cueCode2 = "VO_000213";
						break;
					case MusicClearrankID.Rank_SSSP:
						cueCode2 = "VO_000211";
						break;
					}
					VoicePlayableAsset obj3 = (VoicePlayableAsset)clip.asset;
					obj3.MonitorIndex = _playerIndex;
					obj3.CueCode = cueCode2;
				}
			}
			break;
		}
		case "RankSETrack":
		{
			foreach (TimelineClip clip2 in track.GetClips())
			{
				if (clip2.displayName == "RankSEClip")
				{
					SePlayableAsset sePlayableAsset = (SePlayableAsset)clip2.asset;
					sePlayableAsset.MonitorIndex = _playerIndex;
					string text;
					switch (_clearRank)
					{
					case MusicClearrankID.Rank_B:
					case MusicClearrankID.Rank_BB:
					case MusicClearrankID.Rank_BBB:
						text = "B";
						break;
					case MusicClearrankID.Rank_A:
					case MusicClearrankID.Rank_AA:
					case MusicClearrankID.Rank_AAA:
						text = "A";
						break;
					case MusicClearrankID.Rank_S:
					case MusicClearrankID.Rank_SP:
						text = "S";
						break;
					case MusicClearrankID.Rank_SS:
					case MusicClearrankID.Rank_SSP:
						text = "SS";
						break;
					case MusicClearrankID.Rank_SSS:
					case MusicClearrankID.Rank_SSSP:
						text = "SSS";
						break;
					default:
						text = "B";
						break;
					}
					sePlayableAsset.CueCode = "SE_RESULT_" + text;
				}
			}
			break;
		}
		case "NewRecordVoiceTrack":
			track.muted = !_isNewRecord;
			{
				foreach (TimelineClip clip3 in track.GetClips())
				{
					if (clip3.displayName == "VO_000139")
					{
						((VoicePlayableAsset)clip3.asset).MonitorIndex = _playerIndex;
					}
				}
				break;
			}
		case "ClearSETrack":
			track.muted = !_isClear;
			{
				foreach (TimelineClip clip4 in track.GetClips())
				{
					if (clip4.displayName == "SE_RESULT_CLEAR")
					{
						((SePlayableAsset)clip4.asset).MonitorIndex = _playerIndex;
					}
				}
				break;
			}
		case "MedalSETrack":
		{
			foreach (TimelineClip clip5 in track.GetClips())
			{
				if (clip5.displayName == "MedalSEClip")
				{
					SePlayableAsset obj = (SePlayableAsset)clip5.asset;
					obj.MonitorIndex = _playerIndex;
					obj.CueCode = GetMedalSeCueCode();
				}
			}
			break;
		}
		case "RatingVoiceTrack":
			track.muted = !_isDXRatingUP;
			{
				foreach (TimelineClip clip6 in track.GetClips())
				{
					if (clip6.displayName == "RatingUpVoiceClip")
					{
						((VoicePlayableAsset)clip6.asset).MonitorIndex = _playerIndex;
					}
				}
				break;
			}
		case "RatingSETrack":
			track.muted = 0 < _dxRateIndex;
			{
				foreach (TimelineClip clip7 in track.GetClips())
				{
					if (clip7.displayName == "RatingSEClip")
					{
						((SePlayableAsset)clip7.asset).MonitorIndex = _playerIndex;
					}
				}
				break;
			}
		case "ColorChangeSETrack":
			track.muted = !_isDXColorChange;
			{
				foreach (TimelineClip clip8 in track.GetClips())
				{
					if (clip8.displayName == "ColorChangeSEClip")
					{
						((SePlayableAsset)clip8.asset).MonitorIndex = _playerIndex;
					}
				}
				break;
			}
		}
	}

	private void BindTimeline()
	{
		foreach (PlayableBinding output in _director.playableAsset.outputs)
		{
			switch (output.streamName)
			{
			case "BackgroundTrack":
				_backgroundKey.Key = output.sourceObject;
				if (_backgroundKey.Baggage == null)
				{
					_backgroundKey.Baggage = _director.GetGenericBinding(output.sourceObject);
				}
				else
				{
					_director.SetGenericBinding(output.sourceObject, _backgroundKey.Baggage);
				}
				break;
			case "ClearRankTrack":
				_clearRankKey.Key = output.sourceObject;
				_clearRankKey.Baggage = _rankAnimator;
				_bindKeyList.Add(output.sourceObject);
				_director.SetGenericBinding(output.sourceObject, _rankAnimator);
				break;
			case "DXScore_Star":
				_bindKeyList.Add(output.sourceObject);
				_director.SetGenericBinding(output.sourceObject, _dxStarAnimator);
				break;
			case "NavigationCharacterTrack01":
				if (_partnerCharacter.NaviAnimator[0] != null)
				{
					_partner01Key.Key = output.sourceObject;
					_director.SetGenericBinding(output.sourceObject, _partnerCharacter.NaviAnimator[0]);
				}
				break;
			case "NavigationCharacterTrack02":
				if (_partnerCharacter.NaviAnimator.Length > 1 && _partnerCharacter.NaviAnimator[1] != null)
				{
					_partner02Key.Key = output.sourceObject;
					_director.SetGenericBinding(output.sourceObject, _partnerCharacter.NaviAnimator[1]);
				}
				break;
			case "ScoreDetailsTrack":
				_scoreBoardKey.Key = output.sourceObject;
				if (_scoreBoardKey.Baggage == null)
				{
					_scoreBoardKey.Baggage = _director.GetGenericBinding(output.sourceObject);
				}
				else
				{
					_director.SetGenericBinding(output.sourceObject, _scoreBoardKey.Baggage);
				}
				break;
			case "ScoreSimpleTrack":
				_simpleBoardKey.Key = output.sourceObject;
				if (_simpleBoardKey.Baggage == null)
				{
					_simpleBoardKey.Baggage = _director.GetGenericBinding(output.sourceObject);
				}
				else
				{
					_director.SetGenericBinding(output.sourceObject, _simpleBoardKey.Baggage);
				}
				break;
			case "ComboMedalTrack":
				_comboMedalKey.Key = output.sourceObject;
				if (_comboMedalKey.Baggage == null)
				{
					_comboMedalKey.Baggage = _director.GetGenericBinding(output.sourceObject);
				}
				else
				{
					_director.SetGenericBinding(output.sourceObject, _comboMedalKey.Baggage);
				}
				break;
			case "SyncMedalTrack":
				_syncMedalKey.Key = output.sourceObject;
				if (_syncMedalKey.Baggage == null)
				{
					_syncMedalKey.Baggage = _director.GetGenericBinding(output.sourceObject);
				}
				else
				{
					_director.SetGenericBinding(output.sourceObject, _syncMedalKey.Baggage);
				}
				break;
			case "Rank1stTrack":
				if (_multiPlayUserAnimators != null && _multiPlayUserAnimators[0] != null)
				{
					_multiResultKeys[0].Key = output.sourceObject;
					_director.SetGenericBinding(output.sourceObject, _multiPlayUserAnimators[0]);
				}
				break;
			case "Rank2ndTrack":
				if (_multiPlayUserAnimators != null && _multiPlayUserAnimators[1] != null)
				{
					_multiResultKeys[1].Key = output.sourceObject;
					_director.SetGenericBinding(output.sourceObject, _multiPlayUserAnimators[1]);
				}
				break;
			case "Rank3rdTrack":
				if (_multiPlayUserAnimators != null && _multiPlayUserAnimators[2] != null)
				{
					_multiResultKeys[2].Key = output.sourceObject;
					_director.SetGenericBinding(output.sourceObject, _multiPlayUserAnimators[2]);
				}
				break;
			case "Rank4thTrack":
				if (_multiPlayUserAnimators != null && _multiPlayUserAnimators[3] != null)
				{
					_multiResultKeys[3].Key = output.sourceObject;
					_director.SetGenericBinding(output.sourceObject, _multiPlayUserAnimators[3]);
				}
				break;
			}
		}
	}

	public void ReleaseTimeline()
	{
		foreach (UnityEngine.Object bindKey in _bindKeyList)
		{
			_director.SetGenericBinding(bindKey, null);
		}
		_director.SetGenericBinding(_simpleBoardKey.Key, null);
		_simpleBoardKey.Release();
		_director.SetGenericBinding(_scoreBoardKey.Key, null);
		_scoreBoardKey.Release();
	}

	private void BindLeavingUpdate()
	{
		if (_backgroundKey.IsRelease(_director.time))
		{
			_director.SetGenericBinding(_backgroundKey.Key, null);
			Animator animator = (Animator)_backgroundKey.Baggage;
			string text;
			switch (_clearRank)
			{
			case MusicClearrankID.Rank_SSS:
			case MusicClearrankID.Rank_SSSP:
				text = "Clear_Loop_SSS";
				break;
			case MusicClearrankID.Rank_SS:
			case MusicClearrankID.Rank_SSP:
				text = "Clear_Loop_SS";
				break;
			case MusicClearrankID.Rank_S:
			case MusicClearrankID.Rank_SP:
				text = "Clear_Loop_S";
				break;
			case MusicClearrankID.Rank_A:
			case MusicClearrankID.Rank_AA:
			case MusicClearrankID.Rank_AAA:
				text = "Clear_Loop_A";
				break;
			default:
				text = "Clear_Loop_B";
				break;
			}
			animator.Play(Animator.StringToHash(text), 0, 0f);
			_backgroundKey.Clear();
		}
		if (_clearRankKey.IsRelease(_director.time))
		{
			_director.SetGenericBinding(_clearRankKey.Key, null);
			_clearRankKey.Clear();
		}
		if (_comboMedalKey.IsRelease(_director.time) && PlayComboflagID.None < _comboMedal)
		{
			_director.SetGenericBinding(_comboMedalKey.Key, null);
			Animator animator2 = (Animator)_comboMedalKey.Baggage;
			string text2 = "";
			switch (_comboMedal)
			{
			case PlayComboflagID.AllPerfectPlus:
				text2 = "APp";
				break;
			case PlayComboflagID.AllPerfect:
				text2 = "AP";
				break;
			case PlayComboflagID.Gold:
				text2 = "FCp";
				break;
			case PlayComboflagID.Silver:
				text2 = "FC";
				break;
			}
			animator2.Play(Animator.StringToHash("Get_" + text2 + "_Loop"), 0, 0f);
			_comboMedalKey.Clear();
		}
		if (_syncMedalKey.IsRelease(_director.time) && PlaySyncflagID.None < _syncMedal)
		{
			_director.SetGenericBinding(_syncMedalKey.Key, null);
			Animator animator3 = (Animator)_syncMedalKey.Baggage;
			string text3 = ((_syncMedal < PlaySyncflagID.SyncLow) ? "FS" : "FSD");
			switch (_syncMedal)
			{
			case PlaySyncflagID.SyncHi:
				text3 = "FSDp";
				break;
			case PlaySyncflagID.SyncLow:
				text3 = "FSD";
				break;
			case PlaySyncflagID.ChainHi:
				text3 = "FSp";
				break;
			case PlaySyncflagID.ChainLow:
				text3 = "FS";
				break;
			}
			animator3.Play(Animator.StringToHash("Get_" + text3 + "_Loop"), 0, 0f);
			_syncMedalKey.Clear();
		}
		if (_multiResultKeys != null)
		{
			for (int i = 0; i < 4; i++)
			{
				if (_multiResultKeys[i] != null && _multiResultKeys[i].IsRelease(_director.time))
				{
					_director.SetGenericBinding(_multiResultKeys[i].Key, null);
					_multiPlayUserAnimators[i].Play(Animator.StringToHash((_multiPlayIndex == i) ? "Loop_User" : "Loop"), 0, 0f);
					_multiResultKeys[i].Clear();
				}
			}
		}
		if (_partner01Key.IsRelease(_director.time))
		{
			_director.SetGenericBinding(_partner01Key.Key, null);
			_partner01Key.Clear();
			_partnerCharacter.SetBool("IsClear", _isClear);
			if (_isClear)
			{
				_partnerCharacter.Play(NavigationAnime.FunStartLoop, 0f);
			}
			else
			{
				_partnerCharacter.Play(NavigationAnime.Sad01, 0f);
			}
		}
		if (_partner02Key.IsRelease(_director.time))
		{
			_director.SetGenericBinding(_partner02Key.Key, null);
			_partner02Key.Clear();
			_partnerCharacter.SetBool("IsClear", _isClear);
			if (_isClear)
			{
				_partnerCharacter.Play(NavigationAnime.FunStartLoop, 0f);
			}
			else
			{
				_partnerCharacter.Play(NavigationAnime.Sad01, 0f);
			}
		}
	}

	public string GetMedalSeCueCode()
	{
		string result = string.Empty;
		if (_syncMedal > PlaySyncflagID.None)
		{
			switch (_syncMedal)
			{
			case PlaySyncflagID.SyncLow:
			case PlaySyncflagID.SyncHi:
				result = "SE_RESULT_ICON_FULL_SYNC_DX";
				break;
			case PlaySyncflagID.ChainLow:
			case PlaySyncflagID.ChainHi:
				result = "SE_RESULT_ICON_FULL_SYNC";
				break;
			}
		}
		else
		{
			switch (_comboMedal)
			{
			case PlayComboflagID.AllPerfect:
			case PlayComboflagID.AllPerfectPlus:
				result = "SE_RESULT_ICON_AP";
				break;
			case PlayComboflagID.Silver:
			case PlayComboflagID.Gold:
				result = "SE_RESULT_ICON_FC";
				break;
			}
		}
		return result;
	}
}
