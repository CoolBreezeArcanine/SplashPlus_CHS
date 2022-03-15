using DB;
using Mai2.Mai2Cue;
using MAI2.Util;
using Manager;
using UnityEngine;

namespace Monitor
{
	public class TrackSkip : MonoBehaviour
	{
		private int _monitorIndex;

		private Animator _anim;

		private double _pushTimer;

		private const double PushTime = 3000.0;

		private const double WaitTime = 500.0;

		private int _pushPhase;

		private int _myBest;

		private bool _execTrackSkip;

		private void Awake()
		{
			_anim = base.gameObject.GetComponent<Animator>();
			base.gameObject.SetActive(value: false);
		}

		public void Initialize(int monitorIndex, int myBest)
		{
			_monitorIndex = monitorIndex;
			_pushTimer = 0.0;
			_pushPhase = -1;
			_execTrackSkip = false;
			_myBest = myBest;
			base.gameObject.SetActive(value: false);
			if (GameManager.IsCourseMode && Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).UserOption.TrackSkip != 0)
			{
				Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).UserOption.TrackSkip = OptionTrackskipID.Push;
			}
		}

		public virtual void Execute()
		{
			if (_execTrackSkip)
			{
				return;
			}
			bool flag = IsTrackSkipEnable();
			bool flag2 = IsPushButtonTrackSkipEnable();
			switch (Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).UserOption.TrackSkip)
			{
			case OptionTrackskipID.Push:
				if (InputManager.GetButtonPush(_monitorIndex, InputManager.ButtonSetting.Button02) && InputManager.GetButtonPush(_monitorIndex, InputManager.ButtonSetting.Button03) && InputManager.GetButtonPush(_monitorIndex, InputManager.ButtonSetting.Button06) && InputManager.GetButtonPush(_monitorIndex, InputManager.ButtonSetting.Button07) && Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).GetTheoryAchivement() != 0m && !Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).IsAllJudged() && flag2)
				{
					_pushTimer += GameManager.GetGameMSecAddD();
					if (_pushTimer >= 500.0)
					{
						double num = _pushTimer - 500.0;
						if (_pushPhase < (int)(num / 1000.0))
						{
							_pushPhase = (int)(num / 1000.0);
							SoundManager.PlayGameSE(Cue.SE_SYS_COUNT, _monitorIndex, 1f);
							int pushPhase = _pushPhase;
						}
						if (3000.0 <= num)
						{
							_execTrackSkip = true;
							Singleton<GamePlayManager>.Instance.SetTrackSkipFrag(_monitorIndex);
						}
					}
				}
				else
				{
					_pushTimer = 0.0;
					_pushPhase = -1;
				}
				break;
			case OptionTrackskipID.AutoS:
				if (101.0m - (decimal)MusicClearrankID.Rank_S.GetAchvement() / 10000m - Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).GetTheoryAchivementDiff() < 0m && flag)
				{
					_execTrackSkip = true;
					Singleton<GamePlayManager>.Instance.SetTrackSkipFrag(_monitorIndex);
				}
				break;
			case OptionTrackskipID.AutoSS:
				if (101.0m - (decimal)MusicClearrankID.Rank_SS.GetAchvement() / 10000m - Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).GetTheoryAchivementDiff() < 0m && flag)
				{
					_execTrackSkip = true;
					Singleton<GamePlayManager>.Instance.SetTrackSkipFrag(_monitorIndex);
				}
				break;
			case OptionTrackskipID.AutoSSS:
				if (101.0m - (decimal)MusicClearrankID.Rank_SSS.GetAchvement() / 10000m - Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).GetTheoryAchivementDiff() < 0m && flag)
				{
					_execTrackSkip = true;
					Singleton<GamePlayManager>.Instance.SetTrackSkipFrag(_monitorIndex);
				}
				break;
			case OptionTrackskipID.AutoBest:
				if (101.0m - (decimal)_myBest / 10000m - Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).GetTheoryAchivementDiff() < 0m && flag)
				{
					_execTrackSkip = true;
					Singleton<GamePlayManager>.Instance.SetTrackSkipFrag(_monitorIndex);
				}
				break;
			}
			if (_execTrackSkip)
			{
				PlayAnim();
			}
		}

		private bool IsTrackSkipEnable()
		{
			if (Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).IsChallenge)
			{
				return false;
			}
			if (Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).IsCourse)
			{
				return false;
			}
			return true;
		}

		private bool IsPushButtonTrackSkipEnable()
		{
			if (Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).IsChallenge)
			{
				return false;
			}
			if (Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).IsCourse)
			{
				if (Singleton<CourseManager>.Instance.GetCourseData() != null && Singleton<CourseManager>.Instance.GetCourseData().courseMode.id != 2 && Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).Life == 0)
				{
					return true;
				}
				return false;
			}
			return true;
		}

		public int GetPushPhase()
		{
			return _pushPhase;
		}

		private void PlayAnim()
		{
			SoundManager.PlaySE(Cue.SE_GAME_TRACK_SKIP, _monitorIndex);
			base.gameObject.SetActive(value: true);
			_anim.Play("In_Loop");
		}
	}
}
