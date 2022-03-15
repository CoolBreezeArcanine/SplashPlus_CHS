using System.Collections.Generic;
using System.Linq;
using MAI2.Util;
using Manager.MaiStudio;
using Net.VO.Mai2;

namespace Manager
{
	public class ScoreRankingManager : Singleton<ScoreRankingManager>
	{
		private Dictionary<int, ScoreRankingForMusicSeq> enableRankings = new Dictionary<int, ScoreRankingForMusicSeq>();

		private List<ScoreRankingForAdvertiseSeq> enableAdvertise = new List<ScoreRankingForAdvertiseSeq>();

		public Dictionary<int, ScoreRankingForMusicSeq> ScoreRankings => enableRankings;

		public List<ScoreRankingForAdvertiseSeq> Advertise => enableAdvertise;

		public void UpdateData()
		{
			enableRankings.Clear();
			enableAdvertise.Clear();
			GameTournamentInfo[] gameTournamentInfoDataList = Singleton<OperationManager>.Instance.GetGameTournamentInfoDataList();
			long playBaseTime = TimeManager.PlayBaseTime;
			_ = from t in Singleton<DataManager>.Instance.GetScoreRankings()
				where Singleton<EventManager>.Instance.IsOpenEvent(t.Value.eventName.id)
				select t into x
				select x.Key;
			GameTournamentInfo[] array = gameTournamentInfoDataList;
			for (int i = 0; i < array.Length; i++)
			{
				GameTournamentInfo gameTournamentInfo = array[i];
				if (TimeManager.GetUnixTime(gameTournamentInfo.startDate) > playBaseTime || playBaseTime > TimeManager.GetUnixTime(gameTournamentInfo.endDate))
				{
					continue;
				}
				int tournamentId = gameTournamentInfo.tournamentId;
				ScoreRankingData scoreRanking = Singleton<DataManager>.Instance.GetScoreRanking(tournamentId);
				if (scoreRanking == null)
				{
					continue;
				}
				if (gameTournamentInfo.rankingKind == 0 || gameTournamentInfo.rankingKind == 1)
				{
					ScoreRankingForMusicSeq scoreRankingForMusicSeq = new ScoreRankingForMusicSeq(tournamentId);
					if (scoreRankingForMusicSeq != null)
					{
						scoreRankingForMusicSeq.FileName = scoreRanking.FileName;
						scoreRankingForMusicSeq.GenreColor = scoreRanking.Color;
						scoreRankingForMusicSeq.GenreName = scoreRanking.genreNameTwoLine;
						scoreRankingForMusicSeq.MusicInfoList.Clear();
						GameTournamentMusic[] gameTournamentMusicList = gameTournamentInfo.gameTournamentMusicList;
						for (int j = 0; j < gameTournamentMusicList.Length; j++)
						{
							GameTournamentMusic gameTournamentMusic = gameTournamentMusicList[j];
							ScoreRankingMusicInfo item = default(ScoreRankingMusicInfo);
							item.MusicID = gameTournamentMusic.musicId;
							item.IsLock = gameTournamentMusic.isFirstLock;
							scoreRankingForMusicSeq.MusicInfoList.Add(item);
						}
						enableRankings.Add(scoreRanking.GetID(), scoreRankingForMusicSeq);
					}
				}
				if (gameTournamentInfo.rankingKind == 0 || gameTournamentInfo.rankingKind == 2)
				{
					ScoreRankingForAdvertiseSeq item2 = default(ScoreRankingForAdvertiseSeq);
					item2.Id = tournamentId;
					item2.FileName = scoreRanking.FileName;
					enableAdvertise.Add(item2);
				}
			}
		}

		public ScoreRankingForMusicSeq getSrDataForMS(int rankingID)
		{
			if (enableRankings.ContainsKey(rankingID))
			{
				return enableRankings[rankingID];
			}
			return null;
		}

		public List<int> GetUnlockMusicList(int monitorId)
		{
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, ScoreRankingForMusicSeq> enableRanking in enableRankings)
			{
				List<int> list2 = new List<int>();
				bool flag = true;
				foreach (ScoreRankingMusicInfo musicInfo in enableRanking.Value.MusicInfoList)
				{
					if (musicInfo.IsLock)
					{
						list2.Add(musicInfo.MusicID);
						continue;
					}
					uint maxTrackCount = GameManager.GetMaxTrackCount();
					bool flag2 = false;
					for (int i = 0; i < maxTrackCount; i++)
					{
						if (musicInfo.MusicID == Singleton<GamePlayManager>.Instance.GetGameScore(monitorId, i).SessionInfo.musicId)
						{
							flag2 = true;
							break;
						}
					}
					if (flag2)
					{
						continue;
					}
					flag = false;
					break;
				}
				if (!flag)
				{
					continue;
				}
				foreach (int item in list2)
				{
					list.Add(item);
				}
			}
			return list;
		}
	}
}
