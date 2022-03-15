using System.Collections.Generic;
using System.Linq;

namespace Manager.UserDatas
{
	public class UserActivity
	{
		private const int PlayActivityMax = 15;

		private const int MusicActivityMax = 10;

		public List<UserAct> PlayList { get; set; } = new List<UserAct>();


		public List<UserAct> MusicList { get; set; } = new List<UserAct>();


		public UserActivity()
		{
			Clear();
		}

		public void Clear()
		{
			PlayList.Clear();
			MusicList.Clear();
		}

		public void PlayMaimaiDx()
		{
			UserAct.ActivityCode actCode = UserAct.ActivityCode.PlayDX;
			if (!PlayList.Any((UserAct x) => x.id == (int)actCode && !TimeManager.IsAnotherLogDay(x.sortNumber, TimeManager.GetNowUnixTime())))
			{
				AddPlayResult(actCode);
			}
		}

		public void ClassUp(UdemaeID classID)
		{
			UserAct.ActivityCode actCode = UserAct.ActivityCode.ClassUp;
			bool flag = false;
			if (!PlayList.Where((UserAct x) => x.id == (int)actCode).Any() || 0 < PlayList.RemoveAll((UserAct x) => x.id == (int)actCode && x.param1 < (int)classID))
			{
				AddPlayResult(actCode, (int)classID);
			}
		}

		public void RankUp(int rankID)
		{
			UserAct.ActivityCode actCode = UserAct.ActivityCode.RankUp;
			bool flag = false;
			if (!PlayList.Where((UserAct x) => x.id == (int)actCode).Any() || 0 < PlayList.RemoveAll((UserAct x) => x.id == (int)actCode && x.param1 < rankID))
			{
				AddPlayResult(actCode, rankID);
			}
		}

		public void DxRate(int dxRate)
		{
			UserAct.ActivityCode actCode = UserAct.ActivityCode.DxRate;
			PlayList.RemoveAll((UserAct x) => x.id == (int)actCode);
			AddPlayResult(actCode, dxRate);
		}

		public void AwakeChara(int charaId)
		{
			UpsertActivity(UserAct.ActivityCode.Awake, charaId);
		}

		public void MapComplete(int mapId)
		{
			UpsertActivity(UserAct.ActivityCode.MapComplete, mapId);
		}

		public void MapFound(int mapId)
		{
			UpsertActivity(UserAct.ActivityCode.MapFound, mapId);
		}

		public void TransmissionMusicGet(int music_id)
		{
			UpsertActivity(UserAct.ActivityCode.TransmissionMusic, music_id);
		}

		public void TaskMusicClear(int music_id, int level)
		{
			UpsertActivity(UserAct.ActivityCode.TaskMusicClear, music_id, level);
		}

		public void ChallengeMusicClear(int music_id, int level)
		{
			UpsertActivity(UserAct.ActivityCode.ChallengeMusicClear, music_id, level);
		}

		public void MusicAchivement(int musicId, int difficulty, UserAct.ActivityCode code, int level)
		{
			if (!(from x in PlayList
				where GetMusicResultCode(x.id) == GetMusicResultCode((int)code)
				where !TimeManager.IsAnotherLogDay(x.sortNumber, TimeManager.GetNowUnixTime())
				select x).Any())
			{
				AddPlayResult(code, musicId, difficulty, level);
			}
			else if (0 < PlayList.RemoveAll((UserAct x) => GetMusicResultCode(x.id) == GetMusicResultCode((int)code) && !TimeManager.IsAnotherLogDay(x.sortNumber, TimeManager.GetNowUnixTime()) && (x.id < (int)code || (x.id == (int)code && x.param3 < level))))
			{
				AddPlayResult(code, musicId, difficulty, level);
			}
		}

		private void AddPlayResult(UserAct.ActivityCode code, int param1 = 0, int param2 = 0, int param3 = 0, int param4 = 0)
		{
			PlayList.Add(new UserAct(1, (int)code, TimeManager.GetNowUnixTime(), param1, param2, param3, param4));
			TrimPlayActivityList();
		}

		public void PlayMusic(int musicId)
		{
			MusicList.RemoveAll((UserAct c) => c.id == musicId);
			MusicList.Add(new UserAct(2, musicId, TimeManager.GetNowUnixTime(), 0, 0, 0, 0));
			TrimMusicActivityList();
		}

		private bool IsPlayActivityAnotherDay(int code)
		{
			foreach (UserAct item in PlayList.Where((UserAct x) => x.id == code))
			{
				if (!TimeManager.IsAnotherLogDay(item.sortNumber, TimeManager.GetNowUnixTime()))
				{
					return false;
				}
			}
			return true;
		}

		private UserAct.MusicResult GetMusicResultCode(int code)
		{
			switch (code)
			{
			case 20:
			case 21:
			case 22:
			case 23:
			case 24:
			case 25:
				return UserAct.MusicResult.ScoreRank;
			case 30:
			case 31:
			case 32:
			case 33:
				return UserAct.MusicResult.FullCombo;
			case 40:
			case 41:
			case 42:
			case 43:
				return UserAct.MusicResult.FullSync;
			default:
				return UserAct.MusicResult.None;
			}
		}

		private void TrimPlayActivityList()
		{
			if (PlayList.Count > 15)
			{
				PlayList.RemoveRange(0, PlayList.Count - 15);
			}
		}

		private void TrimMusicActivityList()
		{
			if (MusicList.Count > 10)
			{
				MusicList.RemoveRange(0, MusicList.Count - 10);
			}
		}

		private void UpsertActivity(UserAct.ActivityCode code, int param1 = 0, int param2 = 0, int param3 = 0, int param4 = 0)
		{
			PlayList.RemoveAll((UserAct x) => x.id == (int)code && !TimeManager.IsAnotherLogDay(x.sortNumber, TimeManager.GetNowUnixTime()));
			AddPlayResult(code, param1, param2, param3, param4);
		}
	}
}
