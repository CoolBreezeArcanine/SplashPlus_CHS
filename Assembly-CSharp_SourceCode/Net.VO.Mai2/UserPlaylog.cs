using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public struct UserPlaylog
	{
		public ulong userId;

		public int orderId;

		public ulong playlogId;

		public int version;

		public int placeId;

		public string placeName;

		public long loginDate;

		public string playDate;

		public string userPlayDate;

		public int type;

		public int musicId;

		public int level;

		public int trackNo;

		public int vsMode;

		public string vsUserName;

		public int vsStatus;

		public int vsUserRating;

		public int vsUserAchievement;

		public int vsUserGradeRank;

		public int vsRank;

		public int playerNum;

		public ulong playedUserId1;

		public string playedUserName1;

		public int playedMusicLevel1;

		public ulong playedUserId2;

		public string playedUserName2;

		public int playedMusicLevel2;

		public ulong playedUserId3;

		public string playedUserName3;

		public int playedMusicLevel3;

		public int characterId1;

		public int characterLevel1;

		public int characterAwakening1;

		public int characterId2;

		public int characterLevel2;

		public int characterAwakening2;

		public int characterId3;

		public int characterLevel3;

		public int characterAwakening3;

		public int characterId4;

		public int characterLevel4;

		public int characterAwakening4;

		public int characterId5;

		public int characterLevel5;

		public int characterAwakening5;

		public int achievement;

		public int deluxscore;

		public int scoreRank;

		public int maxCombo;

		public int totalCombo;

		public int maxSync;

		public int totalSync;

		public int tapCriticalPerfect;

		public int tapPerfect;

		public int tapGreat;

		public int tapGood;

		public int tapMiss;

		public int holdCriticalPerfect;

		public int holdPerfect;

		public int holdGreat;

		public int holdGood;

		public int holdMiss;

		public int slideCriticalPerfect;

		public int slidePerfect;

		public int slideGreat;

		public int slideGood;

		public int slideMiss;

		public int touchCriticalPerfect;

		public int touchPerfect;

		public int touchGreat;

		public int touchGood;

		public int touchMiss;

		public int breakCriticalPerfect;

		public int breakPerfect;

		public int breakGreat;

		public int breakGood;

		public int breakMiss;

		public bool isTap;

		public bool isHold;

		public bool isSlide;

		public bool isTouch;

		public bool isBreak;

		public bool isCriticalDisp;

		public bool isFastLateDisp;

		public int fastCount;

		public int lateCount;

		public bool isAchieveNewRecord;

		public bool isDeluxscoreNewRecord;

		public int comboStatus;

		public int syncStatus;

		public bool isClear;

		public int beforeRating;

		public int afterRating;

		public int beforeGrade;

		public int afterGrade;

		public int afterGradeRank;

		public int beforeDeluxRating;

		public int afterDeluxRating;

		public bool isPlayTutorial;

		public bool isEventMode;

		public bool isFreedomMode;

		public int playMode;

		public bool isNewFree;

		public int extNum1;

		public int extNum2;
	}
}
