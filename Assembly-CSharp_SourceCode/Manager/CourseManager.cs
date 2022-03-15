using System.Collections.Generic;
using System.Linq;
using DB;
using MAI2.Util;
using Manager.MaiStudio;
using Manager.UserDatas;

namespace Manager
{
	public class CourseManager : Singleton<CourseManager>
	{
		public enum Mode
		{
			Normal = 1,
			Sin,
			Random
		}

		public enum LifeColor
		{
			Green,
			Orange,
			Red,
			Gray
		}

		public enum ImageType
		{
			Title,
			Plate
		}

		private int _selectCourseId;

		private int[] _restLifes;

		private int[] _beforeRecoverLifes;

		private int[,] _musicRestLife;

		private Dictionary<int, List<int>> _scoreIdList;

		public CourseManager()
		{
			_selectCourseId = -1;
			_restLifes = new int[2];
			_beforeRecoverLifes = new int[2];
			_musicRestLife = new int[2, 4];
			for (int i = 0; i < _restLifes.Length; i++)
			{
				_restLifes[i] = -1;
				_beforeRecoverLifes[i] = -1;
				for (int j = 0; (long)j < 4L; j++)
				{
					_musicRestLife[i, j] = 0;
				}
			}
			_scoreIdList = new Dictionary<int, List<int>>();
		}

		public void Initialize()
		{
			_selectCourseId = -1;
			for (int i = 0; i < _restLifes.Length; i++)
			{
				_restLifes[i] = -1;
				_beforeRecoverLifes[i] = -1;
				for (int j = 0; (long)j < 4L; j++)
				{
					_musicRestLife[i, j] = 0;
				}
			}
			_scoreIdList.Clear();
		}

		public List<int> GetCourseList()
		{
			Singleton<NotesListManager>.Instance.CreateScore();
			IEnumerable<int> enumerable = from t in Singleton<DataManager>.Instance.GetCourses()
				where Singleton<EventManager>.Instance.IsOpenEvent(t.Value.eventId.id)
				select t.Key;
			List<int> list = new List<int>();
			foreach (int item in enumerable)
			{
				CourseData course = Singleton<DataManager>.Instance.GetCourse(item);
				if (course == null)
				{
					continue;
				}
				if (!course.isLock)
				{
					if (SetMusicList(course.GetID()))
					{
						list.Add(course.GetID());
					}
					continue;
				}
				bool flag = false;
				int needUnlockCoutseID = course.conditionsUnlockCourse.id;
				for (int i = 0; i < 2; i++)
				{
					UserCourse userCourse = Singleton<UserDataManager>.Instance.GetUserData(i).CourseList.Find((UserCourse t) => t.courseId == needUnlockCoutseID);
					if (userCourse != null && userCourse.totalRestlife != 0)
					{
						flag = true;
					}
				}
				if (flag && SetMusicList(course.GetID()))
				{
					list.Add(course.GetID());
				}
			}
			return list;
		}

		public bool FirstSinRankAdd(int playerId)
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(playerId);
			if (userData.Detail.ContentBit.IsFlagOn(ContentBitID.FirstSinCourseAdd))
			{
				return false;
			}
			foreach (int item in from t in Singleton<DataManager>.Instance.GetCourses()
				where Singleton<EventManager>.Instance.IsOpenEvent(t.Value.eventId.id)
				select t.Key)
			{
				CourseData course = Singleton<DataManager>.Instance.GetCourse(item);
				if (course != null && course.courseMode.id == 2)
				{
					int needUnlockCoutseID = course.conditionsUnlockCourse.id;
					UserCourse userCourse = userData.CourseList.Find((UserCourse t) => t.courseId == needUnlockCoutseID);
					if (userCourse != null && userCourse.totalRestlife != 0)
					{
						userData.Detail.ContentBit.SetFlag(ContentBitID.FirstSinCourseAdd, flag: true);
						return true;
					}
				}
			}
			return false;
		}

		public bool IsCourseAdd(int playerId)
		{
			IEnumerable<int> enumerable = from t in Singleton<DataManager>.Instance.GetCourses()
				where Singleton<EventManager>.Instance.IsOpenEvent(t.Value.eventId.id)
				select t.Key;
			int num = 0;
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(playerId);
			foreach (int item in enumerable)
			{
				CourseData course = Singleton<DataManager>.Instance.GetCourse(item);
				if (course == null)
				{
					continue;
				}
				if (!course.isLock)
				{
					num++;
					continue;
				}
				int needUnlockCoutseID = course.conditionsUnlockCourse.id;
				UserCourse userCourse = userData.CourseList.Find((UserCourse t) => t.courseId == needUnlockCoutseID);
				if (userCourse != null && userCourse.totalRestlife != 0)
				{
					num++;
				}
			}
			int lastCountCourse = userData.Detail.LastCountCourse;
			if (lastCountCourse == 0)
			{
				userData.Detail.LastCountCourse = num;
				return false;
			}
			if (lastCountCourse < num)
			{
				userData.Detail.LastCountCourse = num;
				return true;
			}
			if (lastCountCourse > num)
			{
				userData.Detail.LastCountCourse = num;
			}
			return false;
		}

		private bool SetMusicList(int courseId)
		{
			CourseData course = Singleton<DataManager>.Instance.GetCourse(courseId);
			if (course != null)
			{
				List<int> list = new List<int>();
				if (course.isRandom)
				{
					int count = course.courseMusicData.Count;
					int lowerLevel = course.lowerLevel;
					int upperLevel = course.upperLevel;
					MusicDifficultyID id = (MusicDifficultyID)course.courseMusicData[0].difficulty.id;
					List<int> randomMusic = Singleton<NotesListManager>.Instance.GetRandomMusic(lowerLevel, upperLevel, id, count);
					if (randomMusic.Count == 0)
					{
						return false;
					}
					list = randomMusic;
				}
				else
				{
					foreach (CourseMusicData courseMusicDatum in course.courseMusicData)
					{
						int item = courseMusicDatum.musicId.id * 100 + courseMusicDatum.difficulty.id;
						list.Add(item);
					}
				}
				_scoreIdList.Add(courseId, list);
				return true;
			}
			return false;
		}

		public void SetCourseId(int courseId)
		{
			CourseData course = Singleton<DataManager>.Instance.GetCourse(courseId);
			if (course == null)
			{
				return;
			}
			_selectCourseId = courseId;
			for (int i = 0; i < _restLifes.Length; i++)
			{
				if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry)
				{
					_restLifes[i] = course.life;
					_beforeRecoverLifes[i] = course.life;
				}
			}
		}

		public int GetNextMusicId()
		{
			return GetMusicId(_selectCourseId, (int)(GameManager.MusicTrackNumber - 1));
		}

		public int GetNextDifficultyId()
		{
			return GetDifficultyId(_selectCourseId, (int)(GameManager.MusicTrackNumber - 1));
		}

		public int GetMusicId(int track)
		{
			return GetMusicId(_selectCourseId, track);
		}

		public int GetMusicId(int courseId, int track)
		{
			if (_scoreIdList.ContainsKey(courseId) && track < _scoreIdList[courseId].Count)
			{
				return _scoreIdList[courseId][track] / 100;
			}
			return -1;
		}

		public int GetDifficultyId(int track)
		{
			return GetDifficultyId(_selectCourseId, track);
		}

		public int GetDifficultyId(int courseId, int track)
		{
			if (_scoreIdList.ContainsKey(courseId) && track < _scoreIdList[courseId].Count)
			{
				return _scoreIdList[courseId][track] % 100;
			}
			return -1;
		}

		public int GetRestLife(int playerIndex)
		{
			return _restLifes[playerIndex];
		}

		public int GetBeforeRestLife(int playerIndex)
		{
			return _beforeRecoverLifes[playerIndex];
		}

		private void SetDecreaseLife(int playerIndex, int decreaseLife, int trackNo = -1)
		{
			if (decreaseLife >= _restLifes[playerIndex])
			{
				_restLifes[playerIndex] = 0;
			}
			else
			{
				_restLifes[playerIndex] -= decreaseLife;
			}
			if (trackNo < 0)
			{
				trackNo = (int)GameManager.MusicTrackNumber;
			}
			_beforeRecoverLifes[playerIndex] = _restLifes[playerIndex];
			_musicRestLife[playerIndex, trackNo - 1] = _restLifes[playerIndex];
			bool flag = GameManager.MusicTrackNumber >= 4;
			if (_restLifes[playerIndex] <= 0 || flag)
			{
				return;
			}
			CourseData course = Singleton<DataManager>.Instance.GetCourse(_selectCourseId);
			if (course != null)
			{
				_restLifes[playerIndex] += course.recover;
				if (_restLifes[playerIndex] > course.life)
				{
					_restLifes[playerIndex] = course.life;
				}
			}
		}

		public void SetLifeCalc(int playerIndex, GameScoreList scoreList, int trackNo = -1)
		{
			CourseData courseData = GetCourseData();
			if (courseData != null)
			{
				int num = 0;
				num += (int)scoreList.TruePerfectNum * courseData.perfectDamage;
				num += (int)scoreList.GreatNum * courseData.greatDamage;
				num += (int)scoreList.GoodNum * courseData.goodDamage;
				num += (int)scoreList.MissNum * courseData.missDamage;
				SetDecreaseLife(playerIndex, num, trackNo);
			}
		}

		public void SetLifeCalc(int playerIndex, int truePerfectNum, int greatNum, int goodNum, int missNum, int trackNo = -1)
		{
			CourseData courseData = GetCourseData();
			if (courseData != null)
			{
				int num = 0;
				num += truePerfectNum * courseData.perfectDamage;
				num += greatNum * courseData.greatDamage;
				num += goodNum * courseData.goodDamage;
				num += missNum * courseData.missDamage;
				SetDecreaseLife(playerIndex, num, trackNo);
			}
		}

		public bool IsGameOver()
		{
			if (GameManager.MusicTrackNumber >= 4)
			{
				return true;
			}
			if (IsForceFail())
			{
				bool result = true;
				for (int i = 0; i < _restLifes.Length; i++)
				{
					if (_restLifes[i] > 0)
					{
						result = false;
					}
				}
				return result;
			}
			return false;
		}

		public bool IsForceFail()
		{
			CourseData courseData = GetCourseData();
			if (courseData != null)
			{
				int id = courseData.courseMode.id;
				CourseModeData courseMode = Singleton<DataManager>.Instance.GetCourseMode(id);
				if (courseMode != null)
				{
					return courseMode.isForceFail;
				}
			}
			return false;
		}

		public CourseData GetCourseData()
		{
			return Singleton<DataManager>.Instance.GetCourse(_selectCourseId);
		}

		public string GetCourseImage(ImageType type)
		{
			string text = "";
			if (GetCourseData() != null)
			{
				CourseData courseData = GetCourseData();
				int id = courseData.courseMode.id;
				switch (type)
				{
				case ImageType.Title:
					text = "Process/Course/Sprites/UI_DNM_DaniTitle_";
					break;
				case ImageType.Plate:
					text = "Process/Course/Sprites/DaniPlate/UI_DNM_DaniPlate_";
					break;
				}
				switch (id)
				{
				case 1:
				case 2:
					text += courseData.baseDaniId.id.ToString("00");
					break;
				case 3:
				{
					string text2 = "";
					switch (courseData.baseCourseId.id / 100)
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
					text = text + text2 + (courseData.baseCourseId.id % 100).ToString("00");
					break;
				}
				}
			}
			return text;
		}

		public int GetTrackLife(int playerIndex, int trackNo)
		{
			return _musicRestLife[playerIndex, trackNo];
		}

		public LifeColor GetLifeColor(uint life)
		{
			uint num = 999u;
			if (GetCourseData() != null)
			{
				num = (uint)GetCourseData().life;
			}
			if (num == life)
			{
				return LifeColor.Green;
			}
			switch (life)
			{
			default:
				return LifeColor.Green;
			case 10u:
			case 11u:
			case 12u:
			case 13u:
			case 14u:
			case 15u:
			case 16u:
			case 17u:
			case 18u:
			case 19u:
			case 20u:
			case 21u:
			case 22u:
			case 23u:
			case 24u:
			case 25u:
			case 26u:
			case 27u:
			case 28u:
			case 29u:
			case 30u:
			case 31u:
			case 32u:
			case 33u:
			case 34u:
			case 35u:
			case 36u:
			case 37u:
			case 38u:
			case 39u:
			case 40u:
			case 41u:
			case 42u:
			case 43u:
			case 44u:
			case 45u:
			case 46u:
			case 47u:
			case 48u:
			case 49u:
			case 50u:
			case 51u:
			case 52u:
			case 53u:
			case 54u:
			case 55u:
			case 56u:
			case 57u:
			case 58u:
			case 59u:
			case 60u:
			case 61u:
			case 62u:
			case 63u:
			case 64u:
			case 65u:
			case 66u:
			case 67u:
			case 68u:
			case 69u:
			case 70u:
			case 71u:
			case 72u:
			case 73u:
			case 74u:
			case 75u:
			case 76u:
			case 77u:
			case 78u:
			case 79u:
			case 80u:
			case 81u:
			case 82u:
			case 83u:
			case 84u:
			case 85u:
			case 86u:
			case 87u:
			case 88u:
			case 89u:
			case 90u:
			case 91u:
			case 92u:
			case 93u:
			case 94u:
			case 95u:
			case 96u:
			case 97u:
			case 98u:
			case 99u:
				return LifeColor.Orange;
			case 1u:
			case 2u:
			case 3u:
			case 4u:
			case 5u:
			case 6u:
			case 7u:
			case 8u:
			case 9u:
				return LifeColor.Red;
			case 0u:
				return LifeColor.Gray;
			}
		}
	}
}
