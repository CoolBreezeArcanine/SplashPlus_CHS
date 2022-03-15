using System.Collections;
using Mai2.Mai2Cue;
using MAI2.Util;
using Manager;
using Process.CourseResult;
using Timeline;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.CourseResult
{
	public class CourseResultMonitor : MonitorBase
	{
		private enum ButtonEnum
		{
			OK
		}

		[SerializeField]
		[Header("各種コントローラー")]
		private CourseResultButtonController _buttonController;

		[SerializeField]
		[Header("nullポジション")]
		private Transform _courseBGTransform;

		[SerializeField]
		private Transform[] _trackDataTransform;

		[SerializeField]
		private Transform[] _stampMarkTransform;

		[SerializeField]
		private Transform _totalLifeTransform;

		[SerializeField]
		private Transform _clearPlateTransform;

		[SerializeField]
		private Transform _rankUpTransform;

		[SerializeField]
		private Transform _transitionTransform;

		[SerializeField]
		private Transform _getTicketWindowTransform;

		[SerializeField]
		[Header("オリジナルprefab")]
		private GameObject _courseBGObj;

		[SerializeField]
		private GameObject _trackDataObj;

		[SerializeField]
		private GameObject _stampMarkObj;

		[SerializeField]
		private GameObject _totalLifeObj;

		[SerializeField]
		private GameObject _clearPlateObj;

		[SerializeField]
		private GameObject _rankUpObj;

		[SerializeField]
		private GameObject _transitionObj;

		[SerializeField]
		private GameObject _getTicketWindowObj;

		[SerializeField]
		[Header("設定物")]
		private Animator _mainAnim;

		[SerializeField]
		private GameObject _daniBG;

		[SerializeField]
		private GameObject _sinDaniBG;

		[SerializeField]
		private GameObject _randomBG;

		[SerializeField]
		private TextMeshProUGUI _dxscoreText;

		[SerializeField]
		private GameObject _notClearObj;

		[SerializeField]
		private Image _classImage;

		[SerializeField]
		private AchievementCounterObject _totalAchieve;

		private CourseBGController _courseBg;

		private KOP_ResultTrackData[] _trackData;

		private CourseStamp[] _stampMark;

		private CourseLife _totalLife;

		private CourseClearPlate _clearPlate;

		private CourseRankUp _rankUp;

		private CourseTransition _transition;

		private GetTicketWindow _getTicketWindow;

		private int _courseMode;

		private int _restLife;

		private bool[] _isPlay;

		private int[] _totalAchieveNum;

		private int[] _totalScore;

		private int[] _totalLifeNum;

		public override void Initialize(int monIndex, bool active)
		{
			_trackData = new KOP_ResultTrackData[_trackDataTransform.Length];
			_stampMark = new CourseStamp[_stampMarkTransform.Length];
			base.Initialize(monIndex, active);
			_courseBg = Object.Instantiate(_courseBGObj, _courseBGTransform).GetComponent<CourseBGController>();
			_totalLife = Object.Instantiate(_totalLifeObj, _totalLifeTransform).GetComponent<CourseLife>();
			_clearPlate = Object.Instantiate(_clearPlateObj, _clearPlateTransform).GetComponent<CourseClearPlate>();
			_rankUp = Object.Instantiate(_rankUpObj, _rankUpTransform).GetComponent<CourseRankUp>();
			_transition = Object.Instantiate(_transitionObj, _transitionTransform).GetComponent<CourseTransition>();
			_getTicketWindow = Object.Instantiate(_getTicketWindowObj, _getTicketWindowTransform).GetComponent<GetTicketWindow>();
			int coursePlayTicketId = Singleton<TicketManager>.Instance.GetCoursePlayTicketId(monIndex);
			if (coursePlayTicketId != -1)
			{
				Sprite ticketSprite = Resources.Load<Sprite>(Singleton<TicketManager>.Instance.GetGetWindowFileName(coursePlayTicketId));
				string ticketName = Singleton<TicketManager>.Instance.GetTicketName(coursePlayTicketId);
				_getTicketWindow.SetTicket(ticketSprite, ticketName);
			}
			_getTicketWindow.gameObject.SetActive(value: false);
			for (int i = 0; i < _trackDataTransform.Length; i++)
			{
				_trackData[i] = Object.Instantiate(_trackDataObj, _trackDataTransform[i]).GetComponent<KOP_ResultTrackData>();
				_trackData[i].gameObject.SetActive(value: false);
			}
			for (int j = 0; j < _stampMarkTransform.Length; j++)
			{
				_stampMark[j] = Object.Instantiate(_stampMarkObj, _stampMarkTransform[j]).GetComponent<CourseStamp>();
				_stampMark[j].gameObject.SetActive(value: false);
			}
			_isPlay = new bool[_trackDataTransform.Length];
			_totalAchieveNum = new int[_trackDataTransform.Length];
			_totalScore = new int[_trackDataTransform.Length];
			_totalLifeNum = new int[_trackDataTransform.Length];
			_rankUp.gameObject.SetActive(value: false);
			_notClearObj.SetActive(value: false);
			_buttonController.Initialize(monIndex);
		}

		public override void ViewUpdate()
		{
			if (IsActive())
			{
				UpdateButtonAnimation();
			}
		}

		public void SetResultTrackData(int track, CourseResultProcess.CourseResultMusic crm)
		{
			if (crm.isPlay)
			{
				_trackData[track].SetDisplay(isResultOpen: true);
				_trackData[track].SetMusicData(track, crm.musicName, crm.scoreType, crm.jacket);
				_trackData[track].SetAchievementData((uint)crm.achievement);
				_trackData[track].SetDeluxeScore(crm.dxscore);
				_trackData[track].SetDifficultyLevel(crm.level, crm.levelId, crm.difficultyId);
				_stampMark[track].SetClear(crm.life != 0);
				_isPlay[track] = true;
				_totalAchieveNum[track] = crm.totalAchievement;
				_totalScore[track] = crm.totalDxscore;
				_totalLifeNum[track] = crm.life;
			}
			else
			{
				_isPlay[track] = false;
				_trackData[track].SetDisplay(isResultOpen: false);
			}
		}

		public void SetResultTotalData(CourseResultProcess.CourseResultTotal crt)
		{
			SetCourseMode(crt.courseMode);
			SetResultSprite(crt.daniImage, crt.daniPlate);
			SetCourseData(crt.startLife, 0, 0);
		}

		public void FirstFadeIn()
		{
			if (IsActive())
			{
				StartCoroutine(FirstFadeInCoroutine());
			}
		}

		private IEnumerator FirstFadeInCoroutine()
		{
			_transition.SetAnim(CourseTransition.BGAnim.Idle, base.MonitorIndex);
			_rankUp.gameObject.SetActive(value: false);
			SetButtonOut();
			_mainAnim.Play("In");
			yield return new WaitForSeconds(1f);
			_mainAnim.Play("In_Life");
			yield return new WaitForSeconds(0.6f);
			_mainAnim.Play("In_TotalScore");
		}

		public void MainResultFadeIn()
		{
			if (IsActive())
			{
				_transition.SetAnim(CourseTransition.BGAnim.In, base.MonitorIndex);
			}
		}

		public void TrackFadeIn()
		{
			if (IsActive())
			{
				StartCoroutine(TrackFadeInCoroutine());
			}
		}

		private IEnumerator TrackFadeInCoroutine()
		{
			string[] animStr = new string[_trackDataTransform.Length];
			animStr[0] = "In_Track01";
			animStr[1] = "In_Track02";
			animStr[2] = "In_Track03";
			animStr[3] = "In_Track04";
			for (int i = 0; i < _trackDataTransform.Length; i++)
			{
				if (i != 0)
				{
					yield return new WaitForSeconds(0.2f);
				}
				if (_isPlay[i])
				{
					_trackData[i].gameObject.SetActive(value: true);
					_stampMark[i].gameObject.SetActive(value: true);
					SoundManager.PlaySE(Cue.SE_DANI_RESULT_SCORE, base.MonitorIndex);
					SetCourseData(_totalLifeNum[i], _totalAchieveNum[i], _totalScore[i]);
					_mainAnim.Play(animStr[i]);
					_stampMark[i].PlayClearAnim();
				}
			}
		}

		public void ClearFadeIn()
		{
			StartCoroutine(ClearFadeInCoroutine());
		}

		private IEnumerator ClearFadeInCoroutine()
		{
			if (IsActive())
			{
				if (_restLife == 0)
				{
					_notClearObj.SetActive(value: true);
					SoundManager.PlaySE(Cue.SE_DANI_FAILED, base.MonitorIndex);
					_mainAnim.Play("Dani_NoClear");
				}
				else
				{
					_notClearObj.SetActive(value: false);
					SoundManager.PlaySE(Cue.SE_DANI_SUCCESS, base.MonitorIndex);
					_mainAnim.Play("Dani_Clear");
					_clearPlate.ViewClear(isClear: true);
				}
			}
			yield return new WaitForSeconds(0.02f);
			if (IsActive() && _restLife != 0)
			{
				_clearPlate.SetAnim(CourseClearPlate.BGAnim.Loop);
			}
		}

		public void RankUpEffectIn()
		{
			if (IsActive())
			{
				SoundManager.PlaySE(Cue.SE_DANI_UP_01, base.MonitorIndex);
				_rankUp.gameObject.SetActive(value: true);
				switch (_courseMode)
				{
				case 1:
					_rankUp.SetAnim(CourseRankUp.BGAnim.In_Dani);
					break;
				case 2:
					_rankUp.SetAnim(CourseRankUp.BGAnim.In_SinDani);
					break;
				case 3:
					_rankUp.SetAnim(CourseRankUp.BGAnim.In_Random);
					break;
				}
			}
		}

		public void RankUpEffectOut()
		{
			if (IsActive())
			{
				_rankUp.SetAnim(CourseRankUp.BGAnim.Out);
			}
		}

		public void GetTicketIn()
		{
			if (IsActive())
			{
				_getTicketWindow.gameObject.SetActive(value: true);
				_getTicketWindow.FadeIn();
				SoundManager.PlaySE(Cue.JINGLE_CHARA_GET, base.MonitorIndex);
			}
		}

		public void GetTicketOut()
		{
			if (IsActive())
			{
				_getTicketWindow.FadeOut();
			}
		}

		private void SetCourseMode(int courseMode)
		{
			_courseMode = courseMode;
			if (_daniBG != null && _sinDaniBG != null && _sinDaniBG != null && _courseBg != null)
			{
				switch (_courseMode)
				{
				case 1:
					_daniBG.SetActive(value: true);
					_sinDaniBG.SetActive(value: false);
					_randomBG.SetActive(value: false);
					_courseBg.SetAnim(CourseBGController.BGAnim.Dani_Track_Loop);
					break;
				case 2:
					_daniBG.SetActive(value: false);
					_sinDaniBG.SetActive(value: true);
					_randomBG.SetActive(value: false);
					_courseBg.SetAnim(CourseBGController.BGAnim.SinDani_Track_Loop);
					break;
				case 3:
					_daniBG.SetActive(value: false);
					_sinDaniBG.SetActive(value: false);
					_randomBG.SetActive(value: true);
					_courseBg.SetAnim(CourseBGController.BGAnim.Dani_Track_Loop);
					break;
				}
			}
		}

		private void SetCourseData(int restLife, int achievement, int dxScore)
		{
			_restLife = restLife;
			if (_dxscoreText != null)
			{
				_dxscoreText.text = dxScore.ToString();
			}
			if (_totalAchieve != null)
			{
				_totalAchieve.SetAchievement(0u, (uint)achievement);
			}
			if (_totalAchieve != null)
			{
				_totalAchieve.OnClipTailEnd();
			}
			if (_totalLife != null)
			{
				_totalLife.SetLife((uint)_restLife);
			}
		}

		private void SetResultSprite(Sprite imageSprite, Sprite plateSprite)
		{
			if (_classImage != null)
			{
				_classImage.sprite = imageSprite;
			}
			if (_rankUp != null)
			{
				_rankUp.SetClassSprite(plateSprite);
			}
		}

		private void UpdateButtonAnimation()
		{
			if (isPlayerActive)
			{
				_buttonController.ViewUpdate(GameManager.GetGameMSecAdd());
			}
		}

		public void SetButtonIn()
		{
			_buttonController.SetVisible(true, default(int));
		}

		public void SetButtonSkip()
		{
			_buttonController.SetAnimationActive(0);
		}

		public void SetButtonOut()
		{
			_buttonController.SetVisible(false, default(int));
		}
	}
}
