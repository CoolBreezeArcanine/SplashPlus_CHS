using System.Collections.Generic;
using Mai2.Mai2Cue;
using MAI2.Util;
using Manager;
using Manager.MaiStudio;
using Manager.UserDatas;
using Monitor.CourseSelect;
using UnityEngine;
using Util;

namespace Process.CourseSelect
{
	public class CourseSelectProcess : ProcessBase
	{
		private enum ProcessState : byte
		{
			Init,
			Wait,
			CourseModeSelect,
			CourseSelect,
			SelectConfirm,
			Release,
			Released
		}

		private enum MoveState
		{
			Back,
			Decide,
			RightMove,
			LeftMove,
			RightCategory,
			LeftCategory,
			None
		}

		public class CourseInfo
		{
			public int courseId = -1;

			public int baseDaniId = -1;

			public int baseCourseId = -1;

			public int courseModeId = -1;

			public int levelUpper;

			public MusicLevelID levelIDUpper;

			public int levelLower;

			public MusicLevelID levelIDLower = MusicLevelID.End;

			public int startLife;

			public int recoverLife;

			public int greatDamage;

			public int goodDamage;

			public int missDamage;

			public int maxAchievement;

			public int maxLife;

			public bool isPlay;

			public CourseCardData cardData;

			public void SetCardData()
			{
				Sprite sprite = Resources.Load<Sprite>("Process/Course/Sprites/UI_DNM_DaniTitle_" + baseDaniId.ToString("00"));
				cardData = new CourseCardData(sprite, courseModeId, levelIDUpper, startLife, recoverLife, greatDamage, goodDamage, missDamage, maxAchievement, maxLife, isPlay);
			}
		}

		public class CourseModeInfo
		{
			public int courseModeId = -1;

			public Color color;

			public CourseModeCardData modeCardData;

			public void SetCardData()
			{
				Sprite sprite = Resources.Load<Sprite>("Process/Course/Sprites/UI_DNM_Selector_Text_" + courseModeId.ToString("00"));
				modeCardData = new CourseModeCardData(sprite, courseModeId, color);
			}
		}

		private const int FIRST_FADEIN_TIME = 1500;

		private const int CATEGORY_IN_TIME = 500;

		private const int LIST_MOVE_TIME = 100;

		private const int CATEGORY_MOVE_TIME = 100;

		private float _timeCounter;

		private CourseSelectMonitor[] _monitors;

		private ProcessState _state;

		private MoveState[] _moveState;

		private bool[] _courseDecide;

		private List<CourseModeInfo> _courseModeList;

		private List<List<CourseInfo>[]> _courseListAll;

		private List<CourseModeCardData> _modeCardDataList;

		private List<List<CourseCardData>[]> _cardDataListAll;

		private int _courseModeIndex;

		private int _courseIndex;

		private int _courseModeIndexForCourseSelectTemp;

		private bool[] _isItemGet;

		public CourseSelectProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public override void OnAddProcess()
		{
		}

		public override void OnRelease()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				Object.Destroy(_monitors[i].gameObject);
			}
		}

		public override void OnStart()
		{
			GameObject prefs = Resources.Load<GameObject>("Process/Course/CourseSelectProcess");
			_monitors = new CourseSelectMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<CourseSelectMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<CourseSelectMonitor>()
			};
			_moveState = new MoveState[_monitors.Length];
			_courseDecide = new bool[_monitors.Length];
			_isItemGet = new bool[_monitors.Length];
			for (int i = 0; i < _monitors.Length; i++)
			{
				_moveState[i] = MoveState.None;
				_courseDecide[i] = false;
				_isItemGet[i] = true;
				SetInputLockInfo(i, 1500f);
			}
			_courseModeList = new List<CourseModeInfo>();
			_courseListAll = new List<List<CourseInfo>[]>();
			_modeCardDataList = new List<CourseModeCardData>();
			_cardDataListAll = new List<List<CourseCardData>[]>();
			GameManager.IsCourseMode = true;
			for (int j = 0; j < _monitors.Length; j++)
			{
				if (Singleton<TicketManager>.Instance.GetCoursePlayTicketId(j) == -1)
				{
					_isItemGet[j] = false;
				}
			}
			SetCourseData();
			for (int k = 0; k < _monitors.Length; k++)
			{
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(k);
				_monitors[k].Initialize(k, userData.IsEntry);
				_monitors[k].SetItemGot(_isItemGet[k]);
				if (!userData.IsEntry)
				{
					continue;
				}
				UserData userData2 = Singleton<UserDataManager>.Instance.GetUserData(k);
				int[] charaSlot = userData2.Detail.CharaSlot;
				List<UserChara> charaList = userData2.CharaList;
				for (int l = 0; l < charaSlot.Length; l++)
				{
					if (charaSlot[l] != 0)
					{
						CharaData charaData = Singleton<DataManager>.Instance.GetChara(charaSlot[l]);
						UserChara userChara = charaList.Find((UserChara a) => a.ID == charaData.GetID());
						if (userChara != null)
						{
							MapColorData mapColorData = Singleton<DataManager>.Instance.GetMapColorData(charaData.color.id);
							Color regionColor = new Color32(mapColorData.Color.R, mapColorData.Color.G, mapColorData.Color.B, byte.MaxValue);
							Texture2D characterTexture2D = container.assetManager.GetCharacterTexture2D(charaData.imageFile);
							MessageCharactorInfomationData messageCharactorInfomationData = new MessageCharactorInfomationData(l, charaData.genre.id, characterTexture2D, userChara.Level, userChara.Awakening, userChara.NextAwakePercent, regionColor);
							container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20021, k, messageCharactorInfomationData));
						}
					}
				}
				UserDetail detail = userData.Detail;
				UserOption option = userData.Option;
				MessageUserInformationData messageUserInformationData = new MessageUserInformationData(k, container.assetManager, detail, option.DispRate, isSubMonitor: true);
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20010, k, true));
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20030, k, true));
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20011, k, messageUserInformationData));
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30008, k, option.SubmonitorAppeal));
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20020, k, true));
			}
			container.processManager.PrepareTimer(99, 0, isEntry: false, null);
			container.processManager.NotificationFadeIn();
		}

		public void SetCourseData()
		{
			List<int> courseList = Singleton<CourseManager>.Instance.GetCourseList();
			Safe.ReadonlySortedDictionary<int, CourseModeData> courseModes = Singleton<DataManager>.Instance.GetCourseModes();
			foreach (int item2 in courseList)
			{
				CourseData course = Singleton<DataManager>.Instance.GetCourse(item2);
				int modeId = course.courseMode.id;
				if (_courseModeList.Find((CourseModeInfo n) => n.courseModeId == modeId) == null)
				{
					foreach (KeyValuePair<int, CourseModeData> item3 in courseModes)
					{
						if (item3.Value.GetID() == modeId)
						{
							_courseListAll.Add(new List<CourseInfo>[2]
							{
								new List<CourseInfo>(),
								new List<CourseInfo>()
							});
							_cardDataListAll.Add(new List<CourseCardData>[2]
							{
								new List<CourseCardData>(),
								new List<CourseCardData>()
							});
							CourseModeInfo courseModeInfo = new CourseModeInfo();
							courseModeInfo.courseModeId = modeId;
							courseModeInfo.SetCardData();
							_courseModeList.Add(courseModeInfo);
							CourseModeCardData item = new CourseModeCardData(Resources.Load<Sprite>("Process/Course/Sprites/UI_DNM_Selector_Text_" + modeId.ToString("00")), modeId, Color.blue);
							_modeCardDataList.Add(item);
							break;
						}
					}
				}
				int index = _courseModeList.FindIndex((CourseModeInfo n) => n.courseModeId == modeId);
				for (int i = 0; i < _monitors.Length; i++)
				{
					CourseInfo courseInfo = new CourseInfo();
					CourseCardData courseCardData = new CourseCardData();
					courseInfo.courseId = course.GetID();
					courseInfo.baseDaniId = course.baseDaniId.id;
					courseInfo.baseCourseId = course.baseCourseId.id;
					courseInfo.courseModeId = modeId;
					courseInfo.levelUpper = course.upperLevel;
					courseInfo.levelLower = course.lowerLevel;
					courseInfo.startLife = course.life;
					courseInfo.recoverLife = course.recover;
					courseInfo.greatDamage = course.greatDamage;
					courseInfo.goodDamage = course.goodDamage;
					courseInfo.missDamage = course.missDamage;
					UserCourse userCourse = Singleton<UserDataManager>.Instance.GetUserData(i).CourseList.Find((UserCourse t) => t.courseId == courseInfo.courseId);
					if (userCourse != null)
					{
						courseInfo.maxAchievement = (int)userCourse.totalAchievement;
						courseInfo.maxLife = (int)userCourse.totalRestlife;
						courseInfo.isPlay = true;
					}
					else
					{
						courseInfo.maxAchievement = 0;
						courseInfo.maxLife = 0;
						courseInfo.isPlay = false;
					}
					courseInfo.SetCardData();
					string text = "Process/Course/Sprites/UI_DNM_DaniTitle_";
					switch (modeId)
					{
					case 1:
					case 2:
						text += courseInfo.baseDaniId.ToString("00");
						foreach (CourseMusicData courseMusicDatum in course.courseMusicData)
						{
							MusicLevelID musicLevelID = (MusicLevelID)Singleton<DataManager>.Instance.GetMusic(courseMusicDatum.musicId.id).notesData[courseMusicDatum.difficulty.id].musicLevelID;
							if (courseInfo.levelIDUpper < musicLevelID)
							{
								courseInfo.levelIDUpper = musicLevelID;
							}
							if (courseInfo.levelIDLower > musicLevelID)
							{
								courseInfo.levelIDLower = musicLevelID;
							}
						}
						break;
					case 3:
					{
						string text2 = "";
						switch (courseInfo.baseCourseId / 100)
						{
						case 0:
							text2 = "BSC";
							break;
						case 1:
							text2 = "ADV";
							break;
						case 2:
							text2 = "EXP";
							break;
						case 3:
							text2 = "MST";
							break;
						case 4:
							text2 = "REM";
							break;
						}
						text = text + text2 + (courseInfo.baseCourseId % 100).ToString("00");
						courseInfo.levelIDUpper = GameManager.GetMusicLevelID(courseInfo.levelUpper);
						courseInfo.levelIDLower = GameManager.GetMusicLevelID(courseInfo.levelLower);
						break;
					}
					}
					courseCardData = new CourseCardData(Resources.Load<Sprite>(text), modeId, courseInfo.levelIDUpper, course.life, course.recover, course.greatDamage, course.goodDamage, course.missDamage, courseInfo.maxAchievement, courseInfo.maxLife, courseInfo.isPlay);
					_courseListAll[index][i].Add(courseInfo);
					_cardDataListAll[index][i].Add(courseCardData);
				}
			}
			int num = -1;
			int num2 = -1;
			for (int j = 0; j < _monitors.Length; j++)
			{
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(j);
				if (userData.IsEntry && !userData.IsGuest())
				{
					num = userData.Detail.LastSelectCourse;
					num2 = j;
					break;
				}
			}
			if (num2 == -1)
			{
				return;
			}
			int num3 = 0;
			bool flag = false;
			foreach (List<CourseInfo>[] item4 in _courseListAll)
			{
				int num4 = 0;
				foreach (CourseInfo item5 in item4[num2])
				{
					if (item5.courseId == num)
					{
						_courseModeIndex = num3;
						_courseIndex = num4;
						flag = true;
						break;
					}
					num4++;
				}
				if (flag)
				{
					break;
				}
				num3++;
			}
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			switch (_state)
			{
			case ProcessState.Init:
			{
				CourseSelectMonitor[] monitors = _monitors;
				foreach (CourseSelectMonitor obj in monitors)
				{
					obj.SetTabData(_modeCardDataList);
					obj.SetCourseModeData(_modeCardDataList);
					obj.UpdateModeData(_courseModeIndex);
					obj.SetFirstFadeIn();
				}
				_courseModeIndexForCourseSelectTemp = _courseModeIndex;
				_state = ProcessState.Wait;
				break;
			}
			case ProcessState.Wait:
				if (_timeCounter >= 300f)
				{
					_timeCounter = 0f;
					for (int num7 = 0; num7 < _monitors.Length; num7++)
					{
						SoundManager.StopBGM(num7);
					}
					SoundManager.PlayBGM(Cue.BGM_DANI_01, 2);
					_state = ProcessState.CourseModeSelect;
				}
				_timeCounter += GameManager.GetGameMSecAdd();
				break;
			case ProcessState.CourseModeSelect:
			{
				MoveState moveState2 = MoveState.None;
				for (int num3 = 0; num3 < _monitors.Length; num3++)
				{
					if (moveState2 > _moveState[num3])
					{
						moveState2 = _moveState[num3];
					}
				}
				switch (moveState2)
				{
				case MoveState.Decide:
				{
					if (_courseModeIndexForCourseSelectTemp != _courseModeIndex)
					{
						_courseIndex = 0;
					}
					for (int num6 = 0; num6 < _monitors.Length; num6++)
					{
						SetInputLockInfo(num6, 500f);
						_monitors[num6].SetCourseData(_cardDataListAll[_courseModeIndex][num6]);
						_monitors[num6].SetModeDecide(_courseModeIndex, _courseIndex);
						_monitors[num6].SetDebugMusicInfoText(GetDebugMusicInfoText());
					}
					_state = ProcessState.CourseSelect;
					break;
				}
				case MoveState.RightMove:
				{
					_courseModeIndex++;
					if (_courseModeIndex >= _courseModeList.Count)
					{
						_courseModeIndex = 0;
					}
					for (int num5 = 0; num5 < _monitors.Length; num5++)
					{
						SetInputLockInfo(num5, 100f);
						_monitors[num5].UpdateModeData(_courseModeIndex);
						_monitors[num5].SetModeRightMove();
					}
					break;
				}
				case MoveState.LeftMove:
				{
					_courseModeIndex--;
					if (_courseModeIndex < 0)
					{
						_courseModeIndex = _courseModeList.Count - 1;
					}
					for (int num4 = 0; num4 < _monitors.Length; num4++)
					{
						SetInputLockInfo(num4, 100f);
						if (_courseModeIndex >= _courseModeList.Count)
						{
							_courseModeIndex = 0;
						}
						_monitors[num4].UpdateModeData(_courseModeIndex);
						_monitors[num4].SetModeLeftMove();
					}
					break;
				}
				}
				break;
			}
			case ProcessState.CourseSelect:
			{
				MoveState moveState = MoveState.None;
				for (int j = 0; j < _monitors.Length; j++)
				{
					if (moveState > _moveState[j])
					{
						moveState = _moveState[j];
					}
				}
				bool flag = false;
				switch (moveState)
				{
				case MoveState.Back:
				{
					for (int num2 = 0; num2 < _monitors.Length; num2++)
					{
						SetInputLockInfo(num2, 500f);
						_monitors[num2].SetCourseBack(_courseModeIndex);
					}
					_courseModeIndexForCourseSelectTemp = _courseModeIndex;
					_state = ProcessState.CourseModeSelect;
					break;
				}
				case MoveState.Decide:
				{
					for (int m = 0; m < _monitors.Length; m++)
					{
						_monitors[m].SetCourseStart();
					}
					container.processManager.SetVisibleTimers(isVisible: false);
					_state = ProcessState.Release;
					break;
				}
				case MoveState.RightMove:
				{
					_courseIndex++;
					if (_courseIndex >= _courseListAll[_courseModeIndex][0].Count)
					{
						_courseModeIndex++;
						if (_courseModeIndex >= _courseListAll.Count)
						{
							_courseModeIndex = 0;
						}
						_courseIndex = 0;
						flag = true;
					}
					for (int l = 0; l < _monitors.Length; l++)
					{
						SetInputLockInfo(l, 100f);
						if (flag)
						{
							_monitors[l].SetCourseData(_cardDataListAll[_courseModeIndex][l]);
							_monitors[l].UpdateCourseData(_courseModeIndex, _courseIndex);
							_monitors[l].SetCourseRightCategoryMoveByMoveButton();
						}
						else
						{
							_monitors[l].UpdateCourseData(_courseModeIndex, _courseIndex);
							_monitors[l].SetCourseRightMove();
						}
						_monitors[l].SetDebugMusicInfoText(GetDebugMusicInfoText());
					}
					break;
				}
				case MoveState.LeftMove:
				{
					_courseIndex--;
					if (_courseIndex < 0)
					{
						_courseModeIndex--;
						if (_courseModeIndex < 0)
						{
							_courseModeIndex = _courseListAll.Count - 1;
						}
						_courseIndex = _courseListAll[_courseModeIndex][0].Count - 1;
						flag = true;
					}
					for (int n = 0; n < _monitors.Length; n++)
					{
						SetInputLockInfo(n, 100f);
						if (flag)
						{
							_monitors[n].SetCourseData(_cardDataListAll[_courseModeIndex][n]);
							_monitors[n].UpdateCourseData(_courseModeIndex, _courseIndex);
							_monitors[n].SetCourseLeftCategoryMoveByMoveButton();
						}
						else
						{
							_monitors[n].UpdateCourseData(_courseModeIndex, _courseIndex);
							_monitors[n].SetCourseLeftMove();
						}
						_monitors[n].SetDebugMusicInfoText(GetDebugMusicInfoText());
					}
					break;
				}
				case MoveState.RightCategory:
				{
					_courseModeIndex++;
					if (_courseModeIndex >= _courseListAll.Count)
					{
						_courseModeIndex = 0;
					}
					_courseIndex = 0;
					for (int num = 0; num < _monitors.Length; num++)
					{
						SetInputLockInfo(num, 100f);
						_monitors[num].SetCourseData(_cardDataListAll[_courseModeIndex][num]);
						_monitors[num].UpdateCourseData(_courseModeIndex, _courseIndex);
						_monitors[num].SetCourseRightCategoryMoveByCategoryButton();
						_monitors[num].SetDebugMusicInfoText(GetDebugMusicInfoText());
					}
					break;
				}
				case MoveState.LeftCategory:
				{
					_courseModeIndex--;
					if (_courseModeIndex < 0)
					{
						_courseModeIndex = _courseListAll.Count - 1;
					}
					_courseIndex = _courseListAll[_courseModeIndex][0].Count - 1;
					for (int k = 0; k < _monitors.Length; k++)
					{
						SetInputLockInfo(k, 100f);
						_monitors[k].SetCourseData(_cardDataListAll[_courseModeIndex][k]);
						_monitors[k].UpdateCourseData(_courseModeIndex, _courseIndex);
						_monitors[k].SetCourseLeftCategoryMoveByCategoryButton();
						_monitors[k].SetDebugMusicInfoText(GetDebugMusicInfoText());
					}
					break;
				}
				}
				break;
			}
			case ProcessState.SelectConfirm:
			{
				bool flag2 = false;
				for (int num9 = 0; num9 < _monitors.Length; num9++)
				{
					if (!_courseDecide[num9])
					{
						switch (_moveState[num9])
						{
						case MoveState.Back:
							flag2 = true;
							break;
						case MoveState.Decide:
							_courseDecide[num9] = true;
							_monitors[num9].SetCourseConfirmWait();
							break;
						}
					}
				}
				if (flag2)
				{
					for (int num10 = 0; num10 < _monitors.Length; num10++)
					{
						if (_courseDecide[num10])
						{
							_courseDecide[num10] = false;
							_monitors[num10].SetCourseConfirmWaitBack();
						}
						_monitors[num10].SetCourseConfirmBack();
					}
					_state = ProcessState.CourseSelect;
					break;
				}
				bool flag3 = true;
				for (int num11 = 0; num11 < _monitors.Length; num11++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(num11).IsEntry && !_courseDecide[num11])
					{
						flag3 = false;
					}
				}
				if (flag3)
				{
					for (int num12 = 0; num12 < _monitors.Length; num12++)
					{
						_monitors[num12].SetCourseStart();
					}
					container.processManager.SetVisibleTimers(isVisible: false);
					_state = ProcessState.Release;
				}
				break;
			}
			case ProcessState.Release:
				if (_timeCounter >= 500f)
				{
					_timeCounter = 0f;
					int courseId = _courseListAll[_courseModeIndex][0][_courseIndex].courseId;
					Singleton<CourseManager>.Instance.SetCourseId(courseId);
					for (int i = 0; i < _monitors.Length; i++)
					{
						Singleton<UserDataManager>.Instance.GetUserData(i).Detail.LastSelectCourse = courseId;
					}
					_state = ProcessState.Released;
					container.processManager.AddProcess(new MusicSelectProcess(container), 50);
					container.processManager.ReleaseProcess(this);
				}
				_timeCounter += GameManager.GetGameMSecAdd();
				break;
			}
			for (int num13 = 0; num13 < _monitors.Length; num13++)
			{
				_monitors[num13].ViewUpdate();
			}
			for (int num14 = 0; num14 < _monitors.Length; num14++)
			{
				_moveState[num14] = MoveState.None;
			}
		}

		protected override void UpdateInput(int monitorId)
		{
			_moveState[monitorId] = MoveState.None;
			if (IsInputLock(monitorId) || !Singleton<UserDataManager>.Instance.GetUserData(monitorId).IsEntry)
			{
				return;
			}
			bool flag = container.processManager.IsTimeUp(monitorId);
			switch (_state)
			{
			case ProcessState.CourseModeSelect:
				if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button04) || flag)
				{
					_moveState[monitorId] = MoveState.Decide;
				}
				else if (!IsModeSelectRightLimit() && (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3) || InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L)))
				{
					_moveState[monitorId] = MoveState.RightMove;
				}
				else if (!IsModeSelectLeftLimit() && (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6) || InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L)))
				{
					_moveState[monitorId] = MoveState.LeftMove;
				}
				break;
			case ProcessState.CourseSelect:
				if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button05))
				{
					_moveState[monitorId] = MoveState.Back;
				}
				else if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button04) || flag)
				{
					_moveState[monitorId] = MoveState.Decide;
				}
				else if (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3) || InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L))
				{
					_moveState[monitorId] = MoveState.RightMove;
				}
				else if (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6) || InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L))
				{
					_moveState[monitorId] = MoveState.LeftMove;
				}
				else if (InputManager.GetTouchPanelAreaDown(monitorId, InputManager.TouchPanelArea.E2))
				{
					_moveState[monitorId] = MoveState.RightCategory;
				}
				else if (InputManager.GetTouchPanelAreaDown(monitorId, InputManager.TouchPanelArea.E8))
				{
					_moveState[monitorId] = MoveState.LeftCategory;
				}
				break;
			case ProcessState.SelectConfirm:
				if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button05))
				{
					_moveState[monitorId] = MoveState.Back;
				}
				else if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button04) || flag)
				{
					_moveState[monitorId] = MoveState.Decide;
				}
				break;
			case ProcessState.Init:
			case ProcessState.Wait:
				break;
			}
		}

		public override void OnLateUpdate()
		{
		}

		private bool IsModeSelectLeftLimit()
		{
			if (_courseModeIndex - 1 < 0)
			{
				return true;
			}
			return false;
		}

		private bool IsModeSelectRightLimit()
		{
			if (_courseModeIndex + 1 >= _courseListAll.Count)
			{
				return true;
			}
			return false;
		}

		private string GetDebugMusicInfoText()
		{
			string text = "";
			int courseId = _courseListAll[_courseModeIndex][0][_courseIndex].courseId;
			for (int i = 0; i < 4; i++)
			{
				int musicId = Singleton<CourseManager>.Instance.GetMusicId(courseId, i);
				int difficultyId = Singleton<CourseManager>.Instance.GetDifficultyId(courseId, i);
				if (Singleton<DataManager>.Instance.GetMusics().ContainsKey(musicId))
				{
					MusicData music = Singleton<DataManager>.Instance.GetMusic(musicId);
					string str = music.name.str;
					int level = music.notesData[difficultyId].level;
					int levelDecimal = music.notesData[difficultyId].levelDecimal;
					string text2 = ((musicId >= 10000) ? "[DX]" : "[STD]");
					string text3 = "";
					switch (difficultyId + 1)
					{
					case 1:
						text3 = "6FC943";
						break;
					case 2:
						text3 = "F8B709";
						break;
					case 3:
						text3 = "FE838B";
						break;
					case 4:
						text3 = "9F51DC";
						break;
					case 5:
						text3 = "BA67F8";
						break;
					}
					text += "[Track ";
					text += i + 1;
					text += "] ";
					text += " ";
					text = text + "level : " + level + "." + levelDecimal;
					text += " ";
					text = text + "<color=#" + text3 + ">";
					text += str;
					text += " ";
					text += text2;
					text += "</color>";
					text += "\n";
				}
			}
			return text;
		}
	}
}
