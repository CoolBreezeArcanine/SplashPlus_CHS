using System.Collections.Generic;
using System.Linq;
using DB;
using IO;
using Mai2.Mai2Cue;
using MAI2.Util;
using Manager;
using Manager.MaiStudio;
using Manager.UserDatas;
using UI;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace Monitor
{
	public class TrackStartMonitor : MonitorBase
	{
		public enum TrackStartType
		{
			Normal,
			Versus,
			BossBattle,
			Challenge,
			Course,
			End
		}

		private enum SubMonitorState
		{
			None,
			ChallengeFadeIn,
			ChallengeAction,
			Finish,
			End
		}

		[SerializeField]
		private SpriteCounter _trackNumber;

		[SerializeField]
		private SpriteCounter _bossTrackNumber;

		[SerializeField]
		[Header("通常用データ")]
		private GameObject _normalNextTrack;

		[SerializeField]
		private MultipleImage _musicBaseImage;

		[SerializeField]
		private MultipleImage _musicTabImage;

		[SerializeField]
		private GameObject[] _musicTabObj;

		[SerializeField]
		private RawImage _jacket;

		[SerializeField]
		private CustomTextScroll _musicName;

		[SerializeField]
		private CustomTextScroll _artistName;

		[SerializeField]
		[Header("難易度レベル")]
		private SpriteCounter _difficultySingle;

		[SerializeField]
		private SpriteCounter _difficultyDouble;

		[SerializeField]
		private Image _levelTextImage;

		[SerializeField]
		[Header("レベル画像")]
		private List<ResultMonitor.SpriteSheet> _musicLevelSpriteSheets;

		[SerializeField]
		[Header("バーサス用データ")]
		private GameObject _vsNextTrack;

		[SerializeField]
		[Header("通常戦")]
		private UserInformationController _upInfo;

		[SerializeField]
		private UserInformationController _downInfo;

		[SerializeField]
		[Header("ボス戦")]
		private UserInformationController _bossUpInfo;

		[SerializeField]
		private UserInformationController _bossDownInfo;

		[SerializeField]
		[Header("再生タイムライン")]
		private PlayableAsset _normalPlayable;

		[SerializeField]
		private PlayableAsset _vslPlayable;

		[SerializeField]
		private PlayableAsset _bossPlayable;

		[SerializeField]
		private GameObject _haveDirectorObject;

		private PlayableDirector _director;

		[SerializeField]
		[Header("LIFE")]
		private Animator _challengeObject;

		[SerializeField]
		private SpriteCounter _lifeNum;

		[SerializeField]
		private Animator _otohime01;

		[SerializeField]
		private Animator _otohime02;

		[SerializeField]
		[Header("コースモード")]
		private GameObject _originalCourseTS;

		[SerializeField]
		private Transform _courseTSTransform;

		private CourseTrackStart _courseTrackStart;

		[SerializeField]
		[Header("Prefab挿入NULL")]
		private GameObject _derakkumaRoot;

		[SerializeField]
		private GameObject _bossDerakkumaRoot;

		[SerializeField]
		[Header("外部Prefab")]
		private GameObject _originalDerakkuma;

		private AssetManager _assetManager;

		private const string DetakkmaTrackName = "DerakkumaTrack";

		[SerializeField]
		private string _vsVoiceName;

		[SerializeField]
		private string _ifWinUpTrackName;

		[SerializeField]
		private string _ifWinUpVoiceName;

		[SerializeField]
		private string _ifLoseDownTrackName;

		[SerializeField]
		private string _ifLoseDownVoiceName;

		public bool _isChallenge;

		public int _challenge_id = -1;

		private int _life = 100;

		private string _str = "";

		public bool _isCourse;

		private SubMonitorState _sub_state;

		public void SetLife()
		{
			string text = _life.ToString("D000");
			if (_life > 999)
			{
				text = "999";
			}
			_lifeNum.ChangeText(text);
			if (_life < 100)
			{
				_lifeNum.FrameList[2].Scale = 0f;
				_lifeNum.FrameList[0].RelativePosition.x = 52f;
				_lifeNum.FrameList[1].RelativePosition.x = 24f;
				if (_life < 10)
				{
					_lifeNum.FrameList[1].Scale = 0f;
					_lifeNum.FrameList[0].RelativePosition.x = 74f;
				}
			}
		}

		public override void Initialize(int monIndex, bool isActive)
		{
			base.Initialize(monIndex, isActive);
			if (IsActive())
			{
				MechaManager.LedIf[monIndex].ButtonLedReset();
			}
			if (!isActive)
			{
				Main.gameObject.SetActive(value: false);
				Sub.gameObject.SetActive(value: false);
				return;
			}
			_director = _haveDirectorObject.GetComponent<PlayableDirector>();
			_director.Play();
			_director.Pause();
			_director.time = 0.0;
			_trackNumber.ChangeText(GameManager.MusicTrackNumber.ToString().PadLeft(2, ' '));
			_bossTrackNumber.ChangeText(GameManager.MusicTrackNumber.ToString().PadLeft(2, ' '));
			_courseTrackStart = Object.Instantiate(_originalCourseTS, _courseTSTransform).GetComponent<CourseTrackStart>();
			_courseTrackStart.Initialize();
			_isChallenge = false;
			if (GameManager.IsPerfectChallenge)
			{
				_isChallenge = true;
			}
			bool flag = false;
			if (_isChallenge)
			{
				int num = GameManager.SelectMusicID[monIndex];
				if (num > 0)
				{
					NotesWrapper notesWrapper = Singleton<NotesListManager>.Instance.GetNotesList()[num];
					for (int i = 0; i < notesWrapper.ChallengeDetail.Length; i++)
					{
						if (notesWrapper.ChallengeDetail[i].isEnable && notesWrapper.ChallengeDetail[i].startLife > 0)
						{
							_life = notesWrapper.ChallengeDetail[i].startLife;
							_challenge_id = notesWrapper.ChallengeDetail[i].challengeId;
							flag = true;
							break;
						}
					}
				}
				SetLife();
			}
			if (!flag)
			{
				_isChallenge = false;
			}
			_challengeObject.gameObject.transform.parent.gameObject.SetActive(value: false);
			_challengeObject.gameObject.SetActive(value: false);
			if (_isChallenge)
			{
				_str = "Lv_01";
				ChallengeData challengeData = Singleton<DataManager>.Instance.GetChallengeData(_challenge_id);
				int num2 = -1;
				if (_challenge_id > 0 && challengeData != null)
				{
					foreach (var item in challengeData.Relax.Select((ChallengeRelax value, int index) => new { value, index }))
					{
						if (item.value.Life == _life)
						{
							num2 = item.index + 1;
							break;
						}
					}
				}
				if (num2 > 0)
				{
					switch (num2)
					{
					case 1:
						_str = "Lv_01";
						break;
					case 2:
						_str = "Lv_02";
						break;
					case 3:
						_str = "Lv_03";
						break;
					case 4:
						_str = "Lv_04";
						break;
					case 5:
						_str = "Lv_05";
						break;
					}
				}
				else
				{
					if (_life >= 1)
					{
						_str = "Lv_01";
					}
					if (_life >= 10)
					{
						_str = "Lv_02";
					}
					if (_life >= 50)
					{
						_str = "Lv_03";
					}
					if (_life >= 100)
					{
						_str = "Lv_04";
					}
					if (_life >= 300)
					{
						_str = "Lv_05";
					}
				}
			}
			_isCourse = false;
			if (GameManager.IsCourseMode)
			{
				_isCourse = true;
			}
			bool flag2 = false;
			if (_isCourse)
			{
				string courseImage = Singleton<CourseManager>.Instance.GetCourseImage(CourseManager.ImageType.Title);
				if (courseImage != "")
				{
					Sprite sprite = Resources.Load<Sprite>(courseImage);
					if (sprite != null)
					{
						int id = Singleton<CourseManager>.Instance.GetCourseData().courseMode.id;
						int musicTrackNumber = (int)GameManager.MusicTrackNumber;
						int beforeRestLife = Singleton<CourseManager>.Instance.GetBeforeRestLife(monIndex);
						int restLife = Singleton<CourseManager>.Instance.GetRestLife(monIndex);
						_courseTrackStart.Prepare(sprite, id, beforeRestLife, restLife, musicTrackNumber);
						flag2 = true;
						_courseTrackStart.gameObject.SetActive(value: true);
					}
				}
			}
			if (!flag2)
			{
				_isCourse = false;
				_courseTrackStart.gameObject.SetActive(value: false);
			}
			_sub_state = SubMonitorState.None;
		}

		public void SetAssetManager(AssetManager asset)
		{
			_assetManager = asset;
		}

		public void SetTrackStart(TrackStartType type)
		{
			_haveDirectorObject.SetActive(value: true);
			if (type == TrackStartType.BossBattle)
			{
				_bossDerakkumaRoot = Object.Instantiate(_originalDerakkuma, _bossDerakkumaRoot.transform);
			}
			else
			{
				_derakkumaRoot = Object.Instantiate(_originalDerakkuma, _derakkumaRoot.transform);
			}
			MusicData music = Singleton<DataManager>.Instance.GetMusic(GameManager.SelectMusicID[monitorIndex]);
			_jacket.texture = _assetManager.GetJacketTexture2D(music.jacketFile);
			_musicName.SetData(music.name.str);
			_artistName.SetData(music.artistName.str);
			_musicBaseImage.ChangeSprite(GameManager.SelectDifficultyID[monitorIndex]);
			int num = 0;
			string text = "0";
			MusicData music2 = Singleton<DataManager>.Instance.GetMusic(GameManager.SelectMusicID[monitorIndex]);
			num = music2.notesData[GameManager.SelectDifficultyID[monitorIndex]].level;
			text = ((MusicLevelID)music2.notesData[GameManager.SelectDifficultyID[monitorIndex]].musicLevelID).GetLevelNum();
			int id = music2.name.id;
			Sprite[] musicLevelSprites = CommonPrefab.GetMusicLevelSprites(GameManager.SelectDifficultyID[monitorIndex]);
			_difficultySingle.SetSpriteSheet(musicLevelSprites);
			_difficultyDouble.SetSpriteSheet(musicLevelSprites);
			_levelTextImage.sprite = musicLevelSprites[14];
			if (num > 9)
			{
				_difficultyDouble.ChangeText(text.PadRight(3));
				_difficultyDouble.gameObject.SetActive(value: true);
				_difficultySingle.gameObject.SetActive(value: false);
			}
			else
			{
				_difficultySingle.ChangeText(text.PadRight(2));
				_difficultyDouble.gameObject.SetActive(value: false);
				_difficultySingle.gameObject.SetActive(value: true);
			}
			if (id < 10000)
			{
				_musicTabImage.ChangeSprite(GameManager.SelectDifficultyID[monitorIndex]);
				_musicTabImage.transform.localScale = new Vector3(-1f, 1f, 1f);
				_musicTabObj[0].SetActive(value: false);
				_musicTabObj[1].SetActive(value: true);
			}
			else if (10000 < id && id < 20000)
			{
				_musicTabImage.ChangeSprite(GameManager.SelectDifficultyID[monitorIndex]);
				_musicTabImage.transform.localScale = new Vector3(1f, 1f, 1f);
				_musicTabObj[0].SetActive(value: true);
				_musicTabObj[1].SetActive(value: false);
			}
			switch (type)
			{
			case TrackStartType.Normal:
				_director.playableAsset = _normalPlayable;
				SoundManager.PlaySE(Cue.SE_TRACK_START_NORMAL, monitorIndex);
				break;
			case TrackStartType.Versus:
			case TrackStartType.BossBattle:
			{
				_director.playableAsset = ((type == TrackStartType.Versus) ? _vslPlayable : _bossPlayable);
				MessageUserInformationData userData = new MessageUserInformationData(monitorIndex, _assetManager, Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).Detail, Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).Option.DispRate, isSubMonitor: false);
				UserGhost ghostToEnum = Singleton<GhostManager>.Instance.GetGhostToEnum(GameManager.SelectGhostID[monitorIndex]);
				MessageUserInformationData messageUserInformationData = null;
				if (ghostToEnum != null && ghostToEnum.Type == UserGhost.GhostType.Player)
				{
					messageUserInformationData = new MessageUserInformationData(2, _assetManager, new UserDetail
					{
						UserName = ghostToEnum.Name,
						EquipIconID = ghostToEnum.IconId,
						EquipPlateID = ghostToEnum.PlateId,
						EquipTitleID = ghostToEnum.TitleId,
						Rating = (uint)ghostToEnum.Rate,
						ClassRank = (uint)UserUdemae.GetRateToUdemaeID(ghostToEnum.ClassValue),
						CourseRank = ghostToEnum.CourseRank
					}, OptionDisprateID.AllDisp, isSubMonitor: false);
				}
				else if (ghostToEnum != null && ghostToEnum.Type == UserGhost.GhostType.MapNpc)
				{
					messageUserInformationData = new MessageUserInformationData(_assetManager, new UserGhost
					{
						Name = ghostToEnum.Name,
						IconId = ghostToEnum.IconId,
						PlateId = ghostToEnum.PlateId,
						TitleId = ghostToEnum.TitleId,
						Rate = ghostToEnum.Rate,
						ClassValue = ghostToEnum.ClassValue,
						ClassRank = (uint)UserUdemae.GetRateToUdemaeID(ghostToEnum.ClassValue),
						CourseRank = ghostToEnum.CourseRank
					});
				}
				else if (ghostToEnum != null && ghostToEnum.Type == UserGhost.GhostType.Boss)
				{
					messageUserInformationData = new MessageUserInformationData(_assetManager, new UserDetail
					{
						UserName = ghostToEnum.Name,
						EquipIconID = ghostToEnum.IconId,
						EquipPlateID = ghostToEnum.PlateId,
						EquipTitleID = ghostToEnum.TitleId,
						Rating = (uint)ghostToEnum.Rate,
						ClassRank = (uint)UserUdemae.GetRateToUdemaeID(ghostToEnum.ClassValue),
						CourseRank = ghostToEnum.CourseRank
					});
				}
				if (type == TrackStartType.BossBattle)
				{
					_bossUpInfo.SetUserData(userData);
					if (messageUserInformationData != null)
					{
						_bossDownInfo.SetUserData(messageUserInformationData);
					}
				}
				else
				{
					_upInfo.SetUserData(userData);
					if (messageUserInformationData != null)
					{
						_downInfo.SetUserData(messageUserInformationData);
					}
				}
				bool isBoss = (monitorIndex == 0 && GameManager.SelectGhostID[monitorIndex] == GhostManager.GhostTarget.BossGhost_1P) || (monitorIndex == 1 && GameManager.SelectGhostID[monitorIndex] == GhostManager.GhostTarget.BossGhost_2P);
				int classValue = ghostToEnum.ClassValue;
				UserUdemae.ChangeGrade changeGrade = Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).RatingList.Udemae.CalcChangeGrade(classValue, isBoss);
				((TimelineAsset)_director.playableAsset).GetOutputTracks().First((TrackAsset c) => c.name == _ifWinUpTrackName).muted = changeGrade != UserUdemae.ChangeGrade.WinUp && changeGrade != UserUdemae.ChangeGrade.BossWinUp;
				((TimelineAsset)_director.playableAsset).GetOutputTracks().First((TrackAsset c) => c.name == _ifWinUpVoiceName).muted = changeGrade != UserUdemae.ChangeGrade.WinUp && changeGrade != UserUdemae.ChangeGrade.BossWinUp;
				((TimelineAsset)_director.playableAsset).GetOutputTracks().First((TrackAsset c) => c.name == _ifLoseDownTrackName).muted = changeGrade != UserUdemae.ChangeGrade.LoseDown;
				((TimelineAsset)_director.playableAsset).GetOutputTracks().First((TrackAsset c) => c.name == _ifLoseDownVoiceName).muted = changeGrade != UserUdemae.ChangeGrade.LoseDown;
				((TimelineAsset)_director.playableAsset).GetOutputTracks().First((TrackAsset c) => c.name == _vsVoiceName).muted = changeGrade != 0 && changeGrade != UserUdemae.ChangeGrade.BossStay;
				int num2 = 1;
				if (type != TrackStartType.BossBattle)
				{
					break;
				}
				foreach (TrackAsset outputTrack in ((TimelineAsset)_director.playableAsset).GetOutputTracks())
				{
					if (outputTrack.name.Contains("Boss_Track"))
					{
						int num3 = int.Parse(outputTrack.name.Remove(0, "Boss_Track_".Length));
						outputTrack.muted = num3 != num2;
					}
				}
				break;
			}
			case TrackStartType.Challenge:
				_director.playableAsset = _normalPlayable;
				_challengeObject.gameObject.transform.parent.gameObject.SetActive(value: true);
				_challengeObject.gameObject.SetActive(value: true);
				_challengeObject.Play("In", 0, 0f);
				_otohime01.Play("TrackStart_In", 0, 0f);
				_otohime02.Play("TrackStart_In", 0, 0f);
				SoundManager.PlaySE(Cue.SE_TRACK_START_DANGER_01, monitorIndex);
				_sub_state = SubMonitorState.ChallengeFadeIn;
				break;
			case TrackStartType.Course:
				_director.playableAsset = _normalPlayable;
				_courseTrackStart.gameObject.SetActive(value: true);
				_courseTrackStart.SetFadeInAnimation(monitorIndex);
				break;
			}
			Animator value = ((type == TrackStartType.BossBattle) ? _bossDerakkumaRoot.GetComponent<Animator>() : _derakkumaRoot.GetComponent<Animator>());
			PlayableBinding playableBinding = _director.playableAsset.outputs.First((PlayableBinding c) => c.streamName == "DerakkumaTrack");
			_director.SetGenericBinding(playableBinding.sourceObject, value);
			_director.Play();
		}

		public bool IsEnd()
		{
			if (_isChallenge)
			{
				SubMonitorState sub_state = _sub_state;
				if (sub_state == SubMonitorState.ChallengeAction && IsEndAnim(_challengeObject))
				{
					_director.Stop();
					return true;
				}
			}
			else if (_isCourse && _courseTrackStart._isEnd)
			{
				_director.Stop();
				return true;
			}
			if (!isPlayerActive)
			{
				return true;
			}
			return _director.time >= _director.duration;
		}

		public override void ViewUpdate()
		{
			_musicName.ViewUpdate();
			_artistName.ViewUpdate();
			_upInfo.UpdateTextScroll();
			_downInfo.UpdateTextScroll();
			_bossUpInfo.UpdateTextScroll();
			_bossDownInfo.UpdateTextScroll();
			if (_isChallenge)
			{
				SubMonitorState sub_state = _sub_state;
				if (sub_state != SubMonitorState.ChallengeFadeIn)
				{
					_ = 2;
				}
				else if (IsEndAnim(_challengeObject))
				{
					_challengeObject.Play(_str, 0, 0f);
					_otohime01.Play("TrackStart_Lv", 0, 0f);
					_otohime02.Play("TrackStart_Lv", 0, 0f);
					_sub_state = SubMonitorState.ChallengeAction;
				}
			}
		}

		public bool IsEndAnim(Animator anim)
		{
			if (anim == null || anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
			{
				return true;
			}
			return false;
		}
	}
}
