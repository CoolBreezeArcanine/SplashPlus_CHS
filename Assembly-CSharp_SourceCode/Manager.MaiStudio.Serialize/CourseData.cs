using System;
using System.Collections.Generic;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class CourseData : SerializeBase, ISerialize
	{
		public StringID name;

		public StringID courseMode;

		public StringID baseDaniId;

		public StringID baseCourseId;

		public string fileName;

		public bool isRandom;

		public int upperLevel;

		public int lowerLevel;

		public bool isLock;

		public StringID conditionsUnlockCourse;

		public List<CourseMusicData> courseMusicData;

		public int life;

		public int recover;

		public int perfectDamage;

		public int greatDamage;

		public int goodDamage;

		public int missDamage;

		public StringID eventId;

		public StringID netOpenName;

		public string dataName;

		public CourseData()
		{
			name = new StringID();
			courseMode = new StringID();
			baseDaniId = new StringID();
			baseCourseId = new StringID();
			fileName = "";
			isRandom = false;
			upperLevel = 0;
			lowerLevel = 0;
			isLock = false;
			conditionsUnlockCourse = new StringID();
			courseMusicData = new List<CourseMusicData>();
			life = 0;
			recover = 0;
			perfectDamage = 0;
			greatDamage = 0;
			goodDamage = 0;
			missDamage = 0;
			eventId = new StringID();
			netOpenName = new StringID();
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.CourseData(CourseData sz)
		{
			Manager.MaiStudio.CourseData courseData = new Manager.MaiStudio.CourseData();
			courseData.Init(sz);
			return courseData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
			courseMode.AddPath(parentPath);
			baseDaniId.AddPath(parentPath);
			baseCourseId.AddPath(parentPath);
			conditionsUnlockCourse.AddPath(parentPath);
			foreach (CourseMusicData courseMusicDatum in courseMusicData)
			{
				courseMusicDatum.AddPath(parentPath);
			}
			eventId.AddPath(parentPath);
			netOpenName.AddPath(parentPath);
		}

		public int GetID()
		{
			return name.id;
		}

		public void SetPriority(int pri)
		{
		}

		public bool IsDisable()
		{
			return false;
		}
	}
}
