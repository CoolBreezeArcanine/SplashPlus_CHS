using System.Collections.Generic;
using System.Collections.ObjectModel;
using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class CourseData : AccessorBase
	{
		public StringID name { get; private set; }

		public StringID courseMode { get; private set; }

		public StringID baseDaniId { get; private set; }

		public StringID baseCourseId { get; private set; }

		public string fileName { get; private set; }

		public bool isRandom { get; private set; }

		public int upperLevel { get; private set; }

		public int lowerLevel { get; private set; }

		public bool isLock { get; private set; }

		public StringID conditionsUnlockCourse { get; private set; }

		public ReadOnlyCollection<CourseMusicData> courseMusicData { get; private set; }

		public int life { get; private set; }

		public int recover { get; private set; }

		public int perfectDamage { get; private set; }

		public int greatDamage { get; private set; }

		public int goodDamage { get; private set; }

		public int missDamage { get; private set; }

		public StringID eventId { get; private set; }

		public StringID netOpenName { get; private set; }

		public string dataName { get; private set; }

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
			courseMusicData = new ReadOnlyCollection<CourseMusicData>(new List<CourseMusicData>());
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

		public void Init(Manager.MaiStudio.Serialize.CourseData sz)
		{
			name = (StringID)sz.name;
			courseMode = (StringID)sz.courseMode;
			baseDaniId = (StringID)sz.baseDaniId;
			baseCourseId = (StringID)sz.baseCourseId;
			fileName = sz.fileName;
			isRandom = sz.isRandom;
			upperLevel = sz.upperLevel;
			lowerLevel = sz.lowerLevel;
			isLock = sz.isLock;
			conditionsUnlockCourse = (StringID)sz.conditionsUnlockCourse;
			List<CourseMusicData> list = new List<CourseMusicData>();
			foreach (Manager.MaiStudio.Serialize.CourseMusicData courseMusicDatum in sz.courseMusicData)
			{
				list.Add((CourseMusicData)courseMusicDatum);
			}
			courseMusicData = new ReadOnlyCollection<CourseMusicData>(list);
			life = sz.life;
			recover = sz.recover;
			perfectDamage = sz.perfectDamage;
			greatDamage = sz.greatDamage;
			goodDamage = sz.goodDamage;
			missDamage = sz.missDamage;
			eventId = (StringID)sz.eventId;
			netOpenName = (StringID)sz.netOpenName;
			dataName = sz.dataName;
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
