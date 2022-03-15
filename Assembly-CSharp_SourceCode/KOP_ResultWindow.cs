using System;
using Mai2.Mai2Cue;
using MAI2System;
using Manager;
using Timeline;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class KOP_ResultWindow : MonoBehaviour
{
	private enum FadeInState
	{
		None,
		Start,
		FadeIn,
		CountUp,
		Active,
		End
	}

	private enum FadeOutState
	{
		None,
		Start,
		Wait,
		End
	}

	[SerializeField]
	private KOP_ResultTrackData[] _resultTrackData;

	[SerializeField]
	[Header("順位")]
	private MultipleImage _rankImage;

	[SerializeField]
	private TextMeshProUGUI _diffText;

	[SerializeField]
	[Header("達成率")]
	private AchievementCounterObject _achievementCounter;

	[SerializeField]
	private Animator _achievementAnimator;

	[SerializeField]
	[Header("でらっくすスコア")]
	private Image[] _deluxeImages;

	[SerializeField]
	private Sprite[] _numSprites;

	private float _counterUpTime = 1000f;

	private Animator _windowAnimator;

	private FadeInState _fadeInState;

	private FadeOutState _fadeOutState;

	private float _timer;

	private int _maxTrack;

	private Action _onEndFadeOutAction;

	private int _playerIndex;

	public void Initialize(int playerIndex, int maxTrackCount)
	{
		_playerIndex = playerIndex;
		_maxTrack = maxTrackCount;
		if (_maxTrack < 4)
		{
			int num = _resultTrackData.Length;
			while (_maxTrack < num)
			{
				_resultTrackData[num - 1].transform.parent.gameObject.SetActive(value: false);
				num--;
			}
		}
		_windowAnimator = GetComponent<Animator>();
		_fadeInState = FadeInState.None;
		_fadeOutState = FadeOutState.None;
	}

	public void SetTotalResult(uint totalAchievement, int totalDeluxeScore)
	{
		_achievementCounter.SetAchievement(0u, totalAchievement);
		if (totalDeluxeScore > 0)
		{
			int num = (int)Mathf.Log10(totalDeluxeScore) + 1;
			int num2 = totalDeluxeScore;
			for (int i = 0; i < _deluxeImages.Length; i++)
			{
				if (i < num)
				{
					_deluxeImages[i].gameObject.SetActive(value: true);
					int num3 = num2 % 10;
					num2 /= 10;
					_deluxeImages[i].sprite = _numSprites[num3];
				}
				else
				{
					_deluxeImages[i].gameObject.SetActive(value: false);
				}
			}
		}
		else
		{
			_deluxeImages[0].gameObject.SetActive(value: true);
			for (int j = 1; j < _deluxeImages.Length; j++)
			{
				_deluxeImages[j].gameObject.SetActive(value: false);
			}
		}
	}

	public void SetTotalRankData(int rank, int diff)
	{
		_rankImage.ChangeSprite(rank);
		_diffText.text = ((float)diff / 10000f).ToString("##0.0000") + "%";
		_diffText.color = ((diff < 0) ? CommonScriptable.GetColorSetting().DeclineColor : CommonScriptable.GetColorSetting().RiseColor);
	}

	public void SetResultData(int trackNo, string musicName, ConstParameter.ScoreKind kind, Texture2D jacket, uint achievement, int deluxeScore, int level, MusicLevelID levelId, int difficulty)
	{
		_resultTrackData[trackNo].SetDisplay(isResultOpen: true);
		_resultTrackData[trackNo].SetMusicData(trackNo, musicName, kind, jacket);
		_resultTrackData[trackNo].SetDeluxeScore(deluxeScore);
		_resultTrackData[trackNo].SetDifficultyLevel(level, levelId, difficulty);
		_resultTrackData[trackNo].SetAchievementData(achievement);
	}

	public void SetTrackDisplay(int trackNo, bool isOpen)
	{
		_resultTrackData[trackNo].SetDisplay(isOpen);
	}

	public void PlayFadeIn(int currentTrack)
	{
		_fadeInState = FadeInState.Start;
		for (int i = 0; i < _resultTrackData.Length; i++)
		{
			_resultTrackData[i].SetDisplay(i < currentTrack);
		}
		_achievementCounter.OnBehaviourPlay();
		_windowAnimator.Play(Animator.StringToHash("FadeIn"), 0, 0f);
	}

	public void PlayFadeOut(Action onEndAction)
	{
		if (_windowAnimator != null)
		{
			_fadeOutState = FadeOutState.Start;
			_onEndFadeOutAction = onEndAction;
			_windowAnimator.Play(Animator.StringToHash("FadeOut"), 0, 0f);
		}
		else
		{
			onEndAction?.Invoke();
		}
	}

	public void ViewUpdate(float deltaTime)
	{
		switch (_fadeInState)
		{
		case FadeInState.Start:
			_fadeInState = FadeInState.FadeIn;
			break;
		case FadeInState.FadeIn:
			if (1f <= _windowAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime)
			{
				_windowAnimator.Play(Animator.StringToHash("Loop"), 0, 0f);
				SoundManager.PlayLoopSE(Cue.SE_RESULT_GAUGE_UP, _playerIndex);
				_fadeInState = FadeInState.CountUp;
			}
			break;
		case FadeInState.CountUp:
			_achievementCounter.PrepareFrame(_timer / _counterUpTime);
			if (_counterUpTime <= _timer)
			{
				_timer = 0f;
				SoundManager.StopLoopSe(_playerIndex);
				SoundManager.PlaySE(Cue.SE_RESULT_SCORE_END, _playerIndex);
				_achievementAnimator.Play(Animator.StringToHash("Active"), 0, 0f);
				_fadeInState = FadeInState.Active;
			}
			else
			{
				_timer += deltaTime;
			}
			break;
		case FadeInState.Active:
			_fadeInState = FadeInState.End;
			break;
		}
		switch (_fadeOutState)
		{
		case FadeOutState.Start:
			_fadeOutState = FadeOutState.Wait;
			break;
		case FadeOutState.Wait:
			if (1f <= _windowAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime)
			{
				_onEndFadeOutAction?.Invoke();
				_fadeOutState = FadeOutState.End;
			}
			break;
		case FadeOutState.None:
		case FadeOutState.End:
			break;
		}
	}
}
